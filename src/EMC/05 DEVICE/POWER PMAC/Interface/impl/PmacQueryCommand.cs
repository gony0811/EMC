using EMC;
using System;
using System.Text;

public class PmacQueryCommand<TParams, TResponse> : IPmacCommandBase
{
    public string Name { get; set; }
    public string Template { get; set; }
    public string Description { get; set; }

    // 선택적 변환기
    public Func<string, TResponse> Converter { get; set; }

    public TResponse Execute(uint deviceId, TParams parameters)
    {
        if (deviceId == 0)
            throw new PowerPmacException("Power PMAC 장치가 연결되지 않았습니다.");

        string cmd = Build(parameters);
        byte[] bytes = Encoding.UTF8.GetBytes(cmd + "\r");
        byte[] resp = new byte[1024];

        uint ret = DTKPowerPmac.Instance.GetResponseA(deviceId, bytes, resp, resp.Length);
        if (ret != 0)
            throw new PowerPmacException($"[{Name}] GetResponse 실패", ret);

        string text = Encoding.UTF8.GetString(resp).Trim('\0', '\r', '\n', '>');

        // Converter 지정되어 있으면 우선 사용
        if (Converter != null)
            return Converter(text);

        // 지정되지 않으면 자동 변환
        return DefaultConvert(text);
    }

    public string Build(TParams p)
    {
        string cmd = Template;
        foreach (var prop in typeof(TParams).GetProperties())
            cmd = cmd.Replace($"{{{prop.Name.ToLower()}}}", prop.GetValue(p)?.ToString() ?? "");
        return cmd;
    }

    private TResponse DefaultConvert(string s)
    {
        object result = null;
        Type t = typeof(TResponse);

        if (t == typeof(string)) result = s;
        else if (t == typeof(int)) result = int.TryParse(s, out var i) ? i : 0;
        else if (t == typeof(double)) result = double.TryParse(s, out var d) ? d : 0.0;
        else if (t == typeof(bool))
        {
            s = s.Trim().ToLower();
            result = (s == "1" || s == "true" || s == "on");
        }
        else
        {
            throw new PowerPmacException($"[{Name}] 변환 불가 타입: {t.Name}");
        }

        return (TResponse)result;
    }
}
