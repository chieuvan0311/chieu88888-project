using LiteDB;
using PaypalAccountManager.Entity;
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
    public partial class ModalAddFolder : Form
    {
        public string dbPath;
        MyDatabase myDatabase;
        public ModalAddFolder(MyDatabase myDatabase)
        {
            InitializeComponent();
            this.myDatabase = myDatabase;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            myDatabase.addNewFolder(this.textEdit1.Text);
            this.Close();
        }

        private void ModalAddFolder_Load(object sender, EventArgs e)
        {

        }
    }
}
