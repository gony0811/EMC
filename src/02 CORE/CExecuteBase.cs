using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Threading;

#pragma warning disable CS8625 // null 가능 참조에 대한 역참조입니다.

namespace EGGPLANT
{
    public enum KEY_SWITCH { SW_START = 0, SW_STOP = 1, SW_RESET = 2, SW_INIT = 3, SW_SPARE01 = 4, };

    public abstract class CExecuteBase : IDisposable
    {
        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        private const int WM_USER = 0x400;

        public string Version
        {
            get { return "EXECUTE BASE - sean.kim(V25.08.28.001)"; }
        }

        public CExecuteBase()
        {

        }

        public CExecuteBase(Window AWindowInstance, string AClassName)
        {
            FClassName = AClassName;
            FWindowInstance = AWindowInstance;
            FProcessMap = new CProcessMap();

            FTimer = new DispatcherTimer();
            FTimer.Tick += OnTimer;
            FTimer.IsEnabled = true;
            FTimer.Interval = new TimeSpan(0, 0, 0, 0, 25); // 25ms
        }



        ~CExecuteBase()
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

            FTimer.IsEnabled = false;
            FProcessMap.Dispose();
            FProcessMap = null;

            //FSetUpKey.Dispose();
            //FSetUpKey = null;
            FDisposed = true;
        }
        #endregion

        protected string FClassName;
        public string ClassName { get { return FClassName; } }

        private Window FWindowInstance;
        public Window WindowInstance { get { return FWindowInstance; } }

        public void PostMessage(uint Msg, int wParam, int lParam)
        {
            if (FWindowInstance == null) return;

            if (FWindowInstance.Dispatcher.CheckAccess())
            {
                FWindowInstance.Dispatcher.BeginInvoke((MethodInvoker)delegate
                {
                    IntPtr hwnd = new WindowInteropHelper(FWindowInstance).Handle;
                    CMESSAGE.PostMessage(hwnd, Msg, wParam, lParam);
                });
            }
            else
            {
                IntPtr hwnd = new WindowInteropHelper(FWindowInstance).Handle;
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

        #region Step Process Map
        private CProcessMap FProcessMap;
        private DispatcherTimer FTimer = null;

        private void OnTimer(object? sender, EventArgs e)
        {
            if (Environment.Is64BitProcess) return;

            if ((FProcessMap.MonitorActive & 0x04) == 0x04)
            {
                FProcessMap.MonitorActive = 0;

                if (!System.IO.File.Exists("D:\\MONITOR\\")) System.IO.Directory.CreateDirectory("D:\\MONITOR\\");
                SaveMStepMoniter(String.Format("D:\\MONITOR\\{0:D4} {1:D2} {2:D2} {3:D2} {4:D2} {5:D2}.CSV",
                                                DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                                                DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second));
            }
            if ((FProcessMap.MonitorActive & 0x02) == 0x02)
            {
                FProcessMap.MonitorActive = 0;
                FMStartMoniter = 0;
            }
            if ((FProcessMap.MonitorActive & 0x01) == 0x01)
            {
                FProcessMap.MonitorActive = 0;
                FLStepOverFlow &= 0xFE;
                FMStartMoniter = 1;
                FMStepIndex = 0;
            }

            uint cnt = FItemCount;
            if (cnt > 45) cnt = 45;

            for (int i = 0; i < cnt; i++)
            {
                if ((FProcessMap[i].PauseNext & 0x01) == 0x01)
                {
                    FProcessMap[i].PauseNext = 0;
                    FItem[i].PauseNext();
                }

                if (FProcessMap[i].MoveStep >= 0)
                {
                    FItem[i].Move(FProcessMap[i].MoveStep);
                    FProcessMap[i].MoveStep = -1;
                }

                FProcessMap[i].PauseEnabled = FItem[i].PauseEnabled;
                FProcessMap[i].PauseStatus = FItem[i].PauseStatus;
                FProcessMap[i].ClassName = FItem[i].ClassName;
                FProcessMap[i].Step = FItem[i].Step;
            }
        }
        #endregion

        #region 소요 시간 측정
        protected int FTimeCount = 0;
        protected int FTimerTickCount = 0;
        protected double FExecuteInterval = 0.0f;
        public double ExecuteInterval { get { return FExecuteInterval; } }

        private int FThreadInterval = 1;
        public int ThreadInterval
        {
            get { return FThreadInterval; }
            set
            {
                if (FThreadInterval != value && value > 0)
                {
                    for (int i = 0; i < FItemCount; i++) FItem[i].ThreadInterval = value;
                    FThreadInterval = value;
                }
            }
        }
        #endregion

        #region Step Process 선언 및 관리
        protected uint FItemCount = 0;
        protected CStepProc[] FItem = new CStepProc[100];
        public CStepProc GetItem(uint AIndex)
        {
            if (AIndex >= FItemCount) return null;
            return FItem[AIndex];
        }
        public uint Count { get { return FItemCount; } }
        public uint Add(CStepProc AItem)
        {
            if (FItemCount <= 100)
            {
                FItem[FItemCount] = AItem;
                FItemCount++;
            }
            return FItemCount;
        }
        protected virtual void MoveStep(int AStep)
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
        public void ItemsExecute(int ASetTickCount = 1000)
        {
            if (++FTimeCount >= ASetTickCount)
            {
                FExecuteInterval = (double)(Environment.TickCount - FTimerTickCount) / (double)FTimeCount;
                FTimerTickCount = Environment.TickCount;
                FTimeCount = 0;
            }

            if (FKRunMode != RUN_MODE.NONE) { RunMode = FKRunMode; FKRunMode = RUN_MODE.NONE; }
            for (int i = 0; i < FItemCount; i++) RunMode = FItem[i].Execute(RunMode);
            StepMoniter();
        }
        /// <summary>
        /// 추상 함수 이기때문에 반드시 해당 함수를 선언해 주어야 함.
        /// </summary>
        public abstract void Execute();
        #endregion

        #region Key Switch
        //public CSetUpKey SetUpKey { get { return FSetUpKey; } }
        //protected CSetUpKey FSetUpKey = new CSetUpKey("SET UP KEY");

        protected RUN_MODE FKRunMode = RUN_MODE.NONE;
        public virtual void KeySwitch(KEY_SWITCH AKeySwitch)
        {
            switch (AKeySwitch)
            {
                case KEY_SWITCH.SW_START:
                    if (RunMode == RUN_MODE.STOP)
                    {
                        int doorindex = GetDoorOpen(true);
                        FKRunMode = RUN_MODE.TORUN;
                    }
                    break;
                case KEY_SWITCH.SW_STOP:
                    if (RunMode == RUN_MODE.RUN) { FKRunMode = RUN_MODE.TOSTOP; }
                    if (RunMode == RUN_MODE.INIT) { FKRunMode = RUN_MODE.TOSTOP; }
                    if (RunMode == RUN_MODE.TORUN) { FKRunMode = RUN_MODE.TOSTOP; }

                    if (DoHandOperate(1) != 0) { FKRunMode = RUN_MODE.TOSTOP; }
                    break;

                case KEY_SWITCH.SW_SPARE01:
                    if (FMessageHandle != null) PostMessage(FMessageHandle, WM_USER + 99, (int)KEY_SWITCH.SW_INIT, (int)AKeySwitch);
                    break;
                case KEY_SWITCH.SW_INIT:
                    if (RunMode == RUN_MODE.STOP)
                    {
                        int doorindex = GetDoorOpen(true);
                        FKRunMode = RUN_MODE.INIT;
                    }
                    break;
                case KEY_SWITCH.SW_RESET:
                    if (RunMode == RUN_MODE.JAM) FKRunMode = RUN_MODE.STOP;
                    else if (RunMode == RUN_MODE.EJAM) FKRunMode = RUN_MODE.STOP;
                    break;
            }
        }
        #endregion

        #region RUN MODE 관리
        protected RUN_MODE FRunMode;
        protected IntPtr FMessageHandle = IntPtr.Zero;
        protected virtual void SetRunMode(RUN_MODE ARunMode)
        {
            if (FRunMode == ARunMode) return;

            switch (ARunMode)
            {
                case RUN_MODE.PAUSE:
                    PostMessage(WM_USER, (int)ARunMode, (int)FRunMode);
                    FPossiveHandOperate = true;
                    DoorLock = false;

                    FRunMode = ARunMode;
                    return;

                case RUN_MODE.EJAM:
                    if (FRunMode == RUN_MODE.JAM) return;

                    if (FRunMode != RUN_MODE.JAM) PostMessage(WM_USER, (int)ARunMode, (int)FRunMode);
                    DoorLock = false;

                    FRunMode = RUN_MODE.JAM;
                    break;

                case RUN_MODE.TOJAM:
                case RUN_MODE.TOSTOP:
                    PostMessage(WM_USER, (int)ARunMode, (int)FRunMode);

                    switch (FRunMode)
                    {
                        case RUN_MODE.INIT:
                            Init();
                            MoveStep(0);
                            break;
                        case RUN_MODE.TORUN:
                            MoveStep(0);
                            break;
                    }

                    FRunMode = ARunMode;
                    break;

                case RUN_MODE.RUN:
                    if (FRunMode == RUN_MODE.TOJAM) return;
                    if (FRunMode == RUN_MODE.TOSTOP) return;

                    for (int i = 0; i < FItemCount; i++)
                    {
                        if (FItem[i].Element == 0 && FItem[i].Step != 0) return;
                        if (!FItem[i].ConfirmRunMode(ARunMode)) return;
                    }

                    PostMessage(WM_USER, (int)ARunMode, (int)FRunMode);
                    FPossiveHandOperate = (ARunMode == RUN_MODE.STOP || ARunMode == RUN_MODE.JAM);
                    DoorLock = !(ARunMode == RUN_MODE.STOP || ARunMode == RUN_MODE.JAM);

                    FRunMode = ARunMode;
                    break;
                case RUN_MODE.JAM:
                case RUN_MODE.INIT:
                case RUN_MODE.STOP:
                    for (int i = 0; i < FItemCount; i++)
                    {
                        if (FItem[i].Element == 0 && FItem[i].Step != 0) return;
                        if (!FItem[i].ConfirmRunMode(ARunMode)) return;
                    }

                    PostMessage(WM_USER, (int)ARunMode, (int)FRunMode);
                    FPossiveHandOperate = (ARunMode == RUN_MODE.STOP || ARunMode == RUN_MODE.JAM);
                    DoorLock = !(ARunMode == RUN_MODE.STOP || ARunMode == RUN_MODE.JAM);

                    FRunMode = ARunMode;
                    break;
                case RUN_MODE.TORUN:
                    switch (FRunMode)
                    {
                        case RUN_MODE.PAUSE:
                            PostMessage(WM_USER, (int)RUN_MODE.RUN, (int)FRunMode);
                            FPossiveHandOperate = false;
                            DoorLock = true;

                            FRunMode = RUN_MODE.RUN;
                            return;
                        default:
                            for (int i = 0; i < FItemCount; i++)
                            {
                                if (FItem[i].Element == 0 && FItem[i].Step != 0) return;
                                if (!FItem[i].ConfirmRunMode(ARunMode)) return;
                            }

                            PostMessage(WM_USER, (int)ARunMode, (int)FRunMode);
                            FPossiveHandOperate = false;
                            DoorLock = true;

                            FRunMode = ARunMode;
                            break;
                    }
                    break;
            }

            switch (ARunMode)
            {
                case RUN_MODE.JAM:
                case RUN_MODE.EJAM: MoveStep(101); break;

                case RUN_MODE.STOP: MoveStep(0); break;

                case RUN_MODE.RUN: MoveStep(501); break;
                case RUN_MODE.INIT: MoveStep(201); break;
                case RUN_MODE.TORUN: MoveStep(301); break;
            }
        }
        public RUN_MODE RunMode { get { return FRunMode; } set { SetRunMode(value); } }
        protected virtual void Init()
        {

        }
        #endregion

        #region 수동 조작
        protected bool FPossiveHandOperate = true;              //수동 조작이 가능한 상태를 말함.
        public bool PossiveHandOperate { get { return FPossiveHandOperate; } }
        public int DoHandOperate(int AElement)
        {
            for (int i = 0; i < FItemCount; i++)
            {
                if (AElement == 1)
                {
                    if (FItem[i].Element == 1) continue;
                }

                if (FItem[i].HandOperateIndex != 0) return 1;
            }
            return 0;
        }
        public bool HandOperate(int AIndex, int AAction)
        {
            if (!HandOperateStatus(AIndex, AAction, GetDoorOpen(false)))
            { return false; }
            if (AIndex == 99) { RunMode = RUN_MODE.INIT; return true; }

            for (int i = 0; i < FItemCount; i++)
            {
                FItem[i].HandOperate(AIndex, AAction);
                if (FItem[i].Step != 0) DoorLock = true;
            }
            return true;
        }
        public bool HandOperateStatus(int AIndex, int AAction, int ADoorStatus)
        {
            for (int i = 0; i < FItemCount; i++)
            {
                if (!FItem[i].ConfirmRunMode(RUN_MODE.RUN)) return false;
                if (!FItem[i].HandOperateStatus(AIndex, AAction, ADoorStatus)) return false;
            }
            return true;
        }
        #endregion

        #region DOOR 상태
        protected bool FOnDoorLock = false;
        public bool DoorLock { get { return GetDoorLock(); } set { SetDoorLock(value); } }
        protected virtual bool GetDoorLock()
        {
            return (GetDoorLockInput() == 0);
        }
        protected virtual void SetDoorLock(bool ALock)
        {
            FOnDoorLock = ALock;
        }
        protected virtual int GetDoorLockInput()
        {
            return 0;
        }
        protected virtual int GetDoorLockOutput()
        {
            return -1;
        }
        protected virtual int GetDoorOpen(bool ALock)
        {
            return 0;
        }
        #endregion

        #region Step Log
        protected int FAStepIndex = 0;
        protected int FLStepOverFlow = 0x00;
        public int[,] AStep = new int[100, 10000];
        public int AStepIndex { get { return FAStepIndex; } }
        public int LStepOverFlow { get { return FLStepOverFlow; } }

        protected int FMStepIndex = 0;
        protected int FMStartMoniter = 0;
        protected int[,] FMStep = new int[100, 60000];

        public virtual void StepMoniter()
        {
            if (FAStepIndex >= 10000) { FLStepOverFlow |= 0x02; FAStepIndex = 0; }

            for (int i = 0; i < FItemCount; i++) AStep[i, FAStepIndex] = FItem[i].Step;
            FAStepIndex++;

            if (FMStartMoniter == 0) { return; }
            if (FMStepIndex >= 60000) { FLStepOverFlow |= 0x01; FMStepIndex = 0; }

            for (int i = 0; i < FItemCount; i++) FMStep[i, FMStepIndex] = FItem[i].Step;
            FMStepIndex++;
        }
        public virtual void StopMStepMoniter()
        {
            FMStartMoniter = 0;
        }
        public virtual void StartMStepMoniter()
        {
            FLStepOverFlow &= 0xFE;
            FMStartMoniter = 0;
            FMStepIndex = 0;
        }
        public virtual void SaveMStepMoniter(string AFileName = "")
        {
            if (AFileName == "")
            {
                AFileName = System.IO.Path.GetPathRoot(System.Windows.Forms.Application.ExecutablePath) + $"MONITOR\\{DateTime.Now.Year:D4} {DateTime.Now.Month:D2} {DateTime.Now.Day:D2}.INI";
            }

            string path = System.IO.Path.GetDirectoryName(AFileName);
            if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);

            bool fileexist = System.IO.File.Exists(AFileName);
            using (FileStream fs = new FileStream(AFileName, FileMode.Append, FileAccess.Write))
            {
                string message = "";
                if (!fileexist)
                {
                    for (int i = 0; i < FItemCount; i++) message += $"{FItem[i].ClassName},";
                    message += "\r\n";
                }

                if ((FLStepOverFlow & 0x01) == 0x01)
                {
                    for (int i = FMStepIndex + 1; i < 60000; i++)
                    {
                        for (int k = 0; k < FItemCount; k++) message += $"{FMStep[i, k]},";
                        message += "\r\n";
                    }
                }

                for (int i = 0; i < FMStepIndex; i++)
                {
                    for (int k = 0; k < FItemCount; k++) message += $"{FMStep[i, k]},";
                    message += "\r\n";
                }
                fs.Write(System.Text.UnicodeEncoding.Default.GetBytes(message), 0, System.Text.UnicodeEncoding.Default.GetByteCount(message));
            }
        }
        #endregion
    }
}
