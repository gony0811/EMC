namespace EGGPLANT
{
    public class UserService 
    {
        private readonly IDbRepository<Role> _roleRepo;
        
        public UserService(IDbRepository<Role> roleRepo)
        {
            _roleRepo = roleRepo;
        }

        public async Task<Role?> GetRole(string RoleId, string password)
        {
            var roles = await _roleRepo.ListAsync();
            Role result = null;
            foreach(var role in roles )
            {
                if(role.Name.Equals(RoleId) && role.Password.Equals(password))
                {
                    result = role;
                    return result;
                }
            }
            return result;
        }

        public async Task<IReadOnlyList<Role?>> GetRoles(CancellationToken ct = default)
        {
            var result = await _roleRepo.ListAsync(ct:ct);
            return result;
        }
    }
}
