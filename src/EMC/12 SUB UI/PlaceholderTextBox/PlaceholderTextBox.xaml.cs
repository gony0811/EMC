
using System.Windows;
using System.Windows.Controls;

namespace EMC
{
    public partial class PlaceholderTextBox : UserControl
    {
        public PlaceholderTextBox()
        {
            InitializeComponent();

            InputBox.TextChanged += OnTextChanged;
            InputBox.GotFocus += (s, e) => UpdatePlaceholderVisibility();
            InputBox.LostFocus += (s, e) => UpdatePlaceholderVisibility();
            Loaded += (s, e) => UpdatePlaceholderVisibility();
        }

        private void UpdatePlaceholderVisibility()
        {
            bool showPlaceholder = string.IsNullOrWhiteSpace(InputBox.Text) && !InputBox.IsFocused;
            PlaceholderBlock.Visibility = showPlaceholder ? Visibility.Visible : Visibility.Collapsed;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            UpdatePlaceholderVisibility();
        }

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(PlaceholderTextBox),
                new PropertyMetadata(string.Empty));

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(PlaceholderTextBox),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
    }
}
