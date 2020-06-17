using System.Collections.Generic;

internal class WorkerRoom : BaseUnitRoom
{
    public override RoomType GetRoomType()
    {
        return RoomType.WorkerRoom;
    }

    private void Awake()
    {
        UnitResource = "Prefabs/WorkerAnt";
        RespawnCost = new Dictionary<ResourceType, int>() { { ResourceType.Food, 1 } };
        DefaultRespawnTime = 30;
    }
}