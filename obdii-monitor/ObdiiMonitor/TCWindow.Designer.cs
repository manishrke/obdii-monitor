namespace ObdiiMonitor
{
    partial class TCWindow
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnget = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnreset = new System.Windows.Forms.Button();
            this.Columntc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnget
            // 
            this.btnget.Location = new System.Drawing.Point(12, 12);
            this.btnget.Name = "btnget";
            this.btnget.Size = new System.Drawing.Size(129, 23);
            this.btnget.TabIndex = 0;
            this.btnget.Text = "Get Trouble Codes";
            this.btnget.UseVisualStyleBackColor = true;
            this.btnget.Click += new System.EventHandler(this.btnget_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Columntc});
            this.dataGridView1.Location = new System.Drawing.Point(12, 41);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(129, 96);
            this.dataGridView1.TabIndex = 1;
            // 
            // btnreset
            // 
            this.btnreset.Location = new System.Drawing.Point(12, 143);
            this.btnreset.Name = "btnreset";
            this.btnreset.Size = new System.Drawing.Size(129, 23);
            this.btnreset.TabIndex = 2;
            this.btnreset.Text = "Reset Trouble Codes";
            this.btnreset.UseVisualStyleBackColor = true;
            // 
            // Columntc
            // 
            this.Columntc.HeaderText = "Trouble Codes";
            this.Columntc.Name = "Columntc";
            this.Columntc.ReadOnly = true;
            // 
            // TCWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(398, 354);
            this.Controls.Add(this.btnreset);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.btnget);
            this.Name = "TCWindow";
            this.Text = "Trouble Code Menu";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnget;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnreset;
        private System.Windows.Forms.DataGridViewTextBoxColumn Columntc;
    }
}