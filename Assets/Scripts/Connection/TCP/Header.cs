using ISBEP.Utility;
using System;
using System.Linq;

namespace ISBEP.Communication
{
    public class Header
    {
        /// <summary> Specifies the header version to check header data for.</summary>
        public static readonly byte VERSION = 0;
        /// <summary> Describes number of bytes in the message </summary>
        public readonly int messageSize;

        /// <summary>
        /// Constructor for header using header data to extract values.
        /// </summary>
        /// <param name="headerData">Data to extract header values from.</param>
        public Header(byte[] headerData)
        {
            messageSize = 0;

            if (headerData.Length != GetSize())
            {
                Util.DebugLog($"Header v{VERSION}", $"Header data has {headerData.Length}" +
                $" amount of bytes, while this version expects {GetSize()}");
                return;
            }

            if (headerData[0] != VERSION)
            {
                Util.DebugLog($"Header v{VERSION}", $"Header version {headerData[0]}" +
                    $" does not match this header version {VERSION}");
                return;
            }

            // Reverse the order of the bytes to convert the big endian value to little endian.
            byte[] littleEndianMessageSizeData = headerData.Skip(1).Take(4).Reverse().ToArray();
            messageSize = (int)BitConverter.ToUInt32(littleEndianMessageSizeData);
            Util.DebugLog($"Header v{VERSION}", $"Header received message size {messageSize}");
        }

        /// <summary>
        /// Get the header bytes
        /// </summary>
        /// <param name="message_data">Data to create header for.</param>
        /// <returns>Byte array filled with header data</returns>
        public static byte[] Create(byte[] message_data)
        {
            byte[] header = new byte[GetSize()];

            header[0] = VERSION;

            // Take the unsigned 32 bit int and reverse it to get big endian messageSizeBytes.
            byte[] messageSizeBytes = BitConverter.GetBytes(((UInt32)message_data.Length)).Reverse().ToArray();
            Buffer.BlockCopy(messageSizeBytes, 0, header, 1, 4);

            Util.DebugLog($"Header v{VERSION}", "Created header data:");
            Util.DebugLog("", $"[{string.Join("|", header)}]");
            return header;
        }

        /// <summary>
        /// Get the size of the header in number of bytes.
        /// </summary>
        /// <returns>Size of the header in number of bytes.</returns>
        public static int GetSize()
        {
            return 5;
        }
    }
}
