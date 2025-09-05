using CommunityToolkit.Mvvm.ComponentModel;

namespace EGGPLANT
{
    public partial class RecipeVM : ObservableObject
    {
        public int RecipeId { get; }   
        public RecipeVM(int id) => RecipeId = id;

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private bool isActive;

        public RecipeVM()
        {
        }

        private RecipeVM(RecipeDto dto)
        {
            RecipeId = dto.RecipeId;
            Name = dto.Name;
            IsActive = dto.IsActive;
        }

        public static RecipeVM of(RecipeDto dto)
        {
            return new RecipeVM(dto);
        }
    }
}
