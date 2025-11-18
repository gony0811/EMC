namespace EMC.DB
{
    public interface IDevice
    {
        int Id { get; }
        string Name { get; set; }
        DeviceType Type { get; set; }
        string InstanceName { get; set; }
        string FileName { get; set; }
        bool IsUse { get; set; }
        string Args { get; set; }
        string Description { get; set; }

        bool SetData(string name, object data);
        bool GetBool(string name, out bool result);
        string GetString(string name, out string result);
        int GetInt(string name, out int result);
        double GetDouble(string name, out double result);
        object GetObject(string name, out object result);
    }
}
