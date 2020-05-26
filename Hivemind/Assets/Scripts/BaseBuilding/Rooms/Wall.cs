class Wall : BaseRoom
{
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
