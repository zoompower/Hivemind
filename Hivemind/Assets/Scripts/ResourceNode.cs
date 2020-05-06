using System.Collections;
using UnityEngine;

public enum ResourceType
{
    Rock,
    Crystal,
    Unknown,
}

public class ResourceNode : MonoBehaviour
{
    private Transform resourceNodeTransform;

    private GameObject myResourceNode;

    bool respawningResources = false;

    public GameObject baseObject;
    public int BaseResourceAmount = 4;
    public ResourceType resourceType = ResourceType.Unknown;

    private int resourceAmount;

    private int futureResourceAmount;

    private void Awake()
    {
        resourceAmount = BaseResourceAmount;
        futureResourceAmount = resourceAmount;
        myResourceNode = gameObject;
        resourceNodeTransform = gameObject.transform; GameWorld.AddNewResource(this);
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

    public void OnDestroy()
    {
        GameWorld.RemoveResource(this);
    }

    private IEnumerator respawnResource()
    {
        respawningResources = true;
        yield return new WaitForSeconds(30);
        resourceAmount++;
        futureResourceAmount++;
        ColorResource(resourceAmount);
        respawningResources = false;
    }

    public int DecreaseFutureResources(int amount)
    {
        futureResourceAmount -= amount;
        if(futureResourceAmount > -1)
        {
            return amount;
        }
        else
        {
            int newAmount = amount + futureResourceAmount;
            futureResourceAmount = 0;
            return newAmount;
        }
    }

    public void GrabResource()
    {
        resourceAmount--;
        if (resourceAmount == 0 && resourceType == ResourceType.Crystal)
        {
            myResourceNode.GetComponent<MeshRenderer>().enabled = false;
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

    public int GetResourcesFuture()
    {
        return futureResourceAmount;
    }
}
