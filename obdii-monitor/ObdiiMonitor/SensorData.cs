using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace ObdiiMonitor
{
    public class SensorData
    {
        private static string SENSOR_RESPONSE = "41";
        private static string TC_RESPONSE = "43";
        private Controller controller;

        internal Controller Controller
        {
            set { controller = value; }
        }

        ArrayList pollResponses = new ArrayList();

        public ArrayList PollResponses
        {
            get { return pollResponses; }
        }


        public void loadData(byte[] data)
        {
            try
            {
                PollResponse response = new PollResponse(controller, data);
                pollResponses.Add(response);
                if (response.DataType == "GP")
                    controller.Gps.GpsList.Add(new GPSCoordinate(response));
                else if (response.DataType == "TC")
                    controller.TcWindow.Set_Data(response.Time, response.Data);
                else if (response.DataType == "GT")
                    controller.TimeOfDayConverter.setBaseTime(response.Time, response.Data);
                else if (response.DataType == "CF")
                {
                    this.controller.Config = response.Data2;
                    this.controller.MainWindow.PopulateSelectionWindow();
                }
                else
                {
                    controller.MainWindow.GraphQueue.Enqueue(response);
                }
                Console.WriteLine(response);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return;
            }
        }

        internal void clearPollResponses()
        {
            if (pollResponses != null)
                pollResponses.Clear();
            else
                pollResponses = new ArrayList();
        }
    }
}
