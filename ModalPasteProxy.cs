using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PaypalAccountManager
{
    public partial class ModalPasteProxy : Form
    {
        Form1 form1 = null;
        public int needProxy = 0;
        public ModalPasteProxy(int needProxy, Form1 form1)
        {
            InitializeComponent();
            this.needProxy = needProxy;
            this.form1 = form1;
        }

        private void txtProxy_TextChanged(object sender, EventArgs e)
        {
            this.txtCountProxy.Text = this.txtProxy.Text.Split('\n').Where(x => x.Trim() != "").ToArray().Length.ToString();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (this.txtProxy.Text.Split('\n').Where(x => x.Trim() != "").ToArray().Length < this.needProxy)
            {
                MessageBox.Show("Bạn đã chọn " + this.needProxy + " account nên cần nhập đủ " + this.needProxy + " proxy", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            this.form1.PasteProxy(this.txtProxy.Text.Split('\n').Where(x => x.Trim() != "").ToList());
            this.Close();
        }

        private void ModalPasteProxy_Load(object sender, EventArgs e)
        {

        }
    }
}
