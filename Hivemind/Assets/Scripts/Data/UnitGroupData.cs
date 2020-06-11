using System;
using UnityEngine.UI;

namespace Assets.Scripts.Data
{
    [Serializable]
    public class UnitGroupData
    {
        public int MaxUnits;
        public int CurrentUnits;
        public string UnitGroupId;
        public string Text;

        public UnitGroupData(int maxUnits, int currentUnits, Guid unitGroupId, Text text)
        {
            MaxUnits = maxUnits;
            CurrentUnits = currentUnits;
            UnitGroupId = unitGroupId.ToString();
            if (text)
                Text = text.text;
        }
    }
}