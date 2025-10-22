using EPFramework.IoC;
using System.Windows.Controls;

namespace EMC
{
    [View(Lifetime.Singleton)]
    public partial class USub01 : Page
    {
        public USub01(USub01ViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
           
        }
    }
}
