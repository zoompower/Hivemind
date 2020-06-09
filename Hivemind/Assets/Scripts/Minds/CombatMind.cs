using Assets.Scripts.Data;
using System;
using UnityEngine;


public class CombatMind : IMind
{
    private float minEstimatedDifference;
    private int prefferedHealth;
    private Ant ant;
    private bool busy;

    public CombatMind() : this(0, 0) { }

    public CombatMind(float minEstimated, int prefHealth)
    {
        minEstimatedDifference = minEstimated;
        prefferedHealth = prefHealth;
    }

    public IMind Clone()
    {
        return new CombatMind(minEstimatedDifference, prefferedHealth);
    }

    public void Execute()
    {
        if (this == null) new CombatFight().Execute(ant);

        var healthDifference = ant.health / (float) ant.closestEnemy.health;
        var damageDifference = ant.damage / (float) ant.closestEnemy.damage;
        var strengthDifference = (healthDifference * 1 + damageDifference * 2) / 3;

        if (strengthDifference >= minEstimatedDifference)
            new CombatFight().Execute(ant);
        else
            new CombatFlee().Execute(ant);
    }

    public void GenerateUI()
    {
        throw new NotImplementedException();
    }

    public void Initiate(Ant ant)
    {
        this.ant = ant;
    }

    public double Likelihood()
    {
        if (ant.InCombat())
        {
            busy = true;
            return 100;
        }
        busy = false;
        return 0;
    }

    public float GetMinEstimatedDifference()
    {
        return minEstimatedDifference;
    }

    public void SetMinEstimatedDifference(float estDiff)
    {
        minEstimatedDifference = estDiff;
    }

    public int GetPrefferedHealth()
    {
        return prefferedHealth;
    }

    public void SetPrefferedHealth(int prefHealth)
    {
        prefferedHealth = prefHealth;
    }

    public MindData GetData()
    {
        return new CombatData(minEstimatedDifference, prefferedHealth, ant, busy);
    }

    public void SetData(MindData mindData)
    {
        CombatData data = mindData as CombatData;
        minEstimatedDifference = data.MinEstimatedDifference;
        prefferedHealth = data.PrefferedHealth;
        if(data.AntGuid != "")
        {
            ant = GameWorld.Instance.FindAnt(Guid.Parse(data.AntGuid));
        }
        busy = data.Busy;
    }

    public bool IsBusy()
    {
        return busy;
    }
}