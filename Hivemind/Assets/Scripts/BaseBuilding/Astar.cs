using System;
using System.Collections.Generic;

static class Astar
{
    private static List<BaseTile> resetList = new List<BaseTile>();

    public static void RegisterResetableRoom(BaseTile tile)
    {
        resetList.Add(tile);
    }

    public static void RemoveResetableRoom(BaseTile tile)
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

    public static bool CanFindOwnNeighbor(BaseTile start, BaseTile end, List<BaseTile> ignoreList)
    {
        ResetList();

        foreach (var tile in ignoreList)
        {
            tile.AstarVisited = true;
        }

        BaseTilePriorityQueue queue = new BaseTilePriorityQueue();

        queue.push(start);

        BaseTile currTile;
        while ((currTile = queue.pop(end.transform.position)) != null)
        {
            if (currTile == end) return true;

            if (currTile.AstarVisited) continue;

            currTile.AstarVisited = true;

            foreach (var neighbor in currTile.Neighbors)
            {
                if (neighbor.RoomScript != null && neighbor.RoomScript.IsRoom() && neighbor.RoomScript.GetType() == currTile.GetType())
                {
                    queue.push(neighbor);
                }
            }
        }

        return false;
    }

    internal static int ConvertRoom(BaseTile initialRoom, Guid newId, BaseTile invokingRoom)
    {
        ResetList();
        invokingRoom.AstarVisited = true;
        initialRoom.AstarVisited = true;
        List<BaseTile> roomList = new List<BaseTile>() { initialRoom };

        int tileCount = 0;

        while (roomList.Count > 0)
        {
            var currTile = roomList[0];
            roomList.RemoveAt(0);
            currTile.GetComponentInChildren<BaseUnitRoom>().GroupId = newId;
            currTile.GetComponentInChildren<BaseUnitRoom>();
            tileCount++;

            foreach (var neighbor in currTile.GetComponentInParent<BaseTile>()?.Neighbors)
            {
                if (neighbor.RoomScript != null && neighbor.RoomScript.IsRoom() && neighbor.RoomScript.GetType() == currTile.GetType())
                {
                    if (!neighbor.AstarVisited)
                    {
                        neighbor.AstarVisited = true;
                        roomList.Add(neighbor);
                    }
                }
            }
        }
        return tileCount;
    }

    internal static void CanFindQueen(BaseTile initialRoom, BaseRoom queenRoom)
    {
        ResetList();
        initialRoom.AstarVisited = true;

        List<BaseTile> roomList = new List<BaseTile>() { initialRoom };
    }
}

