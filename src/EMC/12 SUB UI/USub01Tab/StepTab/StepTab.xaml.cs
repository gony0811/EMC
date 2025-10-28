
using EPFramework.IoC;
using System.Windows.Controls;

namespace EMC
{
    [View(Lifetime.Scoped)]
    public partial class StepTab : UserControl
    {
        public StepTab(StepTabViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }
    }
}
