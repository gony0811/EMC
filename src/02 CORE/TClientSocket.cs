using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EGGPLANT
{
    public class TClientSocket
    {
        public TSocketBase Client = new TSocketBase(null);

        public delegate void DelegateOnSocketClientConnect(Socket ASocket);
        public delegate void DelegateOnSocketClientDisconnect(Socket ASocket);
        public delegate void DelegateOnSocketRead(Socket ASocket, byte[] ABytes, int ALength);
        public delegate void DelegateOnSocketSend(Socket ASocket, byte[] ABytes, int ALength);
        public delegate SocketException DelegateOnSocketError(Socket ASocket, SocketException ASocketException);

        public DelegateOnSocketSend OnSocketSend = null;
        public DelegateOnSocketRead OnSocketRead = null;
        public DelegateOnSocketError OnSocketError = null;
        public DelegateOnSocketClientConnect OnSocketClientConnect = null;
        public DelegateOnSocketClientDisconnect OnSocketClientDisconnect = null;

        public string Host
        {
            get
            {
                if (Client == null) return "none";
                return Client.Host;
            }
            set
            {
                if (Client.Host == value) return;
                Client.Host = value;
            }
        }
        public int Port
        {
            get
            {
                if (Client == null) return 0;
                return Client.Port;
            }
            set
            {
                if (Client.Port == value) return;
                Client.Port = value;
            }
        }

        private bool FIsConnecting = false;
        public bool IsConnecting { get { return FIsConnecting; } }

        public bool Connect(bool AIsBlocking = false)
        {
            try
            {
                if (Client.Socket == null)
                {
                    Client.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    Client.Socket.Blocking = AIsBlocking;
                }
                if (Connected) return false;

                FIsConnecting = true;
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(Client.Host), Client.Port);
                Client.Socket.BeginConnect(remoteEP, new AsyncCallback(OnConnected), Client.Socket);
            }
#pragma warning disable CS0168
            catch (SocketException ex)
            {
#if TCLIENT_SOCKET_DEBUG
                Console.WriteLine($"Socket connected exception Error({ex.ToString()})");
#endif
#pragma warning restore CS0168
                return false;
            }
#pragma warning restore CS0168
            return true;
        }
        public void Disconnect()
        {
            if (Client.Socket == null) return;

            try
            {
#if TCLIENT_SOCKET_DEBUG
                string endpoint = "";
                try
                {
                    endpoint = Client.Socket.RemoteEndPoint.ToString();
                }
                catch
                {
                    endpoint = "";
                }
                Console.WriteLine("Socket dis-connected to {0}", endpoint);
#endif

                Client.Socket.Shutdown(SocketShutdown.Both);
            }
            catch
            {

            }
            Client.Socket.Close();
            OnSocketClientDisconnect?.Invoke(Client.Socket);

            Client.Socket = null;
        }

        public bool Connected
        {
            get
            {
                try
                {
                    if (Client.Socket == null) return false;
                    if (Client.Socket.Connected == false) return false;

                    bool poll = (Client.Socket.Poll(1000, SelectMode.SelectRead));
                    bool available = (Client.Socket.Available == 0);
                    return !(poll && available);
                }
                catch
                {
                    return false;
                }
            }
        }

        private void OnConnected(IAsyncResult iar)
        {
            Client.Socket = (Socket)iar.AsyncState;
            FIsConnecting = false;
            try
            {
#if TCLIENT_SOCKET_DEBUG
                Console.WriteLine("Socket connected to {0}", Client.Socket.RemoteEndPoint.ToString());
#endif
                Client.Socket.EndConnect(iar);

                OnSocketClientConnect?.Invoke(Client.Socket);
                Client.Socket.BeginReceive(Client.RxBuffer, 0, Client.RxBufferSize, SocketFlags.None, new AsyncCallback(OnReceiveData), Client);
            }
            catch (SocketException exc)
            {
                SocketException ex;
                if (OnSocketError != null) ex = OnSocketError(Client.Socket, exc);
                else ex = exc;

                if (ex.SocketErrorCode != SocketError.Success)
                {
                    switch (ex.SocketErrorCode)
                    {
                        case SocketError.ConnectionAborted:
#if TCLIENT_SOCKET_DEBUG
                            Console.WriteLine("Socket connected Error(Connection Aborted)");
#endif
                            Disconnect();
                            break;
                        case SocketError.ConnectionRefused:
#if TCLIENT_SOCKET_DEBUG
                            Console.WriteLine("Socket connected Error(Connection Refused)");
#endif
                            Disconnect();
                            break;
                        case SocketError.ConnectionReset:
#if TCLIENT_SOCKET_DEBUG
                            Console.WriteLine("Socket connected Error(Connection Reset)");
#endif
                            Disconnect();
                            break;
                        case SocketError.NotConnected:
#if TCLIENT_SOCKET_DEBUG
                            Console.WriteLine("Socket connected Error(Not Connected)");
#endif
                            Disconnect();
                            break;
                        default:
#if TCLIENT_SOCKET_DEBUG
                            Console.WriteLine($"Socket connected Error({ex.SocketErrorCode.ToString()})");
#endif
                            Disconnect();
                            break;
                    }
                }
            }
#pragma warning disable CS0168
            catch (Exception e)
            {
#if TCLIENT_SOCKET_DEBUG
                Console.WriteLine($"Socket connected exception Error({e.ToString()})");
#endif
                Disconnect();
            }
#pragma warning restore CS0168
        }

        public void SendText(string message)
        {
            if (!Connected) return;

            byte[] bytes = Encoding.Default.GetBytes(message);
            Client.Socket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, new AsyncCallback(OnSendData), bytes);
        }
        public void SendBuf(byte[] bytes, int length)
        {
            if (!Connected) return;
            Client.Socket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, new AsyncCallback(OnSendData), bytes);
        }
        private void OnSendData(IAsyncResult iar)
        {
            byte[] bytes = (byte[])iar.AsyncState;
            OnSocketSend?.Invoke(Client.Socket, bytes, bytes.Length);

#if TCLIENT_SOCKET_DEBUG
            Console.WriteLine("Socket Data Send : {0}", Encoding.ASCII.GetString(bytes, 0, bytes.Length));
#endif
        }

        private int FReceiveLength = 0;
        public int ReceiveLength()
        {
            return FReceiveLength;
        }
        public void ReceiveBuf(byte[] bytes, int length)
        {
            Array.Copy(Client.RxBuffer, bytes, length);
        }
        public string ReceiveText()
        {
            return Encoding.ASCII.GetString(Client.RxBuffer, 0, FReceiveLength);
        }

        private void OnReceiveData(IAsyncResult iar)
        {
            try
            {
                TSocketBase remote = (TSocketBase)iar.AsyncState;
                if (remote.Socket.Connected == false) return;

                FReceiveLength = remote.Socket.EndReceive(iar);
                if (FReceiveLength > 0)
                {
                    OnSocketRead?.Invoke(remote.Socket, remote.RxBuffer, FReceiveLength);

#if TCLIENT_SOCKET_DEBUG
                    string sReciveData = Encoding.UTF8.GetString(remote.RxBuffer, 0, FReceiveLength);
                    Console.WriteLine("Socket Data Receive : {0}", sReciveData);
#endif

                    Client.Socket.BeginReceive(Client.RxBuffer, 0, Client.RxBufferSize, SocketFlags.None, new AsyncCallback(OnReceiveData), Client);
                }
                else
                {
#if TCLIENT_SOCKET_DEBUG
                    Console.WriteLine("Socket Data Receive : but length is zero");
#endif

                    Disconnect();
                }
            }
            catch (SocketException exc)
            {
                SocketException ex;
                if (OnSocketError != null) ex = OnSocketError(Client.Socket, exc);
                else ex = exc;

                if (ex.SocketErrorCode != SocketError.Success)
                {
                    switch (ex.SocketErrorCode)
                    {
                        case SocketError.ConnectionAborted:
#if TCLIENT_SOCKET_DEBUG
                            Console.WriteLine("Socket Data Receive Error(ConnectionAborted)");
#endif
                            Disconnect();
                            break;
                        case SocketError.ConnectionRefused:
#if TCLIENT_SOCKET_DEBUG
                            Console.WriteLine("Socket Data Receive Error(ConnectionRefused)");
#endif
                            Disconnect();
                            break;
                        case SocketError.ConnectionReset:
#if TCLIENT_SOCKET_DEBUG
                            Console.WriteLine("Socket Data Receive Error(ConnectionReset)");
#endif
                            Disconnect();
                            break;
                        default:
#if TCLIENT_SOCKET_DEBUG
                            Console.WriteLine($"Socket Data Receive Error({ex.SocketErrorCode.ToString()})");
#endif
                            Disconnect();
                            break;
                    }
                }
            }
#pragma warning disable CS0168
            catch (Exception e)
            {
#if TCLIENT_SOCKET_DEBUG
                Console.WriteLine($"Socket Data Receive exception Error({e.ToString()})");
#endif
                Disconnect();
            }
#pragma warning restore CS0168
        }
    }
}
