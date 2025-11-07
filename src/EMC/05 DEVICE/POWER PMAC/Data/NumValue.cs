namespace EMC
{
    public class NumValue
    {
        

        public int Num { get; set; }
        public double Value { get; set; }

        public NumValue()
        {
        }

        public NumValue(int num, double value)
        {
            Num = num;
            Value = value;
        }
    }
}
