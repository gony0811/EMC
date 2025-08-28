using System.Windows;
using UserControl = System.Windows.Controls.UserControl;


namespace EGGPLANT._12_SUB_UI
{
    public partial class AxisWaypointList : UserControl
    {
        public AxisWaypointList()
        {
            InitializeComponent();
        }

        // 제목
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string),
                typeof(AxisWaypointList), new PropertyMetadata("WAYPOINTS"));
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        // 아이템들 (AxisWaypointVM 목록)
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register(nameof(Items),
                typeof(IEnumerable<AxisWaypointVM>),
                typeof(AxisWaypointList),
                new PropertyMetadata(null));
        public IEnumerable<AxisWaypointVM> Items
        {
            get => (IEnumerable<AxisWaypointVM>)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

    }
}
