using FluentModbus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airheater_SCADA.BLL
{
    internal class ModbusHandling
    {
        ModbusTcpClient mb_client;
        System.Timers.Timer reconnectTimer;
        public ModbusHandling(string hostname = "localhost", int port = 502)
        {
            Hostname = hostname;
            Port = port;
            mb_client = new ModbusTcpClient();
            Reconnect();

            reconnectTimer = new System.Timers.Timer();
            reconnectTimer.Enabled = true;
            reconnectTimer.Interval = 10000;
            reconnectTimer.Elapsed += ReconnectTimer_Elapsed;
        }

        private void ReconnectTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (!mb_client.IsConnected)
            {
                mb_client.Disconnect();
                Reconnect();
            }
        }

        void Reconnect()
        {
            mb_client.Connect(Hostname + ":" + Port);
            //mb_client.IsConnected;
        }

        public string Hostname { get; }
        public int Port { get; }

        internal double ReadAnalogVoltage()
        {            
            //mb_client.ReadInputRegisters
            Span<ushort> memData = mb_client.ReadInputRegisters<ushort>(1, 10, 1);
            ushort[] rawData = memData.ToArray();
            rawData = SwapBytesInArray(rawData);
            return Convert.ToDouble(rawData[0]) / 100.0; ;
        }
        internal void WriteAnalogVoltage(double voltage)
        {
            double CappedVoltage = Math.Max(0, voltage);
            ushort[] WriteValues = new ushort[] { (ushort)(CappedVoltage * 100) };
            WriteValues = SwapBytesInArray(WriteValues);            
            mb_client.WriteMultipleRegisters<ushort>(1, 10, WriteValues);           
        }
        public static ushort[] SwapBytesInArray(ushort[] originalArray)
        {
            ushort[] swappedArray = new ushort[originalArray.Length];

            for (int i = 0; i < originalArray.Length; i++)
            {
                swappedArray[i] = (ushort)((originalArray[i] >> 8) | (originalArray[i] << 8));
            }
            return swappedArray;
        }
    }
}
