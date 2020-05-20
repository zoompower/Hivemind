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

    internal bool AstarVisited = false;

    private void Awake()
    {
        FindAndAttachNeighbors();
        Astar.RegisterResetTile(this);
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
            SplitRoom();
            RoomScript = null;
        }
    }

    private void SplitRoom()
    {
        List<BaseTile> sameNeighborList = new List<BaseTile>();

        foreach (var neighbor in Neighbors)
        {
            BaseTile neighborTile = neighbor.GetComponent<BaseTile>();
            if (neighborTile.RoomScript != null && neighborTile.RoomScript.IsRoom() && neighborTile.RoomScript.GetType() == RoomScript.GetType())
            {
                sameNeighborList.Add(neighborTile);
            }
        }

        if (sameNeighborList.Count > 1 && sameNeighborList.Count < 5)
        {
            List<BaseTile> checkList = new List<BaseTile>(sameNeighborList);
            List<BaseTile> roomList = new List<BaseTile>();

            while (checkList.Count > 1)
            {
                roomList.Add(checkList[0]);

                List<BaseTile> swapList = new List<BaseTile>();
                for (int i = 1; i < checkList.Count; i++)
                {
                    if (!Astar.CanFind(checkList[0], checkList[i], new List<BaseTile>() { this }))
                    {
                        swapList.Add(checkList[i]);
                    }
                }
                checkList = swapList;
            }

            roomList.AddRange(checkList);

            if (roomList.Count > 1)
            {
                for (int i = 1; i < roomList.Count; i++)
                {
                    Debug.Log($"Create new room {roomList[i]}");
                }
            }
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

    private void OnDestroy()
    {
        Astar.RemoveResetTile(this);
    }
}
