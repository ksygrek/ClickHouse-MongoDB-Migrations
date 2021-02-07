using ClickHouse.Ado;
using MahApps.Metro.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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

namespace DBUplader
{
    /// <summary>
    /// Interaction logic for CHSelectTest.xaml
    /// </summary>
    public partial class CHSelectTest : MetroWindow
    {
        public static ClickHouseConnection con = null;
        public static ClickHouseDataReader reader = null;
        public static ClickHouseConnectionSettings c = null;
        public static String str = "Host=127.0.0.1;Port=9000;User=default;Password=;Database=default;Compress=True;CheckCompressedHash=False;SocketTimeout=60000000;Compressor=lz4";

        public CHSelectTest()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            SelectCH.IsEnabled = false;
            CHConnection(GetTableList);
        }

        private void SelectCH_Click(object sender, RoutedEventArgs e)
        {
            string tableName = CHTablesComboBox.SelectedItem.ToString();

            Stopwatch stopwatch = Stopwatch.StartNew();

            CHConnection(() => Select1(tableName));

            stopwatch.Stop();
            var elapsedBulkMs = stopwatch.ElapsedMilliseconds;
            Timer1.Content = "time : " + stopwatch.ElapsedMilliseconds + " ms";

            stopwatch = Stopwatch.StartNew();

            CHConnection(() => Select2(tableName));

            stopwatch.Stop();
            elapsedBulkMs = stopwatch.ElapsedMilliseconds;
            Timer2.Content = "time : " + stopwatch.ElapsedMilliseconds + " ms";

            stopwatch = Stopwatch.StartNew();

            CHConnection(() => Select3(tableName));

            stopwatch.Stop();
            elapsedBulkMs = stopwatch.ElapsedMilliseconds;
            Timer3.Content = "time : " + stopwatch.ElapsedMilliseconds + " ms";
        }

        public static void CHConnection(Action myMethodName)
        {
            WaitScreen.Splash();

            try
            {
                con = new ClickHouseConnection(str);
                con.Open();
                Console.WriteLine("DBConnected");


                myMethodName();
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

        public void Select1(string table)
        {
            List<string> columns = null;
            using (ClickHouseCommand comm = con.CreateCommand($"SELECT * FROM system.columns where table = '{table}'"))
            {
                using (var reader = comm.ExecuteReader())
                {
                    columns = GetData(reader).OfType<string>().ToList();
                }
            }
        }
        public void Select2(string table)
        {
            using (ClickHouseCommand comm = con.CreateCommand($"SELECT count(*) FROM {table}"))
            {
                using (var reader = comm.ExecuteReader())
                {
                    PrintData(reader);
                }
            }
        }

        public void Select3(string table)
        {
            using (ClickHouseCommand comm = con.CreateCommand($"SELECT * FROM {table} LIMIT 10000"))
            {
                using (var reader = comm.ExecuteReader())
                {
                    List<object> result = new List<object>();
                    do
                    {
                        while (reader.Read())
                        {
                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                var val = reader.GetValue(i);
                                result.Add(val);
                            }
                        }
                    } while (reader.NextResult());
                }
            }
        }

        private static void PrintData(IDataReader reader)
        {
            do
            {
                Console.Write("Fields: ");
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    Console.Write("{0}:{1} ", reader.GetName(i), reader.GetDataTypeName(i));
                }
                Console.WriteLine();
                while (reader.Read())
                {
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        var val = reader.GetValue(i);
                        if (val.GetType().IsArray)
                        {
                            Console.Write('[');
                            Console.Write(string.Join(", ", ((IEnumerable)val).Cast<object>()));
                            Console.Write(']');
                        }
                        else
                        {
                            Console.Write(val);
                        }
                        Console.Write(", ");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            } while (reader.NextResult());
        }

        private void CHTablesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            setButtonVisibility();
        }

        public void GetTableList()
        {
            List<object> tableList = null;
            using (ClickHouseCommand comm = con.CreateCommand("SELECT name FROM system.tables"))
            {
                using (var reader = comm.ExecuteReader())
                {
                    tableList = GetData(reader);
                    CHTablesComboBox.ItemsSource = tableList;
                }
            }
        }

        public static List<object> GetData(IDataReader reader)
        {
            List<object> result = new List<object>();
            do
            {
                while (reader.Read())
                {
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        var val = reader.GetValue(i);
                        result.Add(val);
                    }
                }
            } while (reader.NextResult());
            return result;
        }

        private void setButtonVisibility()
        {
            if (CHTablesComboBox.SelectedIndex > -1)
            {
                SelectCH.IsEnabled = true;
            }
            else
            {
                SelectCH.IsEnabled = false;
            }
        }
    }
}
