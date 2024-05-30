using ISBEP.Situation;
using ISBEP.Utility;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.Text;
using Newtonsoft.Json;
using System.Linq;

namespace ISBEP.Communication
{   
    public class Communicator : MonoBehaviour
    {
        private static SynchronizationContext mainThreadContext;
        public Situation.Situation situation;

        // Start is called before the first frame update
        void Start()
        {
            mainThreadContext = SynchronizationContext.Current;
            TCPServer.AcceptedClientConnectionHandler += Server_AcceptedClientConnection;
        }

        private void Server_AcceptedClientConnection(object sender, Connection connection)
        {
            Util.DebugLog("Communicator", "Setting up new accepted client connection");
            TCPServer server = (TCPServer)sender;
            static void PrintReceivedListener(byte[] receivedData)
            {
                string message = Encoding.UTF8.GetString(receivedData,
                                            0, receivedData.Length);
                Util.Log("Receiver", $"Received message:");
                Util.Log("", $"\'{message}\'");
            }
            connection.AddReceiveListener(PrintReceivedListener);

            connection.SendMessage(Encoding.UTF8.GetBytes("Welcome to the server"));

            void UpdateRobotData(byte[] receivedData)
            {
                string receivedJsonData = Encoding.UTF8.GetString(receivedData);
                mainThreadContext.Post(_ => {
                    RobotData receivedRobotData;
                    try
                    {
                        receivedRobotData = JsonConvert.DeserializeObject<RobotData>(receivedJsonData);
                    } catch
                    {
                        Util.DebugLog("Communicator", "Received not valid robot data");
                        return;
                    }

                    List<ISituationData> elementList = situation.GetSituationDataElements("robot");

                    IEnumerable<ISituationData> matchingRobotList = elementList.Where((element) => {
                        Util.DebugLog("Communicator", $"element : {element}");
                        Util.DebugLog("Communicator", $"element.JsonData : {element.JsonData}");
                        Util.DebugLog("Communicator", $"receivedRobotData.serialNumber : {receivedRobotData.serialNumber}");
                        return JsonConvert.DeserializeObject<RobotData>(element.JsonData).serialNumber == receivedRobotData.serialNumber;
                    });
                    foreach (var matchingRobot in matchingRobotList)
                    {
                        Util.DebugLog("Communicator", "Updating robot data with received data");
                        matchingRobot.JsonData = receivedJsonData;
                    }
                }, null);
            }

            connection.AddReceiveListener(UpdateRobotData);
        }


        void OnDisable()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
