using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using TeamViewerClient.Services.NetworkService;

namespace TeamViewerClient.Helpers
{
    public class ImageHelper
    {
        public string FolderPath { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"CLIENT{NetworkHelper.client.Client.LocalEndPoint.ToString().Replace(":",".")}.Images");

        //Create a folder to store images
        public void CreateFolder()
        {
            int number = 0;
            while (Directory.Exists(FolderPath))
            {
                // Add number to folder name if folder already exists
                if (number != 0)
                {
                    FolderPath.Replace((number - 1).ToString(), "");
                    FolderPath += number;
                }
                number++;
            }

            Directory.CreateDirectory(FolderPath);
        }

        // Convert a byte array to an image and save it to the folder
        //public string GetImagePath(byte[] buffer)
        //{
        //    // Convert byte array to image
        //    ImageConverter ic = new ImageConverter();
        //    var data = ic.ConvertFrom(buffer);
        //    Image img = data as Image;

        //    // If the conversion was successful
        //    if (img != null)
        //    {
        //        Bitmap bitmap = new Bitmap(img);
        //        var strGuid = Guid.NewGuid().ToString();
        //        var imagePath = $@"{FolderPath}\image{strGuid}.png";
        //        bitmap.Save(imagePath);
        //        return imagePath;
        //    }
        //    else
        //    {
        //        return string.Empty;
        //    }
        //}

        public byte[] GetBytesOfImage(string path)
        {
            var image = new Bitmap(path);
            ImageConverter imageconverter = new ImageConverter();
            var imagebytes = (byte[])imageconverter.ConvertTo(image, typeof(byte[]));
            return imagebytes;
        }

        // Take a screenshot and save it to the folder
        public string TakeScreenShot()
        {
            // Create a bitmap with the size of the screen
            Bitmap bmp = new Bitmap(1920, 1080);
            var id = Guid.NewGuid().ToString();
            var source = Path.Combine(FolderPath, "screenshot" + id + ".png");

            // Copy the screen to the bitmap
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(0, 0, 0, 0, new Size(1920, 1080));
                bmp.Save(source);  // saves the image
            }
            return source;
        }
    }
}
