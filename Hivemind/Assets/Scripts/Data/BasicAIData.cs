using System;

namespace Assets.Scripts.Data
{
    [Serializable]
    public class BasicAIData
    {
        public bool WaitingForMindsGotten;
        public int CoolingDown;
        public int TeamID;

        public BasicAIData(bool waitingForMindsGotten, int coolingDown, int teamID)
        {
            WaitingForMindsGotten = waitingForMindsGotten;
            CoolingDown = coolingDown;
            TeamID = teamID;
        }
    }
}
