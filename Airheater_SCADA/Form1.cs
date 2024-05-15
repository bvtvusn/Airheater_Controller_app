using Airheater_SCADA.BLL;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;

namespace Airheater_SCADA
{
    public partial class Form1 : Form
    {
        BLogic bll;
        iSimulatable selectedSim;
        public Form1()
        {
            InitializeComponent();
            bll = new BLogic();
            //test2();
            ChangePipeline();

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 50;
            timer.Tick += Timer_Tick;
            timer.Start();


            //ModbusHandling mb = new ModbusHandling("10.0.0.43");
            //mb.WriteAnalogVoltage(2);
            //double data = mb.ReadAnalogVoltage();

        }

        private void ChangePipeline()
        {
            bll.pipeline.StopSimTimer();
            bll.pipeline.simObjects.Clear();
            listBox1.Items.Clear();
            if (chkSim.Checked)
            {
                InitSimulatedPipeline();
            }
            else
            {
                InitRealPipeline();
            }
            //bll.pipeline.StartSimTimer();
        }
        internal void InitSimulatedPipeline()
        {
            iSimulatable tempsim = new sim_TempSim();
            bll.pipeline.simObjects.Add(tempsim);
            bll.pipeline.simObjects.Add(new sim_Lowpass(0.1, 1));
            bll.pipeline.simObjects.Add(new sim_PID(0.5, 999, 0));
            bll.pipeline.simObjects.Add(tempsim);

            foreach (var item in bll.pipeline.simObjects)
            {
                listBox1.Items.Add(item);
            }
        }
        internal void InitRealPipeline()
        {
            string hostname = txtIP.Text;
            iSimulatable modbusIoDevice = new sim_modbus_IO(hostname);
            bll.pipeline.simObjects.Add(modbusIoDevice);
            bll.pipeline.simObjects.Add(new sim_Lowpass(0.1, 1));
            bll.pipeline.simObjects.Add(new sim_PID(0.5, 999, 0));
            bll.pipeline.simObjects.Add(modbusIoDevice);

            foreach (var item in bll.pipeline.simObjects)
            {
                listBox1.Items.Add(item);
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            var data = bll.pipeline.GetHistory();
            DrawPlots(data);
            txtError.Text = bll.GetErrors();
            txtSimError.Text = bll.pipeline.GetErrors();
        }

              
        

        internal void plotArrays(double[][] data, PlotView myPlotView)
        {
            int columns = data[0].Length;
            LineSeries[] series = new LineSeries[columns];

            var myModel = new PlotModel { Title = "Example 2" };
            for (int j = 0; j < columns; j++)
            {
                series[j] = new LineSeries();
                myModel.Series.Add(series[j]);
            }
            for (int i = 0; i < data.Length; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    var p = new DataPoint(i, data[i][j]);
                    series[j].Points.Add(p);
                }
            }
            myPlotView.Model = myModel;
        }

        internal void plotTransposedArrays(double[][] data, PlotView myPlotView, string title = "myTitle")
        {
            int rows = data.Length;
            int columns = data[0].Length;
            LineSeries[] series = new LineSeries[rows];

            var myModel = new PlotModel { Title = title };
            for (int i = 0; i < rows; i++)
            {
                series[i] = new LineSeries();
                myModel.Series.Add(series[i]);
            }
            for (int j = 0; j < columns; j++)
            {
                for (int i = 0; i < rows; i++)
                {
                    var p = new DataPoint(j, data[i][j]); // Transposed indices
                    series[i].Points.Add(p); // Series index corresponds to rows
                }
            }
            myPlotView.Model = myModel;
        }

       

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedSim = (iSimulatable)(sender as ListBox).SelectedItem;
            propertyGrid1.SelectedObject = selectedSim;
        }

        

        private void btnStart_Click(object sender, EventArgs e)
        {
            bll.pipeline.StartSimTimer();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            bll.pipeline.StopSimTimer();
        }

        
        internal void DrawPlots(double[][] data)
        {
            bool nullFlag = false;
            foreach (var item in data)
            {
                if (item is null) nullFlag = true;
            }
            if (!nullFlag)
            {
                double[][] col1 = new double[][] { data.Select(x => x[0]).ToArray(), data.Select(x => x[1]).ToArray() };
                double[][] col2 = new double[][] { data.Select(x => x[2]).ToArray() };

                plotTransposedArrays(col1, plotView1, "Temperature ");
                plotTransposedArrays(col2, plotView2, "Controller out");
            }
            
        }

       
        private void chkAzure_CheckedChanged(object sender, EventArgs e)
        {
            bll.SendToAure = (sender as CheckBox).Checked;
        }

        private void chkSim_CheckedChanged(object sender, EventArgs e)
        {
            ChangePipeline();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] headers = bll.pipeline.GetSimObjectNames();
            Queue<double[]> hist = bll.pipeline.GetHistorianDataSnapshot();
            string fname = DAL.FileHandling.GetFileNameWithTimestamp();
            DAL.FileHandling.SaveToCSV(fname, hist, headers);
        }
    }
}