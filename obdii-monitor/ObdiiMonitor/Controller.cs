//-----------------------------------------------------------------------
// <copyright file="Controller.cs" company="University of Louisville">
//     Copyright (c) 2010 All Rights Reserved
// </copyright>
//-----------------------------------------------------------------------
namespace ObdiiMonitor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;

    /// <summary>
    /// ModelViewController design pattern's "Controller" class
    /// </summary>
    public class Controller
    {
        /// <summary>
        /// References MainWindow class.
        /// </summary>
        private MainWindow mainWindow;

        /// <summary>
        /// References TCWindow class.
        /// </summary>
        private TCWindow tcWindow = new TCWindow();

        /// <summary>
        /// References ConfigSave class.
        /// </summary>
        private ConfigSave configSave = new ConfigSave();

        /// <summary>
        /// References SensorController class.
        /// </summary>
        private SensorController sensorController = new SensorController();

        /// <summary>
        /// References Serial class.
        /// </summary>
        private Serial serial = new Serial();

        /// <summary>
        /// References SensorData class.
        /// </summary>
        private SensorData sensorData = new SensorData();

        /// <summary>
        /// True if program is using US units, false if using Metric
        /// </summary>
        private bool us = false;

        /// <summary>
        /// Configuration file data stored on the microcontroller's SD card.
        /// </summary>
        private byte[] config = new byte[84];

        /// <summary>
        /// References Load Controller class.
        /// </summary>
        private LoadController loadController = new LoadController();

        /// <summary>
        /// References SaveController class.
        /// </summary>
        private SaveController saveController = new SaveController();

        /// <summary>
        /// References AccelerometerConverter class.
        /// </summary>
        private AccelerometerConverter accelerometerConverter = new AccelerometerConverter();

        /// <summary>
        /// References TimeOfDayConverter class.
        /// </summary>
        private TimeOfDayConverter timeOfDayConverter = new TimeOfDayConverter();

        /// <summary>
        /// References GPS class.
        /// </summary>
        private GPS gps = new GPS();

        /// <summary>
        /// Initializes a new instance of the <see cref="Controller"/> class.
        /// </summary>
        public Controller()
        {
            this.sensorController.Controller = this;
            this.loadController.Controller = this;
            this.saveController.Controller = this;
            this.serial.Controller = this;
            this.sensorData.Controller = this;
            this.accelerometerConverter.Controller = this;
            this.gps.Controller = this;
            this.tcWindow.Controller = this;
            this.timeOfDayConverter.Controller = this;
            this.configSave.Controller = this;
        }

        /// <summary>
        /// Gets or sets the main window class reference.
        /// </summary>
        /// <value>The main window class reference.</value>
        public MainWindow MainWindow
        {
            get { return this.mainWindow; }
            set { this.mainWindow = value; }
        }

        /// <summary>
        /// Gets the trouble code window.
        /// </summary>
        /// <value>The trouble code window.</value>
        public TCWindow TcWindow
        {
            get { return this.tcWindow; }
        }

        /// <summary>
        /// Gets the save confguration reference.
        /// </summary>
        /// <value>The save configuration reference.</value>
        public ConfigSave ConfigSave
        {
            get { return this.configSave; }
        }

        /// <summary>
        /// Gets the sensor controller reference.
        /// </summary>
        /// <value>The sensor controller reference.</value>
        public SensorController SensorController
        {
            get { return this.sensorController; }
        }

        /// <summary>
        /// Gets the serial reference.
        /// </summary>
        /// <value>The serial reference.</value>
        public Serial Serial
        {
            get { return this.serial; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether US units are used.
        /// </summary>
        /// <value><c>true</c> if US units; otherwise (Metric), <c>false</c>.</value>
        public bool US
        {
            get { return this.us; }
            set { this.us = value; }
        }

        /// <summary>
        /// Gets the sensor data reference.
        /// </summary>
        /// <value>The sensor data reference.</value>
        public SensorData SensorData
        {
            get { return this.sensorData; }
        }

        /// <summary>
        /// Gets or sets the config reference.
        /// </summary>
        /// <value>The config reference.</value>
        public byte[] Config
        {
            get { return this.config; }
            set { this.config = value; }
        }

        /// <summary>
        /// Gets the load controller reference.
        /// </summary>
        /// <value>The load controller reference.</value>
        internal LoadController LoadController
        {
            get { return this.loadController; }
        }

        /// <summary>
        /// Gets the save controller reference.
        /// </summary>
        /// <value>The save controller reference.</value>
        internal SaveController SaveController
        {
            get { return this.saveController; }
        }

        /// <summary>
        /// Gets the accelerometer converter reference.
        /// </summary>
        /// <value>The accelerometer converter reference.</value>
        internal AccelerometerConverter AccelerometerConverter
        {
            get { return this.accelerometerConverter; }
        }

        /// <summary>
        /// Gets the time of day converter class.
        /// </summary>
        /// <value>The time of day converter class.</value>
        internal TimeOfDayConverter TimeOfDayConverter
        {
            get { return this.timeOfDayConverter; }
        }

        /// <summary>
        /// Gets the GPS reference.
        /// </summary>
        /// <value>The GPS reference.</value>
        internal GPS Gps
        {
            get { return this.gps; }
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            // reset the calibration reading for the accelerometer as this is a new session.
            this.accelerometerConverter.ResetCalibrationReading();

            // clear the pollResponses ArrayList member in SensorData to begin loading in new data
            this.sensorData.clearPollResponses();

            // Clear the GraphQueue, not sure if the GraphQueue will stick around.
            this.mainWindow.GraphQueue.Clear();

            // clear the gps linked list
            this.gps.GpsList.Clear();

            // cancel all the threads
            this.CancelAllThreads();
        }

        /// <summary>
        /// Cancels all threads.
        /// </summary>
        public void CancelAllThreads()
        {
            if (MainWindow.UpdateGraphPlots != null)
            {
                MainWindow.UpdateGraphPlots.Abort();
            }

            if (this.sensorController.receiving != null)
            {
                this.sensorController.receiving.Abort();
            }
        }
    }
 }
