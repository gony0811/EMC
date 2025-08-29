using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using UserControl = System.Windows.Controls.UserControl;

namespace EGGPLANT
{
    [ContentProperty(nameof(ActionContent))]
    public partial class HeaderAction : UserControl
    {
        public HeaderAction()
        {
            InitializeComponent();
        }


        // Label side
        public static readonly DependencyProperty LabelTextProperty =
        DependencyProperty.Register(nameof(LabelText), typeof(string), typeof(HeaderAction), new PropertyMetadata(""));
        public string LabelText { get => (string)GetValue(LabelTextProperty); set => SetValue(LabelTextProperty, value); }


        public static readonly DependencyProperty LabelTemplateProperty =
        DependencyProperty.Register(nameof(LabelTemplate), typeof(DataTemplate), typeof(HeaderAction), new PropertyMetadata(null));
        public DataTemplate LabelTemplate { get => (DataTemplate)GetValue(LabelTemplateProperty); set => SetValue(LabelTemplateProperty, value); }


        // Right (external content)
        public static readonly DependencyProperty ActionContentProperty =
        DependencyProperty.Register(nameof(ActionContent), typeof(object), typeof(HeaderAction), new PropertyMetadata(null));
        public object ActionContent { get => GetValue(ActionContentProperty); set => SetValue(ActionContentProperty, value); }


        public static readonly DependencyProperty ActionTemplateProperty =
        DependencyProperty.Register(nameof(ActionTemplate), typeof(DataTemplate), typeof(HeaderAction), new PropertyMetadata(null));
        public DataTemplate ActionTemplate { get => (DataTemplate)GetValue(ActionTemplateProperty); set => SetValue(ActionTemplateProperty, value); }


        // Column widths
        public static readonly DependencyProperty LabelColumnWidthProperty =
        DependencyProperty.Register(nameof(LabelColumnWidth), typeof(GridLength), typeof(HeaderAction), new PropertyMetadata(new GridLength(0.6, GridUnitType.Star)));
        public GridLength LabelColumnWidth { get => (GridLength)GetValue(LabelColumnWidthProperty); set => SetValue(LabelColumnWidthProperty, value); }


        public static readonly DependencyProperty ActionColumnWidthProperty =
        DependencyProperty.Register(nameof(ActionColumnWidth), typeof(GridLength), typeof(HeaderAction), new PropertyMetadata(new GridLength(0.4, GridUnitType.Star)));
        public GridLength ActionColumnWidth { get => (GridLength)GetValue(ActionColumnWidthProperty); set => SetValue(ActionColumnWidthProperty, value); }
    }
}