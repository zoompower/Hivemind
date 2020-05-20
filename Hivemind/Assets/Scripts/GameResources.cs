using System;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public static class GameResources
    {
        public static event EventHandler OnResourceAmountChanged;

        private static Dictionary<ResourceType, int> resourceAmounts = new Dictionary<ResourceType, int>();

        public static void AddResources(Dictionary<ResourceType, int> resources)
        {
            foreach (ResourceType resourceType in resources.Keys)
            {
                resourceAmounts[resourceType] += resources[resourceType];
            }
            OnResourceAmountChanged.Invoke(null, EventArgs.Empty);
        }

        public static int GetResourceAmount(ResourceType resourceType)
        {
            if (!resourceAmounts.ContainsKey(resourceType))
            {
                resourceAmounts.Add(resourceType, 0);
            }
            return resourceAmounts[resourceType];
        }

        public static Dictionary<ResourceType, int> GetResourceAmounts()
        {
            return resourceAmounts;
        }

        public static void SetResourceAmounts(List<ResourceType> keyList, List<int> valueList)
        {
            for (int i = 0; i < keyList.Count; i++)
            {
                resourceAmounts[keyList[i]] = valueList[i];
            }
            OnResourceAmountChanged.Invoke(null, EventArgs.Empty);
        }
    }
}