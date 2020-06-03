using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class CombatMind : IMind
{
    private float minEstimatedDifference;
    private int prefferedHealth;
    private Ant ant;
    private bool busy;

    private State state = State.Idle;
    private bool leavingBase = false;
    private bool enterBase = false;
    private Vector3 TeleporterExit;

    public bool IsScout;
    private bool preparingReturn;
    private bool scouting;
    public Direction prefferedDirection { get; set; }

    private Ant target;
    public int EngageRange;

    public enum State
    {
        Idle,
        Scouting,
        MovingToTarget,
        Engaging,
        MovingToNest
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
        NorthWest
    }

    public CombatMind() : this(0, 0) { }

    public CombatMind(float minEstimated, int prefHealth)
    {
        minEstimatedDifference = minEstimated;
        prefferedHealth = prefHealth;
    }

    public IMind Clone()
    {
        return new CombatMind(minEstimatedDifference, prefferedHealth);
    }

    public void Execute()
    {
        if (this == null) new CombatFight().Execute(ant);

        var healthDifference = ant.health / (float) ant.closestEnemy.health;
        var damageDifference = ant.damage / (float) ant.closestEnemy.damage;
        var strengthDifference = (healthDifference * 1 + damageDifference * 2) / 3;

        if (strengthDifference >= minEstimatedDifference)
            new CombatFight().Execute(ant);
        else
            new CombatFlee().Execute(ant);

        ///SwitchState
        if (leavingBase || enterBase) return;

        switch (state)
        {
            case State.Idle:
                ant.GetAgent().isStopped = true;
                break;

            case State.Scouting:
                if (!preparingReturn)
                {
                    preparingReturn = true;
                    ant.StartCoroutine(ReturnToBase());
                }

                if (!scouting)
                {
                    CheckSurroundings();
                    if (target != null && Vector3.Distance(ant.transform.position, target.transform.position) < EngageRange)
                    {
                        ant.StartCoroutine(EnterBase(ant.GetStorage().GetPosition()));
                        state = State.MovingToTarget;
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

            case State.MovingToTarget:

                break;

            case State.Engaging:

                break;

            case State.MovingToNest:

                break;
        }
    }

    private bool CheckSurroundings()
    {
        target = null;
        var NearbyEntitities = ant.SpatialPosition.GetEntitiesWithNeigbors();
        busy = false;
        foreach (GameObject a in NearbyEntitities)
        {
            if (a.GetComponent<Ant>())
            {
                if (a.GetComponent<Ant>().GetStorage() != this.ant.GetStorage())
                {
                    if ( target == null || Vector3.Distance(ant.transform.position, a.transform.position) < Vector3.Distance(ant.transform.position, target.transform.position))
                    {
                        target = a.GetComponent<Ant>();
                    }
                    busy = true;
                }
            }
        }
        if(target != null){     return true;}
        else {                  return false; }
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
        yield return new WaitForSeconds(Random.Range(1, 3));
        scouting = false;
    }

    private IEnumerator ReturnToBase()
    {
        yield return new WaitForSeconds(Random.Range(30, 40));
        if (state == State.Scouting)
        {
            target = null;
            ant.StartCoroutine(EnterBase(ant.GetStorage().GetPosition()));
            state = State.MovingToNest;
            preparingReturn = false;
        }
    }

    private IEnumerator ExitBase(State nextState)
    {
        leavingBase = true;
        ant.GetAgent().SetDestination(TeleporterExit);
        yield return new WaitUntil(() => !ant.AtBase());
        state = nextState;
        leavingBase = false;
    }

    private IEnumerator EnterBase(Vector3 nextPosition)
    {
        enterBase = true;
        ant.GetAgent().SetDestination(TeleporterExit);
        yield return new WaitUntil(() => Vector3.Distance(ant.transform.position, TeleporterExit) < 1f);
        ant.GetAgent().SetDestination(nextPosition);
        enterBase = false;
    }


    public void GenerateUI()
    {
        throw new NotImplementedException();
    }

    public void Initiate(Ant ant)
    {
        this.ant = ant;
    }

    public double Likelihood()
    {
        var NearbyEntitities = ant.SpatialPosition.GetEntitiesWithNeigbors();
        busy = false;
        foreach (GameObject a in NearbyEntitities)
        {
            if (a.GetComponent<Ant>())
            {
                if(a.GetComponent<Ant>().GetStorage() != this.ant.GetStorage())
                {
                    if(Vector3.Distance(ant.transform.position, a.transform.position) < Vector3.Distance(ant.transform.position, ant.closestEnemy.transform.position))
                    {
                        this.ant.SetClosestEnemy(a.GetComponent<Ant>());
                    }
                    busy = true;
                }
            }
        }
        if (busy)
        {
            return 100;
        }
        else
        {
            return 0;
        }
    }

    public float GetMinEstimatedDifference()
    {
        return minEstimatedDifference;
    }

    public void SetMinEstimatedDifference(float estDiff)
    {
        minEstimatedDifference = estDiff;
    }

    public int GetPrefferedHealth()
    {
        return prefferedHealth;
    }

    public void SetPrefferedHealth(int prefHealth)
    {
        prefferedHealth = prefHealth;
    }

    public bool IsBusy()
    {
        return busy;
    }
}