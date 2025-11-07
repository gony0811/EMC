using CommunityToolkit.Mvvm.ComponentModel;
using EMC.DB;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace EMC
{
    public partial class DPowerPmacMotion : ObservableObject
    {
        [ObservableProperty] private int id;
        [ObservableProperty] private string name;
        [ObservableProperty] private bool isEnabled;
        [ObservableProperty] private int powerPMacId;
        
        [ObservableProperty] private string maker;
        [ObservableProperty] private int motorNo;
        [ObservableProperty] private string type;

        // Encoder
        [ObservableProperty] private double jogSpeed;                                                   // Jog 속도 ( 단위 : 미크론 )

        // MotorStatus
        [ObservableProperty] private bool sAmpEnabled = false;   // Servo Enable 상태  
        [ObservableProperty] private bool sClosedLoofMode = false;   // ClosedLoof 모드 상태 


        // === 홈 실행 모드 ===
        [ObservableProperty] private HomeExecutionMode homeExecutionMode = HomeExecutionMode.PLC;       // HOME 모드 방법( PLC, PC )_DB 연동 
        [ObservableProperty] private uint homeExcutionAddress;                                          // PLC 홈 실행 번호 _        DB 연동
        [ObservableProperty] private uint servoOn;  // 
        [ObservableProperty] private uint homeInProgress; // 홈 동작 실행 여부
        // === 분석 덜 됨
        // === 제어 파라미터 ===
        [ObservableProperty] private bool emergencyEnable;
        [ObservableProperty] private bool softLimitEnable;
        [ObservableProperty] private bool inPositionEnable;
        [ObservableProperty] private bool servoOffOnEmergencyEnable;

        // === 신호 레벨 ===
        [ObservableProperty] private bool servoLevel;
        [ObservableProperty] private bool alarmLevel;
        [ObservableProperty] private bool originLevel;
        [ObservableProperty] private bool emergencyLevel;
        [ObservableProperty] private bool inPositionLevel;
        [ObservableProperty] private bool positiveLimitLevel;
        [ObservableProperty] private bool negativeLimitLevel;

        // === 홈 / 리밋 / 속도 모드 ===
        [ObservableProperty] private int homeMode1;
        [ObservableProperty] private int homeMode2;
        [ObservableProperty] private int speedMode;
        [ObservableProperty] private int positiveLimitMode;
        [ObservableProperty] private int negativeLimitMode;

        // === 엔코더 / 펄스 ===
        [ObservableProperty] private bool encoderReverse;
        [ObservableProperty] private int pulseOutputMethod;
        [ObservableProperty] private int encoderInputMethod;

        // === 이동 파라미터 ===
        [ObservableProperty] private double ratio;
        [ObservableProperty] private double maxVelocity;
        [ObservableProperty] private double positionError;
        [ObservableProperty] private double inPositionWidth;

        // === 소프트 리밋 ===
        [ObservableProperty] private double softPositiveLimit;
        [ObservableProperty] private double softNegativeLimit;

        // === 홈 설정 ===
        [ObservableProperty] private double homeOffset;
        [ObservableProperty] private double homeVelocity1;
        [ObservableProperty] private double homeVelocity2;
        [ObservableProperty] private double homeVelocity3;
        [ObservableProperty] private double homeAccelerate1;
        [ObservableProperty] private double homeAccelerate2;
        [ObservableProperty] private double homeDecelerate1;
        [ObservableProperty] private double homeDecelerate2;
        [ObservableProperty] private bool isSaveFailed = false;
        public DPowerPmac powerPmac;

        [ObservableProperty] private bool isInposition;
        [ObservableProperty] private bool isHomeDone;
        [ObservableProperty] private int homeStep;

        public string DisplayName => $"{Name} (#{MotorNo})";

        // 변화 감지
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.PropertyName != nameof(IsSaveFailed) &&
                e.PropertyName != nameof(DisplayName))
            {
                IsSaveFailed = true;
            }
        }

        #region Jog 명령

        public void JogMove(JogMoveType moveType)   // MoveType : Plus, Minus, Stop
        {
            try
            {
                // 모터 상태 확인
                if (!SAmpEnabled) throw new Exception("Amp Disable");
                
                if (!SClosedLoofMode) throw new Exception("ClosedLoofMode를 On 해주세요");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            PowerPmacCommands.JogSpeed.Execute(powerPmac.DeviceId, new NumValue(MotorNo, JogSpeed));
            switch(moveType)
            {
                case JogMoveType.Plus:
                    PowerPmacCommands.JogPlus.Execute(powerPmac.DeviceId, new MotorNumParam(MotorNo));
                    break;
                case JogMoveType.Minus:
                    PowerPmacCommands.JogMinus.Execute(powerPmac.DeviceId, new MotorNumParam(MotorNo));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(moveType), moveType, null);
            }
        }
        #endregion
        #region Home Process

        #endregion

        #region Status Polling 
        public void StartPolling()
        {
            if (powerPmac == null || !powerPmac.IsConnected) return;

            bool result = PowerPmacCommands.HomeDone.Execute(
                powerPmac.DeviceId,
                new MotorNumParam { Num = MotorNo }
            );


        }

        private async Task PoolLoop(CancellationToken ct)
        {
            bool result = PowerPmacCommands.HomeDone.Execute(
                powerPmac.DeviceId, 
                new MotorNumParam{ Num = MotorNo }
                );


            await Task.Delay(50, ct); // 20Hz 주기
        }
        #endregion
    }

    #region 팩토리 클래스
    public static class PowerPMacMotionFactory
    {
        /// <summary>
        /// Entity → DTO 변환
        /// </summary>
        public static DPowerPmacMotion ToDto(this PowerPMacMotion e)
        {
            if (e == null) return null;

            return new DPowerPmacMotion
            {
                Id = e.Id,
                Name = e.Name,
                IsEnabled = e.IsEnabled,
                PowerPMacId = e.PowerPMacId,
                Maker = e.Maker,
                MotorNo = e.MotorNo,
                Type = e.Type,

                EmergencyEnable = e.EmergencyEnable,
                SoftLimitEnable = e.SoftLimitEnable,
                InPositionEnable = e.InPositionEnable,
                ServoOffOnEmergencyEnable = e.ServoOffOnEmergencyEnable,

                ServoLevel = e.ServoLevel,
                AlarmLevel = e.AlarmLevel,
                OriginLevel = e.OriginLevel,
                EmergencyLevel = e.EmergencyLevel,
                InPositionLevel = e.InPositionLevel,
                PositiveLimitLevel = e.PositiveLimitLevel,
                NegativeLimitLevel = e.NegativeLimitLevel,

                HomeMode1 = e.HomeMode1,
                HomeMode2 = e.HomeMode2,
                SpeedMode = e.SpeedMode,
                PositiveLimitMode = e.PositiveLimitMode,
                NegativeLimitMode = e.NegativeLimitMode,

                EncoderReverse = e.EncoderReverse,
                PulseOutputMethod = e.PulseOutputMethod,
                EncoderInputMethod = e.EncoderInputMethod,

                Ratio = e.Ratio,
                MaxVelocity = e.MaxVelocity,
                PositionError = e.PositionError,
                InPositionWidth = e.InPositionWidth,

                SoftPositiveLimit = e.SoftPositiveLimit,
                SoftNegativeLimit = e.SoftNegativeLimit,

                HomeOffset = e.HomeOffset,
                HomeVelocity1 = e.HomeVelocity1,
                HomeVelocity2 = e.HomeVelocity2,
                HomeVelocity3 = e.HomeVelocity3,
                HomeAccelerate1 = e.HomeAccelerate1,
                HomeAccelerate2 = e.HomeAccelerate2,
                HomeDecelerate1 = e.HomeDecelerate1,
                HomeDecelerate2 = e.HomeDecelerate2,
                IsSaveFailed = false
            };
        }

        /// <summary>
        /// DTO → Entity 변환
        /// </summary>
        public static PowerPMacMotion ToEntity(this DPowerPmacMotion d)
        {
            if (d == null) return null;

            return new PowerPMacMotion
            {
                Id = d.Id,
                Name = d.Name,
                IsEnabled = d.IsEnabled,
                PowerPMacId = d.PowerPMacId,
                Maker = d.Maker,
                MotorNo = d.MotorNo,
                Type = d.Type,

                EmergencyEnable = d.EmergencyEnable,
                SoftLimitEnable = d.SoftLimitEnable,
                InPositionEnable = d.InPositionEnable,
                ServoOffOnEmergencyEnable = d.ServoOffOnEmergencyEnable,

                ServoLevel = d.ServoLevel,
                AlarmLevel = d.AlarmLevel,
                OriginLevel = d.OriginLevel,
                EmergencyLevel = d.EmergencyLevel,
                InPositionLevel = d.InPositionLevel,
                PositiveLimitLevel = d.PositiveLimitLevel,
                NegativeLimitLevel = d.NegativeLimitLevel,

                HomeMode1 = d.HomeMode1,
                HomeMode2 = d.HomeMode2,
                SpeedMode = d.SpeedMode,
                PositiveLimitMode = d.PositiveLimitMode,
                NegativeLimitMode = d.NegativeLimitMode,

                EncoderReverse = d.EncoderReverse,
                PulseOutputMethod = d.PulseOutputMethod,
                EncoderInputMethod = d.EncoderInputMethod,

                Ratio = d.Ratio,
                MaxVelocity = d.MaxVelocity,
                PositionError = d.PositionError,
                InPositionWidth = d.InPositionWidth,

                SoftPositiveLimit = d.SoftPositiveLimit,
                SoftNegativeLimit = d.SoftNegativeLimit,

                HomeOffset = d.HomeOffset,
                HomeVelocity1 = d.HomeVelocity1,
                HomeVelocity2 = d.HomeVelocity2,
                HomeVelocity3 = d.HomeVelocity3,
                HomeAccelerate1 = d.HomeAccelerate1,
                HomeAccelerate2 = d.HomeAccelerate2,
                HomeDecelerate1 = d.HomeDecelerate1,
                HomeDecelerate2 = d.HomeDecelerate2,

            };
        }

        /// <summary>
        /// Entity → DTO 속성 복사 (기존 DTO 객체 업데이트)
        /// </summary>
        public static void CopyFrom(this DPowerPmacMotion target, PowerPMacMotion source)
        {
            if (target == null || source == null) return;

            target.Id = source.Id;
            target.Name = source.Name;
            target.IsEnabled = source.IsEnabled;
            target.PowerPMacId = source.PowerPMacId;
            target.Maker = source.Maker;
            target.MotorNo = source.MotorNo;
            target.Type = source.Type;

            target.EmergencyEnable = source.EmergencyEnable;
            target.SoftLimitEnable = source.SoftLimitEnable;
            target.InPositionEnable = source.InPositionEnable;
            target.ServoOffOnEmergencyEnable = source.ServoOffOnEmergencyEnable;

            target.ServoLevel = source.ServoLevel;
            target.AlarmLevel = source.AlarmLevel;
            target.OriginLevel = source.OriginLevel;
            target.EmergencyLevel = source.EmergencyLevel;
            target.InPositionLevel = source.InPositionLevel;
            target.PositiveLimitLevel = source.PositiveLimitLevel;
            target.NegativeLimitLevel = source.NegativeLimitLevel;

            target.HomeMode1 = source.HomeMode1;
            target.HomeMode2 = source.HomeMode2;
            target.SpeedMode = source.SpeedMode;
            target.PositiveLimitMode = source.PositiveLimitMode;
            target.NegativeLimitMode = source.NegativeLimitMode;

            target.EncoderReverse = source.EncoderReverse;
            target.PulseOutputMethod = source.PulseOutputMethod;
            target.EncoderInputMethod = source.EncoderInputMethod;

            target.Ratio = source.Ratio;
            target.MaxVelocity = source.MaxVelocity;
            target.PositionError = source.PositionError;
            target.InPositionWidth = source.InPositionWidth;

            target.SoftPositiveLimit = source.SoftPositiveLimit;
            target.SoftNegativeLimit = source.SoftNegativeLimit;

            target.HomeOffset = source.HomeOffset;
            target.HomeVelocity1 = source.HomeVelocity1;
            target.HomeVelocity2 = source.HomeVelocity2;
            target.HomeVelocity3 = source.HomeVelocity3;
            target.HomeAccelerate1 = source.HomeAccelerate1;
            target.HomeAccelerate2 = source.HomeAccelerate2;
            target.HomeDecelerate1 = source.HomeDecelerate1;
            target.HomeDecelerate2 = source.HomeDecelerate2;
        }

        /// <summary>
        /// DTO → Entity 속성 복사 (기존 Entity 객체 업데이트)
        /// </summary>
        public static void CopyFrom(this PowerPMacMotion target, DPowerPmacMotion source)
        {
            if (target == null || source == null) return;

            target.Id = source.Id;
            target.Name = source.Name;
            target.IsEnabled = source.IsEnabled;
            target.PowerPMacId = source.PowerPMacId;
            target.Maker = source.Maker;
            target.MotorNo = source.MotorNo;
            target.Type = source.Type;

            target.EmergencyEnable = source.EmergencyEnable;
            target.SoftLimitEnable = source.SoftLimitEnable;
            target.InPositionEnable = source.InPositionEnable;
            target.ServoOffOnEmergencyEnable = source.ServoOffOnEmergencyEnable;

            target.ServoLevel = source.ServoLevel;
            target.AlarmLevel = source.AlarmLevel;
            target.OriginLevel = source.OriginLevel;
            target.EmergencyLevel = source.EmergencyLevel;
            target.InPositionLevel = source.InPositionLevel;
            target.PositiveLimitLevel = source.PositiveLimitLevel;
            target.NegativeLimitLevel = source.NegativeLimitLevel;

            target.HomeMode1 = source.HomeMode1;
            target.HomeMode2 = source.HomeMode2;
            target.SpeedMode = source.SpeedMode;
            target.PositiveLimitMode = source.PositiveLimitMode;
            target.NegativeLimitMode = source.NegativeLimitMode;

            target.EncoderReverse = source.EncoderReverse;
            target.PulseOutputMethod = source.PulseOutputMethod;
            target.EncoderInputMethod = source.EncoderInputMethod;

            target.Ratio = source.Ratio;
            target.MaxVelocity = source.MaxVelocity;
            target.PositionError = source.PositionError;
            target.InPositionWidth = source.InPositionWidth;

            target.SoftPositiveLimit = source.SoftPositiveLimit;
            target.SoftNegativeLimit = source.SoftNegativeLimit;

            target.HomeOffset = source.HomeOffset;
            target.HomeVelocity1 = source.HomeVelocity1;
            target.HomeVelocity2 = source.HomeVelocity2;
            target.HomeVelocity3 = source.HomeVelocity3;
            target.HomeAccelerate1 = source.HomeAccelerate1;
            target.HomeAccelerate2 = source.HomeAccelerate2;
            target.HomeDecelerate1 = source.HomeDecelerate1;
            target.HomeDecelerate2 = source.HomeDecelerate2;
        }
    }
    #endregion
}