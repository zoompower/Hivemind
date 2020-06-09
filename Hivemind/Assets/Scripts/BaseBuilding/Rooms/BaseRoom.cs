using UnityEngine;

public abstract class BaseRoom : MonoBehaviour
{
    public GameObject HighlightPrefab;

    protected UnitController unitController;

    public abstract RoomType GetRoomType();

    public abstract bool IsRoom();

    public abstract bool IsDestructable();

    public abstract void Destroy();
}
