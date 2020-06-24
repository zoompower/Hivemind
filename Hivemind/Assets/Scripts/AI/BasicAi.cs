using Assets.Scripts.Data;
using UnityEngine;

public class BasicAi : MonoBehaviour
{
    private BaseController basecontroller;
    private UnitController unitController;
    private bool waitingForMindsGotten;
    private int coolingDown = 0;

    private void Awake()
    {
        GameWorld.Instance.AddAI(this);
    }

    void Start()
    {
        unitController = GetComponent<UnitController>();
        basecontroller = GetComponent<BaseController>();

        InvokeRepeating("ExecuteRules", 1, 1);
    }

    private void ExecuteRules()
    {
        var gameResources = basecontroller.GetGameResources();
        int totalUnits = unitController.MindGroupList.GetMindGroupFromIndex(1).GetTotalCurrentUnitCount();

        if (gameResources.GetResourceAmount(ResourceType.Food) <= unitController.MindGroupList.GetMindGroupFromIndex(1).GetTotalCurrentUnitCount())
        {
            OverrideMinds(new DataEditor[] { DataEditor.GetGatheringEditor(ResourceType.Food, 1, Gathering.Direction.None, true), DataEditor.GetCombatEditor() });
        }
        else if (gameResources.GetResourceAmount(ResourceType.Food) > totalUnits && totalUnits <= GameWorld.Instance.GetEnemyBase(basecontroller.TeamID).GetComponent<UnitController>().MindGroupList.GetTotalAliveAnts())
        {
            OverrideMinds(new DataEditor[] { DataEditor.GetGatheringEditor(ResourceType.Rock, 1, Gathering.Direction.None, true), DataEditor.GetCombatEditor() });
        }
        else if (totalUnits > 5 && coolingDown <= 0)
        {
            if (gameResources.GetResourceAmount(ResourceType.Food) > totalUnits && totalUnits != 0 && !waitingForMindsGotten)
            {
                waitingForMindsGotten = true;
                OverrideMinds(new DataEditor[] { DataEditor.GetCombatEditor(5, false) });
            }
            if (unitController.MindGroupList.GetMindGroupFromIndex(1).NewMindsGotten())
            {
                if (gameResources.GetResourceAmount(ResourceType.Food) > totalUnits && totalUnits != 0 && waitingForMindsGotten)
                {
                    waitingForMindsGotten = false;
                    OverrideMinds(new DataEditor[] { DataEditor.GetCombatEditor(5, true) });
                }
                else
                {
                    OverrideMinds(new DataEditor[] { DataEditor.GetGatheringEditor(), DataEditor.GetCombatEditor() });
                    coolingDown = 20;
                }
            }
        }
        else
        {
            OverrideMinds(new DataEditor[] { DataEditor.GetGatheringEditor(ResourceType.Rock, 1, Gathering.Direction.None, true), DataEditor.GetCombatEditor() });
        }

        if (coolingDown > 0)
        {
            coolingDown--;
        }
    }

    public int GetTeamId()
    {
        return basecontroller.TeamID;
    }

    private void OverrideMinds(DataEditor[] datas)
    {
        unitController.MindGroupList.OverrideMinds(datas);
    }

    public BasicAIData GetData()
    {
        return new BasicAIData(waitingForMindsGotten, coolingDown, GetTeamId());
    }

    public void SetData(BasicAIData data)
    {
        waitingForMindsGotten = data.WaitingForMindsGotten;
        coolingDown = data.CoolingDown;
    }
}
