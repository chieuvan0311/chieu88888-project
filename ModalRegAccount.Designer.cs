
namespace PaypalAccountManager
{
    partial class ModalRegAccount
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
            this.txtData = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnRun = new DevExpress.XtraEditors.SimpleButton();
            this.label2 = new System.Windows.Forms.Label();
            this.countThreadRegAcc = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.txtApiSim = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.countThreadRegAcc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtApiSim.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // txtData
            // 
            this.txtData.Location = new System.Drawing.Point(2, 25);
            this.txtData.Name = "txtData";
            this.txtData.Size = new System.Drawing.Size(796, 356);
            this.txtData.TabIndex = 0;
            this.txtData.Text = "";
            this.txtData.TextChanged += new System.EventHandler(this.txtData_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Data";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnRun
            // 
            this.btnRun.Appearance.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRun.Appearance.Options.UseFont = true;
            this.btnRun.Location = new System.Drawing.Point(329, 391);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(135, 47);
            this.btnRun.TabIndex = 2;
            this.btnRun.Text = "Chạy";
            this.btnRun.Click += new System.EventHandler(this.simpleButton1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 391);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Luồng chạy:";
            // 
            // countThreadRegAcc
            // 
            this.countThreadRegAcc.Location = new System.Drawing.Point(90, 387);
            this.countThreadRegAcc.Name = "countThreadRegAcc";
            this.countThreadRegAcc.Size = new System.Drawing.Size(57, 20);
            this.countThreadRegAcc.TabIndex = 4;
            this.countThreadRegAcc.ValueChanged += new System.EventHandler(this.countThreadRegAcc_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(39, 425);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Api Sim:";
            // 
            // txtApiSim
            // 
            this.txtApiSim.Location = new System.Drawing.Point(90, 421);
            this.txtApiSim.Name = "txtApiSim";
            this.txtApiSim.Size = new System.Drawing.Size(184, 20);
            this.txtApiSim.TabIndex = 5;
            this.txtApiSim.EditValueChanged += new System.EventHandler(this.txtApiSim_EditValueChanged);
            // 
            // ModalRegAccount
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 468);
            this.Controls.Add(this.txtApiSim);
            this.Controls.Add(this.countThreadRegAcc);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtData);
            this.Name = "ModalRegAccount";
            this.Text = "Reg Account";
            this.Load += new System.EventHandler(this.ModalRegAccount_Load);
            ((System.ComponentModel.ISupportInitialize)(this.countThreadRegAcc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtApiSim.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox txtData;
        private System.Windows.Forms.Label label1;
        private DevExpress.XtraEditors.SimpleButton btnRun;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown countThreadRegAcc;
        private System.Windows.Forms.Label label3;
        private DevExpress.XtraEditors.TextEdit txtApiSim;
    }
}