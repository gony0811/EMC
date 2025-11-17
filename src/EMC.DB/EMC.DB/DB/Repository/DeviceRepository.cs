using EPFramework.DB;

namespace EMC.DB
{
    public class DeviceRepository : DbRepository<Device, AppDb>
    {
        public DeviceRepository(AppDb context) : base(context)
        {
        }
    }
}
