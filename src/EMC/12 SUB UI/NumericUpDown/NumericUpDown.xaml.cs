using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EMC
{
    public partial class NumericUpDown : UserControl
    {
        public NumericUpDown()
        {
            InitializeComponent();
        }

        // === Value ===
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(int),
                typeof(NumericUpDown),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        // === Minimum ===
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(nameof(Minimum), typeof(int),
                typeof(NumericUpDown), new PropertyMetadata(0));
        public int Minimum
        {
            get => (int)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        // === Maximum ===
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(nameof(Maximum), typeof(int),
                typeof(NumericUpDown), new PropertyMetadata(100));
        public int Maximum
        {
            get => (int)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        // === Step ===
        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register(nameof(Step), typeof(int),
                typeof(NumericUpDown), new PropertyMetadata(1));
        public int Step
        {
            get => (int)GetValue(StepProperty);
            set => SetValue(StepProperty, value);
        }

        // === IsReadOnly (키보드 입력 차단) ===
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool),
                typeof(NumericUpDown), new PropertyMetadata(true));
        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        // === 색상 커스터마이징 ===
        public static readonly DependencyProperty ButtonBackgroundProperty =
            DependencyProperty.Register(nameof(ButtonBackground), typeof(Brush),
                typeof(NumericUpDown), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(46, 64, 83))));
        public Brush ButtonBackground
        {
            get => (Brush)GetValue(ButtonBackgroundProperty);
            set => SetValue(ButtonBackgroundProperty, value);
        }

        public static readonly DependencyProperty ButtonForegroundProperty =
            DependencyProperty.Register(nameof(ButtonForeground), typeof(Brush),
                typeof(NumericUpDown), new PropertyMetadata(Brushes.White));
        public Brush ButtonForeground
        {
            get => (Brush)GetValue(ButtonForegroundProperty);
            set => SetValue(ButtonForegroundProperty, value);
        }

        public static readonly DependencyProperty ButtonBorderBrushProperty =
            DependencyProperty.Register(nameof(ButtonBorderBrush), typeof(Brush),
                typeof(NumericUpDown), new PropertyMetadata(Brushes.Transparent));
        public Brush ButtonBorderBrush
        {
            get => (Brush)GetValue(ButtonBorderBrushProperty);
            set => SetValue(ButtonBorderBrushProperty, value);
        }

        // === 버튼 클릭 이벤트 ===
        private void Increase_Click(object sender, RoutedEventArgs e)
        {
            if (Value + Step <= Maximum)
                Value += Step;
        }

        private void Decrease_Click(object sender, RoutedEventArgs e)
        {
            if (Value - Step >= Minimum)
                Value -= Step;
        }
    }
}
