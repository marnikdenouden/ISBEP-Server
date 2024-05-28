using UnityEngine;

namespace ISBEP.Situation
{
    public class ColorModifier : MonoBehaviour
    {
        [Tooltip("Set the color for the mesh renderer of this game object.")]
        public Color color;
        void Start()
        {
            MeshRenderer renderer = GetComponent<MeshRenderer>();
            renderer.material.color = color;
        }
    }
}
