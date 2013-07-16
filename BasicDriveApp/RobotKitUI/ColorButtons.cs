using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using Windows.UI;

using BasicDriveApp;

namespace BasicDriveApp
{
    class ColorButtons
    {

        byte m_intensity;
        int m_delay;
        int m_step;

        //! @brief start/stop buttons
        private Button m_bnStart, m_bnStop;

        //! @brief	sphero to control
        private RobotKit.Sphero m_sphero;

        //! @brief	simulator to control
        private RobotKit.SpheroSim m_simul;

        //! @brief  the last time a command was sent in milliseconds
        private long m_lastCommandSentTimeMs;

        //! @brief is an animation ongoing?
        private bool m_animation_active;

        /*!
         * @brief	creates a joystick with the given @a puck element for a @a sphero
         * @param	puck the puck to control with the joystick
         * @param	sphero the sphero to control
         */
        public ColorButtons(RobotKit.Sphero sphero, RobotKit.SpheroSim simul, Button start, Button stop)
        {
            m_sphero = sphero;
            m_simul = simul;


            m_bnStart = start;
            m_bnStop = stop;

            m_intensity = 255;
            m_delay = 24;
            m_step = 12;

            m_animation_active = false;

            m_lastCommandSentTimeMs = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }


        public void SetIntensity(int a)
        {
            m_intensity = (byte)a;
        }

        public void SetDelay(int d)
        {
            m_delay = d;
        }

        public void SetStep(int s)
        {
            m_step = s;
        }

        public async void StartClicked()
        {
            m_bnStop.IsEnabled = true;
            m_bnStart.IsEnabled = false;

            m_animation_active = true;

            // go through a few simple color changes
            int red, green, blue;

            // c=0: green, etc.
            int brightness;
            while (m_animation_active==true)
            {
                for (int c = 0; c < 3; c++)
                {
                    for (int i = 0; i < 512; i+=m_step)
                    {
                        brightness=256-Math.Abs(i-256);
                        switch (c)
                        {
                            case 0:
                                red = brightness; green = 0; blue = 0;
                                break;
                            case 1:
                                red = 0; green = brightness; blue = 0;
                                break;
                            default:
                                red = 0; green = 0; blue = brightness;
                                break;
                        }
                        
                        if (m_simul!=null)
                            m_simul.SetRGBLED(m_intensity, red, green, blue);
                        if (m_sphero != null)
                        {
                            // need to apply intensity differently
                            double red1, green1, blue1;
                            red1=red * m_intensity / 255.0;
                            green1=green * m_intensity / 255.0;
                            blue1=blue * m_intensity / 255.0;
                            red= (int) Math.Round(red1);
                            green = (int) Math.Round(green1);
                            blue = (int) Math.Round(blue1);

                            m_sphero.SetRGBLED(red, green, blue);
                        }

                        // delay a bit; at minimum 10 ms
                        await Task.Delay(m_delay+10);

                        // break if done (through stop button)
                        if (m_animation_active != true)
                            i=10000;
                    } // for i
                    // break if done (through stop button)
                    if (m_animation_active != true)
                        c = 10000;
                } // for c
            } // while


            StopClicked();

        }

        public void StopClicked()
        {
            m_bnStop.IsEnabled = false;
            m_bnStart.IsEnabled = true;

            m_animation_active = false;

        }
    }


}
