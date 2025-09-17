using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace EGGPLANT
{
    public interface IDbRepository<T> where T : class
    {
        // PK(단일/복합 모두) 조회
        ValueTask<T?> FindAsync(CancellationToken ct = default, params object[] keyValues);

        // 목록 조회(필터/정렬/페이징)
        Task<IReadOnlyList<T>> ListAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            int? skip = null,
            int? take = null,
            bool asNoTracking = true,
            CancellationToken ct = default);

        // 단순 카운트
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default);
        
        // 쓰기
        Task AddAsync(T entity, CancellationToken ct = default);
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);
        void Update(T entity);
        Task<bool> Remove(int id, CancellationToken ct = default);
        Task<bool> RemoveByIdAsync(CancellationToken ct = default, params object[] keyValues);

        // 커밋 (Unit of Work)
        Task<int> SaveAsync(CancellationToken ct = default);
        
        Task DiscardChangesAsync(CancellationToken ct = default);
        EntityEntry Attach(T entity);

        event EventHandler? ChangeTrackerChanged;
        bool HasChanges();
    }
}
