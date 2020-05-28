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
                if (neighbor.RoomScript != null && neighbor.RoomScript.IsRoom() && neighbor.RoomScript.GetType() == currTile.RoomScript.GetType())
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
            tileCount++;

            foreach (var neighbor in currTile.Neighbors)
            {
                if (neighbor.RoomScript != null && neighbor.RoomScript.IsRoom() && neighbor.RoomScript.GetType() == currTile.RoomScript.GetType())
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

    internal static bool CanFindQueen(BaseTile initialRoom, BaseTile queenRoom)
    {
        ResetList();

        if (initialRoom.RoomScript != null && !initialRoom.RoomScript.IsRoom()) return false;

        BaseTilePriorityQueue queue = new BaseTilePriorityQueue();

        queue.push(initialRoom);

        BaseTile currTile;
        while ((currTile = queue.pop(queenRoom.transform.position)) != null)
        {
            if (currTile == queenRoom) return true;

            currTile.AstarVisited = true;

            foreach (var neighbor in currTile.Neighbors)
            {
                if (neighbor.RoomScript == null || (neighbor.RoomScript != null && neighbor.RoomScript.IsRoom()))
                {
                    queue.push(neighbor);
                }
            }
        }

        return false;
    }
}

