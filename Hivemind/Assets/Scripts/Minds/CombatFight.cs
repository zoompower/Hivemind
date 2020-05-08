using System;

class CombatFight : IMind
{
    public void Execute(Ant ant)
    {
        ant.GetAgent().SetDestination(ant.closestEnemy.transform.position);
    }

    public void Initiate()
    {
        throw new NotImplementedException();
    }
}