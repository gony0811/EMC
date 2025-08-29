using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EGGPLANT
{
    public delegate void FireMotionError(IntPtr AMotionBasic, int AMotionIndex, MOTION_ERROR AErrorKind);
    public enum MOTION_ERROR { NONE = 0x00, NLIMIT = 0x01, PLIMIT = 0x02, POSITION = 0x04 }
    public class CMotionPositionBasic
    {
        private int FIndex;
        public int Index { get { return FIndex; } }

        private CMotionItemBasic FMotionItemBasic = null;
        public CMotionItemBasic MotionItemBasic { get { return FMotionItemBasic; } }

        public double Jerk = 0.0d;
        public double Position = 0.0d;
        public double Velocity = 1.0d;
        public double Accelerate = 100.0d;
        public double Decelerate = 100.0d;

        public string Explain = "";
        protected double FSpeedPercent = 50;
        virtual public double SpeedPercent
        {
            get { return FSpeedPercent; }
            set
            {
                if (value < 1) value = 1;
                if (value > 100) value = 100;

                FSpeedPercent = value;
            }
        }

        public CMotionPositionBasic(CMotionItemBasic AMotionItemBasic, int AIndex)
        {
            FMotionItemBasic = AMotionItemBasic;
            FIndex = AIndex;
        }

        private double GetSpeedValue()
        {
            if (FMotionItemBasic == null) return 0.0;

            double velocity = 1.0;
            switch (FMotionItemBasic.SpeedMode)
            {
                case 0:
                    velocity = FMotionItemBasic.MaxVelocity;
                    break;
                case 1:
                case 2:
                    velocity = FMotionItemBasic.Position(1).Velocity;
                    break;
                case 3:
                case 4:
                case 5:
                    velocity = Velocity;
                    break;
            }

            if (FMotionItemBasic.SpeedUnitIsPercent) return SpeedPercent;
            return velocity * (double)SpeedPercent / 100.0;
        }
        private void SetSpeedValue(double AValue)
        {
            if (FMotionItemBasic == null) return;
            if (AValue <= 0) return;

            if (!FMotionItemBasic.SpeedUnitIsPercent)
            {
                double velocity = 1.0;
                switch (FMotionItemBasic.SpeedMode)
                {
                    case 0:
                        velocity = FMotionItemBasic.MaxVelocity;
                        break;
                    case 1:
                    case 2:
                        velocity = FMotionItemBasic.Position(1).Velocity;
                        break;
                    case 3:
                    case 4:
                    case 5:
                        velocity = Velocity;
                        break;
                }

                if (AValue >= velocity) AValue = 100;
                else AValue = (int)((AValue / velocity) * 100.0);
            }

            if (AValue < 1) AValue = 1;
            SpeedPercent = AValue;
        }
        public double SpeedValue { get { return GetSpeedValue(); } set { SetSpeedValue(value); } }

        public double GetSpeedPercentFromValue(bool AUnitIsPercent, int APositionIndex, int ASpeedValue)
        {
            if (FMotionItemBasic == null) return 0.0;

            int speedper = ASpeedValue;
            if (!AUnitIsPercent)
            {
                double velocity = 1.0;
                switch (FMotionItemBasic.SpeedMode)
                {
                    case 0:
                        velocity = FMotionItemBasic.MaxVelocity;
                        break;
                    case 1:
                    case 2:
                        velocity = FMotionItemBasic.Position(1).Velocity;
                        break;
                    case 3:
                    case 4:
                    case 5:
                        velocity = Velocity;
                        break;
                }

                if (ASpeedValue >= velocity) speedper = 100;
                else speedper = (int)((ASpeedValue / velocity) * 100.0);
            }
            if (speedper <= 1) speedper = 1;
            if (speedper > 100) speedper = 100;
            return speedper;
        }
        public double GetSpeedValueFromPercent(bool AUnitIsPercent, int APositionIndex, int ASpeedPercent)
        {
            if (FMotionItemBasic == null) return 1.0;

            if (ASpeedPercent <= 1) ASpeedPercent = 1;
            if (ASpeedPercent > 100) ASpeedPercent = 100;

            if (AUnitIsPercent) return ASpeedPercent;
            return FMotionItemBasic.Position(APositionIndex).Velocity * (double)ASpeedPercent / 100.0;
        }

        #region 포지션 저장 및 불러오기
        virtual public void SaveDefaultPosition(XmlDocument ADoc, XmlNode ANode, int AIndex)
        {
            XmlNode position = ADoc.CreateElement("POSITION");
            XmlAttribute attr = ADoc.CreateAttribute("ID");

            attr.Value = AIndex.ToString();
            position.Attributes.Append(attr);

            if (AIndex == 0) CXML.AddElement(ADoc, position, "EXPAIN", "JOG SPEED");
            else CXML.AddElement(ADoc, position, "EXPAIN", Explain);

            CXML.AddElement(ADoc, position, "POSITION", Position);
            CXML.AddElement(ADoc, position, "SPEED_PERCENT", FSpeedPercent);

            CXML.AddElement(ADoc, position, "JERK", Jerk);
            CXML.AddElement(ADoc, position, "VELOCITY", Velocity);
            CXML.AddElement(ADoc, position, "ACCELERATE", Accelerate);
            CXML.AddElement(ADoc, position, "DECELERATE", Decelerate);
            ANode.AppendChild(position);
        }
        virtual public void OpenDefaultPosition(XmlNode ANode, int AIndex)
        {
            XmlNode position = ANode.SelectSingleNode($"./POSITION[@ID='{AIndex}']");

            if (position != null)
            {
                CXML.GetInnerText(position, "POSITION", out Position);
                CXML.GetInnerText(position, "SPEED_PERCENT", out FSpeedPercent);

                CXML.GetInnerText(position, "EXPAIN", out Explain);
                CXML.GetInnerText(position, "JERK", out Jerk);
                CXML.GetInnerText(position, "VELOCITY", out Velocity);
                CXML.GetInnerText(position, "ACCELERATE", out Accelerate);
                CXML.GetInnerText(position, "DECELERATE", out Decelerate);
            }
        }

        virtual public void Save(XmlDocument ADoc, XmlNode ANode, int AIndex)
        {
            XmlNode position = ADoc.CreateElement("POSITION");
            XmlAttribute attr = ADoc.CreateAttribute("ID");

            attr.Value = AIndex.ToString();
            position.Attributes.Append(attr);

            if (AIndex == 0) CXML.AddElement(ADoc, position, "EXPAIN", "JOG SPEED");
            else CXML.AddElement(ADoc, position, "EXPAIN", Explain);

            CXML.AddElement(ADoc, position, "POSITION", Position);
            CXML.AddElement(ADoc, position, "SPEED_PERCENT", FSpeedPercent);

            CXML.AddElement(ADoc, position, "JERK", Jerk);
            CXML.AddElement(ADoc, position, "VELOCITY", Velocity);
            CXML.AddElement(ADoc, position, "ACCELERATE", Accelerate);
            CXML.AddElement(ADoc, position, "DECELERATE", Decelerate);
            ANode.AppendChild(position);
        }
        virtual public void Open(XmlNode ANode, int AIndex)
        {
            XmlNode position = ANode.SelectSingleNode($"./POSITION[@ID='{AIndex}']");

            if (position != null)
            {
                if (FMotionItemBasic != null)
                {
                    CXML.GetInnerText(position, "POSITION", out Position);
                    if (FMotionItemBasic.PositionMode == 0x01)
                    {
                        CXML.GetInnerText(position, "SPEED_PERCENT", out FSpeedPercent);
                    }
                    if (FMotionItemBasic.PositionMode == 0x02)
                    {
                        CXML.GetInnerText(position, "JERK", out Jerk);
                        CXML.GetInnerText(position, "VELOCITY", out Velocity);
                        CXML.GetInnerText(position, "ACCELERATE", out Accelerate);
                        CXML.GetInnerText(position, "DECELERATE", out Decelerate);
                    }
                }
            }
        }

        virtual public void Decoding(XmlNode ASrcNode, XmlDocument ADoc, XmlNode ANode, int AIndex)
        {
            if (AIndex <= 0) return;
            if (Explain.Trim() == "") return;

            XmlNode srcposition = ASrcNode.SelectSingleNode($"./POSITION[@ID='{AIndex}']");
            if (srcposition != null)
            {
                XmlNode position = ANode.SelectSingleNode($"./POSITION[@ID='{AIndex}']");
                if (position != null)
                {
                    CXML.Move(srcposition, position, "POSITION");
                    if (FMotionItemBasic.PositionMode == 0x01)
                    {
                        CXML.Move(srcposition, position, "SPEED_PERCENT");
                    }
                    if (FMotionItemBasic.PositionMode == 0x02)
                    {
                        CXML.Move(srcposition, position, "JERK");
                        CXML.Move(srcposition, position, "VELOCITY");
                        CXML.Move(srcposition, position, "ACCELERATE");
                        CXML.Move(srcposition, position, "DECELERATE");
                    }
                }
            }
        }
        virtual public void Encoding(XmlNode ASrcNode, XmlDocument ADoc, XmlNode ANode, int AIndex)
        {
            if (AIndex <= 0) return;
            if (Explain.Trim() == "") return;

            XmlNode position = ASrcNode.SelectSingleNode($"./POSITION[@ID='{AIndex}']");
            if (position != null) ANode.AppendChild(ADoc.ImportNode(position, true));
        }
        #endregion
    }

    public class CMotionItemBasic
    {
        public const UInt32 __MOTION_MAX_POSITION_COUNT__ = 21;

        protected int FMotionIndex;
        protected string FMotionName;
        protected CMotionPositionBasic[] FPositions = null;
        protected int FPositionMode = 0x01;
        public int MotionIndex { get { return FMotionIndex; } }
        public string MotionName { get { return FMotionName; } }
        public int PositionMode { get { return FPositionMode; } }
        public int PositionCount { get { return FPositions.Length; } }

        public CMotionPositionBasic Position(int AIndex)
        {
            if (AIndex < 0) return null;
            if (AIndex >= FPositions.Length) return null;

            return FPositions[AIndex];
        }

        public CMotionItemBasic(int AMotionIndex, string AMotionName = "", int AMaxPositionCount = 21, int APositionMode = 0x01)
        {
            FPositions = new CMotionPositionBasic[AMaxPositionCount];
            for (int i = 0; i < FPositions.Length; i++) FPositions[i] = new CMotionPositionBasic(this, i);

            FPositionMode = APositionMode;
            FMotionIndex = AMotionIndex;
            FMotionName = AMotionName;
        }

        #region Set Parameter [Enabled]
        protected bool FEmergencyEnable = false;
        protected bool FSoftLimitEnable = false;
        protected bool FInpositionEnable = true;
        protected bool FServoOffOnEmergencyEnable = true;
        virtual public bool EmergencyEnable { get { return FEmergencyEnable; } set { FEmergencyEnable = value; } }
        virtual public bool SoftLimitEnable { get { return FSoftLimitEnable; } set { FSoftLimitEnable = value; } }
        virtual public bool InpositionEnable { get { return FInpositionEnable; } set { FInpositionEnable = value; } }
        virtual public bool ServoOffOnEmergencyEnable { get { return FServoOffOnEmergencyEnable; } set { FServoOffOnEmergencyEnable = value; } }
        #endregion

        #region Set Parameter [Encoder Reverse]
        protected bool FEncoderReverse = false;
        virtual public bool EncoderReverse { get { return FEncoderReverse; } set { FEncoderReverse = value; } }
        #endregion

        #region Set Parameter [       ]
        protected int FHomeMode1 = 1;
        protected int FHomeMode2 = 0;
        protected int FSpeedMode = 1;
        protected int FPositiveLimitMode = 0;
        protected int FNegativeLimitMode = 0;
        protected int FPulseOutputMethod = 4;
        protected int FEncoderInputMethod = 3;
        virtual public int HomeMode1 { get { return FHomeMode1; } set { FHomeMode1 = value; } }
        virtual public int HomeMode2 { get { return FHomeMode2; } set { FHomeMode2 = value; } }
        virtual public int SpeedMode { get { return FSpeedMode; } set { FSpeedMode = value; } }
        virtual public int PositiveLimitMode { get { return FPositiveLimitMode; } set { FPositiveLimitMode = value; } }
        virtual public int NegativeLimitMode { get { return FNegativeLimitMode; } set { FNegativeLimitMode = value; } }
        virtual public int PulseOutputMethod { get { return FPulseOutputMethod; } set { FPulseOutputMethod = value; } }
        virtual public int EncoderInputMethod { get { return FEncoderInputMethod; } set { FEncoderInputMethod = value; } }
        #endregion

        #region Set Parameter [Level  ]
        protected bool FEnabled = true;
        protected bool FServoLevel = false;
        protected bool FAlarmLevel = false;
        protected bool FOriginLevel = false;
        protected bool FEmergencyLevel = false;
        protected bool FInpositionLevel = true;
        protected bool FPositiveLimitLevel = false;
        protected bool FNegativeLimitLevel = false;
        virtual public bool Enabled { get { return FEnabled; } set { FEnabled = value; } }
        virtual public bool ServoLevel { get { return FServoLevel; } set { FServoLevel = value; } }
        virtual public bool AlarmLevel { get { return FAlarmLevel; } set { FAlarmLevel = value; } }
        virtual public bool OriginLevel { get { return FOriginLevel; } set { FOriginLevel = value; } }
        virtual public bool EmergencyLevel { get { return FEmergencyLevel; } set { FEmergencyLevel = value; } }
        virtual public bool InpositionLevel { get { return FInpositionLevel; } set { FInpositionLevel = value; } }
        virtual public bool PositiveLimitLevel { get { return FPositiveLimitLevel; } set { FPositiveLimitLevel = value; } }
        virtual public bool NegativeLimitLevel { get { return FNegativeLimitLevel; } set { FNegativeLimitLevel = value; } }
        #endregion

        #region Set Infomation [       ]
        protected double FRatio = 1.0d;
        protected double FMaxVelocity = +1000000.0d;
        protected double FSoftPositiveLimit = +1000.0d;
        protected double FSoftNegativeLimit = -1000.0d;
        virtual public double Ratio { get { return FRatio; } set { FRatio = value; } }
        virtual public double MaxVelocity { get { return FMaxVelocity; } set { FMaxVelocity = value; } }
        virtual public double SoftPositiveLimit { get { return FSoftPositiveLimit; } set { FSoftPositiveLimit = value; } }
        virtual public double SoftNegativeLimit { get { return FSoftNegativeLimit; } set { FSoftNegativeLimit = value; } }

        protected double FHomeOffSet = 0.0d;
        protected double FHomeVelocity1 = 10.0d;
        protected double FHomeVelocity2 = 10.0d;
        protected double FHomeVelocity3 = 1.0d;
        protected double FHomeAccelerate1 = +100.0d;
        protected double FHomeAccelerate2 = +100.0d;
        protected double FHomeDecelerate1 = +100.0d;
        protected double FHomeDecelerate2 = +100.0d;
        virtual public double HomeOffSet { get { return FHomeOffSet; } set { FHomeOffSet = value; } }
        virtual public double HomeVelocity1 { get { return FHomeVelocity1; } set { FHomeVelocity1 = value; } }
        virtual public double HomeVelocity2 { get { return FHomeVelocity2; } set { FHomeVelocity2 = value; } }
        virtual public double HomeVelocity3 { get { return FHomeVelocity3; } set { FHomeVelocity3 = value; } }
        virtual public double HomeAccelerate1 { get { return FHomeAccelerate1; } set { FHomeAccelerate1 = value; } }
        virtual public double HomeAccelerate2 { get { return FHomeAccelerate2; } set { FHomeAccelerate2 = value; } }
        virtual public double HomeDecelerate1 { get { return FHomeDecelerate1; } set { FHomeDecelerate1 = value; } }
        virtual public double HomeDecelerate2 { get { return FHomeDecelerate2; } set { FHomeDecelerate2 = value; } }
        #endregion

        #region Set Servo On
        protected bool FServoOn = false;
        virtual public bool ServoOn { get { return FServoOn; } set { FServoOn = value; } }
        #endregion

        #region Motion Status
        protected int FHomeDone = 0;
        virtual public int HomeDone { get { return FHomeDone; } set { FHomeDone = value; } }

        protected bool FOnReady = false;
        protected bool FOnAlarm = false;
        protected bool FOnOrigin = false;
        protected bool FInposition = false;
        protected bool FMotionDone = false;
        protected bool FInpositionEx = false;
        protected bool FOnPositiveLimit = false;
        protected bool FOnNegativeLimit = false;
        virtual public bool OnReady { get { return FOnReady; } }
        virtual public bool OnAlarm { get { return FOnAlarm; } }
        virtual public bool OnOrigin { get { return FOnOrigin; } }
        virtual public bool Inposition { get { return FInposition; } }
        virtual public bool MotionDone { get { return FMotionDone; } }
        virtual public bool InpositionEx { get { return FInpositionEx; } }
        virtual public bool OnPositiveLimit { get { return FOnPositiveLimit; } }
        virtual public bool OnNegativeLimit { get { return FOnNegativeLimit; } }

        protected double FPositionError = 0.0d;
        protected double FTargetPosition = 0.0d;
        protected double FCommandPosition = 0.0d;
        protected double FEncoderPosition = 0.0d;
        virtual public double PositionError { get { return FPositionError; } }
        virtual public double TargetPosition { get { return FTargetPosition; } }
        virtual public double CommandPosition { get { return FCommandPosition; } set { FCommandPosition = value; } }
        virtual public double EncoderPosition { get { return FEncoderPosition; } set { FEncoderPosition = value; } }

        virtual public double PositionErrCount { get { return 0.0d; } }
#pragma warning disable CS0649
        public FireMotionError FireMotionError;
#pragma warning restore CS0649
        virtual public void AlarmClear() { }

        protected int FAlarmClearOffDelay = 0;
        protected bool FEmergencyStatus = false;
        protected int FServoOffDelayOnEmergency = 0;
        virtual public bool GetStatus(bool AEmergency) { return true; }

        protected int FPositionIndex = -1;
        protected int FNextPositionIndex = -1;
        virtual public int PositionIndex { get { return FPositionIndex; } }
        virtual public int NextPositionIndex { get { return FNextPositionIndex; } }
        #endregion

        #region Home Process
        protected int FHomeStep = 0;
        virtual public int HomeStep
        {
            get { return FHomeStep; }
        }

        protected int FHomeDelay = 0;
        protected int FHomeResult = 0;
        protected int FHomeDelayEx = 0;
        protected int FHomeSoftLimitEnable = 0;
        virtual public bool HomeStart()
        {
            if (!FInposition) return false;
            if (!MotionDone) return false;

            FHomeDone = 0;
            FHomeStep = 1;
            return true;
        }
        virtual protected void HomeProc() { }
        virtual protected void HomeAbsoluteMove(int APositionIndex) { }
        #endregion

        #region Motion Move
        public double InpositionWidth
        {
            get { return FInpositionWidth; }
            set
            {
                if (value >= 0) FInpositionWidth = value;
            }
        }
        protected double FInpositionWidth = 0.0;
        protected bool FOnSoftInposition = true;
        public bool OnSoftInposition { get { return FOnSoftInposition; } }

        protected double FVelocity = 0.0d;
        protected double FAccelerate = 100.0d;
        protected double FDecelerate = 100.0d;
        protected double FStopDecelerate = 10.0d;
        virtual public double Velocity { get { return FVelocity; } set { FVelocity = value; } }
        virtual public double Accelerate { get { return FAccelerate; } set { FAccelerate = value; } }
        virtual public double Decelerate { get { return FDecelerate; } set { FDecelerate = value; } }
        virtual public double StopDecelerate { get { return FStopDecelerate; } set { FStopDecelerate = value; } }

        virtual public void Stop(bool AEmergencyStop = true, bool AHomeFail = false) { }
        virtual public void JogMove(bool AForward, int ASpeedPercent) { }
        virtual public void VelocityMove(int APositionIndex, bool AForward) { }
        virtual public void JogRelativeMove(bool AForward, double APosition, int ASpeedPercent) { }

        virtual public void RelativeMove(int APositionIndex) { }
        virtual public void AbsoluteMove(int APositionIndex) { }
        virtual public void RelativeMove(int APositionIndex, double APosition) { }
        virtual public void AbsoluteMove(int APositionIndex, double APosition) { }
        virtual public void RelativeMove(int APositionIndex, double APosition, int ASpeedPercent) { }
        virtual public void AbsoluteMove(int APositionIndex, double APosition, int ASpeedPercent) { }
        virtual public void RelativeMove(int APositionIndex, double APosition, int ASpeedPercent, double AVelocity) { }
        virtual public void AbsoluteMove(int APositionIndex, double APosition, int ASpeedPercent, double AVelocity) { }
        virtual public void AbsoluteMove(double position, double speed, double accerlate) { }
        #endregion

        #region 상태 변경
        virtual public void UpdateHomeDone(int AHomeDone)
        {
            if (AHomeDone < 0) { FHomeDone = -1; return; }
            if (AHomeDone > 0) { FHomeDone = 1; return; }

            FHomeDone = 0;
        }
        virtual public void UpdateInpoistion(bool AInpoistion)
        {
            if (!AInpoistion)
            {
                FInposition = false;
                FMotionDone = false;
            }
        }
        virtual public void UpdateMotionDone(bool AMotionDone)
        {
            if (!AMotionDone)
            {
                FMotionDone = false;
                FInposition = false;
            }
        }
        virtual public void UpdateStatus(bool AOnReady, bool AOnAlarm, bool AOnOrgin, bool AOnPositiveLimit, bool AOnNegativeLimit, int AHomeDone, bool AInposition, bool AMotionDone, double ACommandPosition, double AEncoderPosition)
        {
            FOnReady = AOnReady;
            FOnAlarm = AOnAlarm;
            FOnOrigin = AOnOrgin;
            FHomeDone = AHomeDone;
            FInposition = AInposition;
            FMotionDone = AMotionDone;
            FOnPositiveLimit = AOnPositiveLimit;
            FOnNegativeLimit = AOnNegativeLimit;

            FCommandPosition = ACommandPosition;
            FEncoderPosition = AEncoderPosition;

            FInpositionEx = (FOnReady && FInposition && (FHomeDone > 0));
        }
        #endregion

        #region 파라미터 저장 및 불러오기
        virtual public void SaveParameter(XmlDocument ADoc, XmlNode ANode)
        {
            XmlNode item = ADoc.CreateElement("ITEM");
            XmlAttribute attr = ADoc.CreateAttribute("ID");

            attr.Value = FMotionIndex.ToString();
            item.Attributes.Append(attr);

            CXML.AddElement(ADoc, item, "NAME", FMotionName);
            CXML.AddElement(ADoc, item, "ENABLED", FEnabled);
            SaveParameterEx(ADoc, item);

            CXML.AddElement(ADoc, item, "EMERGENCY_ENABLE", FEmergencyEnable);
            CXML.AddElement(ADoc, item, "SOFT_LIMIT_ENABLE", FSoftLimitEnable);
            CXML.AddElement(ADoc, item, "INPOSITION_ENABLE", FInpositionEnable);
            CXML.AddElement(ADoc, item, "SERVO_OFF_ON_EMERGENCY_ENABLE", FServoOffOnEmergencyEnable);

            CXML.AddElement(ADoc, item, "SERVO_LEVEL", FServoLevel);
            CXML.AddElement(ADoc, item, "ALARM_LEVEL", FAlarmLevel);
            CXML.AddElement(ADoc, item, "ORIGIN_LEVEL", FOriginLevel);
            CXML.AddElement(ADoc, item, "EMERGENCY_LEVEL", FEmergencyLevel);
            CXML.AddElement(ADoc, item, "INPOSITION_LEVEL", FInpositionLevel);
            CXML.AddElement(ADoc, item, "POSITIVE_LIMIT_LEVEL", FPositiveLimitLevel);
            CXML.AddElement(ADoc, item, "NEGATIVE_LIMIT_LEVEL", FNegativeLimitLevel);

            CXML.AddElement(ADoc, item, "HOME_MODE1", FHomeMode1);
            CXML.AddElement(ADoc, item, "HOME_MODE2", FHomeMode2);
            CXML.AddElement(ADoc, item, "SPEED_MODE", FSpeedMode);
            CXML.AddElement(ADoc, item, "POSITIVE_LIMIT_MODE", FPositiveLimitMode);
            CXML.AddElement(ADoc, item, "NEGATIVE_LIMIT_MODE", FNegativeLimitMode);

            CXML.AddElement(ADoc, item, "ENCODER_REVERSE", FEncoderReverse);
            CXML.AddElement(ADoc, item, "PULSE_OUTPUT_METHOD", FPulseOutputMethod);
            CXML.AddElement(ADoc, item, "ENCODER_INPUT_METHOD", FEncoderInputMethod);

            CXML.AddElement(ADoc, item, "RATIO", FRatio);
            CXML.AddElement(ADoc, item, "MAX_VELOCITY", FMaxVelocity);
            CXML.AddElement(ADoc, item, "POSITION_ERROR", FPositionError);
            CXML.AddElement(ADoc, item, "INPOSITION_WIDTH", FInpositionWidth);
            CXML.AddElement(ADoc, item, "SOFT_POSITIVE_LIMIT", FSoftPositiveLimit);
            CXML.AddElement(ADoc, item, "SOFT_NEGATIVE_LIMIT", FSoftNegativeLimit);

            CXML.AddElement(ADoc, item, "HOME_OFF_SET", FHomeOffSet);
            CXML.AddElement(ADoc, item, "HOME_VELOCITY1", FHomeVelocity1);
            CXML.AddElement(ADoc, item, "HOME_VELOCITY2", FHomeVelocity2);
            CXML.AddElement(ADoc, item, "HOME_VELOCITY3", FHomeVelocity3);
            CXML.AddElement(ADoc, item, "HOME_ACCELERATE1", FHomeAccelerate1);
            CXML.AddElement(ADoc, item, "HOME_ACCELERATE2", FHomeAccelerate2);
            CXML.AddElement(ADoc, item, "HOME_DECELERATE1", FHomeDecelerate1);
            CXML.AddElement(ADoc, item, "HOME_DECELERATE2", FHomeDecelerate2);

            ANode.AppendChild(item);
        }
        virtual protected void SaveParameterEx(XmlDocument ADoc, XmlNode ANode)
        {

        }

        virtual public void OpenParameter(XmlNode ANode)
        {
            XmlNode item = ANode.SelectSingleNode($"./ITEM[@ID='{FMotionIndex}']");

            if (item != null)
            {
                CXML.GetInnerText(item, "NAME", out FMotionName);
                Enabled = CXML.GetInnerText(item, "ENABLED", FEnabled);

                EmergencyEnable = CXML.GetInnerText(item, "EMERGENCY_ENABLE", FEmergencyEnable);
                SoftLimitEnable = CXML.GetInnerText(item, "SOFT_LIMIT_ENABLE", FSoftLimitEnable);
                InpositionEnable = CXML.GetInnerText(item, "INPOSITION_ENABLE", FInpositionEnable);
                ServoOffOnEmergencyEnable = CXML.GetInnerText(item, "SERVO_OFF_ON_EMERGENCY_ENABLE", FServoOffOnEmergencyEnable);

                ServoLevel = CXML.GetInnerText(item, "SERVO_LEVEL", FServoLevel);
                AlarmLevel = CXML.GetInnerText(item, "ALARM_LEVEL", FAlarmLevel);
                OriginLevel = CXML.GetInnerText(item, "ORIGIN_LEVEL", FOriginLevel);
                EmergencyLevel = CXML.GetInnerText(item, "EMERGENCY_LEVEL", FEmergencyLevel);
                InpositionLevel = CXML.GetInnerText(item, "INPOSITION_LEVEL", FInpositionLevel);
                PositiveLimitLevel = CXML.GetInnerText(item, "POSITIVE_LIMIT_LEVEL", FPositiveLimitLevel);
                NegativeLimitLevel = CXML.GetInnerText(item, "NEGATIVE_LIMIT_LEVEL", FNegativeLimitLevel);

                HomeMode1 = CXML.GetInnerText(item, "HOME_MODE1", FHomeMode1);
                HomeMode2 = CXML.GetInnerText(item, "HOME_MODE2", FHomeMode2);
                SpeedMode = CXML.GetInnerText(item, "SPEED_MODE", FSpeedMode);
                PositiveLimitMode = CXML.GetInnerText(item, "POSITIVE_LIMIT_MODE", FPositiveLimitMode);
                NegativeLimitMode = CXML.GetInnerText(item, "NEGATIVE_LIMIT_MODE", FNegativeLimitMode);

                EncoderReverse = CXML.GetInnerText(item, "ENCODER_REVERSE", FEncoderReverse);
                PulseOutputMethod = CXML.GetInnerText(item, "PULSE_OUTPUT_METHOD", FPulseOutputMethod);
                EncoderInputMethod = CXML.GetInnerText(item, "ENCODER_INPUT_METHOD", FEncoderInputMethod);

                Ratio = CXML.GetInnerText(item, "RATIO", FRatio);
                MaxVelocity = CXML.GetInnerText(item, "MAX_VELOCITY", FMaxVelocity);
                FPositionError = CXML.GetInnerText(item, "POSITION_ERROR", FPositionError);
                FInpositionWidth = CXML.GetInnerText(item, "INPOSITION_WIDTH", FInpositionWidth);
                SoftPositiveLimit = CXML.GetInnerText(item, "SOFT_POSITIVE_LIMIT", FSoftPositiveLimit);
                SoftNegativeLimit = CXML.GetInnerText(item, "SOFT_NEGATIVE_LIMIT", FSoftNegativeLimit);

                HomeOffSet = CXML.GetInnerText(item, "HOME_OFF_SET", FHomeOffSet);
                HomeVelocity1 = CXML.GetInnerText(item, "HOME_VELOCITY1", FHomeVelocity1);
                HomeVelocity2 = CXML.GetInnerText(item, "HOME_VELOCITY2", FHomeVelocity2);
                HomeVelocity3 = CXML.GetInnerText(item, "HOME_VELOCITY3", FHomeVelocity3);
                HomeAccelerate1 = CXML.GetInnerText(item, "HOME_ACCELERATE1", FHomeAccelerate1);
                HomeAccelerate2 = CXML.GetInnerText(item, "HOME_ACCELERATE2", FHomeAccelerate2);
                HomeDecelerate1 = CXML.GetInnerText(item, "HOME_DECELERATE1", FHomeDecelerate1);
                HomeDecelerate2 = CXML.GetInnerText(item, "HOME_DECELERATE2", FHomeDecelerate2);

                OpenParameterEx(item);
            }
        }
        virtual protected void OpenParameterEx(XmlNode ANode)
        {

        }

        virtual public void SaveDefaultPosition(XmlDocument ADoc, XmlNode ANode)
        {
            XmlNode item = ADoc.CreateElement("ITEM");
            XmlAttribute attr = ADoc.CreateAttribute("ID");

            attr.Value = FMotionIndex.ToString();
            item.Attributes.Append(attr);

            for (int i = 0; i < FPositions.Length; i++) FPositions[i].SaveDefaultPosition(ADoc, item, i);
            ANode.AppendChild(item);
        }
        virtual public void OpenDefaultPosition(XmlNode ANode)
        {
            XmlNode item = ANode.SelectSingleNode($"./ITEM[@ID='{FMotionIndex}']");

            if (item != null)
            {
                for (int i = 0; i < FPositions.Length; i++) FPositions[i].OpenDefaultPosition(item, i);
            }
        }

        virtual public void Save(XmlDocument ADoc, XmlNode ANode)
        {
            XmlNode item = ADoc.CreateElement("ITEM");
            XmlAttribute attr = ADoc.CreateAttribute("ID");

            attr.Value = FMotionIndex.ToString();
            item.Attributes.Append(attr);

            for (int i = 0; i < FPositions.Length; i++)
            {
                FPositions[i].Save(ADoc, item, i);
            }
            ANode.AppendChild(item);
        }
        virtual public void Open(XmlNode ANode)
        {
            XmlNode item = ANode.SelectSingleNode($"./ITEM[@ID='{FMotionIndex}']");

            if (item != null)
            {
                for (int i = 0; i < FPositions.Length; i++) FPositions[i].Open(item, i);
            }
        }

        virtual public void Decoding(XmlNode ASrcNode, XmlDocument ADoc, XmlNode ANode)
        {
            XmlNode srcitem = ASrcNode.SelectSingleNode($"./ITEM[@ID='{FMotionIndex}']");
            if (srcitem != null)
            {
                XmlNode item = ANode.SelectSingleNode($"/ROOT/ITEM[@ID='{FMotionIndex}']");
                if (item != null)
                {
                    for (int i = 0; i < FPositions.Length; i++) FPositions[i].Decoding(srcitem, ADoc, item, i);
                }
            }
        }
        virtual public void Encoding(XmlNode ASrcNode, XmlDocument ADoc, XmlNode ANode)
        {
            XmlNode srcitem = ASrcNode.SelectSingleNode($"/ROOT/ITEM[@ID='{FMotionIndex}']");

            if (srcitem != null)
            {
                XmlNode item = CXML.CreateElement(ADoc, "ITEM", FMotionIndex.ToString());
                CXML.AddElement(ADoc, item, "NAME", FMotionName);
                ANode.AppendChild(item);

                for (int i = 0; i < FPositions.Length; i++) FPositions[i].Encoding(srcitem, ADoc, item, i);
            }
        }
        #endregion

        #region Position Info Class
        protected bool FSpeedUnitIsPercent = true;
        virtual protected bool GetSpeedUnitIsPercent()
        {
            return FSpeedUnitIsPercent;
        }
        virtual protected void SetSpeedUnitIsPercent(bool ASet)
        {
            FSpeedUnitIsPercent = ASet;
        }
        public bool SpeedUnitIsPercent { get { return GetSpeedUnitIsPercent(); } set { SetSpeedUnitIsPercent(value); } }

        /// < Caution >     FMotionBasePosition[0] : Jog Speed
        ///                 FMotionBasePosition[1] : Position Speed 0
        ///                 FMotionBasePosition[2] : Position Speed 1





        public double PositionCompare(int AIndex, bool AIndexCheck = true, bool AHomeDoneCheck = true, double ADefReturnPosition = 999999.999)
        {
            if (AIndex < 0) return ADefReturnPosition;
            if (AIndex >= FPositions.Length) return ADefReturnPosition;

            if (AHomeDoneCheck && FHomeDone <= 0) return ADefReturnPosition;
            if (AIndexCheck && FPositionIndex != AIndex) return ADefReturnPosition;

#if (__SIMULATION__ == true)
            return 0.0;
#else
            return Math.Abs(FPositions[AIndex].Position - EncoderPosition);
#endif
        }
        #endregion

    }

    class CMotionBasic : IDeviceAccompany, IDisposable
    {
        public const UInt32 __MOTION_MAX_AXIS_COUNT__ = 30;

        static public string Version
        {
            get { return "MOTION BASIC - sean.kim(V25.08.28.001)"; }
        }

        public CMotionBasic(int AMotionCount) : base()
        {
            FDirectory = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\CONFIG\\";
            if (!System.IO.Directory.Exists(FDirectory)) System.IO.Directory.CreateDirectory(FDirectory);

            FItem = new CMotionItemBasic[AMotionCount];
            for (int i = 0; i < FItem.Length; i++) FItem[i] = new CMotionItemBasic(i);

            OpenParameter();
            OpenDefaultPosition();
        }

        #region Dispose 구문
        protected bool FDisposed = false;

        public void Dispose()
        {
            Dispose(true);
        }
        protected virtual void Dispose(bool ADisposing)
        {
            if (FDisposed) return;
            if (ADisposing) { /* IDisposable 인터페이스를 구현하는 멤버들을 여기서 정리합니다. */}

            FDisposed = true;
        }
        #endregion

        #region 디바이스와 연동 부분
        public bool DeviceOpen(string APath)
        {
            Open(APath);
            return true;
        }
        public bool DeviceDelete(string APath)
        {
            string file = $"{APath}POSITION.XML";
            try
            {
                System.IO.File.Delete(file);

            }
            catch
            {
            }

            if (System.IO.File.Exists(file)) return false;
            return true;
        }
        public bool DeviceOpenPossible(string APath)
        {
            string file = $"{APath}POSITION.XML";

            if (!System.IO.File.Exists(file)) return false;
            return true;
        }
        public bool DeviceCopy(string ASrcPath, string ADesPath)
        {
            string srcfile = $"{ASrcPath}POSITION.XML";
            string desfile = $"{ADesPath}POSITION.XML";

            if (!System.IO.Directory.Exists(ADesPath)) System.IO.Directory.CreateDirectory(ADesPath);
            try
            {
                System.IO.File.Copy(srcfile, desfile, true);
            }
            catch
            {

            }

            if (!System.IO.File.Exists(desfile)) return false;
            return true;
        }

        public bool Decoding(string APath, XmlNode ANode)
        {
            XmlNode srcitems = CXML.SelectSingleNode(ANode, "/ROOT/MOTION");
            if (srcitems != null)
            {
                string file = $"{APath}POSITION.XML";
                if (!System.IO.File.Exists(file)) Save(APath);

                XmlDocument xdoc = new XmlDocument();
                xdoc.Load(file);

                foreach (CMotionItemBasic item in FItem) item.Decoding(srcitems, xdoc, xdoc);
                xdoc.Save(file);
            }
            return true;
        }
        public bool Encoding(string APath, XmlDocument ADoc, XmlNode ANode)
        {
            XmlNode motion = ADoc.CreateElement("MOTION");
            ANode.AppendChild(motion);

            bool ret = true;
            string file = $"{APath}POSITION.XML";
            if (!System.IO.File.Exists(file)) ret = false;
            else
            {
                XmlDocument srcxdoc = new System.Xml.XmlDocument();
                srcxdoc.Load(file);

                foreach (CMotionItemBasic item in FItem) item.Encoding(srcxdoc, ADoc, motion);
            }
            return ret;
        }
        #endregion

        #region 파라미터 저장 및 불러오기
        protected string FDirectory = "";
        virtual public string Directory { get { return FDirectory; } set { FDirectory = value; } }

        virtual public void SaveParameter()
        {
            XmlDocument xdoc = new System.Xml.XmlDocument();
            xdoc.AppendChild(xdoc.CreateXmlDeclaration("1.0", "UTF-8", "yes"));

            XmlNode root = CXML.CreateElement(xdoc, "ROOT");
            xdoc.AppendChild(root);

            XmlNode basic = CXML.CreateElement(xdoc, "BASIC");
            CXML.AddElement(xdoc, basic, "VERSION", Version);
            root.AppendChild(basic);

            foreach (CMotionItemBasic item in FItem)
            {
                if (item == null) break;
                try
                {
                    item.SaveParameter(xdoc, root);
                }
                catch (Exception e)
                {
                    System.Windows.Forms.MessageBox.Show(e.ToString(), "에러", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }

            }
            xdoc.Save($"{FDirectory}MOTION PARAMETER.XML");
        }
        virtual public void OpenParameter()
        {
            string file = $"{FDirectory}MOTION PARAMETER.XML";
            if (!System.IO.File.Exists(file)) { SaveParameter(); return; }

            XmlDocument xdoc = new System.Xml.XmlDocument();
            xdoc.Load(file);

            string version = "";
            XmlNode basic = xdoc.SelectSingleNode($"/ROOT/BASIC");
            if (basic != null) CXML.GetInnerText(basic, "VERSION", out version);

            if (version != Version)
            {
                System.Windows.Forms.MessageBox.Show($"버젼({Version} : {version})이 다릅니다.\r\n정보 불러오기에 문제가 발생될 수 있습니다.", "경고", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
            }

            XmlNode root = xdoc.SelectSingleNode($"/ROOT");
            foreach (CMotionItemBasic item in FItem)
            {
                if (item == null) break;
                item.OpenParameter(root);
            }
        }

        virtual public void SaveDefaultPosition()
        {
            XmlDocument xdoc = new System.Xml.XmlDocument();
            xdoc.AppendChild(xdoc.CreateXmlDeclaration("1.0", "UTF-8", "yes"));

            XmlNode root = CXML.CreateElement(xdoc, "ROOT");
            xdoc.AppendChild(root);

            XmlNode basic = CXML.CreateElement(xdoc, "BASIC");
            CXML.AddElement(xdoc, basic, "VERSION", Version);
            root.AppendChild(basic);

            foreach (CMotionItemBasic item in FItem)
            {
                if (item == null) break;
                item.SaveDefaultPosition(xdoc, root);
            }
            xdoc.Save($"{FDirectory}DEFAULT POSITION.XML");
        }
        virtual public void OpenDefaultPosition()
        {
            string file = $"{FDirectory}DEFAULT POSITION.XML";
            if (!System.IO.File.Exists(file)) { SaveDefaultPosition(); return; }

            XmlDocument xdoc = new System.Xml.XmlDocument();
            xdoc.Load(file);

            string version = "";
            XmlNode basic = xdoc.SelectSingleNode($"/ROOT/BASIC");
            if (basic != null) CXML.GetInnerText(basic, "VERSION", out version);

            if (version != Version)
            {
                System.Windows.Forms.MessageBox.Show($"버젼({Version} : {version})이 다릅니다.\r\n정보 불러오기에 문제가 발생될 수 있습니다.", "경고", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
            }

            XmlNode root = xdoc.SelectSingleNode($"/ROOT");
            foreach (CMotionItemBasic item in FItem)
            {
                if (item == null) break;
                item.OpenDefaultPosition(root);
            }
        }

        virtual public void Save(string APath)
        {
            if (!System.IO.Directory.Exists(APath)) System.IO.Directory.CreateDirectory(APath);

            try
            {
                XmlDocument xdoc = new System.Xml.XmlDocument();
                xdoc.AppendChild(xdoc.CreateXmlDeclaration("1.0", "UTF-8", "yes"));

                XmlNode root = CXML.CreateElement(xdoc, "ROOT");
                xdoc.AppendChild(root);

                XmlNode basic = CXML.CreateElement(xdoc, "BASIC");
                CXML.AddElement(xdoc, basic, "VERSION", Version);
                root.AppendChild(basic);

                foreach (CMotionItemBasic item in FItem)
                {
                    if (item == null) break;
                    item.Save(xdoc, root);
                }
                xdoc.Save($"{APath}POSITION.XML");
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString(), "에러", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
        virtual public void Open(string APath)
        {
            string file = $"{APath}POSITION.XML";
            if (!System.IO.File.Exists(file)) { Save(APath); return; }

            XmlDocument xdoc = new System.Xml.XmlDocument();
            xdoc.Load(file);

            string version = "";
            XmlNode basic = xdoc.SelectSingleNode($"/ROOT/BASIC");
            if (basic != null) CXML.GetInnerText(basic, "VERSION", out version);

            if (version != Version)
            {
                System.Windows.Forms.MessageBox.Show($"버젼({Version} : {version})이 다릅니다.\r\n정보 불러오기에 문제가 발생될 수 있습니다.", "경고", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
            }

            XmlNode root = xdoc.SelectSingleNode($"/ROOT");
            foreach (CMotionItemBasic item in FItem)
            {
                if (item == null) break;
                item.Open(root);
            }
        }
        #endregion

        public int MotionCount { get { return FItem.Length; } }
        protected CMotionItemBasic[] FItem = null;
        public CMotionItemBasic this[int AIndex]
        {
            get
            {
                if (AIndex < 0) return null;
                if (AIndex >= FItem.Length) return null;

                return FItem[AIndex];
            }
        }

        protected bool FInitialized = false;

        virtual public bool Initialize()
        {
            FInitialized = false;
            return FInitialized;
        }
        public virtual bool Initialized
        {
            get { return FInitialized; }
            set { FInitialized = value; }
        }

        virtual public int GetStatus(bool AEmergency)
        {
            int ret = 0x00;
            for (int i = 0; i < FItem.Length; i++)
            {
                if (!FItem[i].GetStatus(AEmergency)) ret |= (0x01 << i);
            }
            return ret;
        }
    }
}
