using System.Collections.Generic;
using UnityEngine;

namespace ISBEP.Situation
{
    [RequireComponent(typeof(SituationStorage))]
    public class Situation : MonoBehaviour
    {
        private readonly Dictionary<string, List<ISituationData>> SituationData = new();

        void Awake()
        {
            SituationData.Clear();
        }

        public List<ISituationData> GetSituationDataElements(string typeLabel)
        {
            if (!SituationData.ContainsKey(typeLabel)) return new List<ISituationData> { };

            return SituationData[typeLabel];
        }

        public void AddSituationController<DataType>(SituationController<DataType> controller)
        {
            string key = controller.TypeLabel;
            
            if (!SituationData.ContainsKey(key))
            {
                SituationData.Add(key, new List<ISituationData>());
            }
            SituationData[key].Add(controller.Element);
        }

        public void SaveSituation()
        {
            GetComponent<SituationStorage>().SaveSituation(SituationData);
        }
    }
}
