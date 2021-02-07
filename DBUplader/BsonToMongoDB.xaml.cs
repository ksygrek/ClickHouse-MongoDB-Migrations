using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MongoDB.Bson;
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
using System.Windows.Shapes;
using WinForms = System.Windows.Forms;


namespace DBUplader
{
    public partial class BsonToMongoDB : MetroWindow
    {
        public BsonToMongoDB()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            BulkToMongoDB.IsEnabled = false;
        }

        private void BulkToMongoDB_Click(object sender, RoutedEventArgs e)

        {
            WaitScreen.Splash();
            UploadCSVToMongoDB();
        }

        private void ChooseFile_Click(object sender, RoutedEventArgs e)
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
                CSVPathTextBox.Text = openFileDialog1.FileName;
            }
        }

        private void UploadCSVToMongoDB()
        {
            var watchBulk = System.Diagnostics.Stopwatch.StartNew();

            string filePath = CSVPathTextBox.Text;
            string tableName = TableNameTextBox.Text;

            try
            {
                using (var fileStream1 = File.OpenRead(filePath));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                WrongFilePath();
                return;
            }

            var collection = new MongoClient("mongodb://localhost:27017").GetDatabase("test")
                .GetCollection<BsonDocument>(tableName);

            var lineCount = File.ReadLines(filePath).Count();
            List<string> headerList = new List<string>();
            List<string> valueList = new List<string>();
            var newDocs = new List<BsonDocument>();
            BsonDocument[] doc = new BsonDocument[lineCount];
            int wrongLines = 0;
            int i = 0;
            int maxBulk = 1000;
            
            const Int32 BufferSize = 128;
            using (var fileStream = File.OpenRead(filePath))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
            {
                String line;

                while ((line = streamReader.ReadLine()) != null)
                {

                    char[] spearator = { ',' };
                    String[] strlist = line.Split(spearator, StringSplitOptions.None);

                    if (headerList.Count > 0)
                    {
                        if (i == maxBulk)
                        {
                            collection.InsertMany(newDocs);
                            newDocs.Clear();
                            i = 0;
                        }

                        foreach (String s in strlist)
                            valueList.Add(s);

                        doc[i] = new BsonDocument
                        {

                        };

                        for (int j = 0; j < headerList.Count; j++)
                            try
                            {
                                doc[i].Add(headerList[j], valueList[j]);
                            }
                            catch (Exception ex)
                            {
                                doc[i].Clear();
                                Console.WriteLine(ex.Message);
                                wrongLines++;
                                break;
                            }
                        if (doc[i].Count() > 0)
                        {
                            newDocs.Add(doc[i]);
                        }
                        valueList.Clear();
                        i++;

                    }
                    else
                    {
                        foreach (String s in strlist)
                            headerList.Add(s);
                    }
                }

            }
            if (i < maxBulk)
                collection.InsertMany(newDocs);

            int records = lineCount - 1 - wrongLines;
            NumberOfRecords.Content = "Records inserted : " + records;
            doc = new BsonDocument[0];

            WrongLinesNumber.Content = "Number of incorrect lines in CSV file : " + wrongLines;
            wrongLines = 0;
            GC.Collect();

            watchBulk.Stop();
            var elapsedBulkMs = watchBulk.ElapsedMilliseconds;
            BsonToMongoTime.Content = "Bulk Bson data time : " + elapsedBulkMs + "ms";

            Logs xd = new DBUplader.Logs();

            xd.AddLog("--------------------------------------------");
            xd.AddLog("CSV to MongoDB");
            xd.AddLog("       Data name : " + tableName);
            xd.AddLog("       Bulk time : " + elapsedBulkMs + "ms");
            xd.AddLog("Records inserted : " + records);

        }

        private void CSVPathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            setButtonVisibility();
        }

        private void TableNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            setButtonVisibility();
        }
        private void setButtonVisibility()
        {
            if ((TableNameTextBox.Text != String.Empty) && (CSVPathTextBox.Text != String.Empty))
            {
                BulkToMongoDB.IsEnabled = true;
            }
            else
            {
                BulkToMongoDB.IsEnabled = false;
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
