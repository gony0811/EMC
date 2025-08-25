using EGGPLANT.ViewModels;
using System.Windows.Controls;

namespace EGGPLANT
{
    /// <summary>
    /// USub09.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class USub09 : Page
    {
        public USub09()
        {
            this.DataContext = new USubViewModel09();
            InitializeComponent();
        }
    }
}
