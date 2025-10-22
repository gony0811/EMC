
using EPFramework.IoC;
using System.Windows.Controls;

namespace EMC
{

    [View(Lifetime.Singleton)]
    public partial class ManualTab : UserControl
    {
        public ManualTab()
        {
            InitializeComponent();
        }
    }
}
