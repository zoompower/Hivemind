using Assets.Scripts.Data;
using System;
using UnityEngine;

public class BuildingTask
{
    public BaseTile BaseTile;
    public BaseBuildingTool BaseBuildingTool;
    public Ant Ant;
    public GameObject HighlightObj;

    internal bool IsRemoved = false;

    public BuildingTask(BaseTile tile, BaseBuildingTool tool)
    {
        BaseTile = tile;
        BaseBuildingTool = tool;
        Ant = null;
    }

    public bool Equals(BaseTile obj)
    {
        return BaseTile.Equals(obj);
    }

    public BuildingTaskData GetData()
    {
        return new BuildingTaskData(IsRemoved, BaseBuildingTool, Ant, HighlightObj, BaseTile);
    }

    public void SetData(BuildingTaskData data)
    {
        IsRemoved = data.IsRemoved;
        BaseBuildingTool = data.BaseBuildingTool;
        if(data.AntGuid != "")
        {
            Ant = GameWorld.Instance.FindAnt(Guid.Parse(data.AntGuid));
        }
        BaseTile = GameObject.Find(data.BaseTileName).GetComponent<BaseTile>();
        HighlightObj = (GameObject)GameObject.Instantiate(Resources.Load($"Prefabs/BaseBuilding/{data.HighlightObjPrefab}"), BaseTile.transform);
    }
}
