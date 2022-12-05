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
    public partial class ModalChangePhone : Form
    {
        Form1 form1 = null;
        public int needPhone = 0;
        public ModalChangePhone(int needPhone, Form1 form1)
        {
            InitializeComponent();
            this.needPhone = needPhone;
            this.form1 = form1;
        }

        private void ModalChangePhone_Load(object sender, EventArgs e)
        {

        }

        private void txtProxy_TextChanged(object sender, EventArgs e)
        {
            this.txtCountProxy.Text = this.txtProxy.Text.Split('\n').Where(x => x.Trim() != "").ToArray().Length.ToString();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (this.txtProxy.Text.Split('\n').Where(x => x.Trim() != "").ToArray().Length < this.needPhone)
            {
                MessageBox.Show("Bạn đã chọn " + this.needPhone + " account nên cần nhập đủ " + this.needPhone + " số đt", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            this.form1.changePhone(this.txtProxy.Text.Split('\n').Where(x => x.Trim() != "").ToList());
            this.Close();
        }
    }
}
