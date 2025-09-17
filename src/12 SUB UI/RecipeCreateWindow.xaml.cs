
using System.Windows;

namespace EGGPLANT
{
    /// <summary>
    /// RecipeCreateWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class RecipeCreateWindow : Window
    {
        public RecipeCreateWindow(RecipeCreateViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            vm.RequestClose += ok =>
            {
                DialogResult = ok;
                Close();
            };
        }
    }
}
