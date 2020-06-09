public class Wall : BaseRoom
{
    public override RoomType GetRoomType()
    {
        return RoomType.Wall;
    }

    public override bool IsRoom()
    {
        return false;
    }

    public override bool IsDestructable()
    {
        return true;
    }

    public override void Destroy()
    {
        Destroy(gameObject);
    }
}
