using Assets.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Gathering :  IAntBehaviour
{
    private enum State
    {
        Idle,
        Scouting,
        MovingToResource,
        Gathering,
        MovingToStorage,
    }

    private State state;
    private Dictionary<ResourceType, int> inventory;
    private int inventoryAmount;
    private int nextHarvest;
    private Ant ant;

    public List<GameObject> carryingObjects;

    private ResourceNode target;
     public void Initiate(Ant ant)
    {
        inventory = new Dictionary<ResourceType, int>();
        state = State.Idle;
        this.ant = ant;
        carryingObjects = new List<GameObject>();
    }
    private ResourceNode findResource(ResourceType PrefferedResource)
    {
        ResourceNode resourceNode = GameWorld.FindNearestResource(ant.transform.position, PrefferedResource);
        if (PrefferedResource != ResourceType.Unknown && resourceNode == null)
        {
            resourceNode = GameWorld.FindNearestResource(ant.transform.position, ResourceType.Unknown);
        }
        return resourceNode;
    }

    private void carryResource(ResourceNode resource)
    {
        GameObject carryingObject = GameObject.Instantiate(resource.baseObject.gameObject, ant.transform.position, Quaternion.identity, ant.transform);
        carryingObject.transform.localScale = new Vector3(carryingObject.transform.localScale.x * 3, carryingObject.transform.localScale.y * 3, carryingObject.transform.localScale.z * 3);
        carryingObjects.Add(carryingObject);
    }

    public void Execute()
    {
       
        switch (state)
        {
            case State.Idle:
                ant.GetAgent().isStopped = true;
                target = findResource(ant.GetResourceMind().GetPrefferedType());
                if (target != null)
                {
                    ant.GetAgent().isStopped = false;
                    nextHarvest = target.DecreaseFutureResources(ant.GetResourceMind().GetCarryWeight() - inventoryAmount);
                    ant.GetAgent().SetDestination(target.GetPosition());
                    state = State.MovingToResource;
                }
                break;
            case State.Scouting:
                break;
            case State.MovingToResource:
                if (Vector3.Distance(ant.transform.position, target.GetPosition()) < 1f)
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
                float speedPower =  (float) Math.Pow( 0.75,  inventoryAmount);
                ant.currentSpeed = ant.baseSpeed * speedPower;
                ant.UpdateSpeed();
                if (inventoryAmount >= ant.GetResourceMind().GetCarryWeight()) 
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
                    target = findResource(ant.GetResourceMind().GetPrefferedType());
                    if (target != null)
                    {
                        nextHarvest = target.DecreaseFutureResources(ant.GetResourceMind().GetCarryWeight()  - inventoryAmount);
                        ant.GetAgent().SetDestination(target.GetPosition());
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
                if (ant.GetStorage() != null)
                {
                    ant.GetAgent().SetDestination(ant.GetStorage().GetPosition());
                    for(int i = 0; i < carryingObjects.Count; i++)
                    {
                        carryingObjects[i].transform.position = new Vector3(ant.transform.position.x, ant.transform.position.y + 0.08f * (i + 1), ant.transform.position.z);
                    }
                    if (ant.AtBase())
                    {
                        GameResources.AddResources(inventory);
                        inventory.Clear();
                        inventoryAmount = 0;
                        foreach(GameObject gameObject in carryingObjects)
                        {
                            GameObject.Destroy(gameObject);
                        }
                        carryingObjects.Clear();
                        ant.currentSpeed = ant.baseSpeed;
                        ant.UpdateSpeed();
                        state = State.Idle;
                    }
                }
                else
                {
                    
                }
                break;
        }
    }

}
