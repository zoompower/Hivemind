using Assets.Scripts.Data;
using System;
using System.Collections;
using System.Collections.Generic;
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

    public List<GameObject> carryingObjects = new List<GameObject>();
    private List<string> gatheredResources = new List<string>();

    private Dictionary<ResourceType, int> inventory = new Dictionary<ResourceType, int>();
    public bool IsScout;
    private int nextHarvest;
    private bool preparingReturn;
    private bool scouting;
    private ResourceNode target;
    public bool busy = false;
    private bool leavingBase = false;
    public State state = State.Idle;
    private State nextState;
    private Vector3 scoutingDestination;
    private float scoutSeconds;
    private float returnSeconds;

    private Vector3 TeleporterExit;
    private Vector3 TeleporterEntrance;
    private bool enterBase = false;

    public Gathering() : this(ResourceType.Unknown, 1, Direction.None)
    {
    }

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

    public void Initiate(Ant ant)
    {
        this.ant = ant;
        TeleporterExit = ant.GetBaseController().TeleporterExit;
        TeleporterEntrance = ant.GetBaseController().TeleporterEntrance;
    }

    public void Execute()
    {
        if (leavingBase || enterBase) return;

        switch (state)
        {
            case State.Idle:
                if (ant.GetAgent().isOnNavMesh)
                {
                    ant.GetAgent().isStopped = true;
                }
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
                    var tempTarget = GameWorld.Instance.FindNearestUnknownResource(ant.transform.position, ant.TeamID);
                    if (tempTarget != null && Vector3.Distance(ant.transform.position, tempTarget.GetPosition()) < 2f)
                    {
                        target = tempTarget;
                        ant.StartCoroutine(EnterBase(ant.GetBaseController().GetPosition()));
                        state = State.MovingToStorage;
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
                    if (Vector3.Distance(ant.transform.position, target.GetPosition()) < 1f)
                        state = State.Gathering;
                    if (Vector3.Distance(ant.GetAgent().destination, target.GetPosition()) > 1f)
                    {
                        ant.GetAgent().SetDestination(target.GetPosition());
                    }
                }
                else
                {
                    TargetResource();
                }
                break;

            case State.Gathering:
                if(target == null)
                {
                    TargetResource();
                    break;
                }
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
                    ant.StartCoroutine(EnterBase(ant.GetBaseController().GetPosition()));
                    state = State.MovingToStorage;
                }
                else
                {
                    if (nextHarvest > 0) break;
                    target = null;
                    TargetResource();
                }

                break;

            case State.MovingToStorage:
                if (ant.GetBaseController()?.QueenRoom != null)
                {
                    if (ant.AtBase())
                    {
                        if (IsScout && target != null && (target.TeamIsKnown & (1 << ant.TeamID)) == 0)
                        {
                            target.Discover(ant.TeamID);
                            ant.StopCoroutine(ReturnToBase());
                        }
                        if (inventory != null)
                        {
                            ant.GetBaseController().GetGameResources().AddResources(inventory);
                            inventory.Clear();
                            foreach (var gameObject in carryingObjects) Object.Destroy(gameObject);
                            carryingObjects.Clear();
                            gatheredResources.Clear();
                            ant.currentSpeed = ant.baseSpeed;
                            ant.UpdateSpeed();
                        }

                        state = State.Idle;
                        busy = false;
                    }
                }

                break;
        }
    }

    public double Likelihood()
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
        var clone = new Gathering(prefferedType, carryWeight, prefferedDirection, IsScout);
        return clone;
    }

    public void GenerateUI()
    {
        throw new NotImplementedException();
    }

    private ResourceNode findResource()
    {
        if (carryWeight <= 0) return null;

        var resourceNode = GameWorld.Instance.FindNearestKnownResource((ant.AtBase()) ? TeleporterExit : ant.transform.position, prefferedType, ant.TeamID);
        if (prefferedType != ResourceType.Unknown && resourceNode == null)
            resourceNode = GameWorld.Instance.FindNearestKnownResource((ant.AtBase()) ? TeleporterExit : ant.transform.position, ResourceType.Unknown, ant.TeamID);
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
            if (state == State.Idle && ant.GetAgent().isOnNavMesh)
            {
                ant.GetAgent().isStopped = false;
            }
            ant.StopCoroutine(Scout());
            ant.StopCoroutine(ReturnToBase());
            scouting = false;
            nextHarvest = target.DecreaseFutureResources(carryWeight - carryingObjects.Count);
            ant.GetAgent().SetDestination(target.GetPosition());
            state = State.MovingToResource;
            busy = true;
        }
        else if (state == State.Idle && IsScout)
        {
            if (ant.GetAgent().isOnNavMesh)
            {
                ant.GetAgent().isStopped = false;
            }
            ant.StartCoroutine(ExitBase(State.Scouting));
        }
        else if (!ant.AtBase())
        {
            ant.StartCoroutine(EnterBase(ant.GetBaseController().GetPosition()));
            state = State.MovingToStorage;
        }
    }

    private IEnumerator ExitBase(State nextState)
    {
        leavingBase = true;
        this.nextState = nextState;
        ant.GetAgent().SetDestination(TeleporterEntrance);
        yield return new WaitUntil(() => Vector3.Distance(ant.transform.position, TeleporterEntrance) < 1f);
        ant.GetAgent().SetDestination(TeleporterExit);
        yield return new WaitUntil(() => !ant.AtBase());
        state = nextState;
        leavingBase = false;
        busy = true;
    }

    private IEnumerator EnterBase(Vector3 nextPosition)
    {
        enterBase = true;
        ant.GetAgent().SetDestination(TeleporterExit);
        yield return new WaitUntil(() => Vector3.Distance(ant.transform.position, TeleporterExit) < 1f);
        ant.GetAgent().SetDestination(nextPosition);
        enterBase = false;
    }

    private void carryResource(ResourceNode resource)
    {
        var carryingObject = Object.Instantiate(resource.baseObject.gameObject, ant.transform.position,
            Quaternion.identity, ant.transform);
        carryingObject.transform.localScale = new Vector3(carryingObject.transform.localScale.x * 3,
            carryingObject.transform.localScale.y * 3, carryingObject.transform.localScale.z * 3);
        carryingObject.transform.position = new Vector3(ant.transform.position.x,
            ant.transform.position.y + (ant.transform.localScale.y * 2.5f) * (carryingObjects.Count + 1), ant.transform.position.z);
        carryingObjects.Add(carryingObject);
        gatheredResources.Add(resource.myGuid.ToString());
    }

    private IEnumerator Scout(float seconds = -1f, Vector3 scoutingDestination = new Vector3())
    {
        scoutSeconds = seconds;
        if (scoutSeconds < 0f)
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
        while (scoutSeconds > 0f)
        {
            yield return new WaitForSeconds(0.1f);
            scoutSeconds -= 0.1f;
        }
        scouting = false;
    }

    private IEnumerator ReturnToBase(float seconds = -1f)
    {
        returnSeconds = seconds;
        if (seconds < 0f)
        {
            returnSeconds = Random.Range(30, 40);
        }
        while (returnSeconds > 0f)
        {
            yield return new WaitForSeconds(0.1f);
            returnSeconds -= 0.1f;
        }
        if (state == State.Scouting)
        {
            target = null;
            ant.StartCoroutine(EnterBase(ant.GetBaseController().GetPosition()));
            state = State.MovingToStorage;
            preparingReturn = false;
        }
    }

    public MindData GetData()
    {
        return new GatheringData(ant, gatheredResources, inventory, IsScout, nextHarvest, preparingReturn, scouting, target, prefferedType, carryWeight, prefferedDirection, busy, leavingBase, state, nextState, scoutingDestination, scoutSeconds, returnSeconds, enterBase, TeleporterExit);
    }

    public void SetData(MindData mindData)
    {
        GatheringData data = mindData as GatheringData;
        carryingObjects = new List<GameObject>();
        gatheredResources = new List<string>();
        inventory = new Dictionary<ResourceType, int>();
        for (int i = 0; i < data.InventoryKeys.Count; i++)
        {
            inventory[data.InventoryKeys[i]] = data.InventoryValues[i];
        }
        IsScout = data.IsScout;
        nextHarvest = data.NextHarvest;
        preparingReturn = data.PreparingReturn;
        scouting = data.Scouting;
        busy = data.Busy;
        leavingBase = data.LeavingBase;
        state = data.State;
        nextState = data.NextState;
        prefferedType = data.PrefferedType;
        carryWeight = data.CarryWeight;
        prefferedDirection = data.PrefferedDirection;
        enterBase = data.EnterBase;
        if (data.AntGuid != "")
        {
            Initiate(GameWorld.Instance.FindAnt(Guid.Parse(data.AntGuid)));
            foreach (string guid in data.GatheredResources)
            {
                carryResource(GameWorld.Instance.FindResourceNode(Guid.Parse(guid)));
            }
            ant.UpdateSpeed();
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
                target = GameWorld.Instance.FindResourceNode(Guid.Parse(data.TargetGuid));
                ant.GetAgent().SetDestination(target.GetPosition());
            }
            if (leavingBase)
            {
                ant.StartCoroutine(ExitBase(nextState));
            }
            else if (enterBase || state == State.MovingToStorage)
            {
                ant.StartCoroutine(EnterBase(ant.GetBaseController().GetPosition()));
            }
        }
    }

    public bool IsBusy()
    {
        return busy || leavingBase;
    }
}
