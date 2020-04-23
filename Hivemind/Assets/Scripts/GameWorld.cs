using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

public static class GameWorld
{
    public static List<ResourceNode> resources = new List<ResourceNode>();
    private static Storage storage = null;

    public static ResourceNode FindNearestResource(Vector3 antPosition)
    {
        ResourceNode closest = null;
        float minDistance = 100000000000000f;
        foreach (ResourceNode resource in resources)
        {
            if (resource.HasResources())
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

    public static Storage GetStorage()
    {
        return storage;
    }

    public static void RemoveResource(ResourceNode resource)
    {
        resources.Remove(resource);
    }

    public static void SetStorage(Storage Storage)
    {
        storage = Storage;
    }

    public static void AddNewResource(ResourceNode resource)
    {
        resources.Add(resource);
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
