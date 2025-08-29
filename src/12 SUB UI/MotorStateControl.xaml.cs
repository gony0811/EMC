using UserControl = System.Windows.Controls.UserControl;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Binding = System.Windows.Data.Binding;

namespace EGGPLANT
{
    public partial class MotorStateControl : UserControl
    {
        public MotorStateControl()
        {
            InitializeComponent();
        }
            // 외부에서 주입할 VM
        public MotorStateControlVM ViewModel
        {
            get => (MotorStateControlVM)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(
                nameof(ViewModel),
                typeof(MotorStateControlVM),
                typeof(MotorStateControl),
                new PropertyMetadata(null));
    }
}
