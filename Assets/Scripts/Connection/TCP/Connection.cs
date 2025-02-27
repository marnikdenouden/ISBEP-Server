using ISBEP.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace ISBEP.Communication
{
    public class Connection
    {
        [Tooltip("Specify whether debug message for the connection should be displayed in the logs.")]
        public bool DebugMessages = false;
        private static readonly string CONTEXT = "Connection";

        private readonly Socket socket;
        public bool Reading { get; private set; }
        public bool Writing { get; private set; }
        private bool started;
        private bool closed;
        private readonly Thread receiver;
        private readonly Thread sender;
        private readonly BlockingCollection<byte[]> messagesToSend;
        private readonly Action<Socket> endListener;

        /// <summary>
        /// Create a connection with a connected socket
        /// </summary>
        /// <param name="connectedSocket">Socket that is connected.</param>
        /// <param name="autoStart">Truth assignment, if connections should automatically start.</param>
        public Connection(Socket connectedSocket, bool autoStart = true)
        {
            Util.Log(CONTEXT, $"Initializing connection for " +
                $"connected socket {connectedSocket.LocalEndPoint}");
            socket = connectedSocket;
            Reading = true;
            Writing = true;
            started = false;
            closed = false;

            socket.ReceiveBufferSize = 8192;
            //socket.ReceiveTimeout = 16000; // Sixteen second timeout
            socket.SendBufferSize = 8192;
            //socket.SendTimeout = 16000; // Sixteen second timeout

            messagesToSend = new BlockingCollection<byte[]>();

            receiver = new Thread(Receiver);
            sender = new Thread(Sender);

            void endOfFileListener(Socket socket)
            {
                if (socket == this.socket)
                {
                    StopReading();
                    StopWriting();
                }
            }
            endListener = endOfFileListener;
            Message.AddEndOfFileListener(endListener);

            if (autoStart) { Start(); }
        }

        /// <summary>
        /// Start the connection to begin reading and writing data 
        /// </summary>
        public void Start()
        {
            if (DebugMessages) Util.AddDebugContext(CONTEXT);

            if (!started)
            {
                Util.DebugLog(CONTEXT, $"Starting reiver and sender thread");
                receiver.Start();
                sender.Start();
                started = true;
            }
        }

        /// <summary>
        /// Stop the connection from reading on the socket
        /// </summary>
        private void StopReading()
        {
            if (Reading)
            {
                Util.DebugLog(CONTEXT, "Stop reading from the socket");
                Reading = false;
                socket.Shutdown(SocketShutdown.Receive);
            }
        }

        /// <summary>
        /// Stop the connection from writing on the socket
        /// </summary>
        private void StopWriting()
        {
            if (Writing)
            {
                Util.DebugLog(CONTEXT, "Stop writing to the socket");
                Writing = false;
                SendMessage(new byte[0]); // Unblock sender from waiting
                socket.Shutdown(SocketShutdown.Send);
            }
        }

        /// <summary>
        /// Close the connection of the socket to the current server
        /// </summary>
        public void Close()
        {
            if (closed)
            {
                Util.Log(CONTEXT, "Connection has already been closed");
                return;
            }
            Util.Log(CONTEXT, $"Closing the TCP connection with socket {socket.LocalEndPoint}");

            StopWriting();
            Util.DebugLog(CONTEXT, "Waiting for sender thread to join");
            sender.Join();

            Util.DebugLog(CONTEXT, $"Waiting for reading to stop " +
                             $"from EOF signal or 4 second timeout");
            socket.ReceiveTimeout = 4000;
            while (Reading)
            {
                Thread.Sleep(100);
            }

            Util.DebugLog(CONTEXT, "Waiting for receiver thread to join");
            receiver.Join();

            Util.DebugLog(CONTEXT, $"Closing connection at {socket.LocalEndPoint} socket to address {socket.RemoteEndPoint}", true);
            socket.Close();
            Message.RemoveEndOfFileListener(endListener);
            closed = true;
        }

        /// <summary>
        /// Add a message to the connection to send
        /// </summary>
        /// <param name="data">Message to send over the connection</param>
        public void SendMessage(byte[] data)
        {
            Util.DebugLog(CONTEXT, $"Adding message to the queue:");
            Util.DebugLog(CONTEXT, $"\'{Encoding.UTF8.GetString(data, 0, data.Length)[..Mathf.Min(data.Length, 64)]}...\'");
            messagesToSend.Add(data);
        }

        /// <summary>
        /// List of listeners for receiving connection data
        /// </summary>
        private static readonly List<Action<byte[]>> receiveListeners = new List<Action<byte[]>>();

        /// <summary>
        /// Add listener for receiving connection data
        /// </summary>
        /// <param name="listener">Listener to receive data</param>
        public void AddReceiveListener(Action<byte[]> listener)
        {
            Util.DebugLog(CONTEXT, $"Adding End Of File listener");
            receiveListeners.Add(listener);
        }

        /// <summary>
        /// Program to run on a thread, which continously receives messages from the connection
        /// </summary>
        private void Receiver()
        {
            Util.DebugLog(CONTEXT, $"Start of receiver thread");

            while (Reading)
            {
                byte[] receivedData = Receive();
                Util.DebugLog(CONTEXT, $"Received message from the connection");

                if (receivedData == null) break;

                foreach (Action<byte[]> listener in receiveListeners)
                {
                    listener(receivedData);
                }
            }
            Util.DebugLog(CONTEXT, "Reached end of receiver thread");
        }

        /// <summary>
        /// Program to run on a thread, which writes all messages to the connection
        /// </summary>
        private void Sender()
        {
                Util.DebugLog(CONTEXT, $"Start of sender thread");

            while (true)
            {
                byte[] message = messagesToSend.Take();
                Util.DebugLog(CONTEXT, $"Got new data to send from queue");

                if (!Writing) { break; } // End sender thread as we should stop writing

                Util.Log(CONTEXT, "Sending message:");
                Util.Log("", $"\'{Encoding.UTF8.GetString(message, 0, message.Length)[..Mathf.Min(message.Length, 64)]}...\'");
                Send(message);
            }
            Util.DebugLog(CONTEXT, "Reached end of sender thread");
        }

        /// <summary>
        /// Wait to receive a message from the socket connection
        /// </summary>
        /// <returns>Data received from the socket connection</returns>
        private byte[] Receive()
        {
            try
            {
                Util.DebugLog(CONTEXT, $"Waiting to receive message");
                return Message.Receive(socket);
            }
            catch (TimeoutException)
            {
                Util.DebugLog(CONTEXT, $"TimeOutError trying to receive message " +
                         $"at socket {socket.LocalEndPoint}");
                StopReading();
                return null;
            }
            catch (Exception exception)
            {
                Util.DebugLog(CONTEXT, $"Exception occurred trying to receive message " +
                         $"at socket {socket.LocalEndPoint}");
                Util.DebugLog(CONTEXT, $"{exception}", true, "Exception");
                StopReading();
                return null;
            }
        }

        /// <summary>
        /// Send a message to the socket connection
        /// </summary>
        /// <param name="data">Data to send over the connection</param>
        private void Send(byte[] data)
        {
            try
            {
                Util.DebugLog(CONTEXT, $"Sending message");
                new Message(data).Send(socket);
            }
            catch (TimeoutException)
            {
                Util.DebugLog(CONTEXT, $"TimeOutError trying to send message " +
                         $"at socket {socket.LocalEndPoint}");
                StopWriting();
            }
            catch (Exception exception)
            {
                Util.DebugLog(CONTEXT, $"Exception occurred trying to send message " +
                         $"at socket {socket.LocalEndPoint}");
                Util.DebugLog(CONTEXT, $"{exception}", true, "Exception");
                StopWriting();
            }
        }
    }
}
