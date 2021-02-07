using MahApps.Metro.Controls;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for MDBCommander.xaml
    /// </summary>
    public partial class MDBCommander : MetroWindow
    {
        public MDBCommander()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            MDBTablesComboBox.ItemsSource = MDBToCH.GetCollections();

        }

        private void Execute_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Exec()
        {
            WaitScreen.Splash();

            string command = MDBBox.Text;

            var server = new MongoClient("mongodb://localhost:27017");
            var database = server.GetDatabase("test");

            var coll = database.GetCollection<BsonDocument>(table);
            var list = coll.Find(new BsonDocument()).Limit(5).ToList();
            //db.getCollection('asd').find({ })
        }
    }
}
