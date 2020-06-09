using System;

namespace Assets.Scripts.Data
{
    [Serializable]
    public class BaseGroupMindData : MindData
    {
        public string AntGuid;
        public bool Waiting;
        public float WaitTimer;

        public BaseGroupMindData(Ant ant, bool waiting, float waitTimer)
        {
            if (ant != null)
            {
                AntGuid = ant.myGuid.ToString();
            }
            Waiting = waiting;
            WaitTimer = waitTimer;
        }
    }
}