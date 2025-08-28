using Autofac;
using EGGPLANT._13_DataStore;
using EGGPLANT.ViewModels;
using UserControl = System.Windows.Controls.UserControl;

namespace EGGPLANT
{
    /// <summary>
    /// Interaction logic for Usub01n02.xaml
    /// </summary>
    public partial class Usub01n03 : UserControl
    {
        public Usub01n03()
        {
            InitializeComponent();

            this.DataContext = new USubViewModel01n03();
            var store = App.Container.Resolve<MotorStateStore>();

            XMotorState.DataContext = store.X;
            YMotorState.DataContext = store.Y;
            ZMotorState.DataContext = store.Z;
        }
    }
}
