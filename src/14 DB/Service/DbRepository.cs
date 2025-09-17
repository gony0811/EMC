using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace EGGPLANT
{
    public class DbRepository<T> : IDbRepository<T> where T : class
    {
        private readonly AppDb _db;
        private readonly DbSet<T> _set;
        public event EventHandler? ChangeTrackerChanged;

        public DbRepository(AppDb db)
        {
            _db = db;
            _set = db.Set<T>();

            _db.ChangeTracker.Tracked += (_, e) =>
            {
                if (e.Entry.Entity is T) ChangeTrackerChanged?.Invoke(this, EventArgs.Empty);
            };
            _db.ChangeTracker.StateChanged += (_, e) =>
            {
                if (e.Entry.Entity is T) ChangeTrackerChanged?.Invoke(this, EventArgs.Empty);
            };
        }

        public ValueTask<T?> FindAsync(CancellationToken ct = default, params object[] keyValues)
            => _set.FindAsync(keyValues, ct);

        public async Task<IReadOnlyList<T>> ListAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            int? skip = null,
            int? take = null,
            bool asNoTracking = true,
            CancellationToken ct = default)
        {
            IQueryable<T> q = _set;
            if (asNoTracking) q = q.AsNoTracking();
            if (predicate != null) q = q.Where(predicate);
            if (orderBy != null) q = orderBy(q);
            if (skip.HasValue) q = q.Skip(skip.Value);
            if (take.HasValue) q = q.Take(take.Value);
            return await q.ToListAsync(ct);
        }

        public Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default)
            => (predicate == null ? _set.CountAsync(ct) : _set.CountAsync(predicate, ct));

        public Task AddAsync(T entity, CancellationToken ct = default)
        {
            _set.Add(entity);
            return Task.CompletedTask;
        }

        public Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default)
        {
            _set.AddRange(entities);
            return Task.CompletedTask;
        }

        public void Update(T entity)
        {
            var set = _db.Set<T>();
            var entry = _db.Entry(entity);

            // PK 이름 추출
            var key = _db.Model.FindEntityType(typeof(T))!.FindPrimaryKey()!.Properties[0].Name;
            var keyValue = entry.Property(key).CurrentValue;

            // Local에 같은 키를 가진 애가 있으면 분리
            var local = set.Local.FirstOrDefault(e =>
                Equals(_db.Entry(e).Property(key).CurrentValue, keyValue));

            if (local != null)
                _db.Entry(local).State = EntityState.Detached;

            // 네비게이션은 비우고(선택) FK만 유지한 상태여야 안전
            set.Attach(entity);
            _db.Entry(entity).State = EntityState.Modified;
        }

        public async Task<bool> Remove(int id, CancellationToken ct = default)
        {
            // 1) 이미 추적 중이면 그걸 삭제
            var local = _set.Local.FirstOrDefault(e => EF.Property<int>(e, "Id") == id);
            if (local is not null)
            {
                _set.Remove(local);
                return true;
            }

            // 2) DB에서 찾아서 삭제
            var entity = await _set.FindAsync(new object[] { id }, ct);
            if (entity is null) return false;

            _set.Remove(entity);
            return true;
        }

        public async Task<bool> RemoveByIdAsync(CancellationToken ct = default, params object[] keyValues)
        {
            var found = await _set.FindAsync(keyValues, ct);
            if (found is null) return false;
            _set.Remove(found);
            return true;
        }

        public bool HasChanges() => _db.ChangeTracker.Entries<T>()
       .Any(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted);

        public async Task DiscardChangesAsync(CancellationToken ct = default)
        {
            var entries = _db.ChangeTracker.Entries<T>().ToList();
            foreach (var e in entries)
            {
                switch (e.State)
                {
                    case EntityState.Added:
                        e.State = EntityState.Detached;
                        break;
                    case EntityState.Modified:
                    case EntityState.Deleted:
                        await e.ReloadAsync(ct);
                        break;
                }
            }
        }

        public EntityEntry Attach(T entity) => _db.Entry(entity);

        public Task<int> SaveAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
    }
}
