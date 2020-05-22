using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class UnitRoomPriorityQueue
{
    private List<BaseUnitRoom> queue = new List<BaseUnitRoom>();

    public void push(BaseUnitRoom tile)
    {
        if (tile.AstarVisited) return;

        queue.Add(tile);
    }

    public BaseUnitRoom pop(Vector3 targetPosition)
    {
        if (queue.Count > 0)
        {
            queue = queue.OrderBy(t => Vector3.Distance(t.gameObject.transform.position, targetPosition)).ToList();

            var item = queue[0];
            queue.RemoveAt(0);

            return item;
        }
        return null;
    }
}
