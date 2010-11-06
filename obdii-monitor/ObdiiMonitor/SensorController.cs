﻿using System;
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
                           /*    new Sensor("Accelleration:", "AC", 3),
                               new Sensor( "Absolute Throttle Position:", "11", 4),
                               new Sensor( "Engine RPM:", "0C", 6),
                               new Sensor( "Vehicle Speed:", "0D", 4),
                               new Sensor( "Calculated Load Value:", "04", 4),
                               new Sensor( "Timing Advance (Cyl. #1):", "0E", 4),
                               new Sensor( "Intake Manifold Pressure:", "0B", 4),
                               new Sensor( "Air Flow Rate (MAF sensor):", "10", 6),*/
                               new Sensor( "Calculated Load value:", "04", 1, 0, 100, "%"),
                               new Sensor( "Engine Coolant Temp:",   "05", 1,-40,215, "°C"),
                               new Sensor( "Short term % trim Bank 1:", "06", 1, -100, 99.22, "%"),
                               new Sensor( "Long term % trim Bank 1:", "07", 1, -100, 99.22, "%"),
                               new Sensor( "Short term % trim Bank 2:", "08", 1, -100, 99.22, "%"),
                               new Sensor( "Long term % trim Bank 2:", "09", 1, -100, 99.22, "%"),
                               new Sensor( "Fuel Pressure:", "0A", 1 , 0, 765, "kPa (gauge)"),
                               new Sensor( "Intake Manifold Abs. Pressure:", "0B", 1, 0, 255, "kPa (absolute)"),
                               new Sensor( "Engine RPM:", "0C", 2, 0, 16383.75, "rpm"),
                               new Sensor( "Vehicle Speed:","0D", 1, 0, 255, "km/h"),
                               new Sensor( "Timing advance", "0E", 1, -64, 63.5, "° relative to #1 cylinder"),
                               new Sensor( "Intake air temperature", "0F", 1, -40, 215, "°C"),
                               new Sensor( "MAF air flow rate", "10", 2, 0, 655.35, "g/s"),
                               new Sensor( "Throttle position", "11", 1, 0, 100, "%"),
                               new Sensor( "Bank 1, Sensor 1: Oxygen sensor voltage", "14", 2, 0, 1.275, "Volts",
                                           "Bank 1, Sensor 1: Short term fuel trim,",-100,99.2,"%"),
                               new Sensor( "Bank 1, Sensor 2: Oxygen sensor voltage", "15", 2, 0, 1.275, "Volts",
                                           "Bank 1, Sensor 2: Short term fuel trim,",-100,99.2,"%"),
                               new Sensor( "Bank 1, Sensor 3: Oxygen sensor voltage", "16", 2, 0, 1.275, "Volts",
                                           "Bank 1, Sensor 3: Short term fuel trim,",-100,99.2,"%"),
                               new Sensor( "Bank 1, Sensor 4: Oxygen sensor voltage", "17", 2, 0, 1.275, "Volts",
                                           "Bank 1, Sensor 4: Short term fuel trim,",-100,99.2,"%"),
                               new Sensor( "Bank 2, Sensor 1: Oxygen sensor voltage", "18", 2, 0, 1.275, "Volts",
                                           "Bank 2, Sensor 1: Short term fuel trim,",-100,99.2,"%"),
                               new Sensor( "Bank 2, Sensor 2: Oxygen sensor voltage", "19", 2, 0, 1.275, "Volts",
                                           "Bank 2, Sensor 2: Short term fuel trim,",-100,99.2,"%"),
                               new Sensor( "Bank 2, Sensor 3: Oxygen sensor voltage", "1A", 2, 0, 1.275, "Volts",
                                           "Bank 2, Sensor 3: Short term fuel trim,",-100,99.2,"%"),
                               new Sensor( "Bank 2, Sensor 4: Oxygen sensor voltage", "1B", 2, 0, 1.275, "Volts",
                                           "Bank 2, Sensor 4: Short term fuel trim,",-100,99.2,"%"),
                               new Sensor( "Run time since engine start", "1F", 2, 0, 65535, "seconds"),
                               new Sensor( "Distance traveled with malfunction indicator lamp (MIL) on", "21", 2, 0, 65535, "km"),
                               new Sensor( "Fuel Rail Pressure (relative to manifold vacuum)", "22", 2, 0, 5177.265, "kPa"),
                               new Sensor( "Fuel Rail Pressure (diesel)", "23", 2, 0, 655350, "kPa (gauge)"),
                               new Sensor( "O2S1_WR_lambda(1): Equivalence Ratio", "24", 4, 0, 2, "",
                                           "O2S1_WR_lambda(1): Voltage", 0, 8, "V"),
                               new Sensor( "O2S2_WR_lambda(1): Equivalence Ratio", "25", 4, 0, 2, "",
                                           "O2S2_WR_lambda(1): Voltage", 0, 8, "V"),
                               new Sensor( "O2S3_WR_lambda(1): Equivalence Ratio", "26", 4, 0, 2, "",
                                           "O2S3_WR_lambda(1): Voltage", 0, 8, "V"),
                               new Sensor( "O2S4_WR_lambda(1): Equivalence Ratio", "27", 4, 0, 2, "",
                                           "O2S4_WR_lambda(1): Voltage", 0, 8, "V"),
                               new Sensor( "O2S5_WR_lambda(1): Equivalence Ratio", "28", 4, 0, 2, "",
                                           "O2S5_WR_lambda(1): Voltage", 0, 8, "V"),
                               new Sensor( "O2S6_WR_lambda(1): Equivalence Ratio", "29", 4, 0, 2, "",
                                           "O2S6_WR_lambda(1): Voltage", 0, 8, "V"),
                               new Sensor( "O2S7_WR_lambda(1): Equivalence Ratio", "2A", 4, 0, 2, "",
                                           "O2S7_WR_lambda(1): Voltage", 0, 8, "V"),
                               new Sensor( "O2S8_WR_lambda(1): Equivalence Ratio", "2B", 4, 0, 2, "",
                                           "O2S8_WR_lambda(1): Voltage", 0, 8, "V"),
                               new Sensor( "Commanded EGR", "2C", 1, 0, 100, "%"),
                               new Sensor( "EGR Error", "2D", 1, -100, 99.22, "%"),
                               new Sensor( "Commanded evaporative purge", "2E", 1, 0, 100, "%"),
                               new Sensor( "Fuel Level Input", "2F", 1, 0, 100, "%"),
                               new Sensor( "# of warm-ups since codes cleared", "30", 1, 0, 255, ""),
                               new Sensor( "Distance traveled since codes cleared", "31", 2, 0, 65535, "km"),
                               new Sensor( "Evap. System Vapor Pressure", "32", 2, -8192, 8192, "Pa"),
                               new Sensor( "Barometric pressure", "33", 1, 0, 255, "kPa (Absolute)"),
                               new Sensor( "O2S1_WR_lambda(1): Equivalence Ratio", "34", 4, 0, 2, "",
                                           "O2S1_WR_lambda(1): Current", -128, 128 ,"mA"),
                               new Sensor( "O2S2_WR_lambda(1): Equivalence Ratio", "35", 4, 0, 2, "",
                                           "O2S2_WR_lambda(1): Current", -128, 128 ,"mA"),
                               new Sensor( "O2S3_WR_lambda(1): Equivalence Ratio", "36", 4, 0, 2, "",
                                           "O2S3_WR_lambda(1): Current", -128, 128 ,"mA"),
                               new Sensor( "O2S4_WR_lambda(1): Equivalence Ratio", "37", 4, 0, 2, "",
                                           "O2S4_WR_lambda(1): Current", -128, 128 ,"mA"),
                               new Sensor( "O2S5_WR_lambda(1): Equivalence Ratio", "38", 4, 0, 2, "",
                                           "O2S5_WR_lambda(1): Current", -128, 128 ,"mA"),
                               new Sensor( "O2S6_WR_lambda(1): Equivalence Ratio", "39", 4, 0, 2, "",
                                           "O2S6_WR_lambda(1): Current", -128, 128 ,"mA"),
                               new Sensor( "O2S7_WR_lambda(1): Equivalence Ratio", "3A", 4, 0, 2, "",
                                           "O2S7_WR_lambda(1): Current", -128, 128 ,"mA"),
                               new Sensor( "O2S8_WR_lambda(1): Equivalence Ratio", "3B", 4, 0, 2, "",
                                           "O2S8_WR_lambda(1): Current", -128, 128 ,"mA"),
                               new Sensor( "Catalyst Temperature Bank 1, Sensor 1", "3C", 2, -40, 6513.50, "°C"),
                               new Sensor( "Catalyst Temperature Bank 2, Sensor 1", "3D", 2, -40, 6513.50, "°C"),
                               new Sensor( "Catalyst Temperature Bank 1, Sensor 2", "3E", 2, -40, 6513.50, "°C"),
                               new Sensor( "Catalyst Temperature Bank 2, Sensor 2", "3F", 2, -40, 6513.50, "°C"),
                               new Sensor( "Control module voltage", "42", 2, 0, 65.535, "V"),
                               new Sensor( "Absolute load value", "43", 2, 0, 25700, "%"),
                               new Sensor( "Command equivalence ratio", "44", 2, 0, 2, ""),
                               new Sensor( "Relative throttle position", "45", 1, 0, 100, "%"),
                               new Sensor( "Ambient air temperature", "46", 1, -40, 215, "°C"),
                               new Sensor( "Absolute throttle position B", "47", 1, 0, 100, "%"),
                               new Sensor( "Absolute throttle position C", "48", 1, 0, 100, "%"),
                               new Sensor( "Accelerator pedal position D", "49", 1, 0, 100, "%"),
                               new Sensor( "Accelerator pedal position E", "4A", 1, 0, 100, "%"),
                               new Sensor( "Accelerator pedal position F", "4B", 1, 0, 100, "%"),
                               new Sensor( "Commanded throttle actuator", "4C", 1, 0, 100, "%"),
                               new Sensor( "Time run with MIL on", "4D", 2, 0, 65535, "minutes"),
                               new Sensor( "Time since trouble codes cleared", "4E", 2, 0, 65535, "minutes"),
                               new Sensor( "Ethanol fuel %", "52", 1, 0, 100, "%"),
                               new Sensor( "Absoulute Evap system Vapour Pressure", "53", 2, 0, 327675, "kpa"),                              
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
                                controller.SensorData.parseData(buffer, (uint)stopWatch.ElapsedMilliseconds);
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
