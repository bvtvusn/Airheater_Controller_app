using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airheater_SCADA.BLL
{
    internal class Pipeline
    {
        internal List<iSimulatable> simObjects { get; set; } = new List<iSimulatable>();
        public int HistorianLength { get; set; } = 1000;
        internal Queue<double[]> historian;
        System.Timers.Timer simTimer; //= new System.Timers.Timer();
        private double deltaT1;
        internal bool isRunning { get { return simTimer.Enabled; } }
        Queue<string> errorQueue;
        //Semaphore errorQueueSemaphore ;
        public double deltaT 
        { 
            get { 
                return deltaT1; 
            } 
            set { 
                deltaT1 = value;
                UpdateDeltaT();
            } 
        }

        public Pipeline(double dt) 
        {
            //deltaT1 = 0.05;
            errorQueue = new Queue<string>();
            historian = new Queue<double[]>(HistorianLength);
            simTimer = new System.Timers.Timer();
            simTimer.Elapsed += SimTimer_Elapsed;
            deltaT = dt;
            UpdateDeltaT();
        }

        private void SimTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            ProcessSingleTimestep();
        }

        internal double[] ProcessSingleTimestep()
        {
            double[] log = new double[simObjects.Count - 1];
            try
            {
                for (int i = 0; i < simObjects.Count - 1; i++)
                {
                    double temp = simObjects[i].GetValue();
                    log[i] = temp;
                    simObjects[i + 1].SetValueAndCompute(temp, deltaT);

                }
                lock (historian)
                {
                    historian.Enqueue(log);
                    if (historian.Count > HistorianLength)
                    {
                        historian.Dequeue();
                    }
                }
            }
            catch (Exception err)
            {
                //errorQueueSemaphore.WaitOne();
                lock (errorQueue)
                {
                    errorQueue.Enqueue(err.Message);
                    if (errorQueue.Count > 5)
                    {
                        errorQueue.Dequeue();
                    }
                }
                //errorQueueSemaphore.Release();
            }
            
            return log;
        }

        internal double[][] Simulate(int steps)
        {
            double[][] rows = new double[steps][];
            for (int i = 0; i < steps; i++)
            {
                rows[i] = ProcessSingleTimestep();
            }
            return rows;
        }

        

        internal void StartSimTimer()
        {
            simTimer.Start();
        }
        private void UpdateDeltaT()
        {
            simTimer.Interval = deltaT * 1000;
        }

        internal void StopSimTimer()
        {
            simTimer.Stop();
        }
        internal double[][] GetHistory()
        {
            lock (historian)
            {
                return historian.ToArray();
            }
            //double[][] data = new double[historian.Count][];
            //for (int i = 0; i < historian.Count; i++)
            //{
            //    int cols = historian.ElementAt(i).Length;
            //    double[] row = new double[cols];
            //    for (int j = 0; j < cols; j++)
            //    {
            //        row[j] = historian.ElementAt(i).ElementAt(j);
            //    }
            //    data.Append(row);
            //}
            //return data;
        }
        //System.Threading.Timer simTimer = new System.Threading.Timer();
        //Timer simTimer = new System.Windows.Forms.Timer();
        internal string GetErrors()
        {
            lock (errorQueue)
            {

            //errorQueueSemaphore.WaitOne();
            string myStr = "";
            foreach (var item in errorQueue)
            {
                myStr += item + "\r\n";
            }
            return myStr;
            //errorQueueSemaphore.Release();
            }
        }

        static Queue<double[]> DeepClone(Queue<double[]> original)
        {
            // Create a new queue to store the cloned data
            Queue<double[]> clone = new Queue<double[]>();

            // Clone each double[] array element in the original queue and add it to the clone queue
            foreach (var array in original)
            {
                double[] arrayClone = array.ToArray(); // Create a shallow clone of the array
                clone.Enqueue(arrayClone);
            }

            return clone;
        }
        internal Queue<double[]> GetHistorianDataSnapshot()
        {
            return DeepClone(historian);
        }
        internal string[] GetSimObjectNames()
        {
            string[] names = new string[simObjects.Count];
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = simObjects[i].ToString();
            }
            return names;   
        }
    }
}
