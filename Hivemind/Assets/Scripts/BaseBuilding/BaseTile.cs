using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseTile : MonoBehaviour
{
    [SerializeField]
    private GameObject StartObject = null;
    internal GameObject CurrTile;
    internal BaseRoom RoomScript;

    [SerializeField]
    internal bool IsIndestructable = false;
    [SerializeField]
    private int DefaultRotation = -1;

    internal List<GameObject> Neighbors = new List<GameObject>();

    [SerializeField]
    private float CollisionSize = 0.50f;
    [SerializeField]
    private bool ShowDebugInfo = false;

    private void Start()
    {
        if (StartObject)
        {
            InitializeObject(StartObject);
        }

        FindAndAttachNeighbors();
    }

    internal void InitializeObject(GameObject gObj)
    {
        if (CurrTile != null || IsIndestructable) return;

        CurrTile = Instantiate(gObj);
        CurrTile.transform.SetParent(gameObject.transform, false);
        CurrTile.transform.localRotation = Quaternion.Euler(0, (DefaultRotation < 0) ? Random.Range(0, 5) * 60 : DefaultRotation, 0);

        RoomScript = CurrTile.GetComponent<BaseRoom>();
    }

    internal void DestroyRoom()
    {
        DestroyRoom(false);
    }

    internal void DestroyRoom(bool forced)
    {
        if ((!IsIndestructable && RoomScript.IsDestructable()) || forced)
        {
            Destroy(CurrTile);
            RoomScript = null;
        }
    }

    private void FindAndAttachNeighbors()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, CollisionSize);
        colliders = colliders.Where(c => c.gameObject.layer == gameObject.layer && c.gameObject != gameObject).ToArray();

        foreach (Collider hit in colliders)
        {
            Neighbors.Add(hit.gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        if (StartObject != null && StartObject.GetComponent<MeshFilter>() != null && true)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawMesh(StartObject.GetComponent<MeshFilter>().sharedMesh, transform.position, transform.rotation, transform.localScale);
        }

        if (!ShowDebugInfo) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, CollisionSize);
    }
}
