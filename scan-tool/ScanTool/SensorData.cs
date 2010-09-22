using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace ScanTool
{
    class SensorData
    {
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

        public void parseData(string data)
        {
            Console.WriteLine(data);

            // remove whitespace
            data = data.Replace(" ", "");

            string dataTag, pollData;

            int length;

            if (data.Contains("41"))
            {
                data = data.Substring(data.IndexOf("41") + 2);
                dataTag = data.Substring(0, 2);
                length = controller.SensorController.returnLength(dataTag);

                if (length == -1)
                    return;

                pollData = data.Substring(2, length * 2);

                PollResponse pollResponse = new PollResponse(DateTime.Now, pollData, dataTag, length);

                Console.WriteLine(pollResponse.ToString());

                pollResponses.Add(pollResponse);

                controller.MainWindow.GraphQueue.Enqueue(pollResponse);
            }
        }

        private void convertData(string data)
        {
            
        }
    }
}
