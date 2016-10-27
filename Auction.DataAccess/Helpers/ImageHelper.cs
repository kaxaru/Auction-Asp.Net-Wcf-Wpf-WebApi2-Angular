using System;
using System.Drawing;
using System.IO;

namespace Auction.DataAccess.Helpers
{
    public static class ImageHelper
    {
        public enum PhotoSize
        {
            Large = 400,
            Small = 200
        }

        public static string LoadImage(string categoryName)
        {
            string image = string.Empty;
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Images", categoryName);        
            try
            {
                Bitmap imageLogo = (Bitmap)Image.FromFile(path, true);
                if (imageLogo.Height > (int)PhotoSize.Large)
                {
                    imageLogo = Resize(imageLogo, (int)PhotoSize.Large, imageLogo.Height * (int)PhotoSize.Large / imageLogo.Width);
                }

                image  = ImageToBase64(imageLogo, System.Drawing.Imaging.ImageFormat.Bmp);
            }
            catch (System.IO.FileNotFoundException)
            {
                File.Delete(path);
            }

            return image;
        }

        public static byte[] Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
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