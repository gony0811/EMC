using Autofac;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EGGPLANT.Device.PowerPmac;
using EGGPLANT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGGPLANT.ViewModels
{
    public partial class UInitializeViewModel: ObservableObject
    {
        [ObservableProperty]
        private int progress;

        [ObservableProperty]
        private string currentTitle;

        [ObservableProperty]
        private string status; // "진행 중...", "완료"

        public List<Step> InitialSequence { get; private set; }
        private CTrace traceMsg = null;

        public UInitializeViewModel()
        {
            Progress = 0;
            Status = "진행 중...";
            CurrentTitle = "초기화 중...";
            traceMsg = App.Container.ResolveKeyed<CTrace>("Trace");
            Init();
        }

        private void Init()
        {
            // 실행 순서대로 생성
            InitialSequence = new List<Step>
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

                    App.Container.Resolve<CErrorList>().Initialize("ERROR LIST");
                    await Task.Delay(150, ct);
                    await Task.CompletedTask;
                }),

                // STEP 3 - 레시피 관리자를 생성 
                new Step("레시피 관리자를 생성 중입니다.", async ct =>
                {
                    await Task.Delay(150, ct);
                    await Task.CompletedTask;
                }),

                // STEP 4 - 데이터 구조체 클래스를 생성
                new Step("장치 옵션 설정 중입니다.", async ct =>
                {
                    //App.Container.Resolve<CStructBase>().Initialize("STRUCT");
                    await Task.Delay(150, ct );
                    await Task.CompletedTask;
                }),            

                // STEP 5  - 모션 클래스를 생성
                new Step("MOTION CONTROL을 생성 중입니다.", async ct =>
                {
                    //App.Container.Resolve<CPmacMotion>().Initialize(5);

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
        }
    }
}
