using MahApps.Metro.Controls;

namespace EGGPLANT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class UMain : MetroWindow
    {
        public UMain(UMainViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        
    }
}