using EGGPLANT.ViewModels;
using UserControl = System.Windows.Controls.UserControl;

namespace EGGPLANT
{
    /// <summary>
    /// Interaction logic for USub01n02.xaml
    /// </summary>
    public partial class USub01n03 : UserControl
    {
        public USub01n03()
        {
            this.DataContext = new USub01n03ViewModel();
            InitializeComponent();
        }
    }
}
