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
            if (currTile == end)
            {
                return true;
            }

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

}

