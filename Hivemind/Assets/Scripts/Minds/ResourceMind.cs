using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class ResourceMind
    {
        public ResourceMind(ResourceType resType, int carryweight)
        {
            this.prefferedType = resType;
            this.carryWeight = carryweight;
        }
        ResourceType prefferedType;
        int carryWeight;

        public ResourceType GetPrefferedType()
        {
            return prefferedType;
        }

        public int GetCarryWeight()
        {
            return carryWeight;
        }
    }
}
