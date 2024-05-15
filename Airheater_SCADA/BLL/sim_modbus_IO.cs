using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airheater_SCADA.BLL
{
    internal class sim_modbus_IO : iSimulatable
    {
        ModbusHandling mb;
        public sim_modbus_IO(string hostname)
        {
            mb = new ModbusHandling(hostname);
        }
        double iSimulatable.GetValue()
        {
            double raw = mb.ReadAnalogVoltage();
            double voltage5 = Map(raw, 0, 3.3, 0, 5);
            readTemperature =  Map(voltage5, 0, 5, 0, 50);
            //inputVoltage = mb.ReadAnalogVoltage();
            return readTemperature;
        }

        void iSimulatable.SetValueAndCompute(double value, double deltaT)
        {
            outputVoltage = value;
            mb.WriteAnalogVoltage(value);
        }


        private double readTemperature;

        public double Temperature
        {
            get { return readTemperature; }            
        }
        private double outputVoltage;

        public double OutputVoltage
        {
            get { return outputVoltage; }           
        }


        
        internal double Map(double value, double fromSource, double toSource, double fromTarget, double toTarget)
        {
            if (fromSource == toSource)
                throw new ArgumentException("fromSource and toSource must be different.");

            double mappedValue = (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
            return mappedValue;
        }
       

    }
}
