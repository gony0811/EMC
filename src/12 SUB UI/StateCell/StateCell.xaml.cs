using System.Windows;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
using UserControl = System.Windows.Controls.UserControl;

namespace EGGPLANT._12_SUB_UI
{
    public partial class StateCell : UserControl
    {
        public StateCell()
        {
            InitializeComponent();

            Loaded += (_, __) =>
            {
                if (AutoUpdateLabel && string.IsNullOrEmpty(Label))
                    Label = DefaultLabel; // 초기 1회 자동 채움
            };
        }

        // 점(인디케이터) 크기
        public static readonly DependencyProperty DotSizeProperty =
            DependencyProperty.Register(nameof(DotSize), typeof(double),
                typeof(StateCell), new PropertyMetadata(14.0));
        public double DotSize
        {
            get => (double)GetValue(DotSizeProperty);
            set => SetValue(DotSizeProperty, value);
        }

        // 상태
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register(nameof(State), typeof(State),
                typeof(StateCell),
                new PropertyMetadata(State.Unknown, OnStateChanged));
        public State State
        {
            get => (State)GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }

        private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = (StateCell)d;

            // 상태 변화에 따라 DefaultLabel 갱신
            c.DefaultLabel = MapStateToDefaultLabel((State)e.NewValue);

            // 자동 라벨 모드면 현재 표시 라벨도 갱신
            if (c.AutoUpdateLabel)
                c.Label = c.DefaultLabel;
        }

        private static string MapStateToDefaultLabel(State state) => state switch
        {
            State.Normal => "정상",
            State.Warning => "경고",
            State.Error => "오류",
            State.Offline => "오프라인",
            _ => "미확인"
        };

        // 경고/에러 시 깜빡임 사용 여부
        public static readonly DependencyProperty BlinkOnAlertProperty =
            DependencyProperty.Register(nameof(BlinkOnAlert), typeof(bool),
                typeof(StateCell), new PropertyMetadata(true));
        public bool BlinkOnAlert
        {
            get => (bool)GetValue(BlinkOnAlertProperty);
            set => SetValue(BlinkOnAlertProperty, value);
        }

        // 상단 라벨
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label), typeof(string),
                typeof(StateCell), new PropertyMetadata(null));
        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        // 하단 보조 라벨
        public static readonly DependencyProperty StatusNameProperty =
            DependencyProperty.Register(nameof(StatusName), typeof(string),
                typeof(StateCell), new PropertyMetadata(null));
        public string StatusName
        {
            get => (string)GetValue(StatusNameProperty);
            set => SetValue(StatusNameProperty, value);
        }

        // DefaultLabel (상태에 따라 자동 갱신)
        public static readonly DependencyProperty DefaultLabelProperty =
            DependencyProperty.Register(nameof(DefaultLabel), typeof(string),
                typeof(StateCell), new PropertyMetadata("STATUS"));
        public string DefaultLabel
        {
            get => (string)GetValue(DefaultLabelProperty);
            set => SetValue(DefaultLabelProperty, value);
        }

        // 하단 보조 라벨 폰트 크기
        public static readonly DependencyProperty SubFontSizeProperty =
            DependencyProperty.Register(nameof(SubFontSize), typeof(double),
                typeof(StateCell), new PropertyMetadata(12.0));
        public double SubFontSize
        {
            get => (double)GetValue(SubFontSizeProperty);
            set => SetValue(SubFontSizeProperty, value);
        }

        // 상단/하단 텍스트 색
        public static readonly DependencyProperty TextBrushProperty =
            DependencyProperty.Register(nameof(TextBrush), typeof(Brush),
                typeof(StateCell),
                new PropertyMetadata((Brush)new BrushConverter().ConvertFrom("#FFFFFFFF")));
        public Brush TextBrush
        {
            get => (Brush)GetValue(TextBrushProperty);
            set => SetValue(TextBrushProperty, value);
        }

        public static readonly DependencyProperty SubTextBrushProperty =
            DependencyProperty.Register(nameof(SubTextBrush), typeof(Brush),
                typeof(StateCell),
                new PropertyMetadata((Brush)new BrushConverter().ConvertFrom("#B3FFFFFF")));
        public Brush SubTextBrush
        {
            get => (Brush)GetValue(SubTextBrushProperty);
            set => SetValue(SubTextBrushProperty, value);
        }

        // 상태 바뀔 때 라벨 자동 갱신 여부
        public static readonly DependencyProperty AutoUpdateLabelProperty =
           DependencyProperty.Register(nameof(AutoUpdateLabel), typeof(bool),
               typeof(StateCell), new PropertyMetadata(true));
        public bool AutoUpdateLabel
        {
            get => (bool)GetValue(AutoUpdateLabelProperty);
            set => SetValue(AutoUpdateLabelProperty, value);
        }
    }
}
