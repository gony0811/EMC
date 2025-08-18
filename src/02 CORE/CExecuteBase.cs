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
    enum KEY_SWITCH { SW_START = 0, SW_STOP = 1, SW_RESET = 2, SW_INIT = 3, SW_SPARE01 = 4, };

    public abstract class CExecuteBase : IDisposable
    {
        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        private const int WM_USER = 0x400;

        public string Version
        {
            get { return "EXECUTE BASE - SEAN (V25.08.13.001)"; }
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

            FTimer.Stop();
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
            IntPtr hwnd = new WindowInteropHelper(FWindowInstance).Handle;
            CMESSAGE.PostMessage(hwnd, Msg, wParam, lParam);
        }
        #region Step Process Map
        private CProcessMap FProcessMap;
        private DispatcherTimer FTimer = null;

        private void OnTimer(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
