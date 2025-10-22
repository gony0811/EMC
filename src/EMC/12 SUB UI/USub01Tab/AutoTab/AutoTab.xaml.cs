
using EPFramework.IoC;
using System.Windows.Controls;
namespace EMC
{
    [View(Lifetime.Scoped)]
    public partial class AutoTab : UserControl
    {
        public AutoTab(AutoTabViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }
    }
}
