using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace SimpleScreenRecorder.Model
{
    public class CustomScreen
    {
        public string Name { get => NativeScreen.DeviceName.Trim(); }
        public int Width { get => NativeScreen.Bounds.Width; }
        public int Height { get => NativeScreen.Bounds.Height; }
        public Screen NativeScreen { get; }

        public CustomScreen(Screen native)
        {
            NativeScreen = native;
        }

        /// <summary>
        /// Getting bitmap.
        ///
        /// Used for recording.
        /// </summary>
        public Bitmap GetBitmap()
        {
            Bitmap bitmap = new Bitmap(NativeScreen.Bounds.Width, NativeScreen.Bounds.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (Graphics graphic = Graphics.FromImage(bitmap))
            {
                graphic.CopyFromScreen(NativeScreen.Bounds.X, NativeScreen.Bounds.Y, 0, 0, NativeScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
                return bitmap;
            }
        }

        /// <summary>
        /// Getting bitmap image.
        ///
        /// This is used for displaying preview image.
        /// </summary>
        public BitmapImage GetBitmapImage()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Bitmap bitmap = GetBitmap();
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                stream.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = stream;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }
}
