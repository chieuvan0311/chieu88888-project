using HtmlAgilityPack;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using PaypalAccountManager.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PaypalAccountManager.Controller
{
    public class Mail
    {

        public static string Between(string STR, string FirstString, string LastString)
        {
            string FinalString;
            int Pos1 = STR.IndexOf(FirstString) + FirstString.Length;
            int Pos2 = STR.IndexOf(LastString);
            FinalString = STR.Substring(Pos1, Pos2 - Pos1);
            return FinalString;
        }

        public static string GetUrlAuth(string email, DateTime timeMail)
        {
            string url_auth = "";
            var time_out = 3;
            while (time_out > 0 && url_auth == "")
            {
                try
                {
                    using (var client = new ImapClient())
                    {
                        using (var cancel = new CancellationTokenSource())
                        {
                            client.Connect(Settings.Default.mailHost, 993, true, cancel.Token);
                            client.Authenticate(Settings.Default.mailUsername, Settings.Default.mailPassword, cancel.Token);
                            var inbox = client.Inbox;
                            inbox.Open(FolderAccess.ReadOnly, cancel.Token);
                            var query = SearchQuery.ToContains(email);
                            var inboxSearch = inbox.Search(query, cancel.Token).ToList();
                            for (int i = inboxSearch.Count - 1; i >= 0 ; i--)
                            {
                                var message = inbox.GetMessage(inboxSearch[i], cancel.Token);
                                 DateTimeOffset d = message.Date;
                                d = d.AddHours(14);
                                if (d.DateTime >= timeMail)
                                {
                                    HtmlDocument htmlDocument = new HtmlDocument();
                                    htmlDocument.LoadHtml(message.HtmlBody);
                                    HtmlNode nbode = htmlDocument.QuerySelector("#button_text > span > a");
                                    if (nbode == null)
                                    {
                                        nbode = htmlDocument.QuerySelector("table.neptuneButtonwhite > tbody > tr > td > table > tbody > tr > td > a");
                                    }
                                    url_auth = nbode.GetAttributeValue("href", "").Replace("amp;", "");
                                    break;
                                }
                              
                            }
                            client.Disconnect(true, cancel.Token);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
                if (url_auth != "")
                {
                    break;
                }
                Thread.Sleep(5000);
                time_out--;
            }
            return url_auth;
        }
    }
}
