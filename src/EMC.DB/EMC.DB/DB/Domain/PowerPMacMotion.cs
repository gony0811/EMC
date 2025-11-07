using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMC.DB
{
    [Table("PowerPMacMotion")]
    public class PowerPMacMotion
    {
        [Key]
        public int Id { get; set; }                     // 내부 고유 ID (논리 ID, 절대 변경 불가)

        [Required, MaxLength(64)]
        public string Name { get; set; }                // 축 이름 (예: "PICKER Z")

        public bool IsEnabled { get; set; } = true;     // 사용 여부

        [ForeignKey("PowerPMacDevice")]
        public int PowerPMacId { get; set; }            // 연결된 Power PMAC 장치 ID
        public PowerPMac PowerPMac { get; set; } // 참조 관계 (N:1)

        [MaxLength(32)]
        public string Maker { get; set; }               // 제조사 (PANASONIC, YASKAWA 등)

        // === 제어 파라미터 ===
        public int MotorNo { get; set; }                // Power PMAC 내부 모터 번호 (#1, #2, ...)

        public string Type { get; set; }                // 제어 타입 (e.g. EtherCAT, Pulse)
        public bool EmergencyEnable { get; set; }       // 비상정지 사용 여부
        public bool SoftLimitEnable { get; set; }       // 소프트리밋 사용 여부
        public bool InPositionEnable { get; set; }      // In-Position 신호 사용 여부
        public bool ServoOffOnEmergencyEnable { get; set; }

        // === 신호 레벨 설정 ===
        public bool ServoLevel { get; set; }
        public bool AlarmLevel { get; set; }
        public bool OriginLevel { get; set; }
        public bool EmergencyLevel { get; set; }
        public bool InPositionLevel { get; set; }
        public bool PositiveLimitLevel { get; set; }
        public bool NegativeLimitLevel { get; set; }

        // === 홈 / 리밋 / 속도 모드 ===
        public int HomeMode1 { get; set; }
        public int HomeMode2 { get; set; }
        public int SpeedMode { get; set; }
        public int PositiveLimitMode { get; set; }
        public int NegativeLimitMode { get; set; }

        // === 엔코더 / 펄스 설정 ===
        public bool EncoderReverse { get; set; }
        public int PulseOutputMethod { get; set; }
        public int EncoderInputMethod { get; set; }

        // === 이동 파라미터 ===
        public double Ratio { get; set; }               // 펄스→거리 변환비
        public double MaxVelocity { get; set; }         // 최대 속도
        public double PositionError { get; set; }       // 허용 위치 오차
        public double InPositionWidth { get; set; }     // InPosition 폭 (단위 mm 등)

         // === 소프트 리밋 ===
        public double SoftPositiveLimit { get; set; }
        public double SoftNegativeLimit { get; set; }

            // === 홈 설정 ===
        public double HomeOffset { get; set; }
        public double HomeVelocity1 { get; set; }
        public double HomeVelocity2 { get; set; }
        public double HomeVelocity3 { get; set; }
        public double HomeAccelerate1 { get; set; }
        public double HomeAccelerate2 { get; set; }
        public double HomeDecelerate1 { get; set; }
        public double HomeDecelerate2 { get; set; }


             // === 유틸리티 메서드 ===
        [NotMapped]
        public string DisplayName => $"{Name} (#{MotorNo})";

        public override string ToString() => DisplayName;
    }
}
