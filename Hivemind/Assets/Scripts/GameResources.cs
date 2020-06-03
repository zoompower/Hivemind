using System;
using System.Collections.Generic;

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