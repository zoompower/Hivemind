using UnityEngine;

internal class CombatFight
{
    private Ant ant;

    public void Execute(Ant ant)
    {
        this.ant = ant;
        ant.GetAgent().SetDestination(ant.closestEnemy.transform.position);

        if (Vector3.Distance(ant.transform.position, ant.closestEnemy.transform.position) < 1f)
        {
            Attack();
        }
        else
        {

        }
    }

    public void Attack()
    {
        ant.closestEnemy.health -= ant.damage;
    }

}