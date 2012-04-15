using System;

namespace pixcelation.gestures.Record
{
    public class ReplaySkeletonFrameReadyEventArgs : EventArgs
    {
        public ReplaySkeletonFrame SkeletonFrame { get; set; }
    }
}
