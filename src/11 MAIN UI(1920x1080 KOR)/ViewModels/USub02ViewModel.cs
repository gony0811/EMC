
using Autofac;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ControlzEx.Standard;
using EGGPLANT;
using System.Collections.ObjectModel;
using System.Windows;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace EGGPLANT.ViewModels
{
    public partial class USub02ViewModel : ObservableObject
    {
        private IRecipeService _recipeService;
        private CommonData _commonData;

        // 현재 디바이스, 공용 파라미터, 기본 파라미터 선택, 디바이스 목록, 아이템 이름, 
        [ObservableProperty]
        private string currentRecipe;

        // 공용 & 기본 파라미터 선택
        [ObservableProperty]
        private bool parameterType = true; // False: 공용, True: 기본 파라미터

        [ObservableProperty]
        private ObservableCollection<RecipeVM> recipes = new ObservableCollection<RecipeVM>();

        [ObservableProperty]
        private RecipeVM selectedRecipe;


        [ObservableProperty]
        private ObservableCollection<ParameterVM> parameters = new ObservableCollection<ParameterVM>();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public USub02ViewModel(IRecipeService recipeService, CommonData commonData)
        {
            _recipeService = recipeService;
            _commonData = commonData;
            InitRecipe();
            
        }

        [RelayCommand]
        public async void CreateRecipe()
        {
            var vm = App.Container.Resolve<RecipeCreateViewModel>();

            var win = App.Container.Resolve<RecipeCreateWindow>(
                new TypedParameter(typeof(RecipeCreateViewModel), vm));

            var owner = Application.Current?.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.IsActive) ?? Application.Current?.MainWindow;
            win.Owner = owner;

            var result = win.ShowDialog();
            if (result == true && vm.Result is not null)
            {
                await _recipeService.CreateRecipe(vm.Result);
                InitRecipe();
            }
        }

        [RelayCommand]
        public async void CopyRecipe(RecipeVM item)
        {
            var vm = App.Container.Resolve<RecipeCreateViewModel>(new NamedParameter("recipeId", item.RecipeId));
            var win = App.Container.Resolve<RecipeCreateWindow>(new TypedParameter(typeof(RecipeCreateViewModel), vm));

            var owner = Application.Current?.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.IsActive) ?? Application.Current?.MainWindow;
            win.Owner = owner;

            var result = win.ShowDialog();

            // 저장 성공이면 화면 갱신
            if (result == true && vm.Result is not null)
            {
                await _recipeService.CopyRecipeAsync(item.RecipeId, vm.Name, vm.IsActive);
                InitRecipe();
            }
        }



        [RelayCommand]
        public void SelectRecipe(RecipeVM item)
        {
            if (item is null) return;
            // 단일 클릭 처리
            SelectedRecipe = item;
            CurrentRecipe = item.Name;
            InitRecipeParams(SelectedRecipe);
        }

        [RelayCommand]
        public void UseRecipe(RecipeVM item)
        {
            var owner = Application.Current?.Windows
               .OfType<Window>()
               .FirstOrDefault(w => w.IsActive) ?? Application.Current?.MainWindow;

            if (ConfirmModal.Ask(owner, "레시피 변경", "레시피를 변경하시겠습니까?") == true)
            {
                _recipeService.UseRecipe(item.RecipeId);
                InitRecipe();
            }
        }


        [RelayCommand]
        public async void CreateParameter()
        {
            if (SelectedRecipe is null) return;

            var id = SelectedRecipe.RecipeId;

            // VM 먼저 생성(레시피 ID 주입)
            var vm = App.Container.Resolve<ParameterCreateVM>(
                new NamedParameter("recipeId", id));

            // 그 VM으로 창 생성
            var win = App.Container.Resolve<ParameterCreateWindow>(
                new TypedParameter(typeof(ParameterCreateVM), vm));

            // 소유자 지정
            var owner = Application.Current?.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.IsActive) ?? Application.Current?.MainWindow;
            win.Owner = owner;

            // 모달 오픈 (창이 내부에서 RequestClose 처리)
            var result = win.ShowDialog();

            // 저장 성공이면 화면 갱신
            if (result == true && vm.Result is not null)
            {
                await _recipeService.CreateParamter(vm.Result);
                InitRecipeParams(SelectedRecipe);
            }
        }

        [RelayCommand]
        public async void UpdateParameter(ParameterVM parameter)
        {
            if (SelectedRecipe is null) return;

            var id = SelectedRecipe.RecipeId;

            // VM 먼저 생성(레시피 ID 주입)
            var vm = App.Container.Resolve<ParameterCreateVM>(
                new NamedParameter("recipeId", id));

            vm.ParameterId = parameter.ParameterId;
            vm.Name = parameter.Name;
            vm.Value = parameter.Value;
            vm.Minimum = parameter.Minimum;
            vm.Maximum = parameter.Maximum;
            vm.SelectedUnit = _commonData.FindUnit(parameter.Unit);
            vm.SelectedValueType = _commonData.FindValueType(parameter.ValueType);
            // 그 VM으로 창 생성
            ParameterCreateWindow win = App.Container.Resolve<ParameterCreateWindow>(
                new TypedParameter(typeof(ParameterCreateVM), vm));

            // 소유자 지정
            var owner = Application.Current?.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.IsActive) ?? Application.Current?.MainWindow;
            win.Owner = owner;

            // 모달 오픈 (창이 내부에서 RequestClose 처리)
            var result = win.ShowDialog();

            // 저장 성공이면 화면 갱신
            if (result == true && vm.Result is not null)
            {
                await _recipeService.UpdateParamter(vm.Result);
                InitRecipeParams(SelectedRecipe);
            }
        }

        [RelayCommand]
        public void DeleteRecipe(RecipeVM item)
        {
            // 창을 DI로 꺼내와서 모달 표시
            var owner = Application.Current?.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.IsActive) ?? Application.Current?.MainWindow;
            if (item.IsActive)
            {
                AlertModal.Ask(owner, "삭제 불가", "활성 레시피는 삭제할 수 없습니다.");
                return;
            }

            if (ConfirmModal.Ask(owner, "삭제", $"{item.Name} 레시피를 정말로 삭제하시겠습니까?") == true)
            {
                _recipeService.DeleteRecipe(item.RecipeId);
                InitRecipe();
            }
        }

        




        // 레시피 목록 갱신
        private async void InitRecipe()
        {
            var list = await _recipeService.GetRecipes();

            Recipes.Clear();

            RecipeVM? activeVm = null;

            foreach (var dto in list)
            {
                var vm = RecipeVM.of(dto);  // 한 번만 생성
                Recipes.Add(vm);

                if (dto.IsActive)
                    activeVm = vm;          // 컬렉션에 넣은 그 인스턴스를 기억
            }

            // 활성 레시피가 없으면 첫 항목 선택(옵션)
            activeVm ??= Recipes.FirstOrDefault();

            if (activeVm != null)
            {
                SelectedRecipe = activeVm;          // 컬렉션에 있는 동일 인스턴스
                CurrentRecipe = activeVm.Name;
                InitRecipeParams(activeVm);
            }
        }

        // 레시피 파라미터 갱신 
        private async void InitRecipeParams(RecipeVM recipeVM)
        {
            var parameters = await _recipeService.GetParameters(recipeVM.RecipeId);
            Parameters.Clear();

            foreach(RecipeParamDto dto in parameters)
            {
                Parameters.Add(ParameterVM.of(dto));
            }

        }
    }

    
}
