using UserControl = System.Windows.Controls.UserControl;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using EGGPLANT._12_SUB_UI;
using Binding = System.Windows.Data.Binding;

namespace EGGPLANT._12_SUB_UI
{
    public partial class MoterStateControl : UserControl
    {
        public MoterStateControl()
        {
            InitializeComponent();
        }
            // 외부에서 주입할 VM
        public MoterStateControlVM ViewModel
        {
            get => (MoterStateControlVM)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(
                nameof(ViewModel),
                typeof(MoterStateControlVM),
                typeof(MoterStateControl),
                new PropertyMetadata(null));
    }
}
