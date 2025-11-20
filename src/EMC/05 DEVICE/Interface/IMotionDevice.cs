using EMC.DB;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Media;

namespace EMC
{
    public interface IMotionDevice : IDevice
    {
        string Ip { get; set; }
        int Port { get; set; }
        MotionDeviceType MotionDeviceType { get; set; }

        ObservableCollection<IMotion> MotionList { get; }
        IMotion FindMotionById(int id);
        IMotion FindMotionByName(string name);

        Task<TResult> Move<TResult>(int motionId, string commands, Dictionary<string, object> parameters);    // 절대값 이동
        void JogMove(int motionId, double jogSpeed, JogMoveType moveType);   // 조그 이동

        Task<TResult> Home<TResult>();


    }
}
