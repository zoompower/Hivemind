using System.Collections.Generic;

namespace Assets.Scripts.Data
{
    public class TeamMindGroup
    {
        public int teamID;
        public List<MindGroup> MindGroups;

        public TeamMindGroup(int teamId, List<MindGroup> list)
        {
            teamID = teamId;
            MindGroups = list;
        }

        internal TeamMindGroupData GetData()
        {
            return new TeamMindGroupData(teamID, MindGroups);
        }
    }
}