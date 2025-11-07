using CommunityToolkit.Mvvm.ComponentModel;
using EMC.DB;
using EPFramework.IoC;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace EMC
{
    [Service(Lifetime.Singleton)]
    public partial class PowerPmacManager : ObservableObject
    {
        private readonly PowerPMacRepository powerPMacRepository;
        private readonly PowerPMacMotionRepository powerPMacMotionRepository;
        public ObservableCollection<DPowerPmac> PowerPMacList { get; } = new ObservableCollection<DPowerPmac>();

        public PowerPmacManager(PowerPMacRepository repository, PowerPMacMotionRepository ppmRepository)
        {
            powerPMacRepository = repository;
            powerPMacMotionRepository = ppmRepository;
            _ = LoadAsync();
        }

        private async Task LoadAsync()
        {
            var lst = await powerPMacRepository.ListAsync(include: q => q.Include(x => x.MotionList));
            foreach (var item in lst)
            {
                PowerPMacList.Add(item.ToDto());
            }
        }

        public async Task Create(DPowerPmac pmac)
        {
            PowerPMac powerPMac = pmac.ToEntity();
            try
            {
                var entity = await powerPMacRepository.AddAsync(powerPMac);
                PowerPMacList.Add(entity.ToDto());
            }
            catch (Exception e)
            {
                MessageBox.Show("저장 오류");
            }
        }

        public async Task MotionSave(DPowerPmacMotion motion, CancellationToken ct = default)
        {
            try
            {
                PowerPMacMotion savedEntity;
                var entity = motion.ToEntity();

                if (motion.Id == 0)
                {
                    savedEntity = await powerPMacMotionRepository.AddAsync(entity, ct);

                }
                else
                {
                    savedEntity = await powerPMacMotionRepository.Update(entity, ct);
                }

                if (savedEntity != null)
                {
                    motion.CopyFrom(savedEntity);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("저장실패");
            }
        }

        public bool FindMotion(string motorName, out DPowerPmacMotion motion, int deviceId = 1)
        {
            motion = PowerPMacList
                .FirstOrDefault(x => x.Id == deviceId)?
                .MotionList.FirstOrDefault(x => x.Name == motorName);

            return motion != null;
        }
    }

}
