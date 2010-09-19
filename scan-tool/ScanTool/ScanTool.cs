using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace ScanTool
{
    public partial class ScanTool : Form
    {
        public ScanTool()
        {
            InitializeComponent();
        }

        private void buttonInitialize_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                    labelStatus.Text = "Com port was open, now closed.  Try Again.";
                }
                else
                {
                    serialPort.BaudRate = int.Parse(comboBoxBaudRate.Text);
                    serialPort.PortName = comboBoxComPort.Text;
                    serialPort.DataBits = 8;
                    serialPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), "1");
                    serialPort.Parity = (Parity)Enum.Parse(typeof(Parity), "0");
                    serialPort.ReadTimeout = 100;
                    serialPort.Open();
                    serialPort.WriteLine("AT Z\r");
                    labelStatus.Text = "Com port now open.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                labelStatus.Text = "An error occured.  Try Again.";
            }
        }

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = serialPort.ReadExisting();
                data = data.Replace("\r", "\r\n");
                textBoxResults.Text += data;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonTransmit_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort.WriteLine(textBoxTransmit.Text + "\r");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ScanTool_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    serialPort.ReadExisting();
                    serialPort.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ScanTool_Load(object sender, EventArgs e)
        {
            comboBoxBaudRate.SelectedIndex = 1;
            comboBoxComPort.SelectedIndex = 3;
            comboBoxMeasurement.SelectedIndex = 1;
        }

    }
}
