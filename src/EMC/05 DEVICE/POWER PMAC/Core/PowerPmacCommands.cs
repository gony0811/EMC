namespace EMC
{
    public static class PowerPmacCommands
    {

        #region Home 명령어
        public static PmacQueryCommand<MotorNumParam, bool> HomeDone = new PmacQueryCommand<MotorNumParam, bool>
        {
            Name = "HomeDone",
            Template = "Motor[{num}].HomeDone",
            Description = "해당 축의 Homing 동작이 완료되었는지 여부를 반환합니다. true이면 홈 위치 설정이 완료된 상태입니다.",
        };

        public static PmacQueryCommand<MotorNumParam, bool> HomeComplete = new PmacQueryCommand<MotorNumParam, bool>
        {
            Name = "HomeComplete",
            Template = "Motor[{num}].HomeComplete",
            Description = "지정된 모터의 Homing 시퀀스 완료 여부를 반환합니다. " +
            "true이면 Homing 절차가 완료되어 원점 위치가 설정된 상태입니다.",
        };
        #endregion

        #region 모터(양방향) 정보 요청
        public readonly static PmacQueryCommand<MotorNumParam, double> ActPos = new PmacQueryCommand<MotorNumParam, double>
        {
            Name = "ActPos",
            Template = "Motor[{num}].ActPos",
            Description = "해당 축의 현재 실제 위치(엔코더 피드백 위치)를 반환합니다. 단위는 설정된 스케일(기계 좌표계 기준)입니다.",
        };

        public readonly static PmacQueryCommand<MotorNumParam, double> DesPos = new PmacQueryCommand<MotorNumParam, double>
        {
            Name = "DesPos",
            Template = "Motor[{num}].DesPos",
            Description = "모터의 목표 위치(Desired Position)를 반환합니다. " +
            "제어 루프가 현재 추종하고 있는 명령 위치 값으로, 실시간 위치 오차 분석에 사용됩니다.",
        };

        public readonly static PmacQueryCommand<MotorNumParam, int> MotorStatus = new PmacQueryCommand<MotorNumParam, int>
        {
            Name = "MotorStatus",
            Template = "Motor[{num}].Status[0]",
            Description = "모터의 상태 비트(Status Word 0)를 반환합니다. " +
            "서보 온/오프, 에러, 리미트 스위치 등 여러 상태 플래그가 비트 단위로 포함되어 있습니다.",
        };

        public readonly static PmacQueryCommand<MotorNumParam, bool> AmpEna = new PmacQueryCommand<MotorNumParam, bool>
        {
            Name = "AmpEna",
            Template = "Motor[num].AmpEna",
            Description = "Servo Enable 모터 상태 비트의 12에 해당 (0: Disable, 1: Enable )"
        };

        public readonly static PmacQueryCommand<MotorNumParam, bool> ClosedLoop = new PmacQueryCommand<MotorNumParam, bool>
        {
            Name = "ClosedLoop",
            Template = "Motor[num].ClosedLoop",
            Description = "클로즈드 루프 모드 Status 비트" +
            "(0: Off, 1: On) " +
            "클로즈드 루프란? 모터의 실제 움직임을 센서로 피드백 받아서 제어하는 방식"


        };

        #endregion

        #region JOG 명령어 

        public readonly static PmacSendOnlyCommand<NumValue> JogSpeed = new PmacSendOnlyCommand<NumValue>
        {
            Name = "JogSpeed",
            Template = "Motor[{num}].JogSpeed={value}",
            Description = "해당 축을 +방향으로 Jog 이동시킵니다."
        };

        public readonly static PmacSendOnlyCommand<MotorNumParam> JogPlus = new PmacSendOnlyCommand<MotorNumParam>
        {
            Name = "JogPlus",
            Template = "#{num}j+",
            Description = "해당 축을 +방향으로 Jog 이동시킵니다."
        };

        // Jog -방향 (기본 속도)
        public readonly static PmacSendOnlyCommand<MotorNumParam> JogMinus = new PmacSendOnlyCommand<MotorNumParam>
        {
            Name = "JogMinus",
            Template = "#{num}j-",
            Description = "해당 축을 -방향으로 Jog 이동시킵니다."
        };

        // Jog 정지
        public readonly static PmacSendOnlyCommand<MotorNumParam> JogStop = new PmacSendOnlyCommand<MotorNumParam>
        {
            Name = "JogStop",
            Template = "#${num}j/",
            Description = "해당 축의 Jog 이동을 정지시킵니다."
        };
        #endregion

        #region Servo 명령어
        public readonly static PmacSendOnlyCommand<MotorNumParam> ServoOff = new PmacSendOnlyCommand<MotorNumParam>
        {
            Name = "ServoOff",
            Template = "#{num}k",
            Description = "모터를 출력 정지시킵니다."
        };
        #endregion
    }
}
