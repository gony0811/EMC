
using EPFramework.IoC;
using System.Linq;
using System.Windows;

namespace EMC
{
    [View(Lifetime.Singleton)]
    public partial class UMain : Window
    {
        public UMain(UMainViewModel vm)
        {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
