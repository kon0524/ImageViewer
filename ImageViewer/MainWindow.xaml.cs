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
        private MainViewModel mainVM;

        public MainWindow()
        {
            InitializeComponent();
            mainVM = new MainViewModel(this.viewer);
            this.DataContext = mainVM;
        }

        /// <summary>
        /// マウスホイール（拡大・縮小）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                // 拡大
                mainVM.Scale *= 1.05;
            }
            else
            {
                // 縮小
                mainVM.Scale *= 0.95;
            }
        }
    }
}
