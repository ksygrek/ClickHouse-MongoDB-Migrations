using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for Logs.xaml
    /// </summary>
    public partial class Logs : MetroWindow
    {
        public Logs()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            string text = File.ReadAllText(WaitScreen.logsFilePath);
            LogsBox.Text = text;
            //using (StreamReader sr = File.OpenText(WaitScreen.logsFilePath))
            //{
            //    string s = "";
            //    while ((s = sr.ReadLine()) != null)
            //    {
            //        Console.WriteLine(s);
            //    }
            //}
        }

        public void AddLog(string log)
        {
            IEnumerable<string> m_oEnum = Enumerable.Empty<string>();
            using (StreamWriter fs = File.AppendText(WaitScreen.logsFilePath))
            {
                fs.WriteLine(log);
            }
        }
    }
}
