using UnityEngine;

class Wall : MonoBehaviour, IBaseRoom
{
    public bool IsRoom()
    {
        return false;
    }
}
