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
        // of the start tag to the the length plus a defined  length of bytes that will always be there
        // it then sends that data to be converted into a PollResponse object
        internal void loadData(string fileName)
        {
            // clear the pollResponses ArrayList member in SensorData to begin loading in new data
            controller.SensorData.clearPollResponses();
            // Clear the GraphQueue, not sure if the GraphQueue will stick around.
            controller.MainWindow.GraphQueue.Clear();

            // open the FileStream of the fileName
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            // while more to read
            while (fileStream.Position < fileStream.Length)
            {
                // read until it finds the start tag
                while ((fileStream.Position < fileStream.Length)&&((char)fileStream.ReadByte() != PollResponse.StartTag))
                    ;

                // if it's the end of the file break from while l0op
                if (fileStream.Position >= fileStream.Length)
                    break;

                // record the start position of the start tag, this may be changed to index the position right after the start tag
                long start = fileStream.Position - 1;

                // record a single byte integer from the file stream which should be the length of the data portion
                int length = fileStream.ReadByte();

                // initialize a byte array that should hold all the data required to create a PollResponse
                byte[] data = new byte[length + PollResponse.StartTagLength];

                fileStream.Position = start;

                // copy the data starting from the start tag to a length of the length byte plus a defined constant number of bytes
                fileStream.Read(data, 0, length + PollResponse.StartTagLength);

                // send what should be a single poll response to the SensorData.loadData function that will construct a PollResponse object
                // and store them in order in the pollRespnses member, any error checking of a bad data parameter will occur there
                controller.SensorData.loadData(data);
            }

            // now that the data has been loaded into the software
            // create a series of graphs to represent the data
            createGraphs(controller.SensorData.PollResponses);

            // load the data into the graphs just created
            controller.MainWindow.loadDataIntoSensorGraphs();

            // now that the data has all been loaded, show the panel
            controller.MainWindow.showSensorDataPanel();
            // show the Reset text on the action button of the window
            controller.MainWindow.showResetButton();
        }

        private void createGraphs(ArrayList pollResponses)
        {
            HashSet<string> set = new HashSet<string>();
            ArrayList numsSelected = new ArrayList();

            // get a set of all the pid's in pollResponses that should correspond to a table in SensorController
            foreach (PollResponse response in pollResponses)
            {
                // if the data type is a sensor response from the elm327
                if (response.DataType == "OB") 
                {
                    // Add the first two characters of data that should represent the sensor type to a set of types
                    // to generate a graph, one for each sensor type
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

            //initialize the selectedSensors member of SensorController
            controller.SensorController.initializeSelectedSensors(numsSelected);
            // create the initial graphs based off the sensors in selectedSensor member of SensorController
            controller.MainWindow.populateGraphWindow(numsSelected);
        }
    }
}
