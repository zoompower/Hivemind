﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MindBuilderTabbed : MonoBehaviour
{
    //get panel UI elements

    //get resourcemind UI elements
    private Slider CarryWeight;

    private CombatMind combat;
    public GameObject combatMindPanel;
    private InputField EstimatedDifference;
    private Dropdown Formation;

    //save old minds
    private Gathering gather;

    [SerializeField]
    private ColorBlock HighlightedColorBlock = new ColorBlock()
    {
        normalColor = new Color(140f / 255f, 209f / 255f, 255f / 255f),
        highlightedColor = new Color(140f / 255f, 209f / 255f, 255f / 255f),
        pressedColor = new Color(140f / 255f, 209f / 255f, 255f / 255f),
        selectedColor = new Color(130f / 255f, 199f / 255f, 245f / 255f),
        colorMultiplier = 1
    };

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
        //initialize all UI elements
        CarryWeight = resourceMindPanel.GetComponentsInChildren<Slider>().Where(x => x.name.Equals("CarryWeight")).FirstOrDefault();
        CarryWeightChanged();

        Scouting = resourceMindPanel.GetComponentsInChildren<Toggle>().Where(x => x.name.Equals("Scouting")).FirstOrDefault();
        //SmartResources = resourceMindPanel.GetComponentsInChildren<Toggle>().Where(x => x.name.Equals("SmartResources")).FirstOrDefault();
        PrefferedType = resourceMindPanel.GetComponentsInChildren<Dropdown>().Where(x => x.name.Equals("ResourceType")).FirstOrDefault();
        PrefferedDirection = resourceMindPanel.GetComponentsInChildren<Dropdown>().Where(x => x.name.Equals("ScoutDirection")).FirstOrDefault();

        PrefferedHealth = combatMindPanel.GetComponentsInChildren<InputField>().Where(x => x.name.Equals("PrefferedHealth")).FirstOrDefault();
        EstimatedDifference = combatMindPanel.GetComponentsInChildren<InputField>().Where(x => x.name.Equals("Estimated Difference")).FirstOrDefault();
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
        gather = (Gathering)mindGroup.Minds.Find(mind => mind.GetType() == typeof(Gathering));
        if (gather == null)
        {
            gather = new Gathering();
            mindGroup.Minds.Add(gather);
        }

        combat = (CombatMind)mindGroup.Minds.Find(mind => mind.GetType() == typeof(CombatMind));
        if (combat == null)
        {
            combat = new CombatMind();
            mindGroup.Minds.Add(combat);
        }

        UpdateResourceValues(gather.carryWeight, gather.IsScout, gather.prefferedType, gather.prefferedDirection);

        UpdateCombatValues(combat.GetMinEstimatedDifference(), combat.GetPrefferedHealth());
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

    public void UpdateResourceValues(int carryweight, bool scouting, ResourceType resType, Gathering.Direction direction)
    {
        //CarryWeight.placeholder.GetComponent<Text>().text = carryweight.ToString();
        CarryWeight.value = carryweight;
        Scouting.isOn = scouting;
        PrefferedType.value = (int)resType;
        PrefferedDirection.value = (int)direction;
        // SmartResources.isOn = gather.smartResources;
    }

    private void UpdateCombatMind()
    {
        if (float.TryParse(EstimatedDifference.text, out var estDiff))
            combat.SetMinEstimatedDifference(estDiff);

        if (int.TryParse(PrefferedHealth.text, out var prefferedHealth))
            combat.SetPrefferedHealth(prefferedHealth);
        // combat.formation = (Formation)Formation.value;
    }

    public void CarryWeightChanged()
    {
        if (CarryWeight)
            CarryWeight.GetComponentsInChildren<Text>().Where(x => x.name.Equals("Value")).FirstOrDefault().text = CarryWeight.value.ToString();
    }

    public void UpdateCombatValues(float estimatedDifference, float prefferedHealth)
    {
        PrefferedHealth.text = prefferedHealth.ToString();
        EstimatedDifference.text = estimatedDifference.ToString();
        //Formation.value = (int) combat.formation;
    }
}