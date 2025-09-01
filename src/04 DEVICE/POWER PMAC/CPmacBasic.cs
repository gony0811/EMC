using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Pcomm32Functions;
using Autofac;

namespace EGGPLANT.Device.PowerPmac
{
    class CPMacBasic : IDisposable
    {
        public CPMacBasic()
        {
            // Initialize if needed
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

            Initialized = false;
            FDisposed = true;

        }
        #endregion

        static private int FLinkCount = 0;
        static public int LinkCount { get { return FLinkCount; } }

        static private string FDirectory;
        public string Directory { get { return FDirectory; } }

        static private string FIP = "172.20.0.200";
        public string IP { get { return FIP; } }

        static public uint FDeviceID1 = 0;
        static public uint FDeviceOpen = 0;
        static public bool Initialized = false;

        static public bool Initialize()
        {
            FDirectory = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\CONFIG\\";
            ParameterOpen();

            if (LinkCount <= 0)
            {
#if (__TURBO_PMAC__ == true)
                FDeviceID1 = 0x00;
                FDeviceOpen = PMAC.OpenPmacDevice(FDeviceID1);
#elif (__POWER_PMAC__ == true)
                FDeviceID1 = 0x00;
                FDeviceOpen = PMAC.DTKPowerPmacOpen(CETC.IpToUInt32(FIP), (UInt32)DTK_MODE_TYPE.DM_GPASCII);
#else
				Initialized = true;
#endif		    
            }

            Initialized = (FDeviceOpen == 0)? false : true;
            return Initialized;
        }

        static public void ReleaseInstance()
        {
            if (FLinkCount <= 0) return;

            FLinkCount--;
            if (FLinkCount <= 0)
            {
#if (__TURBO_PMAC__ == true)
                if (Initialized)
                {
                    if (FDeviceID1 == 0x00)
                    {
                        PMAC.ClosePmacDevice(FDeviceID1);
                        FDeviceID1 = 0xFFFFFFFF;
                    }
                }
#elif (__POWER_PMAC__ == true)
                if (Initialized)
                {
                    if (FDeviceID1 == 0x00)
                    {
                        PMAC.DTKPowerPmacClose(FDeviceID1);
                        FDeviceID1 = 0xFFFFFFFF;
                    }
                }

#endif
                ParameterSave();
            }
        }

        static System.Threading.Mutex FMutex = new System.Threading.Mutex(false, "CPmacBasic");
        static byte[] FByResponse = new byte[255];

        static public string PMacCommand(string AString, int ADeviceIndex = 0)
        {
            if (!FMutex.WaitOne(1000, false)) return "";
            string strResponse = "";
#if (__TURBO_PMAC__ == true)
            uint dwMaxchar = 10240;
            string stringcmd = AString;
            Array.Clear(FByResponse, 0, FByResponse.Length);

            byte[] bycommand = System.Text.Encoding.GetEncoding("euc-kr").GetBytes(stringcmd);
            int ret = 0;
            try
            {
                if (FDeviceID1 == 0xFFFFFFFF) FDeviceID1 = 0x00;
                ret = PMAC.PmacGetResponseA(FDeviceID1, FByResponse, dwMaxchar, bycommand);
                if (ret != 0)
                {
                    //return strResponse;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("[{0,8}]\tCPMacBasic, PMacCommand({2}), {1}", "DEBUG", e.Message, AString));
                App.Container.Resolve<CTrace>().Trace(string.Format("[{0,8}]\tCPMacBasic, PMacCommand({2}), {1}", "DEBUG", e.Message, AString));
            }
            strResponse = System.Text.Encoding.GetEncoding("euc-kr").GetString(FByResponse);
#elif (__POWER_PMAC__ == true)
            string stringcmd = AString;
            Array.Clear(FByResponse, 0, FByResponse.Length);
            byte[] bycommand = System.Text.Encoding.GetEncoding("euc-kr").GetBytes(stringcmd);
            uint ret = 0;
            try
            {
                if (FDeviceID1 == 0xFFFFFFFF) FDeviceID1 = 0x00;
                ret = PMAC.DTKGetResponseA(FDeviceID1, bycommand, FByResponse, bycommand.Length);
                if (ret != 0)
                {
                    //return strResponse;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("[{0,8}]\tCPMacBasic, PMacCommand({2}), {1}", "DEBUG", e.Message, AString));
                App.Container.Resolve<CTrace>().Trace(string.Format("[{0,8}]\tCPMacBasic, PMacCommand({2}), {1}", "DEBUG", e.Message, AString));
            }
            strResponse = System.Text.Encoding.GetEncoding("euc-kr").GetString(FByResponse);
#endif
            FMutex.ReleaseMutex();
            return strResponse;
        }

        public float CheckIO(UInt32 IOnum)
        {
            long ret = 0;
#if (__TURBO_PMAC__ == true)
            ret = PMAC.PmacGetVariableLong(FDeviceID1, 'M', IOnum, 0);
#elif (__POWER_PMAC__ == true)
            // Power PMAC does not support direct bit access like Turbo PMAC.
#endif
            return ret;

        }
        static private void ParameterOpen()
        {
            string file = FDirectory + "POWER PMAC STATUS.INI";
            if (!File.Exists(file)) { ParameterSave(); return; }

            FIP = CINI.ReadString(file, "SYSTEM", "IP", "192.168.0.200");
        }
        static private void ParameterSave()
        {
            if (!System.IO.Directory.Exists(FDirectory)) System.IO.Directory.CreateDirectory(FDirectory);

            string file = FDirectory + "POWER PMAC STATUS.INI";
            CINI.WriteString(file, "SYSTEM", "IP", FIP);
        }

        static public int PMacErrorStatus(String AString)
        {
            int ret = -1;
            ret = AString.IndexOf("SYSTEM FILE NOT AVAILABLE"); if (ret >= 0) return 3;
            ret = AString.IndexOf("SYSTEM LOAD CONFIG FILE"); if (ret >= 0) return 5;
            ret = AString.IndexOf("SYSTEM SAVE CONFIG FILE"); if (ret >= 0) return 6;
            ret = AString.IndexOf("SYSTEM SEMAPHHORES NOT AVAILABLE"); if (ret >= 0) return 7;
            ret = AString.IndexOf("SYSTEM SHM NOT AVAILABLE"); if (ret >= 0) return 8;
            ret = AString.IndexOf("SYSTEM RESET TIMEOUT"); if (ret >= 0) return 9;
            ret = AString.IndexOf("ILLEGAL CMD"); if (ret >= 0) return 20;
            ret = AString.IndexOf("ILLEGAL PARAMETER"); if (ret >= 0) return 21;
            ret = AString.IndexOf("PROGRAM NOT IN BUFFER"); if (ret >= 0) return 22;
            ret = AString.IndexOf("OUT OF RANGE NUMBER"); if (ret >= 0) return 23;
            ret = AString.IndexOf("OUT OF ORDER NUMBER"); if (ret >= 0) return 24;
            ret = AString.IndexOf("INVALID NUMBER"); if (ret >= 0) return 25;
            ret = AString.IndexOf("INVALID RANGE"); if (ret >= 0) return 26;
            ret = AString.IndexOf("BREAK POINTS SET"); if (ret >= 0) return 31;
            ret = AString.IndexOf("BUFFER IN USE"); if (ret >= 0) return 33;
            ret = AString.IndexOf("BUFFER FULL"); if (ret >= 0) return 34;
            ret = AString.IndexOf("INVALID LABEL"); if (ret >= 0) return 35;
            ret = AString.IndexOf("INVALID LINE #"); if (ret >= 0) return 36;
            ret = AString.IndexOf("INVALID BRKPT"); if (ret >= 0) return 37;
            ret = AString.IndexOf("PROGRAM RUNNING"); if (ret >= 0) return 38;
            ret = AString.IndexOf("NOT READY TO RUN"); if (ret >= 0) return 39;
            ret = AString.IndexOf("BUFFER NOT DEFINED"); if (ret >= 0) return 40;
            ret = AString.IndexOf("BUFFER ALREADY DEFINED"); if (ret >= 0) return 41;
            ret = AString.IndexOf("NO MOTORS DEFINED"); if (ret >= 0) return 42;
            ret = AString.IndexOf("MOTOR NOT CLOSED LOOP"); if (ret >= 0) return 43;
            ret = AString.IndexOf("MOTOR NOT PHASED"); if (ret >= 0) return 44;
            ret = AString.IndexOf("MOTOR NOT ACTIVE"); if (ret >= 0) return 45;
            ret = AString.IndexOf("COORD JOGGED OUT OF POSITION"); if (ret >= 0) return 46;
            ret = AString.IndexOf("MACRO COM TIMEOUT"); if (ret >= 0) return 50;
            ret = AString.IndexOf("MACRO PORT NOT OPEN"); if (ret >= 0) return 51;
            ret = AString.IndexOf("MACRO RING SELECTED NOT AVAILABLE OR PPMAC NOT SYNCH MASTER"); if (ret >= 0) return 52;
            ret = AString.IndexOf("MACRO NOT AVAILABLE, NO MACRO ICs"); if (ret >= 0) return 53;
            ret = AString.IndexOf("MACRO ASCII REQUEST EXCEEDED BUFFER SIZE"); if (ret >= 0) return 54;
            ret = AString.IndexOf("MACRO ASCII COM TIMEOUT"); if (ret >= 0) return 55;
            ret = AString.IndexOf("MACRO RING INTEGRITY IN FAILED STATE"); if (ret >= 0) return 56;
            ret = AString.IndexOf("MACRO SYNC MASTER MUST HAVE STN=0"); if (ret >= 0) return 57;
            ret = AString.IndexOf("MACRO ASCII COM IN USE BY ANOTHER THREAD"); if (ret >= 0) return 58;
            ret = AString.IndexOf("MACRO MRO FILE OPEN OR READ ERR"); if (ret >= 0) return 59;
            ret = AString.IndexOf("Struct Write Data Error"); if (ret >= 0) return 70;
            ret = AString.IndexOf("Struct Write Undefined Gate Error"); if (ret >= 0) return 71;
            ret = AString.IndexOf("Struct Write L Parameter Error"); if (ret >= 0) return 72;
            ret = AString.IndexOf("Struct Write Index Error"); if (ret >= 0) return 73;
            ret = AString.IndexOf("Struct Write Card ID Error"); if (ret >= 0) return 74;
            ret = AString.IndexOf("Struct Write Error"); if (ret >= 0) return 75;
            ret = AString.IndexOf("Write To Struct Address Error"); if (ret >= 0) return 76;
            ret = AString.IndexOf("Struct Write Gate Part Number Error"); if (ret >= 0) return 77;
            ret = AString.IndexOf("MODBUS SOCKET NOT CONNECTED"); if (ret >= 0) return 80;
            ret = AString.IndexOf("MODBUS SOCKET BUSY"); if (ret >= 0) return 81;
            ret = AString.IndexOf("MODBUS SOCKET SEND/RECV ERROR"); if (ret >= 0) return 82;
            ret = AString.IndexOf("MODBUS SOCKET CREATE ERROR"); if (ret >= 0) return 83;
            ret = AString.IndexOf("MODBUS SERVER EXCEPTION ERROR"); if (ret >= 0) return 84;
            ret = AString.IndexOf("MODBUS SOCKET IN USE"); if (ret >= 0) return 85;
            ret = AString.IndexOf("MODBUS SERVER RESPONSE FORMAT ERROR"); if (ret >= 0) return 86;
            ret = AString.IndexOf("MODBUS SOCKET CONNECT ERROR"); if (ret >= 0) return 87;
            ret = AString.IndexOf("MODBUS SERVER SOCKET LISTEN ERROR"); if (ret >= 0) return 88;
            ret = AString.IndexOf("MACRO STATION: ILLEGAL(I,M,P,Q) DATA_TYPE"); if (ret >= 0) return 91;
            ret = AString.IndexOf("MACRO STATION: ILLEGAL(I,M,P,Q) DATA_NUMBER"); if (ret >= 0) return 92;
            ret = AString.IndexOf("MACRO STATION: REMOTE COM TIMEOUT"); if (ret >= 0) return 94;
            ret = AString.IndexOf("MACRO STATION: ANOTHER STATION AT THIS ADDRESS"); if (ret >= 0) return 95;

            return 0;
        }
    }
}
