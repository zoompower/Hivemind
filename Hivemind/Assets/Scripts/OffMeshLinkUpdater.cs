using UnityEngine;
using UnityEngine.AI;

public class OffMeshLinkUpdater : MonoBehaviour
{
    private OffMeshLink link;

    void Start()
    {
        link = GetComponent<OffMeshLink>();
        link.UpdatePositions();
        InvokeRepeating("UpdateLinkPositions", 1.0f, 1.0f);
    }

    private void UpdateLinkPositions()
    {
        link.UpdatePositions();
    }

    private void OnDestroy()
    {
        CancelInvoke("UpdateLinkPositions");
    }
}
