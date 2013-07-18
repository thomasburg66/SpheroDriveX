using System;
using Windows.UI;


namespace TBTools
{
    public class TBTools
    {
        static public Color DimmedColor(Color original, byte intensity) {
            // need to apply intensity differently
            double red1, green1, blue1;
            red1 = original.R * intensity / 255.0;
            green1 = original.G * intensity / 255.0;
            blue1 = original.B * intensity / 255.0;
            byte red = (byte)Math.Round(red1);
            byte green = (byte)Math.Round(green1);
            byte blue = (byte)Math.Round(blue1);
            Color retval = Color.FromArgb(255, red, green, blue);
            return retval;
        }

        static public int AccToNum(float a)
        {
            double result = Math.Round(a * 5);
            return (int)result;
        }

        static public int GyroToNum(float g)
        {
            double result = Math.Round(Math.Log10(Math.Abs(g)));
            if (g > 0) result = 0 - result;
            return (int)result;
        }

    }
}
