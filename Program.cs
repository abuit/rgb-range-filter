using System;
using System.Drawing;
using System.IO;

namespace Colorfilter
{
    class Program
    {
        static void Main(string[] args)
        {
            DoRun("show_me_blueish",  (45, 55, 0, 75, 86, 255));
            DoRun("show_me_greenish",  (70, 0, 70, 160, 255, 110));
        }

        static void DoRun(string name, (int fromR, int fromG, int fromB, int toR, int toG, int toB) range)
        {
            Image i = Image.FromFile(@"random_image.jpg");
            Bitmap bmp = new Bitmap(i);
            for(int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    Color pixel = bmp.GetPixel(x, y);
                    if (CheckPixel(range, pixel))
                    {
                        //bmp.SetPixel(x, y, Color.Red);
                    }
                    else 
                    {
                        bmp.SetPixel(x, y, Color.Transparent);
                    }

                }
            }
            
            var targetFile = new FileStream(@"random_image_"+ name + ".png", FileMode.OpenOrCreate);
            bmp.Save(targetFile, System.Drawing.Imaging.ImageFormat.Png);
        }

        static bool CheckPixel((int fromR, int fromG, int fromB, int toR, int toG, int toB) range, Color pixel)
        {
            return (pixel.R >= range.fromR && pixel.R <= range.toR &&
                pixel.G >= range.fromG && pixel.G <= range.toG &&
                pixel.B >= range.fromB && pixel.B <= range.toB);
        }
    }
}
