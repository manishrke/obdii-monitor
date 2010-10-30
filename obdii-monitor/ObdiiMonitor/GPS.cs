using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace ObdiiMonitor
{
    class GPS
    {
        private Controller controller;

        public Controller Controller
        {
            set { controller = value; }
        }

        private ArrayList gpsList = new ArrayList();

        public ArrayList GpsList
        {
            get { return gpsList; }
        }

        public GPSCoordinate get(uint time)
        {
            int length = gpsList.Count;

            for (int i = 0; i < length; ++i)
            {
                GPSCoordinate coord = (GPSCoordinate)gpsList[i];
                if (coord.Time < time)
                    continue;

                return coord;
            }

            return null;
        }
    }
}
