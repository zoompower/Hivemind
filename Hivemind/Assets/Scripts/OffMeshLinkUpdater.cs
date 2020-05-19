using UnityEngine;
using UnityEngine.AI;

public class OffMeshLinkUpdater : MonoBehaviour
{
    [SerializeField]
    private int iterations = 200;
    void Start()
    {
        OffMeshLink link = GetComponent<OffMeshLink>();
        link.UpdatePositions();
    }

    void Update()
    {
        OffMeshLink link = GetComponent<OffMeshLink>();
        link.UpdatePositions();
        if (iterations <= 0)
        {
            Destroy(this);
        }else
        {
            iterations--;
        }
    }
}
