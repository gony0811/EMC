using System.Windows;
namespace EGGPLANT
{
    /// <summary>
    /// ParamterCreateWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ParameterCreateWindow : Window
    {
        public ParameterCreateWindow(ParameterCreateVM vm)
        {
            InitializeComponent();
            DataContext = vm;

            vm.RequestClose += OnRequestClose;          // ← 구독
            Loaded += async (_, __) => await vm.InitAsync();
            Closed += (_, __) => vm.RequestClose -= OnRequestClose; // ← 해제
        }

        private void OnRequestClose(bool ok)
        {
            Dispatcher.Invoke(() =>                   // UI 스레드 보장
            {
                DialogResult = ok;                   // ShowDialog()일 때 설정하면 자동 Close
                Close();
            });
        }
    }

}
