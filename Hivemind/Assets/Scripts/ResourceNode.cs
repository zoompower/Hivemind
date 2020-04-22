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

    public IEnumerator respawnResource()
    {
        yield return new WaitForSeconds(10);
        resourceAmount++;
        ColorResource(resourceAmount);
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
                mesh.material.SetColor("_Color", new Color(0f, 0f, 0f));
                break;
            case 1:
                mesh.material.SetColor("_Color", new Color(0.25f, 0.25f, 0.25f));
                break;
            case 2:
                mesh.material.SetColor("_Color", new Color(0.5f, 0.5f, 0.5f));
                break;
            case 3:
                mesh.material.SetColor("_Color", new Color(0.75f, 0.75f, 0.75f));
                break;
            case 4:
                mesh.material.SetColor("_Color", new Color(1f, 1f, 1f));
                break;
        }
    }

    public bool HasResources()
    {
        return resourceAmount > 0;
    }
}
