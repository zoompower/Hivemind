using Assets.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Authors:
 * René Duivenvoorden
 */
public class UnitGroup
{
    public int MaxUnits { get; private set; }

    public int CurrentUnits { get; private set; }

    internal GameObject Ui_IconObj { get; private set; }

    internal Guid UnitGroupId { get; private set; }

    private Text textBox;

    private MindGroup mindGroup;

    internal UnitGroup(GameObject unitIconBase)
    {
        Ui_IconObj = UnityEngine.Object.Instantiate(unitIconBase);

        UnitGroupId = Guid.NewGuid();

        textBox = Ui_IconObj.GetComponentInChildren<Text>();

        UpdateText();
    }

    public void SetMaxUnits(int amount)
    {
        MaxUnits = amount;
        UpdateText();
    }

    public void SetCurrentUnits(int amount)
    {
        if (amount > MaxUnits || amount < 0) return;

        CurrentUnits = amount;
        UpdateText();
    }

    public void AddUnits(Ant ant)
    {
        ant.SetUnitGroup(this);
    }

    public ResourceMind GetResourceMind()
    {
        return mindGroup.resMind;
    }

    public CombatMind GetCombatMind()
    {
        return mindGroup.combatMind;
    }

    public void SetMindGroup(MindGroup mg)
    {
        mindGroup = mg;   
    }

    private void UpdateText()
    {
        textBox.text = $"{CurrentUnits}/{MaxUnits}";
    }
    
}