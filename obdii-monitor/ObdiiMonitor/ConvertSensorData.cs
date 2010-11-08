using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObdiiMonitor
{
    static class ConvertSensorData
    {
        private static bool US;
        public static string convert(string dataTag, string data, bool us)
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
                    return case100per(data);
                case "34":
                case "35":
                case "36":
                case "37":
                case "38":
                case "39":
                case "3A":
                case "3B":
                    return casecurr(data);
                case "24":
                case "25":
                case "26":
                case "27":
                case "28":
                case "29":
                case "2A":
                case "2B":
                    return casevol(data);
                case "14":
                case "15":
                case "16":
                case "17":
                case "18":
                case "19":
                case "1A":
                case "1B":
                    return casebanksens(data);
                case "06":
                case "07":
                case "08":
                case "09":
                case "2D":
                    return case100per2(data);
                case "3C":
                case "3D":
                case "3E":
                case "3F":
                    return casetemp2(data);
                case "4D":
                case "4E":
                case "1F":
                    return casemins(data);
                case "05":
                case "0F":
                case "46":
                    return temp1(data);
                case "31":
                case "21":
                    return casekm(data);
                // Intake Manifold Pressure
                case "0B": 
                case "33": 
                    return case0B(data);
                case "0A":
                    return case0A(data);
                case "0D": //km/h
                    return case0D(data);
                case "30": //A
                    return case30(data);
                // Engine RPM
                case "0C":
                    return case0C(data);
                // Timing Advance (Cyl. #1)
                case "0E":
                    return case0E(data);
                // Air Flow Rate (MAF sensor)
                case "10":
                    return case10(data);
                case "42":
                    return case42(data);
                case "43":
                    return case43(data);
                case "44":
                    return case44(data);
                case "32":
                    return case32(data);
                case "23":
                    return case23(data);
                case "22":
                    return case22(data);
            }

            return null;
        }


        // Calculated Load Value
        private static string case100per(string data)
        {
            uint num = Convert.ToUInt32(data, 16);
            return ((float)num * 100 / 255).ToString();
        }

        private static string temp1(string data)
        {
            uint num = Convert.ToUInt32(data, 16);
            if (US)
                return (((double)num - 40) * 9.0 / 5.0 + 32).ToString();
            return (((double)num) - 40).ToString();
        }

        private static string case100per2(string data)
        {
            uint num = Convert.ToUInt32(data, 16);
            return ((((double)num) - 128)*100.0/128.0).ToString();
        }
        private static string case0A(string data)
        {
            uint num = Convert.ToUInt32(data, 16);
            if(US)
                return (((double)num) * 3 * 0.145037738).ToString();
            return (((double)num) * 3).ToString();

        }
        private static string case0B(string data)
        {
            uint num = Convert.ToUInt32(data, 16);
            if (US)
                return ((double)num / 3.386389).ToString();
            return ((double)num).ToString();
        }
        // Absolute Throttle Position
        private static string case0C(string data)
        {
            uint num = Convert.ToUInt32(data, 16);
            return ((double)num / 4).ToString();
        }
        // Vehicle Speed
        private static string case0D(string data)
        {
            uint num = Convert.ToUInt32(data, 16);
            if(US)
                return ((int)(num / 1.609)).ToString();
            return ((double)num).ToString();
        }
        // Timing Advance (Cyl. #1)
        private static  string case0E(string data)
        {
            uint num = Convert.ToUInt32(data, 16);
            return (((float)num - 128) / 2).ToString();
        }
        // Air Flow Rate (MAF sensor)
        private static string case10(string data)
        {
            uint num = Convert.ToUInt32(data, 16);
            if(US)
                return (((double)num) * 0.0013227736).ToString();
            return (((double)num) * 0.01).ToString();
        }
        private static string case30(string data)
        {
            uint num = Convert.ToUInt32(data, 16);
            return ((double)num).ToString();
        }
        private static string casetemp2(string data)
        {
            uint A = Convert.ToUInt32(data.Substring(0,2), 16);
            uint B = Convert.ToUInt32(data.Substring(2), 16);
            if (US)
                return (((double)((A*256)+B)/10.0 - 40) * 9.0 / 5.0 + 32).ToString();
            return ((double)((A*256)+B)/10.0 - 40 ).ToString();
        }
        private static string case42(string data)
        {
            uint A = Convert.ToUInt32(data.Substring(0,2), 16);
            uint B = Convert.ToUInt32(data.Substring(2), 16);
            return ((double)((A*256)+B)/1000.0).ToString();
        }
        private static string case43(string data)
        {
            uint A = Convert.ToUInt32(data.Substring(0,2), 16);
            uint B = Convert.ToUInt32(data.Substring(2), 16);
            return ((double)((A*256)+B)*100.0/255.0).ToString();
        }
        private static string casemins(string data)
        {
            uint A = Convert.ToUInt32(data.Substring(0, 2), 16);
            uint B = Convert.ToUInt32(data.Substring(2), 16);
            return ((double)(A*256)+B).ToString();
        }
        private static string case44(string data)
        {
            uint A = Convert.ToUInt32(data.Substring(0, 2), 16);
            uint B = Convert.ToUInt32(data.Substring(2), 16);
            return ((double)((A*256)+B)/32768.0).ToString();
        }
        private static string case32(string data)
        {
            int A = Convert.ToInt32(data.Substring(0,2), 16);
            uint B = Convert.ToUInt32(data.Substring(2), 16);
            if (US)
                return (((double)((A * 256) + B) / 4.0) / 249.088908).ToString();
            return ((double)((A*256)+B)/4.0).ToString();
        }
        private static string casekm(string data)
        {
            uint A = Convert.ToUInt32(data.Substring(0, 2), 16);
            uint B = Convert.ToUInt32(data.Substring(2), 16);
            if(US)
                return ((double)((A * 256) + B) / 1.609).ToString();
            return ((double)((A * 256) + B)).ToString();
        }
        private static string case23(string data)
        {
            uint A = Convert.ToUInt32(data.Substring(0, 2), 16);
            uint B = Convert.ToUInt32(data.Substring(2), 16);
            if (US)
                return ((double)((A * 256) + B) * 10 * 0.145037738).ToString();
            return ((double)((A * 256) + B) * 10).ToString();
        }
        private static string case22(string data)
        {
            uint A = Convert.ToUInt32(data.Substring(0, 2), 16);
            uint B = Convert.ToUInt32(data.Substring(2), 16);
            if (US)
                return ((double)(((A * 256) + B) * 100 * 0.145037738) / 128.0).ToString();
            return ((double)((A * 256) + B) * 10 / 128.0).ToString();
        }
        private static string casecurr(string data)
        {
            uint C = Convert.ToUInt32(data.Substring(4, 2), 16);
            uint D = Convert.ToUInt32(data.Substring(6, 2), 16);
            
            return (((double)((C * 256) + D)/256.0) - 128.0).ToString();
        }
        private static string casevol(string data)
        {
            uint C = Convert.ToUInt32(data.Substring(4, 2), 16);
            uint D = Convert.ToUInt32(data.Substring(6, 2), 16);
            
            return ((double)((C * 256) + D)/8192.0).ToString();
        }
        private static string casebanksens(string data)
        {
            uint A = Convert.ToUInt32(data.Substring(0, 2), 16);

            return((double)(A * 0.005)).ToString();
        }
    }
}
