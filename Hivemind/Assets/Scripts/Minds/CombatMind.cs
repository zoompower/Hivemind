using Assets.Scripts.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

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
    private State nextState;
    private bool enterBase = false;
    private Vector3 TeleporterExit;

    public Direction prefferedDirection { get; set; }
    public Vector3 TeleporterEntrance { get; private set; }

    public bool AttackingQueen;

    private Ant target;
    public int EngageRange;
    private Timer AttackCooldown;
    private bool AttackOnCooldown;
    private bool enteredEnemyBase = false;

    public enum State
    {
        Idle,
        Scouting,
        AttackingQueen,
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

    public CombatMind() : this(2, false)
    {
    }

    public CombatMind(int engageRange, bool attackingQueen)
    {
        AttackCooldown = new Timer(1000);
        AttackCooldown.Elapsed += AttackCooldownElapsed;
        EngageRange = engageRange;
        AttackingQueen = attackingQueen;
        Surroundingcheck = new Timer(1000);
        Surroundingcheck.Elapsed += SurroundingcheckCooldownElapsed;
    }

    public IMind Clone()
    {
        return new CombatMind(EngageRange, AttackingQueen);
    }

    public void Execute()
    {
        ant.GetAgent().isStopped = false;
        ///SwitchState
        if (leavingBase || enterBase) return;
        switch (state)
        {
            case State.Idle:

                if (AttackingQueen)
                {
                    if (ant.AtBase())
                    {
                        ant.StartCoroutine(ExitBase(State.AttackingQueen));
                    }
                    else
                    {
                        state = State.AttackingQueen;
                    }
                }

                if (CheckSurroundings())
                {
                    state = State.MovingToTarget;
                }

                break;

            case State.AttackingQueen:
                if (AttackingQueen)
                {
                    if (CheckAttackDistance() || CheckSurroundings())
                    {
                        state = State.MovingToTarget;
                    }

                    if (Vector3.Distance(GetEnemyBase().TeleporterExitTransform.position, ant.GetAgent().destination) > 1f && !enteredEnemyBase)
                    {
                        ant.GetAgent().SetDestination(GetEnemyBase().TeleporterExitTransform.position);
                    }

                    if (Vector3.Distance(ant.transform.position, ant.GetAgent().destination) <= 1f)
                    {
                        enteredEnemyBase = true;
                        ant.GetAgent().SetDestination(GetEnemyBase().GetPosition());
                    }

                    if (Vector3.Distance(ant.transform.position, GetEnemyBase().GetPosition()) < 8f)
                    {
                        AttackQueen();
                    }
                }

                break;

            case State.MovingToTarget:
                if (!target)
                {
                    state = State.Idle;
                    break;
                }

                if (target.transform.position != ant.GetAgent().destination)
                {
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
                        target = null;
                        if (CheckSurroundings())
                        {
                            state = State.MovingToTarget;
                        }
                        else if (AttackingQueen)
                        {
                            state = State.AttackingQueen;
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
                    ant.StartCoroutine(EnterBase(ant.GetBaseController().GetPosition()));
                }

                break;
        }
    }

    private BaseController GetEnemyBase()
    {
        return GameWorld.Instance.GetEnemyBase(ant.TeamID);
    }

    private bool CheckSurroundings()
    {
        bool FoundEnemy = false;
        List<GameObject> entityList = new List<GameObject>();

        if (ant.SpatialPositionId > 0)
        {
            entityList = GameObject.FindObjectOfType<SpatialPartition>().GetSpatialFromGrid(ant.SpatialPositionId).GetEntitiesWithNeigbors();
        }
        else if (ant.SpatialPositionId == int.MinValue)
        {
            entityList = GameWorld.Instance.GetCurrentBase(ant)?.GetSpatialPartition()?.GetEntities();
        }
        target = null;
        if (entityList != null)
        {
            foreach (GameObject a in entityList)
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
        }
        if (target != null) { FoundEnemy = true; }

        return FoundEnemy;
    }

    private bool CheckAttackDistance()
    {
        if (target == null) return false;
        if (Vector3.Distance(ant.transform.position, target.transform.position) < 10f * ant.transform.localScale.x)
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

    private bool AttackQueen()
    {
        if (!AttackOnCooldown)
        {
            GetEnemyBase().QueenRoom.health -= ant.damage;
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

    public void Initiate(Ant ant)
    {
        this.ant = ant;
        TeleporterEntrance = ant.GetBaseController().TeleporterEntranceTransform.position;
        TeleporterExit = ant.GetBaseController().TeleporterExitTransform.position;
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
        else if (AttackingQueen)
        {
            busy = true;
            return 80;
        }
        else
        {
            return 0;
        }
    }

    public MindData GetData()
    {
        return new CombatData(prefferedHealth, ant, busy, state, leavingBase, nextState, enterBase, TeleporterEntrance, AttackingQueen, target, EngageRange, enteredEnemyBase);
    }

    public void SetData(MindData mindData)
    {
        CombatData data = mindData as CombatData;
        prefferedHealth = data.PrefferedHealth;
        busy = data.Busy;
        state = data.State;
        leavingBase = data.LeavingBase;
        nextState = data.NextState;
        enterBase = data.EnterBase;
        TeleporterEntrance = new Vector3(data.TeleporterEntranceX, data.TeleporterEntranceY, data.TeleporterEntranceZ);
        AttackingQueen = data.AttackingQueen;
        EngageRange = data.EngageRange;
        enteredEnemyBase = data.EnteredEnemyBase;
        AttackCooldown = new Timer(1000);
        AttackCooldown.Elapsed += AttackCooldownElapsed;
        Surroundingcheck = new Timer(1000);
        Surroundingcheck.Elapsed += SurroundingcheckCooldownElapsed;
        if (data.AntGuid != "")
        {
            ant = GameWorld.Instance.FindAnt(Guid.Parse(data.AntGuid));
            ant.UpdateSpeed();
            if (data.TargetGuid != "")
            {
                target = GameWorld.Instance.FindAnt(Guid.Parse(data.TargetGuid));
                ant.GetAgent().SetDestination(target.transform.position);
            }
            if (leavingBase)
            {
                ant.StartCoroutine(ExitBase(nextState));
            }
        }
    }

    public bool IsBusy()
    {
        return busy;
    }
}