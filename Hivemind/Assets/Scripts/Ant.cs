using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Ant : MonoBehaviour
{
    enum AntType
    {
        Worker,
        Soldier,
    }

    private int health;
    private int damage;
    public float baseSpeed;
    public float currentSpeed;
    public bool IsScout = true;

    private List<IMind> minds;
    private IMind behaviour;

    private NavMeshAgent agent;
    public Ant closestEnemy { get; private set; }
    private Storage storage;
    private Guid unitGroupID;


    private CombatMind combatMind;


    void Awake()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        baseSpeed = agent.speed;
        currentSpeed = baseSpeed;
    }

    // Start is called before the first frame update
    void Start()
    {
        storage = GameWorld.GetStorage();
    }

    // Update is called once per frame
    void Update()
    {
        if (InCombat())
        {
            //combatBehaviour = DecideCombatBehavior();
            //combatBehaviour.CombatMode(this, closestEnemy);
            return;
        }

        if (AtBase())
        {
            //combatMind = FindObjectOfType<UnitController>().UnitGroupList.GetMindGroupFromUnitId(unitGroupID).combatMind;

           minds = FindObjectOfType<UnitController>().UnitGroupList.GetMindGroupFromUnitId(unitGroupID).minds;
        }

        minds[0].Execute(this);
    }

    public bool AtBase()
    {
        if (Vector3.Distance(transform.position, storage.GetPosition()) < 2f)
        {
            return true;
        }
        return false;
    }

    public bool InCombat()
    {
        return false;
    }

    IMind DecideCombatBehavior()
    {
        if (combatMind == null)
        {
            return new CombatFight();
        }

        float healthPercantageDifference = ((float)health / (float)closestEnemy.health);
        float damagePercantageDifference = ((float)damage / (float)closestEnemy.damage);
        float strengthDifference = (healthPercantageDifference * 1 + damagePercantageDifference * 2) / 3;
        if (strengthDifference >= combatMind.GetMinEstimetedDifference())
        {
            return new CombatFight();
        }
        return new CombatFlee();
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
}

