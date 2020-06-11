using System;
using System.Collections.Generic;
using UnityEngine;

public class SpatialPartitioning : MonoBehaviour
{
    public List<GameObject> Entities;
    public List<SpatialPartitioning> Neighbors;
    public List<SpatialPartitioning> ExtraNeighbors;

    public int Id;
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
        //TO DO: delete this when done DEBUGGING
        
        if(Entities.Count != 0)
        {
            Debug.Log(this.name+" Count: "+Entities.Count);
        }
    }

    private void OnTriggerEnter(Collider entity)
    {
        Entities.Add(entity.gameObject);
        if (entity.GetComponent<Ant>())
        {
            entity.GetComponent<Ant>().SpatialPositionId = Id;
        }
    }

    private void OnTriggerExit(Collider entity)
    {
        Remove(entity.gameObject);
    }

    public List<GameObject> GetEntitiesWithNeigbors()
    {
        //TO DO: Check the code while in practice, optimize for actual use
        List<GameObject> EntitiesWithNeighbors = new List<GameObject>(Entities);

        List<SpatialPartitioning> Test;

        if (ExtraNeighbors.Count > 0)
        {
            Test = ExtraNeighbors;
        }
        else
        {
            Test = Neighbors;
        }

        //Add entities of Neigboring SpatialPartitioning
        foreach (SpatialPartitioning neig in Test)
        {
                foreach (GameObject ant in neig.Entities)
                {
                    if (ant)
                    {
                        EntitiesWithNeighbors.Add(ant);
                    }
                }
        }
        return EntitiesWithNeighbors;
    }

    public List<GameObject> GetEntitiesWithExtraNeighbors(int steps)
    {
        List<GameObject> EntitiesWithExtraNeighbors;

        for (int i = -steps; i <= steps; i++)
        {
            for (int j = -steps; j <= steps; j++)
            {
                var Testing = transform.parent.Find($"CollisionBox({ height + i},{ width + j})");

                if (Testing != null && Testing.name != this.name)
                {
                    SpatialPartitioning NeigGameObject = Testing.GetComponent<SpatialPartitioning>();
                    ExtraNeighbors.Add(NeigGameObject);
                }
            }
        }
        EntitiesWithExtraNeighbors = GetEntitiesWithNeigbors();
        ExtraNeighbors.Clear();

        return EntitiesWithExtraNeighbors;
    }

    private void SetNeighbors()
    {
        int[,] cords = { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 }, { 1, 1 }, { 1, -1 }, { -1, 1 }, { -1, -1 } };

        for (int i = 0; i < cords.GetLength(0); i++)
        {
            GameObject NeigGameObject = GameObject.Find($"CollisionBox({height + cords[i,0]},{width + cords[i, 1]})");
            if(NeigGameObject != null)
            {
                SpatialPartitioning NeighCollider = NeigGameObject.GetComponent<SpatialPartitioning>();
                Neighbors.Add(NeighCollider);
            }
        }
    }

    internal void Remove(GameObject gameObject)
    {
        Entities.Remove(gameObject);
    }
}
