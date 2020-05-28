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
}
