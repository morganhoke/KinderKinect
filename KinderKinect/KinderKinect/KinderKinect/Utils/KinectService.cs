using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.Threading.Tasks;

namespace KinderKinect.Utils
{
    class KinectService
    {

        /// <summary>
        /// This all sets up a kinect, got it from ms sample code
        /// </summary>
        KinectSensor myKinect;

        Logger errorLogger;

        private bool _isRunning;

        #region frameAccessors
        private DepthImageFrame depthFrame;
        /// <summary>
        /// The current Depth Frame
        /// </summary>
        public DepthImageFrame DepthFrame
        {
            get
            {
                return depthFrame;
            }
        }

        private SkeletonFrame skeletonFrame;
        /// <summary>
        /// The current skeleton frame
        /// </summary>
        public SkeletonFrame SkeletonFrame
        {
            get
            {
                return skeletonFrame;
            }
        }

        private ColorImageFrame colorFranme;
        /// <summary>
        /// The current color frame
        /// </summary>
        public ColorImageFrame ColorFrame
        {
            get
            {
                return colorFranme;
            }
        }
        #endregion

        private List<IKinectListener> children;
        private List<Task> RunningTasks;

        /// <summary>
        /// Are we actively listening to what our Kinect is pumping out at us?
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }
        }
        
        public KinectService(Logger ErrorLogger)
        {
            errorLogger = ErrorLogger;
            _isRunning = false;
        }

        public bool InitKinectService()
        {
            // Check to see if a Kinect is available
            if (KinectSensor.KinectSensors.Count == 0)
            {
                errorLogger.WriteLine("No Kinects detected");
                return false;
            }

            // Get the first Kinect on the computer
            myKinect = KinectSensor.KinectSensors[0];

            // Start the Kinect running and select all the streams
            try
            {
                myKinect.SkeletonStream.Enable();
                myKinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                myKinect.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
                myKinect.Start();
            }
            catch
            {
                errorLogger.WriteLine("Kinect initialise failed");
                return false;
            }

            // connect a handler to the event that fires when new frames are available

            return true;
        }

        /// <summary>
        /// Start getting Kinect Values
        /// </summary>
        public void Start()
        {
            if (_isRunning)
            {
                return;
            }

            myKinect.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(myKinect_AllFramesReady);
            _isRunning = true;
        }

        /// <summary>
        /// Stop getting Kinect Values
        /// </summary>
        public void Stop()
        {
            if (!_isRunning)
            {
                return;
            }
            _isRunning = false;
            while (RunningTasks.Count != 0)
            {
                //spin untill all threads clean themselves up
            }
            myKinect.AllFramesReady -= new EventHandler<AllFramesReadyEventArgs>(myKinect_AllFramesReady);
            
        }

        void myKinect_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            if (_isRunning)
            {
                skeletonFrame = e.OpenSkeletonFrame();
                depthFrame = e.OpenDepthImageFrame();
                colorFranme = e.OpenColorImageFrame();

                foreach (IKinectListener child in children)
                {
                    RunningTasks.Add(Task.Factory.StartNew(child.NewKinectDataReady).ContinueWith(TaskDone));
                }

            }
        }

        /// <summary>
        /// Registers a listener to the Kinect Service
        /// </summary>
        /// <param name="listener">the listener to register</param>
        public void RegisterKinectListener(IKinectListener listener)
        {
            if (_isRunning)
            {
                Stop();
                children.Add(listener);
                Start();
            }
            else
            {
                children.Add(listener);
            }
        }

        public void UnregisterKinectListener(IKinectListener listener)
        {
            if (_isRunning)
            {
                Stop();
                children.Remove(listener);
                Start();
            }
            else
            {
                children.Remove(listener);
            }
        }

        private void TaskDone(Task t)
        {
            RunningTasks.Remove(t);
        }


    }
}
