using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Data
{
    [Serializable]
    public class MindGroupData
    {
        public List<UnitGroupData> UnitGroupDataList = new List<UnitGroupData>();
        public int Count;
        public int MindPoints;

        [SerializeReference]
        public List<IMind> Minds;
        [SerializeReference]
        public List<MindData> MindData = new List<MindData>();

        public MindGroupData(List<UnitGroup> unitGroupList, int count, List<IMind> minds, int mindPoints)
        {
            foreach (UnitGroup unitGroup in unitGroupList)
            {
                UnitGroupDataList.Add(unitGroup.GetData());
            }
            Count = count;
            Minds = minds;
            foreach (IMind mind in minds)
            {
                MindData.Add(mind.GetData());
            }
            MindPoints = mindPoints;
        }
    }
}
