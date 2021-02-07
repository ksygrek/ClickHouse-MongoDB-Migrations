using ClickHouse.Ado;
using CsvHelper;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
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
    /// Interaction logic for CSVToClickHouse.xaml
    /// </summary>
    public partial class CSVToClickHouse : MetroWindow
    {
        public static ClickHouseConnection con = null;
        public static ClickHouseDataReader reader = null;
        public static ClickHouseConnectionSettings c = null;
        public static String str = "Host=127.0.0.1;Port=9000;User=default;Password=;Database=default;Compress=True;CheckCompressedHash=False;SocketTimeout=60000000;Compressor=lz4";
        public static List<string> headerList = new List<string>();
        public static List<string> valueList = new List<string>();

        public CSVToClickHouse()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            BulkToClickHouse.IsEnabled = false;
        }

        private void ChooseFileCH_Click(object sender, RoutedEventArgs e)
        {
            WinForms.OpenFileDialog openFileDialog1 = new WinForms.OpenFileDialog
            {
                InitialDirectory = @"E:\",
                Title = "Browse Text Files",

                CheckFileExists = true,
                CheckPathExists = true,
                RestoreDirectory = true,
                ReadOnlyChecked = true,
                ShowReadOnly = true
            };
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                CSVPathTextBoxCH.Text = openFileDialog1.FileName;
            }
        }

        public static void CHConn(Action action)
        {
            try
            {
                con = new ClickHouseConnection(str);
                con.Open();
                Console.WriteLine("DBConnected");

                action();
            }
            catch (ClickHouseException err)
            {
                Console.WriteLine(err);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }

        private void BulkToClickHouse_Click(object sender, RoutedEventArgs e)
        {
            WaitScreen.Splash();

            string filePath = CSVPathTextBoxCH.Text;
            string tableName = TableNameTextBox.Text;
            CHConn(()=>ReadCSV3(filePath, tableName));
        }

        public static void CreateCHTable(List<string> headers, List<string> types, string tableName)
        {
            string createTableStr = "CREATE TABLE " + tableName + "( ";

            for (int i = 0; i < headers.Count; i++)
            {
                createTableStr += headers[i] + " " + types[i] + ", ";
            }

            createTableStr = createTableStr.Substring(0, createTableStr.Length - 2);
            createTableStr += " ) ENGINE = Log;";

            ClickHouseCommand comm = con.CreateCommand();
            comm.CommandText = createTableStr;

            try
            {
                comm.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        private void ReadCSV3(string filePath, string tableName)
        {
            List<object> result = new List<object>();
            result.Clear();
            const Int32 BufferSize = 128;
            try
            {
                using (var fileStream1 = File.OpenRead(filePath))
                    Console.WriteLine(filePath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                WrongFilePath();
                return;
            }
            List<string> types = null;
            using (var fileStream = File.OpenRead(filePath))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
            {
                String line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    char[] spearator = { ',' };
                    
                    if (headerList.Count > 0)
                    {

                        string[] strlist = line.Split(spearator, StringSplitOptions.None);
                        if (types == null)
                        {
                            types = CheckType(strlist);
                        }
                        object[] x = new object[types.Count];
                        for (int i = 0; i < types.Count; i++)
                        {
                            x[i] = GetValue(types[i], strlist[i]);
                        }
                        result.Add(x);
                    }
                    else
                    {
                        String[] strlist = line.Split(spearator, StringSplitOptions.None);

                        foreach (String s in strlist)
                            headerList.Add(s);
                        for (int i = 0; i < headerList.Count; i++)
                            Console.Write(headerList[i] + " ");
                        Console.WriteLine();
                    }
                }
            }

            var first = result.First();
            
            var lineCount = File.ReadLines(filePath).Count();

            CreateCHTable(headerList, types, tableName);
            Stopwatch stopwatch = Stopwatch.StartNew();
            InsertValues(result, tableName, headerList);
            stopwatch.Stop();


            var elapsedBulkMs = stopwatch.ElapsedMilliseconds;
            Timer.Content = "Time : " + elapsedBulkMs + "ms";
            int records = lineCount - 1;
            NumberOfRecordsCH.Content = "Records inserted : " + records;
            Logs xd = new DBUplader.Logs();

            xd.AddLog("--------------------------------------------");
            xd.AddLog("CSV to ClickHouse");
            xd.AddLog("       Data name : " + tableName);
            xd.AddLog("       Bulk time : " + elapsedBulkMs + "ms");
            xd.AddLog("Records inserted : " + records);

        }
        

        private static string CheckType(string obj)
        {
            int o = 0;
            if(int.TryParse(obj, out o))
            {
                return "Int32" ;
            }
            float f = 0F;
            if(float.TryParse(obj, out f))
            {
                return "Float64";
            }
            DateTime dt = DateTime.Now;
            if (DateTime.TryParse(obj, out dt))
            {
                return "DateTime";
            }
            return "String";
        }

        public static object GetValue(string type, string obj)
        {
            switch(type)
            {
                case "Int32":
                    int o = 0;
                    if (int.TryParse(obj, out o))
                    {
                        return o;
                    }
                    return o;
                case "Float64":
                    float f = 0F;
                    if (float.TryParse(obj, out f))
                    {
                        return f;
                    }
                    return f;
                case "DateTime":
                    DateTime dt = DateTime.Now;
                    if (DateTime.TryParse(obj, out dt))
                    {
                        return dt;
                    }
                    return null;
                default: return obj;
            }
        }

        public static List<string> CheckType(string[] first)
        {
            List<string> types = new List<string>();

            for (int i = 0; i < first.Length; i++)
            {
                types.Add(CheckType(first[i]));
            }
            return types;
        }

        public static void InsertValues(IEnumerable<object> data, string tableName, List<string> headers)
        {

            
            ClickHouseCommand comm = con.CreateCommand();
            string tempString = "";



            foreach (string s in headers)
            {
                tempString += s + ", ";
            }
            tempString = tempString.Substring(0, tempString.Length - 2);

            comm.CommandText = "INSERT INTO " + tableName + " ( " + tempString + " ) values @bulk";
            comm.Parameters.Add(new ClickHouseParameter
            {
                ParameterName = "bulk",
                Value = data
            });
            comm.ExecuteNonQuery(); 
        }
        

        private void TableNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            setButtonVisibility();
        }
        private void CSVPathTextBoxCH_TextChanged(object sender, TextChangedEventArgs e)
        {
            setButtonVisibility();
        }
        private void setButtonVisibility()
        {
            if ((TableNameTextBox.Text != String.Empty) && (CSVPathTextBoxCH.Text != String.Empty))
            {
                BulkToClickHouse.IsEnabled = true;
            }
            else
            {
                BulkToClickHouse.IsEnabled = false;
            }
        }

        private async void WrongFilePath()
        {
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "OK",
                AnimateShow = true,
                AnimateHide = false
            };
            var result = await this.ShowMessageAsync("Incorrect file path.",
                "Choose file or enter valid file path.",
                MessageDialogStyle.Affirmative, mySettings);
        }
    }
}
