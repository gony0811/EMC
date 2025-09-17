using Autofac;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EGGPLANT.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace EGGPLANT.ViewModels
{
    public partial class USub02ViewModel : ObservableObject
    {
        private readonly IDbRepository<Recipe> _recipeRepo;
        private readonly IDbRepository<RecipeParam> _paramRepo;
        private readonly IDbRepository<Unit> _unitRepo;
        private readonly IDbRepository<ValueTypeDef> _valueTypeRepo;
        private readonly RecipeService _recipeService;
        private readonly ParameterService _parameterService;

        // 목록들
        [ObservableProperty] private ObservableCollection<Recipe> recipes = new();
        [ObservableProperty] private ObservableCollection<RecipeParam> recipeParam = new();

        // 선택/상태
        [ObservableProperty] private Recipe? selectedRecipe;
        [ObservableProperty] private bool isBusy;

        // (필요 시) 기타 UI 상태
        [ObservableProperty] private string? currentDevice;
        [ObservableProperty] private bool parameterType = true; // False: 공용, True: 기본
        [ObservableProperty] private ObservableCollection<DeviceItem> devices = new();
        [ObservableProperty] private DeviceItem? selectedDevice;
        [ObservableProperty] private DeviceItem? activeDevice;
        [ObservableProperty] private ObservableCollection<ParameterModel> items = new();

        // 룩업(다이얼로그에 전달 용)
        public List<Unit> Units { get; } = new();
        public List<ValueTypeDef> ValueTypes { get; } = new();

        public USub02ViewModel(
            IDbRepository<Recipe> repo,
            IDbRepository<RecipeParam> paramRepo,
            IDbRepository<Unit> unitRepo,
            IDbRepository<ValueTypeDef> valueTypeRepo,
            RecipeService recipeService,
            ParameterService parameterService)
        {
            _recipeRepo       = repo;
            _paramRepo        = paramRepo;
            _unitRepo         = unitRepo;
            _valueTypeRepo    = valueTypeRepo;
            _recipeService    = recipeService;
            _parameterService = parameterService;

            _ = InitializeAsync();
        }

        private async Task InitializeAsync(CancellationToken ct = default)
        {
            try
            {
                IsBusy = true;

                // 룩업/레시피 병렬 로딩
                await Task.WhenAll(GetUnits(ct), GetValueTypes(ct), GetAllRecipe(ct));

                // 초기 선택 레시피의 파라미터 로드
                if (SelectedRecipe is not null)
                    await LoadRecipeParamsAsync(SelectedRecipe, ct);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetBaseException().Message, "초기화 오류");
            }
            finally { IsBusy = false; }
        }

        // SelectedRecipe 바뀌면 자동 로딩
        partial void OnSelectedRecipeChanged(Recipe? value)
        {
            if (value is null) return;
            _ = LoadRecipeParamsAsync(value);
        }

        // 레시피 목록 로딩
        [RelayCommand]
        public async Task GetAllRecipe(CancellationToken ct = default)
        {
            try
            {
                var list = await _recipeRepo.ListAsync(
                    orderBy: q => q.OrderBy(r => r.Id),
                    asNoTracking: true,
                    ct: ct);

                var prevId  = SelectedRecipe?.Id;
                var active  = list.FirstOrDefault(r => r.IsActive);
                var keepPrev = list.FirstOrDefault(r => r.Id == prevId);
                var toSelect = active ?? keepPrev ?? list.FirstOrDefault();

                Recipes.Clear();
                foreach (var r in list) Recipes.Add(r);

                if (!ReferenceEquals(SelectedRecipe, toSelect))
                    SelectedRecipe = toSelect;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.GetBaseException().Message, "레시피 로드 오류");
            }
        }

        // (선택) DataGrid 단일 클릭 바인딩용
        [RelayCommand(CanExecute = nameof(CanClickRecipe))]
        public async Task ClickRecipe(Recipe? recipe)
        {
            if (recipe is null) return;
            await LoadRecipeParamsAsync(recipe);
        }
        private bool CanClickRecipe(Recipe? recipe) => recipe is not null && !IsBusy;

        private async Task LoadRecipeParamsAsync(Recipe recipe, CancellationToken ct = default)
        {
            try
            {
                IsBusy = true;
                RecipeParam.Clear();
                var ps = await _parameterService.GetParametersAsync(recipe.Id, ct);
                foreach (var p in ps) RecipeParam.Add(p);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.GetBaseException().Message, "파라미터 로드 오류");
            }
            finally { IsBusy = false; }
        }

        // ── 생성 ─────────────────────────────────────────────────────────────────

        [RelayCommand]
        public async Task CreateRecipe(CancellationToken ct = default)
        {

            var initial = new Recipe();
            var result = await ShowRecipeDialogAsync(initial, "레시피 생성");
            if (result is null) return;

            try
            {
                await _recipeRepo.AddAsync(result);
                await _recipeRepo.SaveAsync();
                AlertModal.Ask(GetOwnerWindow(), "저장", "저장되었습니다.");
                await GetAllRecipe(ct);
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show(ex.GetBaseException().Message, "저장 오류");
            }
        }

        // ㅡㅡ 복사  ㅡㅡㅡ 
        [RelayCommand]
        public async Task CopyRecipe(Recipe recipe, CancellationToken ct = default)
        {
            Recipe initial = new Recipe();
            initial.Id = recipe.Id;

            var result = await ShowRecipeDialogAsync(initial, "레시피 복사");
            if (result is null) return;

            try
            {
                await _recipeService.CloneAsync(initial.Id, newName: result.Name);
                AlertModal.Ask(GetOwnerWindow(), "저장", "저장되었습니다.");
                await GetAllRecipe(ct);
            }
            catch (DbUpdateException ex)
            {
                AlertModal.Ask(GetOwnerWindow(), "중복 오류", "이미 존재하는 이름이 있습니다. 다시 입력해주세요");
            }
        }

        [RelayCommand(CanExecute = nameof(CanDeleteRecipe))]
        public async Task DeleteRecipe(Recipe? recipe)
        {
            if (recipe is null) return;

            var owner = GetOwnerWindow();

            // 사용중 레시피는 금지
            if (recipe.IsActive)
            {
                AlertModal.Ask(owner, "경고", "사용중인 레시피는 삭제하실 수 없습니다.");
                return;
            }

            // 확인 팝업 결과 체크
            var yes = ConfirmModal.Ask(owner, "삭제", $"'{recipe.Name}' 레시피를 삭제하시겠습니까?");
            if (!yes) return;

            IsBusy = true;
            try
            {
                // 삭제 시도 (리포지토리 시그니처에 맞춰 호출)
                var ok = await _recipeRepo.Remove(recipe.Id);   // <- RemoveByIdAsync(bool 반환) 가정
                if (!ok)
                {
                    AlertModal.Ask(owner, "안내", "이미 삭제되었거나 존재하지 않는 레시피입니다.");
                    return;
                }

                await _recipeRepo.SaveAsync();

                AlertModal.Ask(owner, "완료", "삭제되었습니다.");

                // 목록/선택/파라미터 갱신
                await GetAllRecipe();
                if (SelectedRecipe is not null)
                    await LoadRecipeParamsAsync(SelectedRecipe);   // 선택된 레시피의 파라미터 다시 로드
                else
                    RecipeParam.Clear();                           // 더 이상 선택이 없으면 비움
            }
            catch (DbUpdateException ex)
            {
                // 대부분 FK 제약(연결된 RecipeParam 존재)일 가능성
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

        private bool CanDeleteRecipe(Recipe? recipe) => !IsBusy && recipe is not null;

        [RelayCommand]
        public async Task CreateParameter(Recipe recipe)
        {
            var initial = new RecipeParam();
            var result = await ShowParameterDialogAsync(initial, "파라미터 생성");
            if (result is null) return;

            try
            {
                NormalizeFK(result, recipeId: recipe.Id);
                await _paramRepo.AddAsync(result);
                await _paramRepo.SaveAsync();
                AlertModal.Ask(GetOwnerWindow(), "저장", "저장되었습니다.");
                await LoadRecipeParamsAsync(recipe);
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show(ex.GetBaseException().Message, "저장 오류");
            }
        }

        [RelayCommand]
        public async Task UpdateParameter(RecipeParam? param)
        {
            if(param is null)
            {
                return;
            }
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
                ValueType = param.ValueType, // 편집용 뷰모델이 SelectedItem을 쓰는 경우 편의를 위해 유지
                Unit = param.Unit
            };

            var result = await ShowParameterDialogAsync(copy, "파라미터 수정");
            if (result is null) return;

            try
            {
                NormalizeFK(result, recipeId: param.RecipeId);
                _paramRepo.Update(result);
                await _paramRepo.SaveAsync();
                AlertModal.Ask(GetOwnerWindow(), "저장", "저장되었습니다.");

                // 목록 갱신 (선택 레시피 기준)
                if (SelectedRecipe is not null)
                    await LoadRecipeParamsAsync(SelectedRecipe);
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show(ex.GetBaseException().Message, "저장 오류");
            }
        }

        // ── 활성 레시피 변경 ─────────────────────────────────────────────────────

        [RelayCommand(IncludeCancelCommand = true, CanExecute = nameof(CanChangeUseRecipe))]
        private async Task ChangeUseRecipeAsync(Recipe? recipe, CancellationToken ct)
        {
            if (recipe is null || recipe.IsActive) return;

            var owner = GetOwnerWindow();
            if (!ConfirmModal.Ask(owner, "변경", "사용할 레시피를 변경하시겠습니까?"))
                return;

            IsBusy = true;
            try
            {
                await _recipeService.SetActiveAsync(recipe.Id, ct);
                await GetAllRecipe(ct);
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
        private bool CanChangeUseRecipe(Recipe? recipe)
            => !IsBusy && recipe is not null && !recipe.IsActive;

        // ── 룩업 로딩 ───────────────────────────────────────────────────────────

        private async Task GetUnits(CancellationToken ct = default)
        {
            var units = await _unitRepo.ListAsync(ct: ct);
            Units.Clear();
            Units.AddRange(units);
        }

        private async Task GetValueTypes(CancellationToken ct = default)
        {
            var vts = await _valueTypeRepo.ListAsync(ct: ct);
            ValueTypes.Clear();
            ValueTypes.AddRange(vts);
        }

        // ── 다이얼로그 헬퍼 ─────────────────────────────────────────────────────

        private async Task<RecipeParam?> ShowParameterDialogAsync(RecipeParam initial, string title)
        {
            var vm = new ParameterCreateVM(
                title: title,
                initial: initial,
                units: Units,           // 이미 로딩된 목록 전달
                valueTypes: ValueTypes);

            var win = new ParameterCreateWindow
            {
                Owner = GetOwnerWindow(),
                DataContext = vm
            };

            // OK 시 vm.Value 반환
            var ok = win.ShowDialog() == true;
            return ok ? vm.Value : null;
        }

        private async Task<Recipe?> ShowRecipeDialogAsync(Recipe initial, string title)
        {
            var vm = new RecipeCreateVM(
                title: title,
                initial: initial);

            var win = new RecipeCreateWindow
            {
                Owner = GetOwnerWindow(),
                DataContext = vm
            };

            // OK 시 vm.Value 반환
            var ok = win.ShowDialog() == true;
            return ok ? vm.Value : null;
        }

        /// FK만 확정하고 네비게이션은 비움(트래킹 문제 방지)
        private static void NormalizeFK(RecipeParam p, int? recipeId = null)
        {
            if (recipeId.HasValue) p.RecipeId = recipeId.Value;

            // SelectedItem 바인딩을 쓰는 경우를 대비해 Id 확정
            if (p.ValueTypeId <= 0 && p.ValueType is not null)
                p.ValueTypeId = p.ValueType.Id;
            if (p.UnitId is null && p.Unit is not null)
                p.UnitId = p.Unit.Id;

            // 네비게이션 비움 (EF가 새 엔터티 Insert 시도 방지)
            p.Recipe = null;
            p.ValueType = null;
            p.Unit = null;

            // 기본 검증(실패는 곧바로 예외 → 호출부에서 잡음)
            if (string.IsNullOrWhiteSpace(p.Name))
                throw new ValidationException("이름을 입력하세요.");
            if (string.IsNullOrWhiteSpace(p.Value))
                throw new ValidationException("값을 입력하세요.");
            if (p.ValueTypeId <= 0)
                throw new ValidationException("값 타입을 선택하세요.");
        }

        private static Window? GetOwnerWindow() =>
            Application.Current?.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive)
            ?? Application.Current?.MainWindow;
    }
}
