using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace ObdiiMonitor
{
    public partial class TCWindow : Form
    {
        private Controller controller;
        private string Codes = "\0\0";
        
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
        public void Set_Data(uint time, byte[] data)
        {
            string outdata = "";
            for (int i = 0; i < data.Length; i = i + 2)
            {
                bool added = false;
                for (int j = 0; j < Codes.Length; j = j + 2)
                {
                    if (((char)data[i] == Codes[j]) && ((char)data[i+1] == Codes[j+1]))
                        added = true;
                }
                if (!added)
                {
                    Codes += (char)data[i];
                    Codes += (char)data[i+1];
                    char[] values = { 'P', 'C', 'B', 'U' };
                    outdata = values[(data[i] >> 4) / 4]+ ((data[i] >> 4) % 4).ToString()
                    + (data[i] % 16).ToString() + (data[i+1] >> 4).ToString() + (data[i+1] % 16).ToString();

                    string[] data2 = new string[2];
                    data2[0] = time.ToString();
                    data2[1] = outdata;
                    dataGridView1.Rows.Add(data2);
                }
            }
        }
        private void btnget_Click(object sender, EventArgs e)
        {
            this.controller.Serial.sendCommand("tc");
        }

        private void btnreset_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("This will clear all codes and sensor data. Are you sure you want to Reset Trouble Codes?",
                            "Reset Trouble Codes", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                this.controller.Serial.sendCommand("tr");
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
    }
}
