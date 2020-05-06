using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Minds
{
    //everything that has this class has minds attachted to it
    public interface IHasMind
    {
         ResourceMind GetResourceMind();
         CombatMind GetCombatMind();
    }
}
