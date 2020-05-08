using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    class CombatFlee : IMind
    {
        public void Execute(Ant ant)
        {
            ant.GetAgent().SetDestination(ant.GetStorage().GetPosition());
        }

    public void Initiate()
    {
        throw new NotImplementedException();
    }
}
