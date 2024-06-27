using ISBEP.Utility;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using System.Threading;

namespace ISBEP.Communication
{
    class TCPServer : MonoBehaviour
    {
        [Tooltip("Specify whether debug message for the TCP server should be displayed in the logs.")]
        public bool DebugMessages = false;
        private readonly string CONTEXT = "TCP Server";

        [Tooltip("Port to bind TCP server to")]
        public int PortNumber = 5000;

        [Tooltip("Ip to bind TCP server to")]
        public string ServerIp = "127.0.0.1";
        public static bool ActiveServer { get; private set; } = true;
        private TCPServerSettings ServerSettings;
        private Thread ServerThread;
        public static event EventHandler<Connection> AcceptedClientConnectionHandler;
        private readonly List<Connection> Connections = new List<Connection>();

        void Start()
        {
            if (DebugMessages) Util.AddDebugContext(CONTEXT);
            StartTCPServer();
        }

        private void OnDisable()
        {
            EndTCPServer();
        }

        public void EndTCPServer()
        {
            Util.DebugLog(CONTEXT, "Ending TCP Server");
            CloseAllConnections();
            ActiveServer = false;
            Util.Log(CONTEXT, $"Closing server socket {ServerSettings.Socket.LocalEndPoint}");
            // Connect to TCP server to get out of blocking socket accept call.
            Socket socket = new Socket(IPAddress.Parse(ServerIp).AddressFamily,
                       SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ServerSettings.IPEndPoint);
            ServerThread.Join();
            socket.Close();
            // Allow the socket to linger for at most 8 seconds after closing.
            ServerSettings.Socket.LingerState = new LingerOption(true, 8);
            // Set the server socket to close after at most 8 seconds.
            ServerSettings.Socket.Close(timeout:8);
            Util.DebugLog(CONTEXT, $"Finished cleaning up server");
        }

        public void StartTCPServer()
        {
            Util.Log(CONTEXT, $"Starting TCP Server on " +
                $"{ServerIp}:{PortNumber}");
            IPAddress localAdd = IPAddress.Parse(ServerIp);

            // Wait to get ip address from a host name
            // IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            // IPAddress ipAddr = ipHost.AddressList[0];

            IPEndPoint localEndPoint = new IPEndPoint(localAdd, PortNumber);

            Socket socket = new Socket(localAdd.AddressFamily,
                       SocketType.Stream, ProtocolType.Tcp);

            ServerSettings = new TCPServerSettings() { 
                IPEndPoint = localEndPoint, Socket = socket 
            };
            Connections.Clear();

            Util.Log(CONTEXT, $"Starting TCP Server thread");
            ServerThread = new Thread(TCPServerThread);
            ServerThread.Start();
        }

        struct TCPServerSettings
        {
            public IPEndPoint IPEndPoint { get; set; }
            public Socket Socket { get; set; }
        }

        private void TCPServerThread()
        {
            Socket socket = ServerSettings.Socket;
            IPEndPoint IPEndPoint = ServerSettings.IPEndPoint;

            try
            {
                Util.DebugLog(CONTEXT, $"Binding server socket to " +
                    $"{IPEndPoint.Address}:{IPEndPoint.Port}");
                socket.Bind(IPEndPoint);

                socket.Listen(10);

                while (ActiveServer)
                {
                    Util.Log(CONTEXT, $"Waiting for client to connect...");
                    Socket clientSocket = socket.Accept();

                    CloseFinishedConnections();

                    if (ActiveServer == false) {
                        clientSocket.Shutdown(SocketShutdown.Both);
                        clientSocket.Close();
                        break; 
                    }

                    Util.Log(CONTEXT, $"Accepted client socket {clientSocket.LocalEndPoint}");

                    Connection connection = new Connection(clientSocket, false);

                    AcceptedClientConnectionHandler?.Invoke(this, connection);
                    Connections.Add(connection);

                    connection.Start();
                }
            }
            catch (Exception exception)
            {
                if (ActiveServer)
                {
                    Debug.LogError(exception.ToString());
                    Util.DebugLog(CONTEXT, $"{exception.StackTrace}", true, "Stacktrace");
                }
                else
                {
                    Debug.LogWarning(exception.ToString());
                    Util.DebugLog(CONTEXT, $"{exception.StackTrace}", true, "Stacktrace");
                }
            }
            Util.DebugLog(CONTEXT, $"Reached end of TCP server thread");
        }

        private void CloseFinishedConnections()
        {
            List<Connection> closed_connection = new List<Connection>();
            foreach (Connection open_connection in Connections)
            {
                if (!open_connection.Writing && !open_connection.Reading)
                {
                    Util.DebugLog(CONTEXT, $"Closing connection that stopped writing and reading");
                    open_connection.Close();
                    closed_connection.Add(open_connection);
                }
            }
            closed_connection.ForEach((connection) => { Connections.Remove(connection); });
        }

        private void CloseAllConnections()
        {
            Util.DebugLog(CONTEXT, $"Closing all remaining connections");
            foreach (Connection connection in Connections)
            {
                connection.Close();
            }
            Connections.Clear();
        }
    }

}
