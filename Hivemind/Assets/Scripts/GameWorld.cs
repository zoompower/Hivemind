using Assets.Scripts;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public static class GameWorld
{
    public static List<ResourceNode> ResourceList = new List<ResourceNode>();
    private static Storage storage = null;

    public static ResourceNode FindNearestUnknownResource(Vector3 antPosition)
    {
        ResourceNode closest = null;
        float minDistance = float.MaxValue;
        foreach (ResourceNode resource in ResourceList)
        {
            if (!resource.IsKnown)
            {
                float dist = Vector3.Distance(antPosition, resource.GetPosition());
                if (dist < minDistance)
                {
                    closest = resource;
                    minDistance = dist;
                }
            }
        }
        return closest;
    }

    public static ResourceNode FindNearestKnownResource(Vector3 antPosition, ResourceType prefType)
    {
        ResourceNode closest = null;
        float minDistance = float.MaxValue;
        foreach (ResourceNode resource in ResourceList)
        {
            if (resource.IsKnown && prefType == ResourceType.Unknown || resource.resourceType == prefType)
            {
                if (resource.GetResourcesFuture() > 0)
                {
                    float dist = Vector3.Distance(antPosition, resource.GetPosition());
                    if (dist < minDistance)
                    {
                        closest = resource;
                        minDistance = dist;
                    }
                }
            }
        }
        return closest;
    }

    public static Storage GetStorage()
    {
        return storage;
    }

    public static void RemoveResource(ResourceNode resource)
    {
        ResourceList.Remove(resource);
    }

    public static void SetStorage(Storage Storage)
    {
        storage = Storage;
    }

    public static void AddNewResource(ResourceNode resource)
    {
        ResourceList.Add(resource);
    }

    public static void Save()
    {
        SaveObject saveObject = new SaveObject
        {
            ResourceAmountsKeys = GameResources.GetResourceAmounts().Keys.ToList(),
            ResourceAmountsValues = GameResources.GetResourceAmounts().Values.ToList(),
            Resources = ResourceList
        };
        string json = saveObject.ToJson();
        Debug.Log(json);
        File.WriteAllText(Application.dataPath + "/save.txt", json);
    }

    public static void Load()
    {
        if (File.Exists(Application.dataPath + "/save.txt"))
        {
            string saveString = File.ReadAllText(Application.dataPath + "/save.txt");
            SaveObject saveObject = JsonUtility.FromJson<SaveObject>(saveString);
            GameResources.SetResourceAmounts(saveObject.ResourceAmountsKeys, saveObject.ResourceAmountsValues);
            for (int i = 0; i < ResourceList.Count;)
            {
                ResourceList[i].Destroy();
            }
            for (int i = 0; i < saveObject.Resources.Count; i++)
            {
                ResourceNodeData data = saveObject.ResourceData[i];
                GameObject newNode = (GameObject)GameObject.Instantiate(Resources.Load($"Resources/{data.Prefab}"), new Vector3(data.PositionX, data.PositionY, data.PositionZ), Quaternion.identity);
                newNode.GetComponent<ResourceNode>().SetData(data);
            }
        }
        else
        {
            Debug.LogError($"Save file could not be found at {Application.dataPath}/save.txt");
        }
    }
}