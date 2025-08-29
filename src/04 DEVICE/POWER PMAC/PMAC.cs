using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Pcomm32Functions
{
    static class Constants
    {
        public const uint COMM_EOT = 0x80000000;// An acknowledge character(ACK ASCII 9) was received indicating end of transmission from PMAC to Host PC.
        public const uint COMM_TIMEOUT = 0xC0000000;//A timeout occurred.The time for the PC to wait for PMAC to respond had been exceeded.
        public const uint COMM_BADCKSUM = 0xD0000000;//Used when using Checksum communication.If a bad checksum occurred, this error will be returned.
        public const uint COMM_ERROR = 0xE0000000;// Unable to communicate.
        public const uint COMM_FAIL = 0xF0000000;//Serious failure.
        public const uint COMM_ANYERR = 0x70000000;//Some error occurred.
        public const uint COMM_UNSOLICITED = 0x10000000;//An unsolicited response has been received from PMAC. Usually caused by PLC’s or Motion Programs that have “Send” or “Command” statements.


        public const string STR_COMM_EOT = "An acknowledge character(ACK ASCII 9) was received indicating end of transmission from PMAC to Host PC.\n";
        public const string STR_COMM_TIMEOUT = "A timeout occurred.The time for the PC to wait for PMAC to respond had been exceeded.\n";
        public const string STR_COMM_BADCKSUM = "Used when using Checksum communication.If a bad checksum occurred, this error will be returned.\n";
        public const string STR_COMM_ERROR = "Unable to communicate.\n";
        public const string STR_COMM_FAIL = "Serious failure.\n";
        public const string STR_COMM_ANYERR = "Some error occurred.\n";
        public const string STR_COMM_UNSOLICITED = "An unsolicited response has been received from PMAC. Usually caused by PLC’s or Motion Programs that have “Send” or “Command” statements.\n";


        //#define COMM_CHARS(c) (c & 0x0FFFFFFF) // Returns the number of characters
        //#define COMM_STATUS(c) (c & 0xF0000000) // Returns the status byte To check for individual error codes the MACROs below are very useful:
        //#define IS_COMM_MORE(c) ((c & COMM_FAIL) == 0)
        //#define IS_COMM_EOT(c) ((c & COMM_FAIL) == COMM_EOT)
        //#define IS_COMM_TIMEOUT(c) ((c & COMM_FAIL) == COMM_TIMEOUT)
        //#define IS_COMM_BADCKSUM(c) ((c & COMM_FAIL) == COMM_BADCKSUM)
        //#define IS_COMM_ERROR(c) ((c & COMM_FAIL) == COMM_ERROR)
        //#define IS_COMM_FAIL(c) ((c & COMM_FAIL) == COMM_FAIL)
        //#define IS_COMM_ANYERROR(c) ((c & COMM_ANYERR) > 0)
        //#define IS_COMM_UNSOLICITED(c) ((c & 0xF0000000) == COMM_UNSOLICITED)
    }

#if __TURBO_PMAC__ == true

    public class PMAC
    {
        // Returns the number of characters
        public static long getCOMM_CHARS(long c) { return (c & 0x0FFFFFFF); }

        // Returns the status byte
        public static long COMM_STATUS(long c) { return (c & 0xF0000000); }

        //To check for individual error codes the MACROs below are very useful:
        public static bool IS_COMM_MORE(long c) { return ((c & Constants.COMM_FAIL) == 0); }
        public static bool IS_COMM_EOT(long c) { return ((c & Constants.COMM_FAIL) == Constants.COMM_EOT); }
        public static bool IS_COMM_TIMEOUT(long c) { return ((c & Constants.COMM_FAIL) == Constants.COMM_TIMEOUT); }
        public static bool IS_COMM_BADCKSUM(long c) { return ((c & Constants.COMM_FAIL) == Constants.COMM_BADCKSUM); }
        public static bool IS_COMM_ERROR(long c) { return ((c & Constants.COMM_FAIL) == Constants.COMM_ERROR); }
        public static bool IS_COMM_FAIL(long c) { return ((c & Constants.COMM_FAIL) == Constants.COMM_FAIL); }
        public static bool IS_COMM_ANYERROR(long c) { return ((c & Constants.COMM_ANYERR) > 0); }
        public static bool IS_COMM_UNSOLICITED(long c) { return ((c & 0xF0000000) == Constants.COMM_UNSOLICITED); }



        //PCOMM32 PRO.pdf - page 95
        /*
        typedef long (FAR WINAPI *DOWNLOADGETPROC)(long nIndex,LPTSTR lpszBuffer,long nMaxLength);
            This function type is used by some program downloading procedures to offer the option of extracting text
            lines for the download from another source other than a disk file.

            Arguments
                nIndex Line number asked for.
                lpszBuffer Buffer pointer to copy text line into.
                nMaxLength Maximum length of buffer.
            Return Value
                The number of characters copied into the buffer.
        */
        public delegate int DOWNLOADGETPROC(Int32 nIndex, String lpszBuffer, Int32 nMaxLength);

        /*
        typedef void (FAR WINAPI *DOWNLOADPROGRESS)(long nPercent);
           This function type is used by some program downloading procedures to offer the option of setting the
           current percent of progress during the procedure.

           Arguments
               nPercent Current percent of progress.
           Return Value : None
        */
        public delegate void DOWNLOADPROGRESS(Int32 nPercent);

        /*
        typedef void (FAR WINAPI *DOWNLOADMSGPROC)(LPTSTR str,BOOL newline);
           This function type is used by some program downloading procedures to offer the option of reporting a
           status message.

           Arguments
               str Message string.
               newline Indicates if a new line should be added by the called procedure.
           Return Value : None
        */
        public delegate void DOWNLOADMSGPROC(String str, Boolean newline);

        //DPRTESTMSGPROC 
        //typedef void (FAR WINAPI *DPRTESTMSGPROC)(LONG NumErrors, LPTSTR Action,LONG CurrentOffset);
        public delegate void DPRTESTMSGPROC(Int32 nNumErrors, String lpszAction, Int32 nCurrentOffset);

        //typedef void (FAR WINAPI *DPRTESTPROGRESS)(LONG Percent);
        public delegate void DPRTESTPROGRESS(Int32 nPercent);

        //public delegate void DOWNLOADERRORPROC(String fname, Int32 err, Int32 line, String szLine);


        [DllImport("Pcomm32.dll")]
        public static extern Int32 OpenPmacDevice(UInt32 dwDevice);
        [DllImport("Pcomm32.dll")]
        public static extern UInt32 PmacSelect(UInt32 dwDevice);
        [DllImport("Pcomm32.dll")]
        public static extern UInt32 ClosePmacDevice(UInt32 dwDevice);
        [DllImport("Pcomm32.dll")]
        //public static extern Int32 PmacGetResponseA(UInt32 dwDevice, StringBuilder s, UInt32 maxchar, StringBuilder outstr);
        public static extern Int32 PmacGetResponseA(UInt32 dwDevice, Byte[] s, UInt32 maxchar, Byte[] outstr);
        [DllImport("Pcomm32.dll")]
        public static extern Int32 PmacGetResponseA(UInt32 dwDevice, StringBuilder s, UInt32 maxchar, StringBuilder outstr);

        [DllImport("Pcomm32.dll")]
        public static extern long PmacGetResponseExA(uint dwDevice, IntPtr response, uint maxchar, IntPtr command);
        // [DllImport("Pcomm32.dll")]
        //public static extern long PmacGetResponseExA(uint dwDevice, ref IntPtr response, uint maxchar, StringBuilder command);

        [DllImport("Pcomm32.dll")]
        public static extern long PmacGetResponseExA(uint dwDevice, IntPtr response, uint maxchar, StringBuilder command);
        /*
   long PmacGetResponseExA(DWORD dwDevice,PCHAR response,UINT maxchar,PCHAR command);

            dwDevice Device number
            response Pointer to string buffer to copy the PMAC’s response into.
            maxchar Maximum characters to copy.
            command Pointer to NULL terminated string to be sent to the PMAC as a question/command.

            Return Value : If successful Not False(0)
         */
        [DllImport("Pcomm32.dll")]
        public static extern Int32 PmacDownloadA(UInt32 dwDevice, DOWNLOADMSGPROC msgp, DOWNLOADGETPROC getp, DOWNLOADPROGRESS pprg, Byte[] filename, Int32 macro, Int32 map, Int32 log, Int32 dnld);
        [DllImport("Pcomm32.dll")]
        public static extern Int32 PmacDownloadA(UInt32 dwDevice, DOWNLOADMSGPROC msgp, DOWNLOADGETPROC getp, DOWNLOADPROGRESS pprg,
            string filename, Int32 macro, Int32 map, Int32 log, Int32 dnld);

        [DllImport("Pcomm32.dll")]
        public static extern UInt32 PmacGetVariableLong(UInt32 dwDevice, char ch, UInt32 num, UInt32 def);

        /*
          PVOID PmacDPRGetMem(DWORD dwDevice, DWORD offset, size_t count, PVOID val);
Copies a block of dual-ported RAM memory.
Arguments
dwDevice : Device number
offset : Offset from the start of dual-ported RAM.
count : Size of memory block to copy.
val  : Pointer to destination.

            Return - Pointer to destination.
         */
        [DllImport("Pcomm32.dll")]
        public static extern IntPtr PmacDPRGetMem(UInt32 dwDevice, UInt32 offset, UInt32 count, IntPtr val);


        /*
        PmacDPRSetMem()
PVOID PmacDPRSetMem(DWORD dwDevice, DWORD offset, size_t count, PVOID val);
Copies a block of memory into dual-ported RAM.

Arguments
  dwDevice Device number.
  offset Offset from the start of dual-ported RAM.
  count Size of data to transfer.
  val Pointer to memory to transfer.
Return Values - Pointer to transferred data.
        */
        [DllImport("Pcomm32.dll")]
        public static extern IntPtr PmacDPRSetMem(UInt32 dwDevice, UInt32 offset, UInt32 count, IntPtr val);

        [DllImport("Pcomm32.dll")]
        //int CALLBACK PmacGetErrorStrA(DWORD dwDevice, PCHAR str, int maxchar);
        public static extern Int32 PmacGetErrorStrA(UInt32 dwDevice, IntPtr str, UInt32 maxchar);
        [DllImport("Pcomm32.dll")]
        public static extern Int32 PmacGetErrorStrA(UInt32 dwDevice, StringBuilder str, UInt32 maxchar);
    }

#else

    enum DTK_MODE_TYPE
    {
        DM_GPASCII = 0,
        DM_GETSENDS_0 = 1,
        DM_GETSENDS_1 = 2,
        DM_GETSENDS_2 = 3,
        DM_GETSENDS_3 = 4,
        DM_GETSENDS_4 = 5
    };

    enum DTK_STATUS
    {
        DS_Ok = 0,
        DS_Exception = 1,
        DS_TimeOut = 2,
        DS_Connected = 3,
        DS_NotConnected = 4,
        DS_Failed = 5,
        DS_InvalidDevice = 11,
        DS_LengthExceeds = 21,
        DS_RunningDownload = 22,
        DS_RunningRead = 23
    };

    enum DTK_RESET_TYPE
    {
        DR_Reset = 0,
        DR_FullReset = 1
    };



    public class PMAC
    {
        public delegate void PDOWNLOAD_MESSAGE_A(String lpMessage);
        public delegate void PDOWNLOAD_PROGRESS(Int32 nPercent);
        public delegate void PRECEIVE_PROC_A(String lpReveive);

        // 라이브러리 오픈
        #region PowerPmac64.DLL
        // 인자를 NULL로 할 경우 DTKDeviceSelect 함수를 사용하여 장치를 연결해야 한다.
        [DllImport("PowerPmac64.dll", EntryPoint = "DTKPowerPmacOpen")]
        public static extern UInt32 DTKPowerPmacOpen64(UInt32 dwIPAddress, UInt32 uMode);

        // 라이브리리 클로즈
        [DllImport("PowerPmac32.dll", EntryPoint = "DTKPowerPmacClose")]
        public static extern UInt32 DTKPowerPmacClose64(UInt32 uDeviceID);

        // 등록된 디바이스 갯수
        [DllImport("PowerPmac64.dll", EntryPoint = "DTKGetDeviceCount")]
        public static extern UInt32 DTKGetDeviceCount64(out Int32 pnDeviceCount);

        // IP Address 확인
        [DllImport("PowerPmac64.dll", EntryPoint = "DTKGetIPAddress")]
        public static extern UInt32 DTKGetIPAddress64(UInt32 uDeviceID, out UInt32 pdwIPAddress);

        // 장치를 연결
        [DllImport("PowerPmac64.dll", EntryPoint = "DTKConnect")]
        public static extern UInt32 DTKConnect64(UInt32 uDeviceID);

        // 장치를 해제
        [DllImport("PowerPmac64.dll", EntryPoint = "DTKDisconnect")]
        public static extern UInt32 DTKDisconnect64(UInt32 uDeviceID);

        // 장치가 연결되었는지 확인
        [DllImport("PowerPmac64.dll", EntryPoint = "DTKIsConnected")]
        public static extern UInt32 DTKIsConnected64(UInt32 uDeviceID, out Int32 pbConnected);

        [DllImport("PowerPmac64.dll", EntryPoint = "DTKGetResponseA")]
        public static extern UInt32 DTKGetResponseA64(UInt32 uDeviceID, Byte[] lpCommand, Byte[] lpResponse, Int32 nLength);

        //  [DllImport("PowerPmac64.dll", EntryPoint = "DTKGetResponseW")]
        //  public static extern UInt32 DTKGetResponseW64(UInt32 uDeviceID, String lpwCommand, ref String lpwResponse, Int32 nLength);

        [DllImport("PowerPmac64.dll", EntryPoint = "DTKSendCommandA")]
        public static extern UInt32 DTKSendCommandA64(UInt32 uDeviceID, Byte[] lpCommand);

        [DllImport("PowerPmac64.dll", EntryPoint = "DTKAbort")]
        public static extern UInt32 DTKAbort64(UInt32 uDeviceID);

        [DllImport("PowerPmac64.dll", EntryPoint = "DTKDownloadA")]
        public static extern UInt32 DTKDownloadA64(UInt32 uDeviceID, Byte[] lpwDownload, Int32 bDowoload, PDOWNLOAD_PROGRESS lpDownloadProgress, PDOWNLOAD_MESSAGE_A lpDownloadMessage);

        [DllImport("PowerPmac64.dll", EntryPoint = "DTKSetReceiveA")]
        public static extern UInt32 DTKSetReceiveA64(UInt32 uDeviceID, PRECEIVE_PROC_A lpReveiveProc);
        #endregion

        #region PowerPmac32.DLL
        // 인자를 NULL로 할 경우 DTKDeviceSelect 함수를 사용하여 장치를 연결해야 한다.
        [DllImport("PowerPmac32.dll", EntryPoint = "DTKPowerPmacOpen")]
        public static extern UInt32 DTKPowerPmacOpen32(UInt32 dwIPAddress, UInt32 uMode);

        // 라이브리리 클로즈
        [DllImport("PowerPmac32.dll", EntryPoint = "DTKPowerPmacClose")]
        public static extern UInt32 DTKPowerPmacClose32(UInt32 uDeviceID);

        // 등록된 디바이스 갯수
        [DllImport("PowerPmac32.dll", EntryPoint = "DTKGetDeviceCount")]
        public static extern UInt32 DTKGetDeviceCount32(out Int32 pnDeviceCount);

        // IP Address 확인
        [DllImport("PowerPmac32.dll", EntryPoint = "DTKGetIPAddress")]
        public static extern UInt32 DTKGetIPAddress32(UInt32 uDeviceID, out UInt32 pdwIPAddress);

        // 장치를 연결
        [DllImport("PowerPmac32.dll", EntryPoint = "DTKConnect")]
        public static extern UInt32 DTKConnect32(UInt32 uDeviceID);

        // 장치를 해제
        [DllImport("PowerPmac32.dll", EntryPoint = "DTKDisconnect")]
        public static extern UInt32 DTKDisconnect32(UInt32 uDeviceID);

        // 장치가 연결되었는지 확인
        [DllImport("PowerPmac32.dll", EntryPoint = "DTKIsConnected")]
        public static extern UInt32 DTKIsConnected32(UInt32 uDeviceID, out Int32 pbConnected);

        [DllImport("PowerPmac32.dll", EntryPoint = "DTKGetResponseA")]
        public static extern UInt32 DTKGetResponseA32(UInt32 uDeviceID, Byte[] lpCommand, Byte[] lpResponse, Int32 nLength);

        //  [DllImport("PowerPmac32.dll", EntryPoint = "DTKGetResponseW")]
        //  public static extern UInt32 DTKGetResponseW32(UInt32 uDeviceID, String lpwCommand, ref String lpwResponse, Int32 nLength);

        [DllImport("PowerPmac32.dll", EntryPoint = "DTKSendCommandA")]
        public static extern UInt32 DTKSendCommandA32(UInt32 uDeviceID, Byte[] lpCommand);

        [DllImport("PowerPmac32.dll", EntryPoint = "DTKAbort")]
        public static extern UInt32 DTKAbort32(UInt32 uDeviceID);

        [DllImport("PowerPmac32.dll", EntryPoint = "DTKDownloadA")]
        public static extern UInt32 DTKDownloadA32(UInt32 uDeviceID, Byte[] lpwDownload, Int32 bDowoload, PDOWNLOAD_PROGRESS lpDownloadProgress, PDOWNLOAD_MESSAGE_A lpDownloadMessage);

        [DllImport("PowerPmac32.dll", EntryPoint = "DTKSetReceiveA")]
        public static extern UInt32 DTKSetReceiveA32(UInt32 uDeviceID, PRECEIVE_PROC_A lpReveiveProc);
        #endregion

        public static UInt32 DTKPowerPmacOpen(UInt32 dwIPAddress, UInt32 uMode)
        {
            if (Environment.Is64BitProcess) return DTKPowerPmacOpen64(dwIPAddress, uMode);
            else return DTKPowerPmacOpen32(dwIPAddress, uMode);
        }

        public static UInt32 DTKPowerPmacClose(UInt32 uDeviceID)
        {
            if (Environment.Is64BitProcess) return DTKPowerPmacClose64(uDeviceID);
            else return DTKPowerPmacClose32(uDeviceID);
        }

        public static UInt32 DTKGetDeviceCount(out Int32 pnDeviceCount)
        {
            if (Environment.Is64BitProcess) return DTKGetDeviceCount64(out pnDeviceCount);
            else return DTKGetDeviceCount32(out pnDeviceCount);
        }
        public static UInt32 DTKGetIPAddress(UInt32 uDeviceID, out UInt32 pdwIPAddress)
        {
            if (Environment.Is64BitProcess) return DTKGetIPAddress64(uDeviceID, out pdwIPAddress);
            else return DTKGetIPAddress32(uDeviceID, out pdwIPAddress);
        }
        public static UInt32 DTKConnect(UInt32 uDeviceID)
        {
            if (Environment.Is64BitProcess) return DTKConnect64(uDeviceID);
            else return DTKConnect32(uDeviceID);
        }
        public static UInt32 DTKDisconnect(UInt32 uDeviceID)
        {
            if (Environment.Is64BitProcess) return DTKDisconnect64(uDeviceID);
            else return DTKDisconnect32(uDeviceID);
        }
        public static UInt32 DTKIsConnected(UInt32 uDeviceID, out Int32 pbConnected)
        {
            if (Environment.Is64BitProcess) return DTKIsConnected64(uDeviceID, out pbConnected);
            else return DTKIsConnected32(uDeviceID, out pbConnected);
        }
        public static UInt32 DTKGetResponseA(UInt32 uDeviceID, Byte[] lpCommand, Byte[] lpResponse, Int32 nLength)
        {
            if (Environment.Is64BitProcess) return DTKGetResponseA64(uDeviceID, lpCommand, lpResponse, nLength);
            else return DTKGetResponseA32(uDeviceID, lpCommand, lpResponse, nLength);
        }


        public static UInt32 DTKSendCommandA(UInt32 uDeviceID, Byte[] lpCommand)
        {
            if (Environment.Is64BitProcess) return DTKSendCommandA64(uDeviceID, lpCommand);
            else return DTKSendCommandA32(uDeviceID, lpCommand);
        }
        public static UInt32 DTKAbort(UInt32 uDeviceID)
        {
            if (Environment.Is64BitProcess) return DTKAbort64(uDeviceID);
            else return DTKAbort32(uDeviceID);
        }
        public static UInt32 DTKDownloadA(UInt32 uDeviceID, Byte[] lpwDownload, Int32 bDowoload, PDOWNLOAD_PROGRESS lpDownloadProgress, PDOWNLOAD_MESSAGE_A lpDownloadMessage)
        {
            if (Environment.Is64BitProcess) return DTKDownloadA64(uDeviceID, lpwDownload, bDowoload, lpDownloadProgress, lpDownloadMessage);
            else return DTKDownloadA32(uDeviceID, lpwDownload, bDowoload, lpDownloadProgress, lpDownloadMessage);
        }
        public static UInt32 DTKSetReceiveA(UInt32 uDeviceID, PRECEIVE_PROC_A lpReveiveProc)
        {
            if (Environment.Is64BitProcess) return DTKSetReceiveA64(uDeviceID, lpReveiveProc);
            else return DTKSetReceiveA32(uDeviceID, lpReveiveProc);
        }
    }

#endif

}
