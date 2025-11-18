
using EPFramework.IoC;
using System.Windows.Controls;

namespace EMC
{
    [View(Lifetime.Singleton)]
    public partial class USub08 : Page
    {
        public USub08(USub08ViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }
    }
}
