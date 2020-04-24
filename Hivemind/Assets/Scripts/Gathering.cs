using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.AI;
using static ResourceNode;

public class Gathering : MonoBehaviour
{
    private enum State
    {
        Idle,
        Scouting,
        MovingToResource,
        Gathering,
        MovingToStorage,
    }
    public int CarryAmount = 3;
    public ResourceType PreferredResource = ResourceType.Unknown;

    private State state;
    private Dictionary<ResourceType, int> inventory;
    private int inventoryAmount;
    private int nextHarvest;
    private ResourceNode target;
    private Storage storage;
    private NavMeshAgent agent;
    private float baseSpeed;
    private List<GameObject> carryingObjects;

    private void Awake()
    {
        inventory = new Dictionary<ResourceType, int>();
        carryingObjects = new List<GameObject>();
        agent = gameObject.GetComponent<NavMeshAgent>();
        baseSpeed = agent.speed;
        storage = GameWorld.GetStorage();
        state = State.Idle;
    }

    private ResourceNode findResource()
    {
        ResourceNode resourceNode = GameWorld.FindNearestResource(transform.position, PreferredResource);
        if (PreferredResource != ResourceType.Unknown && resourceNode == null)
        {
            resourceNode = GameWorld.FindNearestResource(transform.position, ResourceType.Unknown);
        }
        return resourceNode;
    }

    private void carryResource(ResourceNode resource)
    {
        GameObject carryingObject = Instantiate(resource.gameObject, transform.position, Quaternion.identity, transform);
        carryingObject.GetComponent<ResourceNode>().ColorResource(30);
        Destroy(carryingObject.GetComponent<ResourceNode>());
        carryingObject.transform.localScale = new Vector3(carryingObject.transform.localScale.x * 3, carryingObject.transform.localScale.y * 3, carryingObject.transform.localScale.z * 3);
        carryingObjects.Add(carryingObject);
    }

    private void Update()
    {
        switch (state)
        {
            case State.Idle:
                target = findResource();
                if (target != null)
                {
                    nextHarvest = target.DecreaseFutureResources(CarryAmount - inventoryAmount);
                    agent.SetDestination(target.GetPosition());
                    state = State.MovingToResource;
                }
                break;
            case State.Scouting:
                break;
            case State.MovingToResource:
                if (Vector3.Distance(transform.position, target.GetPosition()) < 1f)
                {
                    state = State.Gathering;
                }
                break;
            case State.Gathering:
                if (!inventory.ContainsKey(target.resourceType))
                {
                    inventory.Add(target.resourceType, 1);
                }
                else
                {
                    inventory[target.resourceType]++;
                }
                inventoryAmount++;
                carryResource(target);
                target.GrabResource();
                agent.speed *= 0.9f;
                if (inventoryAmount >= CarryAmount)
                {
                    state = State.MovingToStorage;
                }
                else
                {
                    if (nextHarvest > 0)
                    {
                        nextHarvest--;
                        break;
                    }
                    target = findResource();
                    if (target != null)
                    {
                        target.DecreaseFutureResources(CarryAmount - inventoryAmount);
                        agent.SetDestination(target.GetPosition());
                        state = State.MovingToResource;
                    }
                    else
                    {
                        state = State.MovingToStorage;
                        break;
                    }
                }
                break;
            case State.MovingToStorage:
                if (storage != null)
                {
                    agent.SetDestination(storage.GetPosition());
                    for(int i = 0; i < carryingObjects.Count; i++)
                    {
                        carryingObjects[i].transform.position = new Vector3(transform.position.x, transform.position.y + 0.08f * (i + 1), transform.position.z);
                    }
                    if (Vector3.Distance(transform.position, storage.GetPosition()) < 2f)
                    {
                        GameResources.AddResources(inventory);
                        inventory.Clear();
                        inventoryAmount = 0;
                        foreach(GameObject gameObject in carryingObjects)
                        {
                            Destroy(gameObject);
                        }
                        carryingObjects.Clear();
                        agent.speed = baseSpeed;
                        state = State.Idle;
                    }
                }
                else
                {
                    storage = GameWorld.GetStorage();
                }
                break;
        }
    }
}
