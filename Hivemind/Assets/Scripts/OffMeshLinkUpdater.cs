using UnityEngine;
using UnityEngine.AI;

public class OffMeshLinkUpdater : MonoBehaviour
{
    private OffMeshLink link;

    private void Start()
    {
        link = GetComponent<OffMeshLink>();
        link.UpdatePositions();
        InvokeRepeating("UpdateLinkPositions", 0f, 0.5f);
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