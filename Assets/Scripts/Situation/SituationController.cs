using UnityEngine;

namespace ISBEP.Situation {
    public abstract class SituationController<DataType> : MonoBehaviour
    {
        [Tooltip("Type label to categorize situation elements with.")]
        public string TypeLabel;

        [HideInInspector]
        public SituationElement<DataType> Element { get; protected set; }

        protected virtual void Start()
        {
            Situation[] situations = GetComponentsInParent<Situation>();
            foreach (Situation situation  in situations)
            {
                situation.AddSituationController(this);
            }
        }
    }
}
