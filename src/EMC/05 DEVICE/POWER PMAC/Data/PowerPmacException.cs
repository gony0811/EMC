using System;

namespace EMC
{
    public class PowerPmacException : Exception
    {
        public uint ErrorCode { get; }

        public PowerPmacException(string message, uint code = 0)
            : base(message)
        {
            ErrorCode = code;
        }
    }
}
