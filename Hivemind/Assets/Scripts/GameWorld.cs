using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GameWorld
{
    public static List<ResourceNode> UnknownResources = new List<ResourceNode>();
    public static List<ResourceNode> KnownResources = new List<ResourceNode>();
    private static List<TeamResourceNodes> teamResourceNodes = new List<TeamResourceNodes>();
    private static List<ResourceNode> allResourceNodes = new List<ResourceNode>();

    public static bool AddNewTeam(int teamId)
    {
        if (teamResourceNodes.FirstOrDefault(teamResource => teamResource.TeamID == teamId) != null) return false;

        teamResourceNodes.Add(new TeamResourceNodes(teamId, allResourceNodes));

        return true;
    }

    public static ResourceNode FindNearestUnknownResource(Vector3 antPosition, int teamID)
    {
        var team = teamResourceNodes.FirstOrDefault(teamResource => teamResource.TeamID == teamID);

        if (team != null)
        {
            return team.FindNearestUnknownResource(antPosition);
        }

        Debug.LogWarning($"An ant tried to get teamId:{teamID}, which does not exist!");
        return null;
    }

    public static ResourceNode FindNearestKnownResource(Vector3 antPosition, ResourceType prefType, int teamID)
    {
        var team = teamResourceNodes.FirstOrDefault(teamResource => teamResource.TeamID == teamID);

        if (team != null)
        {
            return team.FindNearestKnownResource(antPosition, prefType);
        }

        Debug.LogWarning($"An ant tried to get teamId:{teamID}, which does not exist!");
        return null;
    }

    public static void RemoveResource(ResourceNode resource)
    {
        allResourceNodes.Remove(resource);

        foreach (TeamResourceNodes teamResource in teamResourceNodes)
        {
            teamResource.RemoveResource(resource);
        }
    }

    public static void AddNewResource(ResourceNode resource)
    {
        allResourceNodes.Add(resource);

        foreach (TeamResourceNodes teamResource in teamResourceNodes)
        {
            teamResource.AddNewResource(resource);
        }
    }

    public static void AddNewKnownResource(ResourceNode resource, int teamID)
    {
        var team = teamResourceNodes.FirstOrDefault(teamResource => teamResource.TeamID == teamID);

        if (team != null)
        {
            team.AddNewKnownResource(resource);
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