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
        private TCWindow tc;
        internal Controller Controller
        {
            set { controller = value; }
        }

        ArrayList pollResponses = new ArrayList();

        public ArrayList PollResponses
        {
            get { return pollResponses; }
        }

        public void parseData(string data, uint time)
        {
            Console.WriteLine(data);

            // remove whitespace
            data = data.Replace(" ", "");

            int length;

            string dataType, pollData;

            if (data.Contains(SENSOR_RESPONSE))
            {
                dataType = "OB";
                data = data.Substring(data.IndexOf(SENSOR_RESPONSE) + SENSOR_RESPONSE.Length);

                if (data.Length > 1)
                    length = controller.SensorController.returnLength(data.Substring(0, 2));
                else
                    return;

                pollData = data.Substring(0, length);

                PollResponse pollResponse = new PollResponse(controller, time, pollData, dataType, length);

                Console.WriteLine(pollResponse.ToString());

                pollResponses.Add(pollResponse);

                controller.MainWindow.GraphQueue.Enqueue(pollResponse);
            }
            else if (data.Contains(TC_RESPONSE))
            {
                dataType = "TC";
                data = data.Substring(data.IndexOf(SENSOR_RESPONSE) + SENSOR_RESPONSE.Length);

                if (data.Length > 1)
                    length = controller.SensorController.returnLength(data.Substring(0, 2));
                else
                    return;

                pollData = data.Substring(0, length);
               // .Set_Data(pollData);
 //               PollResponse pollResponse = new PollResponse(time, pollData, dataType, length);

 //               Console.WriteLine(pollResponse.ToString());

  //              pollResponses.Add(pollResponse);

  //              controller.MainWindow.GraphQueue.Enqueue(pollResponse);
            }
        }

        public void loadData(byte[] data)
        {
            try
            {
                PollResponse response = new PollResponse(controller, data);
                if (response.DataType == "GP")
                    controller.Gps.GpsList.Add(new GPSCoordinate(response));
                if (response.DataType == "TC")
                    controller.TcWindow.Set_Data(response.Time, response.Data);
                else
                {
                    pollResponses.Add(response);
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
