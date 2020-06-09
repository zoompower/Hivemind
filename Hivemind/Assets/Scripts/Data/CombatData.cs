using System;

namespace Assets.Scripts.Data
{
    [Serializable]
    public class CombatData : MindData
    {
        public float MinEstimatedDifference;
        public int PrefferedHealth;
        public string AntGuid;
        public bool Busy;

        public CombatData(float minEstimatedDifference, int prefferedHealth, Ant ant, bool busy)
        {
            MinEstimatedDifference = minEstimatedDifference;
            PrefferedHealth = prefferedHealth;
            if (ant != null)
            {
                AntGuid = ant.myGuid.ToString();
            }
            Busy = busy;
        }
    }
}