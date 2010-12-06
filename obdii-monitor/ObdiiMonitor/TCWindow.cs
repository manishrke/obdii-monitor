//-----------------------------------------------------------------------
// <copyright file="TCWindow.cs" company="University of Louisville">
//     Copyright (c) 2010 All Rights Reserved
// </copyright>
//-----------------------------------------------------------------------
namespace ObdiiMonitor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;

    /// <summary>
    /// Trouble Code Window form
    /// </summary>
    public partial class TCWindow : Form
    {
        /// <summary>
        /// Controller in MVC design pattern.
        /// </summary>
        private Controller controller;

        /// <summary>
        /// Contains the trouble codes
        /// </summary>
        private string codes = "0000";

        /// <summary>
        /// Initializes a new instance of the <see cref="TCWindow"/> class.
        /// </summary>
        public TCWindow()
        {
            InitializeComponent();
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormIsClosing);
            this.Resize += new System.EventHandler(this.Resizing);
            this.Rsize();
        }

        /// <summary>
        /// Sets the controller reference.
        /// </summary>
        /// <value>The controller reference.</value>
        internal Controller Controller
        {
            set { this.controller = value; }
        }

        /// <summary>
        /// Sets the data given a time in ms
        /// </summary>
        /// <param name="time">The time in ms.</param>
        /// <param name="data">The data to set.</param>
        public void Set_Data(uint time, string data)
        {
            string outdata = string.Empty;
            for (int i = 0; i < data.Length; i = i + 4)
            {
                if (0 == (i % 14))
                {
                    i = i + 2;
                }

                bool added = false;
                for (int j = 0; j < this.codes.Length; j = j + 4)
                {
                    if (data.Substring(i, 4) == this.codes.Substring(j, 4))
                    {
                        added = true;
                    }
                }

                if (!added)
                {
                    this.codes += data.Substring(i, 4);
                    bool goodData = true;

                    char[] values = { 'P', 'C', 'B', 'U' };
                    switch (data[i])
                    {
                        case '0':
                            outdata = "P0";
                            break;
                        case '1':
                            outdata = "P1";
                            break;
                        case '2':
                            outdata = "P2";
                            break;
                        case '3':
                            outdata = "P3";
                            break;
                        case '4':
                            outdata = "C0";
                            break;
                        case '5':
                            outdata = "C1";
                            break;
                        case '6':
                            outdata = "C2";
                            break;
                        case '7':
                            outdata = "C3";
                            break;
                        case '8':
                            outdata = "B0";
                            break;
                        case '9':
                            outdata = "B1";
                            break;
                        case 'A':
                            outdata = "B2";
                            break;
                        case 'B':
                            outdata = "B3";
                            break;
                        case 'C':
                            outdata = "U0";
                            break;
                        case 'D':
                            outdata = "U1";
                            break;
                        case 'E':
                            outdata = "U2";
                            break;
                        case 'F':
                            outdata = "U3";
                            break;
                        default:
                            goodData = false;
                            break;
                    }

                    if (goodData)
                    {
                        outdata += data.Substring(i + 1, 3);
                        string[] data2 = new string[2];
                        data2[0] = time.ToString();
                        data2[1] = outdata;
                        dataGridView1.Rows.Add(data2);
                    }
                }
            }
        }

        /// <summary>
        /// Enables the Get Trouble Codes and Reset Trouble Codes buttons.
        /// </summary>
        public void Enable()
        {
            btnget.Enabled = true;
            btnreset.Enabled = true;
        }

        /// <summary>
        /// Disables the Get Trouble Codes and Reset Trouble Codes buttons.
        /// </summary>
        public void Disable()
        {
            btnget.Enabled = false;
            btnreset.Enabled = false;
        }

        /// <summary>
        /// Handles the Click event of the Btnget control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Btnget_Click(object sender, EventArgs e)
        {
            this.controller.Serial.Flush();
            int loop = 0;

            this.controller.Serial.SendCommand("tc");

            byte[] response = this.controller.Serial.DataReceived();

            while (response == null)
            {
                response = this.controller.Serial.DataReceived();
                Thread.Sleep(300);
                if (loop > 30)
                {
                    this.controller.Serial.Flush();
                    this.controller.Serial.SendCommand("tc");
                    loop = 0;
                }

                loop++;
            }

            this.controller.LoadController.LoadData(response);
        }

        /// <summary>
        /// Handles the Click event of the Btnreset control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Btnreset_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show(
                "This will clear all codes and sensor data. Are you sure you want to Reset Trouble Codes?", 
                "Reset Trouble Codes",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question))
            {
                this.controller.Serial.SendCommand("tr");
                this.codes = "0000";
                dataGridView1.Rows.Clear();
            }
        }

        /// <summary>
        /// Called when the event of the trouble code window being resized occurs
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Resizing(object sender, EventArgs e)
        {
            this.Rsize();
        }

        /// <summary>
        /// Resizes this instance.
        /// </summary>
        private void Rsize()
        {
            pnl.Width = this.Width;
            pnl.Height = btnreset.Top - (btnget.Top + btnget.Height + 15);
            btnreset.Top = this.Height - (50 + btnreset.Height);
        }

        /// <summary>
        /// Called when the event of the trouble code window closing occurs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.FormClosingEventArgs"/> instance containing the event data.</param>
        private void FormIsClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
