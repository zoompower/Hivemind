using UnityEngine;

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
        transform.parent.GetComponentInParent<BaseController>().SetQueenRoom(this);
    }

    public override void Destroy()
    {
        Destroy(gameObject);
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public BaseTile GetBaseTile()
    {
        return GetComponentInParent<BaseTile>();
    }
}
