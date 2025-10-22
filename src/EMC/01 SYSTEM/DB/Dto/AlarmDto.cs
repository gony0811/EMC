﻿using CommunityToolkit.Mvvm.ComponentModel;
using EMC.DB;
using System;

namespace EMC
{
    [ObservableObject]
    public partial class AlarmDto
    {
        public int Id { get; private set; }
        [ObservableProperty] private string code;
        [ObservableProperty] private string name;
        [ObservableProperty] private string description;
        [ObservableProperty] private string action;
        [ObservableProperty] private AlarmLevel level = AlarmLevel.Light;
        [ObservableProperty] private AlarmStatus status = AlarmStatus.RESET;
        [ObservableProperty] private bool enabled = true;
        [ObservableProperty] private DateTime? lastRaisedAt;

        // 🔹 통합 상태 플래그
        [ObservableProperty] private bool isModified;

        public AlarmDto()
        {
            PropertyChanged += (_, e) =>
            {
                if (e.PropertyName != nameof(IsModified))
                    IsModified = true;
            };
        }

        public static AlarmDto From(Alarm alarm)
        {
            bool enable = alarm.Enable == AlarmEnable.ENABLED;

            return new AlarmDto
            {
                Id = alarm.Id,
                Code = alarm.Code,
                Name = alarm.Name,
                Description = alarm.Description,
                Action = alarm.Action,
                Level = alarm.Level,
                Status = alarm.Status,
                Enabled = enable,
                LastRaisedAt = alarm.LastRaisedAt == DateTime.MinValue ? (DateTime?)null : alarm.LastRaisedAt,
                IsModified = false
            };
        }

        public Alarm ToEntity()
        {
            AlarmEnable enable = Enabled ? AlarmEnable.ENABLED : AlarmEnable.DISABLED;
            return new Alarm
            {
                Id = Id,
                Code = Code,
                Name = Name,
                Description = Description,
                Action = Action,
                Level = Level,
                Status = Status,
                Enable = enable,
                LastRaisedAt = LastRaisedAt ?? DateTime.MinValue
            };
        }
    }
}