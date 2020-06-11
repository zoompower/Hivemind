internal class CombatFlee
{
    public void Execute(Ant ant)
    {
        ant.GetAgent().SetDestination(ant.GetBaseController().GetPosition());
    }
}