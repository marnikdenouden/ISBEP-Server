using ISBEP.Utility;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace ISBEP.Communication
{
    public class Message
    {
        private readonly byte[] data;
        /// <summary>
        /// Create a message with specified data payload
        /// </summary>
        /// <param name="data">Message data that can be send</param>
        public Message(byte[] data)
        {
            this.data = data;
        }

        /// <summary>
        /// Send the create message to the specified socket
        /// </summary>
        /// <param name="socket">Socket to send the message to</param>
        public void Send(Socket socket)
        {
            Util.DebugLog("Message", $"Sending message header");
            socket.Send(Header.Create(data));

            Util.DebugLog("Message", $"Sending message data");
            socket.Send(data);
        }

        /// <summary>
        /// Wait to receive a message from the specified socket
        /// </summary>
        /// <param name="socket">Socket to receive message from</param>
        /// <returns></returns>
        public static byte[] Receive(Socket socket)
        {
            byte[] headerData = ReceiveSizedMessage(socket, Header.GetSize());

            if (headerData == null) { return null; }

            Util.DebugLog("Message", $"Received message header");
            byte[] messageData = ReceiveSizedMessage(socket, new Header(headerData).messageSize);

            if (messageData == null) { return null; }

            Util.DebugLog("Message", $"Received message data");
            return messageData;
        }

        /// <summary>
        /// Receive a message of specified size from the socket
        /// </summary>
        /// <param name="socket">Socket to receive message from</param>
        /// <param name="size">Size of the message to receive</param>
        /// <returns>Message received from the socket, zero size represents EOF signal</returns>
        public static byte[] ReceiveSizedMessage(Socket socket, int size)
        {
            byte[] dataReceived = new byte[size];
            int totalBytesReceived = 0;

            // Continue to read data until we got the specified amount
            while (totalBytesReceived < size)
            {
                int bytesReceived = socket.Receive(dataReceived, totalBytesReceived,
                    size - totalBytesReceived, SocketFlags.None);

                // Check if data was received
                if (bytesReceived == 0)
                {
                    Util.DebugLog("Message", $"Received End Of File signal");
                    // No data received is End Of File signal, so we alert listeners
                    EndOfFileHandler(socket);
                    return null;
                }
                //Add data to currently received data buffer
                totalBytesReceived += bytesReceived;
            }
            return dataReceived;
        }

        private static readonly List<Action<Socket>> endOfFileListeners = new List<Action<Socket>>();

        public static void AddEndOfFileListener(Action<Socket> listener)
        {
            Util.DebugLog("Message", $"Adding End Of File listener");
            endOfFileListeners.Add(listener);
        }

        public static void RemoveEndOfFileListener(Action<Socket> listener)
        {
            Util.DebugLog("Message", $"Removing End Of File listener");
            endOfFileListeners.Remove(listener);
        }

        public static void EndOfFileHandler(Socket socket)
        {
            foreach (Action<Socket> listener in endOfFileListeners)
            {
                listener(socket);
            }
        }
    }
}
