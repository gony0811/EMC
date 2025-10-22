using CommunityToolkit.Mvvm.ComponentModel;
using EMC.DB;

namespace EMC
{
    public sealed class RecipeCreateVM: PromptDialogVM<Recipe>
    {

        public RecipeCreateVM(string title = null, Recipe initial = default)
            : base(initial ?? new Recipe(), title ?? "레시피 생성")
        {
        }
    }
}
