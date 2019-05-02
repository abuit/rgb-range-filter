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
            DoRun("show_me_white-ish",  Color.FromArgb(255, 255, 255), Color.FromArgb(45, 125, 150), 15);
            DoRun("show_me_greenish",  Color.FromArgb(9, 19, 13), Color.FromArgb(116, 192, 145), 15);
        }

        static void DoRun(string name, Color from, Color to, int lienency)
        {
            Console.WriteLine(name);
            Color[] gradient = CreateGradient(from, to);

            RenderTestImage(gradient, lienency, name);
            
            Image baseImage = Image.FromFile(@"random_image.jpg");
            Bitmap bmpToCheck = new Bitmap(baseImage);

            Bitmap targetBpm1 = new Bitmap(baseImage);
            Bitmap targetBpm2 = new Bitmap(baseImage);
            Bitmap targetBpm3 = new Bitmap(baseImage);

            for(int x = 0; x < bmpToCheck.Width; x++)
            {
                for (int y = 0; y < bmpToCheck.Height; y++)
                {
                    Color pixel = bmpToCheck.GetPixel(x, y);
                    int lieniency = 3;
                    bool hit = CheckPixelInGradient(gradient, pixel, lieniency, out int appliedLieniency);

                    Color fill = CreateBlendedRedOverlay(hit, lieniency, appliedLieniency, pixel);
                    Color fill2 = CreateRedOverlay(hit, lieniency, appliedLieniency, pixel);
                    Color fill3 = CreateAlphaOverlay(hit, lieniency, appliedLieniency, pixel);

                    targetBpm1.SetPixel(x, y, fill);
                    targetBpm2.SetPixel(x, y, fill2);
                    targetBpm3.SetPixel(x, y, fill3);
                }
            }
            
            using (var targetFile = new FileStream(@"map_" + name + "_1.png", FileMode.OpenOrCreate))
            {
                targetBpm1.Save(targetFile, System.Drawing.Imaging.ImageFormat.Png);
            }

            using(var targetFile = new FileStream(@"map_" + name + "_2.png", FileMode.OpenOrCreate))
            {
                targetBpm2.Save(targetFile, System.Drawing.Imaging.ImageFormat.Png);
            }

            using (var targetFile = new FileStream(@"map_" + name + "_3.png", FileMode.OpenOrCreate))
            {
                targetBpm3.Save(targetFile, System.Drawing.Imaging.ImageFormat.Png);
            }
        }
        
        static Color CreateBlendedRedOverlay(bool hit, int lieniency, int appliedLieniency, Color pixel)
        {
            return hit ? Color.FromArgb((255 * (1 + lieniency - appliedLieniency) + (int)pixel.R * (1 + appliedLieniency)) / (lieniency + 2), pixel.G, pixel.B) : pixel;
        }

        static Color CreateRedOverlay(bool hit, int lieniency, int appliedLieniency, Color pixel)
        {
            return hit ? Color.Red : pixel;
        }

        static Color CreateAlphaOverlay(bool hit, int lieniency, int appliedLieniency, Color pixel)
        {
            //Create a red with 100 to 200 opacity based on the applied lieniency
            return hit ? Color.FromArgb(100 + (int)(100 * (1f - appliedLieniency * 1f / lieniency * 1f)), 255, 0, 0) : Color.Transparent;
        }

        static void RenderTestImage(Color[] range, int lienency, string name)
        {
            //Lazy bitmap height calculation
            int testheight = 0;
            for(int extraR = lienency * -1; extraR <= lienency; extraR++)
            {
                for(int extraG = lienency * -1; extraG <= lienency; extraG++)
                {   
                    for(int extraB = lienency * -1; extraB <= lienency; extraB++)
                    {   
                        testheight ++;
                    }   
                }   
            }

            //Render example of the gradient used to test
            Bitmap bp = new Bitmap(range.Length, testheight);

            for(int i = 0; i < range.Length; i++)
            {
                Color c = range[i];
                int ypos = 0; 
                for(int extraR = lienency * -1; extraR <= lienency; extraR++)
                {
                    for(int extraG = lienency * -1; extraG <= lienency; extraG++)
                    {   
                        for(int extraB = lienency * -1; extraB <= lienency; extraB++)
                        {   
                            Color c2 = Color.FromArgb(
                                Math.Max(Math.Min(c.R + extraR, 255), 0), 
                                Math.Max(Math.Min(c.G + extraG, 255), 0), 
                                Math.Max(Math.Min(c.B + extraB, 255), 0));
                            bp.SetPixel(i, ypos, c2);
                            ypos++;
                        }   
                    }   
                }
            }

            var targetFile = new FileStream(@"test_" + name + ".png", FileMode.OpenOrCreate);
            bp.Save(targetFile, System.Drawing.Imaging.ImageFormat.Png);
        }

        static Color[] CreateGradient(Color from, Color to)
        {
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

            return fullRange;
        }

        static bool CheckPixelInGradient(Color[] gradient, Color pixel, int lieniency, out int appliedLieniency)
        {
            appliedLieniency = 0;
            while(appliedLieniency <= lieniency) 
            {
                int localAppliedLieniency = appliedLieniency;
                if (gradient.Any(c => 
                    //Allow for lieniency within the range instead of just c = pixel
                    Math.Abs(c.R - pixel.R) <= localAppliedLieniency &&
                    Math.Abs(c.G - pixel.G) <= localAppliedLieniency &&
                    Math.Abs(c.B - pixel.B) <= localAppliedLieniency))
                {
                    return true;
                }
                appliedLieniency++;
            }
            return false;
        }
    }
}
