using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatialPartitioning : MonoBehaviour
{
    public List<GameObject> Entities;
    private List<Collider> Neighbors;

    public int height;
    public int width;

    // Start is called before the first frame update
    void Start()
    {
        Entities = new List<GameObject>();
        Neighbors = new List<Collider>();
        SetNeighbors();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider entity)
    {
        Entities.Add(entity.gameObject);
    }

    private void OnTriggerExit(Collider entity)
    {
        Entities.Remove(entity.gameObject);
    }

    private void SetNeighbors()
    {
        /*var left = width+1;
        var right = width-1;
        var top = height+1;
        var bottom = height-1;

        var topLeft = width+1 + height+1;
        var topRight = width-1 + height+1;
        var bottomLeft = width+1 + height-1;
        var bottomRight = width-1 + height-1; */

        var left =          GameObject.Find("CollisionBox(" + (height + 0) + "," + (width + 1) + ")");
        var right =         GameObject.Find("CollisionBox(" + (height + 0) + "," + (width - 1) + ")");
        var top =           GameObject.Find("CollisionBox(" + (height + 1) + "," + (width + 0) + ")");
        var bottom =        GameObject.Find("CollisionBox(" + (height - 1) + "," + (width + 0) + ")");

        var topLeft =       GameObject.Find("CollisionBox(" + (height + 1) + "," + (width + 1) + ")");
        var topRight =      GameObject.Find("CollisionBox(" + (height + 1) + "," + (width - 1) + ")");
        var bottomLeft =    GameObject.Find("CollisionBox(" + (height - 1) + "," + (width + 1) + ")");
        var bottomRight =   GameObject.Find("CollisionBox(" + (height - 1) + "," + (width - 1) + ")");
    }
}
