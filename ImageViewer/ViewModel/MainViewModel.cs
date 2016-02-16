using System;
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

        // 画像サイズ
        private Size imageSize;
        public Size ImageSize
        {
            get { return imageSize; }
            private set
            {
                imageSize = value;
                NotifyPropertyChanged("ImageSize");
            }
        }

        // 画像位置
        private Point imagePos;
        public Point ImagePos
        {
            get { return imagePos; }
            private set
            {
                imagePos = value;
                NotifyPropertyChanged("ImagePos");
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

        // 等倍・フィット切替
        public ICommand ScaleChange { get; private set; }

        private Canvas canvas;
        private bool isFit;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="imagePath"></param>
        public MainViewModel(Canvas canvas, string imagePath) 
        {
            if (imagePath == null) imagePath = @"C:\Users\sound-k\Pictures\7284e00d8957708e01fb76d9615d7168.JPG";
            InputImage = new BitmapImage(new Uri(imagePath));
            
            this.canvas = canvas;

            // command
            ScaleChange = new DelegateCommand(scaleChangeExecute, null);

            // event
            App.Current.MainWindow.Loaded += new RoutedEventHandler(windowLoaded);
            App.Current.MainWindow.SizeChanged += new SizeChangedEventHandler(windowSizeChenged);
        }

        /// <summary>
        /// 拡大・縮小
        /// </summary>
        /// <param name="delta"></param>
        public void Zoom(int delta)
        {
            double ratio = (delta > 0) ? 1.1 : 0.9;
            ImageSize = new Size(ImageSize.Width * ratio, ImageSize.Height * ratio);
            ImagePos = UpdateImagePos();
            isFit = false;
        }

        /// <summary>
        /// 移動
        /// </summary>
        /// <param name="diff"></param>
        public void Move(Point diff)
        {
            // フィット表示中は移動しない
            if (isFit) return;

            // 画像が表示領域からはみ出していなければ移動しない
            if (canvas.RenderSize.Width >= ImageSize.Width
                && canvas.RenderSize.Height >= ImageSize.Height) return;

            ImagePos = new Point(ImagePos.X + diff.X, ImagePos.Y + diff.Y);
        }

        private void ImageFitSize()
        {
            double canvasAspect = canvas.RenderSize.Height / canvas.RenderSize.Width;
            double imageAspect = InputImage.Height / InputImage.Width;

            if (canvasAspect < imageAspect)
            {   // 左右に余白が付くケース
                ImageSize = new Size(canvas.RenderSize.Height / imageAspect, canvas.RenderSize.Height);
                ImagePos = UpdateImagePos();
            }
            else
            {   // 上下に余白が付くケース
                ImageSize = new Size(canvas.RenderSize.Width, canvas.RenderSize.Width * imageAspect);
                ImagePos = UpdateImagePos();
            }

            isFit = true;
        }

        private void ImageDotByDot()
        {
            ImageSize = new Size(InputImage.PixelWidth, InputImage.PixelHeight);
            ImagePos = UpdateImagePos();
            isFit = false;
        }

        private Point UpdateImagePos()
        {
            double x = (canvas.RenderSize.Width - ImageSize.Width) / 2;
            double y = (canvas.RenderSize.Height - ImageSize.Height) / 2;
            return new Point(x, y);
        }

        /// <summary>
        /// 等倍・フィット切替
        /// </summary>
        /// <param name="param"></param>
        private void scaleChangeExecute(object param)
        {
            if (isFit)
            {
                ImageDotByDot();
            }
            else
            {
                ImageFitSize();
            }
        }

        private void windowLoaded(object o, RoutedEventArgs args)
        {
            ImageFitSize();
        }

        private void windowSizeChenged(object o, SizeChangedEventArgs args)
        {
            if (isFit) ImageFitSize();
            else ImagePos = UpdateImagePos();
        }
    }
}
