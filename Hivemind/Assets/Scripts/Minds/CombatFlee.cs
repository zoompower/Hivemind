using System;


class CombatFlee : IMind
{
    public void Execute(Ant ant)
    {
        ant.GetAgent().SetDestination(ant.GetStorage().GetPosition());
    }

    public void Initiate()
    {
        throw new NotImplementedException();
    }
}
