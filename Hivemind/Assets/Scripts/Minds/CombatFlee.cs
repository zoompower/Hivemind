using System;


class CombatFlee
{
    public void Execute(Ant ant)
    {
        ant.GetAgent().SetDestination(ant.GetStorage().GetPosition());
    }
}
