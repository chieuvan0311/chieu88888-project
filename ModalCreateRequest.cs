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
    public partial class ModalCreateRequest : Form
    {
        Form1 form1 = null;
        public ModalCreateRequest(Form1 form1)
        {
            InitializeComponent();
            this.form1 = form1;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            this.form1.createRequest((int)this.txtAmount.Value, this.txtEmail.Text, this.txtNote.Text);
            this.Close();
        }
    }
}
