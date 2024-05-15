using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airheater_SCADA.BLL
{

    public class PID_BV
    {
        private double myInput;
        private double myOutput;
        private double mySetpoint;
        private double kp;
        private double ti;
        private double td;
        private bool isDirect;
        private double dispKp;
        private double dispTi;
        private double dispTd;
        private ulong lastTime;
        private double lastInput;
        private double integralSum;
        private ulong SampleTime;
        private double outMin;
        private double outMax;
        private bool inAuto;

        public double debug;

        public double Setpoint { get => mySetpoint; set => mySetpoint = value; }
        

        public bool InAuto
        {
            get { return inAuto; }
            set { inAuto = value; }
        }
        

        public double Output
        {
            get { return myOutput; }
            set { myOutput = value; }
        }



        public PID_BV(double Kp, double Ti, double Td, bool ForwardActingController)
        {
            kp = Kp;
            ti = Ti;
            td = Td;

            //myInput = Input;
            //myOutput = Output;
            //mySetpoint = Setpoint;
            inAuto = true;
            SampleTime = 100;
            //lastTime = (DateTime.Now - TimeSpan.FromMilliseconds(SampleTime)).G;

            SetOutputLimits(0, 255);
            ForwardActionController(ForwardActingController);
            SetTunings(Kp, Ti, Td);
        }

        internal void SetKp(double value)
        {
            this.kp = value;
        }
        internal void SetTi(double value)
        {
            this.ti = value;
        }

        internal void SetTd(double value)
        {
            this.td = value;
        }

        public void SetOutputLimits(double Min, double Max)
        {
            if (Min >= Max) return;
            outMin = Min;
            outMax = Max;

            if (inAuto)
            {
                if (myOutput > outMax) myOutput = outMax;
                else if (myOutput < outMin) myOutput = outMin;
            }
        }

        public void ForwardActionController(bool mode)
        {
            isDirect = mode;
        }

        public void SetTunings(double Kp, double Ti, double Td)
        {
            if (Kp < 0 || Ti < 0 || Td < 0) return;

            kp = Kp;
            ti = Ti;
            td = Td;
        }

        public void SetSampleTime(int NewSampleTime)
        {
            if (NewSampleTime > 0)
            {
                SampleTime = (ulong)NewSampleTime;
            }
        }

        public double Compute( double dt, double pv)
        {

            if (!inAuto) return myOutput;
            //ulong now = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            double timeChange = dt * 1000; // milliseconds

            //if (timeChange >= SampleTime)
            //{
                double input = pv;
                double setpoint = Setpoint;
                double error = setpoint - input;
                double timeChange_Seconds = (timeChange / 1000.0);

                double internalKp = kp;
                if (!isDirect)
                {
                    internalKp = -kp;
                }

                double out_p = error * internalKp;
                double integralIncrement = error * (internalKp / ti) * timeChange_Seconds;

                if (integralIncrement > 0)
                {
                    double maxPositiveIntegralIncrement = Math.Max(0.0, outMax - (out_p + integralSum));
                    integralSum += Math.Min(maxPositiveIntegralIncrement, integralIncrement);
                }
                else
                {
                    double maxNegativeIntegralChange = Math.Max(0.0, (out_p + integralSum) - outMin);
                    integralSum -= Math.Min(maxNegativeIntegralChange, -integralIncrement);
                }

                debug = error;

                double out_d = -internalKp * td * (input - lastInput) / timeChange_Seconds;

                double PIDsum = out_p + integralSum + out_d;
                myOutput = Math.Max(outMin, Math.Min(outMax, PIDsum));

                lastInput = input;
                //lastTime = now;
                //return myOutput;
            //}
            return myOutput;
        }

        public void SetAutoState(bool newAutoState)
        {
            if (newAutoState && !inAuto)
            {
                Initialize();
            }
            inAuto = newAutoState;
        }

        public void Initialize()
        {
            double internalKp = kp;
            if (!isDirect)
            {
                internalKp = -kp;
            }
            double error = Setpoint - myInput;
            double out_p = error * internalKp;

            double targetOutput =myOutput;
            if (targetOutput > outMax) targetOutput = outMax;
            else if (targetOutput < outMin) targetOutput = outMin;

            integralSum = targetOutput - out_p;

            lastInput = myInput;
        }

        public double GetKp() { return kp; }
        public double GetTi() { return ti; }
        public double GetTd() { return td; }
        public double GetIntegral() { return integralSum; }
        public bool inAutoState() { return inAuto; }
        public bool isForwardDirection() { return isDirect; }
    }
}
