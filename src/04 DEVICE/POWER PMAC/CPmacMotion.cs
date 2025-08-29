using Autofac.Features.OwnedInstances;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EGGPLANT.Device.PowerPmac
{
    public class CPMacMotionItem : CMotionItemBasic
    {
        public CPMacMotionItem(CPmacMotion AOwner, int AMotionIndex, string AMotionName = "") : base(AMotionIndex, AMotionName)
        {

            FOwner = AOwner;

            FSoftLimitEnable = false;
            FInpositionEnable = true;
            FServoOffOnEmergencyEnable = true;

            FSpeedMode = 1;
            FEncoderReverse = false;

            FServoLevel = false;
            FAlarmLevel = false;
            FInpositionLevel = true;
            FPositiveLimitLevel = false;
            FNegativeLimitLevel = false;

            FRatio = 1.0;
            FGravity = 1.0;
            FMaxVelocity = +1000000.0;
            FSoftPositiveLimit = +1000.0;
            FSoftNegativeLimit = -1000.0;

            FHomeOffSet = 0.0;
            FHomeVelocity1 = FHomeVelocity2 = 10.0;
            FHomeAccelerate1 = FHomeAccelerate2 = 100.0;
            FHomeDecelerate1 = FHomeDecelerate2 = 100.0;

            FServoOn = false;                              //ampEna
            FOnReady = false;							   //ClosedLoop
            FOnAlarm = false;							   //AmpFault
            FOnOrigin = false;                             //Home senser
            FInposition = false;						   //DesVelZero + Inpos
            FInpositionEx = false;						   //DesVelZero + Inpos + HomeComplete
            FEmergencyStatus = false;
            FOnPositiveLimit = false;					   //PlusLimit 
            FOnNegativeLimit = false;					   //MinusLimit

            FHomeDone = 0;								   //HomeComplete
            FCommandPosition = 0;					       //
            FEncoderPosition = 0;						   //
            FAlarmClearOffDelay = 0;					   //
            FServoOffDelayOnEmergency = 0;				   //

            FPositionIndex = -1;
            FNextPositionIndex = -1;

            FHomeStep = 0;
            FHomeDelay = 0;
            FDoneDelay = 0;
            FSetDoneDelay = 0;

            FActPosition = 0;
            FDacPosition = 0;
            FHomePosision = 0;

            FHomeProgress = false;

        }

        private CPmacMotion FOwner;
        public CPmacMotion Owner { get { return FOwner; } }

        #region 파라미터 저장 및 불러오기
        override public void SaveParameter(XmlDocument ADoc, XmlNode ANode)
        {
            XmlNode item = ADoc.CreateElement("ITEM");
            XmlAttribute attr = ADoc.CreateAttribute("ID");

            attr.Value = MotionIndex.ToString();
            item.Attributes.Append(attr);

            CXML.AddElement(ADoc, item, "NAME", MotionName);
            CXML.AddElement(ADoc, item, "ENABLED", FEnabled);

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
            CXML.AddElement(ADoc, item, "GRAVITY", FGravity);
            CXML.AddElement(ADoc, item, "MAX_VELOCITY", FMaxVelocity);
            CXML.AddElement(ADoc, item, "POSITION_ERROR", FPositionError);
            CXML.AddElement(ADoc, item, "SOFT_POSITIVE_LIMIT", FSoftPositiveLimit);
            CXML.AddElement(ADoc, item, "SOFT_NEGATIVE_LIMIT", FSoftNegativeLimit);

            CXML.AddElement(ADoc, item, "HOME_LOGIC_COMMAND", FHomeLogicCommand);
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
        override public void OpenParameter(XmlNode ANode)
        {
            XmlNode item = ANode.SelectSingleNode($"./ITEM[@ID='{MotionIndex}']");

            if (item != null)
            {
                CXML.GetInnerText(item, "NAME", out FMotionName);

                CXML.GetInnerText(item, "ENABLED", out FEnabled);
                CXML.GetInnerText(item, "EMERGENCY_ENABLE", out FEmergencyEnable);
                CXML.GetInnerText(item, "SOFT_LIMIT_ENABLE", out FSoftLimitEnable);
                CXML.GetInnerText(item, "INPOSITION_ENABLE", out FInpositionEnable);
                CXML.GetInnerText(item, "SERVO_OFF_ON_EMERGENCY_ENABLE", out FServoOffOnEmergencyEnable);

                CXML.GetInnerText(item, "SERVO_LEVEL", out FServoLevel);
                CXML.GetInnerText(item, "ALARM_LEVEL", out FAlarmLevel);
                CXML.GetInnerText(item, "ORIGIN_LEVEL", out FOriginLevel);
                CXML.GetInnerText(item, "EMERGENCY_LEVEL", out FEmergencyLevel);
                CXML.GetInnerText(item, "INPOSITION_LEVEL", out FInpositionLevel);
                CXML.GetInnerText(item, "POSITIVE_LIMIT_LEVEL", out FPositiveLimitLevel);
                CXML.GetInnerText(item, "NEGATIVE_LIMIT_LEVEL", out FNegativeLimitLevel);

                CXML.GetInnerText(item, "HOME_MODE1", out FHomeMode1);
                CXML.GetInnerText(item, "HOME_MODE2", out FHomeMode2);
                CXML.GetInnerText(item, "SPEED_MODE", out FSpeedMode);
                CXML.GetInnerText(item, "POSITIVE_LIMIT_MODE", out FPositiveLimitMode);
                CXML.GetInnerText(item, "NEGATIVE_LIMIT_MODE", out FNegativeLimitMode);

                CXML.GetInnerText(item, "ENCODER_REVERSE", out FEncoderReverse);
                CXML.GetInnerText(item, "PULSE_OUTPUT_METHOD", out FPulseOutputMethod);
                CXML.GetInnerText(item, "ENCODER_INPUT_METHOD", out FEncoderInputMethod);

                CXML.GetInnerText(item, "RATIO", out FRatio);
                CXML.GetInnerText(item, "GRAVITY", out FGravity);
                CXML.GetInnerText(item, "MAX_VELOCITY", out FMaxVelocity);
                CXML.GetInnerText(item, "POSITION_ERROR", out FPositionError);
                CXML.GetInnerText(item, "SOFT_POSITIVE_LIMIT", out FSoftPositiveLimit);
                CXML.GetInnerText(item, "SOFT_NEGATIVE_LIMIT", out FSoftNegativeLimit);

                CXML.GetInnerText(item, "HOME_LOGIC_COMMAND", out FHomeLogicCommand);
                CXML.GetInnerText(item, "HOME_OFF_SET", out FHomeOffSet);
                CXML.GetInnerText(item, "HOME_VELOCITY1", out FHomeVelocity1);
                CXML.GetInnerText(item, "HOME_VELOCITY2", out FHomeVelocity2);
                CXML.GetInnerText(item, "HOME_VELOCITY3", out FHomeVelocity3);
                CXML.GetInnerText(item, "HOME_ACCELERATE1", out FHomeAccelerate1);
                CXML.GetInnerText(item, "HOME_ACCELERATE2", out FHomeAccelerate2);
                CXML.GetInnerText(item, "HOME_DECELERATE1", out FHomeDecelerate1);
                CXML.GetInnerText(item, "HOME_DECELERATE2", out FHomeDecelerate2);
            }
        }
        #endregion

        //-------------------------------------------------------------------------------------------

        #region Set Parameter [Enabled]
        public override bool SoftLimitEnable
        {
            get { return FSoftLimitEnable; }
            set { FSoftLimitEnable = value; }
        }
        public override bool InpositionEnable
        {
            get { return FInpositionEnable; }
            set { FInpositionEnable = value; }
        }
        #endregion

        //-------------------------------------------------------------------------------------------

        #region Set Parameter [       ]
        public override bool EncoderReverse
        {
            get { return FEncoderReverse; }
            set { FEncoderReverse = value; }
        }
        #endregion

        //-------------------------------------------------------------------------------------------

        #region Set Parameter [Level  ]
        public override bool ServoLevel
        {
            get { return FServoLevel; }
            set { FServoLevel = value; }
        }
        public override bool AlarmLevel
        {
            get { return FAlarmLevel; }
            set { FAlarmLevel = value; }
        }
        public override bool InpositionLevel
        {
            get { return FInpositionLevel; }
            set { FInpositionLevel = value; }
        }
        public override bool PositiveLimitLevel
        {
            get { return FPositiveLimitLevel; }
            set { FPositiveLimitLevel = value; }
        }
        public override bool NegativeLimitLevel
        {
            get { return FNegativeLimitLevel; }
            set { FNegativeLimitLevel = value; }
        }

        #endregion

        //-------------------------------------------------------------------------------------------

        #region Set Infomation [       ]
        public double FGravity = 1;  //중력 변수 

        public override double Ratio
        {
            get { return FRatio; }
            set { FRatio = value; }
        }
        public override double MaxVelocity
        {
            get { return FMaxVelocity; }
            set { FMaxVelocity = value; }
        }
        public override double SoftPositiveLimit
        {
            get { return FSoftPositiveLimit; }
            set
            {
                FSoftPositiveLimit = value;
                if (FSoftLimitEnable) FOwner.PMacCommand("Motor[" + (MotionIndex + 1).ToString() + "].MaxPos = " + FSoftPositiveLimit.ToString());
            }
        }
        public override double SoftNegativeLimit
        {
            get { return FSoftNegativeLimit; }
            set
            {
                FSoftNegativeLimit = value;
                if (FSoftLimitEnable) Owner.PMacCommand("Motor[" + (MotionIndex + 1).ToString() + "].MinPos = " + FSoftNegativeLimit.ToString());
            }
        }
        #endregion

        //-------------------------------------------------------------------------------------------

        #region Set Servo On
        public override bool ServoOn
        {
            get { return FServoOn; }
            set
            {
                if (FOwner.Initialized)
                {
                    if (FIsHomeBufferRun) HomeStop();

                    if (!value)
                    {
                        FHomeDone = 0;
                        FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "K");
                    }
                    else
                    {
                        FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J/");
                    }
                    FServoOn = value;
                }
            }
        }
        #endregion
        //-------------------------------------------------------------------------------------------

        #region Motion Status
        private int FPMacMotionIndex = 0;
        public int PMacMotionIndex { get { return FPMacMotionIndex; } }

        new public bool Inposition { get { return FInposition; } set { FInposition = value; } }
        new public bool MotionDone { get { return FMotionDone; } set { FMotionDone = value; } }
        new public bool InpositionEx { get { return FInpositionEx; } set { FInpositionEx = value; } }

        public override double CommandPosition
        {
            get { return FCommandPosition; }
            set { FCommandPosition = value; }
        }
        public override double EncoderPosition
        {
            get { return FEncoderPosition; }
            set { FEncoderPosition = value; }
        }

        private double FActPosition = 0.0;
        public double ActualPosition
        {
            get { return FActPosition; }
            set { FActPosition = value; }
        }

        private double FDacPosition = 0.0;
        public double DacPosition
        {
            get { return FDacPosition; }
            set { FDacPosition = value; }
        }

        private double FHomePosision = 0.0;
        public double HomePosision
        {
            get { return FHomePosision; }
            set { FHomePosision = value; }
        }

        private bool FIsHomeBufferRun = false;
        public bool IsHomeBufferRun { get { return FIsHomeBufferRun; } }

        private bool FHomeProgress = false;
        public bool HomeProgress
        {
            get { return FHomeProgress; }
            set { FHomeProgress = value; }
        }

        private bool FHomeComplete = false;
        public bool HomeComplete
        {
            get { return FHomeComplete; }
            set { FHomeComplete = value; }
        }

        public override void AlarmClear()
        {
            //FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J/");
        }

        public void EncodeClear()
        {
            if (!FOwner.Initialized) return;

            if (FOnAlarm) return;
            if (!FOnReady) return;
            if (!FInposition) return;
            FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "Homez");
        }
        public void HomeCommand()
        {
            if (!FOwner.Initialized) return;

            if (FOnAlarm) return;
            if (!FOnReady) return;
            if (!FInposition) return;
            FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "hm");
        }

        public void HomeLogsicCommand(bool AStart)
        {
            if (!FOwner.Initialized) return;

            if (FOnAlarm) return;
            if (!FOnReady) return;
            if (!FInposition) return;
            if (FHomeLogicCommand == null) return;
            if (AStart) FOwner.PMacCommand("enable " + FHomeLogicCommand);
            else FOwner.PMacCommand("disable " + FHomeLogicCommand);
        }

        #endregion

        //-------------------------------------------------------------------------------------------

        #region Home Process        
        private string FHomeLogicCommand = "";  // Home 동작 명령 커멘드     

        public override bool HomeStart()
        {
            if (!FOwner.Initialized) { return false; }

            if (!FInposition) { Stop(false, true); return false; }
            if (!MotionDone) return false;

            if (FHomeStep != 0)
            {
                HomeStop();
                FHomeStep = 0;
                return false;
            }
            FHomeDone = 0;
            FHomeStep = 1;
            return true;
        }
        public bool HomeStop()
        {
            if (!FOwner.Initialized) { return false; }

            HomeLogsicCommand(false);
            SoftPositiveLimit = FSoftPositiveLimit;
            SoftNegativeLimit = FSoftNegativeLimit;
            FHomeStep = 0;
            FDoneDelay = FSetDoneDelay;
            return true;
        }
        protected override void HomeProc()
        {
            if (FHomeDelay > 0) { FHomeDelay--; return; }

            switch (FHomeStep)
            {
                case 0:
                    break;
                case 1:
                    FNextPositionIndex = -1;
                    FPositionIndex = -1;
                    FHomeDone = -1;
                    FHomeStep++;
                    break;
                case 2:
                    HomeLogsicCommand(true);
                    FHomeDelay = 10;
                    FHomeStep++;
                    break;
                case 3:
                    //home 완료 변수 정의 필요 
                    if (FHomeProgress) break;

                    FDoneDelay = FSetDoneDelay;
                    FHomeStep++;
                    break;
                case 4:
                    if (FHomeDone != 1) break;
                    FHomeStep = 0;
                    break;

                default:
                    FHomeStep = 0;
                    break;
            }
        }
        #endregion

        //-------------------------------------------------------------------------------------------

        #region Motion Move
        private int FDoneDelay;
        private int FSetDoneDelay;

        public override void Stop(bool AEmergencyStop = true, bool AHomeFail = false)
        {
            if (!FOwner.Initialized) return;

            if (AEmergencyStop)
            {
                FHomeDone = 0;
                FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "K");

            }
            else
            {
                FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J/");
            }
            if (AHomeFail)
            {
                FHomeDone = 0;
                FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "K");

            }

            if (FHomeStep != 0)
            {
                FHomeDone = 0;
                FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J/");
            }
            if (FHomeStep != 0)
            {
                HomeStop();
                FHomeStep = 0;
            }

            FNextPositionIndex = -1;
            FPositionIndex = -1;

            FDoneDelay = FSetDoneDelay;
            FInpositionEx = false;
            FInposition = false;
        }

        private void SetSpeed(double AVelocity, double AAccelerate, double ADecelerate, double AJerk)
        {
            //가속 단위가 ms 이기 때문에 s -> ms로 변환 
            AAccelerate = AAccelerate * 1000;
            ADecelerate = ADecelerate * 1000;
            AJerk = AJerk * 1000;

            //중력이 1이 아닐 경우, 중력값을 나눠, 가속 시간을 변경 시킨다. 
            if (FGravity != 1)
            {
                AAccelerate = AVelocity / FGravity / 2;
            }

            String stringcmd = "";

            stringcmd = " I" + (MotionIndex + 1).ToString() + "20=" + (AAccelerate).ToString();
            stringcmd += " I" + (MotionIndex + 1).ToString() + "21=" + (AJerk).ToString();
            stringcmd += " I" + (MotionIndex + 1).ToString() + "22=" + (AVelocity).ToString();

            FOwner.PMacCommand(stringcmd);
        }

        public override void JogMove(bool AForward, int ASpeedPercent)
        {
            if (!FOwner.Initialized) return;

            if (FOnAlarm) return;
            if (!FOnReady) return;
            if (!FInposition) return;

            double velocity = FPositions[0].Velocity * ASpeedPercent / 100;
            SetSpeed(velocity, FPositions[0].Accelerate / 1000.0, FPositions[0].Decelerate / 1000.0, FPositions[0].Jerk / 1000.0);

            if (!AForward)
            {
                FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J-");
            }
            else
            {
                FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J+");
            }

            FNextPositionIndex = -1;
            FPositionIndex = -1;

            FDoneDelay = FSetDoneDelay;
            FInpositionEx = false;
            FInposition = false;
        }
        public override void VelocityMove(int APositionIndex, bool AForward)
        {
            if (!FOwner.Initialized) return;

            if (FOnAlarm) return;
            if (!FOnReady) return;
            if (!FInposition) return;

#if (__TURBO_PMAC__ == false)
            CMotionPositionBasic position = FPositions[APositionIndex];
            if (position.SpeedPercent < 1) position.SpeedPercent = 1;
            if (position.SpeedPercent > 100) position.SpeedPercent = 100;

            double velocity = 0.0;
            switch (FSpeedMode)
            {
                case 0:
                    FVelocity = FMaxVelocity * (double)position.SpeedPercent / 100.0;

                    velocity = FVelocity;
                    if (!AForward) velocity *= -1.0;

                    SetSpeed(velocity, 0.1, 0.1, 0); FStopDecelerate = 0.1;
                    if (velocity > 0)
                    {
                        FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J+");
                    }
                    else
                    {
                        FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J-");
                    }
                    break;
                case 1:
                case 3:
                    velocity = FPositions[1].Velocity * (double)position.SpeedPercent / 100.0;
                    if (!AForward) velocity *= -1.0;

                    FStopDecelerate = FPositions[1].Decelerate / 1000.0;
                    SetSpeed(velocity, FPositions[1].Accelerate / 1000.0, FPositions[1].Decelerate / 1000.0, FPositions[1].Jerk / 1000.0);

                    if (velocity > 0)
                    {
                        FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J+");
                    }
                    else
                    {
                        FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J-");
                    }
                    break;
                case 2:
                case 4:
                    velocity = position.Velocity * (double)position.SpeedPercent / 100.0;
                    if (!AForward) velocity *= -1.0;

                    FStopDecelerate = position.Decelerate / 1000.0;
                    SetSpeed(velocity, position.Accelerate / 1000.0, position.Decelerate / 1000.0, position.Jerk / 1000.0);

                    if (velocity > 0)
                    {
                        FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J+");
                    }
                    else
                    {
                        FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J-");
                    }
                    break;
                case 5:
                    velocity = position.Velocity * (double)position.SpeedPercent / 100.0;
                    if (!AForward) velocity *= -1.0;

                    SetSpeed(velocity, FPositions[1].Accelerate / 1000.0, FPositions[1].Accelerate / 1000.0, FPositions[1].Jerk / 1000.0);
                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J:" + velocity.ToString());
                    break;

                case 11:
                    velocity = position.Velocity;
                    if (!AForward) velocity *= -1.0;

                    FStopDecelerate = FPositions[1].Decelerate / 1000.0;
                    SetSpeed(velocity, FPositions[1].Accelerate / 1000.0, FPositions[1].Accelerate / 1000.0, FPositions[1].Jerk / 1000.0);

                    if (velocity > 0)
                    {
                        FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J+");
                    }
                    else
                    {
                        FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J-");
                    }
                    break;
            }
#endif

            FNextPositionIndex = -1;
            FPositionIndex = -1;

            FDoneDelay = FSetDoneDelay;
            FInpositionEx = false;
            FInposition = false;
        }
        public void VelocityMove(double AVelocity, double AAccelerate, double ADecelerate, double AJerk)
        {
            if (!FOwner.Initialized) return;

            if (FOnAlarm) return;
            if (!FOnReady) return;
            if (!FInposition) return;

            SetSpeed(AVelocity, AAccelerate / 1000.0, ADecelerate / 1000.0, AJerk);
            if (AVelocity > 0)
            {
                FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J+");
            }
            else
            {
                FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J-");
            }
            FNextPositionIndex = -1;
            FPositionIndex = -1;

            FDoneDelay = FSetDoneDelay;
            FInpositionEx = false;
            FInposition = false;
        }
        public override void JogRelativeMove(bool AForward, double APosition, int ASpeedPercent)
        {
            if (!FOwner.Initialized) return;

            if (FOnAlarm) return;
            if (!FOnReady) return;
            if (!FInposition) return;

            if (!AForward) APosition *= -1.0f;
            if (ASpeedPercent < 1) ASpeedPercent = 1;
            if (ASpeedPercent > 100) ASpeedPercent = 100;

            switch (FSpeedMode)
            {
                case 0:
                    FVelocity = FMaxVelocity * (double)ASpeedPercent / 100.0;

                    FStopDecelerate = 0.1;
                    SetSpeed(FVelocity, 0.1, 0.1, 0);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J:" + (APosition / Ratio).ToString());
                    break;
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 11:
                    CMotionPositionBasic position = FPositions[0];
                    SetSpeed(position.Velocity * (double)ASpeedPercent / 100.0, position.Accelerate / 1000.0, position.Decelerate / 1000.0, position.Jerk / 1000.0);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J:" + (APosition / Ratio).ToString());
                    break;
            }


            FNextPositionIndex = -1;
            FPositionIndex = -1;

            FDoneDelay = FSetDoneDelay;
            FInpositionEx = false;
            FInposition = false;
        }

        public override void RelativeMove(int APositionIndex)
        {
            if (!FOwner.Initialized) return;

            if (FOnAlarm) return;
            if (!FOnReady) return;
            if (!FInpositionEx) return;

            if (APositionIndex <= 0) return;
            if (APositionIndex >= FPositions.Length) return;

            CMotionPositionBasic position = FPositions[APositionIndex];
            if (position.SpeedPercent < 1) position.SpeedPercent = 1;
            if (position.SpeedPercent > 100) position.SpeedPercent = 100;

            switch (FSpeedMode)
            {
                case 0:
                    FVelocity = FMaxVelocity * (double)position.SpeedPercent / 100.0;
                    SetSpeed(FVelocity, 0.1, 0.1, 0);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J:" + (position.Position / Ratio).ToString());
                    break;
                case 1:
                case 2:
                    FStopDecelerate = FPositions[1].Decelerate / 1000.0;
                    SetSpeed(FPositions[1].Velocity * (double)position.SpeedPercent / 100.0, FPositions[1].Accelerate / 1000.0, FPositions[1].Decelerate / 1000.0, FPositions[1].Jerk / 1000.0);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J:" + (position.Position / Ratio).ToString());
                    break;
                case 3:
                case 4:
                    FStopDecelerate = position.Decelerate / 1000.0;
                    SetSpeed(position.Velocity * (double)position.SpeedPercent / 100.0, position.Accelerate / 1000.0, position.Decelerate / 1000.0, position.Jerk / 1000.0);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J:" + (position.Position / Ratio).ToString());
                    break;
                case 5:
                    FStopDecelerate = position.Decelerate / 1000.0;
                    SetSpeed(position.Velocity * (double)position.SpeedPercent / 100.0, FPositions[1].Accelerate / 1000.0, FPositions[1].Decelerate / 1000.0, FPositions[1].Jerk / 1000.0);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J:" + (position.Position / Ratio).ToString());
                    break;

                case 11:
                    FStopDecelerate = FPositions[1].Decelerate / 1000.0;
                    SetSpeed(position.Velocity, FPositions[1].Accelerate / 1000.0, FPositions[1].Decelerate / 1000.0, FPositions[1].Jerk / 1000.0);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "Jog:" + (position.Position / Ratio).ToString());
                    break;
            }

            FTargetPosition = EncoderPosition + (position.Position / Ratio);
            FNextPositionIndex = -1;
            FPositionIndex = -1;

            FDoneDelay = FSetDoneDelay;
            FInpositionEx = false;
            FInposition = false;
        }
        public override void AbsoluteMove(int APositionIndex)
        {
            if (!FOwner.Initialized) return;

            if (FOnAlarm) return;
            if (!FOnReady) return;
            if (!FInpositionEx) return;

            if (APositionIndex <= 0) return;
            if (APositionIndex >= FPositions.Length) return;


            CMotionPositionBasic position = FPositions[APositionIndex];
            if (position.SpeedPercent < 1) position.SpeedPercent = 1;
            if (position.SpeedPercent > 100) position.SpeedPercent = 100;

            switch (FSpeedMode)
            {
                case 0:
                    FVelocity = FMaxVelocity * (double)position.SpeedPercent / 100.0;
                    SetSpeed(FVelocity, 0.1, 0.1, 0);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J=" + (position.Position / Ratio).ToString());
                    break;
                case 1:
                case 2:
                    FStopDecelerate = FPositions[1].Decelerate / 1000.0;
                    SetSpeed(FPositions[1].Velocity * (double)position.SpeedPercent / 100.0, FPositions[1].Accelerate / 1000.0, FPositions[1].Decelerate / 1000.0, FPositions[1].Jerk / 1000.0);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J=" + (position.Position / Ratio).ToString());
                    break;
                case 3:
                case 4:
                    FStopDecelerate = position.Decelerate / 1000.0;
                    SetSpeed(position.Velocity * (double)position.SpeedPercent / 100.0, position.Accelerate / 1000.0, position.Decelerate / 1000.0, position.Jerk / 1000.0);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J=" + (position.Position / Ratio).ToString());

                    break;
                case 5:
                    FStopDecelerate = position.Decelerate / 1000.0;
                    SetSpeed(position.Velocity * (double)position.SpeedPercent / 100.0, FPositions[1].Accelerate / 1000.0, FPositions[1].Decelerate / 1000.0, FPositions[1].Jerk / 1000.0);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J=" + (position.Position / Ratio).ToString());
                    break;

                case 11:
                    FStopDecelerate = FPositions[1].Decelerate / 1000.0;
                    SetSpeed(position.Velocity, FPositions[1].Accelerate / 1000.0, FPositions[1].Decelerate / 1000.0, FPositions[1].Jerk / 1000.0);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J=" + (position.Position / Ratio).ToString());
                    break;
            }
            FTargetPosition = position.Position / Ratio;
            FNextPositionIndex = APositionIndex;
            FPositionIndex = -1;

            FDoneDelay = FSetDoneDelay;
            FInpositionEx = false;
            FInposition = false;
        }

        /// <summary>
        /// 박현준이 편의성으로 하나 만듦_220317
        /// </summary>
        /// <param name="APositionIndex"></param>
        public override void AbsoluteMove(double position, double speed, double AAccelerate)
        {
            if (!FOwner.Initialized) return;

            if (FOnAlarm) return;
            if (!FOnReady) return;
            if (!FInpositionEx) return;

            SetSpeed(speed, AAccelerate, AAccelerate, 0);
            FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J=" + (position / Ratio).ToString());

            FTargetPosition = position / Ratio;
            FPositionIndex = -1;

            FDoneDelay = FSetDoneDelay;
            FInpositionEx = false;
            FInposition = false;
        }
        public override void RelativeMove(int APositionIndex, double APosition)
        {
            if (!FOwner.Initialized) return;

            if (FOnAlarm) return;
            if (!FOnReady) return;
            if (!FInpositionEx) return;

            if (APositionIndex <= 0) return;
            if (APositionIndex >= FPositions.Length) return;

            CMotionPositionBasic position = FPositions[APositionIndex];
            if (position.SpeedPercent < 1) position.SpeedPercent = 1;
            if (position.SpeedPercent > 100) position.SpeedPercent = 100;

            switch (FSpeedMode)
            {
                case 0:
                    FVelocity = FMaxVelocity * (double)position.SpeedPercent / 100.0;
                    SetSpeed(FVelocity, 0.1, 0.1, 0);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J:" + (APosition / Ratio).ToString());
                    break;
                case 1:
                case 2:
                    FStopDecelerate = FPositions[1].Decelerate / 1000.0;
                    SetSpeed(FPositions[1].Velocity * (double)position.SpeedPercent / 100.0, FPositions[1].Accelerate / 1000.0, FPositions[1].Decelerate / 1000.0, FPositions[1].Jerk / 1000.0);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J:" + (APosition / Ratio).ToString());
                    break;
                case 3:
                case 4:
                    FStopDecelerate = position.Decelerate / 1000.0;
                    SetSpeed(position.Velocity * (double)position.SpeedPercent / 100.0, position.Accelerate / 1000.0, position.Decelerate / 1000.0, position.Jerk / 1000.0);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J:" + (APosition / Ratio).ToString());
                    break;
                case 5:
                    FStopDecelerate = position.Decelerate / 1000.0;
                    SetSpeed(position.Velocity * (double)position.SpeedPercent / 100.0, FPositions[1].Accelerate / 1000.0, FPositions[1].Decelerate / 1000.0, FPositions[1].Jerk / 1000.0);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J:" + (APosition / Ratio).ToString());
                    break;

                case 11:
                    FStopDecelerate = FPositions[1].Decelerate / 1000.0;
                    SetSpeed(position.Velocity, FPositions[1].Accelerate / 1000.0, FPositions[1].Decelerate / 1000.0, FPositions[1].Jerk / 1000.0);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J:" + (APosition / Ratio).ToString());
                    break;
            }
            FTargetPosition = EncoderPosition + (APosition / Ratio);
            FNextPositionIndex = -1;
            FPositionIndex = -1;

            FDoneDelay = FSetDoneDelay;
            FInpositionEx = false;
            FInposition = false;
        }
        public override void AbsoluteMove(int APositionIndex, double APosition)
        {
            if (!FOwner.Initialized) return;

            if (FOnAlarm) return;
            if (!FOnReady) return;
            if (!FInpositionEx) return;

            if (APositionIndex <= 0) return;
            if (APositionIndex >= FPositions.Length) return;

            CMotionPositionBasic position = FPositions[APositionIndex];
            if (position.SpeedPercent < 1) position.SpeedPercent = 1;
            if (position.SpeedPercent > 100) position.SpeedPercent = 100;

            switch (FSpeedMode)
            {
                case 0:
                    FVelocity = FMaxVelocity * (double)position.SpeedPercent / 100.0;
                    SetSpeed(FVelocity, 0.1, 0.1, 0);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J=" + (APosition / Ratio).ToString());
                    break;
                case 1:
                case 2:
                    FStopDecelerate = FPositions[1].Decelerate / 1000.0;
                    SetSpeed(FPositions[1].Velocity * (double)position.SpeedPercent / 100.0, FPositions[1].Accelerate / 1000.0, FPositions[1].Decelerate / 1000.0, FPositions[1].Jerk / 1000.0);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J=" + (APosition / Ratio).ToString());
                    break;
                case 3:
                case 4:
                    FStopDecelerate = position.Decelerate / 1000.0;
                    SetSpeed(position.Velocity * (double)position.SpeedPercent / 100.0, position.Accelerate / 1000.0, position.Decelerate / 1000.0, position.Jerk / 1000.0);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J=" + (APosition / Ratio).ToString());
                    break;
                case 5:
                    FStopDecelerate = position.Decelerate / 1000.0;
                    SetSpeed(position.Velocity * (double)position.SpeedPercent / 100.0, FPositions[1].Accelerate / 1000.0, FPositions[1].Decelerate / 1000.0, FPositions[1].Jerk / 1000.0);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J=" + (APosition / Ratio).ToString());
                    break;

                case 11:
                    FStopDecelerate = FPositions[1].Decelerate / 1000.0;
                    SetSpeed(position.Velocity, FPositions[1].Accelerate / 1000.0, FPositions[1].Decelerate / 1000.0, FPositions[1].Jerk / 1000.0);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J=" + (APosition / Ratio).ToString());
                    break;
            }
            FTargetPosition = APosition / Ratio;

            FNextPositionIndex = APositionIndex;
            FPositionIndex = -1;
            FDoneDelay = FSetDoneDelay;
            FInpositionEx = false;
            FInposition = false;
        }
        public override void RelativeMove(int APositionIndex, double APosition, int ASpeedPercent)
        {
            if (!FOwner.Initialized) return;

            if (FOnAlarm) return;
            if (!FOnReady) return;
            if (!FInpositionEx) return;

            if (APositionIndex <= 0) return;
            if (APositionIndex >= FPositions.Length) return;

            CMotionPositionBasic position = FPositions[APositionIndex];
            if (position.SpeedPercent < 1) position.SpeedPercent = 1;
            if (position.SpeedPercent > 100) position.SpeedPercent = 100;

            switch (FSpeedMode)
            {
                case 0:
                    FVelocity = FMaxVelocity * (double)ASpeedPercent / 100.0;
                    SetSpeed(FVelocity, 0.1, 0.1, 0);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J:" + (APosition / Ratio).ToString());
                    break;
                case 1:
                case 2:
                    FStopDecelerate = FPositions[1].Decelerate / 1000.0;
                    SetSpeed(FPositions[1].Velocity * (double)ASpeedPercent / 100.0, FPositions[1].Accelerate / 1000.0, FPositions[1].Decelerate / 1000.0, FPositions[1].Jerk / 1000.0);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J:" + (APosition / Ratio).ToString());
                    break;
                case 3:
                case 4:
                    FStopDecelerate = position.Decelerate / 1000.0;
                    SetSpeed(position.Velocity * (double)ASpeedPercent / 100.0, position.Accelerate / 1000.0, position.Decelerate / 1000.0, position.Jerk / 1000.0);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J:" + (APosition / Ratio).ToString());
                    break;
                case 5:
                    FStopDecelerate = position.Decelerate / 1000.0;
                    SetSpeed(position.Velocity * (double)ASpeedPercent / 100.0, FPositions[1].Accelerate / 1000.0, FPositions[1].Decelerate / 1000.0, FPositions[1].Jerk / 1000.0);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J:" + (APosition / Ratio).ToString());
                    break;

                case 11:
                    FStopDecelerate = FPositions[1].Decelerate / 1000.0;
                    SetSpeed(position.Velocity, FPositions[1].Accelerate / 1000.0, FPositions[1].Decelerate / 1000.0, FPositions[1].Jerk / 1000.0);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J:" + (APosition / Ratio).ToString());
                    break;
            }
            FTargetPosition = EncoderPosition + (APosition / Ratio);
            FNextPositionIndex = -1;
            FPositionIndex = -1;

            FDoneDelay = FSetDoneDelay;
            FInpositionEx = false;
            FInposition = false;
        }
        public override void AbsoluteMove(int APositionIndex, double APosition, int ASpeedPercent)
        {
            if (!FOwner.Initialized) return;

            if (FOnAlarm) return;
            if (!FOnReady) return;
            if (!FInpositionEx) return;

            if (APositionIndex <= 0) return;
            if (APositionIndex >= FPositions.Length) return;

            CMotionPositionBasic position = FPositions[APositionIndex];
            if (position.SpeedPercent < 1) position.SpeedPercent = 1;
            if (position.SpeedPercent > 100) position.SpeedPercent = 100;

            switch (FSpeedMode)
            {
                case 0:
                    FVelocity = FMaxVelocity * (double)ASpeedPercent / 100.0;
                    SetSpeed(FVelocity, 0.1, 0.1, 0);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J=" + (APosition / Ratio).ToString());
                    break;
                case 1:
                case 2:
                    FStopDecelerate = FPositions[1].Decelerate / 1000.0;
                    SetSpeed(FPositions[1].Velocity * (double)ASpeedPercent / 100.0, FPositions[1].Accelerate / 1000.0, FPositions[1].Decelerate / 1000.0, FPositions[1].Jerk / 1000.0);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J=" + (APosition / Ratio).ToString());
                    break;
                case 3:
                case 4:
                    FStopDecelerate = position.Decelerate / 1000.0;
                    SetSpeed(position.Velocity * (double)ASpeedPercent / 100.0, position.Accelerate / 1000.0, position.Decelerate / 1000.0, position.Jerk / 1000.0);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J=" + (APosition / Ratio).ToString());
                    break;
                case 5:
                    FStopDecelerate = position.Decelerate / 1000.0;
                    SetSpeed(position.Velocity * (double)ASpeedPercent / 100.0, FPositions[1].Accelerate / 1000.0, FPositions[1].Decelerate / 1000.0, FPositions[1].Jerk / 1000.0);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J=" + (APosition / Ratio).ToString());
                    break;

                case 11:
                    FStopDecelerate = FPositions[1].Decelerate / 1000.0;
                    SetSpeed(position.Velocity, FPositions[1].Accelerate / 1000.0, FPositions[1].Decelerate / 1000.0, FPositions[1].Jerk / 1000.0);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J=" + (APosition / Ratio).ToString());
                    break;
            }
            FTargetPosition = (APosition / Ratio);

            FNextPositionIndex = APositionIndex;
            FPositionIndex = -1;
            FDoneDelay = FSetDoneDelay;
            FInpositionEx = false;
            FInposition = false;
        }
        public override void RelativeMove(int APositionIndex, double APosition, int ASpeedPercent, double AVelocity)
        {
            if (!FOwner.Initialized) return;

            if (FOnAlarm) return;
            if (!FOnReady) return;
            if (!FInpositionEx) return;

            if (APositionIndex <= 0) return;
            if (APositionIndex >= FPositions.Length) return;

            CMotionPositionBasic position = FPositions[APositionIndex];
            if (position.SpeedPercent < 1) position.SpeedPercent = 1;
            if (position.SpeedPercent > 100) position.SpeedPercent = 100;

            switch (FSpeedMode)
            {
                case 0:
                    FVelocity = FMaxVelocity * (double)ASpeedPercent / 100.0;
                    SetSpeed(FVelocity, 0.1, 0.1, 0);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J:" + (APosition / Ratio).ToString());
                    break;
                case 1:
                case 2:
                    FStopDecelerate = FPositions[1].Decelerate / 1000.0;
                    SetSpeed(AVelocity * (double)ASpeedPercent / 100.0, FPositions[1].Accelerate / 1000.0, FPositions[1].Decelerate / 1000.0, FPositions[1].Jerk);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J:" + (APosition / Ratio).ToString());
                    break;
                case 3:
                case 4:
                    FStopDecelerate = position.Decelerate / 1000.0;
                    SetSpeed(AVelocity * (double)ASpeedPercent / 100.0, position.Accelerate / 1000.0, position.Decelerate / 1000.0, position.Jerk);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J:" + (APosition / Ratio).ToString());
                    break;
                case 5:
                    FStopDecelerate = position.Decelerate / 1000.0;
                    SetSpeed(AVelocity * (double)ASpeedPercent / 100.0, FPositions[1].Accelerate / 1000.0, FPositions[1].Decelerate / 1000.0, FPositions[1].Jerk);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J:" + (APosition / Ratio).ToString());
                    break;

                case 11:
                    FStopDecelerate = FPositions[1].Decelerate / 1000.0;
                    SetSpeed(AVelocity, FPositions[1].Accelerate / 1000.0, FPositions[1].Decelerate / 1000.0, FPositions[1].Jerk);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J:" + (APosition / Ratio).ToString());
                    break;
            }
            FTargetPosition = EncoderPosition + (APosition / Ratio);
            FNextPositionIndex = -1;
            FPositionIndex = -1;

            FDoneDelay = FSetDoneDelay;
            FInpositionEx = false;
            FInposition = false;
        }
        public override void AbsoluteMove(int APositionIndex, double APosition, int ASpeedPercent, double AVelocity)
        {
            if (!FOwner.Initialized) return;

            if (FOnAlarm) return;
            if (!FOnReady) return;
            if (!FInpositionEx) return;

            if (APositionIndex <= 0) return;
            if (APositionIndex >= FPositions.Length) return;

            CMotionPositionBasic position = FPositions[APositionIndex];
            if (position.SpeedPercent < 1) position.SpeedPercent = 1;
            if (position.SpeedPercent > 100) position.SpeedPercent = 100;

            switch (FSpeedMode)
            {
                case 0:
                    FVelocity = FMaxVelocity * (double)ASpeedPercent / 100.0;
                    SetSpeed(FVelocity, 0.1, 0.1, 0);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J=" + (APosition / Ratio).ToString());
                    break;
                case 1:
                case 2:
                    FStopDecelerate = FPositions[1].Decelerate / 1000.0;
                    SetSpeed(AVelocity * (double)ASpeedPercent / 100.0, FPositions[1].Accelerate / 1000.0, FPositions[1].Decelerate / 1000.0, FPositions[1].Jerk);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J=" + (APosition / Ratio).ToString());
                    break;
                case 3:
                case 4:
                    FStopDecelerate = position.Decelerate / 1000.0;
                    SetSpeed(AVelocity * (double)ASpeedPercent / 100.0, position.Accelerate / 1000.0, position.Decelerate / 1000.0, position.Jerk);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J=" + (APosition / Ratio).ToString());
                    break;
                case 5:
                    FStopDecelerate = position.Decelerate / 1000.0;
                    SetSpeed(AVelocity * (double)ASpeedPercent / 100.0, FPositions[1].Accelerate / 1000.0, FPositions[1].Decelerate / 1000.0, FPositions[1].Jerk);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J=" + (APosition / Ratio).ToString());
                    break;

                case 11:
                    FStopDecelerate = FPositions[1].Decelerate / 1000.0;
                    SetSpeed(AVelocity, FPositions[1].Accelerate / 1000.0, FPositions[1].Decelerate / 1000.0, FPositions[1].Jerk);

                    FOwner.PMacCommand("#" + (MotionIndex + 1).ToString() + "J=" + (APosition / Ratio).ToString());
                    break;
            }
            FTargetPosition = (APosition / Ratio);

            FNextPositionIndex = APositionIndex;
            FPositionIndex = -1;
            FDoneDelay = FSetDoneDelay;
            FInpositionEx = false;
            FInposition = false;
        }
        #endregion

        //-------------------------------------------------------------------------------------------

        #region 상태 변경

        public override void UpdateHomeDone(int AHomeDone)
        {
            if (AHomeDone < 0) { FHomeDone = -1; return; }
            if (AHomeDone > 0) { FHomeDone = 1; return; }

            FHomeDone = 0;
        }
        public override void UpdateInpoistion(bool AInpoistion)
        {
            if (!AInpoistion)
            {
                FInposition = false;
                FMotionDone = false;
            }
        }
        public override void UpdateMotionDone(bool AMotionDone)
        {
            if (!AMotionDone)
            {
                FMotionDone = false;
                FInposition = false;
            }
        }
        private bool StrSpecialParsing(ref string ARecvString)
        {
            if (ARecvString == null || ARecvString == "") return false;
            //int pos = ARecvString.IndexOf("$");

            //if (pos == 0) return  true;
            //if (pos <  0) return false;
            //ARecvString = ARecvString.Remove(0, pos);
            return true;
        }
        private bool StrParsing(ref string ARecvString, ref string ARecvArray)
        {
            if (ARecvString == null || ARecvString == "") return false;
            int pos = ARecvString.IndexOf("\r");

            if (pos < 0) return false;
            ARecvArray = ARecvString.Substring(0, pos);

            ARecvString = ARecvString.Substring(pos + 1);
            return true;
        }
        string[] FStrResponseArry = new string[10];
        public override bool GetStatus(bool AEmergency)
        {
            bool ret = true;

            if (!FEnabled)
            {
                FOnReady = true;
                FOnOrigin = false;

                FOnAlarm = false;
                FOnPositiveLimit = false;
                FOnNegativeLimit = false; ;

                FHomeDone = 1;
                FMotionDone = true;
                FInposition = true;
                FInpositionEx = true;
            }

            HomeProc();
            if (FServoOffOnEmergencyEnable)
            {
                if (!AEmergency)
                {
                    if (FEmergencyStatus != AEmergency)
                    {
                        if (!ServoOn) ServoOn = true;
                    }
                    FServoOffDelayOnEmergency = 0;
                }

                if (++FServoOffDelayOnEmergency > 2)
                {
                    if (ServoOn && !Inposition) Stop(true);
                }
                FEmergencyStatus = AEmergency;
            }

            string strResponse = "";
            uint intResponse = 0;

            string strCommand = "";
            strCommand = "#" + (MotionIndex + 1).ToString() + "? ";    //모터 상태
            strCommand += "#" + (MotionIndex + 1).ToString() + "P";    //encode 위치 
            strCommand += "#" + (MotionIndex + 1).ToString() + "V";    //command 위치 
                                                                       //if(FHomeLogicCommand != null) strCommand += "plc["   + FHomeLogicCommand.Substring(3, 1) + "].active ";

            try
            {
                strResponse = FOwner.PMacCommand(strCommand, 1);
            }
            catch (Exception e)
            {
                Console.WriteLine("PMAC PMacCommand({2}) ({0}-{1})", e.ToString(), DateTime.Now.ToString(), strCommand);
                return false;
            }

            try
            {
                if (!StrParsing(ref strResponse, ref FStrResponseArry[0])) return false;
                if (!StrParsing(ref strResponse, ref FStrResponseArry[1])) return false;
                if (!StrParsing(ref strResponse, ref FStrResponseArry[2])) return false;
                //if (!StrParsing(ref strResponse, ref FStrResponseArry[3])) return false;
                //if(FHomeLogicCommand != null) if (!StrParsing(ref strResponse, ref FStrResponseArry[4])) return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("PMAC StrParsing({2}) ({0}-{1})", e.ToString(), DateTime.Now.ToString(), strResponse);
                return false;
            }

            try
            {
                if (FStrResponseArry[0].Length > 1)
                {
                    intResponse = Convert.ToUInt32(FStrResponseArry[0].Substring(6, 6), 16);  //Using ToUInt32 not ToUInt64, as per OP comment

                    FInposition = ((intResponse & 0x00000001) == 0x00000001);
                    FOnAlarm = ((intResponse & 0x00000008) == 0x00000008);
                    FHomeComplete = ((intResponse & 0x00000400) == 0x00000400);

                    intResponse = Convert.ToUInt32(FStrResponseArry[0].Substring(0, 6), 16);  //Using ToUInt32 not ToUInt64, as per OP comment
                    FServoOn = ((intResponse & 0x00080000) == 0x00080000);
                    FOnReady = ((intResponse & 0x00800000) == 0x00800000);
                    FOnNegativeLimit = ((intResponse & 0x00400000) == 0x00400000);
                    FOnPositiveLimit = ((intResponse & 0x00200000) == 0x00200000);
                    FMotionDone = !((intResponse & 0x00020000) == 0x00020000);
                    FHomeProgress = ((intResponse & 0x00000400) == 0x00000400);
                }
                else
                {
                    FMotionDone = false;
                    FInposition = false;
                }
            }
            catch
            {
                intResponse = Convert.ToUInt32(FStrResponseArry[0].Substring(6, 6), 16);  //Using ToUInt32 not ToUInt64, as per OP comment


                FInposition = ((intResponse & 0x00000001) == 0x00000001);
                FOnAlarm = ((intResponse & 0x00000008) == 0x00000008);
                FHomeComplete = ((intResponse & 0x00000400) == 0x00000400);

                intResponse = Convert.ToUInt32(FStrResponseArry[0].Substring(0, 6), 16);  //Using ToUInt32 not ToUInt64, as per OP comment
                FServoOn = ((intResponse & 0x00080000) == 0x00080000);
                FOnReady = ((intResponse & 0x00800000) == 0x00800000);
                FOnNegativeLimit = ((intResponse & 0x00400000) == 0x00400000);
                FOnPositiveLimit = ((intResponse & 0x00200000) == 0x00200000);
                FMotionDone = !((intResponse & 0x00020000) == 0x00020000);
                FHomeProgress = ((intResponse & 0x00000400) == 0x00000400);
            }

            if (FMotionIndex != 0) FInposition = FMotionDone;
            if (FInposition)
            {
                if (FDoneDelay >= 0)
                {
                    FDoneDelay--;
                    FInposition = false;
                }
            }

            FIsHomeBufferRun = ((intResponse & 0x00008000) == 0x00008000);

            if (!FHomeProgress && FHomeComplete) FHomeDone = 1;
            else FHomeDone = 0;
            FInpositionEx = (FInposition && FMotionDone && FHomeDone > 0 && FHomeStep == 0);

            FActPosition = CETC.ToDouble(FStrResponseArry[1]);
            FDacPosition = CETC.ToDouble(FStrResponseArry[2]);

            //FEncoderPosition = (FActPosition - FHomePosision) * FRatio; 
            //FCommandPosition = (FDacPosition - FHomePosision) * FRatio; 			
            FEncoderPosition = (FActPosition) * FRatio;
            FCommandPosition = (FDacPosition) * FRatio;

            if (FInpositionEx && FNextPositionIndex >= 0)
            {
                if ((FNegativeLimitMode != 0) && FOnNegativeLimit) FNextPositionIndex = -1;
                if ((FPositiveLimitMode != 0) && FOnPositiveLimit) FNextPositionIndex = -1;

                FPositionIndex = FNextPositionIndex;
                FNextPositionIndex = -1;
            }

            return ret;
        }

        public override void UpdateStatus(bool AOnReady, bool AOnAlarm, bool AOnOrgin, bool AOnPositiveLimit, bool AOnNegativeLimit, int AHomeDone, bool AInposition, bool AMotionDone, double ACommandPosition, double AEncoderPosition)
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
    }

    public class CPmacMotion : IDeviceAccompany, IDisposable
    {
        static public string Version
        {
            get { return "PMAC MOTION BASIC - sean.kim(V25.08.28.001)"; }
        }

        public CPmacMotion()
        {

        }

        public CPmacMotion(int AMotionCount)
        {
            FDirectory = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\CONFIG\\";
            if (!System.IO.Directory.Exists(FDirectory)) System.IO.Directory.CreateDirectory(FDirectory);

            FItem = new CPMacMotionItem[AMotionCount];
            for (int i = 0; i < FItem.Length; i++) FItem[i] = new CPMacMotionItem(this, i);

            FInitialized = Initialize();
            OpenParameter();
            OpenDefaultPosition();
        }

        protected CPMacMotionItem[] FItem = null;
        public int MotionCount { get { return FItem.Length; } }
        public bool IsPmacMotionInposition { get; private set; } = false;

        public CPMacMotionItem this[int AIndex]
        {
            get
            {
                if (AIndex < 0) return null;
                if (AIndex >= FItem.Length) return null;

                return FItem[AIndex];
            }
        }
        //-------------------------------------------------------------------------------------------		

        #region Dispose 구문
        protected bool FDisposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool ADisposing)
        {
            if (FDisposed) return;
            if (ADisposing) { /* IDisposable 인터페이스를 구현하는 멤버들을 여기서 정리합니다. */}

            CPMacBasic.ReleaseInstance();
            FInitialized = false;
            FDisposed = true;
        }
        #endregion

        //-------------------------------------------------------------------------------------------

        #region COMMAND 구문
        public string PMacCommand(string AString, int ADeviceIndex = 0)
        {
            return CPMacBasic.PMacCommand(AString, ADeviceIndex);
        }
        #endregion

        //-------------------------------------------------------------------------------------------

        #region Initalzed 구문

        private bool FInitialized = false;
        public bool Initialized { get { return FInitialized; } }

        public bool Initialize()
        {
            return CPMacBasic.Initialize();
        }

        #endregion

        //-------------------------------------------------------------------------------------------

        #region 디바이스와 연동 부분
        public bool DeviceOpen(string APath)
        {
            Open(APath);
            return true;
        }
        public bool DeviceDelete(string APath)
        {
            string file = $"{APath}PMAC POSITION.XML";
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
            string file = $"{APath}PMAC POSITION.XML";

            if (!System.IO.File.Exists(file)) return false;
            return true;
        }
        public bool DeviceCopy(string ASrcPath, string ADesPath)
        {
            string srcfile = $"{ASrcPath}PMAC POSITION.XML";
            string desfile = $"{ADesPath}PMAC POSITION.XML";

            if (!System.IO.Directory.Exists(ADesPath)) System.IO.Directory.CreateDirectory(ADesPath);
            try
            {
                System.IO.File.Copy(srcfile, desfile);
            }
            catch
            {

            }

            if (!System.IO.File.Exists(desfile)) return false;
            return true;
        }
        public bool Decoding(string APath, XmlNode ANode)
        {
            XmlNode srcitems = CXML.SelectSingleNode(ANode, "/ROOT/PMAC_MOTION");
            if (srcitems != null)
            {
                string file = $"{APath}PMAC_MOTION.XML";
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
            XmlNode motion = ADoc.CreateElement("PMAC_MOTION");
            ANode.AppendChild(motion);

            bool ret = true;
            string file = $"{APath}PMAC_MOTION.XML";
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

        //-------------------------------------------------------------------------------------------

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

            foreach (CPMacMotionItem item in FItem)
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
            xdoc.Save($"{FDirectory}PMAC MOTION PARAMETER.XML");
        }

        virtual public void OpenParameter()
        {
            string file = $"{FDirectory}PMAC MOTION PARAMETER.XML";
            if (!System.IO.File.Exists(file)) { SaveParameter(); return; }

            XmlDocument xdoc = new System.Xml.XmlDocument();
            xdoc.Load(file);

            string version = "";
            XmlNode basic = xdoc.SelectSingleNode($"/ROOT/BASIC");
            if (basic != null)
            {
                CXML.GetInnerText(basic, "VERSION", out version);
            }

            if (version != Version)
            {
                System.Windows.Forms.MessageBox.Show($"버젼({Version} : {version})이 다릅니다.\r\n정보 불러오기에 문제가 발생될 수 있습니다.", "경고", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
            }

            XmlNode root = xdoc.SelectSingleNode($"/ROOT");
            foreach (CPMacMotionItem item in FItem)
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

            foreach (CPMacMotionItem item in FItem)
            {
                if (item == null) break;
                item.SaveDefaultPosition(xdoc, root);
            }
            xdoc.Save($"{FDirectory}PMAC DEFAULT POSITION.XML");
        }
        virtual public void OpenDefaultPosition()
        {
            string file = $"{FDirectory}PMAC DEFAULT POSITION.XML";
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
            foreach (CPMacMotionItem item in FItem)
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

                foreach (CPMacMotionItem item in FItem)
                {
                    if (item == null) break;
                    item.Save(xdoc, root);
                }
                xdoc.Save($"{APath}PMAC POSITION.XML");
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString(), "에러", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
        virtual public void Open(string APath)
        {
            string file = $"{APath}PMAC POSITION.XML";
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
            foreach (CPMacMotionItem item in FItem)
            {
                if (item == null) break;
                item.Open(root);
            }
        }
        #endregion

        //-------------------------------------------------------------------------------------------
        virtual public void GetStatus(bool AEmergency)
        {
            if (!FInitialized) return;

            try
            {
                for (int i = 0; i < FItem.Length; i++)
                {
                    int index = FItem[i].PMacMotionIndex;
                    if (!FItem[i].Enabled) continue;
                    FItem[i].GetStatus(AEmergency);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("PMAC GetStatus Error({0}-{1})", e.ToString(), DateTime.Now.ToString());
            }
        }
    }
}
