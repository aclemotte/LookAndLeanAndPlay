using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LookAndPlayForm.Fusionador
{
    public static class fusionador
    {
        public static PointD getCursorLocation(bool fijacion, PointD deltaCursor, PointD gaze)
        {
            if (fijacion)
            {
                PointD headPlusEyes = gaze + deltaCursor;
                return headPlusEyes;
            }
            else
                return gaze;
        }
    }
}
