using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
#pragma warning disable CS8603 // null 가능 참조에 대한 역참조입니다.

namespace EGGPLANT
{
    [Serializable]
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct TStepMapItem
    {
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.I4, SizeConst = 10)]
        public int[] CycleTime;

        [FieldOffset(40)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public char[] ClassName;

        [FieldOffset(296)]
        public int Step;

        [FieldOffset(300)]
        public int PauseStatus;

        [FieldOffset(304)]
        public int MoveStep;

        [FieldOffset(308)]
        public int PauseNext;

        [FieldOffset(312)]
        public int PauseEnabled;
    }


    class CProcessMapItem
    {
        #region DLL IMPORT
        [System.Runtime.InteropServices.DllImport("StepProcessMapClient.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern IntPtr GetStepMapItem(int AIndex);
        [System.Runtime.InteropServices.DllImport("StepProcessMapClient.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern void SetStepMapItem(int AIndex, IntPtr AItem);

        [System.Runtime.InteropServices.DllImport("StepProcessMapClient.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern int GetStepMapItemCycleTime(int AItemIndex, int AIndex);
        [System.Runtime.InteropServices.DllImport("StepProcessMapClient.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern IntPtr GetStepMapItemClassName(int AItemIndex);
        [System.Runtime.InteropServices.DllImport("StepProcessMapClient.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern int GetStepMapItemStep(int AItemIndex);
        [System.Runtime.InteropServices.DllImport("StepProcessMapClient.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern int GetStepMapItemPauseStatus(int AItemIndex);

        [System.Runtime.InteropServices.DllImport("StepProcessMapClient.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern int GetStepMapItemMoveStep(int AItemIndex);
        [System.Runtime.InteropServices.DllImport("StepProcessMapClient.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern int GetStepMapItemPauseNext(int AItemIndex);
        [System.Runtime.InteropServices.DllImport("StepProcessMapClient.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern int GetStepMapItemPauseEnabled(int AItemIndex);

        [System.Runtime.InteropServices.DllImport("StepProcessMapClient.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern void SetStepMapItemCycleTime(int AItemIndex, int AIndex, int AValue);
        [System.Runtime.InteropServices.DllImport("StepProcessMapClient.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern void SetStepMapItemClassName(int AItemIndex, char[] AptrChar, int ALength);

        [System.Runtime.InteropServices.DllImport("StepProcessMapClient.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern void SetStepMapItemStep(int AItemIndex, int AStep);
        [System.Runtime.InteropServices.DllImport("StepProcessMapClient.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern void SetStepMapItemPauseStatus(int AItemIndex, int AStatus);

        [System.Runtime.InteropServices.DllImport("StepProcessMapClient.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern void SetStepMapItemMoveStep(int AItemIndex, int AStep);
        [System.Runtime.InteropServices.DllImport("StepProcessMapClient.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern void SetStepMapItemPauseNext(int AItemIndex, int ANext);
        [System.Runtime.InteropServices.DllImport("StepProcessMapClient.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern void SetStepMapItemPauseEnabled(int AItemIndex, int AEnabled);
        #endregion

        public CProcessMapItem(int AIndex) { FIndex = AIndex; }
        private int FIndex = 0;

        public void SetCycleTime(int AIndex, int AValue)
        {
            SetStepMapItemCycleTime(FIndex, AIndex, AValue);
        }
        private string FClassName = "";
        public string ClassName
        {
            get { return FClassName; }
            set
            {
                SetStepMapItemClassName(FIndex, value.ToCharArray(), value.Length);
                FClassName = value;
            }
        }

        public int Step
        {
            get { return GetStepMapItemStep(FIndex); }
            set { SetStepMapItemStep(FIndex, value); }
        }
        public int PauseStatus
        {
            get { return GetStepMapItemPauseStatus(FIndex); }
            set { SetStepMapItemPauseStatus(FIndex, value); }
        }

        public int MoveStep
        {
            get { return GetStepMapItemMoveStep(FIndex); }
            set { SetStepMapItemMoveStep(FIndex, value); }
        }
        public int PauseNext
        {
            get { return GetStepMapItemPauseNext(FIndex); }
            set { SetStepMapItemPauseNext(FIndex, value); }
        }
        public int PauseEnabled
        {
            get { return GetStepMapItemPauseEnabled(FIndex); }
            set { SetStepMapItemPauseEnabled(FIndex, value); }
        }
    }

    class CProcessMap : IDisposable
    {
        #region DLL IMPORT
        [System.Runtime.InteropServices.DllImport("StepProcessMapClient.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern IntPtr Version();

        [System.Runtime.InteropServices.DllImport("StepProcessMapClient.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern bool CreateInstance();
        [System.Runtime.InteropServices.DllImport("StepProcessMapClient.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern bool ReleaseInstance();

        [System.Runtime.InteropServices.DllImport("StepProcessMapClient.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern int GetStepMonitorActive();
        [System.Runtime.InteropServices.DllImport("StepProcessMapClient.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern void SetStepMonitorActive(int AActive);

        [System.Runtime.InteropServices.DllImport("StepProcessMapClient.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern IntPtr GetStepMapItem(int AIndex);
        [System.Runtime.InteropServices.DllImport("StepProcessMapClient.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern void SetStepMapItem(int AIndex, IntPtr AItem);
        #endregion

        public CProcessMap()
        {
            for (int i = 0; i < 45; i++) FMapItem[i] = new CProcessMapItem(i);
            bool ret = CreateInstance();
        }
        ~CProcessMap()
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

            ReleaseInstance();
            FDisposed = true;
        }
        #endregion

        public string GetDLLVersion()
        {
            return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(Version());
        }
        public int MonitorActive
        {
            get
            {
                return GetStepMonitorActive();
            }
            set
            {
                SetStepMonitorActive(value);
            }
        }

        public CProcessMapItem this[int AIndex]
        {
            get
            {
                if (AIndex < 0) return null;
                if (AIndex >= 45) return null;

                return FMapItem[AIndex];
            }
        }
        private CProcessMapItem[] FMapItem = new CProcessMapItem[45];
    }
    //---------------------------------------------------------------------------
}
