namespace Henkilotiedot
{
    partial class Form2
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
            this.txbUusNimike = new System.Windows.Forms.TextBox();
            this.lblKakkonen = new System.Windows.Forms.Label();
            this.btnF2Tallenna = new System.Windows.Forms.Button();
            this.btnF2Peruuta = new System.Windows.Forms.Button();
            this.txbUusiYksikko = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txbUusNimike
            // 
            this.txbUusNimike.Location = new System.Drawing.Point(12, 25);
            this.txbUusNimike.Name = "txbUusNimike";
            this.txbUusNimike.Size = new System.Drawing.Size(157, 20);
            this.txbUusNimike.TabIndex = 0;
            // 
            // lblKakkonen
            // 
            this.lblKakkonen.AutoSize = true;
            this.lblKakkonen.Location = new System.Drawing.Point(9, 9);
            this.lblKakkonen.Name = "lblKakkonen";
            this.lblKakkonen.Size = new System.Drawing.Size(93, 13);
            this.lblKakkonen.TabIndex = 1;
            this.lblKakkonen.Text = "Anna uusi nimike :";
            // 
            // btnF2Tallenna
            // 
            this.btnF2Tallenna.Location = new System.Drawing.Point(12, 110);
            this.btnF2Tallenna.Name = "btnF2Tallenna";
            this.btnF2Tallenna.Size = new System.Drawing.Size(75, 23);
            this.btnF2Tallenna.TabIndex = 2;
            this.btnF2Tallenna.Text = "Tallenna";
            this.btnF2Tallenna.UseVisualStyleBackColor = true;
            this.btnF2Tallenna.Click += new System.EventHandler(this.btnF2Tallenna_Click);
            // 
            // btnF2Peruuta
            // 
            this.btnF2Peruuta.Location = new System.Drawing.Point(161, 110);
            this.btnF2Peruuta.Name = "btnF2Peruuta";
            this.btnF2Peruuta.Size = new System.Drawing.Size(75, 23);
            this.btnF2Peruuta.TabIndex = 3;
            this.btnF2Peruuta.Text = "Peruuta";
            this.btnF2Peruuta.UseVisualStyleBackColor = true;
            this.btnF2Peruuta.Click += new System.EventHandler(this.btnF2Peruuta_Click);
            // 
            // txbUusiYksikko
            // 
            this.txbUusiYksikko.Location = new System.Drawing.Point(12, 70);
            this.txbUusiYksikko.Name = "txbUusiYksikko";
            this.txbUusiYksikko.Size = new System.Drawing.Size(157, 20);
            this.txbUusiYksikko.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Anna uusi yksikkö :";
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(248, 182);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txbUusiYksikko);
            this.Controls.Add(this.btnF2Peruuta);
            this.Controls.Add(this.btnF2Tallenna);
            this.Controls.Add(this.lblKakkonen);
            this.Controls.Add(this.txbUusNimike);
            this.Name = "Form2";
            this.Text = "Form2";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox txbUusNimike;
        public System.Windows.Forms.Label lblKakkonen;
        public System.Windows.Forms.Button btnF2Tallenna;
        private System.Windows.Forms.Button btnF2Peruuta;
        public System.Windows.Forms.TextBox txbUusiYksikko;
        public System.Windows.Forms.Label label1;
    }
}