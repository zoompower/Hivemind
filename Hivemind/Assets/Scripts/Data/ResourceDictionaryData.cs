using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Data
{
    [Serializable]
    public class ResourceDictionaryData
    {
        public List<ResourceType> ResourceAmountsKeys;
        public List<int> ResourceAmountsValues;

        public ResourceDictionaryData(Dictionary<ResourceType, int> keyValuePairs)
        {
            ResourceAmountsKeys = keyValuePairs.Keys.ToList();
            ResourceAmountsValues = keyValuePairs.Values.ToList();
        }
    }
}
