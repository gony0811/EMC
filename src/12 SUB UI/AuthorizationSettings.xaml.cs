using System.Collections;
using System.Windows;
using UserControl = System.Windows.Controls.UserControl;

namespace EGGPLANT
{
    /// <summary>
    /// Interaction logic for AuthorizationSettings.xaml
    /// </summary>
    public partial class AuthorizationSettings : UserControl
    {
        public AuthorizationSettings()
        {
            InitializeComponent();
        }

        #region Dependency Properties
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title), typeof(string), typeof(AuthorizationSettings),
                new PropertyMetadata(""));
        public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                nameof(ItemsSource), typeof(IEnumerable), typeof(AuthorizationSettings),
                new PropertyMetadata(null));
        public IEnumerable ItemsSource { get => (IEnumerable)GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }

        public static readonly DependencyProperty LabelTemplateProperty =
            DependencyProperty.Register(
                nameof(LabelTemplate), typeof(DataTemplate), typeof(AuthorizationSettings),
                new PropertyMetadata(null, OnTemplatesChanged));
        public DataTemplate LabelTemplate { get => (DataTemplate)GetValue(LabelTemplateProperty); set => SetValue(LabelTemplateProperty, value); }

        public static readonly DependencyProperty ButtonTemplateProperty =
            DependencyProperty.Register(
                nameof(ButtonTemplate), typeof(DataTemplate), typeof(AuthorizationSettings),
                new PropertyMetadata(null, OnTemplatesChanged));
        public DataTemplate ButtonTemplate { get => (DataTemplate)GetValue(ButtonTemplateProperty); set => SetValue(ButtonTemplateProperty, value); }
        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            // Apply defaults if user hasn't provided templates
            if (LabelTemplate == null && TryFindResource("AS_DefaultLabelTemplate") is DataTemplate lbl)
                LabelTemplate = lbl;
            if (ButtonTemplate == null && TryFindResource("AS_DefaultButtonTemplate") is DataTemplate btn)
                ButtonTemplate = btn;
        }

        private static void OnTemplatesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // nothing else required; bindings in XAML pick up changes automatically
        }
    }
}
