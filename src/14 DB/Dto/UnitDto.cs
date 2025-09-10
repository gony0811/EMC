namespace EGGPLANT
{
    public class UnitDto
    {
        public int UnitId { get; }
        public string Name { get; }
        public string Symbol { get; }

        public UnitDto(int unitId, string name, string symbol)
        {
            UnitId = unitId;
            Name = name;
            Symbol = symbol;
        }

    }
}
