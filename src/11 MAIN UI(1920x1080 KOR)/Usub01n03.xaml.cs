using Autofac;
using EGGPLANT.ViewModels;
using UserControl = System.Windows.Controls.UserControl;

namespace EGGPLANT
{
    /// <summary>
    /// Interaction logic for Usub01n02.xaml
    /// </summary>
    public partial class USub01n03 : UserControl
    {
        public USub01n03()
        {
            InitializeComponent();

            this.DataContext = new USub01n03ViewModel();
            var store = App.Container.Resolve<MotorStateStore>();

            XMotorState.DataContext = store.X;
            YMotorState.DataContext = store.Y;
            ZMotorState.DataContext = store.Z;
        }
    }
}
