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
            int index = 0;

            while ((index < coordinates.Length) && (coordinates[index] != 'N') && (coordinates[index] != 'S'))
            {
                ++index;
            }

            if (index >= coordinates.Length)
            {
                return;
            }

            ++index;

            vertical = coordinates.Substring(0, index);

            int index2 = index;

            while ((index2 < coordinates.Length) && (coordinates[index2] != 'E') && (coordinates[index2] != 'W'))
            {
                ++index2;
            }

            if (index2 >= coordinates.Length)
            {
                return;
            }

            ++index2;

            horizontal = coordinates.Substring(index, index2 - index);

        this.time = time;
        }

        public GPSCoordinate(PollResponse response) : this (response.Time, response.Data)
        {
            
        }

        public override string ToString()
        {
            return horizontal + " " + vertical;
        }
    }
}
