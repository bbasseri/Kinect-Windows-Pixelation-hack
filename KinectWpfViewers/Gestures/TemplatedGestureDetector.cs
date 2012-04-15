﻿using System.Linq;
using Microsoft.Research.Kinect.Nui;
using System.IO;

namespace pixcelation.gestures
{
    public class TemplatedGestureDetector : GestureDetector
    {
        const float Epsilon = 0.035f;
        const float MinimalScore = 0.80f;
        const float MinimalSize = 0.1f;
        readonly LearningMachine learningMachine;
        RecordedPath path;
        readonly string gestureName;

        public bool IsRecordingPath
        {
            get { return path != null; }
        }

        public LearningMachine LearningMachine
        {
            get { return learningMachine; }
        }

        public TemplatedGestureDetector(string gestureName, Stream kbStream, int windowSize = 60)
            : base(windowSize)
        {
            this.gestureName = gestureName;
            learningMachine = new LearningMachine(kbStream);
        }

        public override void Add(Vector position, SkeletonEngine engine)
        {
            base.Add(position, engine);

            if (path != null)
            {
                path.Points.Add(position.ToVector2());
            }
        }

        protected override void LookForGesture()
        {
            if (LearningMachine.Match(Entries.Select(e => new Vector2(e.Position.X, e.Position.Y)).ToList(), Epsilon, MinimalScore, MinimalSize))
                RaiseGestureDetected(gestureName);
        }

        public void StartRecordTemplate()
        {
            path = new RecordedPath(WindowSize);
        }

        public void EndRecordTemplate()
        {
            LearningMachine.AddPath(path);
            path = null;
        }

        public void SaveState(Stream kbStream)
        {
            LearningMachine.Persist(kbStream);
        }
    }
}
