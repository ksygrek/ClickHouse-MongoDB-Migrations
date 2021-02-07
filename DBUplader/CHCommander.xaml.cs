using ClickHouse.Ado;
using MahApps.Metro.Controls;
using System;
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
    /// Interaction logic for CHCommander.xaml
    /// </summary>
    public partial class CHCommander : MetroWindow
    {
        public static ClickHouseConnection con = null;
        public static ClickHouseDataReader reader = null;
        public static ClickHouseConnectionSettings c = null;
        public static String str = "Host=127.0.0.1;Port=9000;User=default;Password=;Database=default;Compress=True;CheckCompressedHash=False;SocketTimeout=60000000;Compressor=lz4";

        public CHCommander()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            CHConnection(() => GetTableList());
        }



        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tableName = CHTablesComboBox.SelectedItem.ToString();
            }
            catch { }

            CHConnection(() => Exec());
        }
        public void Exec()
        {
            Stopwatch stopwatch = Stopwatch.StartNew(); //creates and start the instance of Stopwatch

            WaitScreen.Splash();
            string command = CHBox.Text;
            using (ClickHouseCommand comm = con.CreateCommand(command))
            {
                try
                {
                    comm.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Response.Content = "Wrong command.";
                    return;
                }
                
                stopwatch.Stop();

                var elapsedBulkMs = stopwatch.ElapsedMilliseconds;
                Logs xd = new DBUplader.Logs();

                xd.AddLog("--------------------------------------------");
                xd.AddLog("Execute ClickHouse Command");
                xd.AddLog(command);
                xd.AddLog("Execute time : " + elapsedBulkMs + "ms");

                Response.Content = "OK. " + elapsedBulkMs + "ms";

            }
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
    }
}
