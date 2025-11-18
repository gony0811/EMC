using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMC.DB
{

    public class Device : IDevice
    {
        [Key]
        public int Id { get; private set; }

        [Required]
        [Index(nameof(Name), IsUnique = true)]
        public string Name { get; set; }
        public DeviceType Type { get; set; }
        public string InstanceName { get; set; }
        public string FileName { get; set; }
        public bool IsUse { get; set; }
        public string Args { get; set; }
        public string Description { get; set; }

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
