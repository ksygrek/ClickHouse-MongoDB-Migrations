using ClickHouse.Ado;
using MahApps.Metro.Controls;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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

namespace DBUplader
{
    /// <summary>
    /// Interaction logic for CHToMDB.xaml
    /// </summary>
    public partial class CHToMDB : MetroWindow
    {
        public static ClickHouseConnection con = null;
        public static ClickHouseDataReader reader = null;
        public static ClickHouseConnectionSettings c = null;
        public static String str = "Host=127.0.0.1;Port=9000;User=default;Password=;Database=default;Compress=True;CheckCompressedHash=False;SocketTimeout=60000000;Compressor=lz4";
        public CHToMDB()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            CHConnection(GetTableList);
            MigrateCHToMDB.IsEnabled = false;
        }

        private void MigrateCHToMDB_Click(object sender, RoutedEventArgs e)
        {
            WaitScreen.Splash();
            string tableName = CHTablesComboBox.SelectedItem.ToString();

            Stopwatch stopwatch = Stopwatch.StartNew(); //creates and start the instance of Stopwatch

            CHConnection(() => InsertToMDB(GetTableColumns(tableName), tableName));
            
            stopwatch.Stop();

            var elapsedBulkMs = stopwatch.ElapsedMilliseconds;
            int records = CountMDB(tableName);

            Console.WriteLine("time : " + stopwatch.ElapsedMilliseconds);
            Timer.Content = "Migration time : " + elapsedBulkMs + "ms";
            NumberOfRecordsCH.Content = "Records inserted : " + records;

            Logs xd = new DBUplader.Logs();

            xd.AddLog("--------------------------------------------");
            xd.AddLog("Migrate ClickHouse to MongoDB");
            xd.AddLog("       Data name : " + tableName);
            xd.AddLog("       Bulk time : " + elapsedBulkMs + "ms");
            xd.AddLog("Records inserted : " + records);
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

        public static List<string> GetTableColumns(string table)
        {
            List<string> columns = null;
            using (ClickHouseCommand comm = con.CreateCommand($"SELECT name FROM system.columns where table = '{table}'"))
            {
                using (var reader = comm.ExecuteReader())
                {
                    columns = GetData(reader).OfType<string>().ToList();
                }
            }
            return columns;
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

        private static void InsertToMDB(List<string> headers, string tableName)
        {

            var collection = new MongoClient("mongodb://localhost:27017").GetDatabase("test")
                .GetCollection<BsonDocument>(tableName);

            int maxBulk = 1000;
            var newDocs = new List<BsonDocument>();
            BsonDocument[] doc = new BsonDocument[maxBulk];
            int i = 0;

            using (ClickHouseCommand comm = con.CreateCommand($"SELECT * FROM {tableName}"))
            {
                using (var reader = comm.ExecuteReader())
                {
                    do
                    {
                        while (reader.Read())
                        {
                            doc[i] = new BsonDocument
                            {

                            };
                            for (var k = 0; k < reader.FieldCount; k++)
                            {
                                var val = reader.GetValue(k);
                                doc[i].Add(headers[k], val.ToString());

                            }
                            newDocs.Add(doc[i]);
                            i++;
                            if (i == maxBulk)
                            {
                                collection.InsertMany(newDocs);
                                newDocs.Clear();
                                i = 0;
                            }
                        }
                    } while (reader.NextResult());
                }

                if (i < maxBulk)
                    collection.InsertMany(newDocs);
            }
        }

        public static int CountMDB(string tableName)
        {
            var server = new MongoClient("mongodb://localhost:27017");
            var database = server.GetDatabase("test");

            var coll = database.GetCollection<BsonDocument>(tableName);
            int records = (int)coll.Count(new BsonDocument());
            return records;
        }

        private void setButtonVisibility()
        {
            if (CHTablesComboBox.SelectedIndex > -1)
            {
                MigrateCHToMDB.IsEnabled = true;
            }
            else
            {
                MigrateCHToMDB.IsEnabled = false;
            }
        }

        private void CHTablesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            setButtonVisibility();
        }
    }
}
