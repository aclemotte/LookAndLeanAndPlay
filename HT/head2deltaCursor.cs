using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LookAndPlayForm.HT
{
    class Head2deltaCursor
    {
        HT.Wimu wimuDevice;
        WimuData refPointHead;
        PointD deltaCursor;
        double linearCoef = 50;


        public Head2deltaCursor(HT.Wimu wimuDevice)
        {
            this.wimuDevice = wimuDevice;
        }



        public PointD GetDeltaLocationFromHEADTracking()
        {
            if(refPointHead==null)
            {
                refPointHead = currentHeadLocation();
                deltaCursor = new PointD(0,0);
                return deltaCursor;
            }
            else
            {	
                PointD deltaCursorLocation = linearTransformation();
                return deltaCursorLocation;
            }
        }	







        WimuData currentHeadLocation()
        {
            WimuData currentHeadLocation = wimuDevice.WimuData;
            return currentHeadLocation;
        }

        PointD linearTransformation()
        {
            WimuData offsetHead = currentHeadLocation() + refPointHead;
            PointD deltaCursorLocation = offsetHead2CursorLocation(offsetHead);
            return deltaCursorLocation;
        }

        PointD offsetHead2CursorLocation(WimuData offsetHead)
        {
            PointD deltaCursorLocation = new PointD();
            deltaCursor.X = offsetHead.yaw * linearCoef;
            deltaCursor.Y = offsetHead.pitch * linearCoef;
            return deltaCursorLocation;
        }
    }

    class deltaCursor2Cursor
    {

    }
}
