using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.Threading.Tasks;
using System.Threading;

namespace KinderKinect.Utils
{
    class KinectService
    {

        /// <summary>
        /// This all sets up a kinect, got it from ms sample code
        /// </summary>
        private KinectSensor myKinect;
        /// <summary>
        /// Lets me grab a kinect when I need it
        /// </summary>
        public KinectSensor Kinect
        {
            get
            {
                return myKinect;
            }
        }

        Logger errorLogger;

        private List<IKinectListener> children;
        private bool isRunning;
        bool fill0 = true;

        #region frameAccessors
        private DepthImageFrame depthFrame0;
        private DepthImageFrame depthFrame1;
        /// <summary>
        /// The current Depth Frame
        /// </summary>
        public DepthImageFrame DepthFrame
        {
            get
            {
                if (fill0)
                {
                    return depthFrame1;
                }
                else
                {
                    return depthFrame0;
                }
            }
        }

        private SkeletonFrame skeletonFrame0;
        private SkeletonFrame skeletonFrame1;
        /// <summary>
        /// The current skeleton frame
        /// </summary>
        public SkeletonFrame SkeletonFrame
        {
            get
            {
                if (fill0)
                {
                    return skeletonFrame1;
                }
                else
                {
                    return skeletonFrame0;
                }
            }
        }

        private ColorImageFrame colorFrame0;
        private ColorImageFrame colorFrame1;
        /// <summary>
        /// The current color frame
        /// </summary>
        public ColorImageFrame ColorFrame
        {
            get
            {
                if (fill0)
                {
                    return colorFrame1;
                }
                else
                {
                    return colorFrame0;
                }
            }
        }
        #endregion

        private Skeleton[] skeletons;
        /// <summary>
        /// Get all the active skeletons
        /// </summary>
        public Skeleton[] Skeletons
        {
            get
            {
                return skeletons;
            }
        }
        
        private int activeSkeletonNumber;
        /// <summary>
        /// The identifier of the active skeleton
        /// </summary>
        public int ActiveSkeletonNumber
        {
            get 
            {
                return activeSkeletonNumber;
            }
        }
        private Skeleton activeSkeleton;

        /// <summary>
        /// Are we actively listening to what our Kinect is pumping out at us?
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return isRunning;
            }
        }
        
        public KinectService(Logger ErrorLogger)
        {
            errorLogger = ErrorLogger;
            isRunning = false;
            children = new List<IKinectListener>();
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
                myKinect.SkeletonStream.Enable(new TransformSmoothParameters(){
                    Smoothing = 0.5f,
                    Correction = 0.5f,
                    Prediction = 0.5f,
                    JitterRadius = 0.05f,
                    MaxDeviationRadius = 0.04f
                    });
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
            if (isRunning)
            {
                return;
            }
            if(myKinect!= null)
            myKinect.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(myKinect_AllFramesReady);
            isRunning = true;
        }

        /// <summary>
        /// Stop getting Kinect Values
        /// </summary>
        public void Stop()
        {
            if (!isRunning)
            {
                return;
            }
            isRunning = false;
            myKinect.AllFramesReady -= new EventHandler<AllFramesReadyEventArgs>(myKinect_AllFramesReady);
            
        }


        void myKinect_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            if (isRunning)
            {
                if (fill0)
                {
                    depthFrame0 = e.OpenDepthImageFrame();
                    colorFrame0 = e.OpenColorImageFrame();
                    skeletonFrame0 = e.OpenSkeletonFrame();
                    if (skeletonFrame0 == null)
                            return;

                        skeletons = new Skeleton[skeletonFrame0.SkeletonArrayLength];
                        skeletonFrame0.CopySkeletonDataTo(skeletons);

                        activeSkeletonNumber = 0;

                        for (int i = 0; i < skeletons.Length; i++)
                        {
                            if (skeletons[i].TrackingState == SkeletonTrackingState.Tracked)
                            {
                                activeSkeletonNumber = i + 1;
                                activeSkeleton = skeletons[i];
                                break;
                            }
                        }

                        foreach (IKinectListener l in children)
                        {
                            l.NewKinectDataReady = true;
                        }


                    if (depthFrame1 != null)
                    {
                        depthFrame1.Dispose();
                        depthFrame1 = null;
                    }
                    if (colorFrame1 != null)
                    {
                        colorFrame1.Dispose();
                        colorFrame1 = null;
                    }
                    if (skeletonFrame1 != null)
                    {
                        skeletonFrame1.Dispose();
                        skeletonFrame1 = null;
                    }
                    fill0 = false;
                }
                else
                {
                    depthFrame1 = e.OpenDepthImageFrame();
                    colorFrame1 = e.OpenColorImageFrame();
                    skeletonFrame1 = e.OpenSkeletonFrame();
                    if (skeletonFrame1 == null)
                        return;

                    skeletons = new Skeleton[skeletonFrame1.SkeletonArrayLength];
                    skeletonFrame1.CopySkeletonDataTo(skeletons);

                    activeSkeletonNumber = 0;

                    for (int i = 0; i < skeletons.Length; i++)
                    {
                        if (skeletons[i].TrackingState == SkeletonTrackingState.Tracked)
                        {
                            activeSkeletonNumber = i + 1;
                            activeSkeleton = skeletons[i];
                            break;
                        }
                    }

                    foreach (IKinectListener l in children)
                    {
                        l.NewKinectDataReady = true;
                    }


                    if (depthFrame0 != null)
                    {
                        depthFrame0.Dispose();
                        depthFrame0 = null;
                    }
                    if (colorFrame0 != null)
                    {
                        colorFrame0.Dispose();
                        colorFrame0 = null;
                    }
                    if (skeletonFrame0 != null)
                    {
                        skeletonFrame0.Dispose();
                        skeletonFrame0 = null;
                    }
                    fill0 = true;
                }

                
                

               
            }
        }

        /// <summary>
        /// Registers a listener to the Kinect Service
        /// </summary>
        /// <param name="listener">the listener to register</param>
        public void RegisterKinectListener(IKinectListener listener)
        {
            if (isRunning)
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
            if (isRunning)
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


    }
}
