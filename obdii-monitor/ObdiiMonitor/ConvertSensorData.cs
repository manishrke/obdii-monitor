using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObdiiMonitor
{
    class ConvertSensorData
    {
        private Controller controller;

        internal Controller Controller
        {
            set { controller = value; }
        }

        private byte[] initialAccelerometerReading = null;

        public void resetInitialAccelerometerReading()
        {
            initialAccelerometerReading = null;
        }

        public string convert(string dataTag, string data)
        {
            switch (dataTag)
            {
                // Accellerometer
                case "AC":
                    return caseAC(data);
                // Absolute Throttle Position
                case "11":
                    return case11(data);
                // Engine RPM
                case "0C":
                    return case0C(data);
                // Vehicle Speed
                case "0D":
                    return case0D(data);
                // Calculated Load Value
                case "04":
                    return case04(data);
                // Timing Advance (Cyl. #1)
                case "0E":
                    return case0E(data);
                // Intake Manifold Pressure
                case "0B":
                    return case0B(data);
                // Air Flow Rate (MAF sensor)
                case "10":
                    return case10(data);
            }

            return null;
        }

        // Accelerometer
        private string caseAC(string data)
        {
            if (data.Length != 3)
            {
                return "0";
            }

            ASCIIEncoding enc = new ASCIIEncoding();
            byte[] bytes = enc.GetBytes(data);

            if (initialAccelerometerReading == null)
            {
                initialAccelerometerReading = bytes;
            }

            int accelleration = bytes[2];

            return accelleration.ToString();
        }

        // Air Flow Rate (MAF sensor)
        private string case10(string data)
        {
            int num = Convert.ToInt32(data, 16);

            return (((double)num) * 0.0013227736).ToString();
        }

        // Intake Manifold Pressure
        private string case0B(string data)
        {
            int num = Convert.ToInt32(data, 16);

            return ((double)num / 3.386389).ToString();
        }

        // Timing Advance (Cyl. #1)
        private string case0E(string data)
        {
            int num = Convert.ToInt32(data, 16);

            return (((float)num - 128) / 2).ToString();
        }

        // Calculated Load Value
        private string case04(string data)
        {
            int num = Convert.ToInt32(data, 16);

            return ((float)num * 100 / 255).ToString();
        }

        // Vehicle Speed
        private string case0D(string data)
        {
            int num = Convert.ToInt32(data, 16);

            return ((int)(num / 1.609)).ToString();
        }

        // Engine RPM
        private string case11(string data)
        {
            int num = Convert.ToInt32(data, 16);

            return ((double) num * 100 / 255).ToString();
        }

        // Absolute Throttle Position
        private string case0C(string data)
        {
            int num = Convert.ToInt32(data, 16);

            return ((double)num / 4).ToString();
        }
    }
}
