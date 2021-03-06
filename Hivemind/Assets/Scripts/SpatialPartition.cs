﻿using UnityEngine;

public class SpatialPartition : MonoBehaviour
{

    [SerializeField]
    private GameObject obj;

    [SerializeField]
    private int width;
    [SerializeField]
    private int depth;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < depth; j++)
            {
                GameObject go = Instantiate(obj, transform, false);
                go.transform.position = new Vector3(
                    (0.0f - transform.localScale.x / 2.0f) + ((transform.localScale.x / width) / 2.0f) + (transform.localScale.x / width) * i,
                    0,
                    (0.0f - transform.localScale.z / 2.0f) + ((transform.localScale.z / depth) / 2.0f) + (transform.localScale.z / depth) * j
                    ) + transform.position;
                go.transform.localScale = new Vector3((go.transform.localScale.x / width), 1, (go.transform.localScale.z / depth));
                go.name = "CollisionBox(" + i + "," + j + ")";
                go.transform.Find("SpatialPartitioning");
                go.GetComponent<SpatialPartitioning>().width = j;
                go.GetComponent<SpatialPartitioning>().height = i;
            }
        }
    }
}
