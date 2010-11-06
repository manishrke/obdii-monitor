using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObdiiMonitor
{
    class TimeOfDayConverter
    {
        private Controller controller;

        public Controller Controller
        {
            set { controller = value; }
        }

        private DateTime dateTime = new DateTime();

        public void setBaseTime(uint time, string data)
        {
            if (data.Length != 12)
                return;

            int hour, minutes, seconds, milliseconds;

            try
            {
                hour = int.Parse(data.Substring(0, 2));
                minutes = int.Parse(data.Substring(2, 2));
                seconds = int.Parse(data.Substring(4, 2));
                milliseconds = int.Parse(data.Substring(6, 3));
            }
            catch 
            {
                return;
            }

            dateTime = new DateTime(1900, 01, 01, hour, minutes, seconds, milliseconds);

            dateTime = dateTime.Subtract(new TimeSpan(0, 0, 0, 0, (int)time));
        }

        public string get(double time)
        {
            return dateTime.AddMilliseconds(time).ToString("HH:mm:ss.fff");
        }
    }
}
