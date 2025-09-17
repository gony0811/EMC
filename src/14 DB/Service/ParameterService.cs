using Microsoft.EntityFrameworkCore;

namespace EGGPLANT
{
    public class ParameterService
    {
        private readonly IAppDbFactory _factory;   // Autofac에 SingleInstance도 OK
        public ParameterService(IAppDbFactory factory) => _factory = factory;
        public async Task<IReadOnlyList<RecipeParam>> GetParametersAsync(int recipeId, CancellationToken ct = default)
        {
            using var h = _factory.Create();
            var db = h.Db;

            return await db.RecipeParams
                .Where(p => p.RecipeId == recipeId)
                .Include(p => p.ValueType)
                .Include(p => p.Unit)
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .ToListAsync(ct);
        }


        
    }
}
