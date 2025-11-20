using EMC.DB;
using System.Collections.ObjectModel;

namespace EMC
{
    public interface IMotion 
    {
        int Id { get; set; }
        string Name { get; set; }
        int MotorNo { get; set; }
        //string ControlType { get; set; }

        double CurrentSpeed { get; set; }
        double LimitMinSpeed { get; set; }
        double LimitMaxSpeed { get; set; }

        double CurrentPosition { get; set; }           // 현재 위치
        double LimitMinPosition { get; set; }           // 최소 위치
        double LimitMaxPosition { get; set; }           // 최대 위치
        double EncoderCountPerUnit { get; set; }        // 모터의 단위당 엔코더 펄스 수

        IMotionDevice Device { get; set; }      // 부모 디바이스

        ObservableCollection<DMotionParameter> ParameterList { get; set; }
        ObservableCollection<DMotionPosition> PositionList { get; set; }
    }
}
