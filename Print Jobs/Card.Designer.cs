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
            this.labelDocumentName = new System.Windows.Forms.Label();
            this.labelPrinterName = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.labelStatus = new System.Windows.Forms.Label();
            this.pictureBoxAnts = new System.Windows.Forms.PictureBox();
            this.pictureBoxIcon = new System.Windows.Forms.PictureBox();
            this.buttonAbort = new System.Windows.Forms.Button();
            this.labelSubmitTime = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAnts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // labelDocumentName
            // 
            this.labelDocumentName.AutoSize = true;
            this.labelDocumentName.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDocumentName.Location = new System.Drawing.Point(3, 0);
            this.labelDocumentName.Name = "labelDocumentName";
            this.labelDocumentName.Size = new System.Drawing.Size(291, 29);
            this.labelDocumentName.TabIndex = 2;
            this.labelDocumentName.Text = "labelDocumentName";
            // 
            // labelPrinterName
            // 
            this.labelPrinterName.AutoSize = true;
            this.labelPrinterName.Font = new System.Drawing.Font("Verdana", 7.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPrinterName.Location = new System.Drawing.Point(3, 40);
            this.labelPrinterName.Name = "labelPrinterName";
            this.labelPrinterName.Size = new System.Drawing.Size(196, 25);
            this.labelPrinterName.TabIndex = 2;
            this.labelPrinterName.Text = "labelPrinterName";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.labelDocumentName);
            this.panel1.Controls.Add(this.labelPrinterName);
            this.panel1.Location = new System.Drawing.Point(92, 14);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(436, 70);
            this.panel1.TabIndex = 3;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.pictureBoxAnts);
            this.panel2.Controls.Add(this.labelStatus);
            this.panel2.Location = new System.Drawing.Point(535, 14);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(153, 68);
            this.panel2.TabIndex = 4;
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(3, 4);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(130, 25);
            this.labelStatus.TabIndex = 0;
            this.labelStatus.Text = "labelStatus";
            // 
            // pictureBoxAnts
            // 
            this.pictureBoxAnts.Image = global::Print_Jobs.Properties.Resources.progress_bar_animated;
            this.pictureBoxAnts.Location = new System.Drawing.Point(4, 40);
            this.pictureBoxAnts.Name = "pictureBoxAnts";
            this.pictureBoxAnts.Size = new System.Drawing.Size(146, 25);
            this.pictureBoxAnts.TabIndex = 1;
            this.pictureBoxAnts.TabStop = false;
            // 
            // pictureBoxIcon
            // 
            this.pictureBoxIcon.Image = global::Print_Jobs.Properties.Resources.docIcon_70x70;
            this.pictureBoxIcon.Location = new System.Drawing.Point(14, 14);
            this.pictureBoxIcon.Name = "pictureBoxIcon";
            this.pictureBoxIcon.Size = new System.Drawing.Size(70, 70);
            this.pictureBoxIcon.TabIndex = 1;
            this.pictureBoxIcon.TabStop = false;
            // 
            // buttonAbort
            // 
            this.buttonAbort.FlatAppearance.BorderSize = 0;
            this.buttonAbort.Font = new System.Drawing.Font("Verdana", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonAbort.Location = new System.Drawing.Point(691, 18);
            this.buttonAbort.Name = "buttonAbort";
            this.buttonAbort.Size = new System.Drawing.Size(47, 46);
            this.buttonAbort.TabIndex = 5;
            this.buttonAbort.Text = "X";
            this.buttonAbort.UseVisualStyleBackColor = true;
            // 
            // labelSubmitTime
            // 
            this.labelSubmitTime.AutoSize = true;
            this.labelSubmitTime.Location = new System.Drawing.Point(753, 18);
            this.labelSubmitTime.Name = "labelSubmitTime";
            this.labelSubmitTime.Size = new System.Drawing.Size(189, 25);
            this.labelSubmitTime.TabIndex = 6;
            this.labelSubmitTime.Text = "labelSubmitTime";
            // 
            // Card
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(950, 114);
            this.Controls.Add(this.labelSubmitTime);
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
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAnts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxIcon;
        private System.Windows.Forms.Label labelDocumentName;
        private System.Windows.Forms.Label labelPrinterName;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox pictureBoxAnts;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Button buttonAbort;
        private System.Windows.Forms.Label labelSubmitTime;
    }
}