using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data
{
    [Serializable]
    public class BasicAIData
    {
        public bool WaitingForMindsGotten;
        public int CoolingDown;

        public BasicAIData(bool waitingForMindsGotten, int coolingDown)
        {
            WaitingForMindsGotten = waitingForMindsGotten;
            CoolingDown = coolingDown;
        }
    }
}
