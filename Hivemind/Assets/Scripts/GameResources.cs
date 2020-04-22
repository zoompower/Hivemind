using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public static class GameResources
    {
        public static event EventHandler OnResourceAmountChanged;
        private static int resourceAmount = 0;

        public static void AddResourceAmount(int amount)
        {
            resourceAmount += amount;
            OnResourceAmountChanged.Invoke(null, EventArgs.Empty);
        }

        public static int GetResourceAmount()
        {
            return resourceAmount;
        }
    }
}
