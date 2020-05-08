using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatialPartitioning : MonoBehaviour
{
    public List<GameObject> Entities;
    private List<SpatialPartitioning> Neighbors;

    public int height;
    public int width;

    // Start is called before the first frame update
    void Start()
    {
        Entities = new List<GameObject>();
        Neighbors = new List<SpatialPartitioning>();
        SetNeighbors();
    }

    // Update is called once per frame
    void Update()
    {
        //TO DO, delete this when done DEBUGGING
        if(Entities.Count != 0)
        {
            Debug.ClearDeveloperConsole();
            Debug.Log(this.name+" Count: "+Entities.Count);
        }
    }

    private void OnTriggerEnter(Collider entity)
    {
        Entities.Add(entity.gameObject);
    }

    private void OnTriggerExit(Collider entity)
    {
        Entities.Remove(entity.gameObject);
    }

    public List<GameObject> GetEntities()
    {
        return Entities;
    }

    public List<GameObject> GetEntitiesWithNeigbors()
    {
        List<GameObject> EntitiesWithNeighbors = GetEntities();

        //Add entities of Neigboring SpatialPartitioning
        foreach (SpatialPartitioning neig in Neighbors)
        {
            foreach(GameObject ant in neig.Entities)
            {
                EntitiesWithNeighbors.Add(ant);
            }
        }
        return EntitiesWithNeighbors;
    }

    public List<GameObject> GetEntitiesWithExtraNeighbors(int steps)
    {
        List<GameObject> EntitiesWithExtraNeighbors = GetEntities();

        for (int i = -steps; i<steps;i++) {

            for (int j = -steps; j < steps; j++)
            {
                GameObject NeigGameObject = GameObject.Find("CollisionBox(" + (height + i) + "," + (width + j) + ")");
                if (NeigGameObject != null)
                {
                    SpatialPartitioning NeighCollider = NeigGameObject.GetComponent<SpatialPartitioning>();
                    Neighbors.Add(NeighCollider);
                }
            }
        }

        return EntitiesWithExtraNeighbors;
    }

    private void SetNeighbors()
    {
        int[,] cords = { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 }, { 1, 1 }, { 1, -1 }, { -1, 1 }, { -1, -1 } };

        for (int i = 0; i < cords.GetLength(0); i++)
        {
            GameObject NeigGameObject = GameObject.Find("CollisionBox(" + (height + cords[i,0]) + "," + (width + cords[i, 1]) + ")");
            if(NeigGameObject != null)
            {
                SpatialPartitioning NeighCollider = NeigGameObject.GetComponent<SpatialPartitioning>();
                Neighbors.Add(NeighCollider);
            }
        }
    }
}
