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
        const int PORT_NO = 5000;
        const string SERVER_IP = "127.0.0.1";
        public static bool ActiveServer { get; private set; } = true;
        private TCPServerSettings ServerSettings;
        private Thread ServerThread;
        public static event EventHandler<Connection> AcceptedClientConnectionHandler;
        private readonly List<Connection> Connections = new List<Connection>();

        void Start()
        {
            if (DebugMessages) Util.AddDebugContext("TCP Server");
            StartTCPServer();
        }

        private void OnDisable()
        {
            EndTCPServer();
        }

        public void EndTCPServer()
        {
            Util.DebugLog("TCP Server", "Ending TCP Server");
            CloseAllConnections();
            ActiveServer = false;
            Util.Log("TCP Server", $"Closing server socket {ServerSettings.Socket.LocalEndPoint}");
            // Connect to TCP server to get out of blocking socket accept call.
            Socket socket = new Socket(IPAddress.Parse(SERVER_IP).AddressFamily,
                       SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ServerSettings.IPEndPoint);
            ServerThread.Join();
            socket.Close();
            ServerSettings.Socket.Close();
            Util.DebugLog("TCP Server", $"Finished cleaning up server");
        }

        public void StartTCPServer()
        {
            Util.Log("TCP Server", $"Starting TCP Server on " +
                $"{SERVER_IP}:{PORT_NO}");
            IPAddress localAdd = IPAddress.Parse(SERVER_IP);

            // Wait to get ip address from a host name
            // IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            // IPAddress ipAddr = ipHost.AddressList[0];

            IPEndPoint localEndPoint = new IPEndPoint(localAdd, PORT_NO);

            Socket socket = new Socket(localAdd.AddressFamily,
                       SocketType.Stream, ProtocolType.Tcp);

            ServerSettings = new TCPServerSettings() { 
                IPEndPoint = localEndPoint, Socket = socket 
            };
            Connections.Clear();

            Util.Log("TCP Server", $"Starting TCP Server thread");
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
                Util.DebugLog("TCP Server", $"Binding server socket to " +
                    $"{IPEndPoint.Address}:{IPEndPoint.Port}");
                socket.Bind(IPEndPoint);

                socket.Listen(10);

                while (ActiveServer)
                {
                    Util.Log("TCP Server", $"Waiting for client to connect...");
                    Socket clientSocket = socket.Accept();

                    CloseFinishedConnections();

                    if (ActiveServer == false) {
                        clientSocket.Shutdown(SocketShutdown.Both);
                        clientSocket.Close();
                        break; 
                    }

                    Util.Log("TCP Server", $"Accepted client socket {clientSocket.LocalEndPoint}");

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
                    Util.DebugLog("Stacktrace", $"{exception.StackTrace}", true);
                }
                else
                {
                    Debug.LogWarning(exception.ToString());
                    Util.DebugLog("Stacktrace", $"{exception.StackTrace}", true);
                }
            }
            Util.DebugLog("TCP Server", $"Reached end of TCP server thread");
        }

        private void CloseFinishedConnections()
        {
            List<Connection> closed_connection = new List<Connection>();
            foreach (Connection open_connection in Connections)
            {
                if (!open_connection.Writing && !open_connection.Reading)
                {
                    Util.DebugLog("TCP Server", $"Closing connection that stopped writing and reading");
                    open_connection.Close();
                    closed_connection.Add(open_connection);
                }
            }
            closed_connection.ForEach((connection) => { Connections.Remove(connection); });
        }

        private void CloseAllConnections()
        {
            Util.DebugLog("TCP Server", $"Closing all remaining connections");
            foreach (Connection connection in Connections)
            {
                connection.Close();
            }
            Connections.Clear();
        }
    }

}
