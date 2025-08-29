using Autofac.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EGGPLANT
{
    public enum SOCKET_CONNECT_STATUS { DISCONNECTED = 0, DISCONNECTING = 1, CONNECTING = 2, CONNECTED = 3, };

    public class CClientInfomation
    {
        public CClientInfomation(Socket ASocket)
        {
            Socket = ASocket;
            ConnectTime = DateTime.Now;
        }

        public int Status = 0;
        public DateTime ConnectTime;

        public Socket Socket = null;
        public string Descrioption = "";

        public int OnLink1 = 0, OnLink2 = 0, OnLink3 = 0;
    }

    public class CCommunicationBase : CTrace
    {

        public CCommunicationBase(bool AClient, String AClassName, String ADirectory = "", bool ATraceEnabled = true) : base(AClassName, ADirectory)
        {
            FDirectory = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\CONFIG\\";
            if (!System.IO.Directory.Exists(FDirectory)) System.IO.Directory.CreateDirectory(FDirectory);
            TraceEnabled = ATraceEnabled;
            FClassName = AClassName;
            FIsClient = AClient;

            if (AClient)
            {
                FClientSocket = new TClientSocket();

                FClientSocket.OnSocketRead = OnSocketRead;
                FClientSocket.OnSocketError = OnSocketError;
                FClientSocket.OnSocketClientConnect = OnSocketConnect;
                FClientSocket.OnSocketClientDisconnect = OnSocketDisconnect;
            }
            else
            {
                FServerSocket = new TServerSocket();

                FServerSocket.OnSocketRead = OnSocketRead;
                FServerSocket.OnSocketError = OnSocketError;
                FServerSocket.OnSocketServerConnect = OnSocketConnect;
                FServerSocket.OnSocketServerDisconnect = OnSocketDisconnect;
            }

            FRxBuffer.Clear();
            ParameterOpen();

            FTimer = new System.Windows.Forms.Timer();
            FTimer.Tick += OnTimer;
            FTimer.Interval = 100;
            FTimer.Enabled = true;
            FMessage.Msg = 0;
        }

        ~CCommunicationBase()
        {
            Dispose(false);
        }
        #region Dispose 구문
        protected override void Dispose(bool ADisposing)
        {
            if (FDisposed) return;
            if (ADisposing) { /* IDisposable 인터페이스를 구현하는 멤버들을 여기서 정리합니다. */}

            if (FServerSocket != null) FServerSocket.Dispose();
            FTimer.Enabled = false;

            base.Dispose(ADisposing);
        }
        #endregion

        protected new string FClassName;
        public new string ClassName { get { return FClassName; } }

        protected string FDirectory;
        public string Directory { get { return FDirectory; } }

        public int AutoConnectionInterval = 0;
        public DateTime ConntectionTryTime = DateTime.MinValue;

        public virtual void ParameterOpen()
        {
            string file = FDirectory + FClassName + ".INI";
            if (!File.Exists(file)) { ParameterSave(); return; }

            if (FIsClient)
            {
                FClientSocket.Host = CINI.ReadString(file, "COMMUNICATION", "HOST", "127.0.0.1");
                FClientSocket.Port = CINI.ReadInteger(file, "COMMUNICATION", "PORT", 5000);

                AutoConnectionInterval = CINI.ReadInteger(file, "AUTO CONNECTION", "INTERVAL", 0);
            }
            else
            {
                FServerSocket.Port = CINI.ReadInteger(file, "COMMUNICATION", "PORT", 5000);
            }
        }
        public virtual void ParameterSave()
        {
            if (!System.IO.Directory.Exists(FDirectory)) System.IO.Directory.CreateDirectory(FDirectory);

            string file = FDirectory + FClassName + ".INI";
            if (FIsClient)
            {
                CINI.WriteString(file, "COMMUNICATION", "HOST", FClientSocket.Host);
                CINI.WriteInteger(file, "COMMUNICATION", "PORT", FClientSocket.Port);

                CINI.WriteInteger(file, "AUTO CONNECTION", "INTERVAL", AutoConnectionInterval);
            }
            else
            {
                CINI.WriteInteger(file, "COMMUNICATION", "PORT", FServerSocket.Port);
            }
        }

        protected bool FIsClient = false;
        public bool TraceEnabled = true;
        public bool IsClient { get { return FIsClient; } }

        protected TClientSocket FClientSocket = null;
        protected TServerSocket FServerSocket = null;
        public TClientSocket ClientSocket { get { return FClientSocket; } }
        public TServerSocket ServerSocket { get { return FServerSocket; } }

        public int Port
        {
            get
            {
                if (FIsClient) return FClientSocket.Port;
                else return FServerSocket.Port;
            }
            set
            {
                if (FIsClient)
                {
                    if (FClientSocket.Port != value) FClientSocket.Port = value;
                }
                else
                {
                    if (FServerSocket.Port != value) FServerSocket.Port = value;
                }
            }
        }
        public string Host
        {
            get
            {
                if (FIsClient) return FClientSocket.Host;
                else return FServerSocket.Host;
            }
            set
            {
                if (FIsClient)
                {
                    if (FClientSocket.Host != value) FClientSocket.Host = value;
                }
                else
                {
                    if (FServerSocket.Host != value) FServerSocket.Host = value;
                }
            }
        }

        #region 타이머
        System.Windows.Forms.Message FMessage;
        private System.Windows.Forms.Timer FTimer = null;
        private void OnTimer(object sender, EventArgs e)
        {
            if (FMessage.Msg == 0x0400 + 1)
            {
                if (FIsClient)
                {
                    if ((SOCKET_CONNECT_STATUS)FMessage.LParam == SOCKET_CONNECT_STATUS.CONNECTED)
                    {
                        FClientSocket.Connect();
                    }
                    else
                    {
                        FClientSocket.Disconnect();
                    }
                }
                else
                {
                    if ((SOCKET_CONNECT_STATUS)FMessage.LParam == SOCKET_CONNECT_STATUS.CONNECTED)
                    {
                        FServerSocket.Active = true;
                    }
                    else
                    {
                        FServerSocket.Active = false;
                    }
                }
                FMessage.Msg = 0x0000;
            }
            else
            {
                if (FIsClient)
                {
                    if (!FClientSocket.Connected)
                    {
                        if (AutoConnectionInterval > 0)
                        {
                            if (FClientSocket.IsConnecting) ConntectionTryTime = DateTime.Now;
                            else
                            {
                                TimeSpan span = DateTime.Now - ConntectionTryTime;
                                if (span.TotalSeconds > AutoConnectionInterval)
                                {
                                    FMessage.LParam = (IntPtr)SOCKET_CONNECT_STATUS.CONNECTED;
                                    ConntectionTryTime = DateTime.Now;
                                    FMessage.Msg = 0x0400 + 1;
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        protected SOCKET_CONNECT_STATUS FConnected = SOCKET_CONNECT_STATUS.DISCONNECTED;
        protected virtual SOCKET_CONNECT_STATUS GetConnect()
        {
            return FConnected;
        }
        protected virtual void SetConnect(SOCKET_CONNECT_STATUS AConnect)
        {
            if (AConnect == SOCKET_CONNECT_STATUS.DISCONNECTING) return;
            if (AConnect == SOCKET_CONNECT_STATUS.CONNECTING) return;

            FMessage.LParam = (IntPtr)AConnect;
            FMessage.Msg = 0x0400 + 1;

            if (AConnect == SOCKET_CONNECT_STATUS.DISCONNECTED) FConnected = SOCKET_CONNECT_STATUS.DISCONNECTING;
            if (AConnect == SOCKET_CONNECT_STATUS.CONNECTED) FConnected = SOCKET_CONNECT_STATUS.CONNECTING;
        }
        public SOCKET_CONNECT_STATUS Connect { get { return GetConnect(); } set { SetConnect(value); } }

        public virtual void OnSocketConnect(Socket ASocket)
        {
            if (!FIsClient) AddClient(ASocket);
            FConnected = SOCKET_CONNECT_STATUS.CONNECTED;

            if (TraceEnabled) Trace("[CONNECT]\t" + ASocket.RemoteEndPoint.ToString());
#if TCOMMUNICATION_BASE_DEBUG
            Console.WriteLine("[CONNECT]\t" + ASocket.RemoteEndPoint.ToString());
#endif
        }
        public virtual void OnSocketDisconnect(Socket ASocket)
        {
            if (FIsClient) FConnected = SOCKET_CONNECT_STATUS.DISCONNECTED;
            else
            {
                DeleteClient(ASocket, false);
                if (FClientList.Count <= 0) FConnected = SOCKET_CONNECT_STATUS.DISCONNECTED;
            }

            string endpoint = "";
            try
            {
                endpoint = ASocket.RemoteEndPoint.ToString();
            }
            catch
            {
                endpoint = "";
            }

            if (TraceEnabled) Trace("[DIS-CONNECT]\t" + endpoint);
            if (FIsClient) ConntectionTryTime = DateTime.Now;
#if TCOMMUNICATION_BASE_DEBUG
            Console.WriteLine("[DIS-CONNECT]\t" + endpoint);
#endif
        }
        public virtual SocketException OnSocketError(Socket ASocket, SocketException ASocketException)
        {
            if (TraceEnabled) Trace("[ERROR]\t" + ASocketException.SocketErrorCode.ToString());
#if TCOMMUNICATION_BASE_DEBUG
            Console.WriteLine("[ERROR]\t" + ASocketException.SocketErrorCode.ToString());
#endif
            return new SocketException((int)ASocketException.SocketErrorCode);
        }

        protected List<CClientInfomation> FClientList = new List<CClientInfomation>();
        public int ConnectCount { get { return FClientList.Count; } }
        public CClientInfomation Connector(int AIndex)
        {
            if (AIndex < 0) return null;
            if (AIndex >= FClientList.Count) return null;

            return FClientList[AIndex];
        }

        protected virtual void AddClient(Socket ASocket)
        {
            if (IsClient) return;

            DeleteClient(ASocket, true);
            FClientList.Add(new CClientInfomation(ASocket));
        }
        protected virtual void DeleteClient(Socket ASocket, bool ADisconnect = true)
        {
            if (IsClient) return;

            int idx = 0;
            while (idx < FClientList.Count)
            {
                if (FClientList[idx].Socket == ASocket)
                {
                    if (ADisconnect) FServerSocket.DisconnectClient(FClientList[idx].Socket);
                    else FClientList.RemoveAt(idx);
                    continue;
                }
                idx++;
            }
        }

        public virtual bool Send(string AMessage)
        {
            if (FConnected != SOCKET_CONNECT_STATUS.CONNECTED) return false;
            if (TraceEnabled) Trace("[SEND]\t" + AMessage);
#if TCOMMUNICATION_BASE_DEBUG
            Console.WriteLine("[SEND]\t" + AMessage);
#endif

            if (FIsClient)
            {
                FClientSocket.SendText(AMessage);
            }
            else
            {
                FServerSocket.SendText(AMessage);
            }
            return true;
        }
        public virtual bool Send(char[] AMessage, int ALength)
        {
            if (FConnected != SOCKET_CONNECT_STATUS.CONNECTED) return false;
            if (TraceEnabled) Trace("[SEND]\t" + AMessage);
#if TCOMMUNICATION_BASE_DEBUG
            Console.WriteLine("[SEND]\t" + AMessage);
#endif

            if (FIsClient)
            {
                FClientSocket.SendBuf(Encoding.ASCII.GetBytes(AMessage), ALength);
            }
            else
            {
                FServerSocket.SendBuf(Encoding.ASCII.GetBytes(AMessage), ALength);
            }
            return true;
        }
        public virtual bool SendBuf(byte[] AMessage, int ALength)
        {
            if (FConnected != SOCKET_CONNECT_STATUS.CONNECTED) return false;
            if (TraceEnabled)
            {
                string bmessage = "", smessage = "";
                for (int i = 0; i < ALength; i++)
                {
                    if (i == 0) { bmessage = AMessage[i].ToString("X2"); smessage = AMessage[i].ToString(); }
                    else { bmessage += " " + AMessage[i].ToString("X2"); smessage += " " + AMessage[i].ToString(); }
                }
#if TCOMMUNICATION_BASE_DEBUG
                Console.WriteLine("[SEND]\t" + bmessage);
                Console.WriteLine("[SEND]\t" + smessage);
#endif
                Trace("[SEND]\t" + bmessage);
                Trace("[SEND]\t" + smessage);
            }

            if (FIsClient)
            {
                FClientSocket.SendBuf(AMessage, ALength);
            }
            else
            {
                FServerSocket.SendBuf(AMessage, ALength);
            }
            return true;
        }

        protected StringBuilder FRxBuffer = new StringBuilder();
        protected virtual void Parsing(Socket ASocket, ref StringBuilder ARxBuffer)
        {
#if TCOMMUNICATION_BASE_DEBUG
            Console.WriteLine("[RECEVIVE]\t" + ARxBuffer);
#endif
            Trace("[RECEVIVE]\t" + ARxBuffer);
            ARxBuffer.Clear();
        }

        public virtual void OnSocketRead(Socket ASocket, byte[] ABytes, int ALength)
        {
            FRxBuffer.Append(Encoding.ASCII.GetString(ABytes, 0, ALength));
            Parsing(ASocket, ref FRxBuffer);
        }
    }
}
