using System;
using UnityEngine;

public class BaseGroupMind : IMind
{
    private Ant ant;

    private BaseController controller;

    public BaseGroupMind()
    {

    }

    public void Initiate(Ant ant)
    {
        this.ant = ant;

        var controllers = GameObject.FindObjectsOfType<BaseController>();

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
        return false;
    }
}

