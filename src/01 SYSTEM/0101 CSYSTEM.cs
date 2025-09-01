using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Xml;
using Control = System.Windows.Controls.Control;

#pragma warning disable CS8601 // null 가능 참조에 대한 역참조입니다.
#pragma warning disable CS8602 // null 가능 참조에 대한 역참조입니다.
#pragma warning disable CS8603 // null 가능 참조에 대한 역참조입니다.
#pragma warning disable CS8625 // null 가능 참조에 대한 역참조입니다.
namespace EGGPLANT
{
    public partial class CSYS
    {
    }
    /****************************************************************************/
    /* 대개의 클래스에서 사용되는 모듈을 별도로 구성하였습니다.                 */
    /* CINI:                                                                    */
    /* CXML:                                                                    */
    /* CMESSAGE:                                                                */
    /* CETC:                                                                    */
    /* SYSTEM:                                                                  */
    /****************************************************************************/
    #region INI 정의 함수
    static class CINI
    {
        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern int GetPrivateProfileString(String section, String key, String def, StringBuilder retVal, int size, String filePath);

        static public string ReadString(string AFileName, string ASection, string AIdent, string ADefault)
        {
            if (!System.IO.File.Exists(AFileName)) return ADefault;

            try
            {
                StringBuilder sb = new StringBuilder(2047);
                GetPrivateProfileString(ASection, AIdent, null, sb, 2047, AFileName);
                return sb.ToString();
            }
            catch
            {
                return ADefault;
            }
        }
        static public double ReadFloat(string AFileName, string ASection, string AIdent, double ADefault)
        {
            if (!System.IO.File.Exists(AFileName)) return ADefault;

            try
            {
                StringBuilder sb = new StringBuilder(2047);
                GetPrivateProfileString(ASection, AIdent, null, sb, 2047, AFileName);

                if (sb.Length < 1) return ADefault;
                return double.Parse(sb.ToString());
            }
            catch
            {
                return ADefault;
            }
        }
        static public int ReadInteger(string AFileName, string ASection, string AIdent, int ADefault)
        {
            if (!System.IO.File.Exists(AFileName)) return ADefault;

            try
            {
                StringBuilder sb = new StringBuilder(2047);
                GetPrivateProfileString(ASection, AIdent, null, sb, 2047, AFileName);

                if (sb.Length < 1) return ADefault;
                return int.Parse(sb.ToString());
            }
            catch
            {
                return ADefault;
            }
        }
        static public bool ReadBool(string AFileName, string ASection, string AIdent, bool ADefault)
        {
            if (!System.IO.File.Exists(AFileName)) return ADefault;

            try
            {
                StringBuilder sb = new StringBuilder(2047);
                GetPrivateProfileString(ASection, AIdent, null, sb, 2047, AFileName);

                if (sb.Length < 1) return ADefault;
                return bool.Parse(sb.ToString());
            }
            catch
            {
                return ADefault;
            }
        }


        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern long WritePrivateProfileString(String section, String key, String val, String filePath);
        static public void WriteString(string AFileName, string ASection, string AIdent, string AValue)
        {
            try
            {
                WritePrivateProfileString(ASection, AIdent, AValue, AFileName);
            }
            catch
            {
            }
        }
        static public void WriteFloat(string AFileName, string ASection, string AIdent, double AValue)
        {
            try
            {
                WritePrivateProfileString(ASection, AIdent, AValue.ToString(), AFileName);
            }
            catch
            {
            }
        }
        static public void WriteInteger(string AFileName, string ASection, string AIdent, int AValue)
        {
            try
            {
                WritePrivateProfileString(ASection, AIdent, AValue.ToString(), AFileName);
            }
            catch
            {
            }
        }
        static public void WriteBool(string AFileName, string ASection, string AIdent, bool AValue)
        {
            try
            {
                WritePrivateProfileString(ASection, AIdent, AValue.ToString(), AFileName);
            }
            catch
            {
            }
        }
    }
    #endregion


    #region XML 정의 함수
    static class CXML
    {
        static public void AddComment(XmlDocument ADoc, XmlNode ANode, string AComment, int AMode = 0)
        {
            var comment = ADoc.CreateComment(AComment);

            if (AMode == 1) ADoc.InsertAfter(comment, ANode);
            else ANode.AppendChild(comment);
        }
        static public XmlNode CreateElement(XmlDocument ADoc, string ANodeName, string AAttributeName = "")
        {
            string nodename = ANodeName.Replace(" ", "_");
            XmlNode item = ADoc.CreateElement(nodename);

            if (AAttributeName != "")
            {
                XmlAttribute attr = ADoc.CreateAttribute("ID");
                attr.Value = AAttributeName;
                item.Attributes.Append(attr);
            }
            return item;
        }
        static public XmlNode SelectSingleNode(XmlNode ANode, string ANodeName, string AAttributeName)
        {
            string nodename = ANodeName.Replace(" ", "_");
            return ANode.SelectSingleNode($"./{ANodeName}[@ID='{AAttributeName}']");
        }
        static public XmlNode SelectSingleNode(XmlNode ANode, string ANodeName)
        {
            string nodename = ANodeName.Replace(" ", "_");
            return ANode.SelectSingleNode(nodename);
        }

        static public bool Move(XmlNode ASrcNode, XmlNode ATarNode, string AItemName, string AAttributeName)
        {
            XmlNode srcitem = SelectSingleNode(ASrcNode, AItemName, AAttributeName);
            if (srcitem == null) return false;

            XmlNode item = SelectSingleNode(ATarNode, AItemName, AAttributeName);
            if (item == null) return false;
            item.InnerText = srcitem.InnerText;
            return true;
        }

        static public bool Move(XmlNode ASrcNode, XmlNode ATarNode, string AItemName)
        {
            XmlNode srcitem = CXML.SelectSingleNode(ASrcNode, AItemName);
            if (srcitem == null) return false;

            XmlNode item = CXML.SelectSingleNode(ATarNode, AItemName);
            if (item == null) return false;
            item.InnerText = srcitem.InnerText;
            return true;
        }

        static public bool CreateMove(XmlNode ASrcNode, XmlDocument ATarDoc, XmlNode ATarNode, string AItemName)
        {
            if (ASrcNode == null) return false;
            string text = ASrcNode.SelectSingleNode(AItemName).InnerText;

            AddElement(ATarDoc, ATarNode, AItemName, text);
            return true;
        }

        static public void AddElement<T>(XmlDocument ADoc, XmlNode ANode, string AElement, T AInnerText)
        {
            var item = ADoc.CreateElement(AElement);
            item.InnerText = AInnerText.ToString();
            ANode.AppendChild(item);
        }
        static public void AddElement(XmlDocument ADoc, XmlNode ANode, string AElement, double AInnerText)
        {
            var item = ADoc.CreateElement(AElement);
            item.InnerText = AInnerText.ToString();
            ANode.AppendChild(item);
        }
        static public void AddElement(XmlDocument ADoc, XmlNode ANode, string AElement, string AInnerText)
        {
            var item = ADoc.CreateElement(AElement);
            item.InnerText = AInnerText;
            ANode.AppendChild(item);
        }
        static public void AddElement(XmlDocument ADoc, XmlNode ANode, string AElement, bool AInnerText)
        {
            var item = ADoc.CreateElement(AElement);
            item.InnerText = AInnerText.ToString();
            ANode.AppendChild(item);
        }
        static public void AddElement(XmlDocument ADoc, XmlNode ANode, string AElement, uint AInnerText)
        {
            var item = ADoc.CreateElement(AElement);
            item.InnerText = AInnerText.ToString();
            ANode.AppendChild(item);
        }
        static public void AddElement(XmlDocument ADoc, XmlNode ANode, string AElement, int AInnerText)
        {
            var item = ADoc.CreateElement(AElement);
            item.InnerText = AInnerText.ToString();
            ANode.AppendChild(item);
        }

        static public void GetInnerText(XmlNode ANode, string ASection, out double AValue, double ADefValue = 0.0d)
        {
            if (ANode == null) { AValue = ADefValue; return; }

            try
            {
                if (!double.TryParse(ANode.SelectSingleNode(ASection).InnerText, out AValue)) AValue = ADefValue;
            }
            catch (Exception)
            {
                AValue = ADefValue;
            }
        }
        static public void GetInnerText(XmlNode ANode, string ASection, out float AValue, float ADefValue = 0.0f)
        {
            if (ANode == null) { AValue = ADefValue; return; }

            try
            {
                if (!float.TryParse(ANode.SelectSingleNode(ASection).InnerText, out AValue)) AValue = ADefValue;
            }
            catch (Exception)
            {
                AValue = ADefValue;
            }
        }
        static public void GetInnerText(XmlNode ANode, string ASection, out string AValue, string ADefValue = "")
        {
            if (ANode == null) { AValue = ADefValue; return; }
            try
            {
                AValue = ANode.SelectSingleNode(ASection).InnerText;
            }
            catch (Exception)
            {
                AValue = ADefValue;
            }
        }
        static public void GetInnerText(XmlNode ANode, string ASection, out bool AValue, bool ADefValue = false)
        {
            if (ANode == null) { AValue = ADefValue; return; }
            try
            {
                if (!bool.TryParse(ANode.SelectSingleNode(ASection).InnerText, out AValue)) AValue = ADefValue;
            }
            catch (Exception)
            {
                AValue = ADefValue;
            }
        }
        static public void GetInnerText(XmlNode ANode, string ASection, out uint AValue, uint ADefValue = 0)
        {
            if (ANode == null) { AValue = ADefValue; return; }
            try
            {
                if (!uint.TryParse(ANode.SelectSingleNode(ASection).InnerText, out AValue)) AValue = ADefValue;
            }
            catch (Exception)
            {
                AValue = ADefValue;
            }
        }
        static public void GetInnerText(XmlNode ANode, string ASection, out int AValue, int ADefValue = 0)
        {
            if (ANode == null) { AValue = ADefValue; return; }
            try
            {
                if (!int.TryParse(ANode.SelectSingleNode(ASection).InnerText, out AValue)) AValue = ADefValue;
            }
            catch (Exception)
            {
                AValue = ADefValue;
            }
        }
        static public void GetInnerText(XmlNode ANode, string ASection, out DateTime AValue, DateTime ADefValue = new DateTime())
        {
            if (ANode == null) { AValue = ADefValue; return; }
            try
            {
                if (!DateTime.TryParse(ANode.SelectSingleNode(ASection).InnerText, out AValue)) AValue = ADefValue;
            }
            catch (Exception)
            {
                AValue = ADefValue;
            }
        }

        static public double GetInnerText(XmlNode ANode, string ASection, double ADefValue = 0.0d)
        {
            if (ANode == null) { return ADefValue; }

            try
            {
                double value;
                if (!double.TryParse(ANode.SelectSingleNode(ASection).InnerText, out value)) value = ADefValue;

                return value;
            }
            catch (Exception)
            {
                return ADefValue;
            }
        }
        static public string GetInnerText(XmlNode ANode, string ASection, string ADefValue = "")
        {
            if (ANode == null) { return ADefValue; }
            try
            {
                string value;
                value = ANode.SelectSingleNode(ASection).InnerText;

                return value;
            }
            catch (Exception)
            {
                return ADefValue;
            }
        }
        static public bool GetInnerText(XmlNode ANode, string ASection, bool ADefValue = false)
        {
            if (ANode == null) { return ADefValue; }
            try
            {
                bool value;
                if (!bool.TryParse(ANode.SelectSingleNode(ASection).InnerText, out value)) value = ADefValue;

                return value;
            }
            catch (Exception)
            {
                return ADefValue;
            }
        }
        static public uint GetInnerText(XmlNode ANode, string ASection, uint ADefValue = 0)
        {
            if (ANode == null) { return ADefValue; }
            try
            {
                uint value;
                if (!uint.TryParse(ANode.SelectSingleNode(ASection).InnerText, out value)) value = ADefValue;

                return value;
            }
            catch (Exception)
            {
                return ADefValue;
            }
        }
        static public int GetInnerText(XmlNode ANode, string ASection, int ADefValue = 0)
        {
            if (ANode == null) { return ADefValue; }
            try
            {
                int value;

                if (!int.TryParse(ANode.SelectSingleNode(ASection).InnerText, out value)) value = ADefValue;

                return value;
            }
            catch (Exception)
            {
                return ADefValue;
            }
        }
        static public ushort GetInnerText(XmlNode ANode, string ASection, ushort ADefValue = 0)
        {
            if (ANode == null) { return ADefValue; }
            try
            {
                ushort value;
                if (!ushort.TryParse(ANode.SelectSingleNode(ASection).InnerText, out value)) value = ADefValue;

                return value;
            }
            catch (Exception)
            {
                return ADefValue;
            }
        }
        static public short GetInnerText(XmlNode ANode, string ASection, short ADefValue = 0)
        {
            if (ANode == null) { return ADefValue; }
            try
            {
                short value;
                if (!short.TryParse(ANode.SelectSingleNode(ASection).InnerText, out value)) value = ADefValue;

                return value;
            }
            catch (Exception)
            {
                return ADefValue;
            }
        }
        static public DateTime GetInnerText(XmlNode ANode, string ASection, DateTime ADefValue = new DateTime())
        {
            if (ANode == null) { return ADefValue; }
            try
            {
                DateTime value;
                if (!DateTime.TryParse(ANode.SelectSingleNode(ASection).InnerText, out value)) value = ADefValue;

                return value;
            }
            catch (Exception)
            {
                return ADefValue;
            }
        }
    }
    #endregion

    #region 메세지 정의
    static class CMESSAGE
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static public extern long ReleaseCapture();
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static public extern IntPtr SetCapture(long hWnd);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static public extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static public extern bool SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        /// <summary>
        /// 윈도우 위치 설정하기
        /// </summary>
        /// <param name="windowHandle">윈도우 핸들</param>
        /// <param name="windowHandleInsertAfter">삽입 이후 윈도우 핸들</param>
        /// <param name="x">X 좌표</param>
        /// <param name="y">X 좌표</param>
        /// <param name="width">너비</param>
        /// <param name="height">높이</param>
        /// <param name="flag">플래그</param>
        /// <returns>처리 결과</returns>
        static public extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static public extern bool SetForegroundWindow(IntPtr hWnd);

        static public void PostMessage(Window AWindow, uint Msg, int wParam, int lParam)
        {
            if (AWindow == null) return;
            IntPtr hwnd = new WindowInteropHelper(AWindow).Handle;
            CMESSAGE.PostMessage(hwnd, Msg, wParam, lParam);
        }

        public const int WM_USER = 0x400;
        public const int WM_DRAWITEM = 0x002B;
        public const int WM_REFLECT = WM_USER + 0x1C00;

        public const int WM_SETFOCUS = 0x0007;
        public const int WM_ACTIVATE = 0x0006;

        public const int WM_KEYDOWN = 0x0100;
        public const int WM_KEYUP = 0x0101;
        public const int WM_CHAR = 0x0102;

        public const int WM_MOUSEMOVE = 0x0200;
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_LBUTTONUP = 0x0202;
        public const int WM_LBUTTONDBLCLK = 0x0203;
        public const int WM_RBUTTONDOWN = 0x0204;
        public const int WM_RBUTTONUP = 0x0205;
        public const int WM_RBUTTONDBLCLK = 0x0206;
        public const int WM_MBUTTONDOWN = 0x0207;
        public const int WM_MBUTTONUP = 0x0208;
        public const int WM_MBUTTONDBLCLK = 0x0209;
        public const int WM_MOUSEWHEEL = 0x020A;
        public const int WM_XBUTTONDOWN = 0x020B;
        public const int WM_XBUTTONUP = 0x020C;
        public const int WM_XBUTTONDBLCLK = 0x020D;

        #region 가상 키보드 이벤트
        public const int VK_CANCEL = 0x03;
        public const int VK_BACK = 0x08;
        public const int VK_CLEAR = 0x0C;
        public const int VK_RETURN = 0x0D;

        public const int VK_LEFT = 0x25;
        public const int VK_UP = 0x26;
        public const int VK_RIGHT = 0x27;
        public const int VK_DOWN = 0x28;

        public const int VK_ESCAPE = 0x1B;
        public const int VK_0 = 0x30;
        public const int VK_1 = 0x31;
        public const int VK_2 = 0x32;
        public const int VK_3 = 0x33;
        public const int VK_4 = 0x34;
        public const int VK_5 = 0x35;
        public const int VK_6 = 0x36;
        public const int VK_7 = 0x37;
        public const int VK_8 = 0x38;
        public const int VK_9 = 0x39;

        public const int VK_NUMPAD0 = 0x60;
        public const int VK_NUMPAD1 = 0x61;
        public const int VK_NUMPAD2 = 0x62;
        public const int VK_NUMPAD3 = 0x63;
        public const int VK_NUMPAD4 = 0x64;
        public const int VK_NUMPAD5 = 0x65;
        public const int VK_NUMPAD6 = 0x66;
        public const int VK_NUMPAD7 = 0x67;
        public const int VK_NUMPAD8 = 0x68;
        public const int VK_NUMPAD9 = 0x69;

        public const int VK_ADD = 0x6B;
        public const int VK_OEM_PLUS = 0xBB;

        public const int VK_SUBTRACT = 0x6D;
        public const int VK_OEM_MINUS = 0xBD;
        #endregion

        #region 컴포넌트 이벤트
        public const int SWP_NOSIZE = 0x0001;
        public const int SWP_NOMOVE = 0x0002;
        public const int SWP_NOZORDER = 0x0004;
        public const int SWP_SHOWWINDOW = 0x0040;
        #endregion
    }
    #endregion

    #region 기타 공통
    static class CETC
    {
        static public int ToInt(object AValue, int ADefine = 0)
        {
            Control control = AValue as Control;
            if (control == null || control.Tag == null) return ADefine;

            int v = ADefine;
            if (int.TryParse(control.Tag.ToString(), out v)) return v;

            return ADefine;
        }
        static public bool ToBool(object AValue, bool ADefine = false)
        {
            Control control = AValue as Control;
            if (control == null || control.Tag == null) return ADefine;

            bool v = ADefine;
            if (bool.TryParse(control.Tag.ToString(), out v)) return v;

            return ADefine;
        }
        static public double ToDouble(object AValue, double ADefine = 0.0)
        {
            Control control = AValue as Control;
            if (control == null || control.Tag == null) return ADefine;
            if (double.TryParse(control.Tag.ToString(), out double v)) return v;

            return ADefine;
        }

        static public int ToInt(string AValue, int ADefine = 0)
        {
            int v = ADefine;
            if (int.TryParse(AValue, out v)) return v;

            return ADefine;
        }
        static public bool ToBool(string AValue, bool ADefine = false)
        {
            bool v = ADefine;
            if (bool.TryParse(AValue, out v)) return v;

            return ADefine;
        }
        static public double ToDouble(string AValue, double ADefine = 0.0)
        {
            double v = ADefine;
            if (double.TryParse(AValue, out v)) return v;

            return ADefine;
        }

        static public bool GetBit(int AValue, int AIndex)
        {
            return ((AValue & (0x01 << AIndex)) != 0x00);
        }
        static public void SetBit(ref int AValue, int AIndex, bool ASet)
        {
            if (ASet) AValue = AValue | (0x01 << AIndex);
            else AValue = AValue & ~(0x01 << AIndex);
        }

        static public uint IpToUInt32(string ip)
        {
            var bytes = System.Net.IPAddress.Parse(ip).GetAddressBytes();
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes); // 네트워크(빅엔디언)로 맞춤
            return BitConverter.ToUInt32(bytes, 0);
        }

        static public uint StrIpAddrToDWord(string ipAddress)
        {
            UInt32 uIPAddress;
            String[] strIP = new String[4];
            strIP = ipAddress.Split('.');
            uIPAddress = (Convert.ToUInt32(strIP[0]) << 24) | (Convert.ToUInt32(strIP[1]) << 16) | (Convert.ToUInt32(strIP[2]) << 8) | Convert.ToUInt32(strIP[3]);

            return uIPAddress;
        }
    }
    #endregion

    #region 윈도우 시간 설정(반드시 관리자 권한으로 실행해야 합니다.)
    public class SYSTEM
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        public extern static void GetSystemTime(ref SYSTEMTIME lpSystemTime);

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        public extern static uint SetSystemTime(ref SYSTEMTIME lpSystemTime);

        public struct SYSTEMTIME
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;
        }

        public static bool IsAdministrator()
        {
            //System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();

            //if (identity != null)
            //{
            //    System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(identity);
            //    return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
            //}
            return false;
        }

        public static bool RunToAdministrator()
        {
            if (IsAdministrator() == false)
            {
                try
                {
                    ProcessStartInfo procInfo = new ProcessStartInfo();
                    procInfo.UseShellExecute = true;
                    procInfo.FileName = AppDomain.CurrentDomain.BaseDirectory;
                    procInfo.WorkingDirectory = Environment.CurrentDirectory;
                    procInfo.Verb = "runas";
                    Process.Start(procInfo);
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }
    }
    #endregion

    /****************************************************************************/
    /* 디버깅을 위해서 만들어 놓은 클래스 모음입니다.                           */
    /* MIDebug: 프로그램 강제 종료되었을때 "MI DEBUG.TXT"파일이 저장됩니다.     */
    /* MinidumpWriter: 메모리 누수등을 점검을 위한 덤프 파일 생성 모듈입니다.   */
    /****************************************************************************/
    #region 프로그램 오류로 인한 로그 저장(디버깅 용)
    static public class MIDebug
    {
        static public string Directory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"\\");
        static public System.Diagnostics.TextWriterTraceListener TextWriter = null;

        static public void Write(string AMessage)
        {
            if (TextWriter == null)
            {
                TextWriter = new System.Diagnostics.TextWriterTraceListener(Directory + "MI DEBUG.TXT");
                Trace.Listeners.Add(TextWriter);
            }

            Trace.WriteLine(String.Format("{0}\t{1}\t{2}", new object[] { DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss.fff"), AMessage }));
            Trace.Flush();
        }
    }
    #endregion

    #region 덤프 파일 작성(디버깅 용)
    /// <summary>
    /// [출처] C# - 프로세스 스스로 풀 덤프 남기는 방법|작성자 techshare
    /// http://blog.naver.com/PostView.nhn?blogId=techshare&logNo=100194859982
    /// 사용법
    /// int pid = System.Diagnostics.Process.GetCurrentProcess().Id;
    /// MinidumpWriter.MakeDump(@"c:\test.dmp", pid);
    /// </summary>
    public class MinidumpWriter
    {
        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWow64Process([In] IntPtr processHandle,
             [Out, MarshalAs(UnmanagedType.Bool)] out bool wow64Process);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetCurrentProcess();

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool OpenProcessToken(IntPtr ProcessHandle,
            UInt32 DesiredAccess, out IntPtr TokenHandle);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool LookupPrivilegeValue(string lpSystemName, string lpName,
            out LUID lpLuid);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AdjustTokenPrivileges(IntPtr TokenHandle,
           [MarshalAs(UnmanagedType.Bool)] bool DisableAllPrivileges,
           ref TOKEN_PRIVILEGES NewState,
           UInt32 Zero,
           IntPtr Null1,
           IntPtr Null2);

        [DllImport("DbgHelp.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        private static extern Boolean MiniDumpWriteDump(
                                    IntPtr hProcess,
                                    Int32 processId,
                                    IntPtr fileHandle,
                                    MiniDumpType dumpType,
                                    IntPtr excepInfo,
                                    IntPtr userInfo,
                                    IntPtr extInfo);

        [StructLayout(LayoutKind.Sequential)]
        public struct LUID
        {
            public UInt32 LowPart;
            public Int32 HighPart;
        }

        public enum MiniDumpType
        {
            Normal = 0x00000000,
            WithDataSegs = 0x00000001,
            WithFullMemory = 0x00000002,
            WithHandleData = 0x00000004,
            FilterMemory = 0x00000008,
            ScanMemory = 0x00000010,
            WithUnloadedModules = 0x00000020,
            WithIndirectlyReferencedMemory = 0x00000040,
            FilterModulePaths = 0x00000080,
            WithProcessThreadData = 0x00000100,
            WithPrivateReadWriteMemory = 0x00000200,
            WithoutOptionalData = 0x00000400,
            WithFullMemoryInfo = 0x00000800,
            WithThreadInfo = 0x00001000,
            WithCodeSegs = 0x00002000,
            WithoutAuxiliaryState = 0x00004000,
            WithFullAuxiliaryState = 0x00008000
        }

        private static uint STANDARD_RIGHTS_REQUIRED = 0x000F0000;
        private static uint STANDARD_RIGHTS_READ = 0x00020000;
        private static uint TOKEN_ASSIGN_PRIMARY = 0x0001;
        private static uint TOKEN_DUPLICATE = 0x0002;
        private static uint TOKEN_IMPERSONATE = 0x0004;
        private static uint TOKEN_QUERY = 0x0008;
        private static uint TOKEN_QUERY_SOURCE = 0x0010;
        private static uint TOKEN_ADJUST_PRIVILEGES = 0x0020;
        private static uint TOKEN_ADJUST_GROUPS = 0x0040;
        private static uint TOKEN_ADJUST_DEFAULT = 0x0080;
        private static uint TOKEN_ADJUST_SESSIONID = 0x0100;
        private static uint TOKEN_READ = (STANDARD_RIGHTS_READ | TOKEN_QUERY);
        private static uint TOKEN_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | TOKEN_ASSIGN_PRIMARY |
            TOKEN_DUPLICATE | TOKEN_IMPERSONATE | TOKEN_QUERY | TOKEN_QUERY_SOURCE |
            TOKEN_ADJUST_PRIVILEGES | TOKEN_ADJUST_GROUPS | TOKEN_ADJUST_DEFAULT |
            TOKEN_ADJUST_SESSIONID);

        public const string SE_DEBUG_NAME = "SeDebugPrivilege";

        public const UInt32 SE_PRIVILEGE_ENABLED_BY_DEFAULT = 0x00000001;
        public const UInt32 SE_PRIVILEGE_ENABLED = 0x00000002;
        public const UInt32 SE_PRIVILEGE_REMOVED = 0x00000004;
        public const UInt32 SE_PRIVILEGE_USED_FOR_ACCESS = 0x80000000;

        [StructLayout(LayoutKind.Sequential)]
        public struct TOKEN_PRIVILEGES
        {
            public UInt32 PrivilegeCount;
            public LUID Luid;
            public UInt32 Attributes;
        }

        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VMOperation = 0x00000008,
            VMRead = 0x00000010,
            VMWrite = 0x00000020,
            DupHandle = 0x00000040,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            Synchronize = 0x00100000
        }

        public static bool MakeDump(string dumpFilePath, int processId)
        {
            SetDumpPrivileges();

            Process targetProcess = Process.GetProcessById(processId);
            using (System.IO.FileStream stream = new System.IO.FileStream(dumpFilePath, System.IO.FileMode.Create))
            {
                Boolean res = MiniDumpWriteDump(
                    targetProcess.Handle,
                                    processId,
                                    stream.SafeFileHandle.DangerousGetHandle(),
                                    MiniDumpType.WithFullMemory,
                                    IntPtr.Zero,
                                    IntPtr.Zero,
                                    IntPtr.Zero);

                int dumpError = res ? 0 : Marshal.GetLastWin32Error();
                Console.WriteLine(dumpError);
            }

            CloseHandle(targetProcess.Handle);

            return true;
        }

        private static void SetDumpPrivileges()
        {
            IntPtr hToken;
            LUID luidSEDebugNameValue;
            TOKEN_PRIVILEGES tkpPrivileges;

            if (!OpenProcessToken(GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, out hToken))
            {
                Console.WriteLine("OpenProcessToken() failed, error = {0} . SeDebugPrivilege is not available", Marshal.GetLastWin32Error());
                return;
            }

            if (!LookupPrivilegeValue(null, SE_DEBUG_NAME, out luidSEDebugNameValue))
            {
                Console.WriteLine("LookupPrivilegeValue() failed, error = {0} .SeDebugPrivilege is not available", Marshal.GetLastWin32Error());
                CloseHandle(hToken);
                return;
            }

            tkpPrivileges.PrivilegeCount = 1;
            tkpPrivileges.Luid = luidSEDebugNameValue;
            tkpPrivileges.Attributes = SE_PRIVILEGE_ENABLED;

            if (!AdjustTokenPrivileges(hToken, false, ref tkpPrivileges, 0, IntPtr.Zero, IntPtr.Zero))
            {
                Console.WriteLine("LookupPrivilegeValue() failed, error = {0} .SeDebugPrivilege is not available", Marshal.GetLastWin32Error());
                return;
            }

            CloseHandle(hToken);
        }

        static public void WriteMessage(string AFileName, string AMessage)
        {
            using (System.IO.FileStream fs = new System.IO.FileStream(AFileName, System.IO.FileMode.Append, System.IO.FileAccess.Write))
            {
                string message = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + AMessage + "\r\n";

                byte[] text = System.Text.UnicodeEncoding.Default.GetBytes(message.ToString());
                fs.Write(text, 0, text.Length);
            }
        }
    }
    #endregion
}
