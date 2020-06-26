using Assets.Scripts.Data;
using System;
using UnityEngine;

public class BasicAi : MonoBehaviour
{
    private BaseController baseController;
    private UnitController unitController;

    private AttackingState attackingState = AttackingState.Idle;
    private int coolingDown = 0;
    private int attackCount = 0;

    public enum AttackingState
    {
        Idle,
        GatherAtBase,
        Attack
    }

    private void Awake()
    {
        GameWorld.Instance.AddAI(this);
    }

    void Start()
    {
        unitController = GetComponent<UnitController>();
        baseController = GetComponent<BaseController>();

        InvokeRepeating("ExecuteRules", 1, 1);
    }

    private void ExecuteRules()
    {
        var currentResources = baseController.GetGameResources();
        int totalAliveUnits = unitController.MindGroupList.GetTotalAliveAnts();
        int totalAllowedUnits = unitController.MindGroupList.GetTotalPossibleAnts();
        // The curve for this calculation is https://www.desmos.com/calculator/zshcxyt744, for X we take attackCount times two to get an easier curve for what its purpose.
        int minimumAttackAmount = (int)Math.Round((3f * Math.Pow(1.04d, (attackCount * 2) + 20)) - 1.5d);

        if (totalAliveUnits >= minimumAttackAmount && coolingDown <= 0 && currentResources.GetResourceAmount(ResourceType.Food) > totalAllowedUnits)
        {
            if (attackingState == AttackingState.Idle)
            {
                attackingState = AttackingState.GatherAtBase;
                OverrideMinds(new DataEditor[] { DataEditor.GetCombatEditor(5, false) });
            }
            if (unitController.MindGroupList.GetMindGroupFromIndex(1).NewMindsGotten() && attackingState != AttackingState.Idle)
            {
                if (currentResources.GetResourceAmount(ResourceType.Food) > totalAliveUnits && attackingState == AttackingState.GatherAtBase)
                {
                    attackingState = AttackingState.Attack;
                    OverrideMinds(new DataEditor[] { DataEditor.GetCombatEditor(5, true) });
                }
                else
                {
                    OverrideMinds(new DataEditor[] { DataEditor.GetGatheringEditor(), DataEditor.GetCombatEditor() });
                    coolingDown = 60;
                    attackCount++;
                    attackingState = AttackingState.Idle;
                }
            }
        }
        else if (currentResources.GetResourceAmount(ResourceType.Food) <= totalAllowedUnits * 2 && attackingState == AttackingState.Idle)
        {
            attackingState = AttackingState.Idle;
            OverrideMinds(new DataEditor[] { DataEditor.GetGatheringEditor(ResourceType.Food, 1, Gathering.Direction.None, true), DataEditor.GetCombatEditor() });
        }
        else if (totalAllowedUnits < minimumAttackAmount)
        {
            attackingState = AttackingState.Idle;
            OverrideMinds(new DataEditor[] { DataEditor.GetGatheringEditor(ResourceType.Rock, 1, Gathering.Direction.None, true), DataEditor.GetCombatEditor() });
        }
        else
        {
            attackingState = AttackingState.Idle;
            OverrideMinds(new DataEditor[] { DataEditor.GetGatheringEditor(ResourceType.None, 1, Gathering.Direction.None, true), DataEditor.GetCombatEditor() });
        }

        if (coolingDown > 0)
        {
            coolingDown--;
        }
    }

    public int GetTeamId()
    {
        return baseController.TeamID;
    }

    private void OverrideMinds(DataEditor[] datas)
    {
        unitController.MindGroupList.OverrideMinds(datas);
    }

    private void OnDestroy()
    {
        GameWorld.Instance.RemoveAI(this);
    }

    public BasicAIData GetData()
    {
        return new BasicAIData(attackingState, coolingDown, attackCount, GetTeamId());
    }

    public void SetData(BasicAIData data)
    {
        attackingState = data.AttackingState;
        coolingDown = data.CoolingDown;
        attackCount = data.AttackCount;
    }
}
