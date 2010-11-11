namespace ObdiiMonitor
{
    partial class ConfigPanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cbobxName = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txttimes = new System.Windows.Forms.TextBox();
            this.lbloption = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cbobxName
            // 
            this.cbobxName.FormattingEnabled = true;
            this.cbobxName.Location = new System.Drawing.Point(100, 2);
            this.cbobxName.Name = "cbobxName";
            this.cbobxName.Size = new System.Drawing.Size(327, 21);
            this.cbobxName.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Choose a Sensor:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(426, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Pull once every";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(543, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "times. (0-31)";
            // 
            // txttimes
            // 
            this.txttimes.Location = new System.Drawing.Point(512, 2);
            this.txttimes.Name = "txttimes";
            this.txttimes.Size = new System.Drawing.Size(25, 20);
            this.txttimes.TabIndex = 4;
            this.txttimes.Text = "1";
            // 
            // lbloption
            // 
            this.lbloption.AutoSize = true;
            this.lbloption.Location = new System.Drawing.Point(613, 6);
            this.lbloption.Name = "lbloption";
            this.lbloption.Size = new System.Drawing.Size(143, 13);
            this.lbloption.TabIndex = 6;
            this.lbloption.Text = "Click to the Right to Add      :";
            // 
            // ConfigPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.lbloption);
            this.Controls.Add(this.txttimes);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbobxName);
            this.MinimumSize = new System.Drawing.Size(759, 23);
            this.Name = "ConfigPanel";
            this.Size = new System.Drawing.Size(759, 23);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbloption;
        public System.Windows.Forms.ComboBox cbobxName;
        public System.Windows.Forms.TextBox txttimes;
    }
}
