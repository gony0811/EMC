using EGGPLANT.ViewModels;
using UserControl = System.Windows.Controls.UserControl;

namespace EGGPLANT
{
    public partial class USub01n04 : UserControl
    {
        public USub01n04()
        {
            this.DataContext = new USub01n04ViewModel();
            InitializeComponent();
        }
    }
}
