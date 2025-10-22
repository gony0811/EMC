using EPFramework.IoC;
using System.Windows.Controls;

namespace EMC
{
    [View(Lifetime.Singleton)]
    public partial class USub07 : Page
    {
        public USub07(USub07ViewModel vm)
        {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
