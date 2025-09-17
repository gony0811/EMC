using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EGGPLANT
{
    public class ErrorService
    {
        private readonly IDbRepository<Error> _repo;

        public ErrorService(IDbRepository<Error> errorRepository)
        {
            _repo = errorRepository;
        }

        public event EventHandler? Changed
        {
            add => _repo.ChangeTrackerChanged += value;
            remove => _repo.ChangeTrackerChanged -= value;
        }
        public bool HasPendingChanges() => _repo.HasChanges();
        public Task DiscardChangesAsync(CancellationToken ct = default) => _repo.DiscardChangesAsync(ct);


        public Task AddAsync(Error e, CancellationToken ct = default)
            => _repo.AddAsync(e, ct);

        // 생성 (번호 중복 금지)
        public async Task SaveAsync(CancellationToken ct = default)
        {
            try
            {
                await _repo.SaveAsync(ct);
            }
            catch (DbUpdateException ex )
            {
                throw new DbUpdateException();
            }
            
        }

        // 단건 조회
        public async Task<Error?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _repo.FindAsync(ct, id);
        }

        // 목록 조회
        public async Task<IReadOnlyList<Error>> GetListAsync(CancellationToken ct = default)
        {
            return await _repo.ListAsync(orderBy: q => q.OrderBy(e => e.Number),asNoTracking: false,  ct: ct); ;
        }

        // 수정 (번호 중복 금지)
        public async Task<Error> UpdateAsync(Error error, CancellationToken ct = default)
        {
            if (error is null) throw new ArgumentNullException(nameof(error));

            // 1) 존재 확인 + 트래킹된 엔티티 로드 (충돌 방지)
            var dbEntity = await _repo.FindAsync(ct, error.Id);
            if (dbEntity is null)
                throw new KeyNotFoundException($"Id={error.Id} 인 에러가 존재하지 않습니다.");

            // 2) 번호 중복 검사 (자기 자신 제외)
            if (await IsNumberDuplicateAsync(error.Number, error.Id, ct))
                throw new ValidationException($"에러 번호 {error.Number} 는 이미 존재합니다.");

            // 3) 필드 복사 (Update로 새 인스턴스 붙이지 않고, 로드한 엔티티만 수정)
            dbEntity.Number = error.Number;
            dbEntity.Name = error.Name;
            dbEntity.Cause = error.Cause;
            dbEntity.Solution = error.Solution;
            dbEntity.BuzzerId = error.BuzzerId;

            await _repo.SaveAsync(ct);
            return dbEntity;
        }

        // 삭제
        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var ok = await _repo.Remove(id, ct);
            if (!ok) return false;

            await _repo.SaveAsync(ct);
            return true;
        }

       
        // 번호 중복 검사 (excludeId가 있으면 그 Id는 제외)
        public async Task<bool> IsNumberDuplicateAsync(int number, int? excludeId = null, CancellationToken ct = default)
        {
            if (excludeId.HasValue)
            {
                return await _repo.CountAsync(e => e.Number == number && e.Id != excludeId.Value, ct) > 0;
            }
            else
            {
                return await _repo.CountAsync(e => e.Number == number, ct) > 0;
            }
        }


    }
}
