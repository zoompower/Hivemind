using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CombatBehaviour
{
    class CombatFight : ICombatAntBehaviour
    {
        public void CombatMode(Ant ant, Ant enemy)
        {
            ant.GetAgent().SetDestination(enemy.transform.position);
        }
    }
}
