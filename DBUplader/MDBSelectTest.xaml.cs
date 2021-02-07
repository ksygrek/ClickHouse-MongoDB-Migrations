using MahApps.Metro.Controls;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for MDBSelectTest.xaml
    /// </summary>
    public partial class MDBSelectTest : MetroWindow
    {
        public MDBSelectTest()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            SelectMDB.IsEnabled = false;
            MDBTablesComboBox.ItemsSource = MDBToCH.GetCollections();
        }

        private void MDBTablesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            setButtonVisibility();
        }

        private void SelectMDB_Click(object sender, RoutedEventArgs e)
        {
            string tableName = MDBTablesComboBox.SelectedItem.ToString();

            Stopwatch stopwatch = Stopwatch.StartNew();

            Select1();

            stopwatch.Stop();
            var elapsedBulkMs = stopwatch.ElapsedMilliseconds;
            Timer1.Content = "time : " + stopwatch.ElapsedMilliseconds + " ms";

            stopwatch = Stopwatch.StartNew();

            Select2(tableName);

            stopwatch.Stop();
            elapsedBulkMs = stopwatch.ElapsedMilliseconds;
            Timer2.Content = "time : " + stopwatch.ElapsedMilliseconds + " ms";

            stopwatch = Stopwatch.StartNew();

            Select3(tableName);

            stopwatch.Stop();
            elapsedBulkMs = stopwatch.ElapsedMilliseconds;
            Timer3.Content = "time : " + stopwatch.ElapsedMilliseconds + " ms";
        }

        private void Select1()
        {
            Console.WriteLine(MDBToCH.GetCollections());
        }

        private void Select2(string table)
        {
            //List<string> ls = new List<string>();
            var server = new MongoClient("mongodb://localhost:27017");
            var database = server.GetDatabase("test");

            var coll = database.GetCollection<BsonDocument>(table);
            var list = coll.Find(new BsonDocument()).Limit(10000).ToList();
        }

        private void Select3(string table)
        {
            var server = new MongoClient("mongodb://localhost:27017");
            var database = server.GetDatabase("test");

            var coll = database.GetCollection<BsonDocument>(table);
            int records = (int)coll.Count(new BsonDocument());
        }

        private void setButtonVisibility()
        {
            if (MDBTablesComboBox.SelectedIndex > -1)
            {
                SelectMDB.IsEnabled = true;
            }
            else
            {
                SelectMDB.IsEnabled = false;
            }
        }
    }
}
