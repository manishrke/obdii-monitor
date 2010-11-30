using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObdiiMonitor
{
    class GPSCoordinate
    {
        private string horizontal;

        public string Horizontal
        {
            get { return horizontal; }
        }

        private string vertical;

        public string Vertical
        {
            get { return vertical; }
        }

        private uint time;

        public uint Time
        {
            get { return time; }
        }

        public GPSCoordinate(uint time, string horizontal, string vertical)
        {
            this.time = time;
            this.horizontal = horizontal;
            this.vertical = vertical;
        }

        public GPSCoordinate(uint time, string coordinates)
        {

            vertical = Convert.ToString(Math.Round(Convert.ToDouble(coordinates.Substring(0, 2)) + Convert.ToDouble(coordinates.Substring(2, 7)) / 60.0, 6)) + coordinates.Substring(9, 1);

            horizontal = Convert.ToString(Math.Round(Convert.ToDouble(coordinates.Substring(10, 3)) + Convert.ToDouble(coordinates.Substring(13, 7)) / 60.0, 6)) + coordinates.Substring(20, 1);
            
            this.time = time;
        }

        public GPSCoordinate(PollResponse response) : this (response.Time, response.Data)
        {
            
        }

        public override string ToString()
        {
            return vertical + " " + horizontal;
        }
    }
}
