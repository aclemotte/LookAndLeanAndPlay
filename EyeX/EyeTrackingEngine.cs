using System;
using System.Collections.Generic;
using System.Threading;
using Tobii.Gaze.Core;

namespace LookAndPlayForm
{

    /// <summary>
    /// The eyetracking engine provides gaze data from the currently setup eyetracker.
    /// It reads and validates the current eye tracker configuration,
    /// connects to and prepares the eye tracker for eyetracking and then
    /// provides gaze data until the eye tracker is disconnected or eyetracking engine is disposed. 
    /// </summary>
    public sealed class EyeTrackingEngine : IDisposable
    {
        public EventHandler<GazePointEventArgs> GazePoint;
        public event EventHandler onAddCalibrationPointCompletedEvent;
        public event EventHandler onStartCalibrationCompletedEvent;
        public event EventHandler onComputeandSetCalibrationCompletedEvent;
        public event EventHandler<CalibrationReadyEventArgs> OnGetCalibrationCompletedEvent;

        private Uri _eyeTrackerUrl;
        private IEyeTracker _eyeTracker;
        private Thread _thread;
        private eyesDetector detectorDeOjos;
        
        /// <summary>
        /// Create eye tracking engine 
        /// Throws EyeTrackerException if not successful
        /// </summary>
        public EyeTrackingEngine()
        {
            detectorDeOjos = new eyesDetector();            
        }


        /// <summary>
        /// Stop eye tracking and dispose eye tracking engine and Tobii EyeTracking
        /// </summary>
        public void Dispose()
        {
            if (_eyeTracker != null)
            {
                _eyeTracker.BreakEventLoop();
                if (_thread != null)
                {
                    _thread.Join();
                }

                _eyeTracker.EyeTrackerError -= OnEyeTrackerError;
                _eyeTracker.GazeData -= OnGazeData;
                _eyeTracker.Dispose();
                _eyeTracker = null;
            }

            if (_thread != null)
            {
                _thread.Abort();
                _thread = null;
            }
        }

        /// <summary>
        /// Initialize the eye tracker engine.
        /// State changes are notified to the client with the StateChanged event handler. 
        /// </summary>
        public void Initialize()
        {
            InitializeEyeTrackerAndRunEventLoop();
        }        

        public void requestStartCalibration()
        {
            _eyeTracker.StartCalibrationAsync(OnStartCalibrationCompleted);
        }

        public void AddCalibrationPoint(Point2D CalibrationDotPosition)
        {
            _eyeTracker.AddCalibrationPointAsync(CalibrationDotPosition, OnAddCalibrationPointCompleted);
        }

        public void ComputeAndSetCalibration()
        {
            _eyeTracker.ComputeAndSetCalibrationAsync(OnComputeAndSetCalibrationCompleted);
        }




        private void InitializeEyeTrackerAndRunEventLoop()
        {
            try
            {
                _eyeTrackerUrl = new EyeTrackerCoreLibrary().GetConnectedEyeTracker();
                if (_eyeTrackerUrl == null)
                {
                    return;
                }
            }
            catch (EyeTrackerException)
            {
                return;
            }

            try
            {
                _eyeTracker = new EyeTracker(_eyeTrackerUrl);
                _eyeTracker.EyeTrackerError += OnEyeTrackerError;
                _eyeTracker.GazeData += OnGazeData;

                CreateAndRunEventLoopThread();

                _eyeTracker.ConnectAsync(OnConnectFinished);

            }
            catch (EyeTrackerException)
            {
            }
        }

        private void CreateAndRunEventLoopThread()
        {
            if (_thread != null)
            {
                throw new InvalidOperationException("_thread parameter is already set");
            }

            _thread = new Thread(() =>
            {
                try
                {
                    _eyeTracker.RunEventLoop();
                }
                catch (EyeTrackerException)
                {
                    return;
                }
            });

            _thread.Start();
        }

        private void RaiseGazePoint(GazeData GazeData)
        {
            var handler = GazePoint;
            if (handler != null)
            {
                handler(this, new GazePointEventArgs(GazeData));
            }
        }

        
        //Cuando X tal cosa ...
        
        private void OnConnectFinished(ErrorCode errorCode)
        {
            if (errorCode != ErrorCode.Success)
            {
                return;
            }

            _eyeTracker.GetDeviceInfoAsync(OnDeviceInfoCompleted);
            _eyeTracker.GetCalibrationAsync(OnGetCalibrationCompleted);
            _eyeTracker.StartTrackingAsync(OnStartTrackingFinished);
        }

        private void OnDeviceInfoCompleted(DeviceInfo deviceInfo, Tobii.Gaze.Core.ErrorCode errorCode)
        {
            LookAndPlayForm.Program.datosCompartidos.EyeTrackerInfo = deviceInfo.Model;
        }

        private void OnGetCalibrationCompleted(Calibration calibration, Tobii.Gaze.Core.ErrorCode errorCode)
        {
            LookAndPlayForm.Program.datosCompartidos.calibrationDataEyeX = calibration;

            CalibrationError errorCalibracion = new CalibrationError(LookAndPlayForm.Program.datosCompartidos.calibrationDataEyeX.GetCalibrationPointDataItems());
            LookAndPlayForm.Program.datosCompartidos.meanCalibrationErrorLeftPx = errorCalibracion.meanCalibrationErrorLeftPx;
            LookAndPlayForm.Program.datosCompartidos.meanCalibrationErrorRightPx = errorCalibracion.meanCalibrationErrorRightPx;

            Console.WriteLine("EyetrackingEngine.OnGetCalibrationCompleted. errorCode: " + errorCode.ToString() + 
                "error: " +
                errorCalibracion.meanCalibrationErrorLeftPx.ToString() + ", " +
                errorCalibracion.meanCalibrationErrorRightPx.ToString());

            if (OnGetCalibrationCompletedEvent != null)
                OnGetCalibrationCompletedEvent(this, new CalibrationReadyEventArgs(errorCalibracion.CalibrationPointDataL));                       
        }

        private void OnStartCalibrationCompleted(ErrorCode errorCode)
        {
            Console.WriteLine("EyeTRackingEngine.OnStartCalibrationCompleted. errorCode: " + errorCode.ToString());

            if (errorCode != ErrorCode.Success)
            {
                return;
            }
            else
            {
                if (onStartCalibrationCompletedEvent != null) onStartCalibrationCompletedEvent(this, EventArgs.Empty);
            }
        }

        private void OnAddCalibrationPointCompleted(ErrorCode errorCode)
        {
            Console.WriteLine("EyeTrackingEngine.OnAddCalibrationPointCompleted. errorCode: " + errorCode.ToString());

            if (errorCode != ErrorCode.Success)
            {
                return;
            }

            if (onAddCalibrationPointCompletedEvent != null) onAddCalibrationPointCompletedEvent(this, EventArgs.Empty);
        }

        private void OnComputeAndSetCalibrationCompleted(ErrorCode errorCode)
        {
            Console.WriteLine("EyeTrackingEngine.OnComputeAndSetCalibrationCompleted. errorCode: " + errorCode.ToString());

            if (errorCode != ErrorCode.Success)
            {
                if (errorCode == ErrorCode.FirmwareOperationFailed)
                {
                }

                //return;
            }

            if (onComputeandSetCalibrationCompletedEvent != null) 
                onComputeandSetCalibrationCompletedEvent(this, EventArgs.Empty);
            
            _eyeTracker.StopCalibrationAsync(OnStopCalibrationCompleted);
        }

        private void OnStopCalibrationCompleted(ErrorCode errorCode)
        {
            Console.WriteLine("EyeTrackingEngine.OnStopCalibrationCompleted. errorCode: " + errorCode.ToString());

            _eyeTracker.GetCalibrationAsync(OnGetCalibrationCompleted);

            if (errorCode != ErrorCode.Success)
            {
                return;
            }

            //State = EyeTrackingState.Tracking;
        }

        private void OnStartTrackingFinished(ErrorCode errorCode)
        {
            if (errorCode != ErrorCode.Success)
            {
            }
        }

        private void OnEyeTrackerError(object sender, EyeTrackerErrorEventArgs eyeTrackerErrorEventArgs)
        {
            Console.WriteLine("EyeTrackingEngine.OnEyeTrackerError. errorCode: " + eyeTrackerErrorEventArgs.ErrorCode.ToString());

            if (eyeTrackerErrorEventArgs.ErrorCode != ErrorCode.Success)
            {
            }
        }

        private void OnGazeData(object sender, GazeDataEventArgs gazeDataEventArgs)
        {
            var gazeData = gazeDataEventArgs.GazeData;

            detectorDeOjos.dataReceived(gazeData.TrackingStatus);

            RaiseGazePoint(gazeData);
        }


    }

    public class CalibrationReadyEventArgs: EventArgs
    {
        public CalibrationReadyEventArgs(List<myCalibrationPointData> CalibrationPointDataLTemp)
        {
            CalibrationPointDataL = CalibrationPointDataLTemp;
        }

        public List<myCalibrationPointData> CalibrationPointDataL;
    }

    public class GazePointEventArgs : EventArgs
    {
        public GazePointEventArgs(GazeData GazedataTemp)
        {
            GazeDataReceived = GazedataTemp;
        }

        public GazeData GazeDataReceived;
    }
}
