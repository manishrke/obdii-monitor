using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace ObdiiMonitor
{
    class LoadController
    {
        private Controller controller;

        internal Controller Controller
        {
            set { controller = value; }
        }

        // the following function will attempt to open fileName 
        // and attempt to find sections of the stream that start with with the chosen start tag
        // if so it supsequently looks for a single byte length and copies the bytes from the beginning
        // of the start tag to the the length plus nine bytes that will always be there
        // it then sends that data to be converted into a PollResponse object
        internal void loadData(string fileName)
        {
            controller.SensorData.clearPollResponses();

            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            while (fileStream.Position < fileStream.Length)
            {

                while ((char)fileStream.ReadByte() != PollResponse.startTag[0])
                    ;

                if (fileStream.Position >= fileStream.Length)
                    break;

                long start = fileStream.Position - 1;

                if ((char)fileStream.ReadByte() != PollResponse.startTag[1])
                    continue;

                int length = fileStream.ReadByte();

                byte[] data = new byte[length + 9];

                fileStream.Position = start;

                fileStream.Read(data, 0, length + 9);

                controller.SensorData.loadData(data);
            }

            createGraphs(controller.SensorData.PollResponses);
        }

        private void createGraphs(ArrayList pollResponses)
        {
            HashSet<string> set = new HashSet<string>();
            ArrayList numsSelected = new ArrayList();

            // get a set of all the pid's in pollResponses that should correspond to a table in SensorController
            foreach (PollResponse response in pollResponses)
            {
                if (response.DataType == "OB") 
                {
                    if (response.Data.Length > 1)
                    {
                        set.Add(response.Data.Substring(0, 2));
                    }
                }    
            }

            // now compile a list of the index of the pids from the sensors table in SensorController 
            foreach (string str in set)
            {
                for (int i=0; i < controller.SensorController.Sensors.Length; ++i)
                {
                    if (controller.SensorController.Sensors[i].Pid == str)
                    {
                        numsSelected.Add(i);
                    }
                }
            }

            controller.SensorController.initializeSelectedSensors(numsSelected);

            controller.MainWindow.showSetupGraphWindow(numsSelected);

            controller.MainWindow.startGraphPlotThread();
        }
    }
}
