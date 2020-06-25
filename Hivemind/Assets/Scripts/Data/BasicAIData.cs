using System;

namespace Assets.Scripts.Data
{
    [Serializable]
    public class BasicAIData
    {
        public BasicAi.AttackingState AttackingState;
        public int CoolingDown;
        public int AttackCount;
        public int TeamID;

        public BasicAIData(BasicAi.AttackingState attackingState, int coolingDown, int attackCount, int teamID)
        {
            AttackingState = attackingState;
            CoolingDown = coolingDown;
            AttackCount = attackCount;
            TeamID = teamID;
        }
    }
}
