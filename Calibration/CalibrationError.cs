using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tobii.Gaze.Core;

namespace LookAndPlayForm
{
    public struct myCalibrationPointData
    {
        public Point2D LeftMapPosition;
        public CalibrationPointStatus LeftStatus;
        public Point2D RightMapPosition;
        public CalibrationPointStatus RightStatus;
        public Point2D TruePosition;
    }

    class CalibrationError
    {
        private const float PaddingRatio = 0.07F;
        private IEnumerable<CalibrationPointData> _calibrationData;

        private readonly List<Point2D> _calibrationPoints;

        private List<int> _calibrationErroLeftPx;
        private List<int> _calibrationErrorRightPx;

        public int meanCalibrationErrorLeftPx;
        public int meanCalibrationErrorRightPx;
               
        public List<myCalibrationPointData> CalibrationPointDataL;

        public CalibrationError(IEnumerable<CalibrationPointData> calibrationData)
        {
            _calibrationPoints = new List<Point2D>();

            _calibrationErroLeftPx = new List<int>();
            _calibrationErrorRightPx = new List<int>();

            CalibrationPointDataL = new List<myCalibrationPointData>();

            _calibrationData = calibrationData;
            calculateCalibrationError();
        }

        private void calculateCalibrationError()
        {
            if (_calibrationData != null && _calibrationPoints != null)
            {
                _calibrationErroLeftPx.Clear();
                _calibrationErrorRightPx.Clear();

                foreach (var plotItem in _calibrationData)
                {

                    myCalibrationPointData myCalibrationPointDataTemp = new myCalibrationPointData();
                    myCalibrationPointDataTemp.LeftMapPosition = plotItem.LeftMapPosition;
                    myCalibrationPointDataTemp.LeftStatus = plotItem.LeftStatus;
                    myCalibrationPointDataTemp.RightMapPosition = plotItem.RightMapPosition;
                    myCalibrationPointDataTemp.RightStatus = plotItem.RightStatus;
                    myCalibrationPointDataTemp.TruePosition = plotItem.TruePosition;
                    CalibrationPointDataL.Add(myCalibrationPointDataTemp);

                    if (plotItem.LeftStatus == CalibrationPointStatus.CalibrationPointValidAndUsedInCalibration)
                    {

                        Point p1 = PixelPointFromNormalizedPoint(plotItem.TruePosition);
                        Point p2 = PixelPointFromNormalizedPoint(plotItem.LeftMapPosition);
                        _calibrationErroLeftPx.Add(distanceBetweenPoints(p1, p2));
                    }

                    if (plotItem.RightStatus == CalibrationPointStatus.CalibrationPointValidAndUsedInCalibration)
                    {

                        Point p1 = PixelPointFromNormalizedPoint(plotItem.TruePosition);
                        Point p2 = PixelPointFromNormalizedPoint(plotItem.RightMapPosition);
                        _calibrationErrorRightPx.Add(distanceBetweenPoints(p1, p2));
                    }
                }
                meanCalibrationErrorLeftPx = getMeanCalibrationErrorPx(_calibrationErroLeftPx);
                meanCalibrationErrorRightPx = getMeanCalibrationErrorPx(_calibrationErrorRightPx);
            }
        }

        private int distanceBetweenPoints(Point punto1, Point punto2)
        {
            int retorno;
            retorno = (int)Math.Sqrt(Math.Pow(Convert.ToDouble(punto1.X) - Convert.ToDouble(punto2.X), 2) + Math.Pow(Convert.ToDouble(punto1.Y) - Convert.ToDouble(punto2.Y), 2));
            return retorno;
        }

        private Point PixelPointFromNormalizedPoint(Point2D normalizedPoint)
        {
            Point pixelPoint = new Point();
            pixelPoint.X = (int)(normalizedPoint.X * Screen.PrimaryScreen.Bounds.Width);
            pixelPoint.Y = (int)(normalizedPoint.Y * Screen.PrimaryScreen.Bounds.Height);
            return pixelPoint;
        }

        private int getMeanCalibrationErrorPx(List<int> _calibrationErroLeftPx)
        {
            double sumaError = 0;

            for (int i = 0; i < _calibrationErroLeftPx.Count; i++)
            {
                sumaError += _calibrationErroLeftPx[i];
            }
            int result = (int)(sumaError / _calibrationErroLeftPx.Count);
            return result;
        }
    }
}
