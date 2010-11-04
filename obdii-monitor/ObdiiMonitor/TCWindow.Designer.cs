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
            this.pnl = new System.Windows.Forms.Panel();
            this.Time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Columntc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.pnl.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnget
            // 
            this.btnget.Location = new System.Drawing.Point(0, 12);
            this.btnget.Name = "btnget";
            this.btnget.Size = new System.Drawing.Size(204, 23);
            this.btnget.TabIndex = 0;
            this.btnget.Text = "Get Trouble Codes";
            this.btnget.UseVisualStyleBackColor = true;
            this.btnget.Click += new System.EventHandler(this.btnget_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Time,
            this.Columntc});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidth = 4;
            this.dataGridView1.Size = new System.Drawing.Size(247, 272);
            this.dataGridView1.TabIndex = 1;
            // 
            // btnreset
            // 
            this.btnreset.Location = new System.Drawing.Point(0, 319);
            this.btnreset.Name = "btnreset";
            this.btnreset.Size = new System.Drawing.Size(204, 23);
            this.btnreset.TabIndex = 2;
            this.btnreset.Text = "Reset Trouble Codes";
            this.btnreset.UseVisualStyleBackColor = true;
            this.btnreset.Click += new System.EventHandler(this.btnreset_Click);
            // 
            // pnl
            // 
            this.pnl.AutoSize = true;
            this.pnl.Controls.Add(this.dataGridView1);
            this.pnl.Location = new System.Drawing.Point(0, 41);
            this.pnl.Name = "pnl";
            this.pnl.Size = new System.Drawing.Size(247, 272);
            this.pnl.TabIndex = 3;
            // 
            // Time
            // 
            this.Time.HeaderText = "Time";
            this.Time.Name = "Time";
            this.Time.ReadOnly = true;
            this.Time.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // Columntc
            // 
            this.Columntc.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Columntc.HeaderText = "Trouble Codes";
            this.Columntc.Name = "Columntc";
            this.Columntc.ReadOnly = true;
            this.Columntc.Width = 101;
            // 
            // TCWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(203, 354);
            this.Controls.Add(this.pnl);
            this.Controls.Add(this.btnreset);
            this.Controls.Add(this.btnget);
            this.MinimumSize = new System.Drawing.Size(200, 200);
            this.Name = "TCWindow";
            this.Text = "Trouble Code Menu";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.pnl.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnget;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnreset;
        private System.Windows.Forms.Panel pnl;
        private System.Windows.Forms.DataGridViewTextBoxColumn Time;
        private System.Windows.Forms.DataGridViewTextBoxColumn Columntc;
    }
}