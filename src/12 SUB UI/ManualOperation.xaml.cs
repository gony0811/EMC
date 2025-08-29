
using EGGPLANT.Models;
using System.Windows;
using UserControl = System.Windows.Controls.UserControl;

namespace EGGPLANT
{
    /// <summary>
    /// Interaction logic for ManualOperation.xaml
    /// </summary>
    public partial class ManualOperation : UserControl
    {
        public ManualOperation()
        {
            InitializeComponent();
        }

        public ManualOperationModel Model
        {
            get => (ManualOperationModel)GetValue(ModelProperty);
            set => SetValue(ModelProperty, value);
        }

        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.Register(
                nameof(Model),
                typeof(ManualOperationModel),
                typeof(ManualOperation),
                new PropertyMetadata(null));
    }
}
