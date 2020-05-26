using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class Gathering : IMind
{
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
        NorthWest
    }

    public enum State
    {
        Idle,
        Scouting,
        MovingToResource,
        Gathering,
        MovingToStorage
    }

    private Ant ant;

    private List<GameObject> carryingObjects;


    private Dictionary<ResourceType, int> inventory;
    public bool IsScout;
    private int nextHarvest;
    private bool preparingReturn;
    private bool scouting;
    private ResourceNode target;

    private bool leavingBase = false;

    [SerializeField]
    private Vector3 TeleporterExit = new Vector3(4.231f, 0, 8.612f);

    public Gathering(ResourceType resType, int carryweight, Direction exploreDirection, bool isScout = false)
    {
        prefferedType = resType;
        carryWeight = carryweight;
        prefferedDirection = exploreDirection;
        IsScout = isScout;
    }

    public ResourceType prefferedType { get; set; }
    public int carryWeight { get; set; }
    public Direction prefferedDirection { get; set; }

    public void Initiate()
    {
        inventory = new Dictionary<ResourceType, int>();
        carryingObjects = new List<GameObject>();
    }

    public void Execute(Ant ant)
    {
        if (leavingBase) return;

        this.ant = ant;
        switch (ant.state)
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
                    var tempTarget = GameWorld.FindNearestUnknownResource(ant.transform.position);
                    if (tempTarget != null && Vector3.Distance(ant.transform.position, tempTarget.GetPosition()) < 2f)
                    {
                        target = tempTarget;
                        ant.state = State.MovingToStorage;
                        ant.GetAgent().SetDestination(ant.GetStorage().GetPosition());
                        ant.StartCoroutine(Discover());
                        preparingReturn = false;
                    }
                    else
                    {
                        scouting = true;
                        ant.StartCoroutine(Scout());
                    }
                }

                break;

            case State.MovingToResource:
                if (target != null)
                {
                    if (Vector3.Distance(ant.transform.position, target.GetPosition()) < 2f)
                        ant.state = State.Gathering;
                }
                else
                {
                    TargetResource();
                }

                break;

            case State.Gathering:
                if (!inventory.ContainsKey(target.resourceType))
                    inventory.Add(target.resourceType, 1);
                else
                    inventory[target.resourceType]++;
                carryResource(target);
                target.GrabResource();

                //calculate new speed
                var speedPower = (float)Math.Pow(0.75, carryingObjects.Count);
                ant.currentSpeed = ant.baseSpeed * speedPower;

                ant.UpdateSpeed();

                nextHarvest--;
                if (carryingObjects.Count >= carryWeight)
                {
                    ant.state = State.MovingToStorage;
                    ant.GetAgent().SetDestination(ant.GetStorage().GetPosition());
                }
                else
                {
                    if (nextHarvest > 0) break;
                    target = null;
                    TargetResource();
                }

                break;

            case State.MovingToStorage:
                if (ant.GetStorage() != null)
                {
                    if (ant.AtBase())
                    {
                        if (IsScout && target != null && !target.knownResource())
                        {
                            target.AddToKnownResourceList();
                        }
                        if (inventory != null)
                        {
                            GameResources.AddResources(inventory);
                            inventory.Clear();
                            foreach (var gameObject in carryingObjects) Object.Destroy(gameObject);
                            carryingObjects.Clear();
                            ant.currentSpeed = ant.baseSpeed;
                            ant.UpdateSpeed();
                        }
                        ant.finishedTask = true;
                        ant.state = State.Idle;
                    }
                }
                else
                {
                    ant.SetStorage(GameWorld.GetStorage());
                }

                break;
        }
    }

    public double Likelihood(Ant ant)
    {
        return 50;
    }

    private IEnumerator Discover()
    {
        ant.PlaySoundDiscovery();

        var excla = (GameObject)GameObject.Instantiate(Resources.Load("ExclamationMark"), ant.transform, false);
        excla.transform.localScale *= 3;

        yield return new WaitForSeconds(0.8f);
        UnityEngine.GameObject.Destroy(excla.gameObject);
    }

    public IMind Clone()
    {
        var clone = new Gathering(prefferedType, carryWeight, prefferedDirection);
        clone.Initiate();
        return clone;
    }

    public void Update(IMind mind)
    {
        var gathering = mind as Gathering;
        if (gathering != null)
        {
            prefferedType = gathering.prefferedType;
            carryWeight = gathering.carryWeight;
            prefferedDirection = gathering.prefferedDirection;
            IsScout = gathering.IsScout;
        }
    }

    public bool Equals(IMind mind)
    {
        var gathering = mind as Gathering;
        if (gathering != null)
            if (gathering.prefferedType == prefferedType
                && gathering.carryWeight == carryWeight
                && gathering.prefferedDirection == prefferedDirection
                && gathering.IsScout == IsScout)
                return true;
        return false;
    }

    public void GenerateUI()
    {
        throw new NotImplementedException();
    }

    private ResourceNode findResource()
    {
        var resourceNode = GameWorld.FindNearestKnownResource((ant.AtBase()) ? TeleporterExit : ant.transform.position, prefferedType);
        if (prefferedType != ResourceType.Unknown && resourceNode == null)
            resourceNode = GameWorld.FindNearestKnownResource((ant.AtBase()) ? TeleporterExit : ant.transform.position, ResourceType.Unknown);
        return resourceNode;
    }

    private void TargetResource()
    {
        if (target != null)
        {
            target.IncreaseResourceAmount(nextHarvest);
        }

        target = findResource();

        if (target != null)
        {
            if (ant.state == State.Idle) ant.GetAgent().isStopped = false;
            ant.StopCoroutine(Scout());
            ant.StopCoroutine(ReturnToBase());
            scouting = false;
            nextHarvest = target.DecreaseFutureResources(carryWeight - carryingObjects.Count);
            ant.GetAgent().SetDestination(target.GetPosition());
            ant.state = State.MovingToResource;

            ant.finishedTask = false;
        }
        else if (ant.state == State.Idle && IsScout)
        {
            ant.GetAgent().isStopped = false;

            ant.finishedTask = false;
            ant.StartCoroutine(ExitBase(State.Scouting));
        }
        else if (Vector3.Distance(ant.transform.position, ant.GetStorage().GetPosition()) > 2f)
        {
            ant.state = State.MovingToStorage;
            ant.GetAgent().SetDestination(ant.GetStorage().GetPosition());
        }
    }

    private IEnumerator ExitBase(State nextState)
    {
        leavingBase = true;
        ant.GetAgent().SetDestination(TeleporterExit);
        yield return new WaitUntil(() => !ant.AtBase());
        ant.state = nextState;
        leavingBase = false;
    }

    private void carryResource(ResourceNode resource)
    {
        var carryingObject = Object.Instantiate(resource.baseObject.gameObject, ant.transform.position,
            Quaternion.identity, ant.transform);
        carryingObject.transform.localScale = new Vector3(carryingObject.transform.localScale.x * 3,
            carryingObject.transform.localScale.y * 3, carryingObject.transform.localScale.z * 3);
        carryingObject.transform.position = new Vector3(ant.transform.position.x,
            ant.transform.position.y + 0.08f * (carryingObjects.Count + 1), ant.transform.position.z);
        carryingObjects.Add(carryingObject);
    }

    private IEnumerator Scout()
    {
        var destination = new Vector3(ant.transform.position.x + Random.Range(-5, 5), ant.transform.position.y,
            ant.transform.position.z + Random.Range(-5, 5));
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
        yield return new WaitForSeconds(Random.Range(1,3));
        scouting = false;
    }

    private IEnumerator ReturnToBase()
    {
        yield return new WaitForSeconds(Random.Range(30, 40));
        if(ant.state != State.MovingToStorage)
        {
        target = null;
        ant.state = State.MovingToStorage;
        ant.GetAgent().SetDestination(ant.GetStorage().GetPosition());
        preparingReturn = false;
        }
    }
}