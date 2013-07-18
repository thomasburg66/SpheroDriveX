using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Reflection;

using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;



// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

using RobotKit;

namespace BasicDriveApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //! @brief  the default string to show no sphero connected
        private const string kNoSpheroConnected = "No Sphero Connected";

        //! @brief  the default string to show when connecting to a sphero ({0})
        private const string kConnectingToSphero = "Connecting to {0}";

        //! @brief  the default string to show when connected to a sphero ({0})
        private const string kSpheroConnected = "Connected to {0}";


        //! @brief  the robot we're connecting to
        Sphero m_robot = null;

        //! @brief  the joystick to control m_robot
        private Joystick m_joystick;

        //! @brief  the color wheel to control m_robot color
        private ColorWheel m_colorwheel;

        // TB
        private ColorButtons m_colorbuttons;
        SpheroSim m_simul;

        //! @brief  the calibration wheel to calibrate m_robot
        private CalibrateElement m_calibrateElement;

        public MainPage() {
            this.InitializeComponent();
        }

        private void WriteDebugOutput(String text) {
            txtOutput.Text=txtOutput.Text + "\n" + text;
            Debug.WriteLine(text);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            StartUpRobot();

            // added by TB
            String version = "Lupo 1.2.0.1";
            txtAppName.Text = "SpheroDriveX " + version;

        }

        private void StartUpRobot()
        {
            // use simulator?
            m_simul = new SpheroSim(ellColor);

            if (chkSimul.IsChecked == true)
            {
                m_robot = null;
                SetupControls();
                return;
            }

            SetupRobotConnection();
            Application app = Application.Current;
            app.Suspending += OnSuspending;
        }

        /*!
         * @brief   handle the user launching this page in the application
         * 
         *  connects to sphero and sets up the ui
         */
        protected override void OnNavigatedFrom(NavigationEventArgs e) {
            base.OnNavigatedFrom(e);

            ShutdownRobotConnection();
            ShutdownControls();

            Application app = Application.Current;
            app.Suspending -= OnSuspending;
        }

        //! @brief  handle the application entering the background
        private void OnSuspending(object sender, SuspendingEventArgs args) {
            ShutdownRobotConnection();
        }

        //! @brief  search for a robot to connect to
        private void SetupRobotConnection() {
            SpheroName.Text = kNoSpheroConnected;

            RobotProvider provider = RobotProvider.GetSharedProvider();
            provider.DiscoveredRobotEvent += OnRobotDiscovered;
            provider.NoRobotsEvent += OnNoRobotsEvent;
            provider.ConnectedRobotEvent += OnRobotConnected;
            provider.FindRobots();
        }

        //! @brief  disconnect from the robot and stop listening
        private void ShutdownRobotConnection() {
            if (m_robot != null) {
                m_robot.SensorControl.StopAll();
                m_robot.Sleep();
                // temporary while I work on Disconnect.
                //m_robot.Disconnect();
                ConnectionToggle.OffContent = "Disconnected";
                SpheroName.Text = kNoSpheroConnected;

                m_robot.SensorControl.AccelerometerUpdatedEvent -= OnAccelerometerUpdated;
                m_robot.SensorControl.GyrometerUpdatedEvent -= OnGyrometerUpdated;

                m_robot.CollisionControl.StopDetection();
                m_robot.CollisionControl.CollisionDetectedEvent -= OnCollisionDetected;

                RobotProvider provider = RobotProvider.GetSharedProvider();
                provider.DiscoveredRobotEvent -= OnRobotDiscovered;
                provider.NoRobotsEvent -= OnNoRobotsEvent;
                provider.ConnectedRobotEvent -= OnRobotConnected;
            }
            m_robot = null;
        }

        //! @brief  configures the various sphero controls
        private void SetupControls() {
            WriteDebugOutput(
                string.Format("Creating new Controls, m_robot={0}, m_simul={1}",m_robot,m_simul));

            if (m_colorwheel == null)
                m_colorwheel = new ColorWheel(ColorPuck, m_robot, m_simul, slColorIntensity);
            else
                m_colorwheel.UpdateSphero(m_robot, m_simul);

            if (m_joystick == null)
                m_joystick = new Joystick(Puck, m_robot);
            else
                m_joystick.update(m_robot);

            if (m_calibrateElement == null)
                m_calibrateElement = new CalibrateElement(
                    CalibrateRotationRoot,
                    CalibrateTarget,
                    CalibrateRingOuter,
                    CalibrateRingMiddle,
                    CalibrateRingInner,
                    CalibrationFingerPoint,
                    rectCalibration,
                    m_robot);
            else
                m_calibrateElement.update(m_robot);

            m_colorbuttons = new ColorButtons(m_robot,m_simul, bnStartColor1,bnStopColorAnim);

            String step_string= slStep.Value.ToString();
            String delay_string=slDelay.Value.ToString();
            String intensity_string=slColorIntensity.Value.ToString();
            m_colorwheel.SetIntensity(Int32.Parse(intensity_string));
            m_colorbuttons.SetIntensity(Int32.Parse(intensity_string));
            m_colorbuttons.SetDelay(Int32.Parse(step_string));

        }

        //! @brief  shuts down the various sphero controls
        private void ShutdownControls() {
            // I'm pretty sure this does nothing, we should just write modifiers - PJM
            m_joystick = null;
            m_colorwheel = null;
            m_calibrateElement = null;
        }

        //! @brief  when a robot is discovered, connect!
        private void OnRobotDiscovered(object sender, Robot robot) {
            WriteDebugOutput(string.Format("Discovered \"{0}\"", robot.BluetoothName));

            if (m_robot == null) {
                WriteDebugOutput("Creating new Robot");
                RobotProvider provider = RobotProvider.GetSharedProvider();
                provider.ConnectRobot(robot);
                ConnectionToggle.OnContent = "Connecting...";
                m_robot = (Sphero)robot;
                SpheroName.Text = string.Format(kConnectingToSphero, robot.BluetoothName);
            }
        }


        private void OnNoRobotsEvent(object sender, EventArgs e) {
            MessageDialog dialog = new MessageDialog("No Sphero Paired");
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;
            dialog.ShowAsync();
        }


        //! @brief  when a robot is connected, get ready to drive!
        private void OnRobotConnected(object sender, Robot robot) {
            WriteDebugOutput(string.Format("Connected to {0}", robot));
            ConnectionToggle.IsOn = true;
            ConnectionToggle.OnContent = "Connected";

            m_robot.SetRGBLED(255, 255, 255);
            SpheroName.Text = string.Format(kSpheroConnected, robot.BluetoothName);
            SetupControls();

            m_robot.SensorControl.Hz = 10;
            m_robot.SensorControl.AccelerometerUpdatedEvent += OnAccelerometerUpdated;
            m_robot.SensorControl.GyrometerUpdatedEvent += OnGyrometerUpdated;

            m_robot.CollisionControl.StartDetectionForWallCollisions();
            m_robot.CollisionControl.CollisionDetectedEvent += OnCollisionDetected;
        }


        private void ConnectionToggle_Toggled(object sender, RoutedEventArgs e) {
            WriteDebugOutput("Connection Toggled : " + ConnectionToggle.IsOn);
            ConnectionToggle.OnContent = "Connecting...";
            if (ConnectionToggle.IsOn) {
                if (m_robot == null) {
                    SetupRobotConnection();
                }
            } else {
                ShutdownRobotConnection();
            }
        }

        private void OnAccelerometerUpdated(object sender, AccelerometerReading reading) {
            AccelerometerX.Text = "" + reading.X;
            AccelerometerY.Text = "" + reading.Y;
            AccelerometerZ.Text = "" + reading.Z;

            slAccX.Value = TBTools.TBTools.AccToNum(reading.X);
            slAccY.Value = TBTools.TBTools.AccToNum(reading.Y);
            slAccZ.Value = TBTools.TBTools.AccToNum(reading.Z);


        }

        private void OnGyrometerUpdated(object sender, GyrometerReading reading) {
            GyroscopeX.Text = "" + reading.X;
            GyroscopeY.Text = "" + reading.Y;
            GyroscopeZ.Text = "" + reading.Z;

            slGyroX.Value = TBTools.TBTools.GyroToNum(reading.X);
            slGyroY.Value = TBTools.TBTools.GyroToNum(reading.Y);
            slGyroZ.Value = TBTools.TBTools.GyroToNum(reading.Z);
        }

        private void OnCollisionDetected(object sender, CollisionData data) {
            WriteDebugOutput("Wall collision was detected");
        }

        private void bnStartColor1_Click(object sender, RoutedEventArgs e)
        {
            m_colorbuttons.StartClicked();
        }


        public void SetColorAnimStopEnabled(bool is_enabled)
        {
            bnStopColorAnim.IsEnabled = is_enabled;
        }

        public void SetColorAnim1StartEnabled(bool is_enabled)
        {
            bnStartColor1.IsEnabled = is_enabled;
        }

        private void bnStopColorAnim_Click(object sender, RoutedEventArgs e)
        {
            m_colorbuttons.StopClicked();

        }

        private void slColorIntensity_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            String slider_string = e.NewValue.ToString();
            int slider_int = Int32.Parse(slider_string);
            if (m_colorwheel != null)
            {
                m_colorwheel.SetIntensity(slider_int);
                m_colorbuttons.SetIntensity(slider_int);

            }
        }

        private void slDelay_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            String delay_string = e.NewValue.ToString();
            int delay_int = Int32.Parse(delay_string);
            if (m_colorbuttons!=null)
                m_colorbuttons.SetDelay(delay_int);

        }

        private void slStep_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            String step_string = e.NewValue.ToString();
            int step_int = Int32.Parse(step_string);
            if (m_colorbuttons != null)
                m_colorbuttons.SetStep(step_int);

        }

        private void chkSimul_Unchecked(object sender, RoutedEventArgs e)
        {
            StartUpRobot();
        }

        private void chkSimul_Checked(object sender, RoutedEventArgs e)
        {
            if (m_robot != null)
            {
                m_robot = null;
                ShutdownRobotConnection();
                SetupControls();
            }
        }

    }



}
