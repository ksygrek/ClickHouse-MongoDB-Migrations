using ClickHouse.Ado;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for CHPreview.xaml
    /// </summary>
    public partial class CHPreview : MetroWindow
    {
        public static ClickHouseConnection con = null;
        public static ClickHouseDataReader reader = null;
        public static ClickHouseConnectionSettings c = null;
        public static String str = "Host=127.0.0.1;Port=9000;User=default;Password=;Database=default;Compress=True;CheckCompressedHash=False;SocketTimeout=60000000;Compressor=lz4";

        public CHPreview()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            CHConnection(GetTableList);
        }

        private void CHTablesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string tableName = CHTablesComboBox.SelectedItem.ToString();
            CHBox.Clear();
            CHConnection(() => Preview(tableName));
        }

        public void Preview(string table)
        {
            WaitScreen.Splash();

            using (ClickHouseCommand comm = con.CreateCommand($"SELECT * FROM {table} LIMIT 5"))
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
                                CHBox.AppendText(result[i].ToString() + ", ");
                            }
                            CHBox.AppendText(Environment.NewLine);
                            result.Clear();
                        }
                    } while (reader.NextResult());
                }
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
