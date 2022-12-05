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
    public partial class ModalSetNameBusiness : Form
    {
        public int needProxy = 0;
        Form1 form1 = null;

        public ModalSetNameBusiness(int needProxy, Form1 form1)
        {
            InitializeComponent();
            this.needProxy = needProxy;
            this.form1 = form1;
        }

        private void ModalSetNameBusiness_Load(object sender, EventArgs e)
        {

        }

        private void txtProxy_TextChanged(object sender, EventArgs e)
        {
            this.txtCountProxy.Text = this.txtProxy.Text.Split('\n').Where(x => x.Trim() != "").ToArray().Length.ToString();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (this.txtProxy.Text.Split('\n').Where(x => x.Trim() != "").ToArray().Length < this.needProxy)
            {
                MessageBox.Show("Bạn đã chọn " + this.needProxy + " account nên cần nhập đủ " + this.needProxy + " tên", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            this.form1.SetNameBusiness(this.txtProxy.Text.Split('\n').Where(x => x.Trim() != "").ToList());
            this.Close();
        }
    }
}
