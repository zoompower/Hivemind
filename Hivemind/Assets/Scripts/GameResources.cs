using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.VR;
using static ResourceNode;

namespace Assets.Scripts
{
    public static class GameResources
    {
        public static event EventHandler OnResourceAmountChanged;
        private static int rockResources = 0;
        private static int crystalResources = 0;

        public static void AddResources(List<ResourceType> resources)
        {
            foreach(ResourceType resource in resources)
            {
                if(resource == ResourceType.Rock)
                {
                    rockResources++;
                }
                else if (resource == ResourceType.Crystal)
                {
                    crystalResources++;
                }
            }
            OnResourceAmountChanged.Invoke(null, EventArgs.Empty);
        }

        public static int GetRockAmount()
        {
            return rockResources;
        }

        public static int GetCrystalAmount()
        {
            return crystalResources;
        }
    }
}
