using LiteDB;
using PaypalAccountManager.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaypalAccountManager.Sesrvices
{
    public class MyDatabase
    {
        public string dbPath;
        LiteDatabase db = null;
        public MyDatabase(string dbPath)
        {
            this.dbPath = dbPath;
            this.db = new LiteDatabase(this.dbPath);
        }
        public void closeDB()
        {
            db.Dispose();
        }
        public void loadDatabase(string dbPath)
        {
            this.dbPath = dbPath;
            this.db = new LiteDatabase(this.dbPath);
        }
        public List<Folder> getAllFolders()
        {
            var folders = db.GetCollection<Folder>("folders");
            var listFolder = folders.FindAll().ToList();
            foreach (var folder in listFolder)
            {
                var listAccount = this.getAccounts(folder.Id);
                var totalAmount = 0;
                foreach (var account in listAccount)
                {
                    int amountAc = 0;
                    string amount = account.amount;
                    if (amount != null)
                    {
                        amount = amount.Replace("$", "").Split(',')[0].Split('.')[0];
                        if (Int32.TryParse(amount, out amountAc))
                        {
                            totalAmount += amountAc;
                        }
                    }

                }
                folder.name += " - " + listAccount.Count + " - " + totalAmount + "$";
            }
            return listFolder;
        }

        public bool updateFolder(int id, string name)
        {
            var folders = db.GetCollection<Folder>("folders");
            var folder = folders.FindById(id);
            folder.name = name;
            folders.Update(folder);
            return true;
        }
        public bool deleteFolder(int id)
        {
            var folders = db.GetCollection<Folder>("folders");
            folders.Delete(id);
            var accounts = db.GetCollection<Account>("accounts");
            accounts.UpdateMany("{isDelete: true}", BsonExpression.Create("IdFolder = " + id));
            return true;
        }
        public bool addNewFolder(string name)
        {
            var folders = db.GetCollection<Folder>("folders");
            folders.Insert(new Folder { name = name });
            return true;

        }
        public bool addNewAccount(Account account)
        {
            var accounts = db.GetCollection<Account>("accounts");
            accounts.Insert(account);
            return true;
        }

        public bool addNewListAccount(List<Account> listAccount)
        {
            listAccount = listAccount.GroupBy(x => x.email).Select(x => x.First()).ToList();

            var accounts = db.GetCollection<Account>("accounts");
            List<Account> newListAccount = new List<Account>();
            foreach (var item in listAccount)
            {
                var checkExist = accounts.FindOne(x => x.email == item.email && !x.isDelete);
                if (checkExist == null)
                {
                    item.Id = 0;
                    newListAccount.Add(item);
                }
            }
            if (newListAccount.Count > 0)
            {
                accounts.InsertBulk(newListAccount);
            }
            return true;
        }

        public List<Account> getAllAccountDeleted()
        {
            List<Account> listAccount = new List<Account>();
            var accounts = db.GetCollection<Account>("accounts");
            listAccount = accounts.Find(item => item.isDelete).ToList();
            return listAccount;
        }

        public List<Account> getAllAccount()
        {
            List<Account> listAccount = new List<Account>();
            var accounts = db.GetCollection<Account>("accounts");
            var xxx = accounts.FindAll();
            //xxx = accounts.Find(item => item.IdFolder == idFolder && !item.isDelete).ToList();
            try
            {
                foreach (var item in xxx)
                {
                    try
                    {
                        if (!item.isDelete)
                        {
                            listAccount.Add(item);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception)
            {
            }
            return listAccount;
        }

        public List<Account> getAccounts(int idFolder)
        {
            List<Account> listAccount = new List<Account>();
            var accounts = db.GetCollection<Account>("accounts");

            var xxx = accounts.FindAll();
            //xxx = accounts.Find(item => item.IdFolder == idFolder && !item.isDelete).ToList();

            try
            {
                foreach (var item in xxx)
                {
                    try
                    {
                        if (item.IdFolder == idFolder && !item.isDelete)
                        {
                            listAccount.Add(item);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception)
            {
            }
            return listAccount;
        }

        public Account findAccountByEmail(string email)
        {
            var accounts = db.GetCollection<Account>("accounts");
            Account account = accounts.FindOne(item => item.email == email && !item.isDelete);
            return account;
        }

        public Account updateAccount(Account account)
        {
            var accounts = db.GetCollection<Account>("accounts");
            accounts.Update(account);
            return account;
        }

        public void removeAccountByEmail(List<string> listAccount)
        {
            var accounts = db.GetCollection<Account>("accounts");
            accounts.DeleteMany(x => listAccount.Contains(x.email) && x.isDelete);
        }


        public void deleteAccountByEmail(List<string> listAccount)
        {
            //var accounts = db.GetCollection<Account>("accounts");
            //accounts.DeleteMany(x => listAccount.Contains(x.email) && !x.isDelete);

            var accounts = db.GetCollection<Account>("accounts");
            accounts.UpdateMany(x => new Account { isDelete = true, }, x => listAccount.Contains(x.email)); ;
        }

        public void updateChangeFolder(List<string> listEmail, int folderId)
        {
            var accounts = db.GetCollection<Account>("accounts");
            accounts.UpdateMany(x => new Account { IdFolder = folderId, }, x => listEmail.Contains(x.email)); ;
        }

        public void updateAccountSetStatus(List<string> listEmail, string status)
        {
            var accounts = db.GetCollection<Account>("accounts");
            accounts.UpdateMany(x => new Account { status = status, }, x => listEmail.Contains(x.email)); ;
        }


        public void updateNameBusiness(List<string> listEmail, List<string> listName)
        {
            var accounts = db.GetCollection<Account>("accounts");

            for (int i = 0; i < listEmail.Count && i < listName.Count; i++)
            {
                string email = listEmail[i];
                var account = accounts.FindOne(item => item.email == email && !item.isDelete);
                account.businessName = listName[i];
                accounts.Update(account);
            }
        }
        public void updateProxyAccount(List<string> listEmail, List<string> listProxy)
        {
            var accounts = db.GetCollection<Account>("accounts");

            for (int i = 0; i < listEmail.Count && i < listProxy.Count; i++)
            {
                string email = listEmail[i];
                var account = accounts.FindOne(item => item.email == email && !item.isDelete);
                account.proxy = listProxy[i];
                accounts.Update(account);
            }
        }

        public void removeCookieOfAccount(List<string> listEmail)
        {
            var accounts = db.GetCollection<Account>("accounts");
            accounts.UpdateMany(x => new Account { cookie = "" }, x => listEmail.Contains(x.email)); ;
        }
    }
}
