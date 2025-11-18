
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EMC.DB;
using EPFramework.IoC;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EMC
{
    [ViewModel(Lifetime.Singleton)]
    public partial class USub08ViewModel : ObservableObject
    {
        private readonly DeviceManager deviceManager;
        [ObservableProperty] private ObservableCollection<IDevice> deviceList;
        [ObservableProperty] private IDevice selectedDevice;

        public USub08ViewModel(DeviceManager deviceManager)
        {
            this.deviceManager = deviceManager;
            this.deviceList = deviceManager.DeviceList;
            this.SelectedDevice = deviceList.FirstOrDefault();
        }

        [RelayCommand]
        public async Task DeviceCreate()
        {
            var vm = new DeviceCreateViewModel();
            DeviceCreateModal modal = new DeviceCreateModal
            {
                DataContext = vm,
                Width = 400,
                Height = 600
            };
            var result = modal.ShowDialog();

            if(result == true)
            {
                var entity = new Device
                {
                    Name = vm.Name,
                    Type = vm.Type,
                    InstanceName = vm.File,
                    FileName = Path.GetFileName(vm.File),
                    IsUse = vm.IsUse,
                    Description = vm.Description,
                    Args = vm.SerializeArgs()
                };
                await deviceManager.AddDevice(entity);
            }
        }
    }
}
