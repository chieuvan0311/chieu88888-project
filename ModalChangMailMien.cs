using LiteDB;
using Microsoft.WindowsAPICodePack.Dialogs;
using PaypalAccountManager.Controller;
using PaypalAccountManager.Properties;
using PaypalAccountManager.Sesrvices;
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
    public partial class ModalChangMailMien : Form
    {
        Form1 form1 = null;
        public List<string> listDatabase = null;

        public ModalChangMailMien(Form1 form1)
        {
            InitializeComponent();

            this.form1 = form1;
        }

        private void txtHost_EditValueChanged(object sender, EventArgs e)
        {
        }

        private void txtEmail_EditValueChanged(object sender, EventArgs e)
        {
        }

        private void txtPass_EditValueChanged(object sender, EventArgs e)
        {
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            Settings.Default.mailHost = txtHost.Text;
            Settings.Default.mailUsername = txtEmail.Text;
            Settings.Default.mailPassword = txtPass.Text;
            Settings.Default.proxyAutoReset = this.cbAutoReset.Checked;
            Settings.Default.listProxy = this.listProxy.Text;
            Settings.Default.TokenProxy = this.txtTokenProxy.Text;
            Settings.Default.Save();
            this.Close();
        }

        private void ModalChangMailMien_Load(object sender, EventArgs e)
        {
            this.txtPpvn.Text = Settings.Default.ppvnPath;
            this.txtProfilePath.Text = Settings.Default.profilePath;
            this.txtInfoVerify.Text = Settings.Default.infoVerifyPath;
            this.txtHost.Text = Settings.Default.mailHost;
            this.txtEmail.Text = Settings.Default.mailUsername;
            this.txtPass.Text = Settings.Default.mailPassword;
            this.txtTokenProxy.Text = Settings.Default.TokenProxy;
            this.cbAutoReset.Checked = Settings.Default.proxyAutoReset;
            this.listProxy.Text = Settings.Default.listProxy;

            //this.cbUseGologin.Checked = Settings.Default.useGologin;
            switch (Settings.Default.browser)
            {
                case 0:
                    this.rdoChromnium.Checked = true;
                    break;
                case 1:
                    this.rdoGologin.Checked = true;
                    break;
                case 2:
                    this.rdoGPMBrowser.Checked = true;
                    break;
            }

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void txtPpvn_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = Path.Combine(Application.StartupPath, "PPVN");
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                this.txtPpvn.Text = dialog.FileName;
                Settings.Default.ppvnPath = dialog.FileName;
                Settings.Default.Save();
            }
        }

        private void btnChoseFolderDatabase_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = Path.Combine(Settings.Default.databasePath);
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                if (dialog.FileName != Settings.Default.databasePath)
                {
                    Settings.Default.databasePath = dialog.FileName;
                    Settings.Default.Save();
                    //this.getFolder();
                    //this.getAccount();
                }
            }
        }

        private void txtProfilePath_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = Path.Combine(Application.StartupPath, "Profiles");
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                this.txtProfilePath.Text = dialog.FileName;
                Settings.Default.profilePath = dialog.FileName;
                Settings.Default.Save();
            }
        }

        private void txtProfilePath_EditValueChanged(object sender, EventArgs e)
        {
            Settings.Default.profilePath = txtProfilePath.Text;
            Settings.Default.Save();
        }

        private void buttonEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = Settings.Default.infoVerifyPath;
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                this.txtInfoVerify.Text = dialog.FileName;
                Settings.Default.infoVerifyPath = dialog.FileName;
                Settings.Default.Save();
            }
        }

        private void buttonEdit1_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void cbUseGologin_CheckedChanged(object sender, EventArgs e)
        {
            //Settings.Default.useGologin = this.cbUseGologin.Checked;
            Settings.Default.Save();
        }

        private void rdoChromnium_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoChromnium.Checked)
            {
                Settings.Default.browser = 0;
            }
        }

        private void rdoGologin_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoGologin.Checked)
            {
                Settings.Default.browser = 1;
            }
        }

        private void rdoGPMBrowser_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoGPMBrowser.Checked)
            {
                Settings.Default.browser = 2;
            }
        }

        private void cbAutoReset_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void listProxy_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
