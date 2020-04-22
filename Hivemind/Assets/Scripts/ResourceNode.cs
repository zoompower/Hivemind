using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceNode
{
    private Transform resourceNodeTransform;

    private GameObject myResourceNode;

    private int resourceAmount;
    public ResourceNode(Transform resourceNodeTransform, GameObject myResourceNode, int resourceAmount = 4)
    {
        this.resourceNodeTransform = resourceNodeTransform;
        this.myResourceNode = myResourceNode;
        this.resourceAmount = resourceAmount;
        ColorResource(resourceAmount);
    }

    public Vector3 GetPosition()
    {
        return resourceNodeTransform.position;
    }

    public void GrabResource()
    {
        resourceAmount--;
        ColorResource(resourceAmount);
    }

    public void ColorResource(int amount)
    {
        MeshRenderer mesh = myResourceNode.GetComponent<MeshRenderer>();
        switch (amount)
        {
            case 0:
                mesh.material.SetColor("_Color", Color.black);
                break;
            case 1:
                mesh.material.SetColor("_Color", Color.gray);
                break;
            case 2:
                mesh.material.SetColor("_Color", Color.red);
                break;
            case 3:
                mesh.material.SetColor("_Color", Color.yellow);
                break;
            case 4:
                mesh.material.SetColor("_Color", Color.blue);
                break;
            case 5:
                mesh.material.SetColor("_Color", Color.green);
                break;
        }
    }

    public bool HasResources()
    {
        return resourceAmount > 0;
    }
}
