using System.Collections.Generic;
using System.Linq;

public class BuildingQueue
{
    private List<BuildingTask> Queue = new List<BuildingTask>();
    private List<BuildingTask> WaitQueue = new List<BuildingTask>();

    private BaseController controller;

    public BuildingQueue(BaseController controller)
    {
        this.controller = controller;
    }

    public BuildingTask GetJob(Ant ant)
    {
        var job = Queue.Where((task) => task.Ant == null).FirstOrDefault();

        if (job != null)
        {
            job.Ant = ant;
            return job;
        }

        return null;
    }

    public void AddNewJob(BaseTile tile, BaseBuildingTool tool)
    {
        if (Queue.FirstOrDefault(task => task.BaseTile == tile) == null)
        {
            BuildingTask buildingTask = new BuildingTask(tile, tool);
            switch (tool)
            {
                case BaseBuildingTool.Destroy:
                    if (!tile.IsIndestructable && tile.RoomScript != null && tile.RoomScript.IsRoom())
                    {
                        Queue.Add(buildingTask);
                    }
                    break;
                case BaseBuildingTool.Wall:
                    //Queue.Add(buildingTask); // TODO: when building walls is gonna be a thing
                    break;
                case BaseBuildingTool.AntRoom:
                    if (!tile.IsUnbuildable)
                    {
                        Queue.Add(buildingTask);
                    }
                    break;
                default:
                    if (tile.RoomScript != null && !tile.RoomScript.IsRoom() && !tile.IsIndestructable)
                    {
                        bool able = false;
                        foreach (var neighbor in buildingTask.BaseTile.Neighbors)
                        {
                            if (Astar.CanFindQueen(neighbor, controller.QueenRoom.GetComponentInParent<BaseTile>()))
                            {
                                able = true;
                                break;
                            }
                        }
                        if (able)
                        {
                            Queue.Add(buildingTask);
                        }
                        else
                        {
                            WaitQueue.Add(buildingTask);
                        }
                    }
                    break;
            }
        }
    }

    public void FinishTask(BuildingTask task)
    {
        Queue.Remove(task);
    }

    public void VerifyTasks()
    {
        if (WaitQueue.Count == 0) return;

        for (int i = WaitQueue.Count - 1; i >= 0; i--)
        {
            foreach (var neighbor in WaitQueue[i].BaseTile.Neighbors)
            {
                if (Astar.CanFindQueen(neighbor, controller.QueenRoom.GetComponentInParent<BaseTile>()))
                {
                    Queue.Add(WaitQueue[i]);
                    WaitQueue.RemoveAt(i);
                    break;
                }
            }
        }
    }
}