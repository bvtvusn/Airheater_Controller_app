using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airheater_SCADA.BLL
{
    internal class sim_TempSim : iSimulatable
    {
        //private double state_sim_tankLevel = 0.0;
        //private double tankLevelMeas = 0.0;
        public double tankLevelMeas { get; set; }
        public double state_sim_tankLevel { get; set; }
        Queue<double> controlsignalQueue = new Queue<double>();

        //public double SimulateWaterTank(double controlSignal, double dt)
        //{
        //    //ulong now = (ulong)DateTimeOffset.Now.ToUnixTimeMilliseconds();
        //    //float dt = (float)(now - sim_lastTime) / 1000.0f;  // time in seconds
        //    //sim_lastTime = now;

        //    double effectiveControlSignal = controlSignal;
        //    double inflowRate = effectiveControlSignal * 0.5;  // Arbitrary factor for simulation
        //    double outflowRate = 0.8;

        //    state_sim_tankLevel += (inflowRate - outflowRate) * dt;  // Time step of the simulation

        //    // Ensure the water level doesn't go negative
        //    if (state_sim_tankLevel < 0.0)
        //    {
        //        state_sim_tankLevel = 0.0;
        //    }

        //    Random rand = new Random();
        //    return state_sim_tankLevel += (rand.Next(-1000, 1000) / 1000.0);
        //}
        public double SimulateWaterTank(double controlSignal, double dt)
        {
            double t_delay = 2;
            double t_env = 21.5;
            double k_h = 3.5;
            double t_const = 5; // 22 is recommended value

            controlsignalQueue.Enqueue(controlSignal);
            int IdealQueueLength = Convert.ToInt32(t_delay / dt);
            double delayedControlsignal = controlsignalQueue.Peek();
            while (controlsignalQueue.Count > IdealQueueLength)
            {
                delayedControlsignal = controlsignalQueue.Dequeue();
            }


            

            //"t_delay" add delay to controlsignal.
            double dS = ((t_env - state_sim_tankLevel) + k_h * delayedControlsignal) / t_const;
            state_sim_tankLevel += dS * dt;


            Random rand = new Random();
            return state_sim_tankLevel + (rand.Next(-1000, 1000) / 1000.0);
        }
        double iSimulatable.GetValue()
        {
            return tankLevelMeas;
        }

        void iSimulatable.SetValueAndCompute(double value, double deltaT)
        {
            tankLevelMeas = SimulateWaterTank(value, deltaT);
        }
        public override string ToString()
        {
            return "Air heater sim";
        }

    }
}
