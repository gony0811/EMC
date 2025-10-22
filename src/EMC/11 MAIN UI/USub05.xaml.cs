using EPFramework.IoC;
using System.Windows.Controls;

namespace EMC
{
    [View(Lifetime.Singleton)]
    public partial class USub05 : Page
    {
        public USub05(USub05ViewModel vm)
        {
            this.DataContext = vm;
            InitializeComponent();
        }

    }
}
