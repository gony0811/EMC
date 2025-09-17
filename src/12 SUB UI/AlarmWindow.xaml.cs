using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace EGGPLANT
{
    /// <summary>
    /// AlarmWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AlarmWindow : Window
    {
        public AlarmWindow()
        {
            InitializeComponent();
            // 기본 DataContext를 자기 자신으로 (DP 바인딩 편의)
            DataContext = this;
        }

        // ── 노출 속성 (바인딩 대상) ───────────────────────
        public static readonly DependencyProperty TitleTextProperty =
            DependencyProperty.Register(nameof(TitleText), typeof(string), typeof(AlarmWindow),
                new PropertyMetadata("알림"));

        public string TitleText
        {
            get => (string)GetValue(TitleTextProperty);
            set => SetValue(TitleTextProperty, value);
        }

        public static readonly DependencyProperty MessageTextProperty =
            DependencyProperty.Register(nameof(MessageText), typeof(string), typeof(AlarmWindow),
                new PropertyMetadata("설명을 입력하세요."));

        public string MessageText
        {
            get => (string)GetValue(MessageTextProperty);
            set => SetValue(MessageTextProperty, value);
        }

        public static readonly DependencyProperty ActionsProperty =
            DependencyProperty.Register(nameof(Actions), typeof(ObservableCollection<AlarmAction>), typeof(AlarmWindow),
                new PropertyMetadata(null));

        public ObservableCollection<AlarmAction> Actions
        {
            get => (ObservableCollection<AlarmAction>)GetValue(ActionsProperty);
            set => SetValue(ActionsProperty, value);
        }

        // ── 타이틀바 동작 ────────────────────────────────
        private void Title_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();
    }

    // 버튼 한 개 정의
    public class AlarmAction
    {
        public string Text { get; set; }
        public ICommand Command { get; set; }
        public object CommandParameter { get; set; }
        public ActionKind Kind { get; set; } = ActionKind.Secondary;
        public bool IsDefault { get; set; } // Enter
        public bool IsCancel { get; set; } // Esc
    }

    public enum ActionKind { Primary, Secondary, Danger }

    public sealed class DelegateCommand : ICommand
    {
        private readonly Action<object> _exec;
        private readonly Func<object, bool> _can;
        public DelegateCommand(Action<object> exec, Func<object, bool> can = null)
        { _exec = exec ?? throw new ArgumentNullException(nameof(exec)); _can = can ?? (_ => true); }

        public bool CanExecute(object p) => _can(p);
        public void Execute(object p) => _exec(p);
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
