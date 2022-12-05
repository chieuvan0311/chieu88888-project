using LiteDB;
using PaypalAccountManager.Entity;
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
    public partial class ModalEditAccount : Form
    {
        Account account = null;
        Form1 form1 = null;
        public ModalEditAccount(Account account, Form1 form1)
        {
            InitializeComponent();
            this.txtName.Text = account.name;
            this.txtEmail.Text = account.email;
            this.textEdit3.Text = account.password;
            this.textEdit4.Text = account.passwordEmail;
            this.textEdit5.Text = account.address;
            this.textEdit6.Text = account.cmndNumber;
            this.textEdit7.Text = account.birthday;
            this.textEdit8.Text = account.phone;
            this.textEdit9.Text = account.note;
            this.textEdit10.Text = account.proxy;
            this.textEdit11.Text = account.securityQuestion;
            this.textEdit12.Text = account.twoFA;
            this.textEdit13.Text = account.cookie;
            this.textEdit1.Text = account.amount;
            this.txtHistoryPassword.Text = account.HistoryPassword;
            this.textEdit2.Text = account.subEmail;
            this.textEdit14.Text = account.history;
            this.txtBusinessName.Text = account.businessName;


            this.account = account;
            this.form1 = form1;
        }

        private void ModalEditAccount_Load(object sender, EventArgs e)
        {

        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.account.name = this.txtName.Text;
            this.account.email = this.txtEmail.Text;
            this.account.password = this.textEdit3.Text;
            this.account.passwordEmail = this.textEdit4.Text;
            this.account.address = this.textEdit5.Text;
            this.account.cmndNumber = this.textEdit6.Text;
            this.account.birthday = this.textEdit7.Text;
            this.account.phone = this.textEdit8.Text;
            this.account.note = this.textEdit9.Text;
            this.account.proxy = this.textEdit10.Text;
            this.account.securityQuestion = this.textEdit11.Text;
            this.account.twoFA = this.textEdit12.Text;
            this.account.cookie = this.textEdit13.Text;
            this.account.amount = this.textEdit1.Text;
            this.account.subEmail = this.textEdit2.Text;
            this.account.history = this.textEdit14.Text;
            this.account.businessName = this.txtBusinessName.Text;


            this.form1.updateAccount(this.account);
            this.Close();
        }

        private void textEdit14_EditValueChanged(object sender, EventArgs e)
        {

        }
    }
}
