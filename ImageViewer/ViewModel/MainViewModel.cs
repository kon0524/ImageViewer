using System;
using System.Collections.Generic;
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

        // 画像位置
        private Rect imageRect;
        public Rect ImageRect
        {
            get { return imageRect; }
            private set
            {
                imageRect = value;
                NotifyPropertyChanged("ImageRect");
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
        private List<string> imageList;
        private int index;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="imagePath"></param>
        public MainViewModel(Canvas canvas, string imagePath) 
        {
            if (imagePath != null)
            {
                InputImage = new BitmapImage(new Uri(imagePath));
                string directory = System.IO.Path.GetDirectoryName(imagePath);
                imageList = new List<string>();
                foreach (string file in System.IO.Directory.GetFiles(directory))
                {
                    string ext = System.IO.Path.GetExtension(file).ToUpper();
                    if (ext == ".JPG" || ext == ".JPEG") imageList.Add(file);
                }
                index = imageList.IndexOf(imagePath);
            }
            this.canvas = canvas;

            // command
            ScaleChange = new DelegateCommand(scaleChangeExecute, null);

            // event
            App.Current.MainWindow.Loaded += new RoutedEventHandler(windowLoaded);
            App.Current.MainWindow.SizeChanged += new SizeChangedEventHandler(windowSizeChenged);
        }

        public void DropImage(string imagePath)
        {
            if (imagePath != null)
            {
                string ext = System.IO.Path.GetExtension(imagePath).ToUpper();
                if (ext != ".JPG" && ext != ".JPEG") return;

                InputImage = new BitmapImage(new Uri(imagePath));
                string directory = System.IO.Path.GetDirectoryName(imagePath);
                imageList = new List<string>();
                foreach (string file in System.IO.Directory.GetFiles(directory))
                {
                    ext = System.IO.Path.GetExtension(file).ToUpper();
                    if (ext == ".JPG" || ext == ".JPEG") imageList.Add(file);
                }
                index = imageList.IndexOf(imagePath);
                ImageFitSize();
            }
        }

        /// <summary>
        /// 拡大・縮小
        /// </summary>
        /// <param name="delta"></param>
        public void Zoom(int delta, Point center)
        {
            double ratio = (delta > 0) ? 1.1 : 0.9;
            Size prevImageSize = ImageRect.Size;
            Point prevImagePos = ImageRect.Location;
            Size newImageSize = new Size(prevImageSize.Width * ratio, prevImageSize.Height * ratio);

            // 画像が表示領域からはみ出していなければ移動しない
            Point newPos;
            if (canvas.RenderSize.Width >= newImageSize.Width
                && canvas.RenderSize.Height >= newImageSize.Height)
            {
                newPos = UpdateImagePos(newImageSize);
            }
            else
            {
                newPos = new Point(prevImagePos.X + (1 - ratio) * center.X, prevImagePos.Y + (1 - ratio) * center.Y);
            }
            isFit = false;

            ImageRect = new Rect(newPos, newImageSize);
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
            if (canvas.RenderSize.Width >= ImageRect.Size.Width
                && canvas.RenderSize.Height >= ImageRect.Size.Height) return;

            ImageRect = new Rect(new Point(ImageRect.Location.X + diff.X, ImageRect.Location.Y + diff.Y), ImageRect.Size);
        }

        /// <summary>
        /// キー押下イベント
        /// </summary>
        /// <param name="key"></param>
        public void KeyDown(Key key)
        {
            switch (key)
            {
                case Key.Left:
                    // 次の画像を開く
                    if (index > 0)
                    {
                        index--;
                        InputImage = new BitmapImage(new Uri(imageList[index]));
                        ImageFitSize();
                    }
                    break;
                case Key.Right:
                    // 前の画像を開く
                    if (index < imageList.Count - 1)
                    {
                        index++;
                        InputImage = new BitmapImage(new Uri(imageList[index]));
                        ImageFitSize();
                    }
                    break;
                case Key.Space:
                    // 等倍・フィットを切替える
                    scaleChangeExecute(null);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 画像を中央フィット表示にする
        /// </summary>
        private void ImageFitSize()
        {
            if (InputImage == null) return;

            double canvasAspect = canvas.RenderSize.Height / canvas.RenderSize.Width;
            double imageAspect = InputImage.Height / InputImage.Width;

            Size newImageSize;
            Point newImagePos;
            if (canvasAspect < imageAspect)
            {   // 左右に余白が付くケース
                newImageSize = new Size(canvas.RenderSize.Height / imageAspect, canvas.RenderSize.Height);
                newImagePos = UpdateImagePos(newImageSize);
            }
            else
            {   // 上下に余白が付くケース
                newImageSize = new Size(canvas.RenderSize.Width, canvas.RenderSize.Width * imageAspect);
                newImagePos = UpdateImagePos(newImageSize);
            }

            isFit = true;

            ImageRect = new Rect(newImagePos, newImageSize);
        }

        /// <summary>
        /// 画像を等倍表示する
        /// </summary>
        private void ImageDotByDot()
        {
            Size newImageSize = new Size(InputImage.PixelWidth, InputImage.PixelHeight);
            Point newImagePos = UpdateImagePos(newImageSize);
            ImageRect = new Rect(newImagePos, newImageSize);
            isFit = false;
        }

        /// <summary>
        /// 画像位置を計算する
        /// </summary>
        /// <returns></returns>
        private Point UpdateImagePos(Size newSize)
        {
            double x = (canvas.RenderSize.Width - newSize.Width) / 2;
            double y = (canvas.RenderSize.Height - newSize.Height) / 2;
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

        /// <summary>
        /// ウィンドウロード完了イベント
        /// </summary>
        /// <param name="o"></param>
        /// <param name="args"></param>
        private void windowLoaded(object o, RoutedEventArgs args)
        {
            ImageFitSize();
        }

        /// <summary>
        /// ウィンドウサイズ変更イベント
        /// </summary>
        /// <param name="o"></param>
        /// <param name="args"></param>
        private void windowSizeChenged(object o, SizeChangedEventArgs args)
        {
            if (isFit) ImageFitSize();
            else
            {
                Point newImagePos = UpdateImagePos(ImageRect.Size);
                ImageRect = new Rect(newImagePos, ImageRect.Size);
            }
        }
    }
}
