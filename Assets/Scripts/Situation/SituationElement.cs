using Newtonsoft.Json;
using System;

namespace ISBEP.Situation
{
    public interface ISituationData
    {
        string JsonData { get; set; }
        Type DataStruct { get; }
    }

    public abstract class SituationElement<DataType> : ISituationData
    {
        protected abstract DataType Data { get; set; }
        public Type DataStruct { get; } = typeof(DataType);

        public string JsonData
        {
            get
            {
                return JsonConvert.SerializeObject(Data);
            }

            set
            {
                Data = JsonConvert.DeserializeObject<DataType>(value);
            }
        }
    }
}
