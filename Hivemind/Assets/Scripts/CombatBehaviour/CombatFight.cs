using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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