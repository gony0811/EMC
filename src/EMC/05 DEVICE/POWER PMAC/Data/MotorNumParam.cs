namespace EMC
{
    public class MotorNumParam : IPmacCommandParams
    {
        public int Num { get; set; }

        public MotorNumParam()
        {
        }

        public MotorNumParam(int num)
        {
            Num = num;
        }
    }
}
