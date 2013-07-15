using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Input;
using Windows.UI;

using BasicDriveApp;

namespace BasicDriveApp
{
    class ColorButtons
{

        //! @brief	sphero to control
        private RobotKit.Sphero m_sphero;

        //! @brief  the last time a command was sent in milliseconds
        private long m_lastCommandSentTimeMs;

        /*!
         * @brief	creates a joystick with the given @a puck element for a @a sphero
         * @param	puck the puck to control with the joystick
         * @param	sphero the sphero to control
         */
        public ColorButtons(RobotKit.Sphero sphero)
        {
            m_sphero = sphero;

            m_lastCommandSentTimeMs = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        public async void Anim1Clicked() {
            // MainPage.SetColorAnimStopEnabled(true);
            
            // go through a few simple color changes
            int red, green, blue;
            for (int i = 0; i < 512; i++)
            {
                red = Math.Abs( (i * 1) % 512 - 256) ;
                green = Math.Abs((i * 2) % 512 - 256);
                blue = Math.Abs((i * 3) % 512 - 256);

                m_sphero.SetRGBLED(red,green,blue);

                // delay 10 ms
                await Task.Delay(10);
            }




        }

    }
}
