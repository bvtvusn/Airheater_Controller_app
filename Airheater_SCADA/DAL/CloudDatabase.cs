using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airheater_SCADA.DAL
{
    internal class CloudDatabase
    {
        public static string Connectionstring { get; } = "Server=tcp:iotwarehouse.database.windows.net,1433;Initial Catalog=iotdb;Persist Security Info=False;User ID=sgdf;Password=minusone;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=1;";
        //public static string Connectionstring { get; } = "Server=tcp:iotwarehouse.Xdatabase.windows.net,1433;Initial Catalog=iotdb;Persist Security Info=False;User ID=bvtv;Password=T-krok99;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=1;";

        internal void SendDatapoint()
        {
            
        }
        public static void InsertDatapoint(string connectionString, Datapoint datapoint)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = "INSERT INTO Datapoint (timestamp, value, sensor_id) VALUES (@Timestamp, @Value, @SensorId); SELECT SCOPE_IDENTITY()";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Timestamp", datapoint.Timestamp);
                cmd.Parameters.AddWithValue("@Value", datapoint.Value);
                cmd.Parameters.AddWithValue("@SensorId", datapoint.SensorId);

                // ExecuteScalar to retrieve the newly generated identity value
                int newId = Convert.ToInt32(cmd.ExecuteScalar());

                // Update the datapoint object with the new ID
                datapoint.ID = newId;

                con.Close();
            }
        }
        // SENSORS
        // 1	Temperature Sensor  Measures ambient temperature	°C	
        // 2	temp_sp Temperature Setpoint	°C	
        // 3	pid_out Controller output	%	
        // 4	temp_meas Measured temperature	°C	
        // 5	Temperature Sensor filtered Temperature measurement with lowpass filter	°C
        public static void BulkInsertDatapoints(string connectionString, List<Datapoint> datapoints)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                

                // Create SqlBulkCopy object
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(con))
                {
                    // Set the destination table name
                    bulkCopy.DestinationTableName = "Datapoint";

                    // Map columns from the source data to the destination table
                    bulkCopy.ColumnMappings.Add("Timestamp", "timestamp");
                    bulkCopy.ColumnMappings.Add("Value", "value");
                    bulkCopy.ColumnMappings.Add("SensorId", "sensor_id");

                    // Define the batch size (optional)
                    bulkCopy.BatchSize = 1000; // Adjust batch size as needed

                    // Create a DataTable to hold the data
                    DataTable dataTable = new DataTable();
                    dataTable.Columns.Add("Timestamp", typeof(DateTime));
                    dataTable.Columns.Add("Value", typeof(double));
                    dataTable.Columns.Add("SensorId", typeof(int));

                    // Populate the DataTable with the data from the list of Datapoints
                    foreach (Datapoint datapoint in datapoints)
                    {
                        dataTable.Rows.Add(datapoint.Timestamp, datapoint.Value, datapoint.SensorId);
                    }

                    // Write the data to the SQL Server
                    bulkCopy.WriteToServer(dataTable);
                }
            }
        }

    }
}
