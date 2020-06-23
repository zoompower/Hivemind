using Assets.Scripts.Data;
using System;
using System.Collections.Generic;

public class GameResources
{
    public event EventHandler OnResourceAmountChanged;

    private Dictionary<ResourceType, int> resourceAmounts = new Dictionary<ResourceType, int>();

    public void AddResources(Dictionary<ResourceType, int> resources)
    {
        foreach (ResourceType resourceType in resources.Keys)
        {
            if (resourceAmounts.ContainsKey(resourceType))
            {
                resourceAmounts[resourceType] += resources[resourceType];
            }
            else
            {
                resourceAmounts.Add(resourceType, resources[resourceType]);
            }
        }
        SendOnResourceAmountChangedEvent();
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
        SendOnResourceAmountChangedEvent();
    }

    public ResourceDictionaryData GetData()
    {
        return new ResourceDictionaryData(resourceAmounts);
    }

    public void SetData(ResourceDictionaryData data)
    {
        SetResourceAmounts(data.ResourceAmountsKeys, data.ResourceAmountsValues);
    }

    internal void SubtractResources(Dictionary<ResourceType, int> resourceList)
    {
        foreach (var item in resourceList)
        {
            if (resourceAmounts.ContainsKey(item.Key))
            {
                resourceAmounts[item.Key] -= item.Value;
            }
        }
        SendOnResourceAmountChangedEvent();
    }

    private void SendOnResourceAmountChangedEvent()
    {
        if (OnResourceAmountChanged != null)
            OnResourceAmountChanged.Invoke(null, EventArgs.Empty);
    }

    public static Dictionary<ResourceType, int> GetToolCost(BaseBuildingTool tool)
    {
        Dictionary<ResourceType, int> totalCost = new Dictionary<ResourceType, int>();
        switch (tool)
        {
            case BaseBuildingTool.Default:
                break;

            case BaseBuildingTool.DestroyRoom:
                break;

            case BaseBuildingTool.Wall:
                break;

            case BaseBuildingTool.AntRoom:
                totalCost = new Dictionary<ResourceType, int>() { { ResourceType.Rock, 10 } };
                break;

            default:
                break;
        }

        return totalCost;
    }

    public static bool EnoughResources(Dictionary<ResourceType, int> costobj, GameResources resources)
    {
        foreach (var costItem in costobj)
        {
            if (resources.GetResourceAmount(costItem.Key) < costItem.Value)
            {
                return false;
            }
        }
        return true;
    }
}