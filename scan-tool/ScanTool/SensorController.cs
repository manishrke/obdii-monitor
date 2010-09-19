using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;

namespace ScanTool
{
    class SensorController
    {
        Thread polling, receiving;

        Mutex mutex = new Mutex();

        private Controller controller;

        internal Controller Controller
        {
            set { controller = value; }
        }

        Sensor[] sensors = {
                               new Sensor( "Absolute Throttle Position:", "11", 1),
                               new Sensor( "Engine RPM:", "0C", 2),
                               new Sensor( "Vehicle Speed:", "0D", 1),
                               new Sensor( "Calculated Load Value:", "04", 1),
                               new Sensor( "Timing Advance (Cyl. #1):", "0E", 1),
                               new Sensor( "Intake Manifold Pressure:", "0B", 1),
                               new Sensor( "Air Flow Rate (MAF sensor):", "10", 2),
                               new Sensor( "Fuel System 1 Status:", "03", 2),
                               new Sensor( "Fuel System 2 Status:", "03", 2)
                           };

        public Sensor[] Sensors
        {
            get { return sensors; }
        }

        Sensor[] selectedSensors;

        public Sensor[] SelectedSensors
        {
            get { return selectedSensors; }
        }

        public void initializeSelectedSensors(ArrayList numsSelected)
        {
            selectedSensors = new Sensor[numsSelected.Count];

            for (int i = 0; i < numsSelected.Count; ++i)
            {
                selectedSensors[i] = sensors[(int)numsSelected[i]];
            }
        }

        private void beginPolling()
        {
            while (true)
            {
                for (int i = 0; i < selectedSensors.Length; ++i)
                {
                    controller.Serial.sendCommand("01" + selectedSensors[i].Pid + "\r");
                    Thread.Sleep(250);
                }
            }
        }

        private void beginCollecting()
        {
            while (true)
            {
                    try
                    {
                        string buffer = controller.Serial.dataReceived();
                        controller.SensorData.parseData(buffer);
                    }
                    catch (TimeoutException ex)
                    {

                    }
            }
        }

        public void initializePollingReceivingThreads()
        {
            polling = new Thread(new ThreadStart(beginPolling));
            receiving = new Thread(new ThreadStart(beginCollecting));

            polling.Name = "Polling";
            receiving.Name = "Receiving";
            
            polling.Start();
            receiving.Start();
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


    }
}
