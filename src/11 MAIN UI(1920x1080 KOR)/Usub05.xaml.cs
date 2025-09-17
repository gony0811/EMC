using System.Windows.Controls;


namespace EGGPLANT
{
    /// <summary>
    /// Usub05.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class USub05 : Page
    {
        public USub05(USub05ViewModel vm)
        {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
