using CsvHelper;
using MahApps.Metro.Controls;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
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
    /// Interaction logic for MDBToCH.xaml
    /// </summary>
    public partial class MDBToCH : MetroWindow
    {
        public int lineCount = 0;

        public MDBToCH()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            MigrateMDBToCH.IsEnabled = false;
            MDBTablesComboBox.ItemsSource = GetCollections();

        }

        private void MigrateMDBToCH_Click(object sender, RoutedEventArgs e)
        {
            WaitScreen.Splash();
            string tableName = MDBTablesComboBox.SelectedItem.ToString();
            Stopwatch stopwatch = Stopwatch.StartNew();
            CSVToClickHouse.CHConn(() => GetData(tableName));
            stopwatch.Stop();
            var elapsedBulkMs = stopwatch.ElapsedMilliseconds;
            Timer.Content = "Migration time : " + elapsedBulkMs + "ms";
            
            NumberOfRecords.Content = "Records : " + (lineCount);

            Logs xd = new DBUplader.Logs();

            xd.AddLog("--------------------------------------------");
            xd.AddLog("Migrate MongoDB to ClickHouse");
            xd.AddLog("       Data name : " + tableName);
            xd.AddLog("       Bulk time : " + elapsedBulkMs + "ms");
            xd.AddLog("Records inserted : " + lineCount);
        }

        public static List<string> GetCollections()
        {
            WaitScreen.Splash();

            List<string> collections = new List<string>();
            var db = new MongoClient("mongodb://localhost:27017").GetDatabase("test");
            foreach (BsonDocument collection in db.ListCollectionsAsync().Result.ToListAsync<BsonDocument>().Result)
            {
                string name = collection["name"].AsString;
                collections.Add(name);
            }

            return collections;
        }

        private List<string> _Types;
        private List<string> _Names;
        
        public void GetData(string tableName)
        {
            WaitScreen.Splash();
            this._Types = null;
            this._Names = null;
            var db = new MongoClient("mongodb://localhost:27017").GetDatabase("test").GetCollection<BsonDocument>(tableName);
            
            var count = db.CountDocuments(FilterDefinition<BsonDocument>.Empty);
            var stepSize = 1000;

            for (int i = 0; i < Math.Ceiling((double)count / stepSize); i++)
            {
                var list = db.Find(new BsonDocument()).Skip(i * stepSize).Limit(stepSize).ToList();
                var values = this.getValues(list);
                if(i == 0)
                {
                    values = values.ToList();
                    CSVToClickHouse.CreateCHTable(this._Names, this._Types, tableName);
                }
                CSVToClickHouse.InsertValues(values, tableName, this._Names);
            }
            lineCount = (int)count;
        }

        private IEnumerable<object> getValues(List<BsonDocument> tab)
        {
            if (tab.Any())
            {
                var fItems = tab[0].Values.Where(p => p.BsonType != BsonType.ObjectId).Select(p => p.AsString).ToArray();
                if (_Types == null)
                {
                    _Names = tab[0].Names.Where(p => p != "_id").ToList();
                    _Types = CSVToClickHouse.CheckType(fItems);
                }
                foreach (var t in tab)
                {
                    object[] tabResult = new object[_Types.Count];
                    for (int i = 0; i < _Names.Count; i++)
                    {
                        var v = CSVToClickHouse.GetValue(_Types[i], t.GetValue(_Names[i]).AsString);
                        tabResult[i] = v;
                    }
                    yield return tabResult;
                }
            }
        }

        private void setButtonVisibility()
        {
            if (MDBTablesComboBox.SelectedIndex > -1)
            {
                MigrateMDBToCH.IsEnabled = true;
            }
            else
            {
                MigrateMDBToCH.IsEnabled = false;
            }
        }

        private void MDBTablesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            setButtonVisibility();
        }
    }
}
