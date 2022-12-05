using PaypalAccountManager.Controller;
using PaypalAccountManager.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PaypalAccountManager
{
    public partial class ModalRegAccount : Form
    {
        MyController myController = null;
        public ModalRegAccount(MyController myController)
        {
            InitializeComponent();
            this.myController = myController;
            if (Settings.Default.apiSim != null)
            {
                this.txtApiSim.Text = Settings.Default.apiSim;
            }
            if (Settings.Default.countThreadRegAcc != 0)
            {
                this.countThreadRegAcc.Value = Settings.Default.countThreadRegAcc;
            }
            try
            {
                this.txtData.Text = File.ReadAllText("dataRegAccount.txt");
            }
            catch (Exception)
            {
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (this.btnRun.Text == "Chạy")
            {
                this.btnRun.Text = "Dừng";
                this.myController.StartRegAccount(this.txtData.Text.Split('\n').Where(x => x.Trim() != "").ToList(), this);
            }
            else
            {
                this.myController.StopThreadRegAcc();
                this.btnRun.Text = "Chạy";
            }
        }

        private void txtApiSim_EditValueChanged(object sender, EventArgs e)
        {
            Settings.Default.apiSim = this.txtApiSim.Text;
            Settings.Default.Save();
        }

        private void countThreadRegAcc_ValueChanged(object sender, EventArgs e)
        {
            Settings.Default.countThreadRegAcc = (int) this.countThreadRegAcc.Value;
            Settings.Default.Save();
        }

        private void simpleButton1_Click_1(object sender, EventArgs e)
        {
           
        }

        private void ModalRegAccount_Load(object sender, EventArgs e)
        {
            //this.txtData.Text = "LinhQL1Rtc1814@hotmail.com|L1Q2vfa1826|L1Q2vfa1826|Trưởng|Thị Phương|Thạo|079989664410|749575|12/21/1989|185 Hoàng Việt|Phú Mỹ|Quận 7|hungamazon:IAed3909ia4PjJsxTodZ@103.95.199.245:3171";
        }
        public void removeAccount(string email)
        {
            if (this.txtData.InvokeRequired)
            {
                this.txtData.Invoke(new MethodInvoker(() =>
                {
                    var listAccount = this.txtData.Text.Split('\n').ToList();
                    listAccount = listAccount.Where(x => !x.StartsWith(email)).ToList();
                    string listText = "";
                    foreach (var item in listAccount)
                    {
                        listText += item + "\n";
                    }
                    this.txtData.Text = listText;
                    File.WriteAllText("dataRegAccount.txt", listText);
                }));
            }else
            {
                var listAccount = this.txtData.Text.Split('\n').ToList();
                listAccount = listAccount.Where(x => !x.StartsWith(email)).ToList();
                string listText = "";
                foreach (var item in listAccount)
                {
                    listText += item + "\n";
                }
                this.txtData.Text = listText;
                File.WriteAllText("dataRegAccount.txt", listText);
            }
           
        }

        private void txtData_TextChanged(object sender, EventArgs e)
        {
            File.WriteAllText("dataRegAccount.txt", this.txtData.Text);
        }
    }
}
