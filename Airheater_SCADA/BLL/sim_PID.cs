using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airheater_SCADA.BLL
{
    internal class sim_PID : iSimulatable
    {
        PID_BV controller;
        public sim_PID(double Kp, double Ti, double Td)
        {
            controller = new PID_BV(Kp, Ti, Td, true);
            controller.Setpoint = 35;
            controller.SetOutputLimits(0, 5);
            //controller
        }

        double state = 0;
        double iSimulatable.GetValue()
        {
            return state;
        }

        void iSimulatable.SetValueAndCompute(double value, double deltaT)
        {
            state = controller.Compute(deltaT, value);
        }
        public override string ToString()
        {
            return "PID Controller";
        }
        

        public double Kp
        {
            get { return controller.GetKp(); }
            set { controller.SetKp(value); }
        }
        public double Ti
        {
            get { return controller.GetTi(); }
            set { controller.SetTi(value); }
        }
        public double Td
        {
            get { return controller.GetTd(); }
            set { controller.SetTd(value); }
        }
        public double SP
        {
            get { return controller.Setpoint; }
            set { controller.Setpoint = value; }
        }
        public double Output
        {
            get { return controller.Output; }
            set { controller.Output = value; }
        }
        public bool InAuto
        {
            get { return controller.InAuto; }
            set { controller.InAuto = value; }
        }


    }
}
