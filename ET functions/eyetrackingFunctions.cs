using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tobii.Gaze.Core;

namespace LookAndPlayForm
{
    public static class eyetrackingFunctions
    {
        public static PointD WeighGaze(Tobii.Gaze.Core.GazeData GazeData2Weigh)
        {
            PointD GazeWeighed = new PointD();
            double leftWeigh;
            double rightWeigh;
            double totalWeigh;

            switch (GazeData2Weigh.TrackingStatus)
            {
                //los dos ojos
                case TrackingStatus.BothEyesTracked:
                    leftWeigh = 1;
                    rightWeigh = 1;
                    totalWeigh = 0.5;
                    break;
                
                //left solo
                case TrackingStatus.OneEyeTrackedProbablyLeft:
                    leftWeigh = 1;
                    rightWeigh = 0;
                    totalWeigh = 1;
                    break;
                case TrackingStatus.OnlyLeftEyeTracked:
                    leftWeigh = 1;
                    rightWeigh = 0;
                    totalWeigh = 1;
                    break;

                //rigth solo
                case TrackingStatus.OneEyeTrackedProbablyRight:
                    leftWeigh = 0;
                    rightWeigh = 1;
                    totalWeigh = 1;
                    break;
                case TrackingStatus.OnlyRightEyeTracked:
                    leftWeigh = 0;
                    rightWeigh = 1;
                    totalWeigh = 1;
                    break;
                
                //peores casos
                case TrackingStatus.NoEyesTracked:
                    leftWeigh = Double.NaN;
                    rightWeigh = Double.NaN;
                    totalWeigh = Double.NaN;
                    break;
                case TrackingStatus.OneEyeTrackedUnknownWhich:
                    leftWeigh = Double.NaN;
                    rightWeigh = Double.NaN;
                    totalWeigh = Double.NaN;
                    break;

                default:
                    leftWeigh = Double.NaN;
                    rightWeigh = Double.NaN;
                    totalWeigh = Double.NaN;
                    break;
            }

            GazeWeighed.X = ((GazeData2Weigh.Left.GazePointOnDisplayNormalized.X) * leftWeigh + (GazeData2Weigh.Right.GazePointOnDisplayNormalized.X * rightWeigh)) * totalWeigh;
            GazeWeighed.Y = ((GazeData2Weigh.Left.GazePointOnDisplayNormalized.Y) * leftWeigh + (GazeData2Weigh.Right.GazePointOnDisplayNormalized.Y * rightWeigh)) * totalWeigh;                    

            return GazeWeighed;
        }        

        public static int distanceBetweenDev2User(Tobii.Gaze.Core.GazeData SoruceGazeData)
        {
            double distanceDev2User = (SoruceGazeData.Left.EyePositionInTrackBoxNormalized.Z + SoruceGazeData.Right.EyePositionInTrackBoxNormalized.Z) * 0.5 * 100;
            int returnValue = Convert.ToInt32(distanceDev2User);

            if (returnValue > 100)
            {
                returnValue = 100;
            }
            else if (returnValue < 0)
            {
                returnValue = 0;
            }

            return returnValue;
        }

        public static PointD normalized2Pixels(PointD normalized)
        {
            double posX = normalized.X * (double)(Screen.PrimaryScreen.Bounds.Size.Width + Screen.PrimaryScreen.Bounds.X);
            double posY = normalized.Y * (double)(Screen.PrimaryScreen.Bounds.Size.Height + Screen.PrimaryScreen.Bounds.Y);
            PointD posicionMousePx = new PointD(posX, posY);
            return posicionMousePx;
        }  
    }
}
