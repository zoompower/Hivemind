using Assets.Scripts.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class GameResources
    {
        public event EventHandler OnResourceAmountChanged;

        private Dictionary<ResourceType, int> resourceAmounts = new Dictionary<ResourceType, int>();

        public void AddResources(Dictionary<ResourceType, int> resources)
        {
            foreach (ResourceType resourceType in resources.Keys)
            {
                resourceAmounts[resourceType] += resources[resourceType];
            }
            OnResourceAmountChanged.Invoke(null, EventArgs.Empty);
        }

        public int GetResourceAmount(ResourceType resourceType)
        {
            if (!resourceAmounts.ContainsKey(resourceType))
            {
                resourceAmounts.Add(resourceType, 0);
            }
            return resourceAmounts[resourceType];
        }

        public Dictionary<ResourceType, int> GetResourceAmounts()
        {
            return resourceAmounts;
        }

        public void SetResourceAmounts(List<ResourceType> keyList, List<int> valueList)
        {
            for (int i = 0; i < keyList.Count; i++)
            {
                resourceAmounts[keyList[i]] = valueList[i];
            }
            OnResourceAmountChanged.Invoke(null, EventArgs.Empty);
        }

        public ResourceDictionaryData GetData()
        {
            return new ResourceDictionaryData(resourceAmounts);
        }

        public void SetData(ResourceDictionaryData data)
        {
            SetResourceAmounts(data.ResourceAmountsKeys, data.ResourceAmountsValues);
        }
    }
}