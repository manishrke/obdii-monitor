using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace ObdiiMonitor
{
    public class SensorController
    {
        internal Thread polling, receiving;

        private Controller controller;

        Sensor[] sensors = {
                               new Sensor("Accelleration:", "AC", 3),
                               new Sensor( "Absolute Throttle Position:", "11", 4),
                               new Sensor( "Engine RPM:", "0C", 6),
                               new Sensor( "Vehicle Speed:", "0D", 4),
                               new Sensor( "Calculated Load Value:", "04", 4),
                               new Sensor( "Timing Advance (Cyl. #1):", "0E", 4),
                               new Sensor( "Intake Manifold Pressure:", "0B", 4),
                               new Sensor( "Air Flow Rate (MAF sensor):", "10", 6),
                           };

        public Sensor[] Sensors
        {
            get { return sensors; }
        }

        internal Controller Controller
        {
            set { controller = value; }
        }

        Sensor[] selectedSensors;

        public Sensor[] SelectedSensors
        {
            get { return selectedSensors; }
        }

        // initializes an array of the sensors that will either be polled or the results from prior polling
        // will be displayed in graphs
        public void initializeSelectedSensors(ArrayList numsSelected)
        {
            selectedSensors = new Sensor[numsSelected.Count];

            for (int i = 0; i < numsSelected.Count; ++i)
            {
                selectedSensors[i] = sensors[(int)numsSelected[i]];
            }
        }

        // the following function will both poll and receive data from the elm 327
        // it will timeout after 300 ms and send a new command
        private void beginPolling()
        {
            controller.SensorData.clearPollResponses();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            while (true)
            {
                for (int i = 0; i < selectedSensors.Length; ++i)
                {
                    controller.Serial.sendCommand("01" + selectedSensors[i].Pid + "1\r");
                    bool stopTrying = false, response = false;
                    DateTime time = DateTime.Now;
                    while (!stopTrying && !response)
                    {
                        try
                        {
                            string buffer = controller.Serial.dataReceived();
                            if (buffer.Length > 0 && buffer[0] == '>')
                            {
                                response = true;
                                controller.SensorData.parseData(buffer, (int)stopWatch.ElapsedMilliseconds);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (DateTime.Now.Subtract(time) > new TimeSpan(12000000))
                            {
                                stopTrying = true;
                            }
                        }
                    }
                }
            }
        }

        public void initializePollingReceivingThreads()
        {
            polling = new Thread(new ThreadStart(beginPolling));
           // receiving = new Thread(new ThreadStart(beginCollecting));

            polling.Name = "Polling";
            //receiving.Name = "Receiving";
            
            polling.Start();
          //  receiving.Start();
        }

        public void stopPollingReceiving()
        {
            if (polling != null)
            {
                polling.Abort();
            }

            if (receiving != null)
            {
                receiving.Abort();
            }
        }

        public int returnLength(string dataTag)
        {
            foreach (Sensor sensor in sensors)
            {
                if (sensor.Pid == dataTag)
                    return sensor.Bytes;
            }

            return -1;
        }
    }
}
