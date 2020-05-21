﻿using System;
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

    internal UnitGroup(GameObject unitIconBase)
    {
        Ui_IconObj = UnityEngine.Object.Instantiate(unitIconBase);

        UnitGroupId = Guid.NewGuid();

        textBox = Ui_IconObj.GetComponentInChildren<Text>();

        UpdateText();
    }

    public bool RemoveMax()
    {
        return SetMaxUnits(MaxUnits - 1);
    }

    public bool AddMax()
    {
        return SetMaxUnits(MaxUnits + 1);
    }

    public bool SetMaxUnits(int amount)
    {
        MaxUnits = amount;
        UpdateText();

        return true;
    }

    public bool AddUnit()
    {
        return SetCurrentUnits(CurrentUnits + 1);
    }

    public void RemoveUnit()
    {
        CurrentUnits--;
        UpdateText();
    }

    public bool SetCurrentUnits(int amount)
    {
        return SetCurrentUnits(amount, false);
    }

    public bool SetCurrentUnits(int amount, bool force)
    {
        if (!force && (amount > MaxUnits || amount < 0)) return false;

        CurrentUnits = amount;
        UpdateText();
        return true;
    }

    private void UpdateText()
    {
        if (textBox != null)
            textBox.text = $"{CurrentUnits}/{MaxUnits}";
    }

    internal void MergeGroupIntoThis(UnitGroup other)
    {
        this.MaxUnits += other.MaxUnits;
        this.CurrentUnits += other.CurrentUnits;
    }

}