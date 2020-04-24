using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UnitGroup
    {
        public int MaxUnits { get; private set; }
        public int CurrentUnits { get; private set; }

        internal GameObject gameObject { get; private set; }
        private Text textBox;

        internal System.Guid ID { get; private set; }

        internal UnitGroup(GameObject unitIconBase)
        {
            gameObject = Object.Instantiate(unitIconBase);

            ID = System.Guid.NewGuid();

            textBox = gameObject.GetComponentInChildren<Text>();

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

        public void AddUnit()
        {
            SetCurrentUnits(CurrentUnits + 1);
        }

        public void RemoveUnit()
        {
            SetCurrentUnits(CurrentUnits - 1);
        }

        private void UpdateText()
        {
            textBox.text = $"{CurrentUnits}/{MaxUnits}";
        }
    }
}