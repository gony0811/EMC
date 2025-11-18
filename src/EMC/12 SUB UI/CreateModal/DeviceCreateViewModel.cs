using CommunityToolkit.Mvvm.ComponentModel;
using EMC.DB;
using Newtonsoft.Json;
using PropertyTools.DataAnnotations;

namespace EMC
{
    public partial class DeviceCreateViewModel : ObservableObject
    {

        [Browsable(false)] public int Id { get; }

        [ObservableProperty] private string name;
        [ObservableProperty] private DeviceType type;

        [property: InputFilePath]
        [ObservableProperty] private string file;

        [ObservableProperty] private bool isUse;
        [ObservableProperty] private string description;

        // Args 모델(타입별 설정)
        [Browsable(false)]
        [ObservableProperty] private IDeviceArgs argsModel;

        public DeviceCreateViewModel()
        {
            Type = DeviceType.PowerPmac;
            ArgsModel = CreateArgsModel(Type);
        }

        partial void OnTypeChanged(DeviceType value)
        {
            ArgsModel = CreateArgsModel(value);
        }

        private IDeviceArgs CreateArgsModel(DeviceType type)
        {
            return type switch
            {
                DeviceType.PowerPmac => new MotionArgs(),
                DeviceType.Pmac => new MotionArgs(),
                _ => null
            };
        }
        public string SerializeArgs()
        {
            return JsonConvert.SerializeObject(ArgsModel, Formatting.Indented);
        }
    }
}
