using Assets.Scripts.Data;
using System;

[Serializable]
public class CombatMind : IMind
{
    private float minEstimatedDifference;
    private int prefferedHealth;


    public CombatMind(float minEstimeted, int prefHealth)
    {
        minEstimatedDifference = minEstimeted;
        prefferedHealth = prefHealth;
    }

    public IMind Clone()
    {
        return new CombatMind(minEstimatedDifference, prefferedHealth);
    }

    public bool Equals(IMind mind)
    {
        var combatmind = mind as CombatMind;
        if (combatmind != null)
            if (combatmind.minEstimatedDifference == minEstimatedDifference &&
                combatmind.prefferedHealth == prefferedHealth)
                return true;
        return false;
    }

    public void Execute(Ant ant)
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

    public void Initiate()
    {
    }

    public double Likelihood(Ant ant)
    {
        if (ant.InCombat())
            return 100;
        return 0;
    }

    public void Update(IMind mind)
    {
    }

    public float GetMinEstimetedDifference()
    {
        return minEstimatedDifference;
    }

    public void SetMinEstimetedDifference(float estDiff)
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
        return null;
    }

    public void SetData(MindData mindData)
    {

    }
}