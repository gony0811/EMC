
using EPFramework.IoC;
using System.Windows.Controls;

namespace EMC
{

    [View(Lifetime.Scoped)]
    public partial class ManualTab : UserControl
    {
        public ManualTab(ManualTabViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }
    }
}
