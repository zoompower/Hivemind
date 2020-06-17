using System;

public class ToolChangedEventArgs : EventArgs
{
    public BaseBuildingTool oldTool;
    public BaseBuildingTool newTool;

    public ToolChangedEventArgs(BaseBuildingTool oldTool, BaseBuildingTool newTool)
    {
        this.oldTool = oldTool;
        this.newTool = newTool;
    }
}