using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MindBuilderTabbed : MonoBehaviour
{
    //get panel UI elements

    //get resourcemind UI elements
    private InputField CarryWeight;
    private CombatMind combat;
    public GameObject combatMindPanel;
    private InputField EstimatedDifference;
    private Dropdown Formation;

    //save old minds
    private Gathering gather;
    private ColorBlock HighlightedColorBlock;

    public MindGroup mindGroup;
    private Dropdown PrefferedDirection;

    //get combatmind UI elements
    private InputField PrefferedHealth;
    private Dropdown PrefferedType;
    public GameObject resourceMindPanel;
    private Toggle Scouting;
    private Toggle SmartResources;

    // Start is called before the first frame update
    private void Awake()
    {
        HighlightedColorBlock = new ColorBlock();
        HighlightedColorBlock.normalColor = new Color(140f / 255f, 209f / 255f, 255f / 255f);
        HighlightedColorBlock.highlightedColor = new Color(140f / 255f, 209f / 255f, 255f / 255f);
        HighlightedColorBlock.pressedColor = new Color(140f / 255f, 209f / 255f, 255f / 255f);
        HighlightedColorBlock.selectedColor = new Color(130f / 255f, 199f / 255f, 245f / 255f);
        HighlightedColorBlock.colorMultiplier = 1;

        //initialize all UI elements
        CarryWeight = resourceMindPanel.GetComponentInChildren<InputField>();
        Scouting = resourceMindPanel.GetComponentsInChildren<Toggle>().Where(x => x.name.Equals("Scouting"))
            .FirstOrDefault();
        //SmartResources = resourceMindPanel.GetComponentsInChildren<Toggle>().Where(x => x.name.Equals("SmartResources")).FirstOrDefault();
        PrefferedType = resourceMindPanel.GetComponentsInChildren<Dropdown>().Where(x => x.name.Equals("ResourceType"))
            .FirstOrDefault();
        PrefferedDirection = resourceMindPanel.GetComponentsInChildren<Dropdown>()
            .Where(x => x.name.Equals("ScoutDirection")).FirstOrDefault();

        PrefferedHealth = combatMindPanel.GetComponentsInChildren<InputField>()
            .Where(x => x.name.Equals("PrefferedHealth")).FirstOrDefault();
        EstimatedDifference = combatMindPanel.GetComponentsInChildren<InputField>()
            .Where(x => x.name.Equals("Estimated Difference")).FirstOrDefault();
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

    private void Update()
    {
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
        gather = (Gathering) mindGroup.Minds.Find(mind => mind.GetType() == typeof(Gathering));
        combat = (CombatMind) mindGroup.Minds.Find(mind => mind.GetType() == typeof(CombatMind));
        CarryWeight.placeholder.GetComponent<Text>().text = gather.carryWeight.ToString();
        CarryWeight.text = gather.carryWeight.ToString();
        Scouting.isOn = gather.IsScout;
        PrefferedType.value = (int) gather.prefferedType;
        PrefferedDirection.value = (int) gather.prefferedDirection;
        // SmartResources.isOn = gather.smartResources;

        PrefferedHealth.text = combat.GetPrefferedHealth().ToString();
        EstimatedDifference.text = combat.GetMinEstimatedDifference().ToString();
        //Formation.value = (int) combat.formation;
    }

    public void UpdateMind()
    {
        UpdateCombatMind();
        UpdateResourceMind();
    }

    private void UpdateResourceMind()
    {
        if (int.TryParse(CarryWeight.text, out var carryweight))
            gather.carryWeight = carryweight;

        gather.IsScout = Scouting.isOn;
        gather.prefferedType = (ResourceType) PrefferedType.value;
        gather.prefferedDirection = (Gathering.Direction) PrefferedDirection.value;
    }

    private void UpdateCombatMind()
    {
        if (float.TryParse(EstimatedDifference.text, out var estDiff))
            combat.SetMinEstimatedDifference(estDiff);

        if (int.TryParse(PrefferedHealth.text, out var prefferedHealth))
            combat.SetPrefferedHealth(prefferedHealth);
        // combat.formation = (Formation)Formation.value;
    }
}