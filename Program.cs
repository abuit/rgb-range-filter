using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Colorfilter
{
    class Program
    {
        static void Main(string[] args)
        {
            DoRun("show_me_blueish",  (32, 89, 104, 218, 237, 248));
            DoRun("show_me_greenish",  (9, 19, 13, 116, 192, 145));
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
                        bmp.SetPixel(x, y, Color.Red);
                    }
                    else 
                    {
                        //bmp.SetPixel(x, y, Color.Transparent);
                    }

                }
            }
            
            var targetFile = new FileStream(@"random_image_"+ name + ".png", FileMode.OpenOrCreate);
            bmp.Save(targetFile, System.Drawing.Imaging.ImageFormat.Png);
        }

        static bool CheckPixel((int fromR, int fromG, int fromB, int toR, int toG, int toB) range, Color pixel)
        {
            int lieniency = 10;
            bool hardBounds = false;

            Color from = Color.FromArgb(range.fromR, range.fromG, range.fromB);
            Color to = Color.FromArgb(range.toR, range.toG, range.toB);

            int steps = Math.Abs(from.R - to.R);
            steps = Math.Max(steps, Math.Abs(from.G - to.G));
            steps = Math.Max(steps, Math.Abs(from.B - to.B));

            Color[] fullRange = new Color[steps + 1];
            fullRange[0] = from;
            fullRange[steps] = to;
            for(int i = 1; i < steps; i++) 
            {
                fullRange[i] = Color.FromArgb(
                    from.R + (int)((to.R - from.R) * ((float)i / steps)),
                    from.G + (int)((to.G - from.G) * ((float)i / steps)),
                    from.B + (int)((to.B - from.B) * ((float)i / steps)));
            }

            return fullRange.Any(c => 
                //Allow for lieniency within the range instead of just c = pixel
                Math.Abs(c.R - pixel.R) <= lieniency &&
                Math.Abs(c.G - pixel.G) <= lieniency &&
                Math.Abs(c.B - pixel.B) <= lieniency &&
                //Hard bounds or lienency at bounds?
                (!hardBounds || (
                    Math.Min(from.R, to.R) <= pixel.R &&
                    Math.Min(from.G, to.G) <= pixel.G &&
                    Math.Min(from.B, to.B) <= pixel.B &&
                    Math.Max(from.R, to.R) >= pixel.R &&
                    Math.Max(from.G, to.G) >= pixel.G &&
                    Math.Max(from.B, to.B) >= pixel.B)
                ));
        }
    }
}
