using MahApps.Metro.Controls;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DBUplader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            CreateFile();
        }

        private void CreateFile()
        {
            try
            {
                using (FileStream fs = File.Create(WaitScreen.logsFilePath))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes("");
                    fs.Write(info, 0, info.Length);
                }

                using (StreamReader sr = File.OpenText(WaitScreen.logsFilePath))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(s);
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void BsonToMDB_Click(object sender, RoutedEventArgs e)
        {
            BsonToMongoDB win = new BsonToMongoDB();
            win.Show();
        }

        private void CSVToCH_Click(object sender, RoutedEventArgs e)
        {
            CSVToClickHouse win = new CSVToClickHouse();
            win.Show();
        }

        private void CHToMDB_Click(object sender, RoutedEventArgs e)
        {
            CHToMDB win = new CHToMDB();
            win.Show();
        }

        private void MDBToCH_Click(object sender, RoutedEventArgs e)
        {
            MDBToCH win = new MDBToCH();
            win.Show();
        }

        private void Logs_Click(object sender, RoutedEventArgs e)
        {
            Logs win = new Logs();
            win.Show();
        }

        private void MDBSelectTest_Click(object sender, RoutedEventArgs e)
        {
            MDBSelectTest win = new MDBSelectTest();
            win.Show();
        }

        private void CHSelectTest_Click(object sender, RoutedEventArgs e)
        {
            CHSelectTest win = new CHSelectTest();
            win.Show();
        }

        private void CHPreview_Click(object sender, RoutedEventArgs e)
        {
            CHPreview win = new CHPreview();
            win.Show();
        }

        private void MDBPreview_Click(object sender, RoutedEventArgs e)
        {
            MDBPreview win = new MDBPreview();
            win.Show();
        }

        private void CHCommander_Click(object sender, RoutedEventArgs e)
        {
            CHCommander win = new CHCommander();
            win.Show();
        }

        private void MDBCommander_Click(object sender, RoutedEventArgs e)
        {
            MDBCommander win = new MDBCommander();
            win.Show();
        }
    }
}