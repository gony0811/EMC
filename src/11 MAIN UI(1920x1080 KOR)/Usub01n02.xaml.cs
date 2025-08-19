using EGGPLANT.ViewModels;
using UserControl = System.Windows.Controls.UserControl;

namespace EGGPLANT
{
    /// <summary>
    /// Interaction logic for Usub01n02.xaml
    /// </summary>
    public partial class Usub01n02 : UserControl
    {
        public Usub01n02()
        {
            this.DataContext = new USubViewModel01n02();
            InitializeComponent();
            
        }
    }
}
