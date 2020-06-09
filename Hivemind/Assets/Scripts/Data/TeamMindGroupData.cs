using System;
using System.Collections.Generic;

namespace Assets.Scripts.Data
{
    [Serializable]
    public class TeamMindGroupData
    {
        public int TeamId;
        public List<MindGroupData> MindGroupDataList = new List<MindGroupData>();

        public TeamMindGroupData(int teamId, List<MindGroup> mindList)
        {
            TeamId = teamId;

            foreach (var mindGroup in mindList)
            {
                MindGroupDataList.Add(mindGroup.GetData());
            }
        }
    }
}