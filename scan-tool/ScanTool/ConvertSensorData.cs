using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScanTool
{
    class ConvertSensorData
    {
        public static string convert(string dataTag, string data)
        {
            switch (dataTag)
            {
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

        // Air Flow Rate (MAF sensor)
        private static string case10(string data)
        {
            int num = Convert.ToInt32(data, 16);

            return (((double)num) * 0.0013227736).ToString();
        }

        // Intake Manifold Pressure
        private static string case0B(string data)
        {
            int num = Convert.ToInt32(data, 16);

            return ((double)num / 3.386389).ToString();
        }

        // Timing Advance (Cyl. #1)
        private static string case0E(string data)
        {
            int num = Convert.ToInt32(data, 16);

            return (((float)num - 128) / 2).ToString();
        }

        // Calculated Load Value
        private static string case04(string data)
        {
            int num = Convert.ToInt32(data, 16);

            return ((float)num * 100 / 255).ToString();
        }

        // Vehicle Speed
        private static string case0D(string data)
        {
            int num = Convert.ToInt32(data, 16);

            return ((int)(num / 1.609)).ToString();
        }

        // Engine RPM
        private static string case11(string data)
        {
            int num = Convert.ToInt32(data, 16);

            return ((double) num * 100 / 255).ToString();
        }

        // Absolute Throttle Position
        private static string case0C(string data)
        {
            int num = Convert.ToInt32(data, 16);

            return ((double)num / 4).ToString();
        }
    }
}
