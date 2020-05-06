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
    private bool respawningResources = false;

    public GameObject baseObject;
    public int BaseResourceAmount = 4;
    public ResourceType resourceType = ResourceType.Unknown;
    public bool CanRespawn = false;
    public bool DestroyWhenEmpty = false;

    private int resourceAmount;

    private int futureResourceAmount;

    private void Awake()
    {
        resourceAmount = BaseResourceAmount;
        futureResourceAmount = resourceAmount;
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        GameWorld.AddNewResource(this);
    }

    public void AddToKnownResourceList()
    {
        if (!GameWorld.KnownResources.Contains(this) && gameObject != null)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
            GameWorld.AddNewKnownResource(this);
        }
    }

    private void Update()
    {
        if (CanRespawn && resourceAmount < BaseResourceAmount && !respawningResources)
        {
            StartCoroutine(respawnResource());
        }
    }

    public Vector3 GetPosition()
    {
        return transform.position;
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
        if (futureResourceAmount > -1)
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
        if (resourceAmount == 0 && DestroyWhenEmpty)
        {
            Destroy(gameObject);
        }
        if (resourceType == ResourceType.Rock)
        {
            ColorResource(resourceAmount);
        }
    }

    public void ColorResource(int amount)
    {
        MeshRenderer mesh = gameObject.GetComponent<MeshRenderer>();
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