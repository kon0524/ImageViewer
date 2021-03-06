﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ImageViewer
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            string imagePath = (e.Args.Length != 0) ? e.Args[0] : null;
            if (imagePath != null)
            {
                string ext = System.IO.Path.GetExtension(imagePath).ToUpper();
                if (ext != ".JPG" && ext != ".JPEG")
                {
                    imagePath = null;
                }
            }

            // メイン画面を起動
            MainWindow mainWindow = new MainWindow(imagePath);
            mainWindow.Show();
        }
    }
}
