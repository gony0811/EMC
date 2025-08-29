using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using EGGPLANT.Device.PowerPmac;

namespace EGGPLANT
{
    class CExecute : CExecuteBase
    {
        private CPmacMotion PmacMotion = null;
        private CError Error = null;
        public CExecute()
        {
            
        }

        public CExecute(Window AMessageWindowInstance, string AClassName) : base(AMessageWindowInstance, AClassName)
        {
            Status = 1;
            FOnSoftEmergency = 0;
            PmacMotion = App.Container.Resolve<CPmacMotion>();
            Error = App.Container.Resolve<CError>();
        }

        public int Status = 1;
        private int FOnEmergency = 0;
        private int FOnSoftEmergency = 0;

        public void OnSoftEmergency()
        {
            if (FOnSoftEmergency == 0) FOnSoftEmergency = 1;
        }

        protected override void Init()
        {
            for (int i = 0; i < PmacMotion.MotionCount; i++) PmacMotion[i].Stop(true);
        }

        #region Dispose 구문
        protected override void Dispose(bool ADisposing)
        {
            if (FDisposed) return;
            if (ADisposing) { /* IDisposable 인터페이스를 구현하는 멤버들을 여기서 정리합니다. */}

            base.Dispose(ADisposing);
        }
        #endregion

        override protected void MoveStep(int AStep)
        {
            for (int i = 0; i < FItemCount; i++)
            {
                if (FItem[i].Element == 0)
                {
                    FItem[i].Move(AStep);
                    if (FRunMode != RUN_MODE.JAM) FItem[i].ErrorIndex = 0;
                }
            }
        }

        private System.Diagnostics.Stopwatch FWatch = new System.Diagnostics.Stopwatch();
#pragma warning disable CS0414
        private int FAIOConnect = 0x00;
        private int FDIOConnect = 0x00;
        private int FMotConnect = 0x00;
#pragma warning restore CS0414

        override public void Execute()
        {
            MainInterLock();
            ItemsExecute();

            if (FOnSoftEmergency == 2) FOnSoftEmergency = 0;
        }

        protected int FPMacTimeCount = 0;
        protected int FPMacTimerTickCount = 0;
        protected double FPMacExecuteInterval = 0.0f;

        public double APMacExecuteInterval { get { return FPMacExecuteInterval; } }


        public void PMacExecute()
        {
            if (++FPMacTimeCount >= 100)
            {
                FPMacExecuteInterval = (double)(Environment.TickCount - FPMacTimerTickCount) / (double)FPMacTimeCount;
                FPMacTimerTickCount = Environment.TickCount;
                FPMacTimeCount = 0;
            }

            int onEmergency = 0;
            //CMIT.PmacIO.Input.Update();

            if (FOnSoftEmergency != 0) { onEmergency = 20; }
            /*
            if ( CMIT.DIO.Input[DI.LOADER_FRONT_EMG_SW  ])      {                       onEmergency = 21; }
            if ( CMIT.DIO.Input[DI.LOADER_REAR_EMG_SW   ])      {                       onEmergency = 22; }
            if ( CMIT.DIO.Input[DI.FRONT_CENTER_EMG_SW  ])      {                       onEmergency = 23; }
            if ( CMIT.DIO.Input[DI.REAR_CENTER_EMG_SW   ])      {                       onEmergency = 24; }
            if ( CMIT.DIO.Input[DI.UNLOADER_FRONT_EMG_SW])      {                       onEmergency = 25; }
            if ( CMIT.DIO.Input[DI.UNLOADER_REAR_EMG_SW ])      {                       onEmergency = 26; }
            //if (!CMIT.DIO.Input[DI.MAIN_AIR_1_PRESSURE])        {                       onEmergency = 16; }
            //if (!CMIT.DIO.Input[DI.MAIN_AIR_2_PRESSURE])        {                       onEmergency = 17; }
            //if (!CMIT.DIO.Input[DI.MAIN_AIR_3_PRESSURE])        {                       onEmergency = 18; }
            //if (!CMIT.DIO.Input[DI.MAIN_AIR_4_PRESSURE])        {                       onEmergency = 19; }
			*/
            //CMIT.PmacIO.Output.Update();
            PmacMotion.GetStatus(onEmergency != 0);
        }

        public void MainInterLock()
        {
#if (__DEBUG__ == false)
            int doorcheck = 0;

            switch (RunMode)
            {
                case RUN_MODE.RUN: doorcheck = 1; break;
                case RUN_MODE.JAM: doorcheck = 0; break;
                case RUN_MODE.EJAM: doorcheck = 0; break;
                case RUN_MODE.INIT: doorcheck = 1; break;
                case RUN_MODE.STOP:
                    if (DoHandOperate(1) != 0) doorcheck = 1;
                    else doorcheck = 0;
                    break;
                case RUN_MODE.TOJAM: doorcheck = 1; break;
                case RUN_MODE.TORUN: doorcheck = 1; break;
                case RUN_MODE.TOSTOP: doorcheck = 1; break;
            }

            int doorindex = GetDoorOpen(doorcheck != 0);
            if (FOnEmergency != 0)
            {
                for (int i = 0; i < PmacMotion.MotionCount; i++)
                {
                    CMotionItemBasic item = PmacMotion[i];
                    if (item.Inposition) continue;
                    if (!item.ServoOn) continue;

                    item.Stop(true, true);
                }

                Error.HappenToEmergency(FOnEmergency);
                RunMode = RUN_MODE.EJAM;
                return;
            }

            int eindex = 0;
            if (FOnEmergency == 0)
            {
                for (int i = 0; i < PmacMotion.MotionCount; i++)
                {
                    CMotionItemBasic item = PmacMotion[i];
                    if (!item.Enabled) continue;

                    if (item.OnAlarm)
                    {
                        if (!item.Inposition) item.Stop(true, true);
                        eindex = 9101 + i;
                        break;
                    }
                }
            }

            if (eindex != 0)
            {
                for (int i = 0; i < PmacMotion.MotionCount; i++)
                {
                    CMotionItemBasic item = PmacMotion[i];
                    if (item.Inposition) continue;
                    if (!item.ServoOn) continue;

                    item.Stop(true, true);
                }

                Error.Index = eindex;
                RunMode = RUN_MODE.EJAM;
                return;
            }
#endif
        }
    }
}
