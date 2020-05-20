using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseTile : MonoBehaviour
{
    [SerializeField]
    private GameObject StartObject = null;
    internal GameObject CurrTile;
    internal BaseRoom RoomScript;

    internal List<GameObject> Neighbors = new List<GameObject>();

    [SerializeField]
    internal bool IsIndestructable = false;
    [SerializeField]
    private int DefaultRotation = -1;

    [SerializeField]
    private float CollisionSize = 0.50f;
    [SerializeField]
    private bool ShowDebugInfo = false;
    [SerializeField]
    private bool ShowMeshPreview = false;
    [SerializeField]
    private Color MeshColor = new Color(53.0f / 255.0f, 124.0f / 255.0f, 44.0f / 255.0f);

    private void Awake()
    {
        FindAndAttachNeighbors();
    }

    private void Start()
    {
        if (StartObject)
        {
            InitializeObject(StartObject);
        }
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
        if (CurrTile == null) return;

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
        if (StartObject != null && StartObject.GetComponent<MeshFilter>() != null && ShowMeshPreview)
        {
            Gizmos.color = MeshColor;
            Gizmos.DrawWireMesh(StartObject.GetComponent<MeshFilter>().sharedMesh, transform.position + StartObject.transform.position, transform.rotation, transform.localScale);
        }

        if (ShowDebugInfo)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, CollisionSize);
        }
    }
}
