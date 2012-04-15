using System;
using System.Windows.Shapes;
using Kinect.Toolbox;

 namespace pixcelation
{
    public class Entry
    {
        public DateTime Time { get; set; }
        public Vector3 Position { get; set; }
        public Ellipse DisplayEllipse { get; set; }
    }
}
