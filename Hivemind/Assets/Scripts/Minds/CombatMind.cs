using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class CombatMind
    {
        private float minEstimetedDifference;
        private int prefferedHealth;
        

        public CombatMind(float minEstimeted, int prefHealth)
        {
            minEstimetedDifference = minEstimeted;
            prefferedHealth = prefHealth;
        }
        public float GetMinEstimetedDifference()
        {
            return minEstimetedDifference;
        }

        public int GetPrefferedHealth()
        {
            return prefferedHealth;
        }
    }
}
