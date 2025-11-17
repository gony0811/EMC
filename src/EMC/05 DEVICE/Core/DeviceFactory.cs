using EMC.DB;

namespace EMC
{
    public static class DeviceFactory
    {
        public static IDevice CreateRuntimeDevice(Device entity)
        {
            if (entity == null)
                return null;

            IDevice device = null;

            switch (entity.Type)
            {
                case DeviceType.PowerPmac:
                    device = CreatePowerPmacDevice(entity);
                    break;
                default:
                    return null;
            }

            return device;
        }

        public static T TypeCasting<T>(IDevice device) where T: class, IDevice
        {
            return device as T;
        }

        public static PowerPmacDevice CreatePowerPmacDevice(Device entity)
        {
            var runtime = new PowerPmacDevice
            {
                Id = entity.Id,
                Name = entity.Name,
                Type = entity.Type,
                FileName = entity.FileName,
                InstanceName = entity.InstanceName,
                Description = entity.Description,
                IsUse = entity.IsUse,
                Args = entity.Args,
            };

            return runtime;
        }
    }
}
