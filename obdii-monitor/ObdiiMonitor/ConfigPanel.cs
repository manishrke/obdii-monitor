using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace ObdiiMonitor
{
    public partial class ConfigPanel : UserControl
    {
        public ConfigPanel()
        {
            InitializeComponent();
            
        }
        private string selected="00";
        public string Selected
        { get { return selected; } }
        private string name;
        public string name1
        { get { return name; } }
        private byte timing=99;
        public byte Timing
        { get { return timing; } }
        private bool remove=false;
        public bool Remove
        { get { return remove; } }
        private string[] pids;

        public void Popcbo(string[] names,string[] pids)
        {
            this.pids = pids;
            for (int i = 0; i < names.Length; i++)
            {
                cbobxName.Items.Add(names[i]);
            }
        }
        public void Popcbo(string names, string[] pids)
        {
            this.pids = pids;
            cbobxName.Items.Add(names);
        }

        public void Add_Click()
        {
            if (selected == "00")
            {
                string value = Regex.Replace(txttimes.Text, @"\D+", string.Empty);
                if (value.Length == 0)
                    MessageBox.Show("Enter a number." + value);
                else if (byte.Parse(value) > 31)
                    MessageBox.Show("Enter a valid number.");
                else
                {
                    if (cbobxName.SelectedIndex == -1)
                        MessageBox.Show("Select a Sensor");
                    else
                    {
                        name = (string) cbobxName.SelectedItem;
                        selected = pids[cbobxName.SelectedIndex];
                        timing = byte.Parse(value);
                        cbobxName.Enabled = false;
                        txttimes.Enabled = false;
                        lbloption.Text = "Click to the Right to Remove:";
                    }
                }


            }
            else
            {
                remove = true;
            }
        }
    }
}
