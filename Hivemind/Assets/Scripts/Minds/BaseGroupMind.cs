using Assets.Scripts.Data;
using System;
using System.Collections;
using UnityEngine;

public class BaseGroupMind : IMind
{
    private Ant ant;

    private BaseController controller;

    private bool busy = false;
    private float waitTimer;
    private bool waiting;

    public BaseGroupMind()
    {
    }

    public void Initiate(Ant ant)
    {
        this.ant = ant;

        var controllers = UnityEngine.Object.FindObjectsOfType<BaseController>();

        foreach (var controller in controllers)
        {
            if (controller.TeamID == ant.TeamID)
            {
                this.controller = controller;
                break;
            }
        }
    }

    public void Execute()
    {
        if (IsBusy() || waiting) return;

        var job = controller.BuildingQueue.GetJob(ant);
        if (job != null)
        {
            ant.StartCoroutine(DoAction(job));
        }
        else
        {
            ant.StartCoroutine(Wait(1));
        }
    }

    private IEnumerator DoAction(BuildingTask task)
    {
        busy = true;
        Vector3 target = (Vector3.Normalize(ant.transform.position - task.BaseTile.transform.position) / 4) + task.BaseTile.transform.position;
        ant.GetAgent().SetDestination(target);

        yield return new WaitWhile(() =>
        {
            if (task.IsRemoved) return false;

            target = Vector3.Normalize(ant.transform.position - task.BaseTile.transform.position) * 1.2f + task.BaseTile.transform.position;
            if (Vector3.Distance(target, ant.transform.position) < 1 && Vector3.Distance(target, ant.GetAgent().destination) > 0.1f)
            {
                if (!ant.GetAgent().isOnNavMesh)
                {
                    ant.GetAgent().enabled = true;
                }
                ant.GetAgent().SetDestination(target);
            }

            if (Vector3.Distance(target, ant.transform.position) < 0.3)
            {
                return false;
            }
            return true;
        });

        ant.GetAgent().ResetPath();

        if (!task.IsRemoved)
        {
            yield return new WaitForSeconds(2);

            controller.BuildingQueue.FinishTask(task);
        }

        busy = false;
    }

    private IEnumerator Wait(float seconds)
    {
        waiting = true;
        waitTimer = seconds;
        while (waitTimer > 0f)
        {
            yield return new WaitForSeconds(0.1f);
            waitTimer -= 0.1f;
        }
        waiting = false;
    }

    public IMind Clone()
    {
        return new BaseGroupMind();
    }

    public double Likelihood()
    {
        return 10000;
    }

    public bool IsBusy()
    {
        return busy;
    }

    public MindData GetData()
    {
        return new BaseGroupMindData(ant, waiting, waitTimer);
    }

    public void SetData(MindData mindData)
    {
        BaseGroupMindData data = mindData as BaseGroupMindData;
        waiting = data.Waiting;
        if (data.AntGuid != "")
        {
            ant = GameWorld.Instance.FindAnt(Guid.Parse(data.AntGuid));
            if (waiting && data.WaitTimer > 0f)
            {
                ant.StartCoroutine(Wait(data.WaitTimer));
            }
        }
    }
}