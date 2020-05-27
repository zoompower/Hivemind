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
                    if (!tile.IsIndestructable)
                    {
                        CheckTask(buildingTask);
                    }
                    break;
                case BaseBuildingTool.Wall:
                    CheckTask(buildingTask);
                    break;
                case BaseBuildingTool.AntRoom:
                    if (!tile.IsUnbuildable)
                    {
                        CheckTask(buildingTask);
                    }
                    break;
                default:
                    if (tile.RoomScript != null && !tile.RoomScript.IsRoom() && !tile.IsIndestructable)
                    {
                        CheckTask(buildingTask);
                    }
                    break;
            }
        }
    }

    private void CheckTask(BuildingTask task)
    {
        switch (task.BaseBuildingTool)
        {
            case BaseBuildingTool.Wall:
                // TODO: When building walls is allowed final restrictions
                break;
            case BaseBuildingTool.AntRoom:
                Queue.Add(task);
                break;
            case BaseBuildingTool.Destroy:
            default:
                bool able = true;
                foreach (var neighbor in task.BaseTile.Neighbors)
                {
                    Astar.CanFindQueen(neighbor, controller.QueenRoom as BaseRoom); // TODO: continue here
                    // TODO: fix bug of rooms not connecting properly.
                }
                if (able) Queue.Add(task);
                break;
        }
    }

    public void FinishTask(BuildingTask task)
    {
        Queue.Remove(task);
    }
}