using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LookAndPlayForm.HT
{
    public class Wimu : IDisposable
    {

        public bool serialPortConfigured { get; set; }
        public WimuData WimuData { get; set; }




        BackgroundWorker trabajador;
        static SerialPort wimuSerialPort;





        public Wimu(string wimuSerialPortNumber)
        {
            WimuData = new WimuData();
            serialPortConfigured = configSerialPort(wimuSerialPortNumber);
            if(serialPortConfigured)
                configBw();
        }

        public void Dispose()
        {
            trabajador.CancelAsync();
            wimuSerialPort.Close();
        }










        private bool configBw()
        {
            trabajador = new BackgroundWorker();
            trabajador.WorkerSupportsCancellation = true;
            trabajador.WorkerReportsProgress = false;
            trabajador.DoWork += bw_DoWork;
            trabajador.RunWorkerAsync();
            return true;
        }

        private bool configSerialPort(string wimuSerialPortNumber)
        {
            wimuSerialPort = new SerialPort();
            wimuSerialPort.ReadTimeout = 500;
            wimuSerialPort.WriteTimeout = 500;
            wimuSerialPort.PortName = wimuSerialPortNumber;
            wimuSerialPort.BaudRate = 57600;
            wimuSerialPort.Parity = Parity.None;
            wimuSerialPort.DataBits = 8;
            wimuSerialPort.StopBits = StopBits.One;
            wimuSerialPort.Handshake = Handshake.None;

            try
            {
                wimuSerialPort.Open();
                return true; // Success
            }
            catch
            {
                return false; // Failure
            }
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!trabajador.CancellationPending)
            {
                WimuData = readWimuData();
            }
        }

        private WimuData readWimuData()
        {
            WimuData wimudata = new WimuData();
            string message = string.Empty;

            do
            {
                try
                {
                    message = wimuSerialPort.ReadLine();
                }
                catch 
                { 
                    //cancelar al trabajador
                    //retornar nada
                    trabajador.CancelAsync();
                    wimudata.SetNullValue();
                    return wimudata;
                }

            }
            while (!message.StartsWith("#YPR") || !message.EndsWith("\r"));

            string[] words = message.Split('=',',','\r');
            //#YPR=Yaw, Pitch, Roll, Yaw+Pitch+Roll, alfa, beta, gama, alfa+beta+gama, timeStamp
            //ver si el último carácter es una "r" para saber si esta completo
            /*
                [0]: "#YPR"
                [1]: "-18.16" yaw
                [2]: "-10.29" pitch
                [3]: "21.03" roll
                [4]: "-7.42"
                [5]: "-3.60"
                [6]: "-30.02"
                [7]: "-4.13"
                [8]: "-37.75"
                [9]: "285980" timestamp
                [10]: ""
             */

            try
            {
                wimudata.yaw = Convert.ToDouble(words[1], CultureInfo.InvariantCulture); // X YAW   
                wimudata.pitch = Convert.ToDouble(words[2], CultureInfo.InvariantCulture); // PITCH   
                wimudata.roll = Convert.ToDouble(words[3], CultureInfo.InvariantCulture); // Y ROLL  
                wimudata.timeStampMiliSec = Convert.ToDouble(words[9], CultureInfo.InvariantCulture);
                wimudata.validValue = true;
            }
            catch
            {
                wimudata.SetNullValue();
            }

            return wimudata;
        }

    }
}
