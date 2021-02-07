using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WinForms = System.Windows.Forms;


namespace DBUplader
{
    /// <summary>
    /// Interaction logic for CSVtoJSON.xaml
    /// </summary>
    /// 

    public partial class CSVtoJSON : Window
    {
        public CSVtoJSON()
        {
            InitializeComponent();
        }


        private void ChooseCSV_Click(object sender, RoutedEventArgs e)
        {
            WinForms.OpenFileDialog openFileDialog1 = new WinForms.OpenFileDialog
            {
                InitialDirectory = @"E:\",
                Title = "Browse Text Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "txt",
                Filter = "txt files (*.txt)|*.txt",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pathTextBox.Text = openFileDialog1.FileName;
            }
        }

        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            string[] cols;
            string[] rows;

            StreamReader sr = new StreamReader(pathTextBox.Text);  //SOURCE FILE
            StreamWriter sw = new StreamWriter(@"D:\clickhouse\sample1.json");  // DESTINATION FILE

            string line = sr.ReadLine();


            cols = Regex.Split(line, ",");

            DataTable table = new DataTable();
            for (int i = 0; i < cols.Length; i++)
            {
                table.Columns.Add(cols[i], typeof(string));
            }
            while ((line = sr.ReadLine()) != null)
            {
                int i;
                string row = string.Empty;
                rows = Regex.Split(line, ",");
                DataRow dr = table.NewRow();

                for (i = 0; i < rows.Length; i++)
                {
                    dr[i] = rows[i];

                }
                table.Rows.Add(dr);
            }
            string json = JsonConvert.SerializeObject(table, Formatting.Indented);
            sw.Write(json);

            sw.Close();
            sr.Close();

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            MessageBox.Show("Convertion completed." + " Time : " + elapsedMs / 1000 + "s");

            GC.Collect();
            GC.WaitForPendingFinalizers();
            this.Close();
        }
    }
}
