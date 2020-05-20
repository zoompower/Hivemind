using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class TileQueue
{
    List<BaseTile> queue = new List<BaseTile>();

    public void push(BaseTile tile)
    {
        if (tile.AstarVisited) return;

        queue.Add(tile);
    }

    public BaseTile pop(Vector3 targetPosition)
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
