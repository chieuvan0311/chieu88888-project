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
    public partial class TableAccountDeleted : Form
    {
        List<string> listHeaderDefault = new List<string>();
        List<Account> listAccount = null;
        MyDatabase database;
        public TableAccountDeleted(MyDatabase database)
        {
            InitializeComponent();
            this.database = database;

            this.tableAccount.Columns.Clear();
            DataGridViewTextBoxCell centerStyle = new DataGridViewTextBoxCell();
            centerStyle.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DataGridViewColumn headerColumn = new DataGridViewColumn();
            headerColumn.MinimumWidth = 10;
            headerColumn.HeaderText = "STT";
            headerColumn.Name = "stt";
            headerColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            headerColumn.FillWeight = 25;
            headerColumn.Resizable = DataGridViewTriState.True;
            headerColumn.CellTemplate = centerStyle;
            tableAccount.Columns.Add(headerColumn);
            listHeaderDefault.Add("name");
            listHeaderDefault.Add("email");
            listHeaderDefault.Add("password");
            listHeaderDefault.Add("passwordEmail");
            listHeaderDefault.Add("address");
            listHeaderDefault.Add("cmndNumber");
            listHeaderDefault.Add("birthday");
            listHeaderDefault.Add("phone");
            listHeaderDefault.Add("note");
            listHeaderDefault.Add("proxy");
            listHeaderDefault.Add("securityQuestion");
            listHeaderDefault.Add("twoFA");
            listHeaderDefault.Add("cookie");

            foreach (var headerItem in DataApp.listHeaderDefault)
            {
                if (listHeaderDefault.Contains(headerItem.Name))
                {

                    headerColumn.SortMode = DataGridViewColumnSortMode.Automatic;
                    headerColumn = new DataGridViewColumn();
                    headerColumn.MinimumWidth = headerItem.MinimumWidth;
                    headerColumn.HeaderText = headerItem.HeaderText;
                    headerColumn.Name = headerItem.Name;
                    headerColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    headerColumn.FillWeight = headerItem.FillWeight;
                    headerColumn.Resizable = DataGridViewTriState.True;
                    headerColumn.CellTemplate = new DataGridViewTextBoxCell();
                    tableAccount.Columns.Add(headerColumn);
                }
            }

        }

        private void TableAccountDeleted_Load(object sender, EventArgs e)
        {
            this.getAccount();
        }

        public void getAccount()
        {
            this.listAccount = this.database.getAllAccountDeleted();
            this.tableAccount.Rows.Clear();
            for (int i = 0; i < listAccount.Count; i++)
            {
                var data = new List<string>();
                data.Add((i + 1).ToString());
                data.Add(listAccount[i].name);
                data.Add(listAccount[i].email);
                data.Add(listAccount[i].password);
                data.Add(listAccount[i].passwordEmail);
                this.tableAccount.Rows.Add(data.ToArray());
            }
        }
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedRows = this.tableAccount.SelectedRows
            .OfType<DataGridViewRow>()
            .Where(row => !row.IsNewRow)
            .ToArray().Reverse();
                string textContent = "";
                foreach (var row in selectedRows)
                {
                    string email = (string)row.Cells[2].Value;
                    var account = this.listAccount.FirstOrDefault(x => x.email == email);
                    textContent += account.ToString();
                    if (textContent.EndsWith("|"))
                    {
                        textContent = textContent.Substring(0, textContent.Length - 1);
                    }
                    textContent += "\n";

                }
                Clipboard.SetText(textContent);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi copy. thử lại...!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedRows = this.tableAccount.SelectedRows
            .OfType<DataGridViewRow>()
            .Where(row => !row.IsNewRow)
            .ToArray().Reverse();
                List<string> listAccount = new List<string>();
                foreach (var row in selectedRows)
                {
                    string email = (string)row.Cells[2].Value;
                    listAccount.Add(email);
                }
                this.database.removeAccountByEmail(listAccount);
                this.getAccount();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xóa. thử lại...!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
