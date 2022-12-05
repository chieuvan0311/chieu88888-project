using PaypalAccountManager.Properties;
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
    public partial class SettingTableDisplay : Form
    {
        public SettingTableDisplay()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            string txtSetting = "";
            if (checkEdit1.Checked)
            {
                txtSetting += "name,";
            }
            if (checkEdit2.Checked)
            {
                txtSetting += "email,";
            }
            if (checkEdit3.Checked)
            {
                txtSetting += "password,";
            }
            if (checkEdit4.Checked)
            {
                txtSetting += "passwordEmail,";
            }
            if (checkEdit5.Checked)
            {
                txtSetting += "amount,";
            }
            if (checkEdit6.Checked)
            {
                txtSetting += "history,";
            }
          
            if (checkEdit8.Checked)
            {
                txtSetting += "resultLimit,";
            }
            if (checkEdit9.Checked)
            {
                txtSetting += "countTransaction,";
            }
            if (checkEdit10.Checked)
            {
                txtSetting += "status,";
            }
            if (checkEdit11.Checked)
            {
                txtSetting += "idTransaction,";
            }
            if (checkEdit12.Checked)
            {
                txtSetting += "mailReceive,";
            }
            if (checkEdit13.Checked)
            {
                txtSetting += "dateCreate,";
            }
            if (checkEdit14.Checked)
            {
                txtSetting += "address,";
            }
            if (checkEdit15.Checked)
            {
                txtSetting += "cmndNumber,";
            }
            if (checkEdit16.Checked)
            {
                txtSetting += "birthday,";
            }
            if (checkEdit17.Checked)
            {
                txtSetting += "phone,";
            }
            if (checkEdit18.Checked)
            {
                txtSetting += "note,";
            }
            if (checkEdit19.Checked)
            {
                txtSetting += "proxy,";
            }
            if (checkEdit20.Checked)
            {
                txtSetting += "ttVeryMail,";
            }
            if (checkEdit21.Checked)
            {
                txtSetting += "securityQuestion,";
            }
            if (checkEdit22.Checked)
            {
                txtSetting += "twoFA,";
            }
            if (checkEdit23.Checked)
            {
                txtSetting += "cookie,";
            }
            if (checkEdit7.Checked)
            {
                txtSetting += "resultKTGD,";
            }
            if (checkEdit24.Checked)
            {
                txtSetting += "log,";
            }
            Settings.Default.headerTable = txtSetting;
            Settings.Default.Save();
            this.Close();
        }

        private void SettingTableDisplay_Load(object sender, EventArgs e)
        {
            string header = Settings.Default.headerTable;
            if (header.Contains("name"))
            {
                checkEdit1.Checked = true;
            }
            if (header.Contains("email"))
            {
                checkEdit2.Checked = true;
            }
            if (header.Contains("password"))
            {
                checkEdit3.Checked = true;
            }
            if (header.Contains("passwordEmail"))
            {
                checkEdit4.Checked = true;
            }
            if (header.Contains("amount"))
            {
                checkEdit5.Checked = true;
            }
            if (header.Contains("history"))
            {
                checkEdit6.Checked = true;
            }
            if (header.Contains("resultKTGD"))
            {
                checkEdit7.Checked = true;
            }
            if (header.Contains("resultLimit"))
            {
                checkEdit8.Checked = true;
            }
            if (header.Contains("countTransaction"))
            {
                checkEdit9.Checked = true;
            }
            if (header.Contains("status"))
            {
                checkEdit10.Checked = true;
            }
            if (header.Contains("idTransaction"))
            {
                checkEdit11.Checked = true;
            }
            if (header.Contains("mailReceive"))
            {
                checkEdit12.Checked = true;
            }
            if (header.Contains("dateCreate"))
            {
                checkEdit13.Checked = true;
            }
            if (header.Contains("address"))
            {
                checkEdit14.Checked = true;
            }
            if (header.Contains("cmndNumber"))
            {
                checkEdit15.Checked = true;
            }
            if (header.Contains("birthday"))
            {
                checkEdit16.Checked = true;
            }
            if (header.Contains("phone"))
            {
                checkEdit17.Checked = true;
            }
            if (header.Contains("note"))
            {
                checkEdit18.Checked = true;
            }
            if (header.Contains("proxy"))
            {
                checkEdit19.Checked = true;
            }
            if (header.Contains("ttVeryMail"))
            {
                checkEdit20.Checked = true;
            }
            if (header.Contains("securityQuestion"))
            {
                checkEdit21.Checked = true;
            }
            if (header.Contains("twoFA"))
            {
                checkEdit22.Checked = true;
            }
            if (header.Contains("cookie"))
            {
                checkEdit23.Checked = true;
            }
            if (header.Contains("log"))
            {
                checkEdit24.Checked = true;
            }

        }
    }
}
