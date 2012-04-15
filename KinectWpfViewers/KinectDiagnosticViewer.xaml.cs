using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Research.Kinect.Nui;
using KinectNui = Microsoft.Research.Kinect.Nui;
using pixcelation.gestures;
using DTWGestureRecognition;
using System.Collections;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using PhotoStuntsPix;
using System.IO;
using System.Threading;
using Attassa;
namespace Microsoft.Samples.Kinect.WpfViewers
{
    /// <summary>
    /// Interaction logic for KinectDiagnosticViewer.xaml
    /// </summary>
    public partial class KinectDiagnosticViewer : UserControl
    {

        #region probably Need to Be Moved


        SwipeGestureDetector swipeGestureRecognizer;

        void OnGestureDetected(string gesture)
        {
           //   int pos = detectedGestures.Items.Add(string.Format("{0} : {1}", gesture, DateTime.Now));

            txtDebug.AppendText("Detected Motion " + gesture + " \n");
          //  detectedGestures.SelectedIndex = pos;
        }
        #endregion

        #region DTW Param

        // Constants for directions

        const String LEFT = "@Left";
        const String RIGHT = "@Right";
        const String TOP = "Up";
        const String DOWN = "Down";
        const String ZOOM_IN = "zoomin";
        const String ZOOM_OUT = "zoomout";

        private DateTime lastUpdate = DateTime.Now;
        readonly TimeSpan threshHold = new TimeSpan(10000 * 1100);
        private ArrayList pictures;
        int currentImg = 0;
        // We want to control how depth data gets converted into false-color data
        // for more intuitive visualization, so we keep 32-bit color frame buffer versions of
        // these, to be updated whenever we receive and process a 16-bit frame.

        /// <summary>
        /// The red index
        /// </summary>
        private const int RedIdx = 2;

        /// <summary>
        /// The green index
        /// </summary>
        private const int GreenIdx = 1;

        /// <summary>
        /// The blue index
        /// </summary>
        private const int BlueIdx = 0;

        /// <summary>
        /// How many skeleton frames to ignore (_flipFlop)
        /// 1 = capture every frame, 2 = capture every second frame etc.
        /// </summary>
        private const int Ignore = 2;

        /// <summary>
        /// How many skeleton frames to store in the _video buffer
        /// </summary>
        private const int BufferSize = 32;

        /// <summary>
        /// The minumum number of frames in the _video buffer before we attempt to start matching gestures
        /// </summary>
        private const int MinimumFrames = 6;

        /// <summary>
        /// The minumum number of frames in the _video buffer before we attempt to start matching gestures
        /// </summary>
        private const int CaptureCountdownSeconds = 3;

        /// <summary>
        /// Where we will save our gestures to. The app will append a data/time and .txt to this string
        /// </summary>
        //        private const string GestureSaveFileLocation = @"H:\My Dropbox\Dropbox\Microsoft Kinect SDK Beta\DTWGestureRecognition\DTWGestureRecognition\";

        private const string GestureSaveFileLocation = @"C:\Users\Reza\Documents\pixel2\pixelation\SkeletalViewer\records\";//@".\DTWGestureRecognition\";

        private const string GestureFileRecord = @"C:\Users\Reza\Documents\pixel2\pixelation\SkeletalViewer\records\RecordedGestures2011-11-20_14-27.txt";//RecordedGestures2011-11-20_02-23.txt"; //RecordedGestures2011-11-20_02-23.txt";
        /// <summary>
        /// Where we will save our gestures to. The app will append a data/time and .txt to this string
        /// </summary>
        private const string GestureSaveFileNamePrefix = @"RecordedGestures";


        /// <summary>
        /// The depth frame byte array. Only supports 320 * 240 at this time
        /// </summary>
        private readonly byte[] _depthFrame32 = new byte[320 * 240 * 4];

        /// <summary>
        /// Flag to show whether or not the gesture recogniser is capturing a new pose
        /// </summary>
        private bool _capturing;

        /// <summary>
        /// Dynamic Time Warping object
        /// </summary>
        private DtwGestureRecognizer _dtw;

        /// <summary>
        /// How many frames occurred 'last time'. Used for calculating frames per second
        /// </summary>
        private int _lastFrames;

        /// <summary>
        /// The 'last time' DateTime. Used for calculating frames per second
        /// </summary>
        private DateTime _lastTime = DateTime.MaxValue;


        /// <summary>
        /// Total number of framed that have occurred. Used for calculating frames per second
        /// </summary>
        private int _totalFrames;

        /// <summary>
        /// Switch used to ignore certain skeleton frames
        /// </summary>
        private int _flipFlop;

        /// <summary>
        /// ArrayList of coordinates which are recorded in sequence to define one gesture
        /// </summary>
        private ArrayList _video;

        /// <summary>
        /// ArrayList of coordinates which are recorded in sequence to define one gesture
        /// </summary>
        private DateTime _captureCountdown = DateTime.Now;

        /// <summary>
        /// ArrayList of coordinates which are recorded in sequence to define one gesture
        /// </summary>
        private System.Windows.Forms.Timer _captureCountdownTimer;

        #endregion

        #region Dtw

        /// <summary>
        /// Runs every time our 2D coordinates are ready.
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="a">Skeleton 2Ddata Coord Event Args</param>
        private void NuiSkeleton2DdataCoordReady(object sender, Skeleton2DdataCoordEventArgs a)
        {
            

            // We need a sensible number of frames before we start attempting to match gestures against remembered sequences
            if (_video.Count > MinimumFrames && _capturing == false)
            {
                ////Debug.WriteLine("Reading and video.Count=" + video.Count);
                string s = _dtw.Recognize(_video);

               
                if (s.StartsWith(LEFT))
                {
                    leftGesture();
                    txtDebug.Text = ("Recognised as: " + s + "\n");

                    txtDebug.AppendText("\n" + this.currentImg.ToString());
                }
                else if (s.StartsWith(RIGHT))
                {
                    rightGesture();
                    txtDebug.Text = ("Recognised as: " + s + "\n");

                    txtDebug.AppendText("\n" + this.currentImg.ToString());
                }
                else if (s.StartsWith(TOP))
                {
                    topGesture();
                }
                else if (s.StartsWith(DOWN))
                {
                    downGesture();
                }else if(s.StartsWith(ZOOM_IN))
                {
                    zoomInGesture();
                }else if(s.StartsWith(ZOOM_OUT))
                {
                    zoomOutGesture();
                }
                else if(!s.Contains("__UNKNOWN"))
                {
                    // There was no match so reset the buffer
                    _video = new ArrayList();
                }
            }

            // Ensures that we remember only the last x frames
            if (_video.Count > BufferSize)
            {
                // If we are currently capturing and we reach the maximum buffer size then automatically store
                //if (_capturing)
                //{
                //    DtwStoreClick(null, null);
                //}
                //else
                
                    // Remove the first frame in the buffer
                    _video.RemoveAt(0);
                
            }

            // Decide which skeleton frames to capture. Only do so if the frames actually returned a number. 
            // For some reason my Kinect/PC setup didn't always return a double in range (i.e. infinity) even when standing completely within the frame.
            // TODO Weird. Need to investigate this
            if (!double.IsNaN(a.GetPoint(0).X))
            {
                // Optionally register only 1 frame out of every n
                _flipFlop = (_flipFlop + 1) % Ignore;
                if (_flipFlop == 0)
                {
                    _video.Add(a.GetCoords());
                }
            }

            // Update the debug window with Sequences information
            //dtwTextOutput.Text = _dtw.RetrieveText();
        }

        private void zoomOutGesture()
        {
            throw new NotImplementedException();
        }

        private void zoomInGesture()
        {
            throw new NotImplementedException();
        }

        private void downGesture()
        {
            throw new NotImplementedException();
        }

        private void topGesture()
        {
            throw new NotImplementedException();
        }

        private void rightGesture()
        {
            if (pictures == null) return;
            if(!checkLastModified()) return;
            MemoryStream ms = new MemoryStream();
            this.currentImg = (Math.Abs(this.currentImg + 1)) % pictures.Count;
            System.Drawing.Image bm = (System.Drawing.Image)pictures[this.currentImg];
            bm.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            System.Windows.Media.Imaging.BitmapImage bImg = new System.Windows.Media.Imaging.BitmapImage();
            bImg.BeginInit();
            bImg.StreamSource = new MemoryStream(ms.ToArray());
            bImg.EndInit();
            image1.Source = bImg;
            this.lastUpdate = DateTime.Now;
            txtDebug.AppendText("\n" + this.currentImg.ToString());
        }

        private void leftGesture()
        {
            if (pictures == null) return;
            if (!checkLastModified()) return;
            MemoryStream ms = new MemoryStream();
            //this.currentImg = (this.currentImg + 1) % pictures.Count;
            this.currentImg--;
            if (currentImg < 0)
                currentImg = pictures.Count - 1;
            System.Drawing.Image bm = (System.Drawing.Image)pictures[this.currentImg];
            bm.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            System.Windows.Media.Imaging.BitmapImage bImg = new System.Windows.Media.Imaging.BitmapImage();
            bImg.BeginInit();
            bImg.StreamSource = new MemoryStream(ms.ToArray());
            bImg.EndInit();
            image1.Source = bImg;
            this.lastUpdate = DateTime.Now;
            txtDebug.AppendText("\n" + this.currentImg.ToString());
       
        }

        private bool checkLastModified()
        {

            TimeSpan dif = DateTime.Now.Subtract(this.lastUpdate);
            if (dif < this.threshHold)
                return false;
            else
                return true;
            
        }

        /// <summary>
        /// Opens the sent text file and creates a _dtw recorded gesture sequence
        /// Currently not very flexible and totally intolerant of errors.
        /// </summary>
        /// <param name="fileLocation">Full path to the gesture file</param>
        public void LoadGesturesFromFile(string fileLocation)
        {
            int itemCount = 0;
            string line;
            string gestureName = String.Empty;

            // TODO I'm defaulting this to 12 here for now as it meets my current need but I need to cater for variable lengths in the future
            ArrayList frames = new ArrayList();
            double[] items = new double[12];

            // Read the file and display it line by line.
            System.IO.StreamReader file = new System.IO.StreamReader(fileLocation);
            while ((line = file.ReadLine()) != null)
            {
                if (line.StartsWith("@"))
                {
                    gestureName = line;
                    continue;
                }

                if (line.StartsWith("~"))
                {
                    frames.Add(items);
                    itemCount = 0;
                    items = new double[12];
                    continue;
                }

                if (!line.StartsWith("----"))
                {
                    items[itemCount] = Double.Parse(line);
                }

                itemCount++;

                if (line.StartsWith("----"))
                {
                    _dtw.AddOrUpdate(frames, gestureName);
                    frames = new ArrayList();
                    gestureName = String.Empty;
                    itemCount = 0;
                }
            }

            file.Close();
        }
        #endregion


        #region Public API

        public KinectDiagnosticViewer()
        {
            InitializeComponent();
            //TO-DO: Organzie this
            swipeGestureRecognizer = new SwipeGestureDetector();
            swipeGestureRecognizer.OnGestureDetected += OnGestureDetected;
        }

        public RuntimeOptions RuntimeOptions { get; private set; }

        public void ReInitRuntime()
        {
            // Will call Uninitialize followed by Initialize.
            this.Kinect = this.Kinect;
        }

        public KinectNui.Runtime Kinect
        {
            get { return _Kinect; }
            set
            {
                //Clean up existing runtime if we are being set to null, or a new Runtime.
                if (_Kinect != null)
                {
                    _Kinect.SkeletonFrameReady -= new EventHandler<SkeletonFrameReadyEventArgs>(nui_SkeletonFrameReady);
                    _Kinect.Uninitialize();
                }
                _Kinect = value;

                if (_Kinect != null)
                {
                    InitRuntime();
                    _Kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(nui_SkeletonFrameReady);

                    UpdateUi();
                }
            }
        }

        //Status and InstanceIndex can change. Those properties in the Runtime should be made to support INotifyPropertyChange.
        public void UpdateUi()
        {
            kinectStatus.Text = _Kinect.Status.ToString();
        }
        #endregion Public API

        #region Init
        private void InitRuntime()
        {
            //Some Runtimes' status will be NotPowered, or some other error state. Only want to Initialize the runtime, if it is connected.
            if (_Kinect.Status == KinectStatus.Connected)
            {
                bool skeletalViewerAvailable = IsSkeletalViewerAvailable;

                // NOTE:  Skeletal tracking only works on one Kinect per process right now.
                RuntimeOptions = skeletalViewerAvailable ?
                                     RuntimeOptions.UseDepthAndPlayerIndex | RuntimeOptions.UseSkeletalTracking | RuntimeOptions.UseColor
                                     : RuntimeOptions.UseDepth | RuntimeOptions.UseColor;
                _Kinect.Initialize(RuntimeOptions);
                skeletonPanel.Visibility = skeletalViewerAvailable ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                if (RuntimeOptions.HasFlag(RuntimeOptions.UseSkeletalTracking))
                {
                    _Kinect.SkeletonEngine.TransformSmooth = true;
                }
            }
            _lastTime = DateTime.Now;

           
            _dtw = new DtwGestureRecognizer(12, 0.6, 2, 2, 10);
            _video = new ArrayList();
            LoadGesturesFromFile(GestureFileRecord);

            Skeleton2DDataExtract.Skeleton2DdataCoordReady += NuiSkeleton2DdataCoordReady;


          
        }
        
        /// <summary>
        /// Skeletal tracking only works on one Kinect right now.  So return false if it is already in use.
        /// </summary>
        private bool IsSkeletalViewerAvailable
        {
            get { return KinectNui.Runtime.Kinects.All(k => k.SkeletonEngine == null); }
        }

        #endregion Init

        #region Skeleton Processing
        private void nui_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            SkeletonFrame skeletonFrame = e.SkeletonFrame;
            
            //KinectSDK TODO: this shouldn't be needed, but if power is removed from the Kinect, you may still get an event here, but skeletonFrame will be null.
            if (skeletonFrame == null)
            {
                return;
            }

            int iSkeleton = 0;
            Brush[] brushes = new Brush[6];
            brushes[0] = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            brushes[1] = new SolidColorBrush(Color.FromRgb(0, 255, 0));
            brushes[2] = new SolidColorBrush(Color.FromRgb(64, 255, 255));
            brushes[3] = new SolidColorBrush(Color.FromRgb(255, 255, 64));
            brushes[4] = new SolidColorBrush(Color.FromRgb(255, 64, 255));
            brushes[5] = new SolidColorBrush(Color.FromRgb(128, 128, 255));

            skeletonCanvas.Children.Clear();
            foreach (SkeletonData data in skeletonFrame.Skeletons)
            {
                if (SkeletonTrackingState.Tracked == data.TrackingState)
                {
                    // Draw bones
                    Brush brush = brushes[iSkeleton % brushes.Length];
                    skeletonCanvas.Children.Add(getBodySegment(data.Joints, brush, JointID.HipCenter, JointID.Spine, JointID.ShoulderCenter, JointID.Head));
                    skeletonCanvas.Children.Add(getBodySegment(data.Joints, brush, JointID.ShoulderCenter, JointID.ShoulderLeft, JointID.ElbowLeft, JointID.WristLeft, JointID.HandLeft));
                    skeletonCanvas.Children.Add(getBodySegment(data.Joints, brush, JointID.ShoulderCenter, JointID.ShoulderRight, JointID.ElbowRight, JointID.WristRight, JointID.HandRight));
                    skeletonCanvas.Children.Add(getBodySegment(data.Joints, brush, JointID.HipCenter, JointID.HipLeft, JointID.KneeLeft, JointID.AnkleLeft, JointID.FootLeft));
                    skeletonCanvas.Children.Add(getBodySegment(data.Joints, brush, JointID.HipCenter, JointID.HipRight, JointID.KneeRight, JointID.AnkleRight, JointID.FootRight));

                    Skeleton2DDataExtract.ProcessData(data);

                    // Draw joints
                    foreach (Joint joint in data.Joints)
                    {
                        Point jointPos = getDisplayPosition(joint);
                        Line jointLine = new Line();
                        jointLine.X1 = jointPos.X - 3;
                        jointLine.X2 = jointLine.X1 + 6;
                        jointLine.Y1 = jointLine.Y2 = jointPos.Y;
                        jointLine.Stroke = jointColors[joint.ID];
                        jointLine.StrokeThickness = 6;
                        skeletonCanvas.Children.Add(jointLine);
                        if (joint.ID == JointID.HandRight || joint.ID == JointID.HandLeft)
                        {
                            swipeGestureRecognizer.Add(joint.Position, Kinect.SkeletonEngine);

                        }
                    }

                }
                iSkeleton++;
            } // for each skeleton
        }

        private Polyline getBodySegment(Microsoft.Research.Kinect.Nui.JointsCollection joints, Brush brush, params JointID[] ids)
        {
            PointCollection points = new PointCollection(ids.Length);
            for (int i = 0; i < ids.Length; ++i)
            {
                points.Add(getDisplayPosition(joints[ids[i]]));
            }

            Polyline polyline = new Polyline();
            polyline.Points = points;
            polyline.Stroke = brush;
            polyline.StrokeThickness = 5;
            return polyline;
        }

        private Point getDisplayPosition(Joint joint)
        {
            float depthX, depthY;
            Kinect.SkeletonEngine.SkeletonToDepthImage(joint.Position, out depthX, out depthY);
            depthX = depthX * 320; //convert to 320, 240 space
            depthY = depthY * 240; //convert to 320, 240 space
            int colorX, colorY;
            ImageViewArea iv = new ImageViewArea();
            // only ImageResolution.Resolution640x480 is supported at this point
            Kinect.NuiCamera.GetColorPixelCoordinatesFromDepthPixel(ImageResolution.Resolution640x480, iv, (int)depthX, (int)depthY, (short)0, out colorX, out colorY);

            // map back to skeleton.Width & skeleton.Height
            return new Point((int)(skeletonCanvas.Width * colorX / 640.0), (int)(skeletonCanvas.Height * colorY / 480));
        }

        private static Dictionary<JointID, Brush> jointColors = new Dictionary<JointID, Brush>() { 
            {JointID.HipCenter, new SolidColorBrush(Color.FromRgb(169, 176, 155))},
            {JointID.Spine, new SolidColorBrush(Color.FromRgb(169, 176, 155))},
            {JointID.ShoulderCenter, new SolidColorBrush(Color.FromRgb(168, 230, 29))},
            {JointID.Head, new SolidColorBrush(Color.FromRgb(200, 0,   0))},
            {JointID.ShoulderLeft, new SolidColorBrush(Color.FromRgb(79,  84,  33))},
            {JointID.ElbowLeft, new SolidColorBrush(Color.FromRgb(84,  33,  42))},
            {JointID.WristLeft, new SolidColorBrush(Color.FromRgb(255, 126, 0))},
            {JointID.HandLeft, new SolidColorBrush(Color.FromRgb(215,  86, 0))},
            {JointID.ShoulderRight, new SolidColorBrush(Color.FromRgb(33,  79,  84))},
            {JointID.ElbowRight, new SolidColorBrush(Color.FromRgb(33,  33,  84))},
            {JointID.WristRight, new SolidColorBrush(Color.FromRgb(77,  109, 243))},
            {JointID.HandRight, new SolidColorBrush(Color.FromRgb(37,   69, 243))},
            {JointID.HipLeft, new SolidColorBrush(Color.FromRgb(77,  109, 243))},
            {JointID.KneeLeft, new SolidColorBrush(Color.FromRgb(69,  33,  84))},
            {JointID.AnkleLeft, new SolidColorBrush(Color.FromRgb(229, 170, 122))},
            {JointID.FootLeft, new SolidColorBrush(Color.FromRgb(255, 126, 0))},
            {JointID.HipRight, new SolidColorBrush(Color.FromRgb(181, 165, 213))},
            {JointID.KneeRight, new SolidColorBrush(Color.FromRgb(71, 222,  76))},
            {JointID.AnkleRight, new SolidColorBrush(Color.FromRgb(245, 228, 156))},
            {JointID.FootRight, new SolidColorBrush(Color.FromRgb(77,  109, 243))}
        };
        #endregion Skeleton Processing

        #region Private State
        private KinectNui.Runtime _Kinect;
        #endregion Private State

        private void button1_Click(object sender, RoutedEventArgs e)
        {

            //Photo p = new Photo();
            //ArrayList a = p.getImage("https://api.500px.com/v1/photos?feature=editors&page=2&consumer_key=HJFMg2LvexMhIKrDs2qaVUxNem27kAxhdfiAYJMW");

            try
            {
                OAuthLinkedIn _oauth = new OAuthLinkedIn();
                String requestToken = _oauth.getRequestToken();
                //txtOutput.Text += "\n" + "Received request token: " + requestToken;
                //succOutput.Text += "Success";
                _oauth.authorizeToken();
                //txtOutput.Text += "\n" + "Token was authorized: " + _oauth.Token + " with verifier: " + _oauth.Verifier;
                //Should be checking the response ;) ;)
         
                String accessToken = _oauth.getAccessToken();
                if (_oauth.Token != "")
                {
                    lblLogin.Content = "You are logged in!";
                    Photo p = new Photo();
                    pictures = p.getImage("https://api.500px.com/v1/photos?feature=editors&page=2&consumer_key=HJFMg2LvexMhIKrDs2qaVUxNem27kAxhdfiAYJMW");
                    MemoryStream ms = new MemoryStream();

                    if (pictures != null)
                    {
                        System.Drawing.Image bm = (System.Drawing.Image)pictures[this.currentImg];
                        bm.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        System.Windows.Media.Imaging.BitmapImage bImg = new System.Windows.Media.Imaging.BitmapImage();
                        bImg.BeginInit();
                        bImg.StreamSource = new MemoryStream(ms.ToArray());
                        bImg.EndInit();
                        image1.Source = bImg;
                    }
                }
                else
                {
                    lblLogin.Content = "Login Failed!";
                }
                //txtOutput.Text += "\n" + "Access token was received: " + _oauth.Token;
            }
            catch (Exception exp)
            {
                //txtOutput.Text += "\nException: " + exp.Message;
            }
        }
    }
}
