using System;

namespace EMC
{
    /// <summary>
    /// 명령 실행 결과 (Value + Type + 변환 헬퍼)
    /// </summary>
    public class PmacResult
    {
        public object Value { get; set; }
        public Type ValueType { get; set; }

        public PmacResult(object value, Type type)
        {
            Value = value;
            ValueType = type;
        }

    }
}
