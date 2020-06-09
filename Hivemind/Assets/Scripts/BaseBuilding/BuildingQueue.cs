﻿using System;
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
                    if (!tile.IsUnbuildable && tile.RoomScript == null)
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
            if (existingTask.BaseBuildingTool == tool)
            {
                Remove(existingTask);
            }
        }
    }

    public void FinishTask(BuildingTask task)
    {
        if (task.IsRemoved) return;

        task.BaseTile.AntDoesAction(task.BaseBuildingTool);
        Remove(task);
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

    private void Remove(BuildingTask task)
    {
        task.IsRemoved = true;
        GameObject.Destroy(task.HighlightObj);

        Queue.Remove(task);
        WaitQueue.Remove(task);
    }
}