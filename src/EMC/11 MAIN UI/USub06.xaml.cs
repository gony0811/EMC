
using Autofac;
using EPFramework.IoC;
using System.Windows.Controls;
using System.Windows.Input;

namespace EMC
{
    [View(Lifetime.Singleton)]
    public partial class USub06 : Page
    {
        public USub06(USub06ViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }

        private void DataGridRow_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow row)
            {
                row.IsSelected = true;
                e.Handled = true;
            }
        }
    }
}
