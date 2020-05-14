﻿using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

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

    public int CarryAmount = 3;
    public ResourceType PreferredResource = ResourceType.Unknown;
    public bool IsScout = false;
    public Direction PrefferedDirection;

    private State state;
    private Dictionary<ResourceType, int> inventory;
    private int nextHarvest;
    private ResourceNode target;
    private Storage storage;
    private NavMeshAgent agent;
    private float baseSpeed;
    private List<GameObject> carryingObjects;
    private bool scouting = false;
    private bool preparingReturn = false;

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
        ResourceNode resourceNode = GameWorld.FindNearestKnownResource(transform.position, PreferredResource);
        if (PreferredResource != ResourceType.Unknown && resourceNode == null)
        {
            resourceNode = GameWorld.FindNearestKnownResource(transform.position, ResourceType.Unknown);
        }
        return resourceNode;
    }

    private void TargetResource()
    {
        target = findResource();
        if (target != null)
        {
            if (state == State.Idle)
            {
                agent.isStopped = false;
            }
            StopAllCoroutines();
            scouting = false;
            nextHarvest = target.DecreaseFutureResources(CarryAmount - carryingObjects.Count);
            agent.SetDestination(target.GetPosition());
            state = State.MovingToResource;
        }
        else if (IsScout)
        {
            agent.isStopped = false;
            state = State.Scouting;
        }
        else if (Vector3.Distance(transform.position, storage.GetPosition()) > 2f)
        {
            state = State.MovingToStorage;
        }
    }

    private void carryResource(ResourceNode resource)
    {
        GameObject carryingObject = Instantiate(resource.baseObject.gameObject, transform.position, Quaternion.identity, transform);
        carryingObject.transform.localScale = new Vector3(carryingObject.transform.localScale.x * 3, carryingObject.transform.localScale.y * 3, carryingObject.transform.localScale.z * 3);
        carryingObject.transform.position = new Vector3(transform.position.x, transform.position.y + 0.08f * (carryingObjects.Count + 1), transform.position.z);
        carryingObjects.Add(carryingObject);
    }

    private void Update()
    {
        switch (state)
        {
            case State.Idle:
                agent.isStopped = true;
                TargetResource();
                break;

            case State.Scouting:
                if (!preparingReturn)
                {
                    preparingReturn = true;
                    StartCoroutine(ReturnToBase());
                }
                if (!scouting)
                {
                    ResourceNode tempTarget = GameWorld.FindNearestUnknownResource(transform.position);
                    if (tempTarget != null && Vector3.Distance(transform.position, tempTarget.GetPosition()) < 2f)
                    {
                        target = tempTarget;
                        state = State.MovingToStorage;
                    }
                    else
                    {
                        scouting = true;
                        StartCoroutine(Scout());
                    }
                }
                break;

            case State.MovingToResource:
                if (target != null && target.IsKnown)
                {
                    if (Vector3.Distance(transform.position, target.GetPosition()) < 1f)
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
                agent.speed *= 0.9f;
                nextHarvest--;
                if (carryingObjects.Count >= CarryAmount)
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
                if (storage != null)
                {
                    agent.SetDestination(storage.GetPosition());
                    if (Vector3.Distance(transform.position, storage.GetPosition()) < 2f)
                    {
                        if (IsScout && target != null)
                        {
                            target.Discover();
                        }
                        if (inventory != null)
                        {
                            GameResources.AddResources(inventory);
                            inventory.Clear();
                            foreach (GameObject gameObject in carryingObjects)
                            {
                                Destroy(gameObject);
                            }
                            carryingObjects.Clear();
                            agent.speed = baseSpeed;
                        }
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

    private IEnumerator Scout()
    {
        Vector3 destination = new Vector3(transform.position.x + Random.Range(-5, 5), transform.position.y, transform.position.z + Random.Range(-5, 5));
        switch (PrefferedDirection)
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
        agent.SetDestination(destination);
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