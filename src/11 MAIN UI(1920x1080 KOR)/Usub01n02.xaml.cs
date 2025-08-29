using Autofac;
using CommunityToolkit.Mvvm.Input;
using EGGPLANT._12_SUB_UI;
using EGGPLANT._12_SUB_UI.ViewModels;
using EGGPLANT._13_DataStore;
using EGGPLANT.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Documents;
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

            // X축 WayPoints
            var list = new ObservableCollection<AxisWaypointVM>
            {
                new("READY",       pos: 0,   spd: 100, minPos: 0, maxPos: 1000, minSpd: 1, maxSpd: 300),
                new("MASTER JIG",  pos: 250, spd: 100, minPos: 0, maxPos: 1000, minSpd: 1, maxSpd: 300),
                new("LDS POS",     pos: 500, spd: 120, minPos: 0, maxPos: 1000, minSpd: 1, maxSpd: 300),
                new("VISION X",    pos: 700, spd:  80, minPos: 0, maxPos: 1000, minSpd: 1, maxSpd: 300),
            };
            XAxisList.Items = list;


            // Y축 WayPoints
            var ylist = new ObservableCollection<AxisWaypointVM>
            {
                new("READY",       pos: 0,   spd: 100, minPos: 0, maxPos: 1000, minSpd: 1, maxSpd: 300),
                new("MASTER JIG",  pos: 250, spd: 100, minPos: 0, maxPos: 1000, minSpd: 1, maxSpd: 300),
                new("LDS POS",     pos: 500, spd: 120, minPos: 0, maxPos: 1000, minSpd: 1, maxSpd: 300),
                new("VISION X",    pos: 700, spd:  80, minPos: 0, maxPos: 1000, minSpd: 1, maxSpd: 300),
            };
            YAxisList.Items = ylist;

            // Z축 WayPoints 
            var zlist = new ObservableCollection<AxisWaypointVM>
            {
                new("READY",       pos: 0,   spd: 100, minPos: 0, maxPos: 1000, minSpd: 1, maxSpd: 300),
                new("MASTER JIG",  pos: 250, spd: 100, minPos: 0, maxPos: 1000, minSpd: 1, maxSpd: 300),
                new("LDS POS",     pos: 500, spd: 120, minPos: 0, maxPos: 1000, minSpd: 1, maxSpd: 300),
                new("VISION X",    pos: 700, spd:  80, minPos: 0, maxPos: 1000, minSpd: 1, maxSpd: 300),
            };
            ZAxisList.Items = zlist;

            var actions = new ObservableCollection<ActionButtonVM>
            {
                new ActionButtonVM("ZERO SET", new RelayCommand(() => {}), 0),
                new ActionButtonVM("ZERO CANCEL", new RelayCommand(() => {}), 0),
                new ActionButtonVM("ZERO LIVE", new RelayCommand(() => {}), 0),
                new ActionButtonVM("ZERO READ", new RelayCommand(() => {}), 0),

            };

            Displacement1.Title = "변위센서 컨트롤1";
            Displacement1.Value = 0.0;
            Displacement1.Actions = actions;

            Displacement2.Title = "변위센서 컨트롤2";
            Displacement2.Value = 0.0;
            Displacement2.Actions = actions;

            Displacement3.Title = "변위센서 컨트롤3";
            Displacement3.Value = 0.0;
            Displacement3.Actions = actions;
        }

        public Usub01n02(USubViewModel01n02 vm, MotorStateStore store)
        {
            InitializeComponent();
            DataContext = vm;

            XMotorState.DataContext = store.X;
            YMotorState.DataContext = store.Y; 
            ZMotorState.DataContext = store.Z;
        }

        //public void test()
        //{

        //}

    }
}
