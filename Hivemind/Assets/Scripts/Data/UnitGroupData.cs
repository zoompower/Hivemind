using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Text = text.text;
        }
    }
}
