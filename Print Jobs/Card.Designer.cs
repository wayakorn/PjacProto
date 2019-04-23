namespace Print_Jobs
{
    partial class Card
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
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBoxDocumentName = new System.Windows.Forms.TextBox();
            this.textBoxPrinterName = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.textBoxStatus = new System.Windows.Forms.TextBox();
            this.textBoxAnts = new System.Windows.Forms.TextBox();
            this.buttonAbort = new System.Windows.Forms.Button();
            this.timerAnts = new System.Windows.Forms.Timer(this.components);
            this.pictureBoxIcon = new System.Windows.Forms.PictureBox();
            this.textBoxSubmitTime = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.textBoxDocumentName);
            this.panel1.Controls.Add(this.textBoxPrinterName);
            this.panel1.Location = new System.Drawing.Point(61, 6);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(194, 50);
            this.panel1.TabIndex = 3;
            // 
            // textBoxDocumentName
            // 
            this.textBoxDocumentName.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxDocumentName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxDocumentName.Cursor = System.Windows.Forms.Cursors.Default;
            this.textBoxDocumentName.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxDocumentName.ForeColor = System.Drawing.Color.Black;
            this.textBoxDocumentName.Location = new System.Drawing.Point(3, 4);
            this.textBoxDocumentName.Name = "textBoxDocumentName";
            this.textBoxDocumentName.Size = new System.Drawing.Size(161, 30);
            this.textBoxDocumentName.TabIndex = 7;
            this.textBoxDocumentName.Text = ".....";
            // 
            // textBoxPrinterName
            // 
            this.textBoxPrinterName.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxPrinterName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxPrinterName.Cursor = System.Windows.Forms.Cursors.Default;
            this.textBoxPrinterName.Font = new System.Drawing.Font("Verdana", 7.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxPrinterName.ForeColor = System.Drawing.Color.DimGray;
            this.textBoxPrinterName.Location = new System.Drawing.Point(3, 25);
            this.textBoxPrinterName.Name = "textBoxPrinterName";
            this.textBoxPrinterName.Size = new System.Drawing.Size(177, 26);
            this.textBoxPrinterName.TabIndex = 7;
            this.textBoxPrinterName.Text = ".....";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.textBoxStatus);
            this.panel2.Controls.Add(this.textBoxAnts);
            this.panel2.Location = new System.Drawing.Point(261, 6);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(153, 50);
            this.panel2.TabIndex = 4;
            // 
            // textBoxStatus
            // 
            this.textBoxStatus.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxStatus.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxStatus.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.textBoxStatus.Font = new System.Drawing.Font("Verdana", 7.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxStatus.ForeColor = System.Drawing.Color.Black;
            this.textBoxStatus.Location = new System.Drawing.Point(3, 5);
            this.textBoxStatus.Name = "textBoxStatus";
            this.textBoxStatus.Size = new System.Drawing.Size(142, 26);
            this.textBoxStatus.TabIndex = 7;
            this.textBoxStatus.Text = ".....";
            // 
            // textBoxAnts
            // 
            this.textBoxAnts.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxAnts.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxAnts.Cursor = System.Windows.Forms.Cursors.Default;
            this.textBoxAnts.Font = new System.Drawing.Font("Courier New", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxAnts.ForeColor = System.Drawing.Color.RoyalBlue;
            this.textBoxAnts.Location = new System.Drawing.Point(3, 25);
            this.textBoxAnts.Name = "textBoxAnts";
            this.textBoxAnts.Size = new System.Drawing.Size(142, 31);
            this.textBoxAnts.TabIndex = 7;
            this.textBoxAnts.TabStop = false;
            this.textBoxAnts.Text = ".....";
            this.textBoxAnts.Visible = false;
            // 
            // buttonAbort
            // 
            this.buttonAbort.FlatAppearance.BorderSize = 0;
            this.buttonAbort.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonAbort.Location = new System.Drawing.Point(420, 12);
            this.buttonAbort.Name = "buttonAbort";
            this.buttonAbort.Size = new System.Drawing.Size(29, 30);
            this.buttonAbort.TabIndex = 5;
            this.buttonAbort.Text = "x";
            this.buttonAbort.UseVisualStyleBackColor = true;
            this.buttonAbort.Click += new System.EventHandler(this.buttonAbort_Click);
            // 
            // timerAnts
            // 
            this.timerAnts.Enabled = true;
            this.timerAnts.Interval = 500;
            this.timerAnts.Tick += new System.EventHandler(this.timerAnts_Tick);
            // 
            // pictureBoxIcon
            // 
            this.pictureBoxIcon.Image = global::Print_Jobs.Properties.Resources.docIcon_50x50;
            this.pictureBoxIcon.Location = new System.Drawing.Point(5, 6);
            this.pictureBoxIcon.Name = "pictureBoxIcon";
            this.pictureBoxIcon.Size = new System.Drawing.Size(50, 50);
            this.pictureBoxIcon.TabIndex = 1;
            this.pictureBoxIcon.TabStop = false;
            // 
            // textBoxSubmitTime
            // 
            this.textBoxSubmitTime.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxSubmitTime.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxSubmitTime.Cursor = System.Windows.Forms.Cursors.Default;
            this.textBoxSubmitTime.Font = new System.Drawing.Font("Verdana", 7.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxSubmitTime.ForeColor = System.Drawing.Color.DimGray;
            this.textBoxSubmitTime.Location = new System.Drawing.Point(469, 16);
            this.textBoxSubmitTime.Name = "textBoxSubmitTime";
            this.textBoxSubmitTime.Size = new System.Drawing.Size(149, 26);
            this.textBoxSubmitTime.TabIndex = 7;
            this.textBoxSubmitTime.Text = ".....";
            // 
            // Card
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(630, 71);
            this.Controls.Add(this.textBoxSubmitTime);
            this.Controls.Add(this.buttonAbort);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pictureBoxIcon);
            this.Font = new System.Drawing.Font("Verdana", 7.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Card";
            this.Text = "Card";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxIcon;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button buttonAbort;
        private System.Windows.Forms.TextBox textBoxAnts;
        private System.Windows.Forms.Timer timerAnts;
        private System.Windows.Forms.TextBox textBoxDocumentName;
        private System.Windows.Forms.TextBox textBoxPrinterName;
        private System.Windows.Forms.TextBox textBoxStatus;
        private System.Windows.Forms.TextBox textBoxSubmitTime;
    }
}