using EPFramework.DB;
using EPFramework.IoC;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EMC.DB
{
    [Service(Lifetime.Scoped)]
    public class ParameterRepository : DbRepository<RecipeParam, AppDb> 
    {
        public ParameterRepository(AppDb db) : base(db)
        {
        }

        public async Task<IReadOnlyList<RecipeParam>> GetParametersAsync(int recipeId, CancellationToken ct = default(CancellationToken))
        {
            
            return await _set
                .Where(p => p.RecipeId == recipeId)
                .Include(p => p.ValueType)
                .Include(p => p.UnitType)
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .ToListAsync(ct)
                .ConfigureAwait(false);
        }
    }
}
