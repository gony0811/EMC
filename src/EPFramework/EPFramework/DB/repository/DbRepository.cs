using EPFramework.IoC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace EPFramework.DB
{
    public abstract class DbRepository<TEntity, TContext> : IDbRepository<TEntity> 
        where TEntity : class
        where TContext : DbContext
    {
        protected readonly TContext _context;
        protected readonly DbSet<TEntity> _set;

        public event EventHandler ChangeTrackerChanged;

        protected DbRepository(TContext context)
        {
            _context = context;
            _set = context.Set<TEntity>();

            _context.ChangeTracker.Tracked += (sender, e) =>
            {
                if (e.Entry != null && e.Entry.Entity is TEntity)
                    RaiseChangeTrackerChanged();
            };

            _context.ChangeTracker.StateChanged += (sender, e) =>
            {
                if (e.Entry != null && e.Entry.Entity is TEntity)
                    RaiseChangeTrackerChanged();
            };
        }

        private void RaiseChangeTrackerChanged()
        {
            ChangeTrackerChanged?.Invoke(this, EventArgs.Empty);
        }

        public Task<TEntity> FindAsync(CancellationToken ct = default, params object[] keyValues)
        {
            return _set.FindAsync(keyValues, ct).AsTask();
        }

        public async Task<IReadOnlyList<TEntity>> ListAsync(
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            bool asNoTracking = true,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> include = null,
            CancellationToken ct = default)
        {
            IQueryable<TEntity> q = _set;

            if (asNoTracking) q = q.AsNoTracking();
            if (predicate != null) q = q.Where(predicate);
            if (include != null) q = include(q);
            if (orderBy != null) q = orderBy(q);
            if (skip.HasValue) q = q.Skip(skip.Value);
            if (take.HasValue) q = q.Take(take.Value);

            return await q.ToListAsync(ct).ConfigureAwait(false);
        }

        public Task<int> CountAsync(
            Expression<Func<TEntity, bool>> predicate = null,
            CancellationToken ct = default)
        {
            return predicate == null 
                ? _set.CountAsync(ct) 
                : _set.CountAsync(predicate, ct);
        }

        public Task AddAsync(TEntity entity, CancellationToken ct = default)
        {
            _set.Add(entity);
            return SaveAsync(ct);
        }

        public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default)
        {
            _set.AddRange(entities);
            return SaveAsync(ct);
        }

        public async Task Update(TEntity entity, CancellationToken ct = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // 엔티티 메타데이터에서 PK 정보 얻기
            var et = _context.Model.FindEntityType(typeof(TEntity))
                ?? throw new InvalidOperationException($"EntityType metadata not found for {typeof(TEntity).Name}");
            var pk = et.FindPrimaryKey()
                ?? throw new InvalidOperationException($"Primary key not found for {typeof(TEntity).Name}");
            var keyName = pk.Properties[0].Name;

            // 현재 PK 값 추출
            var entry = _context.Entry(entity);
            var keyValue = entry.Property(keyName).CurrentValue;

            // Local 캐시에 동일 PK 엔티티가 있으면 Detach
            var local = _set.Local.FirstOrDefault(e =>
            {
                var localEntry = _context.Entry(e);
                return Equals(localEntry.Property(keyName).CurrentValue, keyValue);
            });

            if (local != null)
                _context.Entry(local).State = EntityState.Detached;

            // 새 엔티티를 Attach + Modified 처리
            _set.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;

            await SaveAsync(ct);
        }


        public async Task UpdateRange(IEnumerable<TEntity> entities, CancellationToken ct = default)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            // PK 메타정보 미리 캐싱 (루프마다 FindEntityType 호출 X)
            var et = _context.Model.FindEntityType(typeof(TEntity))
                ?? throw new InvalidOperationException($"EntityType metadata not found for {typeof(TEntity).Name}");
            var pk = et.FindPrimaryKey()
                ?? throw new InvalidOperationException($"Primary key not found for {typeof(TEntity).Name}");
            var keyName = pk.Properties[0].Name;

            foreach (var entity in entities)
            {
                var entry = _context.Entry(entity);
                var keyValue = entry.Property(keyName).CurrentValue;

                // Local 캐시에 동일 PK 엔티티가 있으면 Detach
                var local = _set.Local.FirstOrDefault(e =>
                {
                    var localEntry = _context.Entry(e);
                    return Equals(localEntry.Property(keyName).CurrentValue, keyValue);
                });

                if (local != null)
                    _context.Entry(local).State = EntityState.Detached;

                // Attach 및 Modified 설정
                _set.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }

            await SaveAsync(ct);
        }



        public async Task<bool> Remove(int id, CancellationToken ct = default)
        {
            var local = _set.Local.FirstOrDefault(e =>
                EF.Property<int>(e, "Id") == id);
            if (local != null)
            {
                _set.Remove(local);
                await SaveAsync(ct);
                return true;
            }

            var entity = await _set.FindAsync(new object[] { id }, ct).ConfigureAwait(false);
            if (entity == null) return false;

            _set.Remove(entity);
            await SaveAsync(ct);
            return true;
        }

        public async Task<bool> RemoveByIdAsync(CancellationToken ct = default, params object[] keyValues)
        {
            var found = await _set.FindAsync(keyValues, ct).ConfigureAwait(false);
            if (found == null) return false;

            _set.Remove(found);
            await SaveAsync(ct);
            return true;
        }

        public bool HasChanges()
        {
            return _context.ChangeTracker
                          .Entries<TEntity>()
                          .Any(e => e.State == EntityState.Added
                                 || e.State == EntityState.Modified
                                 || e.State == EntityState.Deleted);
        }

        public async Task DiscardChangesAsync(CancellationToken ct = default)
        {
            var entries = _context.ChangeTracker.Entries<TEntity>().ToList();

            foreach (var e in entries)
            {
                if (e.State == EntityState.Added)
                {
                    e.State = EntityState.Detached;
                }
                else if (e.State == EntityState.Modified || e.State == EntityState.Deleted)
                {
                    await e.ReloadAsync(ct).ConfigureAwait(false);
                }
            }
        }

        public EntityEntry Attach(TEntity entity)
        {
            return _context.Entry(entity);
        }

        public Task<int> SaveAsync(CancellationToken ct = default)
        {
            return _context.SaveChangesAsync(ct);
        }
    }
}
