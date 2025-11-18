
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EMC.DB;
using EPFramework.IoC;
using System.Threading.Tasks;

namespace EMC
{
    [ViewModel(Lifetime.Singleton)]
    public partial class USub08ViewModel : ObservableObject
    {
        private readonly DeviceManager deviceManager;

        public USub08ViewModel(DeviceManager deviceManager)
        {
            this.deviceManager = deviceManager;
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
                    InstanceName = vm.InstanceName,
                    FileName = vm.FileName,
                    IsUse = vm.IsUse,
                    Description = vm.Description,
                    Args = vm.SerializeArgs()
                };
                await deviceManager.AddDevice(entity);
            }
        }
    }
}
