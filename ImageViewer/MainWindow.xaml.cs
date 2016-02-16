using ImageViewer.ViewModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            moving = true;
            prev = e.GetPosition(this.canvas);
        }

        private void canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            moving = false;
        }

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

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            mainVM.KeyDown(e.Key);
        }
    }
}
