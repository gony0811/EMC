using System.Windows.Media;
using System.Windows;
using UserControl = System.Windows.Controls.UserControl;
using Brush = System.Windows.Media.Brush;
using EGGPLANT.ViewModels;
namespace EGGPLANT
{
    public partial class DisplacementControl : UserControl
    {
        public DisplacementControl()
        {
            InitializeComponent();
        }

        // 제목
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string),
                typeof(DisplacementControl), new PropertyMetadata("DISPLACEMENT"));
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        // 표시 값 (double 가정, 필요하면 string으로 바꿔도 됨)
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(double),
                typeof(DisplacementControl), new PropertyMetadata(0.0));
        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        // 값 색상
        public static readonly DependencyProperty ValueBrushProperty =
            DependencyProperty.Register(nameof(ValueBrush), typeof(Brush),
                typeof(DisplacementControl),
                new PropertyMetadata((Brush)new BrushConverter().ConvertFrom("#00FF57")));
        public Brush ValueBrush
        {
            get => (Brush)GetValue(ValueBrushProperty);
            set => SetValue(ValueBrushProperty, value);
        }

        // 동적 버튼 목록
        public static readonly DependencyProperty ActionsProperty =
            DependencyProperty.Register(nameof(Actions), typeof(IEnumerable<ActionButtonVM>),
                typeof(DisplacementControl), new PropertyMetadata(null));
        public IEnumerable<ActionButtonVM> Actions
        {
            get => (IEnumerable<ActionButtonVM>)GetValue(ActionsProperty);
            set => SetValue(ActionsProperty, value);
        }
    }
}

