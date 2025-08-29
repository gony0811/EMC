using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EGGPLANT
{
    public class TSocketBase
    {
        public TSocketBase(Socket ASocket, int ARxBufferSize = 2048)
        {
            Socket = ASocket;
            RxBufferSize = ARxBufferSize;
            RxBuffer = new byte[RxBufferSize];
        }

        public int Port = 5001;
        public Socket Socket = null;
        public string Host = "127.0.0.1";

        private int FRxBufferSize = 0;
        public int RxBufferSize
        {
            get { return FRxBufferSize; }
            set
            {
                if (value <= 0) return;
                if (FRxBufferSize == value) return;

                FRxBufferSize = value;
                RxBuffer = new byte[RxBufferSize];
            }
        }

        public byte[] RxBuffer = null;
        public byte[] TxBuffer = null;
    }

    public class TServerSocket : IDisposable
    {
        private TSocketBase Server = new TSocketBase(null);

        public delegate void DelegateOnSocketServerConnect(Socket ASocket);
        public delegate void DelegateOnSocketServerDisconnect(Socket ASocket);
        public delegate void DelegateOnSocketRead(Socket ASocket, byte[] ABytes, int ALength);
        public delegate void DelegateOnSocketSend(Socket ASocket, byte[] ABytes, int ALength);
        public delegate SocketException DelegateOnSocketError(Socket ASocket, SocketException ASocketException);

        public DelegateOnSocketSend OnSocketSend = null;
        public DelegateOnSocketRead OnSocketRead = null;
        public DelegateOnSocketError OnSocketError = null;
        public DelegateOnSocketServerConnect OnSocketServerConnect = null;
        public DelegateOnSocketServerDisconnect OnSocketServerDisconnect = null;

        ~TServerSocket()
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

            while (FClientSockets.Count > 0)
            {
                FClientSockets[0].Socket.Shutdown(SocketShutdown.Both);
                FClientSockets[0].Socket.Close();
                FClientSockets.RemoveAt(0);
            }
            FDisposed = true;
        }
        #endregion

        public string Host
        {
            get
            {
                if (Server == null) return "none";
                return Server.Host;
            }
            set
            {
                if (Server.Host == value) return;
                Server.Host = value;
            }
        }
        public int Port
        {
            get
            {
                if (Server == null) return 0;
                return Server.Port;
            }
            set
            {
                if (Server.Port == value) return;
                Server.Port = value;
            }
        }

        public bool Blocking = false;
        public bool Active
        {
            get
            {
                try
                {
                    if (Server.Socket != null) return true;
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            set
            {
                if (value)
                {
                    if (Server.Socket == null)
                    {
                        Server.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                        Server.Socket.Blocking = Blocking;

                        IPEndPoint serverEP = new IPEndPoint(IPAddress.Any, Server.Port);
                        Server.Socket.Bind(serverEP);
                        Server.Socket.Listen(10);

                        Server.Socket.BeginAccept(OnAcceptConnect, null);   /* 비동기적으로 클라이언트의 연결 요청을 받는다. */
                    }
                }
                else
                {
                    while (FClientSockets.Count > 0)
                    {
                        FClientSockets[0].Socket.Shutdown(SocketShutdown.Both);
                        FClientSockets[0].Socket.Close();
                        FClientSockets.RemoveAt(0);
                    }

                    if (Server.Socket != null)
                    {
                        Server.Socket.Close();
                        Server.Socket = null;
                    }
                }
            }
        }

        protected System.Collections.Generic.List<TSocketBase> FClientSockets = new System.Collections.Generic.List<TSocketBase>();
        public int ClientCount { get { return FClientSockets.Count; } }
        public Socket ClientSocket(int AIndex)
        {
            if (AIndex < 0) return null;
            if (AIndex >= FClientSockets.Count) return null;

            return FClientSockets[AIndex].Socket;
        }

        public void DisconnectClient(Socket ASocket)
        {
            int idx = 0;
            while (FClientSockets.Count > idx)
            {
                if (FClientSockets[idx].Socket != ASocket) idx++;
                else
                {
#if TSERVER_SOCKET_DEBUG
                    string endpoint = "";
                    try
                    {
                        endpoint = FClientSockets[idx].Socket.RemoteEndPoint.ToString();
                    }
                    catch
                    {
                        endpoint = "";
                    }

                    Console.WriteLine("Socket disconnected to {0}", endpoint);
#endif
                    OnSocketServerDisconnect?.Invoke(FClientSockets[idx].Socket);

                    FClientSockets[idx].Socket.Shutdown(SocketShutdown.Both);
                    FClientSockets[idx].Socket.Close();
                    FClientSockets.RemoveAt(idx);
                }
            }
        }
        private void OnAcceptConnect(IAsyncResult iar)
        {
            TSocketBase client = new TSocketBase(Server.Socket.EndAccept(iar)); /* 클라이언트의 연결 요청을 수락한다.              */
            try
            {
#if TSERVER_SOCKET_DEBUG
                Console.WriteLine("Socket connected to {0}", client.Socket.RemoteEndPoint.ToString());
#endif

                FClientSockets.Add(client);
                OnSocketServerConnect?.Invoke(client.Socket);

                client.Socket.BeginReceive(client.RxBuffer, 0, client.RxBufferSize, SocketFlags.None, new AsyncCallback(OnReceiveData), client);
            }
            catch (SocketException exc)
            {
                SocketException ex;
                if (OnSocketError != null) ex = OnSocketError(client.Socket, exc);
                else ex = exc;

                if (ex.SocketErrorCode != SocketError.Success)
                {
                    switch (ex.SocketErrorCode)
                    {
                        case SocketError.ConnectionAborted:
#if TSERVER_SOCKET_DEBUG
                            Console.WriteLine("Socket connected Error(Connection Aborted)");
#endif
                            DisconnectClient(client.Socket);
                            break;
                        case SocketError.ConnectionRefused:
#if TSERVER_SOCKET_DEBUG
                            Console.WriteLine("Socket connected Error(Connection Refused)");
#endif
                            DisconnectClient(client.Socket);
                            break;
                        case SocketError.ConnectionReset:
#if TSERVER_SOCKET_DEBUG
                            Console.WriteLine("Socket connected Error(Connection Reset)");
#endif
                            DisconnectClient(client.Socket);
                            break;
                        case SocketError.NotConnected:
#if TSERVER_SOCKET_DEBUG
                            Console.WriteLine("Socket connected Error(Not Connected)");
#endif
                            DisconnectClient(client.Socket);
                            break;
                        default:
#if TSERVER_SOCKET_DEBUG
                            Console.WriteLine($"Socket connected Error({ex.SocketErrorCode.ToString()})");
#endif
                            DisconnectClient(client.Socket);
                            break;
                    }
                }
            }
#pragma warning disable CS0168
            catch (Exception e)
            {
#if TSERVER_SOCKET_DEBUG
                Console.WriteLine($"Socket connected exception Error({e.ToString()})");
#endif
                DisconnectClient(client.Socket);
            }
#pragma warning restore CS0168
            Server.Socket.BeginAccept(OnAcceptConnect, null);      /* 비동기적으로 클라이언트의 연결 요청을 받는다.    */
        }

        public void SendText(string message)
        {
            if (!Active) return;
            byte[] txmessage = Encoding.Default.GetBytes(message);

            foreach (TSocketBase client in FClientSockets)
            {
                client.TxBuffer = txmessage;
                client.Socket.BeginSend(txmessage, 0, txmessage.Length, SocketFlags.None, new AsyncCallback(OnSendData), client);
            }
        }
        public void SendBuf(byte[] message, int length)
        {
            if (!Active) return;

            foreach (TSocketBase client in FClientSockets)
            {
                client.TxBuffer = message;
                client.Socket.BeginSend(message, 0, message.Length, SocketFlags.None, new AsyncCallback(OnSendData), client);
            }
        }
        private void OnSendData(IAsyncResult iar)
        {
            TSocketBase client = (TSocketBase)iar.AsyncState;
            OnSocketSend?.Invoke(client.Socket, client.TxBuffer, client.TxBuffer.Length);

#if TSERVER_SOCKET_DEBUG
            Console.WriteLine("Socket Data Send({0}) : {1}", client.Socket.RemoteEndPoint.ToString(), Encoding.ASCII.GetString(client.TxBuffer, 0, client.TxBuffer.Length));
#endif
        }

        private int FReceiveLength = 0;
        private void OnReceiveData(IAsyncResult iar)
        {
            TSocketBase remote = (TSocketBase)iar.AsyncState;
            if (remote.Socket.Connected == false) return;

            try
            {
                FReceiveLength = remote.Socket.EndReceive(iar);
                if (FReceiveLength > 0)
                {
                    OnSocketRead?.Invoke(remote.Socket, remote.RxBuffer, FReceiveLength);

#if TSERVER_SOCKET_DEBUG
                    string sReciveData = Encoding.UTF8.GetString(remote.RxBuffer, 0, FReceiveLength);
                    Console.WriteLine("Socket Data Receive({0}) : {1}", remote.Socket.RemoteEndPoint.ToString(), sReciveData);
#endif

                    remote.Socket.BeginReceive(remote.RxBuffer, 0, remote.RxBufferSize, SocketFlags.None, new AsyncCallback(OnReceiveData), remote);
                }
                else
                {
#if TSERVER_SOCKET_DEBUG
                    Console.WriteLine("Socket Data Receive : but length is zero");
#endif

                    DisconnectClient(remote.Socket);
                }
            }
            catch (SocketException exc)
            {
                SocketException ex;
                if (OnSocketError != null) ex = OnSocketError(remote.Socket, exc);
                else ex = exc;

                if (ex.SocketErrorCode != SocketError.Success)
                {
                    switch (ex.SocketErrorCode)
                    {
                        case SocketError.ConnectionAborted:
#if TSERVER_SOCKET_DEBUG
                            Console.WriteLine("Socket Data Receive Error(ConnectionAborted)");
#endif
                            DisconnectClient(remote.Socket);
                            break;
                        case SocketError.ConnectionRefused:
#if TSERVER_SOCKET_DEBUG
                            Console.WriteLine("Socket Data Receive Error(ConnectionRefused)");
#endif
                            DisconnectClient(remote.Socket);
                            break;
                        case SocketError.ConnectionReset:
#if TSERVER_SOCKET_DEBUG
                            Console.WriteLine("Socket Data Receive Error(ConnectionReset)");
#endif
                            DisconnectClient(remote.Socket);
                            break;
                        default:
#if TSERVER_SOCKET_DEBUG
                            Console.WriteLine($"Socket Data Receive Error({ex.SocketErrorCode.ToString()})");
#endif
                            DisconnectClient(remote.Socket);
                            break;
                    }
                }
            }
#pragma warning disable CS0168
            catch (Exception e)
            {
#if TSERVER_SOCKET_DEBUG
                Console.WriteLine($"Socket Data Receive exception Error({e.ToString()})");
#endif
                DisconnectClient(remote.Socket);
            }
#pragma warning restore CS0168
        }
    }
}
