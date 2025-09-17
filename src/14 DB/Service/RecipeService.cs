

using Microsoft.EntityFrameworkCore;
using System.Windows.Documents;

namespace EGGPLANT
{
    public class RecipeService
    {
        private readonly IAppDbFactory _factory;   // Autofac에 SingleInstance도 OK
        public RecipeService(IAppDbFactory factory) => _factory = factory;

        public async Task SetActiveAsync(int recipeId, CancellationToken ct = default)
        {
            using var h = _factory.Create();       // child scope + DbContext
            var db = h.Db;

            await using var tx = await db.Database.BeginTransactionAsync(ct);

            await db.Recipes.Where(r => r.IsActive)
                            .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsActive, false), ct);

            var target = await db.Recipes.FindAsync([recipeId], ct)
                         ?? throw new InvalidOperationException("Recipe not found.");

            target.IsActive = true;
            await db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }

        public async Task<Recipe> CloneAsync(int sourceRecipeId, string? newName = null, CancellationToken ct = default)
        {
            using var h = _factory.Create();
            var _db = h.Db;
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            // 1) 원본 레시피 로드 (트래킹 없음)
            var source = await _db.Recipes
                .AsNoTracking()
                .SingleOrDefaultAsync(r => r.Id == sourceRecipeId, ct)
                ?? throw new InvalidOperationException("원본 레시피가 없습니다.");

            // 2) 새 레시피 생성 (Id/활성화는 초기화)
            var targetName = string.IsNullOrWhiteSpace(newName)
                ? await GenerateCopyNameAsync(source.Name, ct)  // 중복 회피
                : newName;

            var newRecipe = new Recipe
            {
                Name = targetName,
                IsActive = false,           // 활성은 기본 false
                                            // 필요한 다른 필드 복사
            };

            _db.Recipes.Add(newRecipe);
            await _db.SaveChangesAsync(ct); // Id 확보

            // 3) 원본 파라미터들 로드 → 새 복사본 구성 (FK는 새 RecipeId로)
            var ps = await _db.RecipeParams
                .AsNoTracking()
                .Where(p => p.RecipeId == sourceRecipeId)
                .Select(p => new RecipeParam
                {
                    RecipeId = newRecipe.Id,
                    Name = p.Name,
                    Value = p.Value,
                    Minimum = p.Minimum,
                    Maximum = p.Maximum,
                    ValueTypeId = p.ValueTypeId,
                    UnitId = p.UnitId,
                    Description = p.Description
                })
                .ToListAsync(ct);

            if (ps.Count > 0)
            {
                _db.RecipeParams.AddRange(ps);
                await _db.SaveChangesAsync(ct);
            }

            await tx.CommitAsync(ct);
            return newRecipe;
        }


        private async Task<string> GenerateCopyNameAsync(string baseName, CancellationToken ct)
        {
            using var h = _factory.Create();
            var _db = h.Db;
            var name = $"{baseName} - 복사";
            var i = 2;
            while (await _db.Recipes.AsNoTracking().AnyAsync(r => r.Name == name, ct))
                name = $"{baseName} - 복사 ({i++})";
            return name;
        }

    }
}
