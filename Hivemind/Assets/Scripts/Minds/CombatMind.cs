using Assets.Scripts.Data;
using System;

public class CombatMind : IMind
{
    private float minEstimatedDifference;
    private int prefferedHealth;
    private Ant ant;
    private bool busy;

    public CombatMind() : this(0, 0)
    {
    }

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
        if (data.AntGuid != "")
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