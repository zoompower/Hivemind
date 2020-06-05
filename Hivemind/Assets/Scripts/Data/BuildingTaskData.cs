using System;
using UnityEngine;

namespace Assets.Scripts.Data
{
    [Serializable]
    public class BuildingTaskData
    {
        public bool IsRemoved;
        public BaseBuildingTool BaseBuildingTool;
        public string AntGuid;
        public string HighlightObjPrefab;
        public string BaseTileName;

        public BuildingTaskData(bool isRemoved, BaseBuildingTool baseBuildingTool, Ant ant, GameObject highlightObj, BaseTile baseTile)
        {
            IsRemoved = isRemoved;
            BaseBuildingTool = baseBuildingTool;
            if (ant != null && ant.myGuid != null)
            {
                AntGuid = ant.myGuid.ToString();
            }
            if (highlightObj != null && highlightObj.GetComponent<HighLightScript>() != null)
            {
                HighlightObjPrefab = highlightObj.GetComponent<HighLightScript>().Prefab;
            }
            if (baseTile != null)
            {
                BaseTileName = baseTile.gameObject.name;
            }
        }
    }
}