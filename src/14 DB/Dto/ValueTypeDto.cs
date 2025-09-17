namespace EGGPLANT
{
    public class ValueTypeDto
    {
        public int ValueTypeId { get; }
        public string Name { get; }
        public ValueTypeDto(int valueTypeId, string name)
        {
            ValueTypeId = valueTypeId;
            Name = name;
        }
    }
}
