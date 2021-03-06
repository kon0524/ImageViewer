﻿using ImageViewer.ViewModel;
using System.Windows;
using System.Windows.Input;

namespace ImageViewer
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool moving;
        private Point prev;

        private MainViewModel mainVM;

        public MainWindow(string imagePath)
        {
            InitializeComponent();
            mainVM = new MainViewModel(this.canvas, imagePath);
            this.DataContext = mainVM;
            moving = false;
        }

        /// <summary>
        /// マウスホイール（拡大・縮小）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            mainVM.Zoom(e.Delta, e.GetPosition(this.viewer));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 移動開始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            moving = true;
            prev = e.GetPosition(this.canvas);
        }

        /// <summary>
        /// 移動終了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            moving = false;
        }

        /// <summary>
        /// 移動中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (moving)
            {
                Point current = e.GetPosition(this.canvas);
                Point diff = new Point(current.X - prev.X, current.Y - prev.Y);
                prev = current;
                mainVM.Move(diff);
            }
        }

        /// <summary>
        /// 移動強制終了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            moving = false;
        }

        /// <summary>
        /// キー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            mainVM.KeyDown(e.Key);
        }

        private void Window_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
                e.Effects = DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files != null)
            {
                mainVM.DropImage(files[0]);
            }
        }
    }
}
