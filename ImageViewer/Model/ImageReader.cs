using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.IO;

namespace ImageViewer.Model
{
    /// <summary>
    /// 静的クラスなのでnewできない
    /// </summary>
    public static class ImageReader
    {
        /// <summary>
        /// 読込み可能な拡張子(大文字)
        /// </summary>
        private static string[] IMAGE_EXT = {".JPG", ".JPEG", ".BMP", ".PNG", ".GIF", ".TIF", ".TIFF"};

        /// <summary>
        /// 指定したパスの画像ファイルのBitmapImageを返します
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static BitmapImage GetBitmapImage(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException();
            if (!IsImage(path)) throw new ArgumentException();

            // 何故かTHETAで撮影した画像を開けない
            // 苦肉の策でJPEGの場合はメーカーノートを以外を読み込む
            BitmapImage bitmapImage;
            string ext = Path.GetExtension(path).ToUpper();
            if (ext == ".JPG" || ext == ".JPEG")
            {
                bitmapImage = readJpegImage(path);
            }
            else
            {
                bitmapImage = new BitmapImage(new Uri(path));
            }

            return bitmapImage;
        }

        /// <summary>
        /// 指定したパスのファイルが画像ファイルか拡張子で判定します
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsImage(string path)
        {
            if (string.IsNullOrEmpty(path)) return false;
            if (!File.Exists(path)) return false;

            string ext = Path.GetExtension(path).ToUpper();
            if (!IMAGE_EXT.Contains(ext)) return false;

            return true;
        }

        /// <summary>
        /// JPEG画像を読み込みます
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static BitmapImage readJpegImage(string path)
        {
            byte[] marker = new byte[2];

            using (MemoryStream ms = new MemoryStream())
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    // マーカー読込み
                    fs.Read(marker, 0, marker.Length);
                    switch (marker[1])
                    {
                        case 0xD8:
                            // FFD8
                            ms.Write(marker, 0, marker.Length);
                            break;
                    }
                }
            }

            return null;
        }
    }
}
