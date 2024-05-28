using ISBEP.Utility;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ISBEP.Situation
{
    public class RobotControler : SituationController<RobotData>
    {
        [Tooltip("")]
        public string SerialNumber = "00000000";

        protected override void Start()
        {
            Element = new RobotElement(gameObject, SerialNumber);
            base.Start();
        }
    }

    [Serializable]
    public struct RobotData
    {
        public string serialNumber;
        public Dictionary<string, float> position;
        public Dictionary<string, float> rotation;
    }

    public class RobotElement : SituationElement<RobotData>
    {
        private readonly GameObject gameObject;
        private readonly string serialNumber;

        public RobotElement(GameObject gameObject, string serialNumber) 
        {
            this.gameObject = gameObject;
            this.serialNumber = serialNumber;
        }

        private Dictionary<string, float> Position
        {
            get
            {
                return CreateVector(gameObject.transform.position);
            }

            set
            {
                Dictionary<string, float> vector = value;
                gameObject.transform.position = ReadVector(vector);
                Util.DebugLog("Robot", $"Updated position to {gameObject.transform.position}");
            }
        }
        private Dictionary<string, float> Rotation
        {
            get
            {
                Vector3 vector3 = gameObject.transform.rotation.eulerAngles;
                return new Dictionary<string, float>
            {
                { "pitch", vector3.x },
                { "yaw", vector3.y },
                { "roll", vector3.z }
            };
            }

            set
            {
                Dictionary<string, float> vector = value;
                Vector3 rotationVector = new Vector3(vector["pitch"], vector["yaw"], vector["roll"]);
                gameObject.transform.rotation = Quaternion.Euler(rotationVector);
                Util.DebugLog("Object", $"Updated rotation to {gameObject.transform.rotation.eulerAngles}");
            }
        }

        public Dictionary<string, float> CreateVector(Vector3 vector3)
        {
            Dictionary<string, float> vector = new Dictionary<string, float>
        {
            { "x", vector3.x },
            { "y", vector3.x },
            { "z", vector3.x }
        };
            return vector;
        }

        public Vector3 ReadVector(Dictionary<string, float> vector)
        {
            return new Vector3(vector["x"], vector["y"], vector["z"]);
        }

        public struct Vector
        {
            public float x;
            public float y;
            public float z;
        }

        protected override RobotData Data
        {
            get
            {
                return new RobotData()
                {
                    serialNumber = serialNumber,
                    position = Position,
                    rotation = Rotation
                };
            }

            set
            {
                RobotData data = value;
                if (data.serialNumber != serialNumber)
                {
                    Debug.LogWarning("Serial number of robot data does not match for this robot.");
                    return;
                }

                Util.DebugLog("Robot", "Setting position of robot");
                Position = data.position;
                Util.DebugLog("Robot", "Setting rotation of robot");
                Rotation = data.rotation;
            }
        }
    }
}
