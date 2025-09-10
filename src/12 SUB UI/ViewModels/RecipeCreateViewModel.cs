using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace EGGPLANT
{
    public partial class RecipeCreateViewModel: ObservableObject
    {
        private readonly IRecipeService _svc;

        [ObservableProperty] private string name = string.Empty;
        [ObservableProperty] private bool isActive = false;
        [ObservableProperty] private bool canSave = false;   // 버튼 활성

        public event Action<bool>? RequestClose; // true: OK, false: Cancel

        public RecipeDto Result { get; private set; } = new RecipeDto();

        public RecipeCreateViewModel(IRecipeService svc)
        {
            _svc = svc;
            PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(Name))
                    CanSave = !string.IsNullOrWhiteSpace(Name);
            };
            CanSave = false;
        }

        [RelayCommand]
        private async Task Save()
        {
            if (!CanSave) return;

            if (Name == null || string.IsNullOrWhiteSpace(Name))
            {
                MessageBox.Show("이름은 필수입니다.", "유효성 검사 실패",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Result = new RecipeDto
            {
                Name = Name.Trim(),
                IsActive = IsActive
            };
            //await _svc.CreateRecipe(dto);
            RequestClose?.Invoke(true);            
        }

    }
}
