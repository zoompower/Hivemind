public class QueenRoom : BaseRoom
{
    public override bool IsRoom()
    {
        return true;
    }

    public override bool IsDestructable()
    {
        return false;
    }

    private void Start()
    {
        foreach(var baseTile in GetComponentInParent<BaseTile>().Neighbors)
        {
            baseTile.IsIndestructable = true;
            baseTile.IsUnbuildable = true;
            baseTile.DestroyRoom(true);
        }
        transform.parent.GetComponentInParent<BaseController>().QueenRoom = this;
    }

    public override void Destroy()
    {
        Destroy(gameObject);
    }
}
