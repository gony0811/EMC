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
            var createSteps = new List<Step>
            {
                // STEP 1 - LOG IN / LANGUAGE 초기화
                new Step("LOG IN / LANGUAGE 초기화", async ct =>
                {

                    await Task.CompletedTask;
                }),

                // STEP 2  - ERROR, PRODUCT HISTORY 클래스를 생성
                new Step("ERROR, PRODUCT HISTORY 클래스를 생성 중입니다.", async ct =>
                {
                    //CMIT.ProductHistory = new CProductHistory("PRODUCT HISOTRY");
                    //CMIT.ErrorList = new CErrorList("ERROR LIST");
                    await Task.Delay(150, ct);
                    await Task.CompletedTask;
                }),

                // STEP 3 - 디바이스 클래스를 생성 
                new Step("디바이스 클래스를 생성 중입니다.", async ct =>
                {
                    await Task.Delay(150, ct);
                    await Task.CompletedTask;
                }),

                // STEP 4 - 데이터 구조체 클래스를 생성
                new Step("데이터 구조체 클래스를 생성 중입니다.", async ct =>
                {
                    await Task.Delay(150, ct );
                    await Task.CompletedTask;
                }),

                // STEP 5  - HoPe TRACE 클래스를 생성
                new Step("MOTION 클래스를 생성 중입니다.", async ct =>
                {
                    await Task.CompletedTask;
                    await Task.Delay(150, ct);
                }),

                // STEP 6
                new Step("DIGITAL IO 클래스를 생성 중입니다.", async ct =>
                {
                    await Task.CompletedTask;
                    await Task.Delay(150, ct);
                }),

                // STEP 7
                new Step("MULTI CAMERA, EVISION 클래스를 생성 중입니다.", async ct =>
                {
                    await Task.CompletedTask;
                    await Task.Delay(150, ct);
                }),

                // STEP 8
                new Step("LAMP 클래스를 생성 중입니다.", async ct =>
                {
                    await Task.CompletedTask;
                    await Task.Delay(150, ct);
                }),

                // STEP 9
                new Step("TEN-KEY 클래스를 생성 중입니다.", async ct =>
                {
                    await Task.CompletedTask;
                    await Task.Delay(150, ct);
                }),

                // STEP 10
                new Step("LDS 클래스를 생성 중입니다.", async ct =>
                {
                    await Task.CompletedTask;
                    await Task.Delay(150, ct);
                }),

                // STEP 11
                new Step("REPORT LOG 클래스를 생성 중입니다.", async ct =>
                {
                    await Task.CompletedTask;
                    await Task.Delay(150, ct);
                }),

                // STEP 12
                new Step("SAMPLE PROC 클래스를 생성 중입니다.", async ct =>
                {
                    await Task.CompletedTask;
                    await Task.Delay(150, ct);
                }),

                // STEP 13
                new Step("WORK PROC 클래스를 생성 중입니다.", async ct =>
                {
                    await Task.CompletedTask;
                    await Task.Delay(150, ct);
                }),
                 // STEP 14
                new Step("EXECUTE 클래스를 생성 중입니다.", async ct =>
                {
                    await Task.CompletedTask;
                    await Task.Delay(150, ct);
                }),

                 // STEP 15
                new Step("VISION EXECUTE #1, #2 클래스를 생성 중입니다.", async ct =>
                {
                    await Task.CompletedTask;
                    await Task.Delay(150, ct);
                }),

                 // STEP 16
                new Step("최근 작업 디바이스 정보를 호출합니다.", async ct =>
                {
                    await Task.CompletedTask;
                    await Task.Delay(150, ct);
                }),

                 // STEP 17
                new Step("THREAD 클래스를 생성 중입니다.", async ct =>
                {
                    await Task.CompletedTask;
                    await Task.Delay(150, ct);
                }),

                 // STEP 18
                new Step("THREAD, OT-TIMER를 구동합니다.", async ct =>
                {
                    await Task.CompletedTask;
                    await Task.Delay(150, ct);
                }),
            };

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
                var pipeline = new StartupPipeline(createSteps, ui);
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
