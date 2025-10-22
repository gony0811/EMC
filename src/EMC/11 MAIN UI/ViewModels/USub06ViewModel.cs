using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EMC.DB;
using EPFramework.IoC;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EMC
{
    [ViewModel(Lifetime.Singleton)]
    public partial class USub06ViewModel : ObservableObject
    {
        private readonly MotionRepository _motionRepo;

        [ObservableProperty]
        private ObservableCollection<MotionDto> motions = new ObservableCollection<MotionDto>();
        
        [ObservableProperty] 
        private MotionDto selectedMotion;

        public USub06ViewModel(MotionRepository motionRepository)
        {
            this._motionRepo = motionRepository;
        }

        [RelayCommand]
        public async Task MotionListLoad(CancellationToken ct = default)
        {
            var entities = await _motionRepo.ListAsync(
                orderBy: q => q.OrderBy(m => m.Axis).ThenBy(m => m.Name),
                include: q => q.Include(m => m.Positions),   
                ct: ct);

            Motions.Clear();
            foreach (var dto in entities) Motions.Add(MotionDto.FromEntity(dto));
        }
    }
}
