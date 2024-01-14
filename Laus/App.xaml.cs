using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Laus
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            bool startMinimized = false;
            for (int i = 0; i != e.Args.Length; ++i)
            {
                if (e.Args[i] == "-h" || e.Args[i] == "--hidden")
                {
                    startMinimized = true;
                }
            }

            MainWindow mainWindow = new MainWindow();

            mainWindow.Show();

            if (startMinimized)
                mainWindow.Visibility = Visibility.Hidden;
        }
    }
}
