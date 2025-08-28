using System.Windows;
using UserControl = System.Windows.Controls.UserControl;

namespace EGGPLANT._12_SUB_UI
{
    /// <summary>
    /// Interaction logic for AxisWaypointSet.xaml
    /// </summary>
    public partial class AxisWaypointSet : UserControl
    {
        public AxisWaypointSet()
        {
            InitializeComponent();
            //SetBinding(DataContextProperty, new System.Windows.Data.Binding(nameof(ViewModel)) { Source = this });
        }
        

        // 외부 VM 주입
        public AxisWaypointVM ViewModel
        {
            get => (AxisWaypointVM)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(
                nameof(ViewModel),
                typeof(AxisWaypointVM),
                typeof(AxisWaypointSet),
                new PropertyMetadata(null));
    }
}

