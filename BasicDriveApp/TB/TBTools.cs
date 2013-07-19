using System;
using System.Diagnostics;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

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
    
    public class TBLog {

        private int m_level;
        private TextBox m_log_box;

        public TBLog(TextBox log_box)
        {
            m_level = 50;
            m_log_box = log_box;
        }

        public void SetLogLevel(int level)
        {
            m_level = level;
        }

        public void LogMessage(int level, string text)
        {
            if (level > m_level) return;

            if (m_log_box != null)
            {
                m_log_box.Text = m_log_box.Text + "\n" + text;
                m_log_box.Focus(FocusState.Programmatic);
                int len = m_log_box.Text.Length;
                m_log_box.SelectionStart = len + 1;
            }
            Debug.WriteLine(text);
        }

    } // TBLog


}
