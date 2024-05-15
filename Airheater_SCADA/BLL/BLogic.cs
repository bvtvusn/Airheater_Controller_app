using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airheater_SCADA.BLL
{
    internal class BLogic
    {
        internal Pipeline pipeline;
        System.Timers.Timer timer_azure;
        //private string errorMsg;
        Queue<string> errorQueue;

        internal bool SendToAure { set { timer_azure.Enabled = value; }
            get { return timer_azure.Enabled; }
        }
        //public string ErrorMsg
        //{
        //    get { return errorMsg; }
            
        //}

        public BLogic()
        {
            pipeline = new Pipeline(0.1);
            

            errorQueue = new Queue<string>();
            timer_azure = new System.Timers.Timer();
            timer_azure.Interval = 1000;
            timer_azure.Elapsed += Timer_azure_Elapsed;
            //timer_azure.Start();
        }

        private void Timer_azure_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (pipeline.isRunning)
            {
                try
                {
                    SendDataToAzure();

                }
                catch (Exception err)
                {
                    //errorMsg = err.Message;
                    lock (errorQueue)
                    {
                        errorQueue.Enqueue(err.Message);
                        if (errorQueue.Count > 5)
                        {
                            errorQueue.Dequeue();
                        }
                    }
                }
            }
        }
        internal string GetErrors()
        {
            lock (errorQueue)
            {

                string myStr = "";
                foreach (var item in errorQueue)
                {
                    myStr += item + "\r\n";
                }
                return myStr;
            }
        }

        internal void SendDataToAzure()
        {
            double temp_raw = pipeline.simObjects[0].GetValue();
            double temp_filt = pipeline.simObjects[1].GetValue();
            double pid_out = pipeline.simObjects[2].GetValue();
            double pid_SP = (pipeline.simObjects[2] as sim_PID).SP;

            List<DAL.Datapoint> points = new List<DAL.Datapoint>();
            points.Add(new DAL.Datapoint(-1, DateTime.Now, temp_raw, 4));
            points.Add(new DAL.Datapoint(-1, DateTime.Now, temp_filt, 5));
            points.Add(new DAL.Datapoint(-1, DateTime.Now, pid_out, 3));
            points.Add(new DAL.Datapoint(-1, DateTime.Now, pid_SP, 2));

            string constr = DAL.CloudDatabase.Connectionstring;
            DAL.CloudDatabase.BulkInsertDatapoints(constr, points);
        }
    }
}
