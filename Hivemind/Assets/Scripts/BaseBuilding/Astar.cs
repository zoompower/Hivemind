using System.Collections.Generic;
using UnityEngine;

static class Astar
{
    private static List<BaseTile> resetList = new List<BaseTile>();

    public static void RegisterResetTile(BaseTile tile)
    {
        resetList.Add(tile);
    }

    public static void RemoveResetTile(BaseTile tile)
    {
        resetList.Remove(tile);
    }

    private static void ResetList()
    {
        foreach (var tile in resetList)
        {
            tile.AstarVisited = false;
        }
    }

    public static bool CanFind(BaseTile start, BaseTile end, List<BaseTile> ignoreList)
    {
        ResetList();

        foreach (var tile in ignoreList)
        {
            tile.AstarVisited = true;
        }

        TileQueue queue = new TileQueue();

        queue.push(start);

        BaseTile currTile;
        while ((currTile = queue.pop(end.transform.position)) != null)
        {
            if (currTile == end)
            {
                return true;
            }

            currTile.AstarVisited = true;

            foreach (var neighbor in currTile.Neighbors)
            {
                BaseTile neighborTile = neighbor.GetComponent<BaseTile>();
                if (neighborTile.RoomScript != null && neighborTile.RoomScript.IsRoom() && neighborTile.RoomScript.GetType() == currTile.RoomScript.GetType())
                {
                    queue.push(neighborTile);
                }
            }
        }

        return false;
    }

}

