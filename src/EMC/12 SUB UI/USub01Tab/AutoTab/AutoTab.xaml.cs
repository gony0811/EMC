
using EPFramework.IoC;
using System.Windows.Controls;
namespace EMC
{
    [View(Lifetime.Singleton)]
    public partial class AutoTab : UserControl
    {
        public AutoTab()
        {
            InitializeComponent();
        }
    }
}
