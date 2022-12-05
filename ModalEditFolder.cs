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
    public partial class ModalEditFolder : Form
    {
        Folder folder = null;
        MyDatabase myDatabase;
        public ModalEditFolder(MyDatabase myDatabase, Folder folder)
        {
            InitializeComponent();
            this.folder = folder;
            this.textEdit1.Text = this.folder.name;
            this.myDatabase = myDatabase;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.myDatabase.updateFolder(this.folder.Id, this.textEdit1.Text);
            this.Close();
        }

        private void ModalEditFolder_Load(object sender, EventArgs e)
        {

        }
    }
}
