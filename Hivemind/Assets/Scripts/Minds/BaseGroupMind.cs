using Assets.Scripts.Data;
using System;
using System.Collections;
using UnityEngine;

public class BaseGroupMind : IMind
{
    private Ant ant;

    private BaseController controller;

    private bool busy = false;

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
        if (IsBusy()) return;

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

    private IEnumerator Wait(int seconds)
    {
        busy = true;
        yield return new WaitForSeconds(seconds);
        busy = false;
    }

    public IMind Clone()
    {
        return new BaseGroupMind();
    }

    public void GenerateUI()
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    public void SetData(MindData mindData)
    {
        throw new NotImplementedException();
    }
}

