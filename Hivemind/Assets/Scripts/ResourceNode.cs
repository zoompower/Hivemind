using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    public enum ResourceType
    {
        Rock,
        Crystal,
        Unknown,
    }

    private Transform resourceNodeTransform;

    private GameObject myResourceNode;

    bool respawningResources = false;

    public int BaseResourceAmount = 4;
    public ResourceType resourceType = ResourceType.Unknown;

    private int resourceAmount;

    private void Awake()
    {
        resourceAmount = BaseResourceAmount;
        myResourceNode = gameObject;
        resourceNodeTransform = gameObject.transform;
        GameWorld.AddNewResource(this);
    }

    private void Update()
    {
        if (resourceAmount < BaseResourceAmount && !respawningResources)
        {
            StartCoroutine(respawnResource());
        }
    }

    public Vector3 GetPosition()
    {
        return resourceNodeTransform.position;
    }

    private IEnumerator respawnResource()
    {
        respawningResources = true;
        yield return new WaitForSeconds(30);
        resourceAmount++;
        ColorResource(resourceAmount);
        respawningResources = false;
    }

    public void GrabResource()
    {
        resourceAmount--;
        if (resourceAmount == 0 && resourceType == ResourceType.Crystal)
        {
            GameWorld.RemoveResource(this);
            Destroy(gameObject);
        }
        if (resourceType == ResourceType.Rock)
        {
            ColorResource(resourceAmount);
        }
    }

    public void ColorResource(int amount)
    {
        MeshRenderer mesh = myResourceNode.GetComponent<MeshRenderer>();
        float amountLeft = (float)amount / (float)BaseResourceAmount;
        mesh.material.SetColor("_Color", new Color(amountLeft, amountLeft, amountLeft));
    }

    public bool HasResources()
    {
        return resourceAmount > 0;
    }
}
