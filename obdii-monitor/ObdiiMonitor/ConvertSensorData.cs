//-----------------------------------------------------------------------
// <copyright file="ConvertSensorData.cs" company="University of Louisville">
//     Copyright (c) 2010 All Rights Reserved
// </copyright>
//-----------------------------------------------------------------------
namespace ObdiiMonitor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Translates data collected from sensor to data this program can use
    /// </summary>
    public static class ConvertSensorData
    {
        /// <summary>
        /// True if using US units, false otherwise.
        /// </summary>
        private static bool us = false;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ConvertSensorData"/> is US.
        /// </summary>
        /// <value><c>true</c> if US; otherwise, <c>false</c>.</value>
        public static bool US
        {
            get { return us; }
            set { us = US; }
        }

        /// <summary>
        /// Converts the specified data tag and data to string program can use.
        /// </summary>
        /// <param name="dataTag">The data tag.</param>
        /// <param name="data">The data itself.</param>
        /// <param name="us">if set to <c>true</c> US, <c>false</c> is metric.</param>
        /// <returns>Converted Data</returns>
        public static string ConvertData(string dataTag, string data, bool us)
        {
            US = us;
            switch (dataTag)
            {
                // Calculated Load Value
                case "04":
                case "11":
                case "2C":
                case "2E":
                case "2F":
                case "45":
                case "47":
                case "48":
                case "49":
                case "4A":
                case "4B":
                case "4C":
                case "52":
                    return Case100per(data);
                case "34":
                case "35":
                case "36":
                case "37":
                case "38":
                case "39":
                case "3A":
                case "3B":
                    return Casecurr(data);
                case "24":
                case "25":
                case "26":
                case "27":
                case "28":
                case "29":
                case "2A":
                case "2B":
                    return Casevol(data);
                case "14":
                case "15":
                case "16":
                case "17":
                case "18":
                case "19":
                case "1A":
                case "1B":
                    return Casebanksens(data);
                case "06":
                case "07":
                case "08":
                case "09":
                case "2D":
                    return Case100per2(data);
                case "3C":
                case "3D":
                case "3E":
                case "3F":
                    return Casetemp2(data);
                case "4D":
                case "4E":
                case "1F":
                    return Casemins(data);
                case "05":
                case "0F":
                case "46":
                    return Temp1(data);
                case "31":
                case "21":
                    return Casekm(data);

                // Intake Manifold Pressure
                case "0B": 
                case "33": 
                    return Case0B(data);
                case "0A":
                    return Case0A(data);
                case "0D": // km/h
                    return Case0D(data);
                case "30": // A
                    return Case30(data);

                // Engine RPM
                case "0C":
                    return Case0C(data);

                // Timing Advance (Cyl. #1)
                case "0E":
                    return Case0E(data);

                // Air Flow Rate (MAF sensor)
                case "10":
                    return Case10(data);
                case "42":
                    return Case42(data);
                case "43":
                    return Case43(data);
                case "44":
                    return Case44(data);
                case "32":
                    return Case32(data);
                case "23":
                    return Case23(data);
                case "22":
                    return Case22(data);
            }

            return null;
        }

        // Calculated Load Value

        /// <summary>
        /// Case of a datatag being 2D.
        /// </summary>
        /// <param name="data">The data itself.</param>
        /// <returns>Converted Data</returns>
        private static string Case100per(string data)
        {
            uint num = Convert.ToUInt32(data, 16);
            return ((float)num * 100 / 255).ToString();
        }

        /// <summary>
        /// Case of a datatag being 47.
        /// </summary>
        /// <param name="data">The data itself.</param>
        /// <returns>Converted Data</returns>
        private static string Temp1(string data)
        {
            uint num = Convert.ToUInt32(data, 16);
            if (US)
            {
                return (((((double)num - 40) * 9.0) / 5.0) + 32).ToString();
            }

            return (((double)num) - 40).ToString();
        }

        /// <summary>
        /// Case of a datatag being 2D.
        /// </summary>
        /// <param name="data">The data itself.</param>
        /// <returns>Converted Data</returns>
        private static string Case100per2(string data)
        {
            uint num = Convert.ToUInt32(data, 16);
            return ((((double)num) - 128) * 100.0 / 128.0).ToString();
        }

        /// <summary>
        /// Case of a datatag being 0A.
        /// </summary>
        /// <param name="data">The data itself.</param>
        /// <returns>Converted Data</returns>
        private static string Case0A(string data)
        {
            uint num = Convert.ToUInt32(data, 16);
            if (US)
            {
                return (((double)num) * 3 * 0.145037738).ToString();
            }

            return (((double)num) * 3).ToString();
        }

        /// <summary>
        /// Case of a datatag representing intake manifold pressure.
        /// </summary>
        /// <param name="data">The data itself.</param>
        /// <returns>Converted Data</returns>
        private static string Case0B(string data)
        {
            uint num = Convert.ToUInt32(data, 16);
            if (US)
            {
                return ((double)num / 3.386389).ToString();
            }

            return ((double)num).ToString();
        }

        /// <summary>
        /// Case of a datatag representing Absolute Throttle Position
        /// </summary>
        /// <param name="data">The data itself.</param>
        /// <returns>Converted Data</returns>
        private static string Case0C(string data)
        {
            uint num = Convert.ToUInt32(data, 16);
            return ((double)num / 4).ToString();
        }

        /// <summary>
        /// Case of a datatag representing Vehicle Speed
        /// </summary>
        /// <param name="data">The data itself.</param>
        /// <returns>Converted Data</returns>
        private static string Case0D(string data)
        {
            uint num = Convert.ToUInt32(data, 16);
            if (US)
            {
                return ((int)(num / 1.609)).ToString();
            }

            return ((double)num).ToString();
        }

        /// <summary>
        /// Case of a datatag representing Timing Advance (Cyl. #1)
        /// </summary>
        /// <param name="data">The data itself.</param>
        /// <returns>Converted Data</returns>
        private static string Case0E(string data)
        {
            uint num = Convert.ToUInt32(data, 16);
            return (((float)num - 128) / 2).ToString();
        }

        /// <summary>
        /// Case of a datatag representing Air Flow Rate (MAF sensor)
        /// </summary>
        /// <param name="data">The data itself.</param>
        /// <returns>Converted Data</returns>
        private static string Case10(string data)
        {
            uint num = Convert.ToUInt32(data, 16);
            if (US)
            {
                return (((double)num) * 0.0013227736).ToString();
            }

            return (((double)num) * 0.01).ToString();
        }

        /// <summary>
        /// Case of a datatag being 30.
        /// </summary>
        /// <param name="data">The data itself.</param>
        /// <returns>Converted Data</returns>
        private static string Case30(string data)
        {
            uint num = Convert.ToUInt32(data, 16);
            return ((double)num).ToString();
        }

        /// <summary>
        /// Case of a datatag being 3F.
        /// </summary>
        /// <param name="data">The data itself.</param>
        /// <returns>Converted Data</returns>
        private static string Casetemp2(string data)
        {
            uint a = Convert.ToUInt32(data.Substring(0, 2), 16);
            uint b = Convert.ToUInt32(data.Substring(2), 16);
            if (US)
            {
                return (((double)(((((a * 256) + b) / 10.0) - 40) * 9.0) / 5.0) + 32).ToString();
            }

            return ((double)(((a * 256) + b) / 10.0) - 40).ToString();
        }

        /// <summary>
        /// Case of a datatag being 42.
        /// </summary>
        /// <param name="data">The data itself.</param>
        /// <returns>Converted Data</returns>
        private static string Case42(string data)
        {
            uint a = Convert.ToUInt32(data.Substring(0, 2), 16);
            uint b = Convert.ToUInt32(data.Substring(2), 16);
            return ((double)((a * 256) + b) / 1000.0).ToString();
        }

        /// <summary>
        /// Case of a datatag being 43.
        /// </summary>
        /// <param name="data">The data itself.</param>
        /// <returns>Converted Data</returns>
        private static string Case43(string data)
        {
            uint a = Convert.ToUInt32(data.Substring(0, 2), 16);
            uint b = Convert.ToUInt32(data.Substring(2), 16);
            return ((double)((a * 256) + b) * 100.0 / 255.0).ToString();
        }

        /// <summary>
        /// Case of a datatag being 1F.
        /// </summary>
        /// <param name="data">The data itself.</param>
        /// <returns>Converted Data</returns>
        private static string Casemins(string data)
        {
            uint a = Convert.ToUInt32(data.Substring(0, 2), 16);
            uint b = Convert.ToUInt32(data.Substring(2), 16);
            return ((double)(a * 256) + b).ToString();
        }

        /// <summary>
        /// Case of a datatag being 44.
        /// </summary>
        /// <param name="data">The data itself.</param>
        /// <returns>Converted Data</returns>
        private static string Case44(string data)
        {
            uint a = Convert.ToUInt32(data.Substring(0, 2), 16);
            uint b = Convert.ToUInt32(data.Substring(2), 16);
            return ((double)((a * 256) + b) / 32768.0).ToString();
        }

        /// <summary>
        /// Case of a datatag being 32.
        /// </summary>
        /// <param name="data">The data itself.</param>
        /// <returns>Converted Data</returns>
        private static string Case32(string data)
        {
            int a = Convert.ToInt32(data.Substring(0, 2), 16);
            uint b = Convert.ToUInt32(data.Substring(2), 16);
            if (US)
            {
                return (((double)((a * 256) + b) / 4.0) / 249.088908).ToString();
            }

            return ((double)((a * 256) + b) / 4.0).ToString();
        }

        /// <summary>
        /// Case of a datatag being 21.
        /// </summary>
        /// <param name="data">The data itself.</param>
        /// <returns>Converted Data</returns>
        private static string Casekm(string data)
        {
            uint a = Convert.ToUInt32(data.Substring(0, 2), 16);
            uint b = Convert.ToUInt32(data.Substring(2), 16);
            if (US)
            {
                return ((double)((a * 256) + b) / 1.609).ToString();
            }

            return ((double)((a * 256) + b)).ToString();
        }

        /// <summary>
        /// Case of a datatag being 23.
        /// </summary>
        /// <param name="data">The data itself.</param>
        /// <returns>Converted Data</returns>
        private static string Case23(string data)
        {
            uint a = Convert.ToUInt32(data.Substring(0, 2), 16);
            uint b = Convert.ToUInt32(data.Substring(2), 16);
            if (US)
            {
                return ((double)((a * 256) + b) * 10 * 0.145037738).ToString();
            }

            return ((double)((a * 256) + b) * 10).ToString();
        }

        /// <summary>
        /// Case of a datatag being 22.
        /// </summary>
        /// <param name="data">The data itself.</param>
        /// <returns>Converted Data</returns>
        private static string Case22(string data)
        {
            uint a = Convert.ToUInt32(data.Substring(0, 2), 16);
            uint b = Convert.ToUInt32(data.Substring(2), 16);
            if (US)
            {
                return ((double)(((a * 256) + b) * 100 * 0.145037738) / 128.0).ToString();
            }

            return ((double)((a * 256) + b) * 10 / 128.0).ToString();
        }

        /// <summary>
        /// Case of current sensor.
        /// </summary>
        /// <param name="data">The data itself.</param>
        /// <returns>Converted Data</returns>
        private static string Casecurr(string data)
        {
            uint c = Convert.ToUInt32(data.Substring(4, 2), 16);
            uint d = Convert.ToUInt32(data.Substring(6, 2), 16);

            return (((double)((c * 256) + d) / 256.0) - 128.0).ToString();
        }

        /// <summary>
        /// Case of volume sensor
        /// </summary>
        /// <param name="data">The data itself.</param>
        /// <returns>Converted Data</returns>
        private static string Casevol(string data)
        {
            uint c = Convert.ToUInt32(data.Substring(4, 2), 16);
            uint d = Convert.ToUInt32(data.Substring(6, 2), 16);

            return ((double)((c * 256) + d) / 8192.0).ToString();
        }

        /// <summary>
        /// Case of bank sensor
        /// </summary>
        /// <param name="data">The data itself.</param>
        /// <returns>Converted Data</returns>
        private static string Casebanksens(string data)
        {
            uint a = Convert.ToUInt32(data.Substring(0, 2), 16);

            return ((double)(a * 0.005)).ToString();
        }
    }
}
