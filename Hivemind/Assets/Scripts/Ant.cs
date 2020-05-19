using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ant : MonoBehaviour
{
    private NavMeshAgent agent;
    public float baseSpeed;
    public float currentSpeed;
    public int damage;

    public int health;
    public string Prefab;

    private List<IMind> minds;
    public Gathering.State state;
    private Storage storage;
    internal Guid unitGroupID;
    public Ant closestEnemy { get; private set; }


    private void Awake()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        baseSpeed = agent.speed;
        currentSpeed = baseSpeed;
        minds = new List<IMind>();
        state = Gathering.State.Idle;
    }

    // Start is called before the first frame update
    private void Start()
    {
        storage = GameWorld.GetStorage();
        GameWorld.AddNewAnt(this);
    }

    // Update is called once per frame
    private void Update()
    {
        //if (AtBase())
        //{
        //    var mindGroupMind = FindObjectOfType<UnitController>().UnitGroupList.GetMindGroupFromUnitId(unitGroupID)
        //        .Minds;

        //    if (minds.Count < mindGroupMind.Count)
        //        for (var i = minds.Count; i < mindGroupMind.Count; i++)
        //            minds.Add(mindGroupMind[i].Clone());
        //    for (var i = 0; i < minds.Count; i++)
        //        if (!minds[i].Equals(mindGroupMind[i]))
        //        {
        //            minds[i].Update(mindGroupMind[i]);
        //            if (!minds[i].Equals(mindGroupMind[i])) minds[i] = mindGroupMind[i].Clone();
        //        }
        //}

        //if (minds.Count > 0)
        //{
        //    double likeliest = 0;
        //    var mindIndex = 0;
        //    var currentIndex = 0;
        //    foreach (var mind in minds)
        //    {
        //        var current = mind.Likelihood(this);
        //        if (current > likeliest)
        //        {
        //            mindIndex = currentIndex;
        //            likeliest = current;
        //        }

        //        currentIndex++;
        //    }
        //    minds[mindIndex].Execute(this);
        //}
    }

    public bool AtBase()
    {
        if (Vector3.Distance(transform.position, storage.GetPosition()) < 1f) return true;
        return false;
    }

    public bool InCombat()
    {
        return false;
    }

    public NavMeshAgent GetAgent()
    {
        return agent;
    }

    public Storage GetStorage()
    {
        return storage;
    }

    public void SetUnitGroup(Guid ug)
    {
        unitGroupID = ug;
    }

    internal void UpdateSpeed()
    {
        agent.speed = currentSpeed;
    }

    internal void SetStorage(Storage storage)
    {
        this.storage = storage;
    }

    private enum AntType
    {
        Worker,
        Soldier
    }

    public void Destroy()
    {
        Destroy(gameObject);
        Destroy(this);
        GameWorld.RemoveAnt(this);
    }

    public AntData GetData()
    {
        return new AntData(baseSpeed, currentSpeed, damage, health, minds, state, storage, unitGroupID, closestEnemy, Prefab, gameObject.transform.position, gameObject.transform.localEulerAngles, gameObject.transform.parent);
    }

    public void SetData(AntData data)
    {
        gameObject.SetActive(false);
        gameObject.transform.parent = data.Parent;
        baseSpeed = data.BaseSpeed;
        currentSpeed = data.CurrentSpeed;
        damage = data.Damage;
        health = data.Health;
        Prefab = data.Prefab;
        //minds = data.Minds;
        minds = new List<IMind>();
        state = data.State;
        storage = data.Storage;
        unitGroupID = Guid.Parse(data.UnitGroupID);
        closestEnemy = data.ClosestEnemy;
        gameObject.SetActive(true);
        gameObject.transform.localEulerAngles = new Vector3(data.RotationX, data.RotationY, data.RotationZ);
    }
}