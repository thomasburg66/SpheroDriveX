#region Assembly RobotKit.dll, v1.0.0.0
// C:\Users\Thomas\Documents\Visual Studio 2013\Projects\spherotest1\BasicDriveApp\RobotKit.dll
#endregion

using RobotKit.Internal;
using RobotKit;
using System;
using System.Diagnostics;


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
namespace RobotKit
{
    public class SpheroSim
    {
        public void Roll(int heading, float speed) { }
        public void SetBackLED(float intensity) { }
        public void SetHeading(int heading) { }
        public void SetRGBLED(int red, int green, int blue) { }
        public SpheroSim() { 
        }
    }
}
