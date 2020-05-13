using System;
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
    internal bool isIndestructable = false;

    private List<GameObject> neighbors = new List<GameObject>();

    [SerializeField]
    private float CollisionSize = 0.50f;

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
        CurrTile = Instantiate(gObj);
        CurrTile.transform.SetParent(gameObject.transform, false);
        RoomScript = CurrTile.GetComponent<BaseRoom>();
    }

    internal void DestroyRoom()
    {
        if (!isIndestructable)
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
            neighbors.Add(hit.gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, CollisionSize);
    }
}
