using CommunityToolkit.Mvvm.ComponentModel;
using EMC.DB;

namespace EMC
{
    public partial class PowerPmacDevice : ObservableObject, IDevice
    {
        public int Id { get; set; }
        [ObservableProperty] public string name;
        [ObservableProperty] public DeviceType type;
        [ObservableProperty] public string instanceName;
        [ObservableProperty] public string fileName;
        [ObservableProperty] public string isUse;
        [ObservableProperty] public string args;
        [ObservableProperty] public string description;

        public bool GetBool(string name, out bool result)
        {
            throw new System.NotImplementedException();
        }

        public double GetDouble(string name, out double result)
        {
            throw new System.NotImplementedException();
        }

        public int GetInt(string name, out int result)
        {
            throw new System.NotImplementedException();
        }

        public object GetObject(string name, out object result)
        {
            throw new System.NotImplementedException();
        }

        public string GetString(string name, out string result)
        {
            throw new System.NotImplementedException();
        }

        public bool SetData(string name, object data)
        {
            throw new System.NotImplementedException();
        }
    }
}
