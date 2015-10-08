using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LookAndPlayForm.Resumen
{
    public struct TargetTraceDefinitionEyeX
    {
        public List<GazeData> gazeDataItemL;
        public List<PointD> gazeWeigthedL;
        public List<PointD> gazeFilteredL;
        public bool clickInsideTarget;
    }

    public struct GazeData
    {
        public GazeDataEye Left { get; set; }
        public GazeDataEye Right { get; set; }
        public long Timestamp { get; set; }
        public TrackingStatus TrackingStatus { get; set; }
    }

    public struct GazeDataEye
    {
        public Point3D EyePositionFromEyeTrackerMM { get; set; }
        public Point3D EyePositionInTrackBoxNormalized { get; set; }
        public Point3D GazePointFromEyeTrackerMM { get; set; }
        public Point2D GazePointOnDisplayNormalized { get; set; }
    }

    public struct Point3D
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }

    public struct Point2D
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    public enum TrackingStatus
    {
        NoEyesTracked = 0,
        BothEyesTracked = 1,
        OnlyLeftEyeTracked = 2,
        OneEyeTrackedProbablyLeft = 3,
        OneEyeTrackedUnknownWhich = 4,
        OneEyeTrackedProbablyRight = 5,
        OnlyRightEyeTracked = 6,
    }
}
