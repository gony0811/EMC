using System.Windows.Media;
using UserControl = System.Windows.Controls.UserControl;
using System.Windows;
using EGGPLANT._11_MAIN_UI_1920x1080_KOR_.Models;
using Brush = System.Windows.Media.Brush;

namespace EGGPLANT._12_SUB_UI
{
    /// <summary>
    /// Interaction logic for SensorStatusCell.xaml
    /// </summary>
    public partial class SensorStatusCell : UserControl
    {
        public SensorStatusCell()
        {
            InitializeComponent();

            // 기본 색/폰트
            TextBrush ??= (Brush)new BrushConverter().ConvertFromString("#FFFFFFFF");
            SubTextBrush ??= (Brush)new BrushConverter().ConvertFromString("#B3FFFFFF");

            // 처음 로드될 때도 자동 라벨 1회 보정
            Loaded += (_, __) =>
            {
                if (AutoUpdateLabel && string.IsNullOrEmpty(Label))
                    Label = DefaultLabel;
            };
        }

        // 상태 enum
        // 1) State 변경 시 Label 자동 갱신
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register(nameof(State), typeof(SensorStatus),
                typeof(SensorStatusCell),
                new PropertyMetadata(SensorStatus.Unknown, OnStateChanged));
        private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = (SensorStatusCell)d;
            if (c.AutoUpdateLabel)
                c.Label = c.DefaultLabel;  // 상태에 맞는 라벨로 자동 갱신
        }

        // 2) AutoUpdateLabel: State 바뀔 때 Label 자동 갱신할지
        public static readonly DependencyProperty AutoUpdateLabelProperty =
            DependencyProperty.Register(nameof(AutoUpdateLabel), typeof(bool),
                typeof(SensorStatusCell), new PropertyMetadata(true));

        public bool AutoUpdateLabel
        {
            get => (bool)GetValue(AutoUpdateLabelProperty);
            set => SetValue(AutoUpdateLabelProperty, value);
        }

        public SensorStatus State
        {
            get => (SensorStatus)GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }

        // 상단 라벨(주 라벨)
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label), typeof(string),
                typeof(SensorStatusCell), new PropertyMetadata(null));
        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }


        // 하단 라벨(상태명)
        public static readonly DependencyProperty StatusNameProperty =
            DependencyProperty.Register(nameof(StatusName), typeof(string),
                typeof(SensorStatusCell), new PropertyMetadata(null));
        public string StatusName
        {
            get => (string)GetValue(StatusNameProperty);
            set => SetValue(StatusNameProperty, value);
        }

        // 경고/오류 깜빡임
        public static readonly DependencyProperty BlinkOnAlertProperty =
            DependencyProperty.Register(nameof(BlinkOnAlert), typeof(bool),
                typeof(SensorStatusCell), new PropertyMetadata(true));
        public bool BlinkOnAlert
        {
            get => (bool)GetValue(BlinkOnAlertProperty);
            set => SetValue(BlinkOnAlertProperty, value);
        }

        // 점 크기
        public static readonly DependencyProperty DotSizeProperty =
            DependencyProperty.Register(nameof(DotSize), typeof(double),
                typeof(SensorStatusCell), new PropertyMetadata(10.0));
        public double DotSize
        {
            get => (double)GetValue(DotSizeProperty);
            set => SetValue(DotSizeProperty, value);
        }

        // 상단 텍스트 색
        public static readonly DependencyProperty TextBrushProperty =
            DependencyProperty.Register(nameof(TextBrush), typeof(Brush),
                typeof(SensorStatusCell), new PropertyMetadata(null));
        public Brush TextBrush
        {
            get => (Brush)GetValue(TextBrushProperty);
            set => SetValue(TextBrushProperty, value);
        }

        // 하단 텍스트 색/크기
        public static readonly DependencyProperty SubTextBrushProperty =
            DependencyProperty.Register(nameof(SubTextBrush), typeof(Brush),
                typeof(SensorStatusCell), new PropertyMetadata(null));
        public Brush SubTextBrush
        {
            get => (Brush)GetValue(SubTextBrushProperty);
            set => SetValue(SubTextBrushProperty, value);
        }

        public static readonly DependencyProperty SubFontSizeProperty =
        DependencyProperty.Register(nameof(SubFontSize), typeof(double),
            typeof(SensorStatusCell), new PropertyMetadata(12.0));
        public double SubFontSize
        {
            get => (double)GetValue(SubFontSizeProperty);
            set => SetValue(SubFontSizeProperty, value);
        }

        // 기본 한글 라벨
        public string DefaultLabel => State switch
        {
            SensorStatus.Normal => "정상",
            SensorStatus.Warning => "경고",
            SensorStatus.Error => "오류",
            SensorStatus.Offline => "오프라인",
            _ => "미확인"
        };
    }
}
