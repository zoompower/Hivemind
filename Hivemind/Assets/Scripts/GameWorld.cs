using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

public static class GameWorld
{
    private static List<Transform> resources = new List<Transform>();
    private static Transform storage = CreateStorage();

    public static Transform FindNearestResource(Vector3 antPosition)
    {
        Transform closest = null;
        float minDistance = 100000000000000f;
        foreach (Transform resource in resources)
        {
            float dist = Vector3.Distance(antPosition, resource.position);
            if (dist < minDistance)
            {
                closest = resource;
                minDistance = dist;
            }
        }
        return closest;
    }

    private static Transform CreateStorage()
    {
        storage = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
        storage.position = new Vector3(0f, 0.5f, 0f);
        return storage;
    }

    public static Transform GetStorage()
    {
        return storage;
    }

    public static void RemoveResource(Transform resource)
    {
        resources.Remove(resource);
    }

    public static void CreateNewResource(int amount = 1)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject newResource = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            newResource.transform.position = new Vector3(UnityEngine.Random.Range(-50f, 50f), 0.5f, UnityEngine.Random.Range(-50f, 50f));
            resources.Add(newResource.transform);
        }
    }
}
