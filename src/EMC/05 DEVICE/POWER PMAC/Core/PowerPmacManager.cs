using CommunityToolkit.Mvvm.ComponentModel;
using EMC.DB;
using EPFramework.IoC;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace EMC
{
    [Service(Lifetime.Singleton)]
    public partial class PowerPmacManager : DeviceManager<PowerPmacDevice, PowerPmacMotion>
    {

        private readonly MotionParameterRepository _motionParameterRepository;
        private readonly MotionPositionRepository _motionPositionRepository;

        public PowerPmacManager(DeviceRepository deviceRepository, MotionRepository motionRepository, MotionParameterRepository motionParameterRepository, MotionPositionRepository motionPositionRepository) 
            : base(DeviceType.PowerPmac, deviceRepository, motionRepository, motionParameterRepository, motionPositionRepository)
        {
            _motionParameterRepository = motionParameterRepository;
            _motionPositionRepository = motionPositionRepository;
            _ = LoadAsync();
        }
        public PowerPmacDevice FindDevice(int id = 1)
        {
            if (DeviceList == null || DeviceList.Count == 0)
                return null;

            return DeviceList.FirstOrDefault(d => d.Id == id);
        }

        public PowerPmacMotion FindMotion(int motionId, int deviceId = 1)
        {
            var device = FindDevice(deviceId);
            if (device == null || device.MotionList == null || device.MotionList.Count == 0)
                return null;

            return device.MotionList.FirstOrDefault(m => m.Id == motionId);
        }

        public PowerPmacMotion FindMotion(string motionName, int deviceId = 1)
        {
            var device = FindDevice(deviceId);
            if (device == null || device.MotionList == null || device.MotionList.Count == 0)
                return null;

            return device.MotionList.FirstOrDefault(m => m.Name == motionName);
        }

        public async Task<DMotionPosition> CreatePosition(DMotionPosition position)
        {
            MotionPosition mp = new MotionPosition
            {
                Name = position.Name,
                Location = position.Location,
                MaximumLocation = position.MaximumLocation,
                MinimumLocation = position.MinimumLocation,
                Speed = position.Speed,
                MaximumSpeed = position.MaximumSpeed,
                MinimumSpeed = position.MinimumSpeed,
                MotionId = position.ParentMotionId,
            };
            
            var Result = await _motionPositionRepository.AddAsync(mp);

            return new DMotionPosition
            {
                Id = Result.Id,
                Name = Result.Name,
                Location = Result.Location,
                MaximumLocation = Result.MaximumLocation,
                MinimumLocation = Result.MinimumLocation,

                Speed = Result.Speed,
                MaximumSpeed = Result.MaximumSpeed,
                MinimumSpeed = Result.MinimumSpeed
            };
        }

        public async Task<DMotionPosition> UpdatePosition(DMotionPosition pos)
        {
            var entity = await _motionPositionRepository.FindAsync(keyValues: pos.Id);
            if (entity == null)
                return null;

            entity.Name = pos.Name;
            entity.Location = pos.Location;
            entity.MinimumLocation = pos.MinimumLocation;
            entity.MaximumLocation = pos.MaximumLocation;
            entity.Speed = pos.Speed;
            entity.MinimumSpeed = pos.MinimumSpeed;
            entity.MaximumSpeed = pos.MaximumSpeed;

            await _motionPositionRepository.Update(entity);

            return pos;
        }

        public async Task<bool> DeletePosition(DMotionPosition pos)
        {
            try
            {
                await _motionPositionRepository.Remove(pos.Id);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

}
