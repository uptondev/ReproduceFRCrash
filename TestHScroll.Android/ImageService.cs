using System;
using TestHScroll;
using Xamarin.Forms;
using Android.App;
using Android.Graphics;
using Android.Content.Res;
using Xamarin.Forms.Platform.Android;
using System.Threading.Tasks;
using System.IO;
using Plugin.Media.Abstractions;

[assembly: Xamarin.Forms.Dependency(typeof(TestHScroll.Droid.ImageService))]
namespace TestHScroll.Droid
{
    public class ImageService : IImageService
    {
        public async Task<Size> GetImageDimensions(ImageSource ImgSource)
        {
            Bitmap bmp = await GetBitmapAsync(ImgSource);
            return new Size((double)bmp.Width, (double)bmp.Height);
        }

        public async Task<IScanPage> CreatePageFromImageAsync(MediaFile doc)
        {
            IScanPage page = null;
            await Task.Run(() => {
                page = new ScanPage(doc.Path);
            });
            return page;
        }

        private async Task<Bitmap> GetBitmapAsync(ImageSource source)
        {
            var handler = GetHandler(source);
            var returnValue = (Bitmap)null;

            returnValue = await handler.LoadImageAsync(source, Forms.Context);

            return returnValue;
        }

        private static IImageSourceHandler GetHandler(ImageSource source)
        {
            IImageSourceHandler returnValue = null;
            if (source is UriImageSource)
            {
                returnValue = new ImageLoaderSourceHandler();
            }
            else if (source is FileImageSource)
            {
                returnValue = new FileImageSourceHandler();
            }
            else if (source is StreamImageSource)
            {
                returnValue = new StreamImagesourceHandler();
            }
            return returnValue;
        }

        /// <summary>
        /// Scale and maintain bitmap aspect ratio given a desired width into an ImageSource
        /// </summary>
        /// <example>imgSrc = BitmapScaler.ScaleToFitWidth(bitmapFilePath, 100);</example>
        /// <param name="path"></param>
        /// <param name="width"></param>
        /// <returns>ImageSource</returns>
        public static ImageSource ScaleToFitWidth(string path, int width)
        {
            Bitmap b = BitmapFactory.DecodeFile(path);
            Bitmap bMap = ScaleToFitWidth(b, width);

            return ImageSource.FromStream(() =>
            {
                MemoryStream ms = new MemoryStream();
                bMap.Compress(Bitmap.CompressFormat.Png, 100, ms);
                ms.Seek(0L, SeekOrigin.Begin);
                return ms;
            });
        }

        // Scale and maintain aspect ratio given a desired width
        // BitmapScaler.scaleToFitWidth(bitmap, 100);
        public static Bitmap ScaleToFitWidth(Bitmap b, int width)
        {
            float factor = width / (float)b.Width;
            return Bitmap.CreateScaledBitmap(b, width, (int)(b.Height * factor), true);
        }

        // Scale and maintain aspect ratio given a desired height
        // BitmapScaler.scaleToFitHeight(bitmap, 100);
        public static Bitmap ScaleToFitHeight(Bitmap b, int height)
        {
            float factor = height / (float)b.Height;
            return Bitmap.CreateScaledBitmap(b, (int)(b.Width * factor), height, true);
        }

        // scale and keep aspect ratio
        public static Bitmap ScaleToFill(Bitmap b, int width, int height)
        {
            float factorH = height / (float)b.Height;
            float factorW = width / (float)b.Width;
            float factorToUse = (factorH > factorW) ? factorW : factorH;
            return Bitmap.CreateScaledBitmap(b, (int)(b.Width * factorToUse),
                (int)(b.Height * factorToUse), true);
        }

        // scale and don't keep aspect ratio
        public static Bitmap StretchToFill(Bitmap b, int width, int height)
        {
            float factorH = height / (float)b.Height;
            float factorW = width / (float)b.Width;
            return Bitmap.CreateScaledBitmap(b, (int)(b.Width * factorW),
                (int)(b.Height * factorH), true);
        }
    }
}
