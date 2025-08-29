using Autofac;
using EGGPLANT._13_DataStore;
using EGGPLANT.ViewModels;
using UserControl = System.Windows.Controls.UserControl;

namespace EGGPLANT
{
    public partial class Usub01n04 : UserControl
    {
        public Usub01n04()
        {
            InitializeComponent();

            this.DataContext = new USubViewModel01n04();
            var store = App.Container.Resolve<MotorStateStore>();

            XMotorState.DataContext = store.X;
            YMotorState.DataContext = store.Y;
            ZMotorState.DataContext = store.Z;
        }
    }
}
