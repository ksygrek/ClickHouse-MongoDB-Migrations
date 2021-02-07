using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DBUplader
{
    public class WaitScreen
    {
        public static string logsFilePath = @"E:\DBUploaderLogs.txt";
        public static void Splash()
        {
            SplashScreen splashScreen = new SplashScreen("images\\splash.png");
            splashScreen.Show(true);
        }
    }
}
