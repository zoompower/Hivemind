using UnityEngine.Tilemaps;

class QueenRoom : BaseRoom
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
        foreach(var TileObject in transform.parent.GetComponent<BaseTile>().Neighbors)
        {
            BaseTile tileScript = TileObject.GetComponent<BaseTile>();

            tileScript.IsIndestructable = true;
            tileScript.IsUnbuildable = true;
            tileScript.DestroyRoom(true);
        }
    }
}
