using Autofac;
using EGGPLANT._12_SUB_UI;
using EGGPLANT._13_DataStore;
using EGGPLANT.ViewModels;
using System.Collections.ObjectModel;
using UserControl = System.Windows.Controls.UserControl;

namespace EGGPLANT
{
    /// <summary>
    /// Interaction logic for Usub01n02.xaml
    /// </summary>
    public partial class Usub01n02 : UserControl
    {
        public Usub01n02()
        {

            InitializeComponent();
            // ViewModel은 컨테이너에서 Resolve
            DataContext = App.Container.Resolve<USubViewModel01n02>();

            var store = App.Container.Resolve<MotorStateStore>();

            XMotorState.DataContext = store.X;
            YMotorState.DataContext = store.Y;
            ZMotorState.DataContext = store.Z;


            // X축 WayPoints
            var list = new ObservableCollection<AxisWaypointVM>
            {
                new("READY",       pos: 0,   spd: 100, minPos: 0, maxPos: 1000, minSpd: 1, maxSpd: 300),
                new("MASTER JIG",  pos: 250, spd: 100, minPos: 0, maxPos: 1000, minSpd: 1, maxSpd: 300),
                new("LDS POS",     pos: 500, spd: 120, minPos: 0, maxPos: 1000, minSpd: 1, maxSpd: 300),
                new("VISION X",    pos: 700, spd:  80, minPos: 0, maxPos: 1000, minSpd: 1, maxSpd: 300),
            };
            XAxisList.Title = "X축";
            XAxisList.Items = list;


            // Y축 WayPoints
            var ylist = new ObservableCollection<AxisWaypointVM>
            {
                new("READY",       pos: 0,   spd: 100, minPos: 0, maxPos: 1000, minSpd: 1, maxSpd: 300),
                new("MASTER JIG",  pos: 250, spd: 100, minPos: 0, maxPos: 1000, minSpd: 1, maxSpd: 300),
                new("LDS POS",     pos: 500, spd: 120, minPos: 0, maxPos: 1000, minSpd: 1, maxSpd: 300),
                new("VISION X",    pos: 700, spd:  80, minPos: 0, maxPos: 1000, minSpd: 1, maxSpd: 300),
            };
            YAxisList.Title = "Y축";
            YAxisList.Items = ylist;

            // Z축 WayPoints 
            var zlist = new ObservableCollection<AxisWaypointVM>
            {
                new("READY",       pos: 0,   spd: 100, minPos: 0, maxPos: 1000, minSpd: 1, maxSpd: 300),
                new("MASTER JIG",  pos: 250, spd: 100, minPos: 0, maxPos: 1000, minSpd: 1, maxSpd: 300),
                new("LDS POS",     pos: 500, spd: 120, minPos: 0, maxPos: 1000, minSpd: 1, maxSpd: 300),
                new("VISION X",    pos: 700, spd:  80, minPos: 0, maxPos: 1000, minSpd: 1, maxSpd: 300),
            };
            ZAxisList.Title = "Y축";
            ZAxisList.Items = zlist;
        }

        public Usub01n02(MotorStateStore store)
        {
            InitializeComponent();
            DataContext = new USubViewModel01n02();
            XMotorState.DataContext = store.X;
            YMotorState.DataContext = store.Y;
            ZMotorState.DataContext = store.Z;
        }
    }
}
