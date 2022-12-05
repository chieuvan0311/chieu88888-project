using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PaypalAccountManager.Entity
{
    public class DataApp
    {
        public static List<HeaderTable> listHeaderDefault = new List<HeaderTable>();

        public static void loadHeader()
        {
            HeaderTable stt = new HeaderTable();
            stt.Name = "stt";
            stt.HeaderText = "STT";
            stt.MinimumWidth = 10;
            stt.FillWeight = 50;
            listHeaderDefault.Add(stt);

            HeaderTable name = new HeaderTable();
            name.Name = "name";
            name.HeaderText = "Tên TK";
            name.MinimumWidth = 10;
            name.FillWeight = 50;
            listHeaderDefault.Add(name);

            HeaderTable email = new HeaderTable();
            email.Name = "email";
            email.HeaderText = "Email";
            email.MinimumWidth = 10;
            email.FillWeight = 50;
            listHeaderDefault.Add(email);

            HeaderTable password = new HeaderTable();
            password.Name = "password";
            password.HeaderText = "Mk Paypal";
            password.MinimumWidth = 10;
            password.FillWeight = 50;
            listHeaderDefault.Add(password);

            HeaderTable passwordEmail = new HeaderTable();
            passwordEmail.Name = "passwordEmail";
            passwordEmail.HeaderText = "MK Email";
            passwordEmail.MinimumWidth = 10;
            passwordEmail.FillWeight = 50;

            listHeaderDefault.Add(passwordEmail);
            HeaderTable amount = new HeaderTable();
            amount.Name = "amount";
            amount.HeaderText = "Số dư";
            amount.MinimumWidth = 10;
            amount.FillWeight = 50;
            listHeaderDefault.Add(amount);

            HeaderTable history = new HeaderTable();
            history.Name = "history";
            history.HeaderText = "Lịch sử";
            history.MinimumWidth = 10;
            history.FillWeight = 50;
            listHeaderDefault.Add(history);



            HeaderTable resultLimit = new HeaderTable();
            resultLimit.Name = "resultLimit";
            resultLimit.HeaderText = "KT Limits";
            resultLimit.MinimumWidth = 10;
            resultLimit.FillWeight = 50;
            listHeaderDefault.Add(resultLimit);

            HeaderTable countTransaction = new HeaderTable();
            countTransaction.Name = "countTransaction";
            countTransaction.HeaderText = "Số giao dịch";
            countTransaction.MinimumWidth = 10;
            countTransaction.FillWeight = 50;
            listHeaderDefault.Add(countTransaction);

            HeaderTable status = new HeaderTable();
            status.Name = "status";
            status.HeaderText = "Send Failure";
            status.MinimumWidth = 10;
            status.FillWeight = 50;
            listHeaderDefault.Add(status);

            HeaderTable idTransaction = new HeaderTable();
            idTransaction.Name = "idTransaction";
            idTransaction.HeaderText = "ID giao dịch";
            idTransaction.MinimumWidth = 10;
            idTransaction.FillWeight = 50;
            listHeaderDefault.Add(idTransaction);

            HeaderTable mailReceive = new HeaderTable();
            mailReceive.Name = "mailReceive";
            mailReceive.HeaderText = "Mail nhận";
            mailReceive.MinimumWidth = 10;
            mailReceive.FillWeight = 50;
            listHeaderDefault.Add(mailReceive);

            HeaderTable dateCreate = new HeaderTable();
            dateCreate.Name = "dateCreate";
            dateCreate.HeaderText = "Ngày tạo";
            dateCreate.MinimumWidth = 10;
            dateCreate.FillWeight = 50;
            listHeaderDefault.Add(dateCreate);

            HeaderTable address = new HeaderTable();
            address.Name = "address";
            address.HeaderText = "Địa chỉ";
            address.MinimumWidth = 10;
            address.FillWeight = 50;
            listHeaderDefault.Add(address);

            HeaderTable cmndNumber = new HeaderTable();
            cmndNumber.Name = "cmndNumber";
            cmndNumber.HeaderText = "Số CMND";
            cmndNumber.MinimumWidth = 10;
            cmndNumber.FillWeight = 50;
            listHeaderDefault.Add(cmndNumber);

            HeaderTable birthday = new HeaderTable();
            birthday.Name = "birthday";
            birthday.HeaderText = "Ngày sinh";
            birthday.MinimumWidth = 10;
            birthday.FillWeight = 50;
            listHeaderDefault.Add(birthday);

            HeaderTable phone = new HeaderTable();
            phone.Name = "phone";
            phone.HeaderText = "Số phone";
            phone.MinimumWidth = 10;
            phone.FillWeight = 50;
            listHeaderDefault.Add(phone);

            HeaderTable note = new HeaderTable();
            note.Name = "note";
            note.HeaderText = "Note tạm";
            note.MinimumWidth = 10;
            note.FillWeight = 50;
            listHeaderDefault.Add(note);

            HeaderTable proxy = new HeaderTable();
            proxy.Name = "proxy";
            proxy.HeaderText = "IP Proxy";
            proxy.MinimumWidth = 10;
            proxy.FillWeight = 50;
            listHeaderDefault.Add(proxy);

            HeaderTable ttVeryMail = new HeaderTable();
            ttVeryMail.Name = "ttVeryMail";
            ttVeryMail.HeaderText = "TT Very Mail";
            ttVeryMail.MinimumWidth = 10;
            ttVeryMail.FillWeight = 50;
            listHeaderDefault.Add(ttVeryMail);

            HeaderTable securityQuestion = new HeaderTable();
            securityQuestion.Name = "securityQuestion";
            securityQuestion.HeaderText = "Câu hỏi BM";
            securityQuestion.MinimumWidth = 10;
            securityQuestion.FillWeight = 50;
            listHeaderDefault.Add(securityQuestion);

            HeaderTable twoFA = new HeaderTable();
            twoFA.Name = "twoFA";
            twoFA.HeaderText = "2FA";
            twoFA.MinimumWidth = 10;
            twoFA.FillWeight = 50;
            listHeaderDefault.Add(twoFA);

            HeaderTable cookie = new HeaderTable();
            cookie.Name = "cookie";
            cookie.HeaderText = "Cookie";
            cookie.MinimumWidth = 10;
            cookie.FillWeight = 50;
            listHeaderDefault.Add(cookie);

            HeaderTable resultKTGD = new HeaderTable();
            resultKTGD.Name = "resultKTGD";
            resultKTGD.HeaderText = "Kết Quả KTGD";
            resultKTGD.MinimumWidth = 10;
            resultKTGD.FillWeight = 50;
            listHeaderDefault.Add(resultKTGD);

            HeaderTable log = new HeaderTable();
            log.Name = "log";
            log.HeaderText = "Log";
            log.MinimumWidth = 10;
            log.FillWeight = 50;
            listHeaderDefault.Add(log);


        }

        public static void Init()
        {
            loadHeader();
        }
        public static string randomUserAgent()
        {
            var listUA = File.ReadAllLines(Path.Combine(Application.StartupPath, "user_agent.txt")).Where(item => item.Trim() != "").ToArray();
            var randomNum = new Random().Next(0, listUA.Length);
            return listUA[randomNum];
        }
    }
}
