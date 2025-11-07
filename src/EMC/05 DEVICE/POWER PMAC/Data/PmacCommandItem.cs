

namespace EMC
{
    public class PmacCommandItem
    {
        public string Key { get; set; }
        public IPmacCommandBase Command { get; set; }
        public object Parameter { get; set; }

        public PmacCommandItem(string key, IPmacCommandBase command, object parameter)
        {
            Key = key;
            Command = command;
            Parameter = parameter;
        }
    }

}
