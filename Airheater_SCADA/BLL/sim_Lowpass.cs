using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airheater_SCADA.BLL
{
    internal class sim_Lowpass : iSimulatable
    {
        private double timeConstant;

        //double state = 0.0;
        //double filterCoefficient = 0.0;
        public double timestep { get; set; }
        public double filterCoefficient { get; set; }
        public double state { get; set; }
        public double TimeConstant { get => timeConstant; 
            set { 
                timeConstant = value; 
                CalcFilterCoefficient(); 
            } }
        void CalcFilterCoefficient()
        {
            filterCoefficient = timestep / (timeConstant + timestep);
        }
        public sim_Lowpass(double timestep, double timecontant)
        {
            this.timestep = timestep;
            this.TimeConstant = timecontant;
            
            
            state = 0.0;
        }
        double iSimulatable.GetValue()
        {
            return state;
        }

        void iSimulatable.SetValueAndCompute(double value, double deltaT)
        {
            state = (1 - filterCoefficient) * state + filterCoefficient * value;
        }
        public override string ToString()
        {
            return "Lowpass filter";
        }
    }
}
