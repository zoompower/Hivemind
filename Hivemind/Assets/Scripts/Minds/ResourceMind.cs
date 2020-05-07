using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Gathering;

namespace Assets.Scripts
{
    public class ResourceMind
    {
        private ResourceType prefferedType;
        private int carryWeight;
        Direction PrefferedDirection;

        public ResourceMind(ResourceType resType, int carryweight)
        {
            prefferedType = resType;
            carryWeight = carryweight;
        }
        

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
