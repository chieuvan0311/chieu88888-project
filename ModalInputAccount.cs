using PaypalAccountManager.Entity;
using PaypalAccountManager.Properties;
using PaypalAccountManager.Sesrvices;
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
    public partial class ModalInputAccount : Form
    {
        MyDatabase myDatabase = null;
        int IdFolder = 0;
        public ModalInputAccount(MyDatabase myDatabase, int IdFolder)
        {
            InitializeComponent();
            this.myDatabase = myDatabase;
            this.IdFolder = IdFolder;
            this.richTextBox1.Text = "";
            if (Settings.Default.inputFormat != "")
            {
                var listFormat = Settings.Default.inputFormat.Split(',');

                if (listFormat.Length >= 1 && listFormat[0] != "")
                {
                    this.comboBox1.SelectedIndex = Int32.Parse(listFormat[0]);
                }
                if (listFormat.Length >= 2 && listFormat[1] != "")
                {
                    this.comboBox2.SelectedIndex = Int32.Parse(listFormat[1]);

                }
                if (listFormat.Length >= 3 && listFormat[2] != "")
                {
                    this.comboBox3.SelectedIndex = Int32.Parse(listFormat[2]);

                }
                if (listFormat.Length >= 4 && listFormat[3] != "")
                {
                    this.comboBox4.SelectedIndex = Int32.Parse(listFormat[3]);

                }
                if (listFormat.Length >= 5 && listFormat[4] != "")
                {
                    this.comboBox5.SelectedIndex = Int32.Parse(listFormat[4]);
                }
                if (listFormat.Length >= 6 && listFormat[5] != "")
                {
                    this.comboBox6.SelectedIndex = Int32.Parse(listFormat[5]);
                }
                if (listFormat.Length >= 7 && listFormat[6] != "")
                {
                    this.comboBox7.SelectedIndex = Int32.Parse(listFormat[6]);
                }
                if (listFormat.Length >= 8 && listFormat[7] != "")
                {
                    this.comboBox8.SelectedIndex = Int32.Parse(listFormat[7]);
                }
                if (listFormat.Length >= 9 && listFormat[8] != "")
                {
                    this.comboBox9.SelectedIndex = Int32.Parse(listFormat[8]);
                }
                if (listFormat.Length >= 10 && listFormat[9] != "")
                {
                    this.comboBox10.SelectedIndex = Int32.Parse(listFormat[9]);
                }
                if (listFormat.Length >= 11 && listFormat[10] != "")
                {
                    this.comboBox11.SelectedIndex = Int32.Parse(listFormat[10]);
                }
                if (listFormat.Length >= 12 && listFormat[11] != "")
                {
                    this.comboBox12.SelectedIndex = Int32.Parse(listFormat[11]);
                }
                if (listFormat.Length >= 13 && listFormat[12] != "")
                {
                    this.comboBox13.SelectedIndex = Int32.Parse(listFormat[12]);
                }
            }
            //Tên TK| Email|MK Paypal|Mật Khẩu Emai|Địa Chỉ|Số CMND|Ngày Sinh|Số Phone|Note Tạm|IP Proxy|Câu hỏi bảo mật 1-2|bảo mật 2FA|Cookie
        }

        private void ModalInputAccount_Load(object sender, EventArgs e)
        {
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (Settings.Default.inputFormat == "")
            {
                MessageBox.Show("Vui lòng chọn định dạng", "Lỗi");
                return;
            }
            var listFormats = Settings.Default.inputFormat.Split(',').ToList();
            listFormats = listFormats.Where(x => x != "").ToList();



            var txtAccount = this.richTextBox1.Text.Split('\n').Where(item => item.Trim() != "").ToList();
            var listAccount = new List<Account>();
            foreach (var itemTxtAccount in txtAccount)
            {
                var listValue = itemTxtAccount.Split('|');
                Account newAcc = new Account();
                newAcc.IdFolder = this.IdFolder;
                newAcc.userAgent = DataApp.randomUserAgent();
                newAcc.isDelete = false;

                for (int i = 0; i < listValue.Length; i++)
                {
                    if (i >= listFormats.Count)
                    {
                        break;
                    }
                    int selectedIndexValue = Int32.Parse(listFormats[i]);

                    switch (selectedIndexValue)
                    {
                        case 0:
                            newAcc.name = listValue[i];
                            break;
                        case 1:
                            newAcc.email = listValue[i];
                            break;
                        case 2:
                            newAcc.password = listValue[i];
                            break;
                        case 3:
                            newAcc.passwordEmail = listValue[i];
                            break;
                        case 4:
                            newAcc.address = listValue[i];
                            break;
                        case 5:
                            newAcc.cmndNumber = listValue[i];
                            break;
                        case 6:
                            newAcc.birthday = listValue[i];
                            break;
                        case 7:
                            newAcc.phone = listValue[i];
                            break;
                        case 8:
                            newAcc.note = listValue[i];
                            break;
                        case 9:
                            newAcc.proxy = listValue[i];
                            break;
                        case 10:
                            newAcc.securityQuestion = listValue[i];
                            break;
                        case 11:
                            newAcc.twoFA = listValue[i];
                            break;
                        case 12:
                            newAcc.cookie = listValue[i];
                            break;
                    }
                }
                listAccount.Add(newAcc);
            }
            this.myDatabase.addNewListAccount(listAccount);
            this.Close();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void setInput(int indexItem, int indexSelect)
        {
            string[] list = Settings.Default.inputFormat.Split(',');
            list[indexItem] = indexSelect.ToString();
            string newFormat = "";
            foreach (var item in list)
            {
                newFormat += item + ",";
            }
            Settings.Default.inputFormat = newFormat;
            Settings.Default.Save();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            setInput(0, this.comboBox1.SelectedIndex);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            setInput(1, this.comboBox2.SelectedIndex);

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            setInput(2, this.comboBox3.SelectedIndex);

        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

            setInput(3, this.comboBox4.SelectedIndex);
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {

            setInput(4, this.comboBox5.SelectedIndex);
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {

            setInput(5, this.comboBox6.SelectedIndex);
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {

            setInput(6, this.comboBox7.SelectedIndex);
        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {

            setInput(7, this.comboBox8.SelectedIndex);
        }

        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {

            setInput(8, this.comboBox9.SelectedIndex);
        }

        private void comboBox10_SelectedIndexChanged(object sender, EventArgs e)
        {

            setInput(9, this.comboBox10.SelectedIndex);
        }

        private void comboBox11_SelectedIndexChanged(object sender, EventArgs e)
        {

            setInput(10, this.comboBox11.SelectedIndex);
        }

        private void comboBox12_SelectedIndexChanged(object sender, EventArgs e)
        {

            setInput(11, this.comboBox12.SelectedIndex);
        }

        private void comboBox13_SelectedIndexChanged(object sender, EventArgs e)
        {

            setInput(12, this.comboBox13.SelectedIndex);
        }
    }
}
