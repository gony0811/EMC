using Autofac;
using EGGPLANT.Models;
using EGGPLANT.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EGGPLANT
{
    
    public partial class UInitialize : Window
    {
        public UInitializeViewModel VM { get; } = new UInitializeViewModel();
        private CancellationTokenSource _cts;

        public UInitialize()
        {
            InitializeComponent();
            DataContext = VM;

            Loaded += Initialize;
        }

        // 초기화
        private async void Initialize(object sender, EventArgs e)
        {
            _cts = new CancellationTokenSource();
            // 실행 순서대로 생성     

            // 2) UI 업데이트
            var ui = new Progress<UiStatus>(s =>
            {
                VM.CurrentTitle = s.Title;
                VM.Status = s.Status;
                VM.Progress = s.Progress;
            });

            // 3) StartupPipeline 실행
            try
            {
                var pipeline = new StartupPipeline(VM.InitialSequence, ui);
                bool ok = await pipeline.RunAsync(_cts.Token);

                if (ok)
                {
                    VM.Status = "모든 시스템이 준비되었습니다.";
                    DialogResult = true;
                    Close();
                }
            }
            catch (OperationCanceledException)
            {
                VM.Status = "취소됨";
                DialogResult = false;
                Close();
            }
            catch (Exception ex)
            {
                VM.Status = "오류: " + ex.Message;
                //MessageBox.Show(ex.ToString(), "초기화 오류", MessageBoxButton.OK, MessageBoxImage.Error);
                DialogResult = false;
                Close();
            }
        }
    }
}
