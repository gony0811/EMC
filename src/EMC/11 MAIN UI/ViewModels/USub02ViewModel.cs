﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EMC.DB;
using EPFramework.IoC;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace EMC
{
    [ViewModel(Lifetime.Singleton)]
    public partial class USub02ViewModel : ObservableObject
    {
        private readonly UnitRepository _unitRepo;
        private readonly ValueTypeRepository _valueTypeRepo;
        private readonly RecipeRepository _recipeRepo;
        private readonly ParameterRepository _parameterRepo;

        // 목록들
        [ObservableProperty] private ObservableCollection<Recipe> recipes = new ObservableCollection<Recipe>();
        [ObservableProperty] private ObservableCollection<RecipeParam> recipeParam = new ObservableCollection<RecipeParam>();

        // 선택/상태
        [ObservableProperty] private Recipe selectedRecipe;
        [ObservableProperty] private bool isBusy;

        // (필요 시) 기타 UI 상태
        [ObservableProperty] private string currentDevice;
        [ObservableProperty] private bool parameterType = true; // False: 공용, True: 기본
        [ObservableProperty] private ObservableCollection<DeviceItem> devices = new ObservableCollection<DeviceItem>();
        [ObservableProperty] private DeviceItem selectedDevice;
        [ObservableProperty] private DeviceItem activeDevice;
        [ObservableProperty] private ObservableCollection<ParameterModel> items = new ObservableCollection<ParameterModel>();

        // 룩업(다이얼로그에 전달 용)
        public List<Unit> Units { get; } = new List<Unit>();
        public List<ValueTypeDef> ValueTypes { get; } = new List<ValueTypeDef>();

        public USub02ViewModel(
              UnitRepository unitRepository,
              ValueTypeRepository valueTypeRepository,
               RecipeRepository recipeRepository,
               ParameterRepository parameterRepository)
        {
            _unitRepo = unitRepository;
            _valueTypeRepo = valueTypeRepository;
            _recipeRepo = recipeRepository;
            _parameterRepo = parameterRepository;
            _ = InitializeAsync();
        }
        private async Task InitializeAsync(CancellationToken ct = default(CancellationToken))
        {
            try
            {
                IsBusy = true;

                // 룩업/레시피 병렬 로딩
                await Task.WhenAll(GetUnits(ct), GetValueTypes(ct), GetAllRecipe(ct)).ConfigureAwait(false);

                // 초기 선택 레시피의 파라미터 로드
                if (SelectedRecipe != null)
                    await LoadRecipeParamsAsync(SelectedRecipe, ct).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetBaseException().Message, "초기화 오류");
            }
            finally { IsBusy = false; }
        }

        // SelectedRecipe 바뀌면 자동 로딩
        partial void OnSelectedRecipeChanged(Recipe value)
        {
            if (value == null) return;
            _ = LoadRecipeParamsAsync(value);
        }

        // 레시피 목록 로딩
        [RelayCommand]
        public async Task GetAllRecipe(CancellationToken ct = default(CancellationToken))
        {
            try
            {
                var list = await _recipeRepo.ListAsync(
                    orderBy: q => q.OrderBy(r => r.Id),
                    asNoTracking: true,
                    ct: ct).ConfigureAwait(false);

                var prevId = SelectedRecipe != null ? (int?)SelectedRecipe.Id : null;
                var active = list.FirstOrDefault(r => r.IsActive);
                var keepPrev = prevId.HasValue ? list.FirstOrDefault(r => r.Id == prevId.Value) : null;
                var toSelect = active ?? keepPrev ?? list.FirstOrDefault();

                Recipes.Clear();
                foreach (var r in list) Recipes.Add(r);

                if (!object.ReferenceEquals(SelectedRecipe, toSelect))
                    SelectedRecipe = toSelect;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.GetBaseException().Message, "레시피 로드 오류");
            }
        }

        [RelayCommand]
        public async Task ClickRecipe(Recipe recipe)
        {
            if (recipe == null) return;
            await LoadRecipeParamsAsync(recipe).ConfigureAwait(false);
        }
        private bool CanClickRecipe(Recipe recipe) => recipe != null && !IsBusy;

        private async Task LoadRecipeParamsAsync(Recipe recipe, CancellationToken ct = default(CancellationToken))
        {
            try
            {
                IsBusy = true;
                RecipeParam.Clear();
                var ps = await _recipeRepo.FindAsync(ct, recipe.Id).ConfigureAwait(false);
                foreach (var p in ps.Params) RecipeParam.Add(p);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.GetBaseException().Message, "파라미터 로드 오류");
            }
            finally { IsBusy = false; }
        }

        // ── 생성 ─────────────────────────────────────────────────────────────────

        [RelayCommand]
        public async Task CreateRecipe(CancellationToken ct = default(CancellationToken))
        {
            var initial = new Recipe();
            var result = await ShowRecipeDialogAsync(initial, "레시피 생성").ConfigureAwait(false);
            if (result == null) return;

            try
            {
                await _recipeRepo.AddAsync(result, ct).ConfigureAwait(false);
                await _recipeRepo.SaveAsync(ct).ConfigureAwait(false);
                AlertModal.Ask(GetOwnerWindow(), "저장", "저장되었습니다.");
                await GetAllRecipe(ct).ConfigureAwait(false);
            }
            catch (DbUpdateException)
            {
                MessageBox.Show("저장 중 오류가 발생했습니다.", "저장 오류");
            }
        }

        // ㅡㅡ 복사  ㅡㅡㅡ 
        [RelayCommand]
        public async Task CopyRecipe(Recipe recipe, CancellationToken ct = default(CancellationToken))
        {
            if (recipe == null) return;

            var initial = new Recipe { Id = recipe.Id };

            var result = await ShowRecipeDialogAsync(initial, "레시피 복사").ConfigureAwait(false);
            if (result == null) return;

            try
            {
                await _recipeRepo.CloneAsync(initial.Id, newName: result.Name, ct: ct).ConfigureAwait(false);
                AlertModal.Ask(GetOwnerWindow(), "저장", "저장되었습니다.");
                await GetAllRecipe(ct).ConfigureAwait(false);
            }
            catch (DbUpdateException)
            {
                AlertModal.Ask(GetOwnerWindow(), "중복 오류", "이미 존재하는 이름이 있습니다. 다시 입력해주세요");
            }
        }

        [RelayCommand]
        public async Task DeleteRecipe(Recipe recipe)
        {
            if (recipe == null) return;

            var owner = GetOwnerWindow();

            // 사용중 레시피는 금지
            if (recipe.IsActive)
            {
                AlertModal.Ask(owner, "경고", "사용중인 레시피는 삭제하실 수 없습니다.");
                return;
            }

            // 확인 팝업 결과 체크
            var yes = ConfirmModal.Ask(owner, "삭제", "'" + recipe.Name + "' 레시피를 삭제하시겠습니까?");
            if (!yes) return;

            IsBusy = true;
            try
            {
                var ok = await _recipeRepo.Remove(recipe.Id).ConfigureAwait(false);
                if (!ok)
                {
                    AlertModal.Ask(owner, "안내", "이미 삭제되었거나 존재하지 않는 레시피입니다.");
                    return;
                }

                await _recipeRepo.SaveAsync().ConfigureAwait(false);

                AlertModal.Ask(owner, "완료", "삭제되었습니다.");

                // 목록/선택/파라미터 갱신
                await GetAllRecipe().ConfigureAwait(false);
                if (SelectedRecipe != null)
                    await LoadRecipeParamsAsync(SelectedRecipe).ConfigureAwait(false);
                else
                    RecipeParam.Clear();
            }
            catch (DbUpdateException ex)
            {
                AlertModal.Ask(owner, "삭제 실패",
                    "해당 레시피에 연결된 파라미터가 있어 삭제할 수 없습니다.\n" +
                    "파라미터를 먼저 삭제하거나 관계 설정을 확인하세요.\n\n" +
                    ex.GetBaseException().Message);
            }
            catch (Exception ex)
            {
                AlertModal.Ask(owner, "오류", ex.GetBaseException().Message);
            }
            finally
            {
                IsBusy = false;
                DeleteRecipeCommand.NotifyCanExecuteChanged();
            }
        }

        private bool CanDeleteRecipe(Recipe recipe) => !IsBusy && recipe != null;

        [RelayCommand]
        public async Task CreateParameter(Recipe recipe)
        {
            if (recipe == null) return;

            var initial = new RecipeParam();
            var result = await ShowParameterDialogAsync(initial, "파라미터 생성").ConfigureAwait(false);
            if (result == null) return;

            try
            {
                NormalizeFK(result, recipeId: recipe.Id);
                await _parameterRepo.AddAsync(result).ConfigureAwait(false);
                await _parameterRepo.SaveAsync().ConfigureAwait(false);
                AlertModal.Ask(GetOwnerWindow(), "저장", "저장되었습니다.");
                await LoadRecipeParamsAsync(recipe).ConfigureAwait(false);
            }
            catch (DbUpdateException)
            {
                MessageBox.Show("저장 중 오류가 발생했습니다.", "저장 오류");
            }
        }

        [RelayCommand]
        public async Task UpdateParameter(RecipeParam param)
        {
            if (param == null) return;

            var copy = new RecipeParam
            {
                Id = param.Id,
                RecipeId = param.RecipeId,
                Name = param.Name,
                Value = param.Value,
                Maximum = param.Maximum,
                Minimum = param.Minimum,
                ValueTypeId = param.ValueTypeId,
                UnitId = param.UnitId,
                Description = param.Description,
                ValueType = param.ValueType,
                Unit = param.Unit
            };

            var result = await ShowParameterDialogAsync(copy, "파라미터 수정").ConfigureAwait(false);
            if (result == null) return;

            try
            {
                NormalizeFK(result, recipeId: param.RecipeId);
                _parameterRepo.Update(result);
                await _parameterRepo.SaveAsync().ConfigureAwait(false);
                AlertModal.Ask(GetOwnerWindow(), "저장", "저장되었습니다.");

                if (SelectedRecipe != null)
                    await LoadRecipeParamsAsync(SelectedRecipe).ConfigureAwait(false);
            }
            catch (DbUpdateException)
            {
                MessageBox.Show("저장 중 오류가 발생했습니다.", "저장 오류");
            }
        }

        // ── 활성 레시피 변경 ─────────────────────────────────────────────────────

        [RelayCommand]
        private async Task ChangeUseRecipeAsync(Recipe recipe, CancellationToken ct)
        {
            if (recipe == null || recipe.IsActive) return;

            var owner = GetOwnerWindow();
            if (!ConfirmModal.Ask(owner, "변경", "사용할 레시피를 변경하시겠습니까?"))
                return;

            IsBusy = true;
            try
            {
                await _recipeRepo.SetActiveAsync(recipe.Id, ct).ConfigureAwait(false);
                await GetAllRecipe(ct).ConfigureAwait(false);
            }
            catch (DbUpdateException)
            {
                AlertModal.Ask(owner, "제약 위반", "활성 레시피는 1개만 사용할 수 있어요.\n잠시 후 다시 시도하세요.");
            }
            catch (OperationCanceledException) { /* 무시 */ }
            catch (Exception ex)
            {
                AlertModal.Ask(owner, "에러", ex.GetBaseException().Message);
            }
            finally
            {
                IsBusy = false;
                ChangeUseRecipeCommand.NotifyCanExecuteChanged();
            }
        }
        private bool CanChangeUseRecipe(Recipe recipe)
            => !IsBusy && recipe != null && !recipe.IsActive;

        // ── 룩업 로딩 ───────────────────────────────────────────────────────────

        private async Task GetUnits(CancellationToken ct = default(CancellationToken))
        {
            var units = await _unitRepo.ListAsync(ct: ct).ConfigureAwait(false);
            Units.Clear();
            Units.AddRange(units);
        }

        private async Task GetValueTypes(CancellationToken ct = default(CancellationToken))
        {
            var vts = await _valueTypeRepo.ListAsync(ct: ct).ConfigureAwait(false);
            ValueTypes.Clear();
            ValueTypes.AddRange(vts);
        }

        // ── 다이얼로그 헬퍼 ─────────────────────────────────────────────────────

        private async Task<RecipeParam> ShowParameterDialogAsync(RecipeParam initial, string title)
        {
            var vm = new ParameterCreateVM(
                title: title,
                initial: initial,
                units: Units,
                valueTypes: ValueTypes);

            var win = new ParameterCreateWindow
            {
                Owner = GetOwnerWindow(),
                DataContext = vm
            };

            var ok = win.ShowDialog() == true;
            return ok ? vm.Value : null;
        }

        private async Task<Recipe> ShowRecipeDialogAsync(Recipe initial, string title)
        {
            var vm = new RecipeCreateVM(
                title: title,
                initial: initial);

            var win = new RecipeCreateWindow
            {
                Owner = GetOwnerWindow(),
                DataContext = vm
            };

            var ok = win.ShowDialog() == true;
            return ok ? vm.Value : null;
        }

        /// FK만 확정하고 네비게이션은 비움(트래킹 문제 방지)
        private static void NormalizeFK(RecipeParam p, int? recipeId = null)
        {
            if (recipeId.HasValue) p.RecipeId = recipeId.Value;

            if (p.ValueTypeId <= 0 && p.ValueType != null)
                p.ValueTypeId = p.ValueType.Id;
            if (p.UnitId == null && p.Unit != null)
                p.UnitId = p.Unit.Id;

            p.Recipe = null;
            p.ValueType = null;
            p.Unit = null;

            if (string.IsNullOrWhiteSpace(p.Name))
                throw new ValidationException("이름을 입력하세요.");
            if (string.IsNullOrWhiteSpace(p.Value))
                throw new ValidationException("값을 입력하세요.");
            if (p.ValueTypeId <= 0)
                throw new ValidationException("값 타입을 선택하세요.");
        }

        private static Window GetOwnerWindow()
        {
            var w = Application.Current != null
                ? Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive) ?? Application.Current.MainWindow
                : null;
            return w;
        }
    }
}
