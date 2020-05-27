public class BuildingTask
{
    public BaseTile BaseTile;
    public BaseBuildingTool BaseBuildingTool;
    public Ant Ant;

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
