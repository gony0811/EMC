namespace EGGPLANT.Models
{
    public enum PowerLampMode
    {
        STOP,       // 정지
        INIT,       // 초기화
        READY,      // 동작 준비
        START,      // 동작
        STOP_READY, // 정지 준비
        ALARM_READY,// 알람 준비
        ALARM       // 알람
    }
}
