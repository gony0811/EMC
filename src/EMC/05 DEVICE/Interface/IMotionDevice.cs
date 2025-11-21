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
        IMotion FindMotionByMotorIndex(int mIndex);
        IMotion FindMotionByName(string name);
    }
}
