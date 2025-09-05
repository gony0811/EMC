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

            try
            {
                var dto = new RecipeDto
                {
                    Name = Name.Trim(),
                    IsActive = IsActive
                };
                await _svc.CreateRecipe(dto);
                RequestClose?.Invoke(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "레시피 생성 실패", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
