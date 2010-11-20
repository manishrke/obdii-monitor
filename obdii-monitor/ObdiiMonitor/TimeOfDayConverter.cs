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

            int hour, minutes, seconds, month, day, year;

            try
            {
                hour = int.Parse(data.Substring(0, 2));
                minutes = int.Parse(data.Substring(2, 2));
                seconds = int.Parse(data.Substring(4, 2));
                month = int.Parse(data.Substring(6, 2));
                day = int.Parse(data.Substring(8, 2));
                year = int.Parse(data.Substring(10, 2));
            }
            catch 
            {
                return;
            }

            dateTime = new DateTime(year + 2000, month, day, hour, minutes, seconds);

            dateTime = dateTime.Subtract(new TimeSpan(0, 0, 0, 0, (int)time));
        }

        public string get(double time)
        {
            return dateTime.AddMilliseconds(time).ToString("MM/dd/yyyy HH:mm:ss.fff");
        }
    }
}
