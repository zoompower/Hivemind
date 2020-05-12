
public class CombatMind : IMind
{
    private float minEstimetedDifference;
    private int prefferedHealth;


    public CombatMind(float minEstimeted, int prefHealth)
    {
        minEstimetedDifference = minEstimeted;
        prefferedHealth = prefHealth;
    }

    public IMind Clone()
    {
        return new CombatMind(minEstimetedDifference, prefferedHealth);
    }

    public bool Equals(IMind mind)
    {
        CombatMind combatmind = mind as CombatMind;
        if (combatmind != null)
        {
            if(combatmind.minEstimetedDifference == minEstimetedDifference && combatmind.prefferedHealth == prefferedHealth)
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

            float healthPercantageDifference = ((float)ant.health / (float)ant.closestEnemy.health);
            float damagePercantageDifference = ((float)ant.damage / (float)ant.closestEnemy.damage);
            float strengthDifference = (healthPercantageDifference * 1 + damagePercantageDifference * 2) / 3;

            if (strengthDifference >= minEstimetedDifference)
            {
                 new CombatFight().Execute(ant);
            }
             new CombatFlee().Execute(ant);
    }

    public float GetMinEstimetedDifference()
    {
        return minEstimetedDifference;
    }

    public int GetPrefferedHealth()
    {
        return prefferedHealth;
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
        throw new System.NotImplementedException();
    }
}
