using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColorMine;
using ColorMine.ColorSpaces;

namespace Converter
{
    public class ImageConverter
    {
        public int GridItemSize;
        private static ImageConverter instance;

        private ImageConverter(int gridItemSize)
        {
            this.GridItemSize = gridItemSize;
        }

        public static ImageConverter getInstance(int gridItemSize)
        {
            if (instance == null)
                instance = new ImageConverter(gridItemSize);

            return instance;
        }
        private static bool IsSimilarColors(Color color1, Color color2)
        {
            //Xyz c1 = RGBtoXYZ(color1);
            //Xyz c2 = RGBtoXYZ(color2);
            //double chromaticDistance = Math.Sqrt((c1.X - c2.X) * (c1.X - c2.X) + (c1.Y - c2.Y) * (c1.Y - c2.Y) + (c1.Z - c2.Z) * (c1.Z - c2.Z));

            double chromaticDistance = (Math.Max(color1.R, color2.R) - Math.Min(color1.R, color2.R)) + (Math.Max(color1.G, color2.G) - Math.Min(color1.G, color2.G)) +
                (Math.Max(color1.B, color2.B) - Math.Min(color1.B, color2.B));

            return (chromaticDistance / 255.0) > 0.3 ? false : true;
        }
        public Bitmap AlignColor(Bitmap image)
        {
            Bitmap resImage = image;
            try
            {
                using (Graphics g = Graphics.FromImage(resImage))
                {
                    Point point = new Point();
                    for (int x = 0; x < resImage.Width; x++)
                    {
                        point.X = x;
                        for (int y = 0; y < resImage.Height; y++)
                        {
                            point.Y = y;
                            if (y + 1 < resImage.Height)
                            {
                                bool isSame = IsSimilarColors(resImage.GetPixel(x, y), resImage.GetPixel(x, y + 1));

                                if (isSame)
                                {
                                    System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(resImage.GetPixel(x, y));
                                    g.FillRectangle(myBrush, x, y + 1, 1, 1);
                                    myBrush.Dispose();
                                }
                                else
                                {
                                   
                                }
                            }

                        }
                    }
                    for (int y = 0; y < resImage.Height; y++)
                    {
                        point.Y = y;
                        for (int x = 0; x < resImage.Width; x++)
                        {
                            point.X = x;


                            if (point.X + 1 < resImage.Width)
                            {
                                bool isSame = IsSimilarColors(resImage.GetPixel(x, y), resImage.GetPixel(x + 1, y));

                                if (isSame)
                                {
                                    System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(resImage.GetPixel(x,y));
                                    g.FillRectangle(myBrush, x + 1, y, 1, 1);
                                    myBrush.Dispose();
                                }
                                else
                                {

                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return resImage;
        }
        private static Xyz RGBtoXYZ(Color color)
        {
            ColorMine.ColorSpaces.Rgb c1 = new ColorMine.ColorSpaces.Rgb()
            {
                R = color.R,
                G = color.G,
                B = color.B
            };

            return c1.To<ColorMine.ColorSpaces.Xyz>();
        }
        public System.Drawing.Color GetGeneralColor(Bitmap image, int _x, int _y)
        {
            try
            {
                Dictionary<System.Drawing.Color, int> colors = new Dictionary<System.Drawing.Color, int>();

                for (int x = _x; x < GridItemSize + _x; x++)
                {
                    for (int y = _y; y < GridItemSize + _y; y++)
                    {
                        if (x < image.Width && y < image.Height)
                        {
                            System.Drawing.Color color = image.GetPixel(x, y);
                            if (colors.ContainsKey(color))
                                colors[color] += 1;
                            else
                                colors.Add(color, 1);
                        }
                    }
                }
                return colors.OrderByDescending(x => x.Value).FirstOrDefault().Key;
            }
            catch (Exception ex)
            {
                throw new Exception("");
            }

        }
        public Bitmap ConvertImage(Bitmap src)
        {
            Bitmap finalImage = CombineBitmap(src);


            using (Graphics g = Graphics.FromImage(finalImage))
            {
                for (int x = GridItemSize; x < src.Width; x += GridItemSize)
                {
                    g.DrawLine(Pens.Gray, new System.Drawing.Point(x, 1), new System.Drawing.Point(x, src.Height - 1));
                }

                for (int y = GridItemSize; y < src.Height; y += GridItemSize)
                {
                    g.DrawLine(Pens.Gray, new System.Drawing.Point(1, y), new System.Drawing.Point(src.Width - 1, y));
                }
            }
            return finalImage;
        }
        public Bitmap CombineBitmap(Bitmap image)
        {

            Bitmap finalImage = new Bitmap(image.Width, image.Height);

            using (Graphics g = Graphics.FromImage(finalImage))
            {
                g.Clear(System.Drawing.Color.White);

                for (int x = 0; x < image.Width; x += GridItemSize)
                {
                    int _y = 0;

                    for (int y = 0; y < image.Height; y += GridItemSize)
                    {
                        System.Drawing.Color color = GetGeneralColor(image, x, y);
                        System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(color);
                        g.FillRectangle(myBrush, new System.Drawing.Rectangle(x, y, GridItemSize, GridItemSize));
                        //g.DrawLine(new System.Drawing.Pen(System.Drawing.Color.SlateGray), new System.Drawing.Point(x, y), new System.Drawing.Point(x, y + GridItemSize));
                        //g.DrawLine(new System.Drawing.Pen(System.Drawing.Color.SlateGray), new System.Drawing.Point(x, _y), new System.Drawing.Point(x + GridItemSize, _y));
                        myBrush.Dispose();
                        _y = y;
                    }

                }
            }

            return finalImage;

        }
    }
}
