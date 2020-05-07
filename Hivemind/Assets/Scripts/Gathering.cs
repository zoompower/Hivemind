using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

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
    private Ant ant;

    public List<GameObject> carryingObjects;
    
    private bool scouting = false;
    private bool preparingReturn = false;

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
        ResourceNode resourceNode = GameWorld.FindNearestKnownResource(ant.transform.position, PrefferedResource);
        if (PrefferedResource != ResourceType.Unknown && resourceNode == null)
        {
            resourceNode = GameWorld.FindNearestKnownResource(ant.transform.position, ResourceType.Unknown);
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
        else if (state == State.Idle && IsScout)
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
        GameObject carryingObject = GameObject.Instantiate(resource.baseObject.gameObject, ant.transform.position, Quaternion.identity, ant.transform);
        carryingObject.transform.localScale = new Vector3(carryingObject.transform.localScale.x * 3, carryingObject.transform.localScale.y * 3, carryingObject.transform.localScale.z * 3);
        carryingObject.transform.position = new Vector3(transform.position.x, transform.position.y + 0.08f * (carryingObjects.Count + 1), transform.position.z);
        carryingObjects.Add(carryingObject);
    }

    public void Execute()
    {
       
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
                    StartCoroutine(ReturnToBase());
                }
                if (!scouting)
                {
                    ResourceNode tempTarget = GameWorld.FindNearestUnknownResource(transform.position, ResourceType.Unknown);
                    if (tempTarget != null)
                    {
                        if (Vector3.Distance(transform.position, tempTarget.GetPosition()) < 2f)
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
                float speedPower =  (float) Math.Pow( 0.75,  inventoryAmount);
                ant.currentSpeed = ant.baseSpeed * speedPower;
                ant.UpdateSpeed();
                
                nextHarvest--;
                if (carryingObjects.Count >= ant.GetResourceMind().GetCarryWeight())
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
                    agent.SetDestination(ant.GetStorage().GetPosition());
                    if (Vector3.Distance(ant.transform.position, storage.GetPosition()) < 2f)
                    {
                        if (IsScout && target != null)
                        {
                            target.AddToKnownResourceList();
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
