
using UserControl = System.Windows.Controls.UserControl;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using System.Globalization;
using System.Windows.Data;
using Binding = System.Windows.Data.Binding;

namespace EGGPLANT
{
    public enum LampState { Off, On, Blink }

    public partial class PowerLamp : UserControl
    {
        public PowerLamp() { InitializeComponent(); }

        // 상태 (Off / On / Blink)
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register(nameof(State), typeof(LampState),
                typeof(PowerLamp), new PropertyMetadata(LampState.Off));
        public LampState State { get => (LampState)GetValue(StateProperty); set => SetValue(StateProperty, value); }

        // 색상
        public static readonly DependencyProperty OnBrushProperty =
            DependencyProperty.Register(nameof(OnBrush), typeof(Brush),
                typeof(PowerLamp),
                new PropertyMetadata((Brush)new BrushConverter().ConvertFrom("#17C3B2"))); // 켜짐 색
        public Brush OnBrush { get => (Brush)GetValue(OnBrushProperty); set => SetValue(OnBrushProperty, value); }

        public static readonly DependencyProperty OffBrushProperty =
            DependencyProperty.Register(nameof(OffBrush), typeof(Brush),
                typeof(PowerLamp),
                new PropertyMetadata((Brush)new BrushConverter().ConvertFrom("#6E6E6E"))); // 꺼짐 색
        public Brush OffBrush { get => (Brush)GetValue(OffBrushProperty); set => SetValue(OffBrushProperty, value); }

        // Command (선택)
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand),
                typeof(PowerLamp), new PropertyMetadata(null));
        public ICommand Command { get => (ICommand)GetValue(CommandProperty); set => SetValue(CommandProperty, value); }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(nameof(CommandParameter), typeof(object),
                typeof(PowerLamp), new PropertyMetadata(null));
        public object CommandParameter { get => GetValue(CommandParameterProperty); set => SetValue(CommandParameterProperty, value); }

        // Click RoutedEvent (선택)
        public static readonly RoutedEvent ClickEvent =
            EventManager.RegisterRoutedEvent(nameof(Click), RoutingStrategy.Bubble,
                                             typeof(RoutedEventHandler), typeof(PowerLamp));
        public event RoutedEventHandler Click
        {
            add => AddHandler(ClickEvent, value);
            remove => RemoveHandler(ClickEvent, value);
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
            => RaiseEvent(new RoutedEventArgs(ClickEvent));
    }

    // LampState → 텍스트 변환 ("꺼짐|켜짐|깜빡임" 커스텀 가능)
    public class LampStateToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var labels = (parameter as string)?.Split('|') ?? new[] { "Off", "On", "Blink" };
            return value is LampState s
                ? s switch
                {
                    LampState.Off => labels.Length > 0 ? labels[0] : "Off",
                    LampState.On => labels.Length > 1 ? labels[1] : "On",
                    LampState.Blink => labels.Length > 2 ? labels[2] : "Blink",
                    _ => ""
                }
                : "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => Binding.DoNothing;
    }
}
