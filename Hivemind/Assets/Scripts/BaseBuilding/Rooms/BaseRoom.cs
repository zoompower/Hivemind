using UnityEngine;

public abstract class BaseRoom : MonoBehaviour
{
    protected UnitController unitController;

    public abstract bool IsRoom();

    public abstract bool IsDestructable();

    public abstract void Destroy();
}
