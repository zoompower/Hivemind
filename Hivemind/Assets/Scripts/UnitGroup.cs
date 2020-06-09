using Assets.Scripts.Data;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UnitGroup
{
    public int MaxUnits { get; private set; }

    public int CurrentUnits { get; private set; }

    internal GameObject Ui_IconObj { get; private set; }

    internal Guid UnitGroupId { get; private set; }

    private Text textBox;

    internal UnitGroup(GameObject unitIconBase)
    {
        if (unitIconBase)
        {
            Ui_IconObj = UnityEngine.Object.Instantiate(unitIconBase);
            textBox = Ui_IconObj.GetComponentInChildren<Text>();
        }

        UnitGroupId = Guid.NewGuid();

        UpdateText();
    }

    public void RemoveMax()
    {
        SetMaxUnits(MaxUnits - 1);
    }

    public void AddMax()
    {
        SetMaxUnits(MaxUnits + 1);
    }

    public void SetMaxUnits(int amount)
    {
        MaxUnits = amount;
        UpdateText();
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
        MaxUnits += other.MaxUnits;
        CurrentUnits += other.CurrentUnits;
    }

    public UnitGroupData GetData()
    {
        return new UnitGroupData(MaxUnits, CurrentUnits, UnitGroupId, textBox);
    }

    public void SetData(UnitGroupData data)
    {
        MaxUnits = data.MaxUnits;
        CurrentUnits = data.CurrentUnits;
        UnitGroupId = Guid.Parse(data.UnitGroupId);
        if (textBox != null)
            textBox.text = data.Text;
    }
}