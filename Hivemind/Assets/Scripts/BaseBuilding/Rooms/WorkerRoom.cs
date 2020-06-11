class WorkerRoom : BaseUnitRoom
{
    public override RoomType GetRoomType()
    {
        return RoomType.WorkerRoom;
    }

    private void Awake()
    {
        UnitResource = "Prefabs/WorkerAnt"; 
    }
}
