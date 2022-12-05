
namespace PaypalAccountManager
{
    partial class ModalChangePhone
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
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            this.txtCountProxy = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtProxy = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // simpleButton1
            // 
            this.simpleButton1.ImageOptions.Image = global::PaypalAccountManager.Properties.Resources.icons8_save_201;
            this.simpleButton1.Location = new System.Drawing.Point(222, 307);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(110, 32);
            this.simpleButton1.TabIndex = 8;
            this.simpleButton1.Text = "Xác nhận";
            this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
            // 
            // txtCountProxy
            // 
            this.txtCountProxy.AutoSize = true;
            this.txtCountProxy.Location = new System.Drawing.Point(63, 295);
            this.txtCountProxy.Name = "txtCountProxy";
            this.txtCountProxy.Size = new System.Drawing.Size(13, 13);
            this.txtCountProxy.TabIndex = 6;
            this.txtCountProxy.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 295);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Tổng:";
            // 
            // txtProxy
            // 
            this.txtProxy.Location = new System.Drawing.Point(22, 34);
            this.txtProxy.Name = "txtProxy";
            this.txtProxy.Size = new System.Drawing.Size(516, 254);
            this.txtProxy.TabIndex = 5;
            this.txtProxy.Text = "";
            this.txtProxy.TextChanged += new System.EventHandler(this.txtProxy_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(198, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Danh sách Số ĐT: ( Mỗi  Số ĐT 1 dòng)";
            // 
            // ModalChangePhone
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(557, 355);
            this.Controls.Add(this.simpleButton1);
            this.Controls.Add(this.txtCountProxy);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtProxy);
            this.Controls.Add(this.label1);
            this.Name = "ModalChangePhone";
            this.Text = "Thay đổi số điện thoại";
            this.Load += new System.EventHandler(this.ModalChangePhone_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private System.Windows.Forms.Label txtCountProxy;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox txtProxy;
        private System.Windows.Forms.Label label1;
    }
}