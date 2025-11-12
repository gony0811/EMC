using CommunityToolkit.Mvvm.ComponentModel;
using EMC.DB;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EMC
{
    public abstract partial class DeviceManager<TDevice, TMotion> : ObservableObject
        where TDevice : ADevice<TMotion>, new()
        where TMotion : DMotion, new()
    {
        private readonly DeviceRepository _deviceRepository;
        private readonly MotionRepository _motionRepository;
        private readonly MotionParameterRepository _motionParameterRepository;
        private readonly MotionPositionRepository _motionPositionRepository;

        [ObservableProperty]
        private ObservableCollection<TDevice> deviceList = new ObservableCollection<TDevice>();

        protected DeviceType DeviceType { get; }

        protected DeviceManager(
            DeviceType deviceType,
            DeviceRepository deviceRepository,
            MotionRepository motionRepository,
            MotionParameterRepository motionParameterRepository,
            MotionPositionRepository motionPositionRepository)
        {
            DeviceType = deviceType;
            _deviceRepository = deviceRepository;
            _motionRepository = motionRepository;
            _motionParameterRepository = motionParameterRepository;
            _motionPositionRepository = motionPositionRepository;
        }

        // ======================================================
        // ✅ 1. 로드 (DB → 메모리)
        // ======================================================
        public async Task LoadAsync()
        {
            DeviceList.Clear();

            var entities = await _deviceRepository.ListAsync(
                predicate: x => x.DeviceType == DeviceType.ToString(),
                include: q => q.Include(x => x.MotionList)
            );

            foreach (var entity in entities)
            {
                var device = new TDevice
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    DeviceType = entity.DeviceType,
                    Ip = entity.Ip,
                    IsEnabled = entity.IsEnabled,
                };

                // ✅ Motion 로딩
                foreach (var motionEntity in entity.MotionList)
                {
                    var motion = new TMotion
                    {
                        Id = motionEntity.Id,
                        Name = motionEntity.Name,
                        ControlType = motionEntity.ControlType,
                        IsEnabled = motionEntity.IsEnabled,
                        ParentDeviceId = device.Id
                    };

                    // 파라미터 로딩
                    var parameters = await _motionParameterRepository.ListAsync(
                        predicate: x => x.MotionId == motionEntity.Id);
                    foreach (var p in parameters)
                    {
                        motion.ParameterList.Add(new DMotionParameter
                        {
                            Id = p.Id,
                            Name = p.Name,
                            ValueType = p.ValueType,
                            IntValue = p.IntValue,
                            DoubleValue = p.DoubleValue,
                            BoolValue = p.BoolValue,
                            StringValue = p.StringValue,
                            Unit = p.Unit,
                            ParentMotionId = motion.Id
                        });
                    }

                    // 위치 로딩
                    var positions = await _motionPositionRepository.ListAsync(
                        predicate: x => x.MotionId == motionEntity.Id);
                    foreach (var pos in positions)
                    {
                        motion.PositionList.Add(new DMotionPosition
                        {
                            Id = pos.Id,
                            Name = pos.Name,
                            Speed = pos.Speed,
                            MinimumSpeed = pos.MinimumSpeed,
                            MaximumSpeed = pos.MaximumSpeed,
                            Location = 0,
                            MinimumLocation = pos.MinimumLocation,
                            MaximumLocation = pos.MaximumLocation,
                            ParentMotionId = motion.Id
                        });
                    }

                    device.MotionList.Add(motion);
                }

                DeviceList.Add(device);
            }

            Debug.WriteLine($"[DeviceManager] {DeviceList.Count} {DeviceType} devices loaded.");
        }

        // ======================================================
        // ✅ 2. Device 추가
        // ======================================================
        public async Task<TDevice> AddDeviceAsync(TDevice device)
        {
            if (device == null)
                throw new ArgumentNullException(nameof(device));

            var entity = new Device
            {
                Name = device.Name,
                Ip = device.Ip,
                DeviceType = device.DeviceType,
                IsEnabled = device.IsEnabled
            };

            var saved = await _deviceRepository.AddAsync(entity);
            device.Id = saved.Id;
            DeviceList.Add(device);

            Debug.WriteLine($"[DeviceManager] Added device: {device.Name} ({device.Ip})");
            return device;
        }

        // ======================================================
        // ✅ 3. Device 수정
        // ======================================================
        public async Task UpdateDeviceAsync(TDevice device)
        {
            var entity = await _deviceRepository.FindAsync(keyValues: device.Id);
            if (entity == null)
                throw new InvalidOperationException($"Device not found (Id={device.Id})");

            entity.Name = device.Name;
            entity.Ip = device.Ip;
            entity.IsEnabled = device.IsEnabled;
            await _deviceRepository.Update(entity);

            Debug.WriteLine($"[DeviceManager] Updated device: {device.Name}");
        }

        // ======================================================
        // ✅ 4. Device 삭제
        // ======================================================
        public async Task DeleteDeviceAsync(TDevice device)
        {
            if (device == null)
                return;

            var entity = await _deviceRepository.FindAsync(keyValues: device.Id);
            if (entity != null)
            {
                await _deviceRepository.Remove(entity.Id);
                DeviceList.Remove(device);
                Debug.WriteLine($"[DeviceManager] Deleted device: {device.Name}");
            }
        }

        // ======================================================
        // ✅ 5. Motion 추가
        // ======================================================
        public async Task<TMotion> AddMotionAsync(TDevice parentDevice, TMotion motion)
        {
            if (parentDevice == null)
                throw new ArgumentNullException(nameof(parentDevice));
            if (motion == null)
                throw new ArgumentNullException(nameof(motion));

            var motionEntity = new Motion
            {
                Name = motion.Name,
                ControlType = motion.ControlType,
                DeviceId = parentDevice.Id,
                IsEnabled = motion.IsEnabled
            };

            var saved = await _motionRepository.AddAsync(motionEntity);
            motion.Id = saved.Id;
            motion.ParentDeviceId = parentDevice.Id;

            parentDevice.MotionList.Add(motion);
            Debug.WriteLine($"[DeviceManager] Added Motion '{motion.Name}' to Device '{parentDevice.Name}'");

            return motion;
        }

        // ======================================================
        // ✅ 6. Motion 수정
        // ======================================================
        public async Task UpdateMotionAsync(TMotion motion)
        {
            var entity = await _motionRepository.FindAsync(keyValues: motion.Id);
            if (entity == null)
                throw new InvalidOperationException($"Motion not found (Id={motion.Id})");

            entity.Name = motion.Name;
            entity.ControlType = motion.ControlType;
            entity.IsEnabled = motion.IsEnabled;

            await _motionRepository.Update(entity);
            Debug.WriteLine($"[DeviceManager] Updated Motion: {motion.Name}");
        }

        // ======================================================
        // ✅ 7. Motion 삭제
        // ======================================================
        public async Task DeleteMotionAsync(TDevice parentDevice, TMotion motion)
        {
            if (motion == null || parentDevice == null)
                return;

            var entity = await _motionRepository.FindAsync(keyValues: motion.Id);
            if (entity != null)
            {
                await _motionRepository.Remove(entity.Id);
                parentDevice.MotionList.Remove(motion);
                Debug.WriteLine($"[DeviceManager] Deleted Motion '{motion.Name}' from Device '{parentDevice.Name}'");
            }
        }
    }
}
