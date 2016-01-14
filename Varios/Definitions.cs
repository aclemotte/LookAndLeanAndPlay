using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LookAndPlayForm.Definitions
{       
    public enum clictype
    {
        dwell, mouse, meterse
    };

    public enum filtertype
    {
        meanMedian, average
    };

    public enum pointercontroltype
    {
        mouse, eyetracker, headtracker
    }

    public enum eyetrackertype
    {
        tobii, eyetribe
    }
}
