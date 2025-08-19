using EGGPLANT.ViewModels;
using UserControl = System.Windows.Controls.UserControl;

namespace EGGPLANT
{
    /// <summary>
    /// Interaction logic for Usub01n02.xaml
    /// </summary>
    public partial class Usub01n03 : UserControl
    {
        public Usub01n03()
        {
            this.DataContext = new USubViewModel01n03();
            InitializeComponent();
        }
    }
}
