using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ObdiiMonitor
{
    public partial class ConfigSave : Form
    {
        private Controller controller;
        private ArrayList sensornames;
        private ArrayList pids;
        private ArrayList configs;
        private int height = 0;
        private byte[] data = new byte[84];
        internal Controller Controller
        {
            set { controller = value; }
        }
        public ConfigSave()
        {
            InitializeComponent();
            sensornames = new ArrayList();
            pids = new ArrayList();
            configs = new ArrayList();
            this.Resize += new System.EventHandler(this.Resizing);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormIsClosing);

        }
        private void ConfigSave_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < this.controller.SensorController.Sensors.Length; i++)
            {
                    if (this.controller.SensorController.Sensors[i].Label2 == null)
                        sensornames.Add(this.controller.SensorController.Sensors[i].Label);
                    else
                        sensornames.Add(this.controller.SensorController.Sensors[i].Label + "," +
                                        this.controller.SensorController.Sensors[i].Label2.Substring(this.controller.SensorController.Sensors[i].Label2.LastIndexOf(':') + 1));
                    pids.Add(this.controller.SensorController.Sensors[i].Pid);
            }
            this.Width = this.controller.MainWindow.Width;
            addConfigPanel();
        }
        private void addConfigPanel()
        {

            ConfigPanel config = new ConfigPanel();
            string[] names = (string[])sensornames.ToArray(typeof(string));
            string[] pid = (string[])pids.ToArray(typeof(string));

            config.Popcbo(names, pid);
            config.Location = new Point(0, height);
            config.Width = this.Width;
            config.Click += new System.EventHandler(ConfigPanel_Click);
            this.pnlMain.Controls.Add(config);
            this.configs.Add(config);
            height += 25;

        }

        private void ConfigPanel_Click(object sender, EventArgs e)
        {
            int index;
            ConfigPanel current = (ConfigPanel)sender;
            current.Add_Click();
            if (current.Selected != "00" && !current.Remove)
            {
                index = pids.IndexOf((string)current.Selected);
                pids.RemoveAt(index);
                sensornames.RemoveAt(index);
                addConfigPanel();
            }
            else if (current.Remove)
            {
                pids.Add((string)current.Selected);
                sensornames.Add((string)current.name1);
                configs.Remove(current);
                this.pnlMain.Controls.Remove(current);
                height = 0;
                ConfigPanel[] list = (ConfigPanel[])configs.ToArray(typeof(ConfigPanel));
                for (int i = 0; i < list.Length; i++)
                {
                    list[i].Location = new Point(0, height);
                    height += 25;
                }
                string[] pid = (string[])pids.ToArray(typeof(string));
                list[list.Length - 1].Popcbo(current.name1, pid);
            }
        }

        private void FormIsClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
        private void Resizing(object sender, EventArgs e)
        {
            pnlMain.Width = this.Width;
            pnlMain.Height = this.Height;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            configs.RemoveAt(configs.Count - 1);
            saveConfig();
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = ".ini"; // Default file extension
            dlg.Filter = "Initialize File (*.ini)|*.ini"; // Filter files by extension
            dlg.FileName = "SENSORS.ini";
            dlg.Title = "Save Config (needs to be named SENSORS.ini on SD card)";
            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                if (File.Exists(dlg.FileName))
                {
                    File.Delete(dlg.FileName);
                }

                Stream output = File.OpenWrite(dlg.FileName);
                output.Write(data, 0, data.Length);
                output.Close();
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            height = 0;
            sensornames.Clear();
            pids.Clear();
            configs.Clear();
            pnlMain.Controls.Clear();
            for (int i = 0; i < this.controller.SensorController.Sensors.Length; i++)
            {
                if (this.controller.SensorController.Sensors[i].Label2 == null)
                    sensornames.Add(this.controller.SensorController.Sensors[i].Label);
                else
                    sensornames.Add(this.controller.SensorController.Sensors[i].Label + "," +
                                    this.controller.SensorController.Sensors[i].Label2.Substring(this.controller.SensorController.Sensors[i].Label2.LastIndexOf(':') + 1));
                pids.Add(this.controller.SensorController.Sensors[i].Pid);
            }

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".ini"; // Default file extension
            dlg.Filter = "Initialize File (*.ini)|*.ini"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                FileStream fileStream = new FileStream(dlg.FileName, FileMode.Open, FileAccess.Read);
            // while more to read
            if (data.Length == fileStream.Length)
                fileStream.Read(data,0,data.Length);
            else
                MessageBox.Show("This doesn't seem to be a valid config file.");

                fileStream.Close();
            }
            addConfigPanel();
            for (int i = 0; i < data.Length; i++)
            {
                addConfig(i);
            }
        }

        private void saveConfig()
        {
            int i = 0;
            ConfigPanel[] list = (ConfigPanel[])configs.ToArray(typeof(ConfigPanel));
            for (i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(data[i] & 131);
            }
            for (i = 0; i < list.Length; i++)
            {
                byte temptiming = (byte)((list[i].Timing << 2)+ 128);//128 sets the a in abbbbbcc high
                data[byte.Parse(list[i].Selected, System.Globalization.NumberStyles.HexNumber)] = (byte)(temptiming | data[byte.Parse(list[i].Selected, System.Globalization.NumberStyles.HexNumber)]);
            }
        }
        private void addConfig(int i)
        {
            string[] names = (string[])sensornames.ToArray(typeof(string));
            string[] pid = (string[])pids.ToArray(typeof(string));
            byte add = (byte)(data[i] >> 7);
            if (add == 1)
            {
                int j=0;
                while ( j<pid.Length && byte.Parse(pid[j], System.Globalization.NumberStyles.HexNumber) != (byte)i)
                {
                    j++;
                }
                if (j < pid.Length)
                {

                    ConfigPanel config = (ConfigPanel)configs[configs.Count-1];
                    config.cbobxName.SelectedIndex = j;
                    
                    byte z = (byte)(data[i] << 1);
                    z = (byte)(z >> 3);
                    config.txttimes.Text = z.ToString();
                    EventArgs e = new EventArgs();
                    ConfigPanel_Click(config, e);
                }
            }
        }

    }
}
