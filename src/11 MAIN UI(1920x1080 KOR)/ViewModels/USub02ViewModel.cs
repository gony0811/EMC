
using Autofac;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using Application = System.Windows.Application;

namespace EGGPLANT.ViewModels
{
    public partial class USub02ViewModel : ObservableObject
    {
        private IRecipeService _recipeService;
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
        public USub02ViewModel(IRecipeService recipeService)
        {
            _recipeService = recipeService;
            InitRecipe();
            
        }

        [RelayCommand]
        public async void CreateRecipe()
        {
            // 창을 DI로 꺼내와서 모달 표시
            var win = App.Container.Resolve<RecipeCreateWindow>();
            var owner = System.Windows.Application.Current?.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.IsActive) ?? System.Windows.Application.Current?.MainWindow;
            win.Owner = owner;

            
            if (win.ShowDialog() == true)
            {
                // 생성 성공 → 목록 갱신 등
                InitRecipe();
            }
        }

        

        [RelayCommand]
        public void RecipeClick(RecipeVM item)
        {
            if (item is null) return;
            // 단일 클릭 처리
            SelectedRecipe = item;
            CurrentRecipe = item.Name;
            InitRecipeParams(SelectedRecipe);
        }

        [RelayCommand]
        public void RecipeDoubleClick(RecipeVM item)
        {
            if (item is null) return;

        }

        [RelayCommand]
        public void CreateParameter()
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
            if (result == true)
            {
                InitRecipeParams(SelectedRecipe);
            }
        }


        // 레시피 목록 갱신
        private async void InitRecipe()
        {
            var recipes = await _recipeService.GetRecipes();
            Recipes.Clear();

            foreach (RecipeDto dto in recipes)
            {
                Recipes.Add(RecipeVM.of(dto));
                if (dto.IsActive)
                {
                    SelectedRecipe = RecipeVM.of(dto);
                    CurrentRecipe = dto.Name;
                    InitRecipeParams(SelectedRecipe);
                }
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
