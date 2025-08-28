
using Autofac;
using EGGPLANT._13_DataStore;
using System.Windows.Controls;
using System.Windows.Forms;

namespace EGGPLANT
{
    /// <summary>
    /// USub07.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class USub07 : Page
    {
        public USub07()
        {
            InitializeComponent();
            var store = App.Container.Resolve<MotorStateStore>();
            MoterState.DataContext = store.X;
        }
    }
}
