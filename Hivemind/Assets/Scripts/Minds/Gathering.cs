using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Assets.Scripts;
using Assets.Scripts.Data;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

[Serializable]
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

    private List<GameObject> carryingObjects = new List<GameObject>();
    private List<string> gatheredResources = new List<string>();

    private Dictionary<ResourceType, int> inventory = new Dictionary<ResourceType, int>();
    public bool IsScout;
    private int nextHarvest;
    private bool preparingReturn;
    private bool scouting;
    private ResourceNode target;
    private Vector3 scoutingDestination;
    private float scoutSeconds;
    private float returnSeconds;

    public Gathering(ResourceType resType, int carryweight, Direction exploreDirection, bool isScout = true)
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
    }

    public void Execute(Ant ant)
    {
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
                        ant.StartCoroutine(Discover());
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
                    if (Vector3.Distance(ant.transform.position, target.GetPosition()) < 1f)
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
                    ant.GetAgent().SetDestination(ant.GetStorage().GetPosition());
                    if (ant.AtBase())
                    {
                        if (IsScout && target != null) target.Discover();
                        if (inventory != null)
                        {
                            GameResources.AddResources(inventory);
                            inventory.Clear();
                            foreach (var gameObject in carryingObjects) Object.Destroy(gameObject);
                            carryingObjects.Clear();
                            ant.currentSpeed = ant.baseSpeed;
                            ant.UpdateSpeed();
                        }

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
        var resourceNode = GameWorld.FindNearestKnownResource(ant.transform.position, prefferedType);
        if (prefferedType != ResourceType.Unknown && resourceNode == null)
            resourceNode = GameWorld.FindNearestKnownResource(ant.transform.position, ResourceType.Unknown);
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
        }
        else if (ant.state == State.Idle && IsScout)
        {
            ant.GetAgent().isStopped = false;
            ant.state = State.Scouting;
        }
        else if (Vector3.Distance(ant.transform.position, ant.GetStorage().GetPosition()) > 2f)
        {
            ant.state = State.MovingToStorage;
        }
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
        gatheredResources.Add(resource.myGuid.ToString());
    }

    private IEnumerator Scout(float seconds = -1f, Vector3 scoutingDestination = new Vector3())
    {
        if (seconds < 0f)
        {
            scoutSeconds = Random.Range(1, 3);
        }
        if (scoutingDestination == new Vector3())
        {
            scoutingDestination = new Vector3(ant.transform.position.x + Random.Range(-5, 5), ant.transform.position.y, ant.transform.position.z + Random.Range(-5, 5));
            switch (prefferedDirection)
            {
                case Direction.None:
                    break;

                case Direction.North:
                    scoutingDestination.z += 2;
                    break;

                case Direction.West:
                    scoutingDestination.x -= 2;
                    break;

                case Direction.South:
                    scoutingDestination.z -= 2;
                    break;

                case Direction.East:
                    scoutingDestination.x += 2;
                    break;

                case Direction.NorthWest:
                    scoutingDestination.z += 2;
                    scoutingDestination.x -= 2;
                    break;

                case Direction.NorthEast:
                    scoutingDestination.z += 2;
                    scoutingDestination.x += 2;
                    break;

                case Direction.SouthEast:
                    scoutingDestination.z -= 2;
                    scoutingDestination.x += 2;
                    break;

                case Direction.SouthWest:
                    scoutingDestination.z -= 2;
                    scoutingDestination.x -= 2;
                    break;
            }
        }
        this.scoutingDestination = scoutingDestination;
        ant.GetAgent().SetDestination(scoutingDestination);
        while(scoutSeconds > 0f)
        {
            yield return new WaitForSeconds(0.1f);
            scoutSeconds -= 0.1f;
        }
        scouting = false;
    }

    private IEnumerator ReturnToBase(float seconds = -1f)
    {
        if (seconds < 0f)
        {
            returnSeconds = Random.Range(30, 40);
        }
        while (returnSeconds > 0f)
        {
            yield return new WaitForSeconds(0.1f);
            returnSeconds -= 0.1f;
        }
        target = null;
        ant.state = State.MovingToStorage;
        preparingReturn = false;
    }

    public MindData GetData()
    {
        return new GatheringData(ant, gatheredResources, inventory, IsScout, nextHarvest, preparingReturn, scouting, target, prefferedType, carryWeight, prefferedDirection, scoutingDestination, scoutSeconds, returnSeconds);
    }

    public void SetData(MindData mindData)
    {
        var data = mindData as GatheringData;
        ant = GameWorld.FindAnt(Guid.Parse(data.AntGuid));
        ant.UpdateSpeed();
        carryingObjects = new List<GameObject>();
        gatheredResources = new List<string>();
        foreach(string guid in data.GatheredResources)
        {
            carryResource(GameWorld.FindResourceNode(Guid.Parse(guid)));
        }
        inventory = new Dictionary<ResourceType, int>();
        for (int i = 0; i < data.InventoryKeys.Count; i++)
        {
            inventory[data.InventoryKeys[i]] = data.InventoryValues[i];
        }
        IsScout = data.IsScout;
        nextHarvest = data.NextHarvest;
        preparingReturn = data.PreparingReturn;
        scouting = data.Scouting;
        ant.StopAllCoroutines();
        if (scouting)
        {
            ant.StartCoroutine(Scout(data.ScoutSeconds, new Vector3(data.ScoutDestinationX, data.ScoutDestinationY, data.ScoutDestinationZ)));
        }
        if (preparingReturn)
        {
            ant.StartCoroutine(ReturnToBase(data.ReturnSeconds));
        }
        if (data.TargetGuid != "")
        {
            target = GameWorld.FindResourceNode(Guid.Parse(data.TargetGuid));
            ant.GetAgent().SetDestination(target.GetPosition());
        }
        prefferedType = data.PrefferedType;
        carryWeight = data.CarryWeight;
        prefferedDirection = data.PrefferedDirection;
    }
}