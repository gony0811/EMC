using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace EMC
{
    public partial class KeyValueBox : UserControl
    {
        public KeyValueBox()
        {
            InitializeComponent();
        }

        // === Key ===
        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register(
                "Key",
                typeof(string),
                typeof(KeyValueBox),
                new PropertyMetadata(string.Empty));

        public string Key
        {
            get { return (string)GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        // === Value ===
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(object),
                typeof(KeyValueBox),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // === Key 배경색 (기본값: Gray) ===
        public static readonly DependencyProperty KeyBackgroundProperty =
            DependencyProperty.Register(
                "KeyBackground",
                typeof(Brush),
                typeof(KeyValueBox),
                new PropertyMetadata(Brushes.Gray));

        public Brush KeyBackground
        {
            get { return (Brush)GetValue(KeyBackgroundProperty); }
            set { SetValue(KeyBackgroundProperty, value); }
        }

        // === Key 글자색 (기본값: White) ===
        public static readonly DependencyProperty KeyForegroundProperty =
            DependencyProperty.Register(
                "KeyForeground",
                typeof(Brush),
                typeof(KeyValueBox),
                new PropertyMetadata(Brushes.White));

        public Brush KeyForeground
        {
            get { return (Brush)GetValue(KeyForegroundProperty); }
            set { SetValue(KeyForegroundProperty, value); }
        }

        // === Key 폰트 크기 (기본값: 13) ===
        public static readonly DependencyProperty KeyFontSizeProperty =
            DependencyProperty.Register(
                "KeyFontSize",
                typeof(double),
                typeof(KeyValueBox),
                new PropertyMetadata(13.0));

        public double KeyFontSize
        {
            get { return (double)GetValue(KeyFontSizeProperty); }
            set { SetValue(KeyFontSizeProperty, value); }
        }

        // === Key 폰트 두께 (기본값: Bold) ===
        public static readonly DependencyProperty KeyFontWeightProperty =
            DependencyProperty.Register(
                "KeyFontWeight",
                typeof(FontWeight),
                typeof(KeyValueBox),
                new PropertyMetadata(FontWeights.Bold));

        public FontWeight KeyFontWeight
        {
            get { return (FontWeight)GetValue(KeyFontWeightProperty); }
            set { SetValue(KeyFontWeightProperty, value); }
        }

        // === Value 글자색 (기본값: Black) ===
        public static readonly DependencyProperty ValueForegroundProperty =
            DependencyProperty.Register(
                "ValueForeground",
                typeof(Brush),
                typeof(KeyValueBox),
                new PropertyMetadata(Brushes.Black));

        public Brush ValueForeground
        {
            get { return (Brush)GetValue(ValueForegroundProperty); }
            set { SetValue(ValueForegroundProperty, value); }
        }

        // === Value 폰트 크기 (기본값: 14) ===
        public static readonly DependencyProperty ValueFontSizeProperty =
            DependencyProperty.Register(
                "ValueFontSize",
                typeof(double),
                typeof(KeyValueBox),
                new PropertyMetadata(14.0));

        public double ValueFontSize
        {
            get { return (double)GetValue(ValueFontSizeProperty); }
            set { SetValue(ValueFontSizeProperty, value); }
        }

        // === Value 폰트 두께 (기본값: SemiBold) ===
        public static readonly DependencyProperty ValueFontWeightProperty =
            DependencyProperty.Register(
                "ValueFontWeight",
                typeof(FontWeight),
                typeof(KeyValueBox),
                new PropertyMetadata(FontWeights.SemiBold));

        public FontWeight ValueFontWeight
        {
            get { return (FontWeight)GetValue(ValueFontWeightProperty); }
            set { SetValue(ValueFontWeightProperty, value); }
        }

        // === 숫자 입력 제한 ===
        private void OnTextInput(object sender, TextCompositionEventArgs e)
        {
            var valueType = Value?.GetType();
            if (valueType == typeof(int) || valueType == typeof(double))
            {
                var regex = new Regex("[^0-9.-]+");
                e.Handled = regex.IsMatch(e.Text);
            }
        }
    }
}
