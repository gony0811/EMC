

using EGGPLANT.ViewModels;
using System.Windows.Controls;

namespace EGGPLANT
{
    /// <summary>
    /// Interaction logic for USub08.xaml
    /// </summary>
    public partial class USub08 : Page
    {
        public USub08()
        {
            this.DataContext = new USub08ViewModel();
            InitializeComponent();
        }
    }
}
