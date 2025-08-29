using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace EGGPLANT
{
    public enum RUN_MODE { NONE = -1, STOP = 0, INIT = 1, TORUN = 2, RUN = 3, TOSTOP = 4, EJAM = 5, JAM = 6, TOJAM = 7, PAUSE = 8 }
    public delegate int TTimeOver(bool ASet = false, int AIndex = 0);

    class CStepProc : IDisposable
    {
        public string Version
        {
            get { return "STEP PROC - sean.kim(V25.08.29.001)"; }
        }
        public CStepProc(string AClassName, int AScanTime = 1, int AElement = 0)
        {
            FStep = 0;
            FPrevStep = 0;
            FTimeCount = 0;
            ErrorIndex = 0;
            PauseStatus = 0;
            PauseEnabled = 0;
            FTimeOverCount = 0; ;
            FHandOperateIndex = 0;
            FWindowInstanceCount = 0;

            for (int i = 1; i < 10; i++) { FWindowInstance[i] = null; }
            for (int i = 0; i < 10; i++) { FCycleTime[i] = 0; FCycleCount[i] = 0; }

            RunMode = RUN_MODE.STOP;
            FElement = AElement;
            FScanTime = AScanTime;
            FClassName = AClassName;
        }
        public CStepProc(string AClassName, int AScanTime, int AElement, Window AWindowInstance)
        {
            FStep = 0;
            FPrevStep = 0;
            FTimeCount = 0;
            ErrorIndex = 0;
            PauseStatus = 0;
            PauseEnabled = 0;
            FTimeOverCount = 0; ;
            FHandOperateIndex = 0;
            FWindowInstanceCount = 0;

            for (int i = 1; i < 10; i++) { FWindowInstance[i] = null; }
            for (int i = 0; i < 10; i++) { FCycleTime[i] = 0; FCycleCount[i] = 0; }

            RunMode = RUN_MODE.STOP;
            FElement = AElement;
            FScanTime = AScanTime;
            FClassName = AClassName;
            AddWindowInstance(AWindowInstance);
        }

        ~CStepProc()
        {
            Dispose(false);
        }
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

            FDisposed = true;
        }
        #endregion

        private string FClassName;
        public string ClassName
        { get { return FClassName; } }

        protected int FScanTime;
        public int ScanTime
        { get { return FScanTime; } }

        public void CalibrateDelay(ref int ADelay)
        {
            ADelay = ADelay / FScanTime;
        }

        #region 스텝 시간 측정용
        protected System.Diagnostics.Stopwatch FWatch = new System.Diagnostics.Stopwatch();
        public void WatchStart()
        {
            if (FWatch.IsRunning) FWatch.Restart();
            else FWatch.Start();
        }
        public long WatchReset
        {
            get
            {
                long time = FWatch.ElapsedMilliseconds;
                FWatch.Restart();
                return time;
            }
        }
        public long WatchStop
        {
            get
            {
                FWatch.Stop();
                return FWatch.ElapsedMilliseconds;
            }
        }
        #endregion

        protected int FElement;
        public int Element
        { get { return FElement; } }

        protected int FWindowInstanceCount = 0;
        protected Window[] FWindowInstance = new Window[10];
        public void AddWindowInstance(Window AWindowInstance)
        {
            if (AWindowInstance == null) return;
            if (FWindowInstanceCount >= 10) return;

            FWindowInstance[FWindowInstanceCount] = AWindowInstance;
            FWindowInstanceCount++;
        }

        public void PostMessage(uint Msg, int wParam, int lParam)
        {
            if (FWindowInstance[0] == null) return;

            if (FWindowInstance[0].Dispatcher.CheckAccess())
            {
                FWindowInstance[0].Dispatcher.BeginInvoke((MethodInvoker)delegate
                {
                    IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(FWindowInstance[0]).Handle;
                    CMESSAGE.PostMessage(hwnd, Msg, wParam, lParam);
                });
            }
            else
            {
                IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(FWindowInstance[0]).Handle;
                CMESSAGE.PostMessage(hwnd, Msg, wParam, lParam);
            }
        }
        public void PostMessage(Window window, uint Msg, int wParam, int lParam)
        {
            if (window.Dispatcher.CheckAccess())
            {
                window.Dispatcher.BeginInvoke((MethodInvoker)delegate
                {
                    IntPtr hwnd = new WindowInteropHelper(window).Handle;
                    CMESSAGE.PostMessage(hwnd, Msg, wParam, lParam);
                });
            }
            else
            {
                IntPtr hwnd = new WindowInteropHelper(window).Handle;
                CMESSAGE.PostMessage(hwnd, Msg, wParam, lParam);
            }
        }

        public int ErrorIndex = 0;
        public RUN_MODE RunMode = RUN_MODE.STOP;

        private int FThreadInterval = 1;
        public int ThreadInterval
        {
            get { return FThreadInterval; }
            set
            {
                if (FThreadInterval != value && value > 0)
                {
                    FThreadInterval = value;
                }
            }
        }

        protected CDelay FDelay = new CDelay();
        protected CDelay FDelayEx = new CDelay();

        protected int FStep = 0;
        protected int FPrevStep = 0;
        protected int FTimeCount = 0;
        public int Step { get { return FStep; } }
        public int PrevStep { get { return FPrevStep; } }

        public void Next()
        {
            FTimeCount = 0;
            FStep++;
        }
        public void Move(int AStep)
        {
            FTimeCount = 0;
            FStep = AStep;
        }

        #region 기본 동작 기능
        public virtual int Always()
        {
            return 0;
        }
        public virtual int OverTime()
        {
            FTimeCount++;
            switch (FStep)
            {
                case 0:
                case 501:
                    FTimeCount = 0;
                    break;

                default:
                    if (FTimeCount > 3000)
                    {
                        RunMode = RUN_MODE.JAM;
                        ErrorIndex = FStep;
                        FTimeCount = 0;
                    }
                    break;
            }
            return 0;
        }
        public virtual RUN_MODE Execute(RUN_MODE ARunMode)
        {
            RunMode = ARunMode;
            if (Always() != 0) return RunMode;
            if (OverTime() != 0) return RunMode;

            switch (RunMode)
            {
                case RUN_MODE.JAM: JAM(); break;
                case RUN_MODE.INIT: INIT(); break;
                case RUN_MODE.TORUN: TORUN(); break;

                case RUN_MODE.RUN: RUN(); break;
                case RUN_MODE.STOP:
                    switch (FHandOperateIndex)
                    {
                        case 0: break;
                        case 1: INIT(); break;
                        case 2: RUN(); break;
                        case 3: RUN(); break;
                        case 4: RUN(); break;
                        case 5: RUN(); break;
                    }
                    break;
                case RUN_MODE.TOJAM:
                case RUN_MODE.TOSTOP: RUN(); break;
            }
            return RunMode;
        }

        public virtual void INIT()      /* STEP: 201 ~ 299; */
        {
            if (!FDelay.TimeOut) { return; }

            switch (FStep)
            {
                case 0:
                    FHandOperateIndex = 0;
                    RunMode = (RunMode == RUN_MODE.TOJAM) ? RUN_MODE.JAM : RUN_MODE.STOP; break;

                case 201:
                    SetReSet();
                    Next();
                    break;

                default:
                    Move(0);
                    break;
            }
        }
        public virtual void TORUN()     /* STEP: 301 ~ 399; */
        {
            if (!FDelay.TimeOut) { return; }

            switch (FStep)
            {
                case 0:
                    FHandOperateIndex = 0;
                    RunMode = (RunMode == RUN_MODE.TOJAM) ? RUN_MODE.JAM : RUN_MODE.RUN; break;

                case 301:
                    SetReSet();
                    Next();
                    break;

                default:
                    Move(0);
                    break;
            }
        }
        public virtual void RUN()       /* STEP: 501 ~      */
        {
            if (!FDelay.TimeOut) { return; }

            switch (FStep)
            {
                case 0:
                    FHandOperateIndex = 0;
                    RunMode = (RunMode == RUN_MODE.TOJAM) ? RUN_MODE.JAM : RUN_MODE.STOP; break;

                case 501:
                    if (RunMode == RUN_MODE.STOP) RunMode = RUN_MODE.TOSTOP;
                    if (RunMode == RUN_MODE.TOJAM) { Move(401); break; }
                    if (RunMode == RUN_MODE.TOSTOP) { Move(401); break; }
                    Next();
                    break;
                case 502:
                    Move(501);
                    break;

                case 401:
                    SetReSet();
                    Next();
                    break;

                default:
                    Move(0);
                    break;
            }
        }
        public virtual void JAM()       /* STEP: 101 ~ 199; */
        {
            if (!FDelay.TimeOut) { return; }

            switch (FStep)
            {
                case 0:
                    RunMode = RUN_MODE.JAM;
                    FHandOperateIndex = 0;
                    break;

                case 101:
                    SetReSet();
                    Next();
                    break;

                default:
                    Move(0);
                    break;
            }
        }
        public virtual void STOP()      /* STEP: 001 ~ 099; */
        {
            if (!FDelay.TimeOut) { return; }

            switch (FStep)
            {
                case 0:
                    RunMode = RUN_MODE.STOP;
                    FHandOperateIndex = 0;
                    break;

                case 1:
                    Next();
                    break;

                default:
                    Move(0);
                    break;
            }
        }

        protected virtual void SetReSet(int AElement = 0)
        {

        }
        protected virtual void SetError(int AEIndex, int AStep = 501, RUN_MODE ARunMode = RUN_MODE.TOJAM)
        {

        }
        #endregion

        #region 매뉴얼(수동) 동작 기능
        protected int FHandOperateIndex = 0;
        public int HandOperateIndex { get { return FHandOperateIndex; } }
        public virtual void HandOperate(int AIndex, int AAction)
        {
            switch (AIndex)
            {
                default: break;
            }

        }
        public virtual bool HandOperateStatus(int AIndex, int AAction, int ADoorStatus)
        {
            switch (AIndex)
            {
                default: break;
            }
            if (FStep != 0) return false;
            return true;
        }
        #endregion

        #region 타임 오버 기능
        protected int FTimeOverCount = 0;
        protected TTimeOver[] FTimeOver = new TTimeOver[100];

        protected bool OnTimeOver()
        {
            for (int i = 0; i < FTimeOverCount; i++)
            {
                if (FTimeOver[i]() == 0) return false;
            }
            return true;
        }
        public void AddTimeOver(TTimeOver ATimeOver)
        {
            if (FTimeOverCount >= 100) return;

            FTimeOver[FTimeOverCount] = ATimeOver;
            FTimeOverCount++;
        }

        public virtual void HappenToTimeOver()
        {
            switch (RunMode)
            {
                case RUN_MODE.RUN:
                    RunMode = RUN_MODE.TOJAM;
                    Move(501);
                    break;

                case RUN_MODE.INIT:
                case RUN_MODE.TORUN:
                case RUN_MODE.TOJAM:
                case RUN_MODE.TOSTOP:
                    if (FStep > 500) Move(501);
                    else Move(0);
                    RunMode = RUN_MODE.TOJAM;
                    break;

                case RUN_MODE.STOP:
                    if (FStep > 500)
                    {
                        RunMode = RUN_MODE.TOJAM;
                        Move(501);
                    }
                    else RunMode = RUN_MODE.JAM;
                    Move(0);
                    break;
            }

        }
        public virtual bool ConfirmRunMode(RUN_MODE ARunMode)
        {
            return true;
        }
        #endregion

        #region 싸이클 타임 측정 기능
        protected int[] FCycleTime = new int[10];
        protected int[] FCycleCount = new int[10];

        public int GetCycleTime(int AIndex)
        {
            if (AIndex < 0) return 0;
            if (AIndex >= 10) return 0;

            return FCycleTime[AIndex];
        }

        protected int ReSetCycleTime(int AIndex)
        {
            if (AIndex < 0) return 0;
            if (AIndex >= 10) return 0;

            FCycleTime[AIndex] = (Environment.TickCount - FCycleCount[AIndex]);
            FCycleCount[AIndex] = Environment.TickCount;
            return FCycleTime[AIndex];
        }
        protected int BeginCycleTime(int AIndex)
        {
            if (AIndex < 0) return 0;
            if (AIndex >= 10) return 0;

            FCycleCount[AIndex] = Environment.TickCount;
            return FCycleTime[AIndex];
        }
        protected int EndCycleTime(int AIndex)
        {
            if (AIndex < 0) return 0;
            if (AIndex >= 10) return 0;

            FCycleTime[AIndex] = (Environment.TickCount - FCycleCount[AIndex]);
            return FCycleTime[AIndex];
        }
        #endregion

        #region Pause 기능
        public int PauseStatus = 0;
        public int PauseEnabled = 0;
        public bool Pause()
        {
            if (PauseEnabled == 0) return false;

            switch (PauseEnabled)
            {
                case 1: PauseEnabled = 0; return false;
                case 0: PauseEnabled = -1; break;
                case -1: break;
            }
            FTimeCount = 0;
            return true;
        }
        public void PauseNext()
        {
            if (PauseEnabled == 0) return;

            if (PauseStatus != -1) return;
            PauseStatus = 1;
        }
        #endregion

        //기본적으로 사용하는 옵션
        public bool GetBit(int AValue, int AIndex)
        {
            if ((AValue & (0x01 << AIndex)) != 0) return true;
            return false;
        }
        public void SetBit(ref int AValue, int AIndex, bool ASet)
        {
            if (ASet) AValue = (AValue | (0x01 << AIndex));
            else AValue = (AValue & ~(0x01 << AIndex));
        }
    }

    class CDelay
    {
        public CDelay()
        {
        }
        ~CDelay()
        {
            if (FWatch.IsRunning) FWatch.Stop();
        }

        private System.Diagnostics.Stopwatch FWatch = new System.Diagnostics.Stopwatch();
        public System.Diagnostics.Stopwatch Watch { get { return FWatch; } }

        private long FSetInterval = 0;
        public long SetInterval { get { return FSetInterval; } }
        public long CurrentInterval { get { return FWatch.ElapsedMilliseconds; } }

        public void Set(long AInterval, bool AReSet = true)
        {
            if (AReSet) FWatch.Restart();
            else if (!FWatch.IsRunning) FWatch.Start();

            FSetInterval = AInterval;
            FTimeOut = false;
        }
        public void Stop()
        {
            if (FWatch.IsRunning) FWatch.Stop();
            FTimeOut = false;
        }

        private bool FTimeOut = true;
        public bool TimeOut
        {
            get
            {
                if (!FTimeOut)
                {
                    if (FWatch.ElapsedMilliseconds >= FSetInterval)
                    {
                        FTimeOut = true;
                        FWatch.Stop();
                    }
                }
                return FTimeOut;
            }
        }
    }
}
