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
    public partial class ModalChangeFolder : Form
    {
        Form1 form1 = null;
        public ModalChangeFolder(List<Folder> listFolder, int selectedIndex, Form1 form1)
        {
            InitializeComponent();
            foreach (var item in listFolder)
            {
                this.comboBox1.Items.Add(item);
            }
            this.comboBox1.SelectedIndex = selectedIndex;
            this.form1 = form1;
        }

        private void ModalChangeFolder_Load(object sender, EventArgs e)
        {

        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.form1.setChangeFolder(this.comboBox1.SelectedIndex);
            this.Close();              
        }
    }
}
