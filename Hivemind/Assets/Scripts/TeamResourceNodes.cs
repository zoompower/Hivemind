using System.Collections.Generic;
using UnityEngine;

public class TeamResourceNodes
{
    public int TeamID;
    private List<ResourceNode> unknownResources = new List<ResourceNode>();
    private List<ResourceNode> knownResources = new List<ResourceNode>();

    public TeamResourceNodes(int teamId, List<ResourceNode> resourceList)
    {
        TeamID = teamId;
        unknownResources = new List<ResourceNode>(resourceList);
    }

    public ResourceNode FindNearestUnknownResource(Vector3 antPosition)
    {
        ResourceNode closest = null;
        float minDistance = float.MaxValue;
        foreach (ResourceNode resource in unknownResources)
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

    public ResourceNode FindNearestKnownResource(Vector3 antPosition, ResourceType prefType)
    {
        ResourceNode closest = null;
        float minDistance = float.MaxValue;
        foreach (ResourceNode resource in knownResources)
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

    public void RemoveResource(ResourceNode resource)
    {
        knownResources.Remove(resource);
        unknownResources.Remove(resource);
    }

    public void AddNewResource(ResourceNode resource)
    {
        unknownResources.Add(resource);
    }

    public void AddNewKnownResource(ResourceNode resource)
    {
        if (!knownResources.Contains(resource))
        {
            knownResources.Add(resource);
            unknownResources.Remove(resource);
        }
    }
}
