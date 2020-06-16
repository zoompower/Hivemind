using System.Collections.Generic;
using UnityEngine;

public class BaseSpatialPartition : MonoBehaviour
{
    private List<GameObject> Entities = new List<GameObject>();

    private void Start()
    {
        GetComponentInParent<BaseController>().RegisterSpatial(this);
    }

    private void OnTriggerEnter(Collider col)
    {
        Entities.Add(col.gameObject);
        if (col.GetComponent<Ant>())
        {
            col.GetComponent<Ant>().SpatialPositionId = int.MinValue;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        Entities.Remove(col.gameObject);
    }
    
    public List<GameObject> GetEntities()
    {
        return new List<GameObject>(Entities);
    }

    public bool HasEntity(Ant ant)
    {
        return Entities.Contains(ant.gameObject);
    }
}
