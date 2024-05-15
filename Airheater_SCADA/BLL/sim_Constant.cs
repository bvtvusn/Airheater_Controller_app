using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airheater_SCADA.BLL
{
    internal class sim_Constant : iSimulatable
    {
        public sim_Constant(double constantValue)
        {
            ConstantValue = constantValue;
        }

        public double ConstantValue { get; set; }
        double iSimulatable.GetValue()
        {
            return ConstantValue;
        }

        void iSimulatable.SetValueAndCompute(double value, double deltaT)
        {
            throw new NotImplementedException();
        }
        public override string ToString()
        {
            return "Constant value";
        }
    }
}
