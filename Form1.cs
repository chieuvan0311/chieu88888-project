using LiteDB;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using PaypalAccountManager.Controller;
using PaypalAccountManager.Entity;
using PaypalAccountManager.Properties;
using PaypalAccountManager.Sesrvices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TwoFactorAuthNet;

namespace PaypalAccountManager
{
    public partial class Form1 : Form
    {
        public Folder folderSelected = null;
        public MyDatabase myDatabase = null;
        public List<Account> listAccount = null;
        public List<string> listDatabase = null;
        public List<Folder> listFolder = null;
        public bool isSearch = false;
        public bool firstLoad = true;
        public MyController myController = null;
        LoginForm loginForm = null;
        string publicIP = "";
        public Form1(LoginForm loginForm)
        {
            InitializeComponent();
            this.loginForm = loginForm;
            DataApp.Init();
            if (!Directory.Exists(Path.Combine(Application.StartupPath, "Data")))
            {
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Data"));
            }
            if (!Directory.Exists(Path.Combine(Application.StartupPath, "Profile")))
            {
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Profile"));
            }
            if (!Directory.Exists(Path.Combine(Application.StartupPath, "PPVN")))
            {
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, "PPVN"));
            }
            if (Settings.Default.profilePath == "")
            {
                Settings.Default.profilePath = Path.Combine(Application.StartupPath, "Profile");
                Settings.Default.Save();
            }
            if (Settings.Default.databasePath == "")
            {
                Settings.Default.databasePath = Path.Combine(Application.StartupPath, "Data");
                Settings.Default.Save();
            }
            if (Settings.Default.ppvnPath == "")
            {
                Settings.Default.ppvnPath = Path.Combine(Application.StartupPath, "PPVN");
                Settings.Default.Save();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.publicIP = new System.Net.WebClient().DownloadString("https://api.ipify.org");
            this.Text = "Paypal Account Manager - Dev by Bacnt2412 (0364226686)- v4.5";
            this.loadFileDatabase();
            this.myController = new MyController(this.myDatabase, this, this.publicIP);

            this.cbbCategorySearch.SelectedIndex = 0;
            this.cbbSearchAmount.SelectedIndex = 0;
            this.txtThreadCount.Value = Settings.Default.threadCount;
            this.cbAutoCloseChrome.Checked = Settings.Default.autoCloseChrome;
            firstLoad = false;
        }

        public void loadFileDatabase()
        {
            this.cbbDatabase.Properties.Items.Clear();
            string databasePath = Settings.Default.databasePath;
            if (Directory.Exists(databasePath))
            {

                listDatabase = new List<string>();
                DirectoryInfo directoryInfo = new DirectoryInfo(databasePath);
                foreach (var item in directoryInfo.GetFiles())
                {
                    if (!item.Name.Contains("-log.db"))
                    {
                        listDatabase.Add(item.FullName);
                        this.cbbDatabase.Properties.Items.Add(item.Name);
                    }
                }
                if (listDatabase.Count == 0)
                {
                    using (var db = new LiteDatabase(Path.Combine(databasePath, "MyData.db")))
                    {
                    }
                    directoryInfo = new DirectoryInfo(Path.Combine(Application.StartupPath, "Data"));
                    foreach (var item in directoryInfo.GetFiles())
                    {
                        if (!item.Name.Contains("-log.db"))
                        {
                            listDatabase.Add(item.FullName);
                            this.cbbDatabase.Properties.Items.Add(item.Name);
                        }
                    }
                }

                if (listDatabase.Count > 0)
                {
                    this.cbbDatabase.SelectedIndex = -1;
                    if (Settings.Default.selectedDatabaseIndex < listDatabase.Count)
                    {
                        this.cbbDatabase.SelectedIndex = Settings.Default.selectedDatabaseIndex;

                    }
                    else
                    {
                        this.cbbDatabase.SelectedIndex = 0;
                    }
                }
            }

        }

        public void loadHeaderTable()
        {
            var settingHeader = Settings.Default.headerTable.Split(',');
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
            foreach (var headerItem in DataApp.listHeaderDefault)
            {
                if (settingHeader.Contains(headerItem.Name))
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
        public void setAccountToTable(bool isRandom)
        {
            if (this.folderSelected != null)
            {
                var settingHeader = Settings.Default.headerTable.Split(',');
                var listAccount = this.myDatabase.getAccounts(this.folderSelected.Id);

                if (isRandom)
                {
                    var arrIndex = new List<int>();
                    for (int i = 0; i < listAccount.Count; i++)
                    {
                        arrIndex.Add(i);
                    }
                    var newListAccount = new List<Account>();
                    for (int i = 0; i < listAccount.Count; i++)
                    {
                        var indexRandom = new Random().Next(0, arrIndex.Count);
                        newListAccount.Add(listAccount[arrIndex[indexRandom]]);
                        arrIndex.RemoveAt(indexRandom);
                    }
                    listAccount = newListAccount;
                }
                this.listAccount = listAccount;
                this.tableAccount.Rows.Clear();
                for (int i = 0; i < listAccount.Count; i++)
                {
                    var data = new List<string>();
                    data.Add((i + 1).ToString());
                    var account = listAccount[i];
                    Type myType = account.GetType();
                    IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());

                    foreach (var headerName in settingHeader)
                    {
                        foreach (PropertyInfo prop in props)
                        {
                            if (headerName == prop.Name)
                            {
                                string propValue = (string)prop.GetValue(account, null);
                                if (headerName == "status" && propValue != null && propValue.Trim() != null)
                                {
                                    DateTime timeEnd = DateTime.ParseExact(propValue, "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                                    if (timeEnd > DateTime.Now)
                                    {
                                        TimeSpan t = timeEnd.Subtract(DateTime.Now);
                                        propValue = t.ToString(@"dd\.hh\:mm\:ss");
                                    }
                                    else
                                    {
                                        propValue = "";
                                    }
                                }
                                if (headerName == "mailReceive")
                                {
                                    propValue = account.subEmail;
                                }
                                data.Add(propValue);
                                break;
                            }
                        }
                    }

                    this.tableAccount.Rows.Add(data.ToArray());
                }
            }
            else
            {
                this.listAccount = null;
                this.tableAccount.Rows.Clear();
            }
        }
        public void getAccount(bool isRandom = false)
        {
            if (this.tableAccount.InvokeRequired)
            {
                this.tableAccount.Invoke(new MethodInvoker(() =>
                {
                    this.setAccountToTable(isRandom);
                }));
            }
            else
            {
                this.setAccountToTable(isRandom);
            }
        }
        public void getFolder(bool isSelectEnd = false)
        {
            var listFolder = myDatabase.getAllFolders();
            this.listFolder = listFolder;
            this.cbbFolder.Properties.Items.Clear();
            this.checkListFolderSearch.Items.Clear();


            this.cbbFolder.Text = "";
            var checkSelected = false;
            foreach (var item in listFolder)
            {
                Folder newItem = new Folder();
                newItem.Id = item.Id;
                newItem.name = item.name.Split('-')[0] + "-" + item.name.Split('-')[2];
                this.cbbFolder.Properties.Items.Add(newItem);
                if (!isSelectEnd && this.folderSelected != null && item.Id == this.folderSelected.Id)
                {
                    this.cbbFolder.SelectedIndex = this.cbbFolder.Properties.Items.Count - 1;
                    checkSelected = true;
                }
            }

            foreach (var item in listFolder)
            {
                Folder newItem = new Folder();
                newItem.Id = item.Id;
                newItem.name = item.name.Split('-')[0] + "-" + item.name.Split('-')[1];
                this.checkListFolderSearch.Items.Add(newItem, false);
            }

            this.checkListFolderSearch.Items.Add("ALL", false);

            if (isSelectEnd && listFolder.Count > 0)
            {
                this.cbbFolder.SelectedIndex = listFolder.Count - 1;
                checkSelected = true;
            }
            if (listFolder.Count > 0 && !checkSelected)
            {
                this.cbbFolder.SelectedIndex = 0;
                this.folderSelected = listFolder[0];
            }
            else if (listFolder.Count == 0)
            {
                this.folderSelected = null;
                this.btnEditFolder.Enabled = false;
                this.btnDeleteFolder.Enabled = false;
                this.btnAddAccount.Enabled = false;

            }

            if (this.folderSelected != null)
            {
                this.btnEditFolder.Enabled = true;
                this.btnDeleteFolder.Enabled = true;
                this.btnAddAccount.Enabled = true;

            }

        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }
        private void tableAccount_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnSettingDisplay_Click(object sender, EventArgs e)
        {
            SettingTableDisplay settingTableDisplay = new SettingTableDisplay();
            settingTableDisplay.ShowDialog();
            this.loadHeaderTable();
            this.getAccount();
        }

        private void btnShowAccount_Click(object sender, EventArgs e)
        {
            TableAccountDeleted tableAccountDeleted = new TableAccountDeleted(this.myDatabase);
            tableAccountDeleted.ShowDialog();
        }



        private void btnStopAll_Click(object sender, EventArgs e)
        {
            this.myController.StopAll();
        }

        private void txtThreadCount_ValueChanged(object sender, EventArgs e)
        {
            //Settings.Default.threadCount = (int)txtThreadCount.Value;
            //Settings.Default.Save();
        }
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (!firstLoad)
            {
                this.searchAccount();
            }
        }

        private void cbbCategorySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!firstLoad)
            {
                this.searchAccount();
            }
        }

        public void searchAccount()
        {
            try
            {
                var categorySearch = (string)this.cbbCategorySearch.SelectedItem;

                var listAccountSearch = this.listAccount;
                if (this.cbAllFolder.Checked)
                {
                    listAccountSearch = this.myDatabase.getAllAccount();
                    this.listAccount = listAccountSearch;
                }
                else
                {
                    listAccountSearch = this.myDatabase.getAccounts(this.folderSelected.Id);
                    this.listAccount = listAccountSearch;
                }
                var textSearchs = new List<string>();
                if (categorySearch == "Số dư")
                {
                    textSearchs = this.txtSearch.Text.Split('\n').Where(x => x.Trim() != "").ToList();
                }
                else
                {
                    textSearchs = this.txtSearch.Text.Split('\n').Where(x => x.Trim().Length > 2).ToList();
                }
                if (listAccountSearch != null && listAccountSearch.Count > 0 && textSearchs.Count > 0 && this.txtSearch.Text.Trim() != "")
                {
                    this.isSearch = true;
                    switch (categorySearch)
                    {
                        case "Email":
                            categorySearch = "email";
                            break;
                        case "Tên":
                            categorySearch = "name";
                            break;
                        case "Lịch sử":
                            categorySearch = "history";
                            break;
                        case "ID giao dịch":
                            categorySearch = "idTransaction";
                            break;
                        case "Mail nhận":
                            categorySearch = "mailReceive";
                            break;
                        case "KT Limit":
                            categorySearch = "resultLimit";
                            break;
                        case "Số dư":
                            categorySearch = "amount";
                            break;
                    }
                    List<Account> listAccountResult = new List<Account>();
                    for (int i = 0; i < listAccountSearch.Count; i++)
                    {
                        var account = listAccountSearch[i];
                        if (listAccountResult.Contains(account))
                        {
                            continue;
                        }
                        if (textSearchs.Count > 0)
                        {
                            foreach (var textSearch in textSearchs)
                            {
                                Type myType = account.GetType();
                                IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());
                                foreach (PropertyInfo prop in props)
                                {
                                    if (prop.Name == categorySearch)
                                    {
                                        string propValue = (string)prop.GetValue(account, null);
                                        if (propValue != null && propValue.ToLower().Contains(textSearch.ToLower()))
                                        {
                                            listAccountResult.Add(account);
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            listAccountResult.Add(account);
                        }
                    }
                    listAccountResult.Reverse();
                    listAccountResult = listAccountResult.GroupBy(x => x.Id).Select(x => x.First()).ToList();

                    this.tableAccount.Rows.Clear();
                    if (listAccountResult.Count > 0)
                    {
                        var settingHeader = Settings.Default.headerTable.Split(',');
                        for (int i = 0; i < listAccountResult.Count; i++)
                        {
                            var data = new List<string>();
                            data.Add((i + 1).ToString());
                            var account = listAccountResult[i];
                            Type myType = account.GetType();
                            IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());


                            foreach (var headerName in settingHeader)
                            {
                                foreach (PropertyInfo prop in props)
                                {
                                    if (headerName == prop.Name)
                                    {
                                        string propValue = (string)prop.GetValue(account, null);

                                        if (headerName == "status" && propValue != null && propValue.Trim() != null)
                                        {
                                            DateTime timeEnd = DateTime.ParseExact(propValue, "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                                            if (timeEnd > DateTime.Now)
                                            {
                                                TimeSpan t = timeEnd.Subtract(DateTime.Now);
                                                propValue = t.ToString(@"dd\.hh\:mm\:ss");
                                            }
                                            else
                                            {
                                                propValue = "";
                                            }
                                        }
                                        if (headerName == "mailReceive")
                                        {
                                            propValue = account.subEmail;
                                        }

                                        data.Add(propValue);
                                        break;
                                    }
                                }
                            }
                            this.tableAccount.Rows.Add(data.ToArray());
                        }
                    }
                }
                else
                {
                    if (this.isSearch)
                    {
                        this.getAccount();
                        this.isSearch = false;
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Lỗi search account. thử lại...!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnAddAccount_Click(object sender, EventArgs e)
        {
            ModalInputAccount modalInputAccount = new ModalInputAccount(this.myDatabase, this.folderSelected.Id);
            modalInputAccount.ShowDialog();
            this.getAccount();
        }

        private void cbbDatabase_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Default.selectedDatabaseIndex = this.cbbDatabase.SelectedIndex;
            Settings.Default.Save();

            if (this.myDatabase != null)
            {
                this.myDatabase.closeDB();
            }
            if (this.cbbDatabase.SelectedIndex >= 0)
            {
                this.myDatabase = new MyDatabase(this.listDatabase[this.cbbDatabase.SelectedIndex]);
                this.myController = new MyController(this.myDatabase, this, this.publicIP);
                this.loadHeaderTable();
                this.getFolder();
            }

            //this.getAccount();
        }

        private void btnAddFolder_Click(object sender, EventArgs e)
        {
            ModalAddFolder modalAddFolder = new ModalAddFolder(this.myDatabase);
            modalAddFolder.ShowDialog();
            this.getFolder(true);
        }

        private void btnEditFolder_Click(object sender, EventArgs e)
        {
            ModalEditFolder modalEditFolder = new ModalEditFolder(this.myDatabase, folderSelected);
            modalEditFolder.ShowDialog();
            this.getFolder();
        }

        private void cbbFolder_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.folderSelected = (Folder)this.cbbFolder.SelectedItem;
            this.getAccount();
        }

        private void btnDeleteFolder_Click(object sender, EventArgs e)
        {
            var check = MessageBox.Show("Xác nhận xóa thư mục", "Xác nhận", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (check == DialogResult.OK)
            {
                this.myDatabase.deleteFolder(this.folderSelected.Id);
                this.getFolder(true);
                this.getAccount();
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.myDatabase != null)
            {
                this.myDatabase.closeDB();
            }
            this.myController.StopAll();
            this.loginForm.Close();
        }
        public void onCopy(string[] format)
        {
            this.listAccount = this.myDatabase.getAllAccount();
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
                    if (format.Length == 1 && format[0] == "email")
                    {
                        textContent += email;
                    }
                    else
                    {

                        var account = this.listAccount.FirstOrDefault(x => x.email == email);

                        Type myType = account.GetType();
                        IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());

                        foreach (var f in format)
                        {
                            foreach (PropertyInfo prop in props)
                            {
                                Console.WriteLine(" ### prop.Name: " + prop.Name);
                                if (prop.Name == "twoFA" && f == "twoFA")
                                {
                                    if (account.twoFA != null)
                                    {
                                        TwoFactorAuth twoFactorAuth = new TwoFactorAuthNet.TwoFactorAuth();
                                        var newToken = twoFactorAuth.GetCode(account.twoFA.Replace(" ", ""));
                                        textContent += newToken;
                                        break;
                                    }

                                }
                                else if (f == prop.Name)
                                {
                                    textContent += prop.GetValue(account, null) + "|";
                                    break;
                                }
                            }
                        }


                        if (format.Length == 0)
                        {
                            textContent += account.ToString();
                        }

                        if (textContent.EndsWith("|"))
                        {
                            textContent = textContent.Substring(0, textContent.Length - 1);
                        }
                    }
                    textContent += "\n";

                }
                Clipboard.SetText(textContent);
                this.contextMenuStrip1.Hide();
            }
            catch (Exception e)
            {
                MessageBox.Show("Lỗi copy. thử lại...!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void mậtKhẩuPPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.onCopy(new string[] { "password" });
        }

        private void sốDưIDGiaoDịchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.onCopy(new string[] { "amount", "idTransaction", "email", "mailReceive" });
        }
        private void fullNhậpAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.onCopy(new string[] { });
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //this.onCopy(new string[] { });
        }

        private void emailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.onCopy(new string[] { "email" });
        }

        private void mậtKhẩuEmailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.onCopy(new string[] { "passwordEmail" });
        }

        private void sốDưToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.onCopy(new string[] { "amount" });
        }

        private void mã2FAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.onCopy(new string[] { "twoFA" });
        }

        private void proxyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.onCopy(new string[] { "proxy" });
        }

        private void tênEmailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.onCopy(new string[] { "name", "email" });
        }

        private void iDGiaoDịchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.onCopy(new string[] { "idTransaction", "email" });
        }

        private void iDGiaoDịchEmailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.onCopy(new string[] { "idTransaction", "email", "history" });
        }

        private void tênEmailLịchSửToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.onCopy(new string[] { "name", "email", "history" });
        }

        private void logToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.onCopy(new string[] { "log" });
        }

        public void startAction(ActionRun actionRun, List<string> listPhone = null)
        {
            var selectedRows = this.tableAccount.SelectedRows
              .OfType<DataGridViewRow>()
              .Where(row => !row.IsNewRow)
              .ToArray().Reverse();
            var listEmail = new List<string>();
            foreach (var row in selectedRows)
            {
                string email = (string)row.Cells[2].Value;
                listEmail.Add(email);
            }
            this.myController.StartMain(listEmail, actionRun, listPhone);

        }
        private void checkLịchSửGiaoDịchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.startAction(ActionRun.CHECK_HISTORY);
        }

        private void cbAutoCloseChrome_CheckedChanged(object sender, EventArgs e)
        {
            //Settings.Default.autoCloseChrome = this.cbAutoCloseChrome.Checked;
            //Settings.Default.Save();
        }

        public void updateTableAccount(Account account)
        {
            foreach (DataGridViewRow row in this.tableAccount.Rows)
            {
                if (row.Cells[1].Value != null && row.Cells[2].Value != null && row.Cells[1].Value.ToString() == account.name)
                {
                    if (account.log != null && account.log != "" && account.log != "null" && account.log.ToLower().Contains("error"))
                    {
                        row.DefaultCellStyle.BackColor = Color.Red;
                    }
                    else
                    {
                        row.DefaultCellStyle.BackColor = SystemColors.ButtonFace;
                    }
                    var settingHeader = Settings.Default.headerTable.Split(',');
                    var indexCell = 1;
                    var data = new List<string>();
                    Type myType = account.GetType();
                    IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());

                    foreach (var headerName in settingHeader)
                    {
                        foreach (PropertyInfo prop in props)
                        {
                            if (headerName == prop.Name)
                            {
                                string propValue = (string)prop.GetValue(account, null);
                                if (headerName == "mailReceive")
                                {
                                    propValue = account.subEmail;
                                }

                                if (propValue != null)
                                {

                                    if (headerName == "status" && propValue != null && propValue.Trim() != null)
                                    {
                                        DateTime timeEnd = DateTime.ParseExact(propValue, "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                                        if (timeEnd > DateTime.Now)
                                        {
                                            TimeSpan t = timeEnd.Subtract(DateTime.Now);
                                            propValue = t.ToString(@"dd\.hh\:mm\:ss");
                                        }
                                        else
                                        {
                                            propValue = "";
                                        }
                                    }

                                    row.Cells[indexCell].Value = propValue;
                                }
                                indexCell++;
                                break;
                            }
                        }
                    }

                    break;
                }
            }
        }

        private void lấyThôngTinTàiKhoảnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.startAction(ActionRun.GET_INFOMATION);
        }

        private void btnStopThreadMain_Click(object sender, EventArgs e)
        {
            this.myController.stopThreadMain();
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
                    this.loadFileDatabase();
                    //this.getFolder();
                    //this.getAccount();
                }
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            this.startAction(ActionRun.START_CHROME);
        }

        private void xóaToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void chuyểnDanhMụcToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModalChangeFolder modalChangeFolder = new ModalChangeFolder(this.listFolder, this.cbbFolder.SelectedIndex, this);
            modalChangeFolder.ShowDialog();
            this.getAccount();
        }
        public void setChangeFolder(int selectedIndexFolder)
        {
            var selectedRows = this.tableAccount.SelectedRows
            .OfType<DataGridViewRow>()
            .Where(row => !row.IsNewRow)
            .ToArray().Reverse();
            var listEmail = new List<string>();
            foreach (var row in selectedRows)
            {
                string email = (string)row.Cells[2].Value;
                listEmail.Add(email);
            }
            this.myDatabase.updateChangeFolder(listEmail, this.listFolder[selectedIndexFolder].Id);
        }
        public void updateAccount(Account account)
        {
            this.myDatabase.updateAccount(account);
            this.updateTableAccount(account);
        }

        private void sửaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Account accountSelected = null;
            var selectedRows = this.tableAccount.SelectedRows
           .OfType<DataGridViewRow>()
           .Where(row => !row.IsNewRow)
           .ToArray().Reverse();
            var listEmail = new List<string>();
            foreach (var row in selectedRows)
            {
                string email = (string)row.Cells[2].Value;
                accountSelected = this.myDatabase.findAccountByEmail(email);
                break;
            }

            ModalEditAccount modalEditAccount = new ModalEditAccount(accountSelected, this);
            modalEditAccount.ShowDialog();
        }

        private void cbSearchALL_CheckedChanged(object sender, EventArgs e)
        {
            this.searchAccount();
        }

        private void tạoCâuHỏiBMVà2FAToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void btnReload_Click(object sender, EventArgs e)
        {

        }

        private void xóa2FAToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void checkListFolderSearch_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void checkListFolderSearch_CheckMemberChanged(object sender, EventArgs e)
        {

        }

        private void checkListFolderSearch_SelectedValueChanged(object sender, EventArgs e)
        {
            //var l = this.checkListFolderSearch.SelectedIndex;
            //var l = this.checkListFolderSearch.Se;
            try
            {
                var indexSelect = this.checkListFolderSearch.SelectedItem;
                var selectedIndices = this.checkListFolderSearch.CheckedItems;
                var isSelecting = false;
                foreach (var item in selectedIndices)
                {
                    if (indexSelect == item)
                    {
                        isSelecting = true;
                        break;
                    }
                }
                if (!isSelecting)
                {
                    this.checkListFolderSearch.SetItemChecked(this.checkListFolderSearch.SelectedIndex, true);
                }
                else
                {
                    this.checkListFolderSearch.SetItemChecked(this.checkListFolderSearch.SelectedIndex, false);
                }
            }
            catch (Exception)
            {

            }
        }

        private void checkListFolderSearch_ItemCheck(object sender, DevExpress.XtraEditors.Controls.ItemCheckEventArgs e)
        {
            if (!firstLoad)
            {
                this.FilterAccount();
            }
        }

        private void groupControl3_Paint(object sender, PaintEventArgs e)
        {

        }

        public void FilterAccount()
        {
            var listAccountSearch = this.myDatabase.getAllAccount();
            var listFolderSearch = new List<string>();
            var amountSearch = this.cbbSearchAmount.SelectedItem;
            int firstAmount = 0;
            int lastAmount = 0;
            if (amountSearch.ToString() == "ALL")
            {
                firstAmount = -1;
                firstAmount = -1;
            }
            else if (amountSearch.ToString().Contains("-"))
            {
                firstAmount = Int32.Parse(amountSearch.ToString().Replace("$", "").Replace(" ", "").Split('-')[0]);
                lastAmount = Int32.Parse(amountSearch.ToString().Replace("$", "").Replace(" ", "").Split('-')[1]);
            }
            else if (this.cbbSearchAmount.SelectedIndex == 8)
            {
                firstAmount = (int)this.txtAmountFrom.Value;
                lastAmount = (int)this.txtAmountTo.Value;
            }


            foreach (DevExpress.XtraEditors.Controls.CheckedListBoxItem item in this.checkListFolderSearch.CheckedItems)
            {
                if (item.Value.ToString() == "ALL")
                {
                    listFolderSearch = null;
                    break;
                }
                listFolderSearch.Add(((Folder)item.Value).Id.ToString());
            }
            List<Account> listAccountResult = new List<Account>();
            for (int i = 0; i < listAccountSearch.Count; i++)
            {
                var account = listAccountSearch[i];
                try
                {
                    if (listFolderSearch != null && listFolderSearch.Count > 0)
                    {
                        if (!listFolderSearch.Contains(account.IdFolder.ToString()))
                        {
                            continue;
                        }
                    }
                }
                catch (Exception)
                {
                }
                try
                {

                    if (firstAmount != -1)
                    {
                        if (account.amount == null)
                        {
                            continue;
                        }
                        var stringAmount = account.amount.Replace("$", "").Replace("USD", "").Replace("-", "").Replace("+", "").Substring(0, account.amount.Length - 4).Trim();
                        Console.WriteLine(stringAmount);

                        double amountAccount = Double.Parse(stringAmount);
                        if (firstAmount == 0 && lastAmount == 0)
                        {
                            if (amountAccount != firstAmount)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            if (amountAccount < firstAmount || amountAccount >= lastAmount)
                            {
                                continue;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
                listAccountResult.Add(account);

            }
            this.tableAccount.Rows.Clear();

            if (listAccountResult.Count > 0)
            {
                var settingHeader = Settings.Default.headerTable.Split(',');
                for (int i = 0; i < listAccountResult.Count; i++)
                {
                    var data = new List<string>();
                    data.Add((i + 1).ToString());
                    var account = listAccountResult[i];
                    Type myType = account.GetType();
                    IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());




                    foreach (var headerName in settingHeader)
                    {
                        foreach (PropertyInfo prop in props)
                        {
                            if (headerName == prop.Name)
                            {
                                string propValue = (string)prop.GetValue(account, null);

                                if (headerName == "status" && propValue != null && propValue.Trim() != null)
                                {
                                    DateTime timeEnd = DateTime.ParseExact(propValue, "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                                    if (timeEnd > DateTime.Now)
                                    {
                                        TimeSpan t = timeEnd.Subtract(DateTime.Now);
                                        propValue = t.ToString(@"dd\.hh\:mm\:ss");
                                    }
                                    else
                                    {
                                        propValue = "";
                                    }
                                }
                                if (headerName == "mailReceive")
                                {
                                    propValue = account.subEmail;
                                }

                                data.Add(propValue);
                                break;
                            }
                        }
                    }

                    this.tableAccount.Rows.Add(data.ToArray());
                }
            }

        }

        private void cbbSearchAmount_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!firstLoad)
            {
                this.FilterAccount();
            }
        }

        private void checkChromeProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void nângCấpBusinessToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void đăngNhậpEmailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.startAction(ActionRun.LOGIN_MAIL);
        }

        private void đổiNgônNgữEnglishToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void đổiNgônNgữViệtNamToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void đổiMậtKhẩuPaypalToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void lịchSửToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.onCopy(new string[] { "history" });
        }

        private void iDGiaoDịchToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.onCopy(new string[] { "idTransaction" });
        }

        private void emailNhậnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.onCopy(new string[] { "mailReceive" });
        }

        private void tênEmailLịchSửToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.onCopy(new string[] { "name", "email", "history" });
        }

        private void tạoCâuHỏiBMVà2FAToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.startAction(ActionRun.CREATE_SECURE_QUESTION);
        }

        private void xóa2FAToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.startAction(ActionRun.REMOVE_2FA);
        }

        private void nângCấpBusinessToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.startAction(ActionRun.UPGRADTE_BUSINESS);
        }

        private void đổiNgônNgữTiếngAnhToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.startAction(ActionRun.CHANGE_LANGUAGE_EN);
        }

        private void đổiNgônNgữTiếngViệtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.startAction(ActionRun.CHANGE_LANGUAGE_VI);
        }

        private void xóaTàiKhoảnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var check = MessageBox.Show("Xác nhận xóa những tài khoản đã chọn", "Xác nhận", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (check == DialogResult.OK)
                {

                    var selectedRows = this.tableAccount.SelectedRows
                   .OfType<DataGridViewRow>()
                   .Where(row => !row.IsNewRow)
                   .ToArray().Reverse();
                    var listEmail = new List<string>();
                    foreach (var row in selectedRows)
                    {
                        string email = (string)row.Cells[2].Value;
                        listEmail.Add(email);
                        var profilePath = Path.Combine(Settings.Default.profilePath, email);
                        if (Directory.Exists(profilePath))
                        {
                            Directory.Delete(profilePath, true);
                        }
                    }
                    this.myDatabase.deleteAccountByEmail(listEmail);

                    this.getAccount();
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void xóaChromeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void danhSáchToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void xóaChromeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var selectedRows = this.tableAccount.SelectedRows
              .OfType<DataGridViewRow>()
              .Where(row => !row.IsNewRow)
              .ToArray().Reverse();
            var listEmail = new List<string>();
            foreach (var row in selectedRows)
            {
                string email = (string)row.Cells[2].Value;
                var profilePath = Path.Combine(Settings.Default.profilePath, email);
                if (Directory.Exists(profilePath))
                {
                    Directory.Delete(profilePath, true);
                }
            }

        }

        private void mởThưMụcToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedRows = this.tableAccount.SelectedRows
              .OfType<DataGridViewRow>()
              .Where(row => !row.IsNewRow)
              .ToArray().Reverse();
            var listEmail = new List<string>();
            foreach (var row in selectedRows)
            {
                string email = (string)row.Cells[2].Value;
                var profilePath = Path.Combine(Settings.Default.ppvnPath, email);
                if (!Directory.Exists(profilePath))
                {
                    Directory.CreateDirectory(profilePath);
                }
                Process.Start(profilePath);
            }
        }



        private void tạoRequestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModalCreateRequest modalCreateRequest = new ModalCreateRequest(this);
            modalCreateRequest.ShowDialog();
        }

        private void hủyRequestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.startAction(ActionRun.CANCEL_REQUEST);
        }

        public void createRequest(int amount, string email, string note)
        {
            this.myController.setDataRequest(amount, email, note);
            this.startAction(ActionRun.CREATE_REQUEST);
        }

        private void checkLịchSửKhôngProxyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.startAction(ActionRun.CHECK_HISTORY_NO_PROXY);
        }

        private void dánProxyToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }
        public void PasteProxy(List<string> listProxy)
        {
            Account accountSelected = null;
            var selectedRows = this.tableAccount.SelectedRows
           .OfType<DataGridViewRow>()
           .Where(row => !row.IsNewRow)
           .ToArray().Reverse();
            var listEmail = new List<string>();
            foreach (var row in selectedRows)
            {
                string email = (string)row.Cells[2].Value;
                listEmail.Add(email);
            }
            this.myDatabase.updateProxyAccount(listEmail, listProxy);
        }

        public void SetNameBusiness(List<string> listName)
        {
            Account accountSelected = null;
            var selectedRows = this.tableAccount.SelectedRows
           .OfType<DataGridViewRow>()
           .Where(row => !row.IsNewRow)
           .ToArray().Reverse();
            var listEmail = new List<string>();
            foreach (var row in selectedRows)
            {
                string email = (string)row.Cells[2].Value;
                listEmail.Add(email);
            }
            this.myDatabase.updateNameBusiness(listEmail, listName);
        }


        private void txtAmountFrom_ValueChanged(object sender, EventArgs e)
        {
            if (!firstLoad)
            {
                this.FilterAccount();
            }
        }

        private void txtAmountTo_ValueChanged(object sender, EventArgs e)
        {
            if (!firstLoad)
            {
                this.FilterAccount();
            }
        }

        private void txtAmountTo_Validating(object sender, CancelEventArgs e)
        {
        }

        private void cbAllFolder_CheckedChanged(object sender, EventArgs e)
        {
            this.searchAccount();
        }

        private void btnReload_Click_1(object sender, EventArgs e)
        {
            this.getAccount();
        }

        private void btnOrderRowTable_Click(object sender, EventArgs e)
        {
            this.getAccount(true);
        }

        private void đổiSĐTToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        public void changePhone(List<string> listPhone)
        {
            this.startAction(ActionRun.CHANGE_PHONE, listPhone);
        }

        private void tableAccount_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Console.WriteLine(" ######### ");
        }

        private void thêmEmailToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void sendFailToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void hToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.setTime(48);

        }

        private void hToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.setTime(24);
        }

        private void hToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            this.setTime(72);
        }

        public void setTime(int time)
        {
            Account accountSelected = null;
            var selectedRows = this.tableAccount.SelectedRows
           .OfType<DataGridViewRow>()
           .Where(row => !row.IsNewRow)
           .ToArray().Reverse();
            var listEmail = new List<string>();
            foreach (var row in selectedRows)
            {
                string email = (string)row.Cells[2].Value;
                listEmail.Add(email);
            }
            var timeEnd = DateTime.Now;
            timeEnd = timeEnd.AddHours(time);
            string status = timeEnd.ToString("dd/MM/yyyy HH:mm");

            this.myDatabase.updateAccountSetStatus(listEmail, status);

            foreach (var email in listEmail)
            {
                Account account = this.listAccount.FirstOrDefault(x => x.email == email);
                account.status = status;
                this.updateTableAccount(account);
            }
        }

        private void checkChromeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.startAction(ActionRun.CHECK_CHROME);
        }

        private void đổiEmailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.startAction(ActionRun.CHANGE_MAIL);
        }

        private void đổiSốĐiệnThoạiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedRows = this.tableAccount.SelectedRows
             .OfType<DataGridViewRow>()
             .Where(row => !row.IsNewRow)
             .ToArray().Reverse().ToArray().Length;

            ModalChangePhone modalChangePhone = new ModalChangePhone(selectedRows, this);
            modalChangePhone.ShowDialog();
        }

        private void dánProxyToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            var selectedRows = this.tableAccount.SelectedRows
             .OfType<DataGridViewRow>()
             .Where(row => !row.IsNewRow)
             .ToArray().Reverse().ToArray().Length;

            ModalPasteProxy modalPasteProxy = new ModalPasteProxy(selectedRows, this);
            modalPasteProxy.ShowDialog();
        }

        private void đổiMậtKhẩuPPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.startAction(ActionRun.CHANGE_PASSWORD);
        }

        private void btnSetupMail_Click(object sender, EventArgs e)
        {
            ModalChangMailMien modalChangMailMien = new ModalChangMailMien(this);
            modalChangMailMien.ShowDialog();
        }

        private void xóaCookieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedRows = this.tableAccount.SelectedRows
           .OfType<DataGridViewRow>()
           .Where(row => !row.IsNewRow)
           .ToArray().Reverse();
            var listEmail = new List<string>();
            foreach (var row in selectedRows)
            {
                string email = (string)row.Cells[2].Value;
                listEmail.Add(email);
            }
            this.myDatabase.removeCookieOfAccount(listEmail);
            this.getAccount();
        }

        private void xuấtAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedRows = this.tableAccount.SelectedRows
         .OfType<DataGridViewRow>()
         .Where(row => !row.IsNewRow)
         .ToArray().Reverse();
            var listEmail = new List<Account>();
            foreach (var row in selectedRows)
            {
                string email = (string)row.Cells[2].Value;
                listEmail.Add(this.listAccount.FirstOrDefault(x => x.email == email));
            }
            var data = JsonConvert.SerializeObject(listEmail);
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.DefaultExt = "txt";
            saveFileDialog1.Filter = "Json File|*.json|All files (*.*)|*.*";
            saveFileDialog1.AddExtension = true;
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.Title = "Where do you want to save the file?";
            saveFileDialog1.InitialDirectory = @"C:/";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog1.FileName, data);
            }

        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"D:\",
                Title = "Browse Json Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "json",
                Filter = "Json File (*.json)|*.json",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var data = File.ReadAllText(openFileDialog1.FileName);
                List<Account> list = JsonConvert.DeserializeObject<List<Account>>(data).Where(item => item != null).ToList();
                foreach (var item in list)
                {
                    item.IdFolder = this.folderSelected.Id;
                }
                this.myDatabase.addNewListAccount(list);
                this.getAccount();
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            ModalRegAccount modalRegAccount = new ModalRegAccount(this.myController);
            modalRegAccount.ShowDialog();
        }

        private void cbUseGologin_CheckedChanged(object sender, EventArgs e)
        {
            //Settings.Default.useGologin = this.cbUseGologin.Checked;
            //Settings.Default.Save();
        }

        private void tạoHotmailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.startAction(ActionRun.CREATE_HOSTMAIL);
        }

        private void upInfoXácMinhTkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.startAction(ActionRun.VERIFY_ACCOUNT);
        }

        private void verifyHotMailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.startAction(ActionRun.VERIFY_HOTMAIL);
        }

        private void chuyểnEmailChínhPhụToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.startAction(ActionRun.CHANGE_EMAIL_CHINH_PHU);
        }

        private void tạoHotmailVerifyMailPhụToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.startAction(ActionRun.CREATE_HOSTMAIL_2);
        }

        private void cbAutoCloseChrome_CheckedChanged_1(object sender, EventArgs e)
        {
            Settings.Default.autoCloseChrome = this.cbAutoCloseChrome.Checked;
            Settings.Default.Save();
        }

        private void txtThreadCount_ValueChanged_1(object sender, EventArgs e)
        {
            Settings.Default.threadCount = (int)txtThreadCount.Value;
            Settings.Default.Save();
        }

        private void tạo2FAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.startAction(ActionRun.CREATE_SECURE_2FA);
        }

        private void tạoThêmHotmailChínhToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.startAction(ActionRun.CREATE_HOSTMAIL_3);
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void setTênBusinessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedRows = this.tableAccount.SelectedRows
            .OfType<DataGridViewRow>()
            .Where(row => !row.IsNewRow)
            .ToArray().Reverse().ToArray().Length;

            ModalSetNameBusiness modalSetNameBusiness = new ModalSetNameBusiness(selectedRows, this);
            modalSetNameBusiness.ShowDialog();
        }
    }
}
