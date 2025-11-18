using CommunityToolkit.Mvvm.ComponentModel;
using EMC.DB;
using EPFramework.IoC;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EMC
{
    [Service(Lifetime.Singleton)]
    public partial class DeviceManager : ObservableObject
    {
        private DeviceRepository deviceRepository;
        [ObservableProperty] private ObservableCollection<IDevice> deviceList = new ObservableCollection<IDevice>();
        private readonly Dictionary<string, IDevice> _deviceMap = new Dictionary<string, IDevice>();

        public DeviceManager(DeviceRepository deviceRepository)
        {
            this.deviceRepository = deviceRepository;
            _ = DeviceAttacth();
        }

        public async Task DeviceAttacth()
        {
            DeviceList.Clear();
            _deviceMap.Clear();

            var entityList = await deviceRepository.ListAsync();

            foreach(var entity in entityList)
            {
                var device = DeviceFactory.CreateRuntimeDevice(entity);
                
                if (device != null)
                {
                    DeviceList.Add(device);
                    _deviceMap[device.Name] = device;
                }
            }
        }

        public async Task AddDevice(IDevice device)
        {
            try
            {
                var entity = DeviceFactory.TypeCasting<Device>(device);
                await deviceRepository.AddAsync(entity);

                DeviceList.Add(entity);
                _deviceMap.Add(device.Name, device);

            }catch(Exception e)
            {
                throw e;
            }
            
        }
    }
}
