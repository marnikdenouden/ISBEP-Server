using ISBEP.Situation;
using ISBEP.Utility;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.Text;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine.Events;

namespace ISBEP.Communication
{   
    public class Communicator : MonoBehaviour
    {
        [Tooltip("Specify whether debug message for the communicator should be displayed in the logs.")]
        public bool DebugMessages = false;
        private readonly string CONTEXT = "Communicator";

        private static SynchronizationContext mainThreadContext;
        public Situation.Situation situation;

        public UnityEvent<string> OnReceiveData;

        void Start()
        {
            if (DebugMessages) Util.AddDebugContext(CONTEXT);

            mainThreadContext = SynchronizationContext.Current;
            TCPServer.AcceptedClientConnectionHandler += Server_AcceptedClientConnection;
        }

        private void OnDisable()
        {
            OnReceiveData.RemoveAllListeners();
            TCPServer.AcceptedClientConnectionHandler += Server_AcceptedClientConnection;

            OnReceiveData.AddListener(RelayReceivedDataListener);
        }

        private void Server_AcceptedClientConnection(object sender, Connection connection)
        {
            Util.DebugLog(CONTEXT, "Setting up new accepted client connection");
            TCPServer server = (TCPServer)sender;


            connection.SendMessage(Encoding.UTF8.GetBytes("Welcome to the server"));


            connection.AddReceiveListener((receivedData) =>
            {
                mainThreadContext.Post(_ =>
                {
                    OnReceiveData?.Invoke(
                    Encoding.UTF8.GetString(receivedData, 0, receivedData.Length));
                }, null);
            });

        }

        public static void PrintReceivedListener(string receivedData)
        {
            Util.Log("Receiver", $"Received message:");
            Util.Log("", $"\'{receivedData[..Mathf.Min(64, receivedData.Length)]}...\'");
        }

        public void RelayReceivedDataListener(string receivedData)
        {
            WebSocketServer.Instance.BroadcastWebSocketMessage(receivedData);
        }

        public void UpdateRobotData(string receivedData)
        {
            mainThreadContext.Post(_ => {
                RobotData receivedRobotData;
                try
                {
                    receivedRobotData = JsonConvert.DeserializeObject<RobotData>(receivedData);
                }
                catch
                {
                    Util.DebugLog(CONTEXT, "Received not valid robot data");
                    return;
                }

                List<ISituationData> elementList = situation.GetSituationDataElements("robot");

                IEnumerable<ISituationData> matchingRobotList = elementList.Where((element) => {
                    Util.DebugLog(CONTEXT, $"element : {element}");
                    Util.DebugLog(CONTEXT, $"element.JsonData : {element.JsonData}");
                    Util.DebugLog(CONTEXT, $"receivedRobotData.serialNumber : {receivedRobotData.serial}");
                    return JsonConvert.DeserializeObject<RobotData>(element.JsonData).serial == receivedRobotData.serial;
                });
                foreach (var matchingRobot in matchingRobotList)
                {
                    Util.DebugLog(CONTEXT, "Updating robot data with received data");
                    matchingRobot.JsonData = receivedData;

                    // Relay the robot data, after the data has been set to have additional values.
                    WebSocketServer.Instance.BroadcastWebSocketMessage(matchingRobot.JsonData);
                }
            }, null);
        }
    }
}
