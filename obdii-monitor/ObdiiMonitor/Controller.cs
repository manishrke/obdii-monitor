using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScanTool
{
    class Controller
    {
        private MainWindow mainWindow;

        public MainWindow MainWindow
        {
            get { return mainWindow; }
            set { mainWindow = value; }
        }

        private SensorController sensorController = new SensorController();

        public SensorController SensorController
        {
            get { return sensorController; }
        }

        private Serial serial = new Serial();

        public Serial Serial
        {
            get { return serial; }
        }

        private SensorData sensorData = new SensorData();

        public SensorData SensorData
        {
            get { return sensorData; }
        }

        public Controller()
        {
            sensorController.Controller = this;
            serial.Controller = this;
            sensorData.Controller = this;
        }
    }
}
