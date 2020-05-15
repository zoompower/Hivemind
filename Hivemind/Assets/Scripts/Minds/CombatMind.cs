
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
        CombatMind combatmind = mind as CombatMind;
        if (combatmind != null)
        {
            if (combatmind.minEstimatedDifference == minEstimatedDifference && combatmind.prefferedHealth == prefferedHealth)
            {
                return true;
            }
        }
        return false;
    }

    public void Execute(Ant ant)
    {
        if (this == null)
        {
            new CombatFight().Execute(ant);
        }

        float healthDifference = ((float)ant.health / (float)ant.closestEnemy.health);
        float damageDifference = ((float)ant.damage / (float)ant.closestEnemy.damage);
        float strengthDifference = (healthDifference * 1 + damageDifference * 2) / 3;

        if (strengthDifference >= minEstimatedDifference)
        {
            new CombatFight().Execute(ant);
        }
        else
        {
            new CombatFlee().Execute(ant);
        }
    }

    public void GenerateUI()
    {
        throw new System.NotImplementedException();
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

    public void Initiate()
    {

    }

    public double Likelihood(Ant ant)
    {
        if (ant.InCombat())
        {
            return 100;
        }
        else return 0;

    }

    public void Update(IMind mind)
    {

    }
}
