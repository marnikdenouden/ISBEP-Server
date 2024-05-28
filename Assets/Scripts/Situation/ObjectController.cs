using ISBEP.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ISBEP.Situation
{
    class ObjectController : SituationController<ObjectData>
    {
        protected override void Start()
        {
            if (!TryGetComponent(out MeshRenderer meshRenderer))
            {
                Util.DebugLog("Object", "Could not get mesh render for object color control");
            }
            Element = new ObjectElement(gameObject, meshRenderer);
            base.Start();
        }
    }

    [Serializable]
    public struct ObjectData
    {
        public Dictionary<string, float> position;
        public Dictionary<string, float> scale;
        public Dictionary<string, float> color;
    }

    public class ObjectElement : SituationElement<ObjectData>
    {
        private readonly GameObject gameObject;
        private readonly MeshRenderer meshRenderer;

        public ObjectElement(GameObject gameObject, MeshRenderer meshRenderer)
        {
            this.gameObject = gameObject;
            this.meshRenderer = meshRenderer;
        }

        protected override ObjectData Data
        {
            get
            {
                return new ObjectData()
                {
                    position = Position,
                    scale = Scale,
                    color = Color
                };
            }

            set
            {
                ObjectData data = value;

                Util.DebugLog("Object", "Setting data values of object");
                Position = data.position;
                Scale = data.scale;
                Color = data.color;
            }
        }

        private Dictionary<string, float> Position
        {
            get
            {
                Vector3 vector3 = gameObject.transform.position;
                return new Dictionary<string, float>
            {
                { "x", vector3.x },
                { "y", vector3.y },
                { "z", vector3.z }
            };
            }

            set
            {
                Dictionary<string, float> vector = value;
                gameObject.transform.position = new Vector3(vector["x"], vector["y"], vector["z"]);
                Util.DebugLog("Object", $"Updated position to {gameObject.transform.position}");
            }
        }

        private Dictionary<string, float> Scale
        {
            get
            {
                Vector3 vector3 = gameObject.transform.localScale;
                return new Dictionary<string, float>
            {
                { "x", vector3.x },
                { "y", vector3.y },
                { "z", vector3.z }
            };
            }

            set
            {
                Dictionary<string, float> vector = value;
                gameObject.transform.localScale = new Vector3(vector["x"], vector["y"], vector["z"]);
                Util.DebugLog("Object", $"Updated rotation to {gameObject.transform.localScale}");
            }
        }

        private Dictionary<string, float> Color
        {
            get
            {
                Color color = meshRenderer.material.color;
                return new Dictionary<string, float>
            {
                { "red", color.r },
                { "green", color.g },
                { "blue", color.b }
            };
            }

            set
            {
                Color color = new Color(value["red"], value["green"], value["blue"]);
                meshRenderer.material.color = color;
                Util.DebugLog("Object", $"Updated color to {meshRenderer.material.color}");
            }
        }
    }
}
