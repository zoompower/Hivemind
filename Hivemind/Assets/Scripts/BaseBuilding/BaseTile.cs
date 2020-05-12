using UnityEngine;

public class BaseTile : MonoBehaviour
{
    [SerializeField]
    private GameObject StartObject = null;
    private GameObject CurrTile;

    [SerializeField]
    private bool isIndestructable = false;

    private void Start()
    {
        if (StartObject)
        {
            CurrTile = Instantiate(StartObject);
            CurrTile.transform.SetParent(gameObject.transform, false);
        }
    }

    public void OnLeftClick(BaseBuildingTool tool)
    {
        if (CurrTile == null) return;

        if (!isIndestructable && true /* is wall */)
        {
            Destroy(CurrTile);
        }
    }

    public void OnRightClick(BaseBuildingTool tool)
    {
        if (CurrTile != null) return;

        if (!isIndestructable && tool == BaseBuildingTool.Wall)
        {
            CurrTile = Instantiate(StartObject);
            CurrTile.transform.SetParent(gameObject.transform, false);
        }
    }
}
