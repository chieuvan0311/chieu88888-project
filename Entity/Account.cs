using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaypalAccountManager.Entity
{
    public class Account
    {
        public int Id { get; set; }
        public int IdFolder { get; set; }
        public string businessName { get; set; }
        public string userAgent { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string passwordEmail { get; set; }
        public string amount { get; set; }
        public string history { get; set; }
        public string resultKTGD { get; set; }
        public string resultLimit { get; set; }
        public string countTransaction { get; set; }
        public string status { get; set; }
        public string idTransaction { get; set; }
        public string mailReceive { get; set; }
        public string dateCreate { get; set; }
        public string address { get; set; }
        public string cmndNumber { get; set; }
        public string birthday { get; set; }
        public string phone { get; set; }
        public string note { get; set; }
        public string proxy { get; set; }
        public string ttVeryMail { get; set; }
        public string securityQuestion { get; set; }
        public string twoFA { get; set; }
        public string cookie { get; set; }
        public string cookieHotMail { get; set; }
        public string log { get; set; }
        public bool isDelete { get; set; }
        public string HistoryPassword { get; set; }
        public string subEmail { get; set; }

        public override string ToString()
        {
            return this.name + "|" + this.email + "|" + this.password + "|" + this.passwordEmail + "|"  + this.address + "|" + this.cmndNumber
                + "|" + this.birthday + "|" + this.phone + "|" + this.note + "|" + this.proxy + "|" + this.securityQuestion + "|"
                + this.twoFA + "|" + this.cookie;
          //  return this.name + "|" + this.email + "|" + this.password + "|" + this.passwordEmail + "|" + this.amount + "|" + this.history + "|" + this.resultKTGD + "|" + this.resultLimit + "|"
          //+ this.countTransaction + "|" + this.status + "|" + this.idTransaction + "|" + this.mailReceive + "|" + this.dateCreate + "|" + this.address + "|" + this.cmndNumber
          //+ "|" + this.birthday + "|" + this.phone + "|" + this.note + "|" + this.proxy + "|" + this.ttVeryMail + "|" + this.securityQuestion + "|"
          //+ this.twoFA + "|" + this.cookie;
        }
    }
}
