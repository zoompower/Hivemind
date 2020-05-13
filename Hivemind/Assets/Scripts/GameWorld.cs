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
    public static List<ResourceNode> UnknownResources = new List<ResourceNode>();
    public static List<ResourceNode> KnownResources = new List<ResourceNode>();
    private static Storage storage = null;

    public static ResourceNode FindNearestUnknownResource(Vector3 antPosition, ResourceType prefType)
    {
        ResourceNode closest = null;
        float minDistance = float.MaxValue;
        foreach (ResourceNode resource in UnknownResources)
        {
            float dist = Vector3.Distance(antPosition, resource.GetPosition());
            if (dist < minDistance)
            {
                closest = resource;
                minDistance = dist;
            }
        }
        return closest;
    }

    public static ResourceNode FindNearestKnownResource(Vector3 antPosition, ResourceType prefType)
    {
        ResourceNode closest = null;
        float minDistance = float.MaxValue;
        foreach (ResourceNode resource in KnownResources)
        {
            if (prefType == ResourceType.Unknown || resource.resourceType == prefType)
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
        KnownResources.Remove(resource);
        UnknownResources.Remove(resource);
    }

    public static void SetStorage(Storage Storage)
    {
        storage = Storage;
    }

    public static void AddNewResource(ResourceNode resource)
    {
        UnknownResources.Add(resource);
    }

    public static void AddNewKnownResource(ResourceNode resource)
    {
        if (!KnownResources.Contains(resource))
        {
            KnownResources.Add(resource);
            UnknownResources.Remove(resource);
        }
    }

    public static void Save()
    {
        SaveObject saveObject = new SaveObject
        {
            ResourceAmountsKeys = GameResources.GetResourceAmounts().Keys.ToList(),
            ResourceAmountsValues = GameResources.GetResourceAmounts().Values.ToList(),
            UnknownResources = UnknownResources,
            KnownResources = KnownResources
        };
        string json = saveObject.ToJson();
        Debug.Log(json);
        File.WriteAllText(Application.dataPath + "/save.txt", json);
    }

    public static void Load()
    {
        if(File.Exists(Application.dataPath + "/save.txt"))
        {
            string saveString = File.ReadAllText(Application.dataPath + "/save.txt");
            SaveObject saveObject = JsonUtility.FromJson<SaveObject>(saveString);
            GameResources.SetResourceAmounts(saveObject.ResourceAmountsKeys, saveObject.ResourceAmountsValues);
            UnknownResources = saveObject.UnknownResources;
            KnownResources = saveObject.KnownResources;
        }
    }

    //public static void CreateNewResource(int amount = 1)
    //{
    //    for (int i = 0; i < amount; i++)
    //    {
    //        GameObject newResource = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    //        newResource.transform.position = new Vector3(Random.Range(-50f, 50f), 0.5f, UnityEngine.Random.Range(-50f, 50f));
    //        resources.Add(new ResourceNode(newResource.transform, newResource, UnityEngine.Random.Range(1, 5)));
    //    }
    //}
}