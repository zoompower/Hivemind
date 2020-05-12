using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public interface IMind
{
    void Execute(Ant ant);
    void Initiate();

    double Likelihood(Ant ant);

    IMind Clone();

    bool Equals(IMind mind);

    void Update(IMind mind);
}
