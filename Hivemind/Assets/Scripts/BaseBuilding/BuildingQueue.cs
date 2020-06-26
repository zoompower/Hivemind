using Assets.Scripts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        Queue.Sort((taskA, taskB) =>
        {
            Vector3 va = taskA.BaseTile.transform.position;
            Vector3 vb = taskB.BaseTile.transform.position;
            Vector3 vant = ant.transform.position;

            return Mathf.Abs(va.x - vant.x) + Mathf.Abs(va.z - vant.z) < Mathf.Abs(vb.x - vant.x) + Mathf.Abs(vb.z - vant.z) ? -1 : 1;
        });

        var job = Queue.Where((task) => task.Ant == null || task.Ant == ant).FirstOrDefault();

        if (job != null)
        {
            if (job.BaseBuildingTool == BaseBuildingTool.AntRoom && controller.GetComponent<UnitController>().MindGroupList.GetTotalPossibleAnts() >= GameWorld.UnitLimit)
            {
                Remove(job, true);
            }
            else
            {
                job.Ant = ant;
                return job;
            }
        }

        return null;
    }

    internal void RemoveJob(BuildingTask task)
    {
        task.Ant = null;
    }

    public void AddNewJob(BaseTile tile, BaseBuildingTool tool)
    {
        BuildingTask existingTask = Queue.FirstOrDefault(task => task.BaseTile == tile) ?? WaitQueue.FirstOrDefault(task => task.BaseTile == tile);

        if (existingTask == null)
        {
            BuildingTask buildingTask = new BuildingTask(tile, tool);
            switch (tool)
            {
                case BaseBuildingTool.DestroyRoom:
                    if (!tile.IsIndestructable && tile.RoomScript != null && tile.RoomScript.IsRoom())
                    {
                        Add(buildingTask);
                    }
                    break;

                case BaseBuildingTool.Wall:
                    //Queue.Add(buildingTask); // TODO: when building walls is gonna be a thing
                    break;

                case BaseBuildingTool.AntRoom:
                    if (controller.GetComponent<UnitController>().MindGroupList.GetTotalPossibleAnts() < GameWorld.UnitLimit && !tile.IsUnbuildable && tile.RoomScript == null && CalculateAndDoCost(BaseBuildingTool.AntRoom))
                    {
                        Add(buildingTask);
                    }
                    break;

                default:
                    if (tile.RoomScript != null && !tile.RoomScript.IsRoom() && !tile.IsIndestructable)
                    {
                        bool able = false;
                        foreach (var neighbor in buildingTask.BaseTile.Neighbors)
                        {
                            if (Astar.CanFindQueen(neighbor, controller.QueenRoom.GetBaseTile()))
                            {
                                able = true;
                                break;
                            }
                        }

                        Add(buildingTask, !able);
                    }
                    break;
            }
        }
        else
        {
            if (existingTask.BaseBuildingTool == tool || tool == BaseBuildingTool.DestroyRoom)
            {
                Remove(existingTask, true);
            }
        }
    }

    public void FinishTask(BuildingTask task)
    {
        if (task.BaseBuildingTool == BaseBuildingTool.AntRoom && controller.GetComponent<UnitController>().MindGroupList.GetTotalPossibleAnts() >= GameWorld.UnitLimit)
        {
            CancelTask(task);
        }
        if (task.IsRemoved) return;

        task.BaseTile.AntDoesAction(task.BaseBuildingTool);
        Remove(task);
    }

    public void CancelTask(BuildingTask task)
    {
        if (task.IsRemoved) return;

        Remove(task, true);
    }

    public void VerifyTasks()
    {
        if (WaitQueue.Count == 0) return;

        for (int i = WaitQueue.Count - 1; i >= 0; i--)
        {
            foreach (var neighbor in WaitQueue[i].BaseTile.Neighbors)
            {
                if (Astar.CanFindQueen(neighbor, controller.QueenRoom.GetBaseTile()))
                {
                    Queue.Add(WaitQueue[i]);
                    WaitQueue.RemoveAt(i);
                    break;
                }
            }
        }
    }

    private void Add(BuildingTask task, bool wait = false)
    {
        GameObject prefab = controller.GetHighlightObj(task.BaseBuildingTool);

        if (prefab == null) throw new Exception($"There is no highlight for the tool: {task.BaseBuildingTool}");

        task.HighlightObj = GameObject.Instantiate(prefab);
        task.HighlightObj.transform.SetParent(task.BaseTile.transform, false);

        if (wait)
        {
            WaitQueue.Add(task);
        }
        else
        {
            Queue.Add(task);
        }
    }

    private void Remove(BuildingTask task, bool refund = false)
    {
        task.IsRemoved = true;
        GameObject.Destroy(task.HighlightObj);

        Queue.Remove(task);
        WaitQueue.Remove(task);

        if (refund)
        {
            RefundCost(task.BaseBuildingTool);
        }
    }

    public BuildingQueueData GetData()
    {
        return new BuildingQueueData(Queue, WaitQueue, controller.TeamID);
    }

    public void SetData(BuildingQueueData data)
    {
        foreach (BuildingTask buildingTask in Queue)
        {
            GameObject.Destroy(buildingTask.HighlightObj);
        }
        Queue.Clear();
        foreach (BuildingTask buildingTask in WaitQueue)
        {
            GameObject.Destroy(buildingTask.HighlightObj);
        }
        WaitQueue.Clear();
        foreach (BuildingTaskData buildingTaskData in data.Queue)
        {
            BuildingTask task = new BuildingTask(null, BaseBuildingTool.Default);
            task.SetData(buildingTaskData, controller);
            Queue.Add(task);
        }
        foreach (BuildingTaskData buildingTaskData in data.WaitQueue)
        {
            BuildingTask task = new BuildingTask(null, BaseBuildingTool.Default);
            task.SetData(buildingTaskData, controller);
            WaitQueue.Add(task);
        }
        controller = GameWorld.Instance.FindBaseController(data.ControllerID);
    }

    private bool CalculateAndDoCost(BaseBuildingTool tool)
    {
        Dictionary<ResourceType, int> totalCost = GameResources.GetToolCost(tool);

        if (GameResources.EnoughResources(totalCost, controller.GetGameResources()))
        {
            controller.GetGameResources().SubtractResources(totalCost);
            return true;
        }
        return false;
    }

    private void RefundCost(BaseBuildingTool tool)
    {
        controller.GetGameResources().AddResources(GameResources.GetToolCost(tool));
    }
}
