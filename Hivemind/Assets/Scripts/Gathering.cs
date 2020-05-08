﻿using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Gathering : IMind
{
    private enum State
    {
        Idle,
        Scouting,
        MovingToResource,
        Gathering,
        MovingToStorage,
    }

    public enum Direction
    {
        None,
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest,
    }

  

    private State state;
    private Dictionary<ResourceType, int> inventory;
    private int nextHarvest;

    public List<GameObject> carryingObjects;

    private ResourceType prefferedType;
    private int carryWeight;
    public Direction prefferedDirection;

    private bool scouting = false;
    private bool preparingReturn = false;

    private Ant ant;
    private ResourceNode target;
     public void Initiate()
    {
        inventory = new Dictionary<ResourceType, int>();
        state = State.Idle;
        carryingObjects = new List<GameObject>();
    }

    public Gathering(ResourceType resType, int carryweight, Direction exploreDirection)
    {
        prefferedType = resType;
        carryWeight = carryweight;
        prefferedDirection = exploreDirection;
        Debug.Log(carryweight);
        Debug.Log(resType);
    }

    private ResourceNode findResource(ResourceType PrefferedResource)
    {
        ResourceNode resourceNode = GameWorld.FindNearestKnownResource(ant.transform.position, PrefferedResource);
        if (PrefferedResource != ResourceType.Unknown && resourceNode == null)
        {
            resourceNode = GameWorld.FindNearestKnownResource(ant.transform.position, PrefferedResource);
        }
        return resourceNode;
    }

    private void TargetResource()
    {
        target = findResource(prefferedType);
        if (target != null)
        {
            if (state == State.Idle)
            {
                ant.GetAgent().isStopped = false;
            }
            ant.StopAllCoroutines();
            scouting = false;
            nextHarvest = target.DecreaseFutureResources(carryWeight - carryingObjects.Count);
            ant.GetAgent().SetDestination(target.GetPosition());
            state = State.MovingToResource;
        }
        else if (state == State.Idle && ant.IsScout)
        {
            ant.GetAgent().isStopped = false;
            state = State.Scouting;
        }
        else if (Vector3.Distance(ant.transform.position, ant.GetStorage().GetPosition()) > 2f)
        {
            state = State.MovingToStorage;
        }
    }

    private void carryResource(ResourceNode resource)
    {
        GameObject carryingObject = GameObject.Instantiate(resource.baseObject.gameObject, ant.transform.position, Quaternion.identity, ant.transform);
        carryingObject.transform.localScale = new Vector3(carryingObject.transform.localScale.x * 3, carryingObject.transform.localScale.y * 3, carryingObject.transform.localScale.z * 3);
        carryingObject.transform.position = new Vector3(ant.transform.position.x, ant.transform.position.y + 0.08f * (carryingObjects.Count + 1), ant.transform.position.z);
        carryingObjects.Add(carryingObject);
    }

    public void Execute(Ant ant)
    {
        this.ant = ant;
        switch (state)
        {
            case State.Idle:
                ant.GetAgent().isStopped = true;
               TargetResource();
                break;

            case State.Scouting:
                if (!preparingReturn)
                {
                    preparingReturn = true;
                    ant.StartCoroutine(ReturnToBase());
                }
                if (!scouting)
                {
                    ResourceNode tempTarget = GameWorld.FindNearestUnknownResource(ant.transform.position, prefferedType);
                    if (tempTarget != null)
                    {
                        if (Vector3.Distance(ant.transform.position, tempTarget.GetPosition()) < 2f)
                        {
                            target = tempTarget;
                            state = State.MovingToStorage;
                        }
                        else
                        {
                            scouting = true;
                            ant.StartCoroutine(Scout());
                        }
                    }
                }
                break;

            case State.MovingToResource:
                if (target != null)
                {
                    if (Vector3.Distance(ant.transform.position, target.GetPosition()) < 1f)
                    {
                        state = State.Gathering;
                    }
                }
                else
                {
                    TargetResource();
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
                carryResource(target);
                target.GrabResource();

                //calculate new speed
                float speedPower =  (float) Math.Pow( 0.75, carryingObjects.Count);
                ant.currentSpeed = ant.baseSpeed * speedPower;

                ant.UpdateSpeed();
                
                nextHarvest--;
                if (carryingObjects.Count >= carryWeight)
                {
                    state = State.MovingToStorage;
                }
                else
                {
                    if (nextHarvest > 0)
                    {
                        break;
                    }
                    TargetResource();
                }
                break;

            case State.MovingToStorage:
                if (ant.GetStorage() != null)
                {
                    ant.GetAgent().SetDestination(ant.GetStorage().GetPosition());
                    if (Vector3.Distance(ant.transform.position, ant.GetStorage().GetPosition()) < 2f)
                    {
                        if (ant.IsScout && target != null)
                        {
                            target.AddToKnownResourceList();
                        }
                        if (inventory != null)
                        {
                            GameResources.AddResources(inventory);
                            inventory.Clear();
                            foreach (GameObject gameObject in carryingObjects)
                            {
                                GameObject.Destroy(gameObject);
                            }
                            carryingObjects.Clear();
                            ant.currentSpeed = ant.baseSpeed;
                        ant.UpdateSpeed();
                        }
                        state = State.Idle;
                    }
                }
                else
                {
                    
                }
                break;
        }
    }

    private IEnumerator Scout()
    {
        Vector3 destination = new Vector3(ant.transform.position.x + Random.Range(-5, 5), ant.transform.position.y, ant.transform.position.z + Random.Range(-5, 5));
        switch (prefferedDirection)
        {
            case Direction.None:
                break;

            case Direction.North:
                destination.z += 2;
                break;

            case Direction.West:
                destination.x -= 2;
                break;

            case Direction.South:
                destination.z -= 2;
                break;

            case Direction.East:
                destination.x += 2;
                break;

            case Direction.NorthWest:
                destination.z += 2;
                destination.x -= 2;
                break;

            case Direction.NorthEast:
                destination.z += 2;
                destination.x += 2;
                break;

            case Direction.SouthEast:
                destination.z -= 2;
                destination.x += 2;
                break;

            case Direction.SouthWest:
                destination.z -= 2;
                destination.x -= 2;
                break;
        }
        ant.GetAgent().SetDestination(destination);
        yield return new WaitForSeconds(Random.Range(1, 3));
        scouting = false;
    }

    private IEnumerator ReturnToBase()
    {
        yield return new WaitForSeconds(Random.Range(30, 60));
        target = null;
        state = State.MovingToStorage;
        preparingReturn = false;
    }
}
