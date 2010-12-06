using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace ObdiiMonitor
{
    public partial class TCWindow : Form
    {
        private Controller controller;
        private string Codes = "0000";
        
        internal Controller Controller
        {
            set { controller = value; }
        }

        public TCWindow()
        {
            InitializeComponent();
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormIsClosing);
            this.Resize += new System.EventHandler(this.Resizing);
            Rsize();
        }
        public void Set_Data(uint time, string data)
        {
            string outdata = "";
            for (int i = 0; i < data.Length; i = i + 4)
            {
                if (0 == (i % 14))
                    i = i + 2;
                bool added = false;
                for (int j = 0; j < Codes.Length; j = j + 4)
                {
                    if (data.Substring(i,4) == Codes.Substring(j,4))
                        added = true;
                }
                if (!added)
                {
                    Codes += data.Substring(i, 4);
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
        private void btnget_Click(object sender, EventArgs e)
        {
            controller.Serial.Flush();
            int loop = 0;

            this.controller.Serial.SendCommand("tc");

            byte[] response = controller.Serial.DataReceived();

            while (response == null)
            {
                response = controller.Serial.DataReceived();
                Thread.Sleep(300);
                if (loop > 30)
                {
                    this.controller.Serial.Flush();
                    this.controller.Serial.SendCommand("tc");
                    loop = 0;
                }
                loop++;
            }

            controller.LoadController.LoadData(response);
        }

        private void btnreset_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("This will clear all codes and sensor data. Are you sure you want to Reset Trouble Codes?",
                            "Reset Trouble Codes", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                this.controller.Serial.SendCommand("tr");
                Codes = "0000";
                dataGridView1.Rows.Clear();
            }
        }
        private void Resizing(object sender, EventArgs e)
        {
            Rsize();
        }
        private void Rsize()
        {
            pnl.Width = this.Width;
            pnl.Height = btnreset.Top - (btnget.Top + btnget.Height + 15);
            btnreset.Top = this.Height - (50 + btnreset.Height);
        }
        private void FormIsClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
        public void Enable()
        {
            btnget.Enabled = true;
            btnreset.Enabled = true;
        }
        public void Disable()
        {
            btnget.Enabled = false;
            btnreset.Enabled = false;
        }
    }
}
