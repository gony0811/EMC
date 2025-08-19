using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace EGGPLANT.ViewModels
{
    /** 에러 목록과 관련된 뷰모델 정의 클래스
     *  에러 번호 및 내용, 원인과 조치 방법에 대해 메모
     *  정의된 에러가 발생시 부저가 발생하며 부저음을 설정할 수 있음
     * **/
    public partial class USub05ViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Error> errorList = new ObservableCollection<Error>();

        [ObservableProperty]
        private ObservableCollection<Buzzer> buzzerList = new ObservableCollection<Buzzer>();

        [ObservableProperty] private Error selectedError;

        public USub05ViewModel()
        {
            // 더미 데이터
            var buzzer1 = new Buzzer(id: 1, name: "BUZZER #1");
            var buzzer2 = new Buzzer(id: 2, name: "BUZZER #2");
            BuzzerList.Add(buzzer1);
            BuzzerList.Add(buzzer2);

            ErrorList.Add(new Error { Id = 100, ErrorContents = "모터 과열", Cause = "냉각팬 정지/통풍 불량", Solution = "팬 교체 및 통풍 경로 점검", UseBuzzer = buzzer1 });
            ErrorList.Add(new Error { Id = 101, ErrorContents = "센서 단선", Cause = "커넥터 이탈/케이블 단선", Solution = "배선 재결선 및 고정", UseBuzzer = buzzer1 });
            ErrorList.Add(new Error { Id = 102, ErrorContents = "비상정지 감지", Cause = "E-STOP 버튼 눌림", Solution = "원인 제거 후 리셋", UseBuzzer = buzzer1 });
            ErrorList.Add(new Error { Id = 103, ErrorContents = "도어 오픈", Cause = "안전도어 미폐쇄", Solution = "도어 닫고 인터락 확인", UseBuzzer = buzzer2 });
            ErrorList.Add(new Error { Id = 104, ErrorContents = "공압 저압", Cause = "메인 레귤레이터 압력 저하", Solution = "압력 세팅/누기 점검", UseBuzzer = buzzer1 });
            ErrorList.Add(new Error { Id = 105, ErrorContents = "서보 과전류", Cause = "부하 과대/축 걸림", Solution = "기구 이물 제거 및 토크 설정 조정", UseBuzzer = buzzer1 });
            ErrorList.Add(new Error { Id = 106, ErrorContents = "엔코더 이상", Cause = "엔코더 신호 소실", Solution = "커넥터 재체결/엔코더 교체", UseBuzzer = buzzer2 });
            ErrorList.Add(new Error { Id = 107, ErrorContents = "통신 타임아웃", Cause = "네트워크 지연/케이블 불량", Solution = "스위치/케이블 점검 및 재기동", UseBuzzer = buzzer2 });
            ErrorList.Add(new Error { Id = 108, ErrorContents = "리미트 스위치 감지", Cause = "축 한계 넘김", Solution = "원점 복귀 및 한계값 재설정", UseBuzzer = buzzer1 });
            ErrorList.Add(new Error { Id = 109, ErrorContents = "윤활유 부족", Cause = "오일 레벨 저하", Solution = "윤활유 보충 및 누유 점검", UseBuzzer = buzzer2 });
        }
    }

    // Error 클래스 정의
    public partial class Error : ObservableObject
    {
        [ObservableProperty]
        private int id;   // 에러 번호 ( 식별자 ) 

        [ObservableProperty]
        private string errorContents;   // 에러 내용

        [ObservableProperty]
        private string cause; // 에러 원인

        [ObservableProperty]
        private string solution;    // 해결 방법

        [ObservableProperty]
        private Buzzer useBuzzer;

        public bool Matches(int id)
        {
            if (this.Id == id)
            {
                return true;
            }
            return false;
        }
    }

    public partial class Buzzer : ObservableObject
    {
        [ObservableProperty]
        private int id;     // 식별자

        [ObservableProperty]
        private string name;

        public Buzzer(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }
}
