using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

public static class GameWorld
{
    public static List<ResourceNode> UnknownResources = new List<ResourceNode>();
    public static List<ResourceNode> KnownResources = new List<ResourceNode>();
    private static Storage storage = null;

    public static ResourceNode FindNearestUnknownResource(Vector3 antPosition, ResourceType prefType)
    {
        ResourceNode closest = null;
        float minDistance = 100000000000000f;
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
        float minDistance = 100000000000000f;
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
