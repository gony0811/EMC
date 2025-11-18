

namespace EMC
{
    public interface IDeviceArgs { }
    public class MotionArgs : IDeviceArgs
    {
        public string Ip { get; set; }
        public int Port { get; set; }
    }
}
