﻿using Assets.Scripts;
using System;
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
    [SerializeField]
    private int TimeToRespawn = 30;
    public bool DestroyWhenEmpty = false;
    public bool IsKnown;
    public string Prefab;

    private int resourceAmount;

    private int futureResourceAmount;

    private void Awake()
    {
        resourceAmount = BaseResourceAmount;
        futureResourceAmount = resourceAmount;
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        IsKnown = false;
        GameWorld.AddNewResource(this);
    }

    private void Update()
    {
        if (CanRespawn && resourceAmount < BaseResourceAmount && !respawningResources)
        {
            StartCoroutine(respawnResource());
        }
    }

    public void Discover()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = true;
        IsKnown = true;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void Destroy()
    {
        Destroy(gameObject);
        Destroy(this);
        GameWorld.RemoveResource(this);
    }

    private IEnumerator respawnResource()
    {
        respawningResources = true;
        yield return new WaitForSeconds(TimeToRespawn);
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

    internal void IncreaseResourceAmount(int nextHarvest)
    {
        futureResourceAmount += nextHarvest;
    }

    public void GrabResource()
    {
        resourceAmount--;
        if (resourceAmount == 0 && DestroyWhenEmpty)
        {
            Destroy();
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

    public ResourceNodeData GetData()
    {
        ResourceNodeData data = new ResourceNodeData(IsKnown, respawningResources, BaseResourceAmount, CanRespawn, TimeToRespawn, DestroyWhenEmpty, resourceAmount, futureResourceAmount, gameObject.transform.position, gameObject.transform.localEulerAngles, Prefab, gameObject.transform.parent);
        return data;
    }

    public void SetData(ResourceNodeData data)
    {
        gameObject.SetActive(false);
        gameObject.transform.parent = data.Parent;
        respawningResources = data.RespawningResources;
        BaseResourceAmount = data.BaseResourceAmount;
        CanRespawn = data.CanRespawn;
        TimeToRespawn = data.TimeToRespawn;
        DestroyWhenEmpty = data.DestroyWhenEmpty;
        resourceAmount = data.ResourceAmount;
        futureResourceAmount = data.ResourceAmount;
        ColorResource(resourceAmount);
        IsKnown = data.IsKnown;
        gameObject.GetComponent<MeshRenderer>().enabled = data.IsKnown;
        gameObject.SetActive(true);
        gameObject.transform.localEulerAngles = new Vector3(data.RotationX, data.RotationY, data.RotationZ);
    }
}