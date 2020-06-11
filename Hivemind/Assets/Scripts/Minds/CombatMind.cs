using Assets.Scripts.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using Random = UnityEngine.Random;

public class CombatMind : IMind
{
    private float minEstimatedDifference;
    private int prefferedHealth;
    private Ant ant;
    private bool busy;
    private Timer Surroundingcheck;
    private bool SurroundingcheckOncooldown;

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
    private Timer AttackCooldown;
    private bool AttackOnCooldown;

    public enum State
    {
        Idle,
        Scouting,
        Escort,
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
        AttackCooldown = new Timer(1000);
        AttackCooldown.Elapsed += AttackCooldownElapsed;
        EngageRange = 99999;
        IsScout = false;
        Surroundingcheck = new Timer(1000);
        Surroundingcheck.Elapsed += SurroundingcheckCooldownElapsed;

    }

    public IMind Clone()
    {
        return new CombatMind(minEstimatedDifference, prefferedHealth);
    }

    public void Execute()
    {
        ///SwitchState
        if (leavingBase || enterBase) return;

        switch (state)
        {
            case State.Idle:

                if (CheckSurroundings())
                {
                    state = State.MovingToTarget;
                }
                else
                {
                    state = State.Scouting;
                }

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
                        ant.StartCoroutine(EnterBase(ant.GetBaseController().GetPosition()));
                        state = State.MovingToTarget;
                        ant.StartCoroutine(Discover());
                    }
                    else
                    {
                        scouting = true;
                        ant.StartCoroutine(Scout());
                    }
                }
                break;

            case State.Escort:
                


                break;

            case State.MovingToTarget:
                if (target.transform.position != ant.GetAgent().destination){
                    ant.GetAgent().SetDestination(target.transform.position);
                }

                if (CheckAttackDistance())
                {
                    state = State.Engaging;
                }

                break;

            case State.Engaging:

                if (!CheckAttackDistance())
                {
                    state = State.MovingToTarget;
                }
                else
                {
                    if (target && target.alive)
                    {
                        AttackTarget();
                    }
                    else
                    {
                        Debug.Log("Kom ik hier?!");
                        target = null;
                        if (CheckSurroundings())
                        {
                            state = State.MovingToTarget;
                        }
                        else
                        {
                            state = State.MovingToNest;
                        }
                    }
                }

                break;

            case State.MovingToNest:
                if (CheckSurroundings())
                {
                    state = State.MovingToTarget;
                }
                else
                {
                    ant.StartCoroutine(ReturnToBase());
                }

                break;
        }
    }

    private bool CheckSurroundings()
    {
        bool FoundEnemy = false;
        if (ant.SpatialPositionId != 0)
        {
            target = null;
            foreach (GameObject a in SpatialPartition.GetSpatialFromGrid(ant.SpatialPositionId).GetEntitiesWithNeigbors())
            {
                if (a && a.GetComponent<Ant>())
                {
                    if (a.GetComponent<Ant>().TeamID != ant.TeamID && a.GetComponent<Ant>() != ant && a.GetComponent<Ant>().alive)
                    {
                        if (target == null || Vector3.Distance(ant.transform.position, a.transform.position) < Vector3.Distance(ant.transform.position, target.transform.position))
                        {
                            target = a.GetComponent<Ant>();
                        }
                    }
                }
            }
            if (target != null) { FoundEnemy = true; }
        }

        return FoundEnemy;
    }

    private bool CheckAttackDistance()
    {
        if (target == null) return false;
        if (Vector3.Distance(ant.transform.position, target.transform.position) < 1f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void AttackCooldownElapsed(object sender, ElapsedEventArgs e)
    {
        AttackCooldown.Stop();
        AttackOnCooldown = false;
    }
    private void SurroundingcheckCooldownElapsed(object sender, ElapsedEventArgs e)
    {
        Surroundingcheck.Stop();
        SurroundingcheckOncooldown = false;
    }

    private bool AttackTarget()
    {
        if (!AttackOnCooldown)
        {
            target.health -= ant.damage;
            AttackOnCooldown = true;
            AttackCooldown.Start();
            return true;
        }
        else
        {
            return false;
        }
    }

    private IEnumerator Discover()
    {
        ant.PlaySoundDiscovery();

        var excla = (GameObject)GameObject.Instantiate(Resources.Load("ExclamationMark"), ant.transform, false);
        excla.transform.localScale *= 3;

        yield return new WaitForSeconds(0.8f);
        UnityEngine.GameObject.Destroy(excla.gameObject);
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
            ant.StartCoroutine(EnterBase(ant.GetBaseController().GetPosition()));
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
        if (SurroundingcheckOncooldown == false)
        {
            SurroundingcheckOncooldown = true;
            Surroundingcheck.Start();

            busy = false;
            if (CheckSurroundings())
            {
                busy = true;
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

    public MindData GetData()
    {
        return new CombatData(minEstimatedDifference, prefferedHealth, ant, busy);
    }

    public void SetData(MindData mindData)
    {
        CombatData data = mindData as CombatData;
        minEstimatedDifference = data.MinEstimatedDifference;
        prefferedHealth = data.PrefferedHealth;
        if(data.AntGuid != "")
        {
            ant = GameWorld.Instance.FindAnt(Guid.Parse(data.AntGuid));
        }
        busy = data.Busy;
    }

    public bool IsBusy()
    {
        return busy;
    }
}
