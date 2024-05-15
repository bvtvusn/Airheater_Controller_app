using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airheater_SCADA.DAL
{
    internal class FileHandling
    {
        internal static void SaveToCSV(string filePath, Queue<double[]> data, string[] headers = null)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // Write headers if provided
                if (headers != null)
                {
                    writer.WriteLine(string.Join("\t", headers));
                }

                // Write each data point to a new line in the CSV file
                foreach (var dataPoint in data)
                {
                    writer.WriteLine(string.Join("\t", dataPoint));
                }
            }
        }
        internal static string GetFileNameWithTimestamp()
        {
            // Generate current date and time and format it as "yyMMdd_HHmmss"
            string timestamp = DateTime.Now.ToString("yyMMdd_HHmmss");
            return $"historian_data_{timestamp}.csv";
        }
    }
}
