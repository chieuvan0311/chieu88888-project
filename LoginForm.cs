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
    public partial class LoginForm : Form
    {
        bool isHide = false;
        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            this.Text = "Login Paypal Account Manager - Dev by Bacnt2412 (0364226686)";
            if (Settings.Default.username != "" && Settings.Default.password != "")
            {
                xNet.HttpRequest httpRequest = new xNet.HttpRequest();
                try
                {
                    var result = httpRequest.PostJson("http://45.76.158.231:3000/user/login", "{ \"username\": \"" + Settings.Default.username + "\", \"password\": \"" + Settings.Default.password + "\" }").ToString();
                    if (result.Contains("Authentication successful"))
                    {
                        isHide = true;
                        Form1 form1 = new Form1(this);
                        form1.Show();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(" ################### " + ex.Message);
                    MessageBox.Show("Sai rồi.!");
                }
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            Settings.Default.username = this.username.Text;
            Settings.Default.password = this.password.Text;
            Settings.Default.Save();
            xNet.HttpRequest httpRequest = new xNet.HttpRequest();
            try
            {
                var result = httpRequest.PostJson("http://45.76.158.231:3000/user/login", "{ \"username\": \"" + this.username.Text + "\", \"password\": \"" + this.password.Text + "\" }").ToString();
                if (result.Contains("Authentication successful"))
                {

                    Form1 form1 = new Form1(this);
                    form1.Show();
                    this.Hide();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(" ################### " + ex.Message);
                MessageBox.Show("Sai rồi.!");
            }
        }

        private void LoginForm_Activated(object sender, EventArgs e)
        {
            if (isHide)
            {
                this.Hide();
            }
        }
    }
}
