using Autofac;
using EGGPLANT;
using EGGPLANT.ViewModels;
using UserControl = System.Windows.Controls.UserControl;

namespace EGGPLANT
{
    public partial class USub01n04 : UserControl
    {
        public USub01n04()
        {
            InitializeComponent();
            var store = App.Container.Resolve<MotorStateStore>();

            XMotorState.DataContext = store.X;
            YMotorState.DataContext = store.Y;
            ZMotorState.DataContext = store.Z;
        }
    }
}
