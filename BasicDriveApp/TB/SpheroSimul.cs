#region Assembly RobotKit.dll, v1.0.0.0
// C:\Users\Thomas\Documents\Visual Studio 2013\Projects\spherotest1\BasicDriveApp\RobotKit.dll
#endregion

using RobotKit.Internal;
using RobotKit;
using System.Diagnostics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Input;
using Windows.UI;

// small simulation class for robot kit
// see end of source for original class definition

namespace RobotKit
{
    public class SpheroSim
    {
        // private members
        private Ellipse m_ellipse;
        private TBTools.TBLog m_log;
        //  private Canvas m_canvas;


        // constructor, passing some extra objects we need
        public SpheroSim(Ellipse color_ellipse, TBTools.TBLog log)
        {
            m_ellipse = color_ellipse;
            m_log = log;
        }

        // update log level
        public void SetLogLevel(int level)
        {
            m_log.SetLogLevel(level);
        }

        // methods provided by real kit
        public void Roll(int heading, float speed) {
            m_log.LogMessage(90,"Roll heading="+heading.ToString()+",speed="+speed.ToString());
        }

        public void SetBackLED(float intensity) { }
        
        public void SetHeading(int heading) { }
        
        public void SetRGBLED(int red, int green, int blue) {

            m_log.LogMessage(90,string.Format("SetRGBLED {0}/{1}/{2}",red,green,blue));

            Color c = Color.FromArgb(255, (byte) red, (byte)green, (byte)blue);
            Brush brush = new SolidColorBrush(c);
            m_ellipse.Fill = brush;
        }
        
    }
}







/*
namespace RobotKit
{
    public class Sphero : Robot
    {
        public void Roll(int heading, float speed);
        public void SetBackLED(float intensity);
        public void SetHeading(int heading);
        public void SetRGBLED(int red, int green, int blue);
    }
}
namespace RobotKit
{
    public class Robot
    {
        protected ConnectionState _connectionState;
        public Robot.ConnectionStateChanged OnConnectionStateChanged;

        public string BluetoothName { get; }
        public CollisionControl CollisionControl { get; }
        public ConnectionState ConnectionState { get; }
        public string Name { get; }
        public SensorControl SensorControl { get; }

        [DebuggerStepThrough]
        public void Disconnect();
        public void Sleep(int wakeup, byte macro = 0);
        public void Sleep(int wakeup, int orbBasicLineNum = 0);
        public void Sleep(int wakeup = 0, byte macro = 0, int orbBasic = 0);
        public override string ToString();
        public void WriteName(string name);
        public void WriteToRobot(DeviceMessage msg);

        public delegate void ConnectionStateChanged(Robot sphero, ConnectionState newState);
    }
} 
 */