using System.Text;

namespace EMC
{
    // 단방향 명령 전송 클래스 
    public class PmacSendOnlyCommand<TParams> : IPmacCommandBase
    {
        public string Name { get; set; }
        public string Template { get; set; }
        public string Description { get; set; }

        public void Execute(uint deviceId, TParams parameters)
        {
            string cmd = Build(parameters);
            byte[] bytes = Encoding.UTF8.GetBytes(cmd + "\r");

            uint ret = DTKPowerPmac.Instance.SendCommandA(deviceId, bytes);
            if (ret != 0)
                throw new PowerPmacException($"[{Name}] SendCommand 실패", ret);
        }

        public string Build(TParams p)
        {
            string cmd = Template;
            foreach (var prop in typeof(TParams).GetProperties())
                cmd = cmd.Replace($"{{{prop.Name.ToLower()}}}", prop.GetValue(p)?.ToString() ?? "");
            return cmd;
        }
    }
}
