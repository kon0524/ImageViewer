using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ImageViewer.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        // 入力画像
        private BitmapImage inputImage;
        public BitmapImage InputImage 
        {
            get { return inputImage; }
            private set
            {
                inputImage = value;
                NotifyPropertyChanged("InputImage");
            }
        }

        // 表示倍率
        private double scale;
        public double Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                NotifyPropertyChanged("Scale");
            }
        }

        // デバッグ文
        private string debug;
        public string Debug
        {
            get { return debug; }
            set
            {
                debug = value;
                NotifyPropertyChanged("Debug");
            }
        }

        // 等倍表示
        public ICommand ScaleReset { get; private set; }

        private Image viewer;

        public MainViewModel(Image viewer) 
        {
            // test
            InputImage = new BitmapImage(new Uri(@"C:\Users\sound-k\Pictures\7284e00d8957708e01fb76d9615d7168.JPG"));
            Scale = 1.0;

            this.viewer = viewer;

            // command
            ScaleReset = new DelegateCommand(scaleResetExecute, null);

            // event
            App.Current.MainWindow.SizeChanged += new SizeChangedEventHandler(windowSizeChenged);
        }

        /// <summary>
        /// 等倍表示にする
        /// </summary>
        /// <param name="param"></param>
        private void scaleResetExecute(object param)
        {
            Scale = 1.0;
        }

        private void windowSizeChenged(object o, SizeChangedEventArgs args)
        {
            App.Current.Dispatcher.Invoke(() => 
            {
                Size s = viewer.RenderSize;
            });
        }
    }
}
