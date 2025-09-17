namespace EGGPLANT
{
    public class Authority
    {
        public int Id { get;  }
        public string Name { get; }
        public string Description { get; }

        public Authority()
        {

        }
        private Authority(int id, string name , string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        public static Authority of(Role role)
        {
            return new Authority(id: role.Id, name: role.Name, description: role.Description);
        }
    }
}
