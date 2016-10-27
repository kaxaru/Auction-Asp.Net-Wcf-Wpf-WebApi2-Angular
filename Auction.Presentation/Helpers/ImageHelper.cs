using System;
using System.Drawing;
using System.IO;
using System.Web;

namespace Auction.Presentation.Helpers
{
    public static class ImageHelper
    {
       public enum PhotoSize
        {
            Large = 400,
        }

        public static string LoadImage(HttpPostedFileBase file)
        {
            string image = string.Empty;
            if (!string.IsNullOrEmpty(file.FileName))
            {                
                try
                {
                    var bitmapLogo = new Bitmap(file.InputStream);
                    if (bitmapLogo.Height > (int)PhotoSize.Large)
                    {
                        bitmapLogo = Resize(bitmapLogo, (int)PhotoSize.Large, bitmapLogo.Height * (int)PhotoSize.Large / bitmapLogo.Width);
                    }

                    image = ImageToBase64(bitmapLogo, System.Drawing.Imaging.ImageFormat.Bmp);                   
                }
                catch
                {
                    File.Delete(file.FileName);
                }
            }
           
            return image;
        }

        public static byte[] Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
           /* MemoryStream ms = new MemoryStream(imageBytes, 0,
              imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);*/
            return imageBytes;
        }

        private static string ImageToBase64(Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        private static Bitmap Resize(Bitmap sourceImage, int newWidth, int newHeight)
        {
            var newLogo = new Bitmap(newWidth, newHeight);
            var graph = Graphics.FromImage(newLogo);
            graph.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;

            if (sourceImage.Height > sourceImage.Width)
            {
                graph.DrawImage(sourceImage, new Rectangle(0, 0, newWidth, sourceImage.Height * newWidth / sourceImage.Width));
            }
            else
            {
                graph.DrawImage(sourceImage, new Rectangle(0, 0, sourceImage.Width * newHeight / sourceImage.Height, newHeight));
            }

            graph.DrawImageUnscaledAndClipped(newLogo, new Rectangle(0, 0, newWidth, newHeight));

            return newLogo;
        }
    }
}