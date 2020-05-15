class CombatFight 
{
    public void Execute(Ant ant)
    {
        ant.GetAgent().SetDestination(ant.closestEnemy.transform.position);
    }
}