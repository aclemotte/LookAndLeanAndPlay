using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aclemottelibs
{
    public class WimuData
    {
        public double yaw;
        public double pitch;
        public double roll;
        public double timeStampMiliSec;
        public bool validValue;

        public void SetNullValue()
        {
            this.yaw = 0;
            this.pitch = 0;
            this.roll = 0;
            this.timeStampMiliSec = 0;
            this.validValue = false; 
        }

    }
}
