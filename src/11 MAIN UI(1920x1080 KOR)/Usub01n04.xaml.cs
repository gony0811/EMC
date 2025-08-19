using EGGPLANT.ViewModels;
using UserControl = System.Windows.Controls.UserControl;

namespace EGGPLANT
{
    public partial class Usub01n04 : UserControl
    {
        public Usub01n04()
        {
            this.DataContext = new USubViewModel01n04();
            InitializeComponent();
        }
    }
}
