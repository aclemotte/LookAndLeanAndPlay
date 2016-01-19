using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LookAndPlayForm.HT
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

        public WimuData() { }

        public WimuData(double yaw, double pitch, double roll)
        {
            this.yaw = yaw;
            this.pitch = pitch;
            this.roll = roll;
        }

        public static WimuData operator +(WimuData data1, WimuData data2)
        {
            return new WimuData(data1.yaw + data2.yaw, data1.pitch + data2.pitch, data1.roll + data2.roll);
        }
        public static WimuData operator -(WimuData data1, WimuData data2)
        {
            return new WimuData(data1.yaw - data2.yaw, data1.pitch - data2.pitch, data1.roll - data2.roll);
        }
    }
}
