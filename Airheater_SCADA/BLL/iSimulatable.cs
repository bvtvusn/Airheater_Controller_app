using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airheater_SCADA.BLL
{
    internal interface iSimulatable
    {
        internal void SetValueAndCompute(double value, double deltaT);
        internal double GetValue();

        
    }
}
