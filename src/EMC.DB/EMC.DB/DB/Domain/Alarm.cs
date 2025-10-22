﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMC.DB
{
    public class Alarm
    {
        [Key]
        public int Id { get; set; } // 식별자
        [Required]
        public string Code { get; set; } // 알람 코드 
        public string Name { get; set; } // 알람 이름

        public AlarmLevel Level { get; set; } // 알람 레벨
        public AlarmStatus Status { get; set; } // 알람 상태
        public AlarmEnable Enable { get; set; } // 알람 사용 여부
        public string Description { get; set; } // 알람 발생 원인 설명
        public string Action { get; set; } // 알람 발생 시 조치 방법
        public DateTime LastRaisedAt { get; set; }   // 마지막 알람이 발생된 시간
    }
}
