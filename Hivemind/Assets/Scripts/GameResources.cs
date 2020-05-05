using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.VR;
using static ResourceNode;

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
    }
}
