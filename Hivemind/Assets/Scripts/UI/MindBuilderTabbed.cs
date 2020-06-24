using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MindBuilderTabbed : MonoBehaviour
{
    [SerializeField]
    private ColorBlock HighlightedColorBlock = new ColorBlock()
    {
        normalColor = new Color(140f / 255f, 209f / 255f, 255f / 255f),
        highlightedColor = new Color(140f / 255f, 209f / 255f, 255f / 255f),
        pressedColor = new Color(140f / 255f, 209f / 255f, 255f / 255f),
        selectedColor = new Color(130f / 255f, 199f / 255f, 245f / 255f),
        colorMultiplier = 1
    };

    public GameObject resourceMindPanel;
    // Resource mind elements
    private Slider CarryWeight;
    private Dropdown PrefferedType;
    private Dropdown PrefferedDirection;
    private Toggle Scouting;
    private Toggle SmartResources;

    public GameObject combatMindPanel;
    // Combat mind elements
    private Toggle ChargeQueen;
    private InputField EngageDistance;
    private Dropdown Formation;

    //save old minds
    public MindGroup mindGroup;
    private Gathering gather;
    private CombatMind combat;

    // Start is called before the first frame update
    private void Awake()
    {
        //initialize all UI elements
        CarryWeight = resourceMindPanel.GetComponentsInChildren<Slider>().Where(x => x.name.Equals("CarryWeight")).FirstOrDefault();
        CarryWeightChanged();
        PrefferedType = resourceMindPanel.GetComponentsInChildren<Dropdown>().FirstOrDefault(x => x.name.Equals("ResourceType"));
        PrefferedDirection = resourceMindPanel.GetComponentsInChildren<Dropdown>().FirstOrDefault(x => x.name.Equals("ScoutDirection"));
        Scouting = resourceMindPanel.GetComponentsInChildren<Toggle>().FirstOrDefault(x => x.name.Equals("Scouting"));
        //SmartResources = resourceMindPanel.GetComponentsInChildren<Toggle>().Where(x => x.name.Equals("SmartResources")).FirstOrDefault();

        ChargeQueen = combatMindPanel.GetComponentsInChildren<Toggle>().FirstOrDefault(x => x.name.Equals("AttackingQueen"));
        EngageDistance = combatMindPanel.GetComponentsInChildren<InputField>().FirstOrDefault(x => x.name.Equals("EngageDistance"));
        //Formation = resourceMindPanel.GetComponentsInChildren<Dropdown>().Where(x => x.name.Equals("Formation")).FirstOrDefault();

        //make dropdownmenu's
        PrefferedType.ClearOptions();
        var resourceTypes = Enum.GetValues(typeof(ResourceType));
        var dropdownElements = new List<Dropdown.OptionData>();
        for (var i = 0; i < resourceTypes.Length; i++)
            dropdownElements.Add(new Dropdown.OptionData(resourceTypes.GetValue(i).ToString()));

        var buttons = GetComponentsInChildren<Button>();
        var resButton = buttons.FirstOrDefault(x => x.name == "ResourceMindTab");
        resButton.colors = HighlightedColorBlock;
        PrefferedType.AddOptions(dropdownElements);
        dropdownElements.Clear();
        PrefferedDirection.ClearOptions();
        var directionTypes = Enum.GetValues(typeof(Gathering.Direction));
        for (var i = 0; i < directionTypes.Length; i++)
            dropdownElements.Add(new Dropdown.OptionData(directionTypes.GetValue(i).ToString()));
        PrefferedDirection.AddOptions(dropdownElements);
    }

    public void OpenResourceMind(Button resButton)
    {
        if (!resourceMindPanel.activeSelf)
        {
            combatMindPanel.SetActive(false);
            resourceMindPanel.SetActive(true);
            var otherButtons = GetComponentsInChildren<Button>();
            for (var i = 0; i < otherButtons.Length; i++) otherButtons[i].colors = ColorBlock.defaultColorBlock;
            resButton.colors = HighlightedColorBlock;
        }
    }

    public void OpenCombatMind(Button comButton)
    {
        if (!combatMindPanel.activeSelf)
        {
            resourceMindPanel.SetActive(false);
            combatMindPanel.SetActive(true);
            var otherButtons = GetComponentsInChildren<Button>();
            for (var i = 0; i < otherButtons.Length; i++) otherButtons[i].colors = ColorBlock.defaultColorBlock;
            comButton.colors = HighlightedColorBlock;
        }
    }

    internal void GenerateMind()
    {
        gather = (Gathering)mindGroup.GetMinds().Find(mind => mind.GetType() == typeof(Gathering));
        if (gather == null)
        {
            gather = new Gathering();
            mindGroup.GetMinds().Add(gather);
        }

        combat = (CombatMind)mindGroup.GetMinds().Find(mind => mind.GetType() == typeof(CombatMind));
        if (combat == null)
        {
            combat = new CombatMind();
            mindGroup.GetMinds().Add(combat);
        }

        UpdateResourceValues(gather.carryWeight, gather.IsScout, gather.prefferedType, gather.prefferedDirection);

        UpdateCombatValues(combat.EngageRange, combat.AttackingQueen);
    }

    public void UpdateMind()
    {
        UpdateCombatMind();
        UpdateResourceMind();
    }

    private void UpdateResourceMind()
    {
        gather.carryWeight = (int)CarryWeight.value;

        gather.IsScout = Scouting.isOn;
        gather.prefferedType = (ResourceType)PrefferedType.value;
        gather.prefferedDirection = (Gathering.Direction)PrefferedDirection.value;
    }

    private void UpdateCombatMind()
    {
        combat.AttackingQueen = ChargeQueen.isOn;
        combat.EngageRange = int.Parse(EngageDistance.text);
    }

    public void UpdateResourceValues(int carryweight, bool scouting, ResourceType resType, Gathering.Direction direction)
    {
        //CarryWeight.placeholder.GetComponent<Text>().text = carryweight.ToString();
        CarryWeight.value = carryweight;
        Scouting.isOn = scouting;
        PrefferedType.value = (int)resType;
        PrefferedDirection.value = (int)direction;
        // SmartResources.isOn = gather.smartResources;
    }

    private void UpdateCombatValues(int engageRange, bool attackingQueen)
    {
        EngageDistance.text = engageRange.ToString();
        ChargeQueen.isOn = attackingQueen;
        // combat.formation = (Formation)Formation.value;
    }

    public void CarryWeightChanged()
    {
        if (CarryWeight)
            CarryWeight.GetComponentsInChildren<Text>().Where(x => x.name.Equals("Value")).FirstOrDefault().text = CarryWeight.value.ToString();
    }
}
