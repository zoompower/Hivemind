using System;
using System.Collections.Generic;

static class Astar
{
    private static List<BaseUnitRoom> resetList = new List<BaseUnitRoom>();

    public static void RegisterResetableRoom(BaseUnitRoom tile)
    {
        resetList.Add(tile);
    }

    public static void RemoveResetableRoom(BaseUnitRoom tile)
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

    public static bool CanFind(BaseUnitRoom start, BaseUnitRoom end, List<BaseUnitRoom> ignoreList)
    {
        ResetList();

        foreach (var tile in ignoreList)
        {
            tile.AstarVisited = true;
        }

        UnitRoomPriorityQueue queue = new UnitRoomPriorityQueue();

        queue.push(start);

        BaseUnitRoom currTile;
        while ((currTile = queue.pop(end.transform.position)) != null)
        {
            if (currTile == end) return true;

            if (currTile.AstarVisited) continue;

            currTile.AstarVisited = true;

            foreach (var neighbor in currTile.GetComponentInParent<BaseTile>()?.Neighbors)
            {
                BaseTile neighborTile = neighbor.GetComponent<BaseTile>();
                if (neighborTile.RoomScript != null && neighborTile.RoomScript.IsRoom() && neighborTile.RoomScript.GetType() == currTile.GetType())
                {
                    queue.push(neighborTile.RoomScript as BaseUnitRoom);
                }
            }
        }

        return false;
    }

    internal static int ConvertRoom(BaseUnitRoom initialRoom, Guid newId, BaseUnitRoom invokingRoom)
    {
        ResetList();
        invokingRoom.AstarVisited = true;
        initialRoom.AstarVisited = true;
        List<BaseUnitRoom> roomList = new List<BaseUnitRoom>() { initialRoom };

        int tileCount = 0;

        while (roomList.Count > 0)
        {
            var currTile = roomList[0];
            roomList.RemoveAt(0);
            currTile.GroupId = newId;
            currTile.AttachUnitGroup();
            tileCount++;

            foreach (var neighbor in currTile.GetComponentInParent<BaseTile>()?.Neighbors)
            {
                BaseTile neighborTile = neighbor.GetComponent<BaseTile>();
                if (neighborTile.RoomScript != null && neighborTile.RoomScript.IsRoom() && neighborTile.RoomScript.GetType() == currTile.GetType())
                {
                    if (!(neighborTile.RoomScript as BaseUnitRoom).AstarVisited)
                    {
                        var neighborRoom = neighborTile.RoomScript as BaseUnitRoom;
                        neighborRoom.AstarVisited = true;
                        roomList.Add(neighborRoom);
                    }
                }
            }
        }
        return tileCount;
    }
}

