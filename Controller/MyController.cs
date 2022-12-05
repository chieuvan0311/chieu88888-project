using _2CaptchaAPI;
using AnyCaptchaHelper;
using Ionic.Zip;
using LiteDB;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using PaypalAccountManager.Entity;
using PaypalAccountManager.Properties;
using PaypalAccountManager.Sesrvices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TwoFactorAuthNet;
using xNet;
using OtpSharp;
using GPMSharedLibrary.V2.Models;
using SeleniumExtras.WaitHelpers;

namespace PaypalAccountManager.Controller
{
    public class ResultLogin
    {
        public IWebDriver driver { get; set; }
        public string status { get; set; }
        public string message { get; set; }
        public string cookie { get; set; }
        public string newPassword { get; set; }
        public Gologin gologin { get; set; }
    }
    public class DataRequest
    {
        public int amount { get; set; }
        public string email { get; set; }
        public string note { get; set; }

    }
    public class DataStartDriver
    {
        public IWebDriver driver { get; set; }
        public Gologin gologin { get; set; }

    }
    public class MyController
    {
        private const byte IntegerTag = 0x02;
        private const byte BitStringTag = 0x03;
        private const byte SequenceTag = 0x30;
        private const byte NullTag = 0x50;
        public int totalRunning = 0;
        Form1 form1 = null;
        ModalRegAccount formRegAcc = null;

        public MyDatabase myDatabase = null;
        Thread threadMain = null;
        Thread threadRegAccount = null;
        List<Thread> listChildThreadRegAccount = new List<Thread>();

        List<string> listProfileIDRunning = null;
        List<Thread> listChildThread = new List<Thread>();
        DataRequest dataRequest = null;
        List<IWebDriver> listDriver = new List<IWebDriver>();
        string publicIP = "";

        public MyController(MyDatabase myDatabase, Form1 form1, string publicIP)
        {
            this.myDatabase = myDatabase;
            this.form1 = form1;
            this.publicIP = publicIP;
        }

        public string getBetween(string STR, string FirstString, string LastString)
        {
            string FinalString;
            int Pos1 = STR.IndexOf(FirstString) + FirstString.Length;
            int Pos2 = STR.IndexOf(LastString);
            FinalString = STR.Substring(Pos1, Pos2 - Pos1);
            return FinalString;
        }

        public void setListPhone(List<string> listPhone)
        {
        }

        public string Get2FA(string sercret)
        {
            var code = new HttpRequest().Get("https://2fa.live/tok/" + sercret).ToString();
            return code.Replace("{\"token\":\"", "").Replace("\"}", "");
        }
        public string resolveFunCaptcha(string keySite, string website)
        {
            var recaptchaV2ProxylessRequest = new AnyCaptcha().FunCaptchaProxyless("3124648601484eab8fa78500df71e30f", keySite, website);

            if (recaptchaV2ProxylessRequest.IsSuccess)
                Console.WriteLine(recaptchaV2ProxylessRequest.Result);
            else
                throw new Exception("Anycaptcha Lỗi: " + recaptchaV2ProxylessRequest.Message);
            var resultCode = recaptchaV2ProxylessRequest.Result;
            return resultCode;
        }
        public void verifyMailPaypal(IWebDriver driver, Account account)
        {
            driver.Url = "https://www.paypal.com/myaccount/settings/";
            Thread.Sleep(2000);
            var btnEditMail = this.findElementBy(driver, By.CssSelector("ul.emails.unstyled.lined"));
            try
            {
                btnEditMail.Click();
            }
            catch (Exception)
            {
                Thread.Sleep(1000);
                btnEditMail.Click();
            }
            Thread.Sleep(2000);

            var btnConfirm = this.findElementBy(driver, By.CssSelector("a[class=\"confirm visible js_confirm-message\"]"), 3);
            if (btnConfirm != null)
            {
                btnConfirm.Click();
            }
            else
            {
                Thread.Sleep(5000);
                btnConfirm = this.findElementBy(driver, By.CssSelector("a[class=\"confirm visible js_confirm-message\"]"), 3);
                if (btnConfirm != null)
                {
                    btnConfirm.Click();
                }
                else
                {
                    Thread.Sleep(5000);
                    btnConfirm = this.findElementBy(driver, By.CssSelector("a[class=\"confirm visible js_confirm-message\"]"), 3);
                    if (btnConfirm != null)
                    {
                        btnConfirm.Click();
                    }
                }
            }
            if (btnConfirm == null)
            {
                throw new Exception("btnConfirm not found!");
            }
            driver.Url = "https://outlook.live.com/mail/0/";
            var timeout = 30;
            while (true)
            {
                var checkLogin = this.findElementByWithoutError(driver, By.CssSelector("a[data-task=\"signin\"]"), 1);
                if (checkLogin != null)
                {
                    checkLogin.Click();


                }

                var inputEmail = this.findElementByWithoutError(driver, By.CssSelector("input[name=\"loginfmt\"]"), 1);
                if (inputEmail != null)
                {
                    inputEmail.SendKeys(account.email);
                    Thread.Sleep(1000);
                    this.findElementByWithoutError(driver, By.CssSelector("input[id=\"idSIButton9\"]"), 10).Click();
                    Thread.Sleep(1000);
                    this.findElementByWithoutError(driver, By.CssSelector("input[name=\"passwd\"]"), 10).SendKeys(account.password);
                    Thread.Sleep(1000);
                    this.findElementByWithoutError(driver, By.CssSelector("input[id=\"idSIButton9\"]"), 10).Click();
                    Thread.Sleep(1000);
                    this.findElementByWithoutError(driver, By.Name("DontShowAgain"), 30).Click();
                    Thread.Sleep(500);
                    this.findElementByWithoutError(driver, By.CssSelector("input[id=\"idSIButton9\"]"), 10).Click();
                    Thread.Sleep(1000);
                }

                var checkLoginPaypal = this.findElementByWithoutError(driver, By.CssSelector("input[name=\"login_password\"]"), 1);
                if (checkLoginPaypal != null)
                {
                    checkLoginPaypal.SendKeys(account.password);
                    new WebDriverWait(driver, TimeSpan.FromSeconds(60)).Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[name=\"btnLogin\"]"))).Click();
                }

                var mailPaypal = this.findElementByWithoutError(driver, By.CssSelector("div[aria-label*=\"PayPal\"]"), 1);
                if (mailPaypal != null && !driver.Url.Contains("paypal"))
                {

                    try
                    {
                        var btnCloseAlert = this.findElementByWithoutError(driver, By.CssSelector("div.ms-Callout-main > div > div:nth-child(2) > button"), 1);
                        if (btnCloseAlert != null)
                        {
                            btnCloseAlert.Click();
                        }

                        mailPaypal.Click();
                        var confirmEle = this.findElementByWithoutError(driver, By.CssSelector("a[href*=\"email-confirmation\"]"), 3);
                        if (confirmEle != null)
                        {
                            var urlConfirm = confirmEle.GetAttribute("href");
                            driver.Url = urlConfirm;
                            this.findElementByWithoutError(driver, By.CssSelector("#password"), 3).SendKeys(account.password);
                            Thread.Sleep(1000);
                            new WebDriverWait(driver, TimeSpan.FromSeconds(60)).Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[name=\"btnLogin\"]"))).Click();

                        }
                    }
                    catch (Exception)
                    {

                    }
                }

                var checkSuccess = this.findElementByWithoutError(driver, By.CssSelector("div.cw_tile-currencyContainer"), 1);

                var checkSuccess1 = this.findElementByWithoutError(driver, By.CssSelector("div.test_balance-tile-currency"), 1);

                if (checkSuccess != null || checkSuccess1 != null)
                {
                    return;
                }
                timeout--;
                Thread.Sleep(1000);
            }

        }
        public void verifyMailPaypalAfterRegHotMail(IWebDriver driver, Account account)
        {
            driver.Url = "https://www.paypal.com/myaccount/settings/";
            Thread.Sleep(2000);
            var btnXacMinh = this.findElementBy(driver, By.CssSelector("a[data-track=\"Confirmemail\"]"));
            if (btnXacMinh != null)
            {
                btnXacMinh.Click();
            }
            else
            {
                throw new Exception("Button Confirm not found");
            }
            Thread.Sleep(2000);
            try
            {
                this.findElementBy(driver, By.CssSelector("a[class=\"confirm visible js_confirm-message\"]")).Click();
            }
            catch (Exception)
            {
                try
                {
                    Thread.Sleep(2000);
                    this.findElementBy(driver, By.CssSelector("a[class=\"confirm visible js_confirm-message\"]")).Click();
                }
                catch (Exception)
                {
                    Thread.Sleep(2000);
                    this.findElementBy(driver, By.CssSelector("a[class=\"confirm visible js_confirm-message\"]")).Click();
                }
            }
            driver.Url = "https://outlook.live.com/mail/0/";
            var timeout = 30;
            while (true)
            {
                var checkLogin = this.findElementByWithoutError(driver, By.CssSelector("a[data-task=\"signin\"]"), 1);
                if (checkLogin != null)
                {
                    checkLogin.Click();


                }

                var inputEmail = this.findElementByWithoutError(driver, By.CssSelector("input[name=\"loginfmt\"]"), 1);
                if (inputEmail != null)
                {
                    inputEmail.SendKeys(account.email);
                    Thread.Sleep(1000);
                    this.findElementByWithoutError(driver, By.CssSelector("input[id=\"idSIButton9\"]"), 10).Click();
                    Thread.Sleep(1000);
                    this.findElementByWithoutError(driver, By.CssSelector("input[name=\"passwd\"]"), 10).SendKeys(account.password);
                    Thread.Sleep(1000);
                    this.findElementByWithoutError(driver, By.CssSelector("input[id=\"idSIButton9\"]"), 10).Click();
                    Thread.Sleep(1000);
                    this.findElementByWithoutError(driver, By.Name("DontShowAgain"), 30).Click();
                    Thread.Sleep(500);
                    this.findElementByWithoutError(driver, By.CssSelector("input[id=\"idSIButton9\"]"), 10).Click();
                    Thread.Sleep(1000);
                }

                var checkLoginPaypal = this.findElementByWithoutError(driver, By.CssSelector("input[name=\"login_password\"]"), 10);
                if (checkLoginPaypal != null)
                {
                    checkLoginPaypal.SendKeys(account.password);
                    new WebDriverWait(driver, TimeSpan.FromSeconds(60)).Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[name=\"btnLogin\"]"))).Click();

                }

                var mailPaypal = this.findElementByWithoutError(driver, By.CssSelector("div[aria-label*=\"PayPal\"]"), 10);
                if (mailPaypal != null && !driver.Url.Contains("paypal"))
                {

                    try
                    {
                        var btnCloseAlert = this.findElementByWithoutError(driver, By.CssSelector("div.ms-Callout-main > div > div:nth-child(2) > button"), 10);
                        if (btnCloseAlert != null)
                        {
                            btnCloseAlert.Click();
                        }

                        mailPaypal.Click();
                        var confirmEle = this.findElementByWithoutError(driver, By.XPath("//*[contains(text(), 'Click to activate your account')]"), 10);
                        if (confirmEle != null)
                        {
                            var urlConfirm = confirmEle.GetAttribute("href");
                            driver.Url = urlConfirm;
                            this.findElementByWithoutError(driver, By.CssSelector("#password"), 10).SendKeys(account.password);
                            Thread.Sleep(1000);
                            new WebDriverWait(driver, TimeSpan.FromSeconds(60)).Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[name=\"btnLogin\"]"))).Click();

                        }
                    }
                    catch (Exception)
                    {

                    }
                }

                var checkSuccess = this.findElementByWithoutError(driver, By.CssSelector("div.cw_tile-currencyContainer"), 1);
                var checkSuccess1 = this.findElementByWithoutError(driver, By.CssSelector("div.test_balance-tile-currency"), 1);

                if (checkSuccess != null || checkSuccess1 != null)
                {
                    return;
                }
                timeout--;
                Thread.Sleep(1000);
            }

        }
        public void regHostMail(IWebDriver driver, Account account, string email, string password, string fname, string lname, string dob)
        {
            try
            {
                driver.Url = "https://signup.live.com/signup?wa=wsignin1.0&rpsnv=13&ct=1634232667&rver=7.0.6737.0&wp=MBI_SSL&wreply=https%3a%2f%2foutlook.live.com%2fowa%2f%3fnlp%3d1%26RpsCsrfState%3d47f46e37-2349-b0fa-381b-4eedf2d1ef56&id=292841&aadredir=1&CBCXT=out&lw=1&fl=dob%2cflname%2cwld&cobrandid=90015&contextid=20B1CE18918203F7&bk=1634232667&uiflavor=web&lic=1&mkt=EN-US&lc=1033&uaid=a16953a4526f42deb3431d2c7ac23a28";

                if (email.EndsWith("hotmail.com"))
                {
                    this.findElementBy(driver, By.CssSelector("#LiveDomainBoxList > option:nth-child(2)"), 60, "Select type mail").Click();
                }
                this.findElementBy(driver, By.CssSelector("#MemberName"), 60, "Input Email").SendKeys(email.Split('@')[0]);
                Thread.Sleep(500);
                this.findElementBy(driver, By.CssSelector("#iSignupAction"), 60, "Btn Next").Click();
                Thread.Sleep(500);
                this.findElementBy(driver, By.CssSelector("#PasswordInput"), 60, "Input password").SendKeys(password);
                Thread.Sleep(500);
                this.findElementBy(driver, By.CssSelector("#iSignupAction"), 60, "Btn Next").Click();
                Thread.Sleep(500);
                this.findElementBy(driver, By.CssSelector("#FirstName"), 60, "Input FName").SendKeys(fname);
                Thread.Sleep(500);
                this.findElementBy(driver, By.CssSelector("#LastName"), 60, "Input LName").SendKeys(lname);
                Thread.Sleep(500);
                this.findElementBy(driver, By.CssSelector("#iSignupAction"), 60, "Btn Next").Click();

                int month = new Random().Next(1, 12);
                int day = new Random().Next(1, 28);
                string year = dob.Split('/')[2];


                this.findElementBy(driver, By.CssSelector("#BirthMonth > option:nth-child(" + (month + 1) + ")"), 60, "Select Month").Click();
                this.findElementBy(driver, By.CssSelector("#BirthDay > option:nth-child(" + (day + 1) + ")"), 60, "Select Day").Click();
                this.findElementBy(driver, By.CssSelector("#BirthYear"), 60, "Input FName").SendKeys(year);
                this.findElementBy(driver, By.CssSelector("#iSignupAction"), 60, "Btn Next").Click();

                int timeout = 30;
                var isSuccess = false;
                while (timeout > 0 && !isSuccess)
                {
                    try
                    {
                        var checkCaptchaFrame = this.findElementByWithoutError(driver, By.Id("enforcementFrame"), 1);
                        if (checkCaptchaFrame != null)
                        {

                            try
                            {
                                var urlCatpcha = checkCaptchaFrame.GetAttribute("src");
                                var key = urlCatpcha.Replace("https://iframe.arkoselabs.com/", "").Split('/')[0];

                                driver.SwitchTo().Frame(checkCaptchaFrame);
                                var codeResult = resolveFunCaptcha(key, "https://signup.live.com/signup");
                                ((IJavaScriptExecutor)driver).ExecuteScript("var e = getAllUrlParams(window.location.href); 'xbox_1' === e.uitheme ? navigator.__defineGetter__('userAgent', function () { return 'Xbox_Dark'; }) : 'xbox_2' === e.uitheme ? navigator.__defineGetter__('userAgent', function () { return 'Xbox_Light'; }) : 'win8wiz' === e.uitheme && navigator.__defineGetter__('userAgent', function () { return 'Win8Wiz'; }); var t = window.location.pathname.split('/'), a = new ArkoseEnforcement({ public_key: t[1], language: e.mkt, target_html: 'arkose', callback: function () { parent.postMessage(JSON.stringify({ eventId: 'challenge-complete', payload: { sessionToken: a.getSessionToken() } }), '*'); }, loaded_callback: function () { (frameHeight = document.getElementById('fc-iframe-wrap').offsetHeight), (frameWidth = document.getElementById('fc-iframe-wrap').offsetWidth), 'xbox_1' === e.uitheme || 'xbox_2' === e.uitheme ? a.enableDirectionalInput() : e.uitheme, parent.postMessage( JSON.stringify({ eventId: 'challenge-loaded', payload: { sessionToken: a.getSessionToken(), frameHeight: frameHeight, frameWidth: frameWidth }, }), '*' ), (loadedCheck = 1); }, onsuppress: function () { parent.postMessage(JSON.stringify({ eventId: 'challenge-suppressed', payload: { sessionToken: a.getSessionToken() } }), '*'); }, onshown: function () { parent.postMessage(JSON.stringify({ eventId: 'challenge-shown', payload: { sessionToken: a.getSessionToken() } }), '*'); }, }); var token = '" + codeResult + "'; document.querySelector('#verification-token').value = token; document.querySelector('#FunCaptcha-Token').value = token; a.callback();");
                                Thread.Sleep(3000);
                            }
                            catch (Exception e)
                            {
                                driver.SwitchTo().DefaultContent();
                            }

                        }

                        var checkSuccess = this.findElementByWithoutError(driver, By.Id("idSIButton9"), 1);
                        if (checkSuccess != null)
                        {
                            try
                            {
                                this.findElementByWithoutError(driver, By.Name("DontShowAgain"), 1).Click();
                            }
                            catch (Exception)
                            {
                            }
                            checkSuccess.Click();
                            int timeoutLoadHome = 30;
                            while (timeoutLoadHome > 0)
                            {
                                if (driver.Url.Contains("inbox") || driver.Url.Contains("mail/0/"))
                                {
                                    Thread.Sleep(5000);
                                    isSuccess = true;
                                    break;
                                }
                                timeoutLoadHome--;
                                Thread.Sleep(1000);
                            }
                        }
                    }
                    catch (Exception e)
                    {

                    }
                    if (isSuccess)
                    {
                        break;
                    }
                    timeout--;
                    Thread.Sleep(2000);
                }
                this.verifyMailPaypalAfterRegHotMail(driver, account);
                if (timeout == 0)
                {
                    throw new Exception("Timeout");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw new Exception("Lỗi reg hotmail: " + ex.Message);
            }
        }
        public void verifyMailPaypalAfterRegHotMail2(IWebDriver driver, Account account)
        {
            driver.Url = "https://www.paypal.com/myaccount/profile/email/list";
            Thread.Sleep(2000);
            var btnAddNewMail = this.findElementBy(driver, By.CssSelector("#email-add-link"));


            //ppvx_badge ppvx_badge--type_critical statusBadge
            //ppvx_badge ppvx_badge--type_neutral statusBadge
            //https://www.paypal.com/myaccount/profile/email/list
            btnAddNewMail.Click();
            Thread.Sleep(2000);

            var inputNewMail = this.findElementBy(driver, By.CssSelector("#text-input-emailAdd"));
            if (inputNewMail != null)
            {
                inputNewMail.SendKeys(account.email);
                Thread.Sleep(1000);
            }
            else
            {
                throw new Exception("Input New Mail not found");
            }

            var btnAddMail = this.findElementBy(driver, By.CssSelector("#test_addUpdateEmailButton"));
            if (btnAddMail != null)
            {
                btnAddMail.Click();
            }
            else
            {
                throw new Exception("Button Add Mail not found");
            }
            var confirmChangeMail = this.findElementBy(driver, By.CssSelector("button[name=\"resendConfirmation\"]"));
            Thread.Sleep(1000);

            driver.Url = "https://outlook.live.com/mail/0/";
            var timeout = 60;
            while (true)
            {
                try
                {
                    var html = driver.Url;
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi mất kết nối: " + ex.Message);
                }

                try
                {
                    var checkLogin = this.findElementByWithoutError(driver, By.CssSelector("a[data-task=\"signin\"]"), 1);
                    if (checkLogin != null)
                    {
                        checkLogin.Click();
                    }


                    var inputPassword = this.findElementByWithoutError(driver, By.CssSelector("input[name=\"login_password\"]"), 1);

                    if (inputPassword == null)
                    {
                        inputPassword = this.findElementByWithoutError(driver, By.Name("login_password"), 1);
                    }
                    if (inputPassword == null)
                    {
                        inputPassword = this.findElementByWithoutError(driver, By.Id("password"), 1);
                    }
                    if (inputPassword != null)
                    {
                        Thread.Sleep(1000);
                        inputPassword.SendKeys(account.password);
                        var btnLogin = this.findElementByWithoutError(driver, By.CssSelector("#btnLogin"), 1);
                        if (btnLogin != null)
                        {
                            Thread.Sleep(1000);
                            btnLogin.Click();
                        }
                    }



                    var checkLoginPaypal = this.findElementByWithoutError(driver, By.CssSelector("input[name=\"login_password\"]"), 1);
                    if (checkLoginPaypal != null)
                    {
                        checkLoginPaypal.SendKeys(account.password);
                        new WebDriverWait(driver, TimeSpan.FromSeconds(60)).Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[name=\"btnLogin\"]"))).Click();
                    }

                    var mailPaypal = this.findElementByWithoutError(driver, By.CssSelector("div[aria-label*=\"PayPal\"]"), 1);
                    if (mailPaypal != null && !driver.Url.Contains("paypal"))
                    {

                        try
                        {
                            var btnCloseAlert = this.findElementByWithoutError(driver, By.CssSelector("div.ms-Callout-main > div > div:nth-child(2) > button"), 1);
                            if (btnCloseAlert != null)
                            {
                                btnCloseAlert.Click();
                            }

                            mailPaypal.Click();
                            //var confirmEle = this.findElementByWithoutError(driver, By.XPath("//*[contains(text(), 'Click to activate your account')]"), 10);
                            var confirmEle = this.findElementByWithoutError(driver, By.CssSelector("a[data-auth=\"NotApplicable\"]"), 3);

                            if (confirmEle != null)
                            {
                                var urlConfirm = confirmEle.GetAttribute("href");
                                driver.Url = urlConfirm;
                                Thread.Sleep(2000);
                                var inputPass = this.findElementByWithoutError(driver, By.CssSelector("#password"), 3);
                                if (inputPass != null)
                                {
                                    inputPass.SendKeys(account.password);
                                    Thread.Sleep(2000);
                                    var btnLogin = this.findElementByWithoutError(driver, By.CssSelector("#btnLogin"), 1);
                                    if (btnLogin != null)
                                    {
                                        Thread.Sleep(1000);
                                        btnLogin.Click();
                                    }
                                }

                            }
                        }
                        catch (Exception eee)
                        {

                        }
                    }
                    // Chec 2FA
                    try
                    {

                        var inputOtp = this.findElementByWithoutError(driver, By.CssSelector("#otpCode"), 1);
                        if (inputOtp != null)
                        {
                            var newToken = this.Get2FA(account.twoFA.Replace(" ", ""));
                            if (newToken == null || newToken == "")
                            {
                                throw new Exception("Lỗi lấy token 2fa");
                            }
                            for (int i = 0; i < newToken.Length; i++)
                            {
                                inputOtp = this.findElementBy(driver, By.CssSelector("#ci-otpCode-" + i), 1);
                                inputOtp.SendKeys(newToken[i].ToString());
                            }

                            Thread.Sleep(1000);
                            try
                            {
                                var skipTwofactorCheckbox = this.findElementBy(driver, By.CssSelector("#skipTwofactorCheckbox"), 3, "skipTwofactorCheckbox");
                                skipTwofactorCheckbox.Click();
                            }
                            catch (Exception)
                            {
                            }
                            Thread.Sleep(1000);
                            var btnSubmit2Fa = this.findElementBy(driver, By.CssSelector("button[type=\"submit\"]"), 3, "btnSubmit2Fa");
                            Thread.Sleep(1000);
                            btnSubmit2Fa.Click();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("2FA: " + ex.Message);
                    }

                    var checkSuccess = this.findElementByWithoutError(driver, By.CssSelector("div.cw_tile-currencyContainer"), 1);

                    var checkSuccess1 = this.findElementByWithoutError(driver, By.CssSelector("div.test_balance-tile-currency"), 1);


                    if (checkSuccess != null || checkSuccess1 != null)
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                }
                timeout--;
                Thread.Sleep(1000);
            }

        }
        public void removeMailGoc(IWebDriver driver, Account account)
        {
            driver.Url = "https://www.paypal.com/myaccount/profile/email/list";
            Thread.Sleep(2000);
            //=========== Set primary =================
            var nodeMailNew = this.findElementBy(driver, By.XPath("//div[contains(text(), '" + account.email + "')]"));
            if (nodeMailNew == null)
            {
                throw new Exception("nodeMailNew is not found");
            }

            var parentMailNew = nodeMailNew.FindElement(By.XPath(".."));
            //==================
            var btnSetPrimaryEmail = parentMailNew.FindElement(By.Name("makePrimary"));
            if (btnSetPrimaryEmail != null)
            {
                btnSetPrimaryEmail.Click();
            }
            else
            {
                throw new Exception("btnSetPrimaryEmail not found");
            }

            var btnConfirmPrimary = this.findElementBy(driver, By.CssSelector("button[name=\"Confirm\"]"));
            if (btnConfirmPrimary != null)
            {
                btnConfirmPrimary.Click();
            }
            else
            {
                throw new Exception("btnConfirmPrimary not found");
            }
            Thread.Sleep(1000);
            //=================
            var nodeMailUnConfirm = this.findElementByWithoutError(driver, By.XPath("//span[contains(text(), 'CHƯA XÁC NHẬN')]"), 2);
            if (nodeMailUnConfirm == null)
            {
                nodeMailUnConfirm = this.findElementByWithoutError(driver, By.XPath("//span[contains(text(), 'UNCONFIRMED')]"), 2); ;
            }
            while (nodeMailUnConfirm != null)
            {
                var parentNodeMailPrimary = nodeMailUnConfirm.FindElement(By.XPath(".."));
                var btnDelete = parentNodeMailPrimary.FindElement(By.Name("remove"));
                if (btnDelete != null)
                {
                    btnDelete.Click();
                }
                else
                {
                    throw new Exception("btnDelete not found");
                }

                Thread.Sleep(1000);
                var btnConfirmDelete = this.findElementBy(driver, By.CssSelector("button[name=\"Remove\"]"));
                if (btnConfirmDelete != null)
                {
                    btnConfirmDelete.Click();
                }
                else
                {
                    throw new Exception("btnConfirmDelete not found");
                }
                Thread.Sleep(3000);
                nodeMailUnConfirm = this.findElementByWithoutError(driver, By.XPath("//span[contains(text(), 'CHƯA XÁC NHẬN')]"), 2);
                if (nodeMailUnConfirm == null)
                {
                    nodeMailUnConfirm = this.findElementByWithoutError(driver, By.XPath("//span[contains(text(), 'UNCONFIRMED')]"), 2); ;
                }
            }

            Thread.Sleep(5000);
        }
        public void removeMailGoc2(IWebDriver driver, Account account)
        {
            driver.Url = "https://www.paypal.com/myaccount/profile/email/list";
            Thread.Sleep(2000);
            //=========== Set primary =================
            var nodeMailNew = this.findElementBy(driver, By.XPath("//div[contains(text(), '" + account.email + "')]"));
            if (nodeMailNew == null)
            {
                throw new Exception("nodeMailNew is not found");
            }

            var parentMailNew = nodeMailNew.FindElement(By.XPath(".."));
            //==================
            var btnSetPrimaryEmail = parentMailNew.FindElement(By.Name("makePrimary"));
            if (btnSetPrimaryEmail != null)
            {
                btnSetPrimaryEmail.Click();
            }
            else
            {
                throw new Exception("btnSetPrimaryEmail not found");
            }

            var btnConfirmPrimary = this.findElementBy(driver, By.CssSelector("button[name=\"Confirm\"]"));
            if (btnConfirmPrimary != null)
            {
                btnConfirmPrimary.Click();
            }
            else
            {
                throw new Exception("btnConfirmPrimary not found");
            }
            Thread.Sleep(1000);
            //=================
            var listNodeMail = this.findElementsBy(driver, By.CssSelector("div.manageComponent-info"), 2);
            listNodeMail.Reverse();

            while (listNodeMail.Count > 1)
            {
                try
                {
                    foreach (var nodeMail in listNodeMail)
                    {

                        if (!nodeMail.Text.ToLower().Contains(account.email.ToLower()))
                        {
                            var parentNodeMailPrimary = nodeMail.FindElement(By.XPath(".."));
                            var btnDelete = parentNodeMailPrimary.FindElement(By.Name("remove"));
                            if (btnDelete != null)
                            {
                                btnDelete.Click();
                            }
                            else
                            {
                                throw new Exception("btnDelete not found");
                            }

                            Thread.Sleep(1000);
                            var btnConfirmDelete = this.findElementBy(driver, By.CssSelector("button[name=\"Remove\"]"));
                            if (btnConfirmDelete != null)
                            {
                                btnConfirmDelete.Click();
                            }
                            else
                            {
                                throw new Exception("btnConfirmDelete not found");
                            }
                            Thread.Sleep(3000);
                        }
                        break;
                    }
                    listNodeMail = this.findElementsBy(driver, By.CssSelector("div.manageComponent-info"), 2);
                    listNodeMail.Reverse();
                }
                catch (Exception)
                {
                    break;
                }
            }

            Thread.Sleep(5000);
        }

        public static string convertToUnSign3(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        public string regHostMail2(IWebDriver driver, Account account, string email, string password, string fname, string lname, string dob)
        {
            try
            {
                email = convertToUnSign3(account.name).Replace(" ", "") + RandomString(6) + "@hotmail.com";
                account.email = email;
                driver.Url = "https://www.paypal.com/myaccount/settings/";
                Thread.Sleep(2000);
                var btnChangeMail = this.findElementBy(driver, By.CssSelector("a[data-track=\"changeEmailLink\"]"));

                //Load xong. Check xem mail chính đã xm chưa
                var spanPrimary = this.findElementBy(driver, By.CssSelector("ul.emails.unstyled.lined > li:nth-child(1)"));
                try
                {
                    var spanUnConfirm = spanPrimary.FindElement(By.CssSelector("span.ppvx_badge.ppvx_badge--type_critical.statusBadge"));
                    if (spanUnConfirm == null)
                    {
                        return null;
                    }
                }
                catch (Exception)
                {
                    return null;
                }

                driver.Url = "https://signup.live.com/signup?wa=wsignin1.0&rpsnv=13&ct=1634232667&rver=7.0.6737.0&wp=MBI_SSL&wreply=https%3a%2f%2foutlook.live.com%2fowa%2f%3fnlp%3d1%26RpsCsrfState%3d47f46e37-2349-b0fa-381b-4eedf2d1ef56&id=292841&aadredir=1&CBCXT=out&lw=1&fl=dob%2cflname%2cwld&cobrandid=90015&contextid=20B1CE18918203F7&bk=1634232667&uiflavor=web&lic=1&mkt=EN-US&lc=1033&uaid=a16953a4526f42deb3431d2c7ac23a28";

                if (email.EndsWith("hotmail.com"))
                {
                    new WebDriverWait(driver, TimeSpan.FromSeconds(60)).Until(ExpectedConditions.ElementExists((By.CssSelector("#LiveDomainBoxList > option:nth-child(2)"))));
                    this.findElementBy(driver, By.CssSelector("#LiveDomainBoxList > option:nth-child(2)"), 60, "Select type mail").Click();
                }
                this.findElementBy(driver, By.CssSelector("#MemberName"), 60, "Input Email").SendKeys(email.Split('@')[0]);
                Thread.Sleep(500);
                this.findElementBy(driver, By.CssSelector("#iSignupAction"), 60, "Btn Next").Click();
                Thread.Sleep(500);
                this.findElementBy(driver, By.CssSelector("#PasswordInput"), 60, "Input password").SendKeys(password);
                Thread.Sleep(500);
                this.findElementBy(driver, By.CssSelector("#iSignupAction"), 60, "Btn Next").Click();
                Thread.Sleep(500);
                this.findElementBy(driver, By.CssSelector("#FirstName"), 60, "Input FName").SendKeys(fname);
                Thread.Sleep(500);
                this.findElementBy(driver, By.CssSelector("#LastName"), 60, "Input LName").SendKeys(lname);
                Thread.Sleep(500);
                this.findElementBy(driver, By.CssSelector("#iSignupAction"), 60, "Btn Next").Click();

                int month = new Random().Next(1, 12);
                int day = new Random().Next(1, 28);
                string year = dob.Split('/')[2];


                this.findElementBy(driver, By.CssSelector("#BirthMonth > option:nth-child(" + (month + 1) + ")"), 60, "Select Month").Click();
                this.findElementBy(driver, By.CssSelector("#BirthDay > option:nth-child(" + (day + 1) + ")"), 60, "Select Day").Click();
                this.findElementBy(driver, By.CssSelector("#BirthYear"), 60, "Input FName").SendKeys(year);
                this.findElementBy(driver, By.CssSelector("#iSignupAction"), 60, "Btn Next").Click();

                int timeout = 30;
                var isSuccess = false;
                while (timeout > 0 && !isSuccess)
                {
                    try
                    {
                        var checkCaptchaFrame = this.findElementByWithoutError(driver, By.Id("enforcementFrame"), 1);
                        if (checkCaptchaFrame != null)
                        {

                            try
                            {
                                var urlCatpcha = checkCaptchaFrame.GetAttribute("src");
                                var key = urlCatpcha.Replace("https://iframe.arkoselabs.com/", "").Split('/')[0];

                                driver.SwitchTo().Frame(checkCaptchaFrame);
                                var codeResult = resolveFunCaptcha(key, "https://signup.live.com/signup");
                                ((IJavaScriptExecutor)driver).ExecuteScript("var e = getAllUrlParams(window.location.href); 'xbox_1' === e.uitheme ? navigator.__defineGetter__('userAgent', function () { return 'Xbox_Dark'; }) : 'xbox_2' === e.uitheme ? navigator.__defineGetter__('userAgent', function () { return 'Xbox_Light'; }) : 'win8wiz' === e.uitheme && navigator.__defineGetter__('userAgent', function () { return 'Win8Wiz'; }); var t = window.location.pathname.split('/'), a = new ArkoseEnforcement({ public_key: t[1], language: e.mkt, target_html: 'arkose', callback: function () { parent.postMessage(JSON.stringify({ eventId: 'challenge-complete', payload: { sessionToken: a.getSessionToken() } }), '*'); }, loaded_callback: function () { (frameHeight = document.getElementById('fc-iframe-wrap').offsetHeight), (frameWidth = document.getElementById('fc-iframe-wrap').offsetWidth), 'xbox_1' === e.uitheme || 'xbox_2' === e.uitheme ? a.enableDirectionalInput() : e.uitheme, parent.postMessage( JSON.stringify({ eventId: 'challenge-loaded', payload: { sessionToken: a.getSessionToken(), frameHeight: frameHeight, frameWidth: frameWidth }, }), '*' ), (loadedCheck = 1); }, onsuppress: function () { parent.postMessage(JSON.stringify({ eventId: 'challenge-suppressed', payload: { sessionToken: a.getSessionToken() } }), '*'); }, onshown: function () { parent.postMessage(JSON.stringify({ eventId: 'challenge-shown', payload: { sessionToken: a.getSessionToken() } }), '*'); }, }); var token = '" + codeResult + "'; document.querySelector('#verification-token').value = token; document.querySelector('#FunCaptcha-Token').value = token; a.callback();");
                                Thread.Sleep(3000);
                            }
                            catch (Exception e)
                            {
                                driver.SwitchTo().DefaultContent();
                            }

                        }

                        var checkSuccess = this.findElementByWithoutError(driver, By.Id("idSIButton9"), 1);
                        if (checkSuccess != null)
                        {
                            try
                            {
                                this.findElementByWithoutError(driver, By.Name("DontShowAgain"), 1).Click();
                            }
                            catch (Exception)
                            {
                            }
                            checkSuccess.Click();
                            int timeoutLoadHome = 30;
                            while (timeoutLoadHome > 0)
                            {
                                if (driver.Url.Contains("inbox") || driver.Url.Contains("mail/0/"))
                                {
                                    Thread.Sleep(5000);
                                    isSuccess = true;
                                    break;
                                }
                                timeoutLoadHome--;
                                Thread.Sleep(1000);
                            }
                        }
                    }
                    catch (Exception e)
                    {

                    }
                    if (isSuccess)
                    {
                        break;
                    }
                    timeout--;
                    Thread.Sleep(2000);
                }
                this.verifyMailPaypalAfterRegHotMail2(driver, account);
                Thread.Sleep(1000);
                this.removeMailGoc(driver, account);
                if (timeout == 0)
                {
                    throw new Exception("Timeout");
                }
                return email;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw new Exception("Lỗi  regHostMail2: " + ex.Message);
            }
        }

        public string regHostMail3(IWebDriver driver, Account account, string email, string password, string fname, string lname, string dob)
        {
            try
            {
                email = convertToUnSign3(account.name).Replace(" ", "") + RandomString(6) + "@hotmail.com";
                account.email = email;
                driver.Url = "https://www.paypal.com/myaccount/settings/";
                Thread.Sleep(2000);
                var btnChangeMail = this.findElementBy(driver, By.CssSelector("a[data-track=\"changeEmailLink\"]"));

                driver.Url = "https://signup.live.com/signup?wa=wsignin1.0&rpsnv=13&ct=1634232667&rver=7.0.6737.0&wp=MBI_SSL&wreply=https%3a%2f%2foutlook.live.com%2fowa%2f%3fnlp%3d1%26RpsCsrfState%3d47f46e37-2349-b0fa-381b-4eedf2d1ef56&id=292841&aadredir=1&CBCXT=out&lw=1&fl=dob%2cflname%2cwld&cobrandid=90015&contextid=20B1CE18918203F7&bk=1634232667&uiflavor=web&lic=1&mkt=EN-US&lc=1033&uaid=a16953a4526f42deb3431d2c7ac23a28";

                if (email.EndsWith("hotmail.com"))
                {
                    new WebDriverWait(driver, TimeSpan.FromSeconds(60)).Until(ExpectedConditions.ElementExists((By.CssSelector("#LiveDomainBoxList > option:nth-child(2)"))));
                    this.findElementBy(driver, By.CssSelector("#LiveDomainBoxList > option:nth-child(2)"), 60, "Select type mail").Click();
                }
                this.findElementBy(driver, By.CssSelector("#MemberName"), 60, "Input Email").SendKeys(email.Split('@')[0]);
                Thread.Sleep(500);
                this.findElementBy(driver, By.CssSelector("#iSignupAction"), 60, "Btn Next").Click();
                Thread.Sleep(500);
                this.findElementBy(driver, By.CssSelector("#PasswordInput"), 60, "Input password").SendKeys(password);
                Thread.Sleep(500);
                this.findElementBy(driver, By.CssSelector("#iSignupAction"), 60, "Btn Next").Click();
                Thread.Sleep(500);
                this.findElementBy(driver, By.CssSelector("#FirstName"), 60, "Input FName").SendKeys(fname);
                Thread.Sleep(500);
                this.findElementBy(driver, By.CssSelector("#LastName"), 60, "Input LName").SendKeys(lname);
                Thread.Sleep(500);
                this.findElementBy(driver, By.CssSelector("#iSignupAction"), 60, "Btn Next").Click();

                int month = new Random().Next(1, 12);
                int day = new Random().Next(1, 28);
                string year = dob.Split('/')[2];


                this.findElementBy(driver, By.CssSelector("#BirthMonth > option:nth-child(" + (month + 1) + ")"), 60, "Select Month").Click();
                this.findElementBy(driver, By.CssSelector("#BirthDay > option:nth-child(" + (day + 1) + ")"), 60, "Select Day").Click();
                this.findElementBy(driver, By.CssSelector("#BirthYear"), 60, "Input FName").SendKeys(year);
                this.findElementBy(driver, By.CssSelector("#iSignupAction"), 60, "Btn Next").Click();

                int timeout = 30;
                var isSuccess = false;
                while (timeout > 0 && !isSuccess)
                {
                    try
                    {
                        var checkCaptchaFrame = this.findElementByWithoutError(driver, By.Id("enforcementFrame"), 1);
                        if (checkCaptchaFrame != null)
                        {

                            try
                            {
                                var urlCatpcha = checkCaptchaFrame.GetAttribute("src");
                                var key = urlCatpcha.Replace("https://iframe.arkoselabs.com/", "").Split('/')[0];

                                driver.SwitchTo().Frame(checkCaptchaFrame);
                                var codeResult = resolveFunCaptcha(key, "https://signup.live.com/signup");
                                ((IJavaScriptExecutor)driver).ExecuteScript("var e = getAllUrlParams(window.location.href); 'xbox_1' === e.uitheme ? navigator.__defineGetter__('userAgent', function () { return 'Xbox_Dark'; }) : 'xbox_2' === e.uitheme ? navigator.__defineGetter__('userAgent', function () { return 'Xbox_Light'; }) : 'win8wiz' === e.uitheme && navigator.__defineGetter__('userAgent', function () { return 'Win8Wiz'; }); var t = window.location.pathname.split('/'), a = new ArkoseEnforcement({ public_key: t[1], language: e.mkt, target_html: 'arkose', callback: function () { parent.postMessage(JSON.stringify({ eventId: 'challenge-complete', payload: { sessionToken: a.getSessionToken() } }), '*'); }, loaded_callback: function () { (frameHeight = document.getElementById('fc-iframe-wrap').offsetHeight), (frameWidth = document.getElementById('fc-iframe-wrap').offsetWidth), 'xbox_1' === e.uitheme || 'xbox_2' === e.uitheme ? a.enableDirectionalInput() : e.uitheme, parent.postMessage( JSON.stringify({ eventId: 'challenge-loaded', payload: { sessionToken: a.getSessionToken(), frameHeight: frameHeight, frameWidth: frameWidth }, }), '*' ), (loadedCheck = 1); }, onsuppress: function () { parent.postMessage(JSON.stringify({ eventId: 'challenge-suppressed', payload: { sessionToken: a.getSessionToken() } }), '*'); }, onshown: function () { parent.postMessage(JSON.stringify({ eventId: 'challenge-shown', payload: { sessionToken: a.getSessionToken() } }), '*'); }, }); var token = '" + codeResult + "'; document.querySelector('#verification-token').value = token; document.querySelector('#FunCaptcha-Token').value = token; a.callback();");
                                Thread.Sleep(3000);
                            }
                            catch (Exception e)
                            {
                                driver.SwitchTo().DefaultContent();
                            }

                        }

                        var checkSuccess = this.findElementByWithoutError(driver, By.Id("idSIButton9"), 1);
                        if (checkSuccess != null)
                        {
                            try
                            {
                                this.findElementByWithoutError(driver, By.Name("DontShowAgain"), 1).Click();
                            }
                            catch (Exception)
                            {
                            }
                            checkSuccess.Click();
                            int timeoutLoadHome = 30;
                            while (timeoutLoadHome > 0)
                            {
                                if (driver.Url.Contains("inbox") || driver.Url.Contains("mail/0/"))
                                {
                                    Thread.Sleep(5000);
                                    isSuccess = true;
                                    break;
                                }
                                timeoutLoadHome--;
                                Thread.Sleep(1000);
                            }
                        }
                    }
                    catch (Exception e)
                    {

                    }
                    if (isSuccess)
                    {
                        break;
                    }
                    timeout--;
                    Thread.Sleep(2000);
                }
                this.verifyMailPaypalAfterRegHotMail2(driver, account);
                Thread.Sleep(1000);
                this.removeMailGoc2(driver, account);
                if (timeout == 0)
                {
                    throw new Exception("Timeout");
                }
                return email;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw new Exception("Lỗi reg hotmail: " + ex.Message);
            }
        }

        public string getCodeRentCode(string api, string requestid)
        {
            int time = 0;
            while (true)
            {
                HttpRequest http = new HttpRequest();
                string ma = http.Get("https://chothuesimcode.com/api?act=code&apik=" + api + "&id=" + requestid, null).ToString();
                try
                {
                    var res = Regex.Matches(ma, @"(?<=Code"":"").*?(?="")", RegexOptions.Singleline);
                    if (res != null)
                    {
                        string session = res[0].ToString();
                        if (session != "" || session != "Loi")
                        {
                            return session;
                        }
                    }
                }
                catch
                {
                    Thread.Sleep(1000);
                    time += 1;
                    if (time == 120)
                    {
                        return null;
                    }
                }
            }
        }
        public string getPhoneChothuecode(string api, string mang)
        {
            string id = "";
            string sdt = "";
            HttpRequest http = new HttpRequest();
            string getid = http.Get("https://chothuesimcode.com/api?act=number&apik=" + api + "&appId=1090&carrier=" + mang + "&prefix=", null).ToString();
            try
            {
                var res = Regex.Matches(getid, @"(?<=Id"":).*?(?=,)", RegexOptions.Singleline);
                if (res != null)
                {
                    string session = res[0].ToString();
                    id = session;
                    res = Regex.Matches(getid, @"(?<=Number"":"").*?(?="",)", RegexOptions.Singleline);
                    if (res != null)
                    {
                        session = res[0].ToString();
                        sdt = session;
                        if (sdt != "")
                        {
                            return sdt + "|" + id;
                        }

                    }

                }
            }
            catch
            {
            }
            return sdt + "|" + id;
        }
        public void inputDelay(IWebDriver driver, By by, int timeout, string text)
        {
            var ele = this.findElementByWithoutError(driver, by, timeout, by.ToString());
            ele.Clear();
            ele.Click();
            //Clipboard.SetText(text);
            //ele.SendKeys(OpenQA.Selenium.Keys.Control + "v");
            for (int i = 0; i < text.Length; i++)
            {
                ele.SendKeys(text[i].ToString());
                Thread.Sleep(100);
            }
            Thread.Sleep(1000);
        }
        public void inputCopy(IWebDriver driver, By by, int timeout, string text)
        {
            var ele = this.findElementByWithoutError(driver, by, timeout, by.ToString());
            ele.Clear();
            ele.Click();
            Clipboard.SetText(text);
            ele.SendKeys(OpenQA.Selenium.Keys.Control + "v");
            //for (int i = 0; i < text.Length; i++)
            //{
            //    ele.SendKeys(text[i].ToString());
            //    Thread.Sleep(100);
            //}
            Thread.Sleep(1000);
        }
        public void StartRegPaypal(string data, int port, ProxyObject proxyObject)
        {
            if (data == "")
            {
                return;
            }
            var apiSim = Settings.Default.apiSim;
            var profile_id = "";
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI2MTQ4MTlmY2NjMGI4NWY1MDI2Y2EzMTUiLCJ0eXBlIjoiZGV2Iiwiand0aWQiOiI2MTQ4ZDAwMjNhYTlhNWZmODZlZTAyYzUifQ.nDUpRUdicCZsUG_MBfrlhZIqn5pOWyVhUmp-uqLAyrQ";
            var email = data.Split('|')[0];
            var passMail = data.Split('|')[1];
            var passPaypal = data.Split('|')[2];
            var fname = data.Split('|')[3];
            var mname = data.Split('|')[4];
            var lname = data.Split('|')[5];
            var cmnd = data.Split('|')[6];
            var zipcode = data.Split('|')[7];
            var dob = data.Split('|')[8];
            var address1 = data.Split('|')[9];
            var address2 = data.Split('|')[10];
            var address3 = data.Split('|')[11];
            var proxy = data.Split('|')[12];


            IWebDriver driver = null;
            Gologin gologin = null;

            Account account = new Account();
            account.email = email;
            account.password = passPaypal;
            account.passwordEmail = passMail;
            account.address = address1 + ", " + address2 + ", " + address2;
            account.name = fname + " " + mname + " " + lname;
            account.proxy = proxy;
            account.email = email;
            account.birthday = dob;
            account.cmndNumber = cmnd;
            account.IdFolder = this.form1.folderSelected.Id;
            account.resultKTGD = "Tạo Account";


            try
            {
                DataStartDriver dataStartDriver = openChromeDriver(account, true, port);
                driver = dataStartDriver.driver;
                gologin = dataStartDriver.gologin;


                driver.Url = "https://www.paypal.com/vn/webapps/mpp/account-selection?locale.x=vi_VN";
                try
                {
                    new WebDriverWait(driver, TimeSpan.FromSeconds(30)).Until(ExpectedConditions.ElementToBeClickable(By.Id("next-btn")));
                    Thread.Sleep(1000);
                    this.findElementBy(driver, By.Id("next-btn"), 60, "Button Next").Click();

                    Thread.Sleep(1000);

                    this.findElementBy(driver, By.CssSelector("#personal-select-1 > option:nth-child(3)"), 60, "Button Next").Click();

                    Thread.Sleep(1000);
                    this.findElementBy(driver, By.Id("next-btn"), 60, "Button Next").Click();
                    var timeout = 30;
                    var idPhone = "";

                    IWebElement keySiteEl = null;
                    while (timeout > 0)
                    {
                        try
                        {
                            keySiteEl = this.findElementByWithoutError(driver, By.CssSelector("input[name=\"_adsRecaptchaSiteKey\"]"), 1);
                            if (keySiteEl != null)
                            {
                                var keySite = keySiteEl.GetAttribute("value");
                                if (keySite != null && keySite != "")
                                {
                                    this.resovleCaptchaAsync(driver, keySite, "#content > form > iframe");
                                }
                            }

                            var checkSuccess = this.findElementByWithoutError(driver, By.CssSelector("button[value='shop_intent']"), 1);
                            if (checkSuccess != null)
                            {
                                break;
                            }
                            //
                            var selectCountry = this.findElementByWithoutError(driver, By.Id("combo_txt_paypalAccountData_countryselector"), 1);
                            if (selectCountry != null)
                            {
                                selectCountry.Click();
                                this.findElementByWithoutError(driver, By.Id("/paypalAccountData/countryselector__VN"), 2).Click();
                                this.findElementBy(driver, By.CssSelector("#paypalAccountData_submit"), 2, "Button Next").Click();

                            }
                            //



                            //==============================
                            var inputDOB = this.findElementByWithoutError(driver, By.Id("paypalAccountData_dob"), 1);
                            if (inputDOB != null)
                            {
                                Thread.Sleep(1000);

                                this.inputCopy(driver, By.Id("paypalAccountData_dob"), 1, dob);

                                this.inputDelay(driver, By.Id("paypalAccountData_identificationNum"), 1, cmnd);
                                this.inputDelay(driver, By.Id("paypalAccountData_address1_0"), 1, address1);
                                this.inputDelay(driver, By.Id("paypalAccountData_address2_0"), 1, address2);
                                this.inputDelay(driver, By.Id("paypalAccountData_city_0"), 1, address3);
                                //       this.findElementByWithoutError(driver, By.CssSelector("li[id=\"smenu_item_Hồ Chí Minh\"]"), 1).Click();

                                try
                                {
                                    this.findElementByWithoutError(driver, By.Id("dropdownMenuButton_paypalAccountData_state_0"), 1).Click();
                                    Thread.Sleep(2000);
                                    this.findElementByWithoutError(driver, By.CssSelector("li[id=\"smenu_item_Hồ Chí Minh\"]"), 1).Click();
                                }
                                catch (Exception)
                                {
                                }
                                Thread.Sleep(1000);
                                this.inputDelay(driver, By.Id("paypalAccountData_zip_0"), 1, zipcode);
                                try
                                {
                                    ((IJavaScriptExecutor)driver).ExecuteScript("if (!document.querySelector('#paypalAccountData_oneTouchCheckbox').checked) { document.querySelector('#paypalAccountData_oneTouchCheckbox').click(); }");

                                }
                                catch (Exception)
                                {
                                }
                                try
                                {
                                    ((IJavaScriptExecutor)driver).ExecuteScript("if (!document.querySelector('#paypalAccountData_termsAgree').checked) { document.querySelector('#paypalAccountData_termsAgree').click(); }");
                                }
                                catch (Exception)
                                {
                                }

                                keySiteEl = this.findElementByWithoutError(driver, By.CssSelector("input[name=\"_adsRecaptchaSiteKey\"]"), 1);
                                if (keySiteEl != null)
                                {
                                    continue;
                                }
                                this.findElementByWithoutError(driver, By.Id("paypalAccountData_emailPassword"), 1).Click();

                            }
                            var inputEmail = this.findElementByWithoutError(driver, By.Id("paypalAccountData_email"), 1);
                            if (inputEmail != null)
                            {

                                keySiteEl = this.findElementByWithoutError(driver, By.CssSelector("input[name=\"_adsRecaptchaSiteKey\"]"), 1);
                                if (keySiteEl != null)
                                {
                                    continue;
                                }
                                Thread.Sleep(1000);
                                this.inputDelay(driver, By.Id("paypalAccountData_email"), 1, email);
                                this.inputDelay(driver, By.Id("paypalAccountData_lastName"), 1, fname);
                                this.inputDelay(driver, By.Id("paypalAccountData_middleName"), 1, mname);
                                this.inputDelay(driver, By.Id("paypalAccountData_firstName"), 1, lname);
                                this.inputDelay(driver, By.Id("paypalAccountData_password"), 1, passPaypal);
                                try
                                {
                                    this.inputDelay(driver, By.Id("paypalAccountData_confirmPassword"), 1, passPaypal);
                                }
                                catch (Exception)
                                {
                                }
                                keySiteEl = this.findElementByWithoutError(driver, By.CssSelector("input[name=\"_adsRecaptchaSiteKey\"]"), 1);
                                if (keySiteEl != null)
                                {
                                    continue;
                                }
                                this.findElementBy(driver, By.CssSelector("#paypalAccountData_emailPassword"), 1, "Button Next").Click();

                            }

                            var checkCodePhone = this.findElementByWithoutError(driver, By.Id("completePhoneConfirmation_0"), 1);


                            if (checkCodePhone != null)
                            {
                                var codePhone = this.getCodeRentCode(apiSim, idPhone);
                                if (codePhone == null)
                                {
                                    //
                                    this.findElementBy(driver, By.CssSelector("#PageMainForm > fieldset > div:nth-child(2) > div > div > button"), 1, "Button Next").Click();
                                    Console.WriteLine("");
                                }
                                //this.inputDelay(driver, By.Id("completePhoneConfirmation_0"),1, codePhone);
                                this.findElementBy(driver, By.Id("completePhoneConfirmation_0"), 1, "checkCodePhone").SendKeys(codePhone);
                            }
                            var inputPhone = this.findElementByWithoutError(driver, By.Id("paypalAccountData_phone"), 1);

                            keySiteEl = this.findElementByWithoutError(driver, By.CssSelector("input[name=\"_adsRecaptchaSiteKey\"]"), 1);
                            if (keySiteEl != null)
                            {
                                continue;
                            }

                            if ((inputPhone != null && idPhone == "") || (inputPhone != null && inputPhone.Text == ""))
                            {
                                var resultGetPhone = this.getPhoneChothuecode(apiSim, "");
                                idPhone = resultGetPhone.Split('|')[1];
                                var phone = resultGetPhone.Split('|')[0];
                                Console.WriteLine("idPhone: " + idPhone + ": " + phone);

                                this.inputDelay(driver, By.Id("paypalAccountData_phone"), 1, phone);
                                keySiteEl = this.findElementByWithoutError(driver, By.CssSelector("input[name=\"_adsRecaptchaSiteKey\"]"), 1);
                                if (keySiteEl != null)
                                {
                                    continue;
                                }
                                this.findElementBy(driver, By.CssSelector("#paypalAccountData_submit"), 1, "Button Next").Click();
                            }

                        }
                        catch (Exception ex)
                        {
                        }

                        timeout--;
                        Thread.Sleep(1000);
                    }

                    if (timeout > 0)
                    {

                        this.myDatabase.addNewAccount(account);
                        this.form1.getAccount();
                        this.formRegAcc.removeAccount(email);

                        //try
                        //{
                        //    this.regHostMail(driver, account.email, account.password, fname + " " + mname, lname, dob);
                        //    account.resultKTGD = "Tạo hotmail";
                        //    this.form1.updateAccount(account);
                        //    this.verifyMailPaypal(driver, account);
                        //    account.resultKTGD = "Verify Account";
                        //    this.form1.updateAccount(account);


                        //}
                        //catch (Exception ex)
                        //{
                        //    account.log = ex.Message;
                        //    this.myDatabase.updateAccount(account);
                        //    this.form1.getAccount();
                        //}
                    }
                    else
                    {
                        throw new Exception("Timeout");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                MessageBox.Show(ex.Message);
            }
            listProfileIDRunning.Remove(profile_id);

            try
            {
                if (driver != null)
                {
                    driver.Manage().Cookies.DeleteAllCookies();
                    driver.Close();
                    driver.Quit();
                    Thread.Sleep(1000);
                }

            }
            catch (Exception)
            {
            }
        }
        public void StopThreadRegAcc()
        {
            if (threadRegAccount != null)
            {
                try
                {
                    threadRegAccount.Abort();
                }
                catch (Exception)
                {
                }
            }
        }
        public void StartRegAccount(List<string> listData, ModalRegAccount formRegAcc)
        {
            this.formRegAcc = formRegAcc;
            listChildThreadRegAccount = new List<Thread>();
            listProfileIDRunning = new List<string>();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.CheckCertificateRevocationList = false;
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            this.listDriver = new List<IWebDriver>();

            List<ProxyObject> proxyObjects = new List<ProxyObject>();
            var listProxyString = Settings.Default.listProxy.Split('\n');
            if (Settings.Default.proxyAutoReset)
            {
                foreach (var proxyString in listProxyString)
                {
                    if (proxyString != null && proxyString != "")
                    {
                        try
                        {
                            ProxyObject proxyObject = new ProxyObject(proxyString);
                            proxyObjects.Add(proxyObject);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Proxy sai định dạng.!");
                        }
                    }
                }
            }

            threadRegAccount = new Thread(() =>
            {

                int threadCount = Settings.Default.countThreadRegAcc;
                int totalData = listData.Count;
                totalRunning = 0;
                int currentIndex = 0;
                bool isRunning = false;
                int indexProxy = 0;
                while (currentIndex < totalData)
                {
                    if (totalRunning == 0)
                    {
                        isRunning = false;
                        indexProxy = 0;
                        this.StopCurrent();
                    }
                    if (!isRunning && totalRunning < threadCount)
                    {
                        var data = listData[currentIndex];
                        int port = 3500 + currentIndex;

                        if (Settings.Default.proxyAutoReset)
                        {
                            ProxyObject proxyObject = getNewProxyObject(proxyObjects);
                            proxyObject.Running = true;
                            proxyObject.Time = DateTime.Now;
                            Thread childThread = new Thread(() =>
                            {
                                int indexP = indexProxy;
                                this.StartRegPaypal(data, port, proxyObject);
                                totalRunning--;
                                proxyObject.Running = false;

                            });
                            childThread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
                            childThread.Start();
                            childThread.IsBackground = true;
                            listChildThreadRegAccount.Add(childThread);
                        }
                        else
                        {
                            Thread childThread = new Thread(() =>
                            {
                                int indexP = indexProxy;
                                this.StartRegPaypal(data, port, null);
                                totalRunning--;
                            });
                            childThread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
                            childThread.Start();
                            childThread.IsBackground = true;
                            listChildThreadRegAccount.Add(childThread);
                        }


                        Thread.Sleep(1000);

                        indexProxy++;
                        totalRunning++;
                        currentIndex++;

                        if (totalRunning == threadCount)
                        {
                            isRunning = true;
                        }
                    }
                    Thread.Sleep(3000);
                }
            });
            threadRegAccount.IsBackground = true;
            threadRegAccount.Start();
        }
        public void setDataRequest(int amount, string email, string note)
        {
            this.dataRequest = new DataRequest();
            this.dataRequest.amount = amount;
            this.dataRequest.email = email;
            this.dataRequest.note = note;
        }
        public bool resovleCaptchaAsync(IWebDriver driver, string keySite, string cssFrame = "")
        {
            string resultCode = "";
            try
            {
                var captcha = new _2Captcha("4ac1f746d6d2b656b6a5cb6ef9b02a21");
                var reCaptcha = captcha.SolveReCaptchaV2(keySite, "https://www.paypal.com/signin");
                reCaptcha.Wait(TimeSpan.FromSeconds(60));
                resultCode = reCaptcha.Result.Response;
                if (resultCode.Length < 30)
                {
                    throw new Exception(resultCode);
                }
            }
            catch (Exception)
            {
                var recaptchaV2ProxylessRequest = new AnyCaptcha().RecaptchaV2Proxyless("3124648601484eab8fa78500df71e30f", keySite, "https://www.paypal.com/signin");
                if (recaptchaV2ProxylessRequest.IsSuccess)
                    Console.WriteLine(recaptchaV2ProxylessRequest.Result);
                else
                    throw new Exception("Anycaptcha Lỗi: " + recaptchaV2ProxylessRequest.Message);
                resultCode = recaptchaV2ProxylessRequest.Result;
            }




            if (cssFrame != "")
            {
                var frameCaptcha = this.findElementBy(driver, By.CssSelector(cssFrame), 5, "frameCaptcha");
                driver.SwitchTo().Frame(frameCaptcha);
            }


            //const elementHandle = await page.waitForSelector('#content > form > iframe', { timeout: 5000 });

            //const frame = await elementHandle.contentFrame();

            ((IJavaScriptExecutor)driver).ExecuteScript("var list = document.getElementsByName(\"g-recaptcha-response\"); for (const item of list) { try { item.setAttribute('value', '" + resultCode + "'); } catch (error) { } }");
            ((IJavaScriptExecutor)driver).ExecuteScript("function checkCallBack(code) { for (const client in ___grecaptcha_cfg.clients) { let item = ___grecaptcha_cfg.clients[client]; if (item.callback) { item.callback(code); return ''; } for (const itemChild in item) { let item2 = item[itemChild]; if (item2.callback) { item2.callback(code); return ''; } for (const itemChild2 in item2) { let item3 = item2[itemChild2]; if (item3.callback) { item3.callback(code); return ''; } } } } } checkCallBack('" + resultCode + "');");
            if (cssFrame != "")
            {
                driver.SwitchTo().DefaultContent();
            }














            return true;
        }
        public string createExtProxy(Account account)
        {
            string profilePath = Path.Combine(Settings.Default.profilePath, account.email);

            string HOST = "";
            string PORT = "";
            string USERNAME = "";
            string PASSWORD = "";
            try
            {
                HOST = account.proxy.Split('@')[1].Split(':')[0];
                PORT = account.proxy.Split('@')[1].Split(':')[1];
                USERNAME = account.proxy.Split('@')[0].Split(':')[0];
                PASSWORD = account.proxy.Split('@')[0].Split(':')[1];
            }
            catch (Exception)
            {
                HOST = account.proxy.Split(':')[0];
                PORT = account.proxy.Split(':')[1];
                USERNAME = account.proxy.Split(':')[2];
                PASSWORD = account.proxy.Split(':')[3];
            }


            string fileBackendJs = "var config = { mode: 'fixed_servers', rules: { singleProxy: { scheme: 'http', host: '" + HOST + "', port: " + PORT + " }, bypassList: ['localhost'] } }; chrome.proxy.settings.set({value: config, scope: 'regular'}, function() {}); function callbackFn(details) { return { authCredentials: { username: '" + USERNAME + "', password: '" + PASSWORD + "' } }; } chrome.webRequest.onAuthRequired.addListener( callbackFn, {urls: ['<all_urls>']}, ['blocking'] );";
            string fileMainfrest = "{ \"version\": \"1.0.0\", \"manifest_version\": 2, \"name\": \"Chrome Proxy\", \"permissions\": [ \"proxy\", \"tabs\", \"unlimitedStorage\", \"storage\", \"<all_urls>\", \"webRequest\", \"webRequestBlocking\" ], \"background\": { \"scripts\": [\"background.js\"] }, \"minimum_chrome_version\":\"22.0.0\" }";
            if (!Directory.Exists(profilePath))
            {
                Directory.CreateDirectory(profilePath);
            }
            if (!Directory.Exists(Path.Combine(profilePath, "proxy_ext")))
            {
                Directory.CreateDirectory(Path.Combine(profilePath, "proxy_ext"));
            }

            File.WriteAllText(Path.Combine(profilePath, "proxy_ext", "background.js"), fileBackendJs);
            File.WriteAllText(Path.Combine(profilePath, "proxy_ext", "manifest.json"), fileMainfrest);

            string destinationPath = Path.Combine(profilePath, "proxy_ext");

            String archivePath = Path.Combine(profilePath, "proxy_ext.zip");

            var zipFile = new ZipFile();
            zipFile.AddDirectory(destinationPath);
            zipFile.Save(archivePath);
            zipFile.Dispose();

            // signing the hash
            byte[] zipBytes = File.ReadAllBytes(archivePath);
            var rsa = new RSACryptoServiceProvider();
            var sha1 = new SHA1Managed();
            byte[] hash = sha1.ComputeHash(zipBytes);
            String sha1Oid = CryptoConfig.MapNameToOID("SHA1");
            byte[] signature = rsa.SignHash(hash, sha1Oid);
            byte[] modulus = rsa.ExportParameters(false).Modulus;
            byte[] exponent = rsa.ExportParameters(false).Exponent;
            String rsaOid = CryptoConfig.MapNameToOID("RSA");
            byte[] objectId = CryptoConfig.EncodeOID(rsaOid);

            // constructing DER-encoded public key structure
            List<byte> publicKeyStructure = new List<byte>();
            publicKeyStructure.AddRange(new byte[] { SequenceTag, 0x81, 0x9f });
            // SEQUENCE (9F bytes), 81 - 10000001, bits 6-0 (0000001) indicates that there is one more byte needed to specify the length (9F)
            publicKeyStructure.AddRange(new byte[] { SequenceTag, 0x0d }); // SEQUENCE (D bytes)
            publicKeyStructure.AddRange(objectId);
            publicKeyStructure.AddRange(new byte[] { NullTag, 0x00 }); // parameters - NULL (0 bytes)
            publicKeyStructure.AddRange(new byte[] { BitStringTag, 0x81, 0x8d }); // BIT_STRING (8d bytes)
            publicKeyStructure.Add(0x00); // unused bits in the final byte of content
            publicKeyStructure.AddRange(new byte[] { SequenceTag, 0x81, 0x89 }); // SEQUENCE (89 bytes)
            publicKeyStructure.AddRange(new byte[] { IntegerTag, 0x81, 0x81 }); // INTEGER (81 bytes)
            publicKeyStructure.Add(0x00); // indicates that number is not negative
            publicKeyStructure.AddRange(modulus);
            publicKeyStructure.AddRange(new byte[] { IntegerTag, 0x03 }); // INTEGER (3 bytes)
            publicKeyStructure.AddRange(exponent);

            // building CRX file
            byte[] crxMagicNumber = Encoding.UTF8.GetBytes("Cr24");
            var crxVersion = new byte[4];
            crxVersion = BitConverter.GetBytes(2);
            var keyLength = new byte[4];
            keyLength = BitConverter.GetBytes(publicKeyStructure.Count);
            var signatureLength = new byte[4];
            signatureLength = BitConverter.GetBytes(signature.Length);
            List<byte> crxPackage = new List<byte>();
            crxPackage.AddRange(crxMagicNumber);
            crxPackage.AddRange(crxVersion);
            crxPackage.AddRange(keyLength);
            crxPackage.AddRange(signatureLength);
            crxPackage.AddRange(publicKeyStructure);
            crxPackage.AddRange(signature);
            crxPackage.AddRange(zipBytes);
            File.WriteAllBytes(destinationPath + ".crx", crxPackage.ToArray());
            File.Delete(archivePath);

            return destinationPath + ".crx";

        }
        public ChromeOptions getChromeOptions(string email)
        {
            string profilePath = Path.Combine(Settings.Default.profilePath, email);
            ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;
            ChromeOptions chromeOption = new ChromeOptions();
            chromeOption.AddArgument("--disable-notifications");
            chromeOption.AddArgument("--disable-blink-features=AutomationControlled");
            chromeOption.AddArguments("excludeSwitches", "enable-automation");
            chromeOption.AddArguments("useAutomationExtension", "false");
            chromeOption.AddArgument("enable-automation");
            //chromeOption.AddArgument("--app= https://www.paypal.com");
            chromeOption.AddArgument(string.Concat("--user-data-dir=", profilePath));
            chromeOption.AddArgument("ignore-certificate-errors");



            //======================================================
            //     ChromeOptions chromeOption = new ChromeOptions();
            //     chromeOption.AddArguments(new string[]
            //{
            //         "--enable-automation"
            //});
            //     chromeOption.AddAdditionalCapability("useAutomationExtension", false);
            //     chromeOption.AddArguments(new string[]
            //     {
            //         "--disable-notifications"
            //     });
            //     chromeOption.AddArguments(new string[]
            //     {
            //         "--enable-application-cache"
            //     });
            //     chromeOption.AddArguments(new string[]
            //     {
            //         "--disable-popup-blocking"
            //     });
            //     chromeOption.AddArguments(new string[]
            //     {
            //         "--disable-geolocation"
            //     });
            //     chromeOption.AddArguments(new string[]
            //     {
            //         "--no-sandbox"
            //     });
            //     chromeOption.AddArguments(new string[]
            //     {
            //         "--disable-gpu"
            //     });
            //     //chromeOptions.AddArguments(new string[]
            //     //{
            //     //"--window-size=350,550"
            //     //});
            //     //chromeOptions.AddArgument("--blink-settings=imagesEnabled=false");
            //     chromeOption.AddArgument("--lang=vi");
            //     chromeOption.AddArgument("--disable-ipv6");
            //     chromeOption.AddArgument("--disable-dev-shm-usage");
            //     chromeOption.AddArgument("--disable-impl-side-painting");
            //     chromeOption.AddArgument("--disable-setuid-sandbox");
            //     chromeOption.AddArgument("--disable-seccomp-filter-sandbox");
            //     chromeOption.AddArgument("--disable-breakpad");
            //     chromeOption.AddArgument("--disable-client-side-phishing-detection");
            //     chromeOption.AddArgument("--disable-cast");
            //     chromeOption.AddArgument("--disable-cast-streaming-hw-encoding");
            //     chromeOption.AddArgument("--disable-cloud-import");
            //     chromeOption.AddArgument("--disable-popup-blocking");
            //     chromeOption.AddArgument("--ignore-certificate-errors");
            //     chromeOption.AddArgument("--disable-session-crashed-bubble");
            //     chromeOption.AddArgument("--allow-http-screen-capture");
            //     chromeOption.AddArgument("--lang=vi");
            //     chromeOption.AddArgument("--disable-blink-features=AutomationControlled");
            //     chromeOption.AddArguments("excludeSwitches", "enable-automation");
            //     chromeOption.AddArguments("useAutomationExtension", "false");
            //     chromeOption.AddArgument("enable-automation");
            //     chromeOption.AddArgument(string.Concat("--user-data-dir=", profilePath));
            //     chromeOption.AddArgument("ignore-certificate-errors");

            //     //NEW
            //     chromeOption.AddAdditionalCapability("useAutomationExtension", false);
            //     chromeOption.AddExcludedArgument("enable-automation");
            chromeOption.BinaryLocation = Path.Combine(Application.StartupPath, "chrome", "chrome.exe"); //@"C:\Program Files\Google\Chrome\Application\chrome.exe";
            //chromeOption.AddExtension(this.createExtProxy(account));

            return chromeOption;
        }
        public DataStartDriver openChromeDriver(Account account, bool isUseProxy, int port)
        {
            string txtGPMKey = "DD00LOIRME0EUGK";

            IWebDriver driver = null;
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI2MTQ4MTlmY2NjMGI4NWY1MDI2Y2EzMTUiLCJ0eXBlIjoiZGV2Iiwiand0aWQiOiI2MTQ4ZDAwMjNhYTlhNWZmODZlZTAyYzUifQ.nDUpRUdicCZsUG_MBfrlhZIqn5pOWyVhUmp-uqLAyrQ";
            Gologin gologin = null;

            string HOST = "";
            int PORT = 0;
            string USERNAME = "";
            string PASSWORD = "";


            if (account.proxy.Count(x => x.Equals(':')) == 1)
            {
                HOST = account.proxy.Split(':')[0];
                PORT = Int32.Parse(account.proxy.Split(':')[1]);
            }
            else if (account.proxy.Count(x => x.Equals('@')) == 1 && account.proxy.Count(x => x.Equals(':')) == 2)
            {
                HOST = account.proxy.Split('@')[1].Split(':')[0];
                PORT = Int32.Parse(account.proxy.Split('@')[1].Split(':')[1]);
                USERNAME = account.proxy.Split('@')[0].Split(':')[0];
                PASSWORD = account.proxy.Split('@')[0].Split(':')[1];
            }
            else if (account.proxy.Count(x => x.Equals(':')) == 3)
            {
                HOST = account.proxy.Split(':')[0];
                PORT = Int32.Parse(account.proxy.Split(':')[1]);
                USERNAME = account.proxy.Split(':')[2];
                PASSWORD = account.proxy.Split(':')[3];
            }

            if (true)
            {

            }

            ChromeOptions chromeOptions = null;
            if (Settings.Default.browser == 2)
            {
                string profileName = account.email.Split('@')[0];
                string proxyString = HOST + ":" + PORT + ":" + USERNAME + ":" + PASSWORD;
                string profilePath = Path.Combine(Settings.Default.profilePath, profileName); //Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GPMBrowser", "profiles", profileName);
                string chromePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GPMBrowser", "gpm_browser");
                string BrowserExtensions = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GPMBrowser", "BrowserExtensions");
                List<string> listExtension = new List<string>();

                DirectoryInfo[] listExtensionFolder = new DirectoryInfo(BrowserExtensions).GetDirectories();

                foreach (var item in listExtensionFolder)
                {
                    listExtension.Add(item.FullName);
                }
                ProfileInfo profileInfo = ProfileInfo.CreateIfNotExists(profilePath, profileName, txtGPMKey, listExtension, proxyString);
                profileInfo.Name = RandomString(20);
                profileInfo.SaveToGPMFile();
                driver = profileInfo.GetDriverForRemote(chromePath, hideConsole: true);

            }
            else if (Settings.Default.browser == 1)
            {
                var profile_id = "";
                HttpRequest httpRequest = new HttpRequest();
                httpRequest.AddHeader("Authorization", "Bearer " + token);
                httpRequest.AddHeader("User-Agent", "Selenium-API");

                var res = httpRequest.Get("https://api.gologin.com/browser").ToString();
                dynamic listProfiles = JsonConvert.DeserializeObject(res);
                List<string> listIDProfile = new List<string>();
                foreach (var profile in listProfiles)
                {
                    var idProfile = (string)profile.id;
                    listIDProfile.Add(idProfile);
                }

                profile_id = listIDProfile[new Random().Next(0, listIDProfile.Count)];
                while (listProfileIDRunning.Contains(profile_id))
                {
                    profile_id = listIDProfile[new Random().Next(0, listIDProfile.Count)];
                }
                listProfileIDRunning.Add(profile_id);

                ProxyGologin proxyGologin = new ProxyGologin();
                proxyGologin.host = HOST;
                proxyGologin.mode = "http";
                proxyGologin.port = PORT;
                proxyGologin.username = USERNAME;
                proxyGologin.password = PASSWORD;
                var listParams = new List<string>();
                //listParams.Add("--headless");
                gologin = new Gologin(token, profile_id, false, proxyGologin, port, listParams);
                string debugger_address = gologin.Start();
                chromeOptions = new ChromeOptions();

                chromeOptions.DebuggerAddress = debugger_address;


                chromeOptions.BinaryLocation = Path.Combine(Application.StartupPath, "orbita-browser", "chrome.exe");
                ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService(Path.Combine(Application.StartupPath, "orbita-browser"));
                chromeDriverService.HideCommandPromptWindow = true;
                driver = new ChromeDriver(chromeDriverService, chromeOptions);
            }
            else
            {
                ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService(Path.Combine(Application.StartupPath, "chrome"));
                chromeDriverService.HideCommandPromptWindow = true;

                ChromeOptions chromeOption = this.getChromeOptions(account.email);
                chromeOption.AddUserProfilePreference("credentials_enable_service", false);
                chromeOption.AddUserProfilePreference("profile.password_manager_enabled", false);
                if (isUseProxy)
                {
                    // ====== PROXY 2======
                    Proxy proxyChrome = new Proxy()
                    {
                        Kind = ProxyKind.Manual,
                        IsAutoDetect = false,
                        HttpProxy = string.Concat(HOST, ":", PORT),
                        SslProxy = string.Concat(HOST, ":", PORT)
                    };
                    chromeOption.Proxy = proxyChrome;

                    // ====== PROXY 2======
                    chromeOption.AddExtension(Path.Combine(Application.StartupPath, "chrome", "Extension", "proxy.crx"));
                }

                driver = new ChromeDriver(chromeDriverService, chromeOption);
                driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(300);
                TimeSpan implicitWait = driver.Manage().Timeouts().ImplicitWait;
                implicitWait.Add(TimeSpan.FromSeconds(40));
                driver.Manage().Window.Position = new Point(0, 0);
                Size size = driver.Manage().Window.Size;
                int width = size.Width;
                size = driver.Manage().Window.Size;
                int height = size.Height;
                driver.Manage().Window.Size = new Size(width, height);

                ((IJavaScriptExecutor)driver).ExecuteScript(string.Format("localStorage['proxy_login'] = '{0}'; localStorage['proxy_password'] = '{1}'", USERNAME, PASSWORD), new object[0]);

                if (isUseProxy)
                {
                    // ====== PROXY 2======
                    ReadOnlyCollection<string> windowHandles = driver.WindowHandles;
                    string firstTab = windowHandles.First();
                    string lastTab = windowHandles.Last();
                    driver.SwitchTo().Window(firstTab);
                    for (int i = 0; i < 10; i++)
                    {
                        try
                        {
                            driver.FindElement(By.Id("login")).Clear();
                            driver.FindElement(By.Id("login")).SendKeys(USERNAME);
                            Thread.Sleep(100);
                            driver.FindElement(By.Id("password")).Clear();
                            driver.FindElement(By.Id("password")).SendKeys(PASSWORD);
                            Thread.Sleep(100);
                            driver.FindElement(By.Id("save")).Click();
                            Thread.Sleep(1000);
                            driver.Close();
                            driver.SwitchTo().Window(lastTab);
                            break;
                        }
                        catch { }
                        Thread.Sleep(1000);
                    }
                }

            }

            return new DataStartDriver { driver = driver, gologin = gologin };

        }
        public void checkChrome(Account account, int port)
        {
            DataStartDriver dataStartDriver = openChromeDriver(account, true, port);
            var driver = dataStartDriver.driver;
            var gologin = dataStartDriver.gologin;

            driver.Navigate().GoToUrl("https://pixelscan.net/");
            ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
            driver.SwitchTo().Window(driver.WindowHandles.Last());
            driver.Navigate().GoToUrl("http://whoer.net");

        }
        public ResultLogin Login(Account account, bool isUseProxy, int port)
        {
            IWebDriver driver = null;
            var newPassword = "";
            Gologin gologin = null;
            try
            {
                DataStartDriver dataStartDriver = openChromeDriver(account, isUseProxy, port);
                driver = dataStartDriver.driver;
                gologin = dataStartDriver.gologin;

                bool isLogin = false;

                var urlLogin = "https://www.paypal.com/signin";
                if (Settings.Default.browser == 1 && (account.cookie == null || account.cookie == ""))
                {
                    urlLogin = "https://www.paypal.com/us/signin";
                }

                if (account.cookie != null && account.cookie != "")
                {
                    driver.Navigate().GoToUrl(urlLogin);
                    if (!driver.Url.Contains("dashboard") && !driver.Url.Contains("summary"))
                    {
                        AllCookie cookieParser = JsonConvert.DeserializeObject<AllCookie>(account.cookie);
                        foreach (var item in cookieParser.AllCookies)
                        {
                            OpenQA.Selenium.Cookie newCookie = new OpenQA.Selenium.Cookie(item.Name, item.Value, item.Domain, item.Path, item.Expiry);
                            driver.Manage().Cookies.AddCookie(newCookie);
                        }
                    }
                    else
                    {
                        isLogin = true;
                    }

                }
                if (!isLogin)
                {
                    driver.Navigate().GoToUrl(urlLogin);
                    if (!driver.Url.Contains("dashboard") && !driver.Url.Contains("summary"))
                    {
                        isLogin = false;
                    }
                    else
                    {
                        isLogin = true;
                    }
                }

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));


                if (!isLogin)
                {
                    var keySiteEl = this.findElementByWithoutErrorNotAwaitCLickAble(driver, By.CssSelector("input[name=\"_adsRecaptchaSiteKey\"]"), 1);
                    if (keySiteEl != null)
                    {
                        var keySite = keySiteEl.GetAttribute("value");
                        if (keySite != null && keySite != "")
                        {
                            this.resovleCaptchaAsync(driver, keySite, "#content > form > iframe");
                            Thread.Sleep(5000);
                        }
                    }

                    try
                    {
                        var requireCodePhones = this.findElementByWithoutError(driver, By.Id("security_codediv"), 3);
                        if (requireCodePhones != null)
                        {
                            //driver.Manage().Cookies.DeleteAllCookies();
                            //driver.Navigate().GoToUrl(urlLogin);
                            //Thread.Sleep(2000);
                            try
                            {
                                var btnAcceptAll = this.findElementBy(driver, By.Id("acceptAllButton"), 1);
                                if (btnAcceptAll != null)
                                {
                                    btnAcceptAll.Click();
                                }
                            }
                            catch (Exception)
                            {
                            }
                            Thread.Sleep(2000);
                            var tryAnotherWayLink = this.findElementBy(driver, By.Id("tryAnotherWayLink"), 3);
                            tryAnotherWayLink.Click();
                            Thread.Sleep(2000);
                            var loginWithPassword = this.findElementBy(driver, By.Id("loginWithPassword"), 3);
                            loginWithPassword.Click();
                            Thread.Sleep(2000);

                            //var tryAnotherWayLinks = this.findElementBy(driver, By.Id("tryAnotherWayLink"), 1);
                            //tryAnotherWayLinks.Click();
                            //Thread.Sleep(3000);
                            //var loginWithPasswords = this.findElementBy(driver, By.Id("loginWithPassword"), 1);
                            //loginWithPasswords.Click();
                        }
                    }
                    catch (Exception)
                    {
                    }


                    var timeout = 30;
                    bool isNewPass = false;

                    while (!isLogin && timeout > 0)
                    {

                        var inputEmail = this.findElementByWithoutError(driver, By.CssSelector("input[type=\"email\""), 1);
                        if (inputEmail != null && inputEmail.GetAttribute("value") != account.email)
                        {
                            inputEmail.Clear();
                            for (int i = 0; i < account.email.Length; i++)
                            {
                                inputEmail.SendKeys(account.email[i].ToString());
                                Thread.Sleep(100);
                            }
                        }

                        try
                        {
                            var requireCodePhones = this.findElementByWithoutError(driver, By.Name("sendOtp"), 3);
                            if (requireCodePhones != null)
                            {
                                var loginWithPassword = this.findElementBy(driver, By.CssSelector("p[class=\"secondaryLink \"]"), 3);
                                loginWithPassword.Click();
                                Thread.Sleep(2000);

                            }
                        }
                        catch (Exception)
                        {
                        }

                        try
                        {
                            var requireCodePhones = this.findElementByWithoutError(driver, By.Id("security_codediv"), 1);
                            if (requireCodePhones != null)
                            {
                                //driver.Manage().Cookies.DeleteAllCookies();
                                //driver.Navigate().GoToUrl(urlLogin);
                                //Thread.Sleep(2000);
                                try
                                {
                                    var btnAcceptAll = this.findElementBy(driver, By.Id("acceptAllButton"), 1, "", 3);
                                    if (btnAcceptAll != null)
                                    {
                                        btnAcceptAll.Click();
                                        Thread.Sleep(2000);
                                    }
                                }
                                catch (Exception)
                                {
                                }
                                var tryAnotherWayLink = this.findElementBy(driver, By.Id("tryAnotherWayLink"), 1, "", 3);
                                tryAnotherWayLink.Click();
                                Thread.Sleep(2000);

                                var loginWithPassword = this.findElementBy(driver, By.Id("loginWithPassword"), 1, "", 3);
                                loginWithPassword.Click();
                                Thread.Sleep(2000);

                            }
                        }
                        catch (Exception)
                        {
                        }

                        var btnNext = this.findElementByWithoutError(driver, By.CssSelector("#btnNext"), 1, "btnNext");
                        if (btnNext != null)
                        {
                            btnNext.Click();
                        }
                        Thread.Sleep(1000);

                        var inputPassword = this.findElementByWithoutError(driver, By.CssSelector("input[type=\"password\""), 1, "inputPassword");
                        if (inputPassword != null && inputPassword.Text != account.password)
                        {
                            inputPassword.Clear();
                            for (int i = 0; i < account.password.Length; i++)
                            {
                                inputPassword.SendKeys(account.password[i].ToString());
                                Thread.Sleep(100);
                            }
                        }

                        var btnLogin = this.findElementByWithoutError(driver, By.CssSelector("button[name=\"btnLogin\"]"), 1, "btnNext");
                        if (btnLogin != null)
                        {
                            btnLogin.Click();
                        }

                        //========= Check Success==========
                        if (driver.Url.Contains("myaccount/summary") || driver.Url.Contains("dashboard"))
                        {
                            isLogin = true;

                            break;
                        }




                        var notiNode = this.findElementByWithoutError(driver, By.CssSelector("p.notification.notification-critical"), 1);
                        if (notiNode != null)
                        {
                            string textNoti = notiNode.Text;
                            if (textNoti != null)
                            {
                                throw new Exception(textNoti);
                            }
                        }
                        // New Pass
                        var newPass = this.findElementByWithoutError(driver, By.CssSelector("div.createPasswordPage"), 1);
                        if (newPass != null)
                        {
                            try
                            {
                                var inputNewPass = this.findElementBy(driver, By.CssSelector("input[name=\"password\"]"), 5);
                                var inputConfirmNewPass = this.findElementBy(driver, By.CssSelector("input[name=\"confirmPassword\"]"), 5);
                                if (inputNewPass != null && inputConfirmNewPass != null)
                                {
                                    newPassword = this.RandomPassword(15);
                                    inputNewPass.SendKeys(newPassword);
                                    Thread.Sleep(1000);
                                    inputConfirmNewPass.SendKeys(newPassword);
                                    Thread.Sleep(1000);
                                    this.findElementBy(driver, By.CssSelector("input[name=\"submitPassword\"]"), 5, "Button Submit Password").Click();
                                    Thread.Sleep(1000);
                                    isNewPass = true;



                                }
                            }
                            catch (Exception ex)
                            {
                                newPassword = "";
                                throw new Exception("Lỗi tạo password mới: " + ex.Message);
                            }
                        }

                        if (isNewPass)
                        {
                            inputPassword = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input[type=\"password\"")));
                            //var inputPassword = this.findElementBy(driver, By.CssSelector("input[type=\"password\""), 5, "inputPassword");
                            if (inputPassword.Text != newPassword)
                            {
                                inputPassword.Clear();
                                for (int i = 0; i < newPassword.Length; i++)
                                {
                                    inputPassword.SendKeys(newPassword[i].ToString());
                                    Thread.Sleep(100);
                                }
                            }
                            new WebDriverWait(driver, TimeSpan.FromSeconds(60)).Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[name=\"btnLogin\"]"))).Click();

                            isNewPass = false;
                        }

                        var error1 = this.findElementByWithoutError(driver, By.CssSelector("#content > div.notifications > p"), 1);
                        if (error1 != null && error1.Text != "")
                        {
                            throw new Exception(error1.Text);
                        }


                        var skip1 = this.findElementByWithoutError(driver, By.CssSelector("div[data-testid=\"skipLink-component\"]"), 1);
                        if (skip1 != null)
                        {
                            skip1.Click();
                        }
                        // ====== Check 5$ ============
                        try
                        {
                            var imageClaim5Dola = this.findElementByWithoutError(driver, By.CssSelector("#main > section > div.containerCentered > div > a > img"), 1);
                            if (imageClaim5Dola != null)
                            {
                                imageClaim5Dola.Click();
                                Thread.Sleep(3000);
                                driver.Url = "https://www.paypal.com/signin";
                                Thread.Sleep(1000);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("5$: " + ex.Message);
                        }
                        // ====== Check Google Captcha =========
                        try
                        {
                            keySiteEl = this.findElementByWithoutErrorNotAwaitCLickAble(driver, By.CssSelector("input[name=\"_adsRecaptchaSiteKey\"]"), 1);
                            if (keySiteEl != null)
                            {
                                var keySite = keySiteEl.GetAttribute("value");
                                if (keySite != null && keySite != "")
                                {
                                    this.resovleCaptchaAsync(driver, keySite, "#content > form > iframe");
                                    Thread.Sleep(5000);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Captcha: " + ex.Message);
                        }

                        // ====== Check Arock Captcha =========
                        try
                        {
                            var checkCaptchaFrame = this.findElementByWithoutErrorNotAwaitCLickAble(driver, By.CssSelector("iframe[name=\"recaptcha\"]"), 1);
                            if (checkCaptchaFrame != null)
                            {
                                var keySite = checkCaptchaFrame.GetAttribute("src");
                                //https://www.paypalobjects.com/web/res/5fd/a5cb801457073d6381adb8a348a84/arkose/arkose.html?siteKey=9409E63B-D2A5-9CBD-DBC0-5095707D0090&locale.x=en_US&country.x=US&checkConnectionTimeout=10000&domain=paypal-api.arkoselabs.com%2Ffc%2Fapi&callback=arkoseCallback
                                keySite = getBetween(keySite, "siteKey=", "&");
                                if (keySite != null && keySite != "")
                                {
                                    driver.SwitchTo().Frame(checkCaptchaFrame);
                                    var codeResult = resolveFunCaptcha(keySite, "https://www.paypal.com/signin");
                                    ((IJavaScriptExecutor)driver).ExecuteScript("var e = getAllUrlParams(window.location.href); 'xbox_1' === e.uitheme ? navigator.__defineGetter__('userAgent', function () { return 'Xbox_Dark'; }) : 'xbox_2' === e.uitheme ? navigator.__defineGetter__('userAgent', function () { return 'Xbox_Light'; }) : 'win8wiz' === e.uitheme && navigator.__defineGetter__('userAgent', function () { return 'Win8Wiz'; }); var t = window.location.pathname.split('/'), a = new ArkoseEnforcement({ public_key: t[1], language: e.mkt, target_html: 'arkose', callback: function () { parent.postMessage(JSON.stringify({ eventId: 'challenge-complete', payload: { sessionToken: a.getSessionToken() } }), '*'); }, loaded_callback: function () { (frameHeight = document.getElementById('fc-iframe-wrap').offsetHeight), (frameWidth = document.getElementById('fc-iframe-wrap').offsetWidth), 'xbox_1' === e.uitheme || 'xbox_2' === e.uitheme ? a.enableDirectionalInput() : e.uitheme, parent.postMessage( JSON.stringify({ eventId: 'challenge-loaded', payload: { sessionToken: a.getSessionToken(), frameHeight: frameHeight, frameWidth: frameWidth }, }), '*' ), (loadedCheck = 1); }, onsuppress: function () { parent.postMessage(JSON.stringify({ eventId: 'challenge-suppressed', payload: { sessionToken: a.getSessionToken() } }), '*'); }, onshown: function () { parent.postMessage(JSON.stringify({ eventId: 'challenge-shown', payload: { sessionToken: a.getSessionToken() } }), '*'); }, }); var token = '" + codeResult + "'; document.querySelector('#verification-token').value = token; document.querySelector('#FunCaptcha-Token').value = token; a.callback();");
                                    Thread.Sleep(3000);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Captcha: " + ex.Message);
                        }

                        // ====== Check Phone =========
                        try
                        {

                            var confirmPhone = this.findElementByWithoutError(driver, By.CssSelector("#content > form > div.phoneNumbers"), 1);
                            if (confirmPhone != null)
                            {
                                var btnConfirmPhone = this.findElementBy(driver, By.CssSelector("#content > form > p.secondaryLink > a"), 2, "btnConfirmPhone");
                                btnConfirmPhone.Click();
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Check Phone: " + ex.Message);
                        }

                        //====== Check 2FA ========
                        try
                        {

                            var inputOtp = this.findElementByWithoutError(driver, By.CssSelector("#otpCode"), 1);
                            if (inputOtp != null)
                            {
                                var newToken = this.Get2FA(account.twoFA.Replace(" ", ""));
                                if (newToken == null || newToken == "")
                                {
                                    throw new Exception("Lỗi lấy token 2fa");
                                }
                                for (int i = 0; i < newToken.Length; i++)
                                {
                                    inputOtp = this.findElementByWithoutError(driver, By.CssSelector("#ci-otpCode-" + i), 1);
                                    inputOtp.SendKeys(newToken[i].ToString());
                                }

                                Thread.Sleep(1000);
                                try
                                {
                                    var skipTwofactorCheckbox = this.findElementBy(driver, By.CssSelector("#skipTwofactorCheckbox"), 3, "skipTwofactorCheckbox");
                                    skipTwofactorCheckbox.Click();
                                }
                                catch (Exception)
                                {
                                }
                                Thread.Sleep(1000);
                                var btnSubmit2Fa = this.findElementBy(driver, By.CssSelector("button[type=\"submit\"]"), 3, "btnSubmit2Fa");
                                Thread.Sleep(1000);
                                btnSubmit2Fa.Click();
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("2FA: " + ex.Message);
                        }

                        //========= Check Verify mobile======
                        try
                        {
                            var confirmPhone = this.findElementByWithoutError(driver, By.CssSelector("button[data-nemo=\"entrySubmit\"]"), 1);
                            if (confirmPhone != null)
                            {
                                confirmPhone.Click();
                            }
                        }
                        catch (Exception)
                        {
                        }
                        //

                        // ====== Check Phone =========
                        var verifyPhone = this.findElementByWithoutError(driver, By.CssSelector("#sms-challenge-option"), 1);
                        if (verifyPhone != null)
                        {
                            throw new Exception("Verify Phone: ");
                        }

                        timeout -= 5;
                        Thread.Sleep(1000);

                        //========= Check REquire code phone ======
                        try
                        {
                            var requireCodePhone = this.findElementByWithoutError(driver, By.Id("security_codediv"), 1);
                            if (requireCodePhone != null)
                            {
                                try
                                {
                                    var btnAcceptAll = this.findElementBy(driver, By.Id("acceptAllButton"), 3, "", 3);
                                    if (btnAcceptAll != null)
                                    {
                                        btnAcceptAll.Click();
                                        Thread.Sleep(2000);
                                    }
                                }
                                catch (Exception)
                                {
                                }
                                var tryAnotherWayLink = this.findElementBy(driver, By.Id("tryAnotherWayLink"), 3, "", 3);
                                tryAnotherWayLink.Click();
                                Thread.Sleep(2000);
                                var loginWithPassword = this.findElementBy(driver, By.Id("loginWithPassword"), 3, "", 3);
                                loginWithPassword.Click();
                                Thread.Sleep(2000);
                            }
                            else
                            {
                            }

                        }
                        catch (Exception)
                        {
                        }

                        try
                        {
                            var requireCodePhones = this.findElementByWithoutError(driver, By.Name("sendOtp"), 3);
                            if (requireCodePhones != null)
                            {
                                var loginWithPassword = this.findElementBy(driver, By.CssSelector("p[class=\"secondaryLink \"]"), 3);
                                loginWithPassword.Click();
                                Thread.Sleep(2000);

                            }
                        }
                        catch (Exception)
                        {
                        }


                        try
                        {
                            var loginWithPassword = this.findElementBy(driver, By.Id("loginWithPassword"), 3, "", 3);
                            if (loginWithPassword != null)
                            {
                                loginWithPassword.Click();
                            }
                        }
                        catch (Exception)
                        {
                        }
                        //

                    }

                    if (!isLogin)
                    {
                        throw new Exception("Timeout check login...!");
                    }
                }


                var newCookies = driver.Manage().Cookies;
                string cookie = JsonConvert.SerializeObject(newCookies);
                return new ResultLogin
                {
                    status = "SUCCESS",
                    driver = driver,
                    cookie = cookie,
                    newPassword = newPassword,
                    gologin = gologin
                };
            }
            catch (Exception e)
            {
                return new ResultLogin
                {
                    status = "ERROR",
                    message = e.Message + "- " + e.StackTrace,
                    driver = driver,
                    newPassword = newPassword,
                    gologin = gologin
                };
            }

        }
        public ResultGetHistory CheckHistoryBusiness(IWebDriver driver)
        {
            string history = "";
            string idTransaction = "";
            string mailReceive = "";
            string status = "";
            int countTransaction = 0;
            string amountAccount = "";
            try
            {
                //
                IWebElement amountNode = null;

                try
                {
                    amountNode = this.findElementBy(driver, By.CssSelector("#money-fragment > div > div > div.css-1edrbbx.e14y2lnn0 > div:nth-child(1) > div.css-1bncj8b.e1pqnvk70 > span"), 5, "Amount Account");
                }
                catch (Exception)
                {
                    amountNode = this.findElementBy(driver, By.XPath("//*[@id=\"money-fragment\"]/div/div/div[2]/div[1]/div[1]/span"), 5, "Amount Account");
                }

                amountAccount = amountNode.Text;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi đọc số dư tài khoản: " + ex.Message);
            }

            // Đọc thông báo
            try
            {
                var listNoti = this.findElementsBy(driver, By.CssSelector("div.vx_panel-tile"), 5);
                if (listNoti != null && listNoti.Count > 0)
                {
                    foreach (var parentNoti in listNoti)
                    {
                        var seeDetailsNode = parentNoti.FindElement(By.CssSelector("div > div:nth-child(3)"));
                        if (seeDetailsNode != null && (seeDetailsNode.Text.Contains("See details") || seeDetailsNode.Text.Contains("Xem chi tiết")))
                        {
                            status = parentNoti.FindElement(By.CssSelector("div > div:nth-child(1)")).Text;
                            break;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi đọc thông báo: " + ex.Message);
            }


            //Last Transaction
            try
            {
                var parentNodeTransaction = this.findElementBy(driver, By.CssSelector("div[data-testid=\"transactions\"]"), 5);
                if (parentNodeTransaction != null)
                {
                    // 
                    //
                    IWebElement infoTransactionNode = null;
                    IWebElement firstTransaction = null;
                    try
                    {
                        infoTransactionNode = parentNodeTransaction.FindElement(By.CssSelector("div > div:nth-child(2) > div:nth-child(2) > div > table > tbody > tr:nth-child(1)"));

                    }
                    catch (Exception)
                    {
                        infoTransactionNode = parentNodeTransaction.FindElement(By.CssSelector("div > div:nth-child(2) > div > div > table > tbody > tr:nth-child(1)"));
                    }
                    history = infoTransactionNode.Text.Replace("\r\n-\r\nnegative\r\n", " -").Replace("\r\n", " ").Replace("+ ", " +");

                    try
                    {
                        firstTransaction = parentNodeTransaction.FindElement(By.CssSelector("div > div:nth-child(2) > div:nth-child(2) > div > table > tbody > tr:nth-child(1) > td:nth-child(2) > div > a"));
                    }
                    catch (Exception)
                    {
                        firstTransaction = parentNodeTransaction.FindElement(By.CssSelector("div > div:nth-child(2) > div > div > table > tbody > tr:nth-child(1) > td > div > a"));
                    }

                    var url = firstTransaction.GetAttribute("href");

                    driver.Url = url;

                    var idTransactionNode = this.findElementBy(driver, By.CssSelector("div[data-testid=\"tdheader_id\"]"), 5, "ID transaction node");

                    idTransaction = idTransactionNode.Text.Contains(":") ? idTransactionNode.Text.Split(':')[1] : idTransactionNode.Text;


                    //var mailReceiveNode = this.findElementBy(driver, By.CssSelector("section.NameEmailPaidBy.pagebreak > div:nth-child(2) > div"), 1, "Mail Receive");
                    //mailReceive = mailReceiveNode.Text;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi đọc lịch sử: " + ex.Message);
            }


            //// Lấy số lần giao dịch
            //try
            //{
            //    DateTime now = DateTime.Now;
            //    string firstTime = now.Year + "-01-01";
            //    string lastTime = now.Year + "-" + now.Month + "-" + now.Day;
            //    string urlActivity = "https://www.paypal.com/myaccount/transactions/?free_text_search=&filter_id=&currency=ALL&issuance_product_name=&asset_names=&asset_symbols=&type=&status=&start_date=" + firstTime + "&end_date=" + lastTime;

            //    driver.Navigate().GoToUrl(urlActivity);

            //    var listTransactionNode = this.findElementsBy(driver, By.CssSelector("ul.transactionList.js_transactionList > li"), 30);
            //    if (listTransactionNode != null && listTransactionNode.Count > 0)
            //    {
            //        countTransaction = listTransactionNode.Count - 1;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Lỗi đọc số lần giao dịch: " + ex.Message);
            //}
            ResultGetHistory resultGetHistory = new ResultGetHistory();
            resultGetHistory.resultLimit = status;
            resultGetHistory.history = history;
            resultGetHistory.amount = amountAccount;
            //resultGetHistory.countTransaction = "1 năm qua: " + countTransaction;
            resultGetHistory.idTransaction = idTransaction;
            resultGetHistory.mailReceive = mailReceive;



            return resultGetHistory;
        }
        public ResultGetHistory CheckHistory(IWebDriver driver)
        {
            string history = "";
            string idTransaction = "";
            string mailReceive = "";
            string status = "";
            int countTransaction = 0;
            string amountAccount = "";
            try
            {
                //span[data-test-id=\"available-balance\"]

                var amountNode = this.findElementBy(driver, By.CssSelector("div[data-test-id=\"available-balance\"]"), 5, "Amount Account");
                amountAccount = amountNode.Text;
            }
            catch (Exception ex)
            {
                try
                {
                    var amountNode = this.findElementBy(driver, By.CssSelector("span[data-test-id=\"available-balance\"]"), 1, "Amount Account");
                    amountAccount = amountNode.Text;
                }
                catch (Exception)
                {
                }
                Console.WriteLine("Lỗi đọc số dư tài khoản: " + ex.Message);
            }

            //Last Transaction
            try
            {
                //var firstTransaction = this.findElementBy(driver, By.CssSelector("#js_transactionCollection > section > ul > li:nth-child(1)"), 5, "First Transaction");
                //firstTransaction.Click();
                //var infoTransactionNode = this.findElementBy(driver, By.CssSelector("#js_transactionDetailsHeaderView > div"), 5, "Info Transaction");
                //history = infoTransactionNode.Text;
                //history = history.Replace("\r\n-\r\nnegative\r\n", " -").Replace("\r\n", " ").Replace("+ ", " +");

                //
                var firstTransaction = this.findElementByWithoutError(driver, By.CssSelector("#js_transactionCollection > section > ul > li:nth-child(1)"), 1, "First Transaction");
                IWebElement infoTransactionNode = null;
                //aria-labelledby="activityModuleHeaderWidget"
                if (firstTransaction == null)
                {
                    firstTransaction = this.findElementByWithoutError(driver, By.CssSelector("div[aria-labelledby=\"activityModuleHeaderWidget\"] > div:nth-child(1)"), 1, "First Transaction");
                    if (firstTransaction == null)
                    {
                        firstTransaction = this.findElementByWithoutError(driver, By.CssSelector("div.js_linkedBlock.js_transactionItem"));

                    }
                    infoTransactionNode = firstTransaction;
                }
                else
                {
                    infoTransactionNode = firstTransaction.FindElement(By.CssSelector("div.transactionDescriptionContainer.js_transactionDescriptionContainer"));

                }

                if (infoTransactionNode != null)
                {
                    history = infoTransactionNode.Text;
                    history = history.Replace("\r\n-\r\nnegative\r\n", " -").Replace("\r\n", " ").Replace("+ ", " +");
                }
                if (firstTransaction == null)
                {
                    throw new Exception("Không tìm thấy giao dịch");
                }
                firstTransaction.Click();

                if (history == "")
                {
                    infoTransactionNode = this.findElementBy(driver, By.CssSelector("#js_transactionDetailsHeaderView > div"), 5, "Info Transaction");
                    history = infoTransactionNode.Text;
                    history = history.Replace("\r\n-\r\nnegative\r\n", " -").Replace("\r\n", " ").Replace("+ ", " +");
                }

                var listNodeTransaction = this.findElementsBy(driver, By.CssSelector("div.ppvx_col-md.mobileTransactionDetails.halfColForPrint > dl"), 5);

                foreach (var item in listNodeTransaction)
                {
                    if (item.Text.Contains("Transaction") || item.Text.Contains("Số giao dịch"))
                    {
                        idTransaction = item.FindElement(By.CssSelector("dd")).Text;
                        break;
                    }
                }
                if (idTransaction == "")
                {
                    var nodeTran = this.findElementBy(driver, By.CssSelector("#td_transactionId > div:nth-child(2)"), 5);
                    if (nodeTran != null)
                    {
                        idTransaction = nodeTran.Text;
                    }
                }
                //var mailReceiveNode = this.findElementBy(driver, By.CssSelector("div.ppvx_col-md.mobileTransactionDetails.halfColForPrint > dl > dd > a.customAnchor"), 5, "Mail Receive");
                //mailReceive = mailReceiveNode.Text;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi đọc lịch sử: " + ex.Message);
            }

            // Đọc thông báo
            try
            {
                try
                {
                    var buttonNoti = this.findElementBy(driver, By.CssSelector("#header-notifications"), 5, "Icon Noti");
                    buttonNoti.Click();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                Thread.Sleep(1000);
                var notiNode = this.findElementBy(driver, By.CssSelector("#cw_panel-notifications > div > div"), 5, "Noti Node");
                status = notiNode.Text;
                //if (status == "Urgent" || status == "Khẩn cấp")
                //{
                var messageNotiNode = this.findElementBy(driver, By.CssSelector("div.cw_notifications-header_new"), 1, "Message Noti Node");
                status += "- " + messageNotiNode.Text;
                var listMessageNotis = this.findElementsBy(driver, By.CssSelector("div.cw_notification-description.cw_notification-description_new > span:nth-child(2)"), 1);
                var statusMessage = "";
                foreach (var itemNoti in listMessageNotis)
                {
                    Debug.WriteLine(itemNoti.Text);
                    if (itemNoti.Text != null && itemNoti.Text.Contains("limit"))
                    {
                        statusMessage += itemNoti.Text;
                        break;
                    }
                }
                if (statusMessage == "")
                {
                    statusMessage += listMessageNotis[0].Text;
                }
                status += "- " + statusMessage;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi đọc thông báo: " + ex.Message);
            }

            // Lấy số lần giao dịch
            try
            {
                DateTime now = DateTime.Now;
                string firstTime = now.Year + "-01-01";
                string lastTime = now.Year + "-" + now.Month + "-" + now.Day;
                string urlActivity = "https://www.paypal.com/myaccount/transactions/?free_text_search=&filter_id=&currency=ALL&issuance_product_name=&asset_names=&asset_symbols=&type=&status=&start_date=" + firstTime + "&end_date=" + lastTime;

                driver.Navigate().GoToUrl(urlActivity);

                var listTransactionNode = this.findElementsBy(driver, By.CssSelector("ul.transactionList.js_transactionList > li"), 30);
                if (listTransactionNode != null && listTransactionNode.Count > 1)
                {
                    countTransaction = listTransactionNode.Count - 1;
                    try
                    {
                        listTransactionNode[1].Click();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi đọc số lần giao dịch: " + ex.Message);
            }
            ResultGetHistory resultGetHistory = new ResultGetHistory();
            resultGetHistory.resultLimit = status;
            resultGetHistory.history = history;
            resultGetHistory.amount = amountAccount;
            resultGetHistory.countTransaction = "1 năm qua: " + countTransaction;
            resultGetHistory.idTransaction = idTransaction;
            resultGetHistory.mailReceive = mailReceive;



            return resultGetHistory;
        }
        public ResultGetInfoAccout GetInfoAccount(IWebDriver driver)
        {
            ResultGetInfoAccout resultGetInfoAccout = new ResultGetInfoAccout();
            resultGetInfoAccout.KQKTDG = "";
            driver.Navigate().GoToUrl("https://www.paypal.com/myaccount/settings/");
            Thread.Sleep(3000);
            try
            {
                //== Name
                var nameNode = this.findElementBy(driver, By.CssSelector("p.vx_h3"), 3, "Name Node");
                resultGetInfoAccout.name = nameNode.Text;

                var spanPrimary = this.findElementByWithoutError(driver, By.Id("email_section_header_id"), 1);
                var parrentPrimary = spanPrimary.FindElement(By.XPath("..")).FindElement(By.XPath(".."));

                if (parrentPrimary.Text.Contains("CHƯA XÁC NHẬN") || parrentPrimary.Text.Contains("UNCONFIRMED"))
                {
                    resultGetInfoAccout.KQKTDG += "Mail chính chưa xác minh- " + resultGetInfoAccout.KQKTDG;
                }
                var text = parrentPrimary.Text;


            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi đọc settings: " + ex.Message);
            }
            try
            {
                //== Name
                var nodeMailPhu = this.findElementBy(driver, By.CssSelector("ul.emails.unstyled.lined > li:nth-child(2) > div > span"), 3, "Email Node");
                resultGetInfoAccout.emailPhu = nodeMailPhu.Text;

                var parrentMailPhu = nodeMailPhu.FindElement(By.XPath("..")).FindElement(By.XPath(".."));

                if (parrentMailPhu.Text.Contains("CHƯA XÁC NHẬN") || parrentMailPhu.Text.Contains("UNCONFIRMED"))
                {
                    resultGetInfoAccout.KQKTDG += " Mail phụ chưa xác minh- " + resultGetInfoAccout.KQKTDG;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi đọc settings: " + ex.Message);
            }

            try
            {
                //== Date create
                var dateCreateNode = this.findElementBy(driver, By.CssSelector("p.profileDetail-joinedSince"), 1, "Date Create Node");
                resultGetInfoAccout.dateCreate = dateCreateNode.Text;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi đọc settings: " + ex.Message);
            }
            try
            {
                //== Address
                var addressNode1 = this.findElementBy(driver, By.CssSelector("div.address > div:nth-child(1)"), 1, "Address 1 Node");
                resultGetInfoAccout.address = addressNode1.Text;
                var addressNode2 = this.findElementBy(driver, By.CssSelector("div.address > div:nth-child(2)"), 1, "Address 2 Node");
                resultGetInfoAccout.address += ", " + addressNode2.Text;
                var addressNode3 = this.findElementBy(driver, By.CssSelector("div.address > div:nth-child(3)"), 1, "Address 3 Node");
                resultGetInfoAccout.address += ", " + addressNode3.Text;
                var addressNode4 = this.findElementBy(driver, By.CssSelector("div.address > div:nth-child(4)"), 1, "Address 4 Node");
                resultGetInfoAccout.address += ", " + addressNode4.Text;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi đọc settings: " + ex.Message);
            }
            try
            {
                var spanPrimary = this.findElementByWithoutError(driver, By.XPath("//span[contains(text(), 'Primary')]"), 1);
                if (spanPrimary == null)
                {
                    spanPrimary = this.findElementByWithoutError(driver, By.XPath("//span[contains(text(), 'CHÍNH')]"), 1); ;
                }
                var parrentPrimary = spanPrimary.FindElement(By.XPath(".."));
                resultGetInfoAccout.ttVeryMail = parrentPrimary.Text;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi đọc settings: " + ex.Message);
            }

            try
            {
                // Birthday
                driver.Navigate().GoToUrl("https://www.paypal.com/policy/hub/cip/L/tasks/58ceac8f7c04f05a39e21084591d99cd");
                Thread.Sleep(3000);
                var inputBirth = driver.FindElement(By.CssSelector("input[name=\"date_of_birth\"]"));
                resultGetInfoAccout.birthday = inputBirth.GetAttribute("value");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi đọc lịch sử: " + ex.Message);
            }

            return resultGetInfoAccout;
        }


        public ResultCreateSecureAnd2FA createSecureQuestion(IWebDriver driver)
        {
            string securityQuestion = this.RandomString(10);
            string twoFA = "";
            driver.Navigate().GoToUrl("https://www.paypal.com/myaccount/security/");

            try
            {
                // ===== Secure =====
                var SecureQuestion = this.findElementBy(driver, By.CssSelector("#main > div > div:nth-child(4)"), 10, "Secure Question");
                SecureQuestion.Click();
                Thread.Sleep(1000);
                SelectElement cbbQuestion1 = new SelectElement(this.findElementBy(driver, By.CssSelector("select[name=\"question1\"]"), 10, "Question 1 Node"));
                cbbQuestion1.SelectByIndex(new Random().Next(1, 8));
                Thread.Sleep(500);

                var inputAnswer1 = this.findElementBy(driver, By.CssSelector("input[name=\"answer1\"]"), 5, "Answer 1 Node");
                inputAnswer1.SendKeys(securityQuestion);
                Thread.Sleep(500);
                SelectElement cbbQuestion2 = new SelectElement(this.findElementBy(driver, By.CssSelector("select[name=\"question2\"]"), 10, "Question 2 Node"));
                cbbQuestion2.SelectByIndex(new Random().Next(1, 8));
                Thread.Sleep(500);
                var inputAnswer2 = this.findElementBy(driver, By.CssSelector("input[name=\"answer2\"]"), 5, "Answer 1 Node");
                inputAnswer2.SendKeys(securityQuestion);
                Thread.Sleep(500);
                var btnSubmit = this.findElementBy(driver, By.CssSelector("button[type=\"submit\"]"), 5, "Button Question Submit");
                btnSubmit.Click();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi set Question Security: " + ex.Message);
            }

            return new ResultCreateSecureAnd2FA { securityQuestion = securityQuestion, twoFA = twoFA };

        }
        public ResultCreateSecureAnd2FA createSecureAnd2FA(IWebDriver driver)
        {
            string securityQuestion = this.RandomString(10);
            string twoFA = "";
            driver.Navigate().GoToUrl("https://www.paypal.com/myaccount/security/");


            try
            {
                // ===== 2FA =====
                var TwoFANode = this.findElementBy(driver, By.CssSelector("#main > div > div:nth-child(2)"), 10, "Two FA Node");
                TwoFANode.Click();
                Thread.Sleep(2000);
                this.findElementBy(driver, By.CssSelector("#_flowModal-header"), 5, "Checkbox 2FA Node").Click();
                var checkbox2FANode = this.findElementBy(driver, By.CssSelector("label[for=\"twofactor_setup_type_SOFTWARE_TOKEN_AUTHENTICATOR\"]"), 5, "Checkbox 2FA Node");
                checkbox2FANode.Click();
                Thread.Sleep(1000);
                var btn2FASubmit = this.findElementBy(driver, By.CssSelector("button[data-nemo=\"twofactor_setup_submit\"]"), 1, "Btn 2FA Submit");
                btn2FASubmit.Click();
                Thread.Sleep(3000);
                var SecureCodeNode = this.findElementBy(driver, By.CssSelector("div.qrCode > p"), 5, "Secure Code Node");
                twoFA = SecureCodeNode.Text;
                Thread.Sleep(500);
                var newToken = this.Get2FA(twoFA.Replace(" ", ""));
                var inputCode = this.findElementBy(driver, By.CssSelector("input[name=\"challenge\"]"), 5, "Input Code");
                inputCode.SendKeys(newToken);
                Thread.Sleep(500);
                var btnSubmit = this.findElementBy(driver, By.CssSelector("button[type=\"submit\"]"), 5, "Button 2FA Submit");
                btnSubmit.Click();
                Thread.Sleep(1000);
                var btnDone = this.findElementBy(driver, By.CssSelector("button[data-nemo=\"twofactor_manage_done\"]"), 5, "Button Done");
                btnDone.Click();
                Thread.Sleep(5000);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi set 2FA: " + ex.Message);
            }

            return new ResultCreateSecureAnd2FA { securityQuestion = securityQuestion, twoFA = twoFA };

        }
        public bool upgradteBusiness(IWebDriver driver, string businessName)
        {
            if (driver.Url.Contains("dashboard"))
            {
                return true;
            }
            driver.Url = "https://www.paypal.com/myaccount/settings/";
            Thread.Sleep(1000);
            var nameAccount = this.findElementByNotAwaitClickAble(driver, By.CssSelector("div.row.profileDetail-container > p.vx_h3"), 10, "Name Account").Text;
            driver.Url = "https://www.paypal.com/VN/merchantsignup/router";
            Thread.Sleep(1000);
            //id="businessType-menu"
            //SelectElement selectElement = new SelectElement(this.findElementBy(driver, By.CssSelector("select[name=\"businessType\"]"), 10, "Select Type"));
            //selectElement.SelectByIndex(1);

            this.findElementBy(driver, By.CssSelector("div[id=\"businessType\"]"), 10, "UL Select").Click();
            Thread.Sleep(1000);
            this.findElementBy(driver, By.CssSelector("li[id=\"business-type-i-n-d-i-v-i-d-u-a-l-1\"]"), 10, "Li Select").Click();
            Thread.Sleep(1000);
            this.findElementBy(driver, By.CssSelector("button[id=\"businessTypeContinueButton\"]"), 10, "Button Submit").Click();
            Thread.Sleep(1000);
            this.findElementBy(driver, By.CssSelector("#createOrUpgrade > div:nth-child(3)"), 10, "Upgrade your account").Click();
            Thread.Sleep(1000);
            this.findElementBy(driver, By.CssSelector("button[id=\"upgradeDecisionContinue\"]"), 10, "Button Submit").Click();
            Thread.Sleep(1000);
            this.findElementByNotAwaitClickAble(driver, By.CssSelector("input[id=\"businessLegalName\"]"), 10, "Input name").SendKeys(businessName);
            Thread.Sleep(1000);

            this.findElementByNotAwaitClickAble(driver, By.CssSelector("input[id=\"agreementAccepted\"]"), 10, "Checkbox Agree");
            Thread.Sleep(1000);
            ((IJavaScriptExecutor)driver).ExecuteScript("document.querySelector(\"#agreementAccepted\").click();");

            Thread.Sleep(1000);
            this.findElementBy(driver, By.CssSelector("button[id=\"upgradeContinueButton\"]"), 10, "Button Submit").Click();

            Thread.Sleep(1000);
            this.findElementBy(driver, By.CssSelector("input[id=\"merchantCategoryCode\"]"), 10, "input type").Click();
            Thread.Sleep(1000);
            this.findElementByNotAwaitClickAble(driver, By.CssSelector("input[id=\"merchantCategoryCode\"]"), 10, "input type").SendKeys("a");
            Thread.Sleep(1000);
            this.findElementBy(driver, By.XPath("//li[contains(@id,'merchantCategoryCode-menu-item-" + new Random().Next(0, 100) + "')]"), 10, "input type").Click();
            Thread.Sleep(1000);
            this.findElementBy(driver, By.CssSelector("button[id=\"continueButton\"]"), 10, "Button Submit").Click();
            Thread.Sleep(1000);
            this.findElementByNotAwaitClickAble(driver, By.CssSelector("input[id=\"homeAddressSameAsBusinessAddressCheckbox\"]"), 10, "Checkbox Address");
            Thread.Sleep(1000);
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            string value = (string)js.ExecuteScript("return document.querySelector(\"#homeAddressSameAsBusinessAddressCheckbox\").value");
            if (value != "true")
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("document.querySelector(\"#homeAddressSameAsBusinessAddressCheckbox\").click();");
            }
            //((IJavaScriptExecutor)driver).ExecuteScript("document.querySelector(\"#homeAddressSameAsBusinessAddressCheckbox\").click();");
            //Thread.Sleep(1000);
            this.findElementBy(driver, By.CssSelector("button[id=\"personalInfoSubmitButton\"]"), 10, "Button Submit").Click();
            Thread.Sleep(10 * 1000);

            return true;
        }
        public bool changeLanguage(IWebDriver driver, string language)
        {
            driver.Url = "https://www.paypal.com/myaccount/settings/";
            Thread.Sleep(2000);
            var cbbLanguage = this.findElementByNotAwaitClickAble(driver, By.Id("language"), 5);
            SelectElement selectElement = new SelectElement(cbbLanguage);
            if (language == "vi")
            {
                try
                {
                    selectElement.SelectByValue("vi_US");
                }
                catch (Exception)
                {
                    selectElement.SelectByValue("vi_VN");
                }
            }
            else
            {
                try
                {
                    selectElement.SelectByValue("en_US");
                }
                catch (Exception)
                {
                    selectElement.SelectByValue("en_VN");
                }
            }
            Thread.Sleep(1000);
            return true;
        }
        public string changePassword(IWebDriver driver, string oldPassword)
        {
            string newPassword = this.RandomPassword(15);
            try
            {
                driver.Url = "https://www.paypal.com/myaccount/security/";
                Thread.Sleep(2000);
                this.findElementBy(driver, By.CssSelector("#main > div > div:nth-child(1) > button"), 10, "Button Update Password").Click();
                Thread.Sleep(2000);
                this.findElementBy(driver, By.CssSelector("input[id=\"password_change_current\"]"), 10, "Input Old Password").SendKeys(oldPassword);
                Thread.Sleep(500);
                this.findElementBy(driver, By.CssSelector("input[id=\"password_change_new\"]"), 10, "Input New Password").SendKeys(newPassword);
                Thread.Sleep(500);
                this.findElementBy(driver, By.CssSelector("input[id=\"password_change_confirm\"]"), 10, "Input Confirm Password").SendKeys(newPassword);
                Thread.Sleep(1000);
                this.findElementBy(driver, By.CssSelector("button[data-nemo=\"change_password_submit\"]"), 10, "Button Submit").Click();
                Thread.Sleep(1000);

                int timout = 30;
                while (timout > 0)
                {
                    if (driver.Url.Contains("signin"))
                    {
                        return newPassword;
                    }
                    timout--;
                    Thread.Sleep(1000);
                }
                return newPassword;
            }
            catch (Exception)
            {
                throw new Exception("Lỗi đổi password: " + newPassword);
            }
        }
        public bool createRequest(IWebDriver driver)
        {
            driver.Url = "https://www.paypal.com/myaccount/transfer/homepage/request";
            Thread.Sleep(2000);

            var inputEmail = this.findElementBy(driver, By.CssSelector("input[id=\"fn-requestRecipient\"]"), 10, "Input Email");
            Thread.Sleep(1000);

            for (int i = 0; i < this.dataRequest.email.Length; i++)
            {
                inputEmail.SendKeys(this.dataRequest.email[i].ToString());
                Thread.Sleep(100);
            }
            Thread.Sleep(500);
            this.findElementBy(driver, By.CssSelector("input[id=\"fn-requestRecipient\"]"), 10, "Input Email").SendKeys(OpenQA.Selenium.Keys.Enter);

            Thread.Sleep(1000);

            this.findElementBy(driver, By.CssSelector("button[data-nemo=\"multi-request-button\"]"), 10, "Button Next").Click();
            Thread.Sleep(1000);

            this.findElementBy(driver, By.CssSelector("input[id=\"fn-amount\"]"), 10, "Input Amount").SendKeys(this.dataRequest.amount.ToString() + "00");
            Thread.Sleep(1000);

            this.findElementBy(driver, By.CssSelector("textarea[id=\"noteField\"]"), 10, "Input Note").SendKeys(this.dataRequest.note);
            Thread.Sleep(1000);

            this.findElementBy(driver, By.CssSelector("button[type=\"submit\"]"), 10, "Button Finist").Click();
            Thread.Sleep(1000);

            this.findElementBy(driver, By.CssSelector("div.successBox.successBox_request > h2.successHeader"), 20, "Button Finist");

            return true;
        }
        public bool cancelRequest(IWebDriver driver)
        {

            DateTime now = DateTime.Now;
            string firstTime = now.Year + "-01-01";
            string lastTime = now.Year + "-" + now.Month + "-" + now.Day;
            string urlActivity = "https://www.paypal.com/myaccount/transactions/?free_text_search=&filter_id=&currency=ALL&issuance_product_name=&asset_names=&asset_symbols=&type=&status=&start_date=" + firstTime + "&end_date=" + lastTime;


            driver.Url = urlActivity;


            Thread.Sleep(2000);

            var listCancelButton = this.findElementsBy(driver, By.CssSelector("a[name=\"cancel\"]"), 10);
            foreach (var btnCancel in listCancelButton)
            {
                try
                {
                    btnCancel.Click();
                    break;
                }
                catch (Exception)
                {
                }
            }

            Thread.Sleep(3000);
            this.findElementBy(driver, By.CssSelector("button[name=\"rmcancelbutton\"]"), 10, "Cancel Node").Click();
            return true;

        }
        public bool loginMail(Account account, int port)
        {
            IWebDriver driver = null;
            Gologin gologin = null;
            try
            {

                DataStartDriver dataStartDriver = openChromeDriver(account, true, port);
                driver = dataStartDriver.driver;
                gologin = dataStartDriver.gologin;


                Thread.Sleep(1000);

                if (account.email.Contains("yahoo"))
                {
                    driver.Url = "https://login.yahoo.com/";
                    Thread.Sleep(1000);
                    this.findElementBy(driver, By.CssSelector("input[name=\"username\"]"), 30, "Input Email").SendKeys(account.email);
                    Thread.Sleep(1000);
                    ((IJavaScriptExecutor)driver).ExecuteScript("document.querySelector('input[type=\"submit\"').click();");

                    Thread.Sleep(3000);
                    this.findElementBy(driver, By.CssSelector("input[name=\"password\"]"), 30, "Input Password").SendKeys(account.passwordEmail);
                    Thread.Sleep(1000);
                    ((IJavaScriptExecutor)driver).ExecuteScript("document.querySelector('input[type=\"submit\"').click();");
                    Thread.Sleep(3000);

                    Console.WriteLine("");
                    var checkCaptcha = this.findElementByWithoutError(driver, By.CssSelector("iframe[id=\"recaptcha-iframe\"]"), 2, "Check Captcha");
                    while (checkCaptcha != null)
                    {
                        var siteKey = checkCaptcha.GetAttribute("src");
                        siteKey = siteKey.Replace("siteKey=", "|").Split('|')[1].Split('&')[0];
                        this.resovleCaptchaAsync(driver, siteKey);
                        Thread.Sleep(5000);
                        checkCaptcha = this.findElementByWithoutError(driver, By.CssSelector("div[id=\"g-recaptcha\"]"), 1, "Check Captcha");
                    }

                }
                else if (account.email.Contains("hotmail"))
                {
                    driver.Url = "https://login.live.com/login.srf?wa=wsignin1.0&rpsnv=13&ct=1632470048&rver=7.0.6737.0&wp=MBI_SSL&wreply=https%3a%2f%2foutlook.live.com%2fowa%2f0%2f%3fstate%3d1%26redirectTo%3daHR0cHM6Ly9vdXRsb29rLmxpdmUuY29tL21haWwvMC9pbmJveC8%26nlp%3d1%26RpsCsrfState%3d9b69167d-22d8-9e7a-1fc8-e1d0fc046408&id=292841&aadredir=1&CBCXT=out&lw=1&fl=dob%2cflname%2cwld&cobrandid=90015";
                    Thread.Sleep(1000);

                    this.findElementBy(driver, By.CssSelector("input[name=\"loginfmt\"]"), 30, "Input Email").SendKeys(account.email);
                    Thread.Sleep(1000);
                    ((IJavaScriptExecutor)driver).ExecuteScript("document.querySelector('input[type=\"submit\"').click();");
                    Thread.Sleep(2000);
                    this.findElementBy(driver, By.CssSelector("input[name=\"passwd\"]"), 30, "Input Email").SendKeys(account.passwordEmail);
                    Thread.Sleep(1000);
                    ((IJavaScriptExecutor)driver).ExecuteScript("document.querySelector('input[type=\"submit\"').click();");

                    for (int i = 0; i < 3; i++)
                    {
                        var skip = this.findElementByWithoutError(driver, By.CssSelector("a[id=\"iShowSkip\"]"), 1);
                        if (skip != null)
                        {
                            skip.Click();
                        }

                        var keepLogin = this.findElementByWithoutError(driver, By.CssSelector("input[name=\"DontShowAgain\"]"), 1);
                        if (keepLogin != null)
                        {
                            keepLogin.Click();
                            Thread.Sleep(500);
                            ((IJavaScriptExecutor)driver).ExecuteScript("document.querySelector('input[type=\"submit\"').click();");

                        }

                        var cancel = this.findElementByWithoutError(driver, By.CssSelector("a[id=\"iCancel\"]"), 1);
                        if (cancel != null)
                        {
                            cancel.Click();
                        }
                    }



                }
                else if (account.email.Contains("hungctn.com"))
                {

                    driver.Url = "https://passport.yandex.ru/auth";
                    Thread.Sleep(1000);

                    this.findElementBy(driver, By.CssSelector("input[name=\"login\"]"), 10, "Input Email").SendKeys(account.email);
                    Thread.Sleep(1000);
                    this.findElementBy(driver, By.CssSelector("button[id=\"passp:sign-in\"]"), 10, "Button Login").Click();
                    Thread.Sleep(2000);

                    this.findElementBy(driver, By.CssSelector("input[id=\"passp-field-passwd\"]"), 10, "Input Email").SendKeys(account.passwordEmail);
                    Thread.Sleep(1000);
                    this.findElementBy(driver, By.CssSelector("button[id=\"passp:sign-in\"]"), 10, "Button Login").Click();
                    Thread.Sleep(1000);
                    this.findElementBy(driver, By.CssSelector("button[type=\"submit\"]"), 5, "Button Login").Click();

                }

            }
            catch (Exception e)
            {

            }
            return true;
        }
        public bool remove2FA(IWebDriver driver)
        {
            bool check = false;
            driver.Navigate().GoToUrl("https://www.paypal.com/myaccount/security/");
            try
            {
                // ===== 2FA =====
                var TwoFANode = this.findElementBy(driver, By.CssSelector("#main > div > div:nth-child(2)"), 10, "Two FA Node");
                TwoFANode.Click();
                Thread.Sleep(2000);

                var btnOff2FA = this.findElementBy(driver, By.CssSelector("button[data-nemo=\"twofactor_manage_turnOff\"]"), 5, "Btn Off 2FA Node");
                btnOff2FA.Click();

                Thread.Sleep(1000);
                var btn2FASubmit = this.findElementBy(driver, By.CssSelector("button[data-nemo=\"twofactor_turnOff_confirm\"]"), 5, "Btn OFF 2FA Submit");
                btn2FASubmit.Click();
                int timeout = 30;
                while (timeout > 0 && driver.Url == "https://www.paypal.com/myaccount/security/twofactor/authentication")
                {
                    timeout--;
                    Thread.Sleep(1000);
                }
                //Check xoa xong

                check = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi set 2FA: " + ex.Message);
            }
            return check;
        }
        public string RandomString(int length)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public string RandomPassword(int length)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public void changePhone(IWebDriver driver, string newPhone)
        {
            if (driver.Url.Contains("dashboard"))
            {
                driver.Url = "https://www.paypal.com/businessprofile/settings/phone";
                Thread.Sleep(2000);
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

                var editElement = this.findElementByWithoutError(driver, By.CssSelector("a[data-pagename=\"main:businessweb:profile:settings:editphone\"]"));
                if (editElement != null)
                {
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("a[data-pagename=\"main:businessweb:profile:settings:editphone\"]")));
                    editElement.Click();


                }
                else
                {
                    var btnAdd = this.findElementByWithoutError(driver, By.CssSelector("a[name=\"addPhone\"]"));
                    btnAdd.Click();
                }
                Thread.Sleep(3000);
                var inputPhone = this.findElementBy(driver, By.CssSelector("input[name=\"phoneNumber\"]"), 10, "Input Phone");
                inputPhone.Clear();
                Thread.Sleep(500);
                inputPhone.SendKeys(newPhone);
                Thread.Sleep(1000);
                this.findElementBy(driver, By.CssSelector("input[class=\"btn button_radius changePhone validateBeforeSubmit\"]"), 10, "Button Submit").Click();
                Thread.Sleep(1000);


            }
            else
            {

                driver.Url = "https://www.paypal.com/myaccount/settings/";
                Thread.Sleep(1000);
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("a[name=\"addPhone\"]"))).Click();

                //this.findElementBy(driver, By.CssSelector("a[name=\"addPhone\"]"), 10, "Button Add New").Click();
                Thread.Sleep(3000);
                this.findElementBy(driver, By.CssSelector("input[name=\"phoneNumber\"]"), 10, "Input Phone").SendKeys(newPhone);
                Thread.Sleep(1000);
                this.findElementBy(driver, By.CssSelector("button[id=\"test_addUpdatePhoneButton\"]"), 10, "Button Add").Click();

                Console.WriteLine("");


                try
                {
                    wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[name=\"makePrimary\"]"))).Click();
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[id=\"test_makePrimaryButton\"]"))).Click();
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[name=\"remove\"]"))).Click();
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[data-name=\"submitRemove\"]"))).Click();
                }
                catch (Exception)
                {
                }

            }
            Thread.Sleep(5000);

        }
        public void changeMail(IWebDriver driver, Account account)
        {

            driver.Url = "https://www.paypal.com/myaccount/settings/";
            Thread.Sleep(3000);


            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            var listMail = this.findElementsBy(driver, By.CssSelector("ul.emails.unstyled.lined > li"), 30);
            bool isExist = false;
            foreach (var item in listMail)
            {
                if (item.Text.ToLower().Contains(account.subEmail.ToLower()))
                {
                    try
                    {
                        isExist = true;
                        var btnEdit = item.FindElement(By.CssSelector("a[aria-describedby=\"edit_email_id\"]"));
                        if (btnEdit != null)
                        {
                            btnEdit.Click();
                            Thread.Sleep(2000);
                            var btnConfirmAgain = this.findElementBy(driver, By.CssSelector("a[data-pagename=\"main:walletweb:settings:email:confirm\"]"), 10, "Button Confirm Again");
                            btnConfirmAgain.Click();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Lỗi Confirm lại: " + ex.Message);
                    }
                    break;
                }
            }
            DateTime timemail = DateTime.Now;
            if (!isExist)
            {
                timemail = DateTime.Now.AddHours(-1);
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("a[name=\"addEmail\"]"))).Click();
                Thread.Sleep(1000);
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input[name=\"email\"]"))).SendKeys(account.subEmail);
                Thread.Sleep(1000);
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input[data-track=\"AddEmailAddress\"]"))).Click();
            }
            else
            {
                timemail = DateTime.Now.AddDays(-3);
            }

            var url = Mail.GetUrlAuth(account.subEmail, timemail);
            if (url == "" || url == null)
            {
                throw new Exception("Không tìm thấy mail");
            }
            driver.Url = url;

            var inputPassword = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input[name=\"login_password\"]")));
            for (int i = 0; i < account.password.Length; i++)
            {
                inputPassword.SendKeys(account.password[i].ToString());
            }
            Thread.Sleep(1000);
            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[name=\"btnLogin\"]"))).Click();
            Thread.Sleep(1000);

            //try
            //{
            //    wait = new WebDriverWait(driver, TimeSpan.FromSeconds(1));
            //    inputPassword = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input[name=\"login_password\"]")));
            //    for (int i = 0; i < account.password.Length; i++)
            //    {
            //        inputPassword.SendKeys(account.password[i].ToString());
            //    }
            //    Thread.Sleep(1000);
            //    wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[name=\"btnLogin\"]"))).Click();
            //}
            //catch (Exception)
            //{
            //}



            var timeout = 30;
            bool isLogin = false;
            while (!isLogin && timeout > 0)
            {

                //========= Check Success==========
                if (driver.Url.Contains("myaccount/summary") || driver.Url.Contains("dashboard"))
                {
                    isLogin = true;

                    break;
                }
                timeout--;
                Thread.Sleep(1000);

                var error1 = this.findElementByWithoutError(driver, By.CssSelector("#content > div.notifications > p"), 1);
                if (error1 != null && error1.Text != "")
                {
                    throw new Exception(error1.Text);
                }


                var skip1 = this.findElementByWithoutError(driver, By.CssSelector("div[data-testid=\"skipLink-component\"]"), 1);
                if (skip1 != null)
                {
                    skip1.Click();
                }
                // ====== Check 5$ ============
                try
                {
                    var imageClaim5Dola = this.findElementByWithoutError(driver, By.CssSelector("#main > section > div.containerCentered > div > a > img"), 1);
                    if (imageClaim5Dola != null)
                    {
                        imageClaim5Dola.Click();
                        Thread.Sleep(3000);
                        driver.Url = "https://www.paypal.com/signin";
                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("5$: " + ex.Message);
                }
                // ====== Check Captcha =========
                try
                {
                    var keySiteEl = this.findElementByWithoutError(driver, By.CssSelector("input[name=\"_adsRecaptchaSiteKey\"]"), 1);
                    if (keySiteEl != null)
                    {
                        var keySite = keySiteEl.GetAttribute("value");
                        if (keySite != null && keySite != "")
                        {
                            this.resovleCaptchaAsync(driver, keySite, "#content > form > iframe");
                            Thread.Sleep(5000);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Captcha: " + ex.Message);
                }

                // ====== Check Phone =========
                try
                {

                    var confirmPhone = this.findElementByWithoutError(driver, By.CssSelector("#content > form > div.phoneNumbers"), 1);
                    if (confirmPhone != null)
                    {
                        var btnConfirmPhone = this.findElementBy(driver, By.CssSelector("#content > form > p.secondaryLink > a"), 2, "btnConfirmPhone");
                        btnConfirmPhone.Click();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Check Phone: " + ex.Message);
                }

                //====== Check 2FA ========
                try
                {

                    var inputOtp = this.findElementByWithoutError(driver, By.CssSelector("#otpCode"), 1);
                    if (inputOtp != null)
                    {
                        var newToken = this.Get2FA(account.twoFA.Replace(" ", ""));
                        if (newToken == null || newToken == "")
                        {
                            throw new Exception("Lỗi lấy token 2fa");
                        }
                        inputOtp.SendKeys(newToken);

                        Thread.Sleep(1000);
                        try
                        {
                            var skipTwofactorCheckbox = this.findElementByNotAwaitClickAble(driver, By.CssSelector("#skipTwofactorCheckbox"), 1, "skipTwofactorCheckbox");
                            skipTwofactorCheckbox.Click();
                        }
                        catch (Exception)
                        {
                        }
                        var btnSubmit2Fa = this.findElementBy(driver, By.CssSelector("button[type=\"submit\"]"), 3, "btnSubmit2Fa");
                        btnSubmit2Fa.Click();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("2FA: " + ex.Message);
                }

                //========= Check Verify mobile======
                //
                // ====== Check Phone =========
                var verifyPhone = this.findElementByWithoutError(driver, By.CssSelector("#sms-challenge-option"), 1);
                if (verifyPhone != null)
                {
                    throw new Exception("Verify Phone: ");
                }


            }

            if (!isLogin)
            {
                throw new Exception("Timeout check login...!");
            }


        }
        public void verifyAccount(IWebDriver driver)
        {
            var listFile = new DirectoryInfo(Settings.Default.infoVerifyPath).GetFiles();
            var filePath = listFile[new Random().Next(0, listFile.Length)].FullName;
            driver.Url = "https://www.paypal.com/restore/dashboard";
            if (driver.Url.Contains("myaccount/summary"))
            {
                return;
            }
            this.findElementBy(driver, By.Id("appealstep-3"), 30, "Button Restore").Click();
            Thread.Sleep(1000);
            this.findElementBy(driver, By.CssSelector("#uploadOption > option:nth-child(4)"), 30, "Type Info").Click();
            Thread.Sleep(1000);
            this.findElementBy(driver, By.CssSelector("input[name=\"desktopUploadControl\"]"), 30, "Input File").SendKeys(filePath);
            Thread.Sleep(1000);

            var btnSubmit = this.findElementBy(driver, By.Name("uploadSubmit"), 30, "Button Restore");
            try
            {
                var btnCancel = this.findElementBy(driver, By.CssSelector("button.vx_btn.vx_btn-secondary.vx_btn-block.vx-modal-component_btn-secondary.appealstep-close-vx"), 30, "Button Cancel");


                Actions actions = new Actions(driver);
                actions.MoveToElement(btnCancel);
                actions.Perform();
            }
            catch (Exception)
            {
            }
            Thread.Sleep(1000);
            btnSubmit.Click();
            Thread.Sleep(1000);
            this.findElementBy(driver, By.CssSelector("#submissionInfoModal > div > div.vx_modal-content.vx-modal-component-content.submission-info-content > div.done-btn > button"), 30, "Check Verify");

        }


        public void startAction(Account account, ActionRun actionRun, int port)
        {
            ResultLogin resultLogin = null;
            var oldEmail = "";
            try
            {

                if (actionRun == ActionRun.CHECK_CHROME)
                {
                    this.checkChrome(account, port);
                    return;
                }
                if (actionRun == ActionRun.LOGIN_MAIL)
                {
                    this.loginMail(account, port);
                    return;
                }
                if (actionRun == ActionRun.CHECK_HISTORY_NO_PROXY)
                {
                    resultLogin = this.Login(account, false, port);
                    actionRun = ActionRun.CHECK_HISTORY;
                }
                else
                {
                    resultLogin = this.Login(account, true, port);
                }
                if (resultLogin.newPassword != "")
                {
                    account.password = resultLogin.newPassword;
                }

                if (resultLogin.status == "SUCCESS")
                {
                    account.cookie = resultLogin.cookie;
                    account.log = "";

                    switch (actionRun)
                    {
                        case ActionRun.VERIFY_HOTMAIL:
                            this.verifyMailPaypal(resultLogin.driver, account);
                            account.resultKTGD = "Verify hotmail " + DateTime.Now.ToString("dd/MM/yyyy");
                            break;
                        case ActionRun.CREATE_HOSTMAIL:
                            this.regHostMail(resultLogin.driver, account, account.email, account.password, account.name.Split(' ')[0], account.name.Split(' ')[1], account.birthday);
                            account.resultKTGD = "Tạo hotmail + Verify" + DateTime.Now.ToString("dd/MM/yyyy");
                            break;
                        case ActionRun.CREATE_HOSTMAIL_2:
                            oldEmail = account.email;
                            string newEmail = this.regHostMail2(resultLogin.driver, account, account.email, account.password, account.name.Split(' ')[0], account.name.Split(' ')[1], account.birthday);
                            if (newEmail != null)
                            {
                                account.email = newEmail;
                            }
                            else
                            {
                                account.email = oldEmail;
                            }
                            account.resultKTGD = "Tạo hotmail + Verify" + DateTime.Now.ToString("dd/MM/yyyy");
                            break;
                        case ActionRun.CREATE_HOSTMAIL_3:
                            oldEmail = account.email;
                            string newEmail2 = this.regHostMail3(resultLogin.driver, account, account.email, account.password, account.name.Split(' ')[0], account.name.Split(' ')[1], account.birthday);
                            if (newEmail2 != null)
                            {
                                account.email = newEmail2;
                            }
                            else
                            {
                                account.email = oldEmail;
                            }
                            account.resultKTGD = "Tạo hotmail + Verify" + DateTime.Now.ToString("dd/MM/yyyy");
                            break;
                        case ActionRun.VERIFY_ACCOUNT:
                            this.verifyAccount(resultLogin.driver);
                            account.resultKTGD = "Verify account " + DateTime.Now.ToString("dd/MM/yyyy");
                            break;

                        case ActionRun.CHECK_HISTORY:
                            ResultGetHistory resultGetHistory = resultLogin.driver.Url.Contains("dashboard") ? this.CheckHistoryBusiness(resultLogin.driver) : this.CheckHistory(resultLogin.driver);
                            account.resultLimit = resultGetHistory.resultLimit;
                            account.history = resultGetHistory.history;
                            account.amount = resultGetHistory.amount;
                            account.countTransaction = resultGetHistory.countTransaction;
                            account.idTransaction = resultGetHistory.idTransaction;
                            account.mailReceive = resultGetHistory.mailReceive;
                            account.resultKTGD = "Lấy lịch sử " + DateTime.Now.ToString("dd/MM/yyyy");
                            break;
                        case ActionRun.GET_INFOMATION:
                            ResultGetInfoAccout resultGetInfoAccout = this.GetInfoAccount(resultLogin.driver);
                            account.address = resultGetInfoAccout.address;
                            account.birthday = resultGetInfoAccout.birthday;
                            account.dateCreate = resultGetInfoAccout.dateCreate;
                            account.name = resultGetInfoAccout.name;
                            account.ttVeryMail = resultGetInfoAccout.ttVeryMail;
                            account.subEmail = resultGetInfoAccout.emailPhu + " " + resultGetInfoAccout.KQKTDG;
                            account.resultKTGD = "Lấy thông tin account: " + DateTime.Now.ToString("dd/MM/yyyy") + ": " + resultGetInfoAccout.KQKTDG;
                            account.ttVeryMail = resultGetInfoAccout.KQKTDG;

                            break;
                        case ActionRun.START_CHROME:
                            break;
                        case ActionRun.CREATE_SECURE_QUESTION:
                            ResultCreateSecureAnd2FA resultCreateSecureAnd2FA = this.createSecureQuestion(resultLogin.driver);
                            account.securityQuestion = resultCreateSecureAnd2FA.securityQuestion;
                            account.resultKTGD = "Tạo câu hỏi BM - " + DateTime.Now.ToString("dd/MM/yyyy");
                            break;
                        case ActionRun.CREATE_SECURE_2FA:
                            ResultCreateSecureAnd2FA resultCreateSecureAnd2FA1 = this.createSecureAnd2FA(resultLogin.driver);
                            account.twoFA = resultCreateSecureAnd2FA1.twoFA;
                            account.resultKTGD = "Tạo 2FA - " + DateTime.Now.ToString("dd/MM/yyyy");
                            break;
                        case ActionRun.REMOVE_2FA:
                            bool checkRemove2FA = this.remove2FA(resultLogin.driver);
                            if (checkRemove2FA)
                            {
                                account.twoFA = null;
                            }
                            break;
                        case ActionRun.UPGRADTE_BUSINESS:
                            bool checkBusiness = this.upgradteBusiness(resultLogin.driver, account.businessName);
                            account.resultKTGD = "Nâng cấp Business " + DateTime.Now.ToString("dd/MM/yyyy");
                            break;
                        case ActionRun.CHANGE_LANGUAGE_VI:
                            this.changeLanguage(resultLogin.driver, "vi");
                            account.resultKTGD = "Đổi ngôn ngữ Việt Nam " + DateTime.Now.ToString("dd/MM/yyyy");
                            break;
                        case ActionRun.CHANGE_LANGUAGE_EN:
                            this.changeLanguage(resultLogin.driver, "en");
                            account.resultKTGD = "Đổi ngôn ngữ tiếng Anh " + DateTime.Now.ToString("dd/MM/yyyy");
                            break;
                        case ActionRun.CHANGE_PASSWORD:

                            string newPassword = "";
                            try
                            {
                                newPassword = this.changePassword(resultLogin.driver, account.password);
                            }
                            catch (Exception ex)
                            {
                                newPassword = ex.Message.Replace("Lỗi đổi password: ", "").Trim();
                                account.log = ex.Message;
                            }
                            account.HistoryPassword = account.password;
                            account.password = newPassword;
                            account.resultKTGD = "Đổi mật khẩu " + DateTime.Now.ToString("dd/MM/yyyy");
                            break;
                        case ActionRun.CREATE_REQUEST:
                            this.createRequest(resultLogin.driver);
                            account.resultKTGD = "Tạo request " + DateTime.Now.ToString("dd/MM/yyyy");
                            break;
                        case ActionRun.CANCEL_REQUEST:
                            this.cancelRequest(resultLogin.driver);
                            account.resultKTGD = "Hủy request " + DateTime.Now.ToString("dd/MM/yyyy");
                            break;
                        case ActionRun.CHANGE_PHONE:
                            try
                            {
                                this.changePhone(resultLogin.driver, account.phone);
                            }
                            catch (Exception ex)
                            {
                                account.phone = this.myDatabase.findAccountByEmail(account.email).phone;
                                throw new Exception("Lỗi đổi sđt: " + ex.Message);
                            }
                            account.resultKTGD = "Thay đổi SĐT " + DateTime.Now.ToString("dd/MM/yyyy");
                            break;
                        case ActionRun.CHANGE_MAIL:
                            account.subEmail = account.email.Split('@')[0] + "@" + Settings.Default.mailUsername.Split('@')[1];
                            this.changeMail(resultLogin.driver, account);
                            account.resultKTGD = "Thêm email " + DateTime.Now.ToString("dd/MM/yyyy");
                            break;
                    }
                }
                else
                {
                    account.log = resultLogin.message;
                }



            }
            catch (Exception ex)
            {
                if (actionRun == ActionRun.CREATE_HOSTMAIL_2)
                {
                    account.email = oldEmail;
                }
                account.log = ex.Message + "- " + ex.StackTrace;
            }

            try
            {
                if (actionRun != ActionRun.START_CHROME)
                {
                    if (Settings.Default.autoCloseChrome)
                    {
                        if (resultLogin.driver != null)
                        {
                            try
                            {
                                resultLogin.driver.Close();
                            }
                            catch (Exception)
                            {
                            }
                            try
                            {
                                resultLogin.driver.Quit();
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }

                this.myDatabase.updateAccount(account);
                //this.form1.getAccount();
                this.form1.updateTableAccount(account);
            }
            catch (Exception)
            {
            }

        }

        public ProxyObject getNewProxyObject(List<ProxyObject> proxyObjects, Account account = null)
        {
            foreach (var item in proxyObjects)
            {
                if (!item.Running)
                {
                    if (item.Time == null)
                    {
                        try
                        {
                            if (account != null)
                            {
                                account.log = "Reset proxy: " + item.ProxyString;
                                this.form1.updateTableAccount(account);
                                account.proxy = item.ProxyString;
                            }
                            item.resetProxy(this.publicIP);
                            Thread.Sleep(1000);
                            if (account != null)
                            {
                                account.log = "Check Proxy: " + account.proxy;
                                this.form1.updateTableAccount(account);
                            }
                            item.checkProxy();
                            if (account != null)
                            {
                                account.log = "Proxy Live";
                                this.form1.updateTableAccount(account);
                            }
                            return item;
                        }
                        catch (Exception)
                        {
                        }
                    }
                    TimeSpan time = DateTime.Now.Subtract(item.Time);
                    if (time.TotalSeconds >= 120)
                    {
                        try
                        {
                            if (account != null)
                            {
                                account.log = "Reset proxy: " + item.ProxyString;
                                this.form1.updateTableAccount(account);
                                account.proxy = item.ProxyString;
                            }
                            item.resetProxy(this.publicIP);
                            Thread.Sleep(1000);
                            if (account != null)
                            {
                                account.log = "Check Proxy: " + account.proxy;
                                this.form1.updateTableAccount(account);
                            }
                            item.checkProxy();
                            if (account != null)
                            {
                                account.log = "Proxy Live";
                                this.form1.updateTableAccount(account);
                            }
                            return item;
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            return null;
        }
        public void StartMain(List<string> listEmailAccount, ActionRun actionRun, List<string> listPhone)
        {
            this.StopAll();
            listProfileIDRunning = new List<string>();

            List<ProxyObject> proxyObjects = new List<ProxyObject>();
            var listProxyString = Settings.Default.listProxy.Split('\n');
            if (Settings.Default.proxyAutoReset)
            {
                foreach (var proxyString in listProxyString)
                {
                    if (proxyString != null && proxyString != "")
                    {
                        try
                        {
                            ProxyObject proxyObject = new ProxyObject(proxyString);
                            proxyObjects.Add(proxyObject);
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Proxy sai định dạng.!");
                        }
                    }
                }
            }


            threadMain = new Thread(() =>
            {

                int threadCount = Settings.Default.threadCount;
                int totalAccount = listEmailAccount.Count;
                totalRunning = 0;
                int currentIndex = 0;
                bool isRunning = false;
                int indexProxy = 0;

                while (currentIndex < totalAccount)
                {
                    if (totalRunning == 0)
                    {
                        indexProxy = 0;
                        isRunning = false;
                        this.StopCurrent();
                    }
                    if (!isRunning && totalRunning < threadCount)
                    {
                        var account = this.myDatabase.findAccountByEmail(listEmailAccount[currentIndex]);

                        if (actionRun == ActionRun.CHANGE_EMAIL_CHINH_PHU)
                        {
                            var email = account.email;
                            account.email = account.subEmail;
                            account.subEmail = email;
                            this.myDatabase.updateAccount(account);
                            this.form1.updateTableAccount(account);
                        }
                        else
                        {
                            if (actionRun == ActionRun.CHANGE_PHONE)
                            {
                                account.phone = listPhone[currentIndex];
                            }

                            int port = 3500 + currentIndex;
                            if (Settings.Default.proxyAutoReset)
                            {
                                string oldProxy = account.proxy;
                                ProxyObject proxyObject = getNewProxyObject(proxyObjects, account);
                                if (proxyObject != null)
                                {
                                    proxyObject.Running = true;
                                    proxyObject.Time = DateTime.Now;

                                    if (proxyObject != null)
                                    {
                                        proxyObject.Time = DateTime.Now;
                                        Thread childThread = new Thread(() =>
                                        {
                                            account.proxy = proxyObject.ProxyString;
                                            startAction(account, actionRun, port);
                                            totalRunning--;
                                            proxyObject.Running = false;
                                            account.proxy = oldProxy;
                                            this.myDatabase.updateAccount(account);
                                            this.form1.updateTableAccount(account);
                                        });
                                        childThread.Start();
                                        childThread.IsBackground = true;
                                        listChildThread.Add(childThread);

                                    }
                                }
                                else
                                {
                                    account.log = "Thiếu Proxy";
                                    account.proxy = oldProxy;
                                    this.myDatabase.updateAccount(account);
                                    this.form1.updateTableAccount(account);
                                }

                            }
                            else
                            {
                                Thread childThread = new Thread(() =>
                                {
                                    startAction(account, actionRun, port);
                                    totalRunning--;
                                });
                                childThread.Start();
                                childThread.IsBackground = true;
                                listChildThread.Add(childThread);
                            }

                            Thread.Sleep(1000);
                            totalRunning++;
                            indexProxy++;

                            if (totalRunning == threadCount)
                            {
                                isRunning = true;
                            }

                        }
                        currentIndex++;

                    }
                    if (actionRun != ActionRun.CHANGE_EMAIL_CHINH_PHU)
                    {
                        Thread.Sleep(3000);
                    }
                }
            });
            threadMain.IsBackground = true;
            threadMain.Start();
        }
        public void StopCurrent()
        {
            foreach (var item in listChildThread)
            {
                try
                {
                    if (item != null)
                    {
                        item.Abort();
                    }
                }
                catch (Exception)
                {
                }
            }
            try
            {
                foreach (var item in Process.GetProcessesByName("chromedriver"))
                {
                    item.Kill();
                }
            }
            catch (Exception)
            {
            }
            try
            {
                foreach (var item in Process.GetProcessesByName("chrome"))
                {
                    try
                    {
                        if (item.MainModule.FileName.Contains(@"\chrome\chrome.exe"))
                        {
                            item.Kill();
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
            try
            {
                foreach (var item in Process.GetProcessesByName("chrome"))
                {
                    try
                    {
                        if (item.MainModule.FileName.Contains(@"\orbita-browser\chrome.exe"))
                        {
                            item.Kill();
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
            listChildThread = new List<Thread>();
        }

        public void KillChromeByProfileName()
        {
            try
            {
                foreach (var item in Process.GetProcessesByName("chrome"))
                {
                    try
                    {
                        var info = item.StartInfo.Arguments;
                        Debug.WriteLine("#### info: " + info);
                        if (item.MainModule.FileName.Contains(@"\orbita-browser\chrome.exe"))
                        {
                            item.Kill();
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
        }
        public void StopChromeAndChromeDriver()
        {
            try
            {
                foreach (var item in Process.GetProcessesByName("chromedriver"))
                {
                    item.Kill();
                }
            }
            catch (Exception)
            {
            }
            try
            {
                foreach (var item in Process.GetProcessesByName("chrome"))
                {
                    try
                    {
                        if (item != null && ((item.MainModule != null && (item.MainModule.FileName.Contains(@"\chrome\chrome.exe") || item.MainModule.FileName.Contains("orbita-browser"))) || item.MainWindowTitle.ToString().Contains("PayPal")))
                        {
                            item.Kill();
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            catch (Exception)
            {
            }

        }
        public void StopAll()
        {
            if (threadMain != null)
            {
                try
                {
                    threadMain.Abort();
                }
                catch (Exception)
                {
                }
            }
            foreach (var item in this.listDriver)
            {
                try
                {
                    item.Close();
                    item.Quit();
                }
                catch (Exception)
                {
                }
            }
            if (threadRegAccount != null)
            {
                try
                {
                    threadRegAccount.Abort();
                }
                catch (Exception)
                {
                }
            }

            foreach (var item in listChildThreadRegAccount)
            {
                try
                {
                    if (item != null)
                    {
                        item.Abort();
                    }
                }
                catch (Exception)
                {
                }
            }

            foreach (var item in listChildThread)
            {
                try
                {
                    if (item != null)
                    {
                        item.Abort();
                    }
                }
                catch (Exception)
                {
                }
            }
            try
            {
                foreach (var item in Process.GetProcessesByName("chromedriver"))
                {
                    item.Kill();
                }
            }
            catch (Exception)
            {
            }
            try
            {
                foreach (var item in Process.GetProcessesByName("chrome"))
                {
                    try
                    {
                        if (item != null && ((item.MainModule != null && (item.MainModule.FileName.Contains(@"\chrome\chrome.exe") || item.MainModule.FileName.Contains("orbita-browser"))) || item.MainWindowTitle.ToString().Contains("PayPal")))
                        {
                            item.Kill();
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            catch (Exception)
            {
            }

            listChildThread = new List<Thread>();
            listChildThreadRegAccount = new List<Thread>();

        }
        private IWebElement findElementBy(IWebDriver driver, By by, int timeout = 30, string title = "", int timout2 = 5)
        {
            IWebElement element = null;
            while (true)
            {
                try
                {
                    element = driver.FindElement(by);
                    if (element != null)
                    {
                        new WebDriverWait(driver, TimeSpan.FromSeconds(timout2)).Until(ExpectedConditions.ElementToBeClickable(by));
                        return element;
                    }
                    Thread.Sleep(1000);
                    timeout--;
                    if (timeout == 0)
                    {
                        throw new Exception(title);
                    }
                }
                catch (Exception)
                {
                    Thread.Sleep(1000);
                    timeout--;
                    if (timeout == 0)
                    {
                        throw new Exception(title + " not found!");
                    }
                }

            }
        }
        private IWebElement findElementByNotAwaitClickAble(IWebDriver driver, By by, int timeout = 30, string title = "")
        {
            IWebElement element = null;
            while (true)
            {
                try
                {
                    element = driver.FindElement(by);
                    if (element != null)
                    {
                        return element;
                    }
                    Thread.Sleep(1000);
                    timeout--;
                    if (timeout == 0)
                    {
                        throw new Exception(title);
                    }
                }
                catch (Exception)
                {
                    Thread.Sleep(1000);
                    timeout--;
                    if (timeout == 0)
                    {
                        throw new Exception(title + " not found!");
                    }
                }

            }
        }

        private IWebElement findElementByWithoutError(IWebDriver driver, By by, int timeout = 30, string title = "")
        {
            IWebElement element = null;
            while (true)
            {
                try
                {
                    element = driver.FindElement(by);
                    if (element != null)
                    {
                        new WebDriverWait(driver, TimeSpan.FromSeconds(10)).Until(ExpectedConditions.ElementToBeClickable(by));
                        return element;
                    }
                    timeout--;
                    if (timeout == 0)
                    {
                        return null;
                    }
                    Thread.Sleep(1000);
                }
                catch (Exception)
                {
                    timeout--;
                    if (timeout == 0)
                    {
                        return null;
                    }
                    Thread.Sleep(1000);
                }

            }
        }
        private IWebElement findElementByWithoutErrorNotAwaitCLickAble(IWebDriver driver, By by, int timeout = 30, string title = "")
        {
            IWebElement element = null;
            while (true)
            {
                try
                {
                    element = driver.FindElement(by);
                    if (element != null)
                    {
                        return element;
                    }
                    Thread.Sleep(1000);
                    timeout--;
                    if (timeout == 0)
                    {
                        return null;
                    }
                }
                catch (Exception)
                {
                    Thread.Sleep(1000);
                    timeout--;
                    if (timeout == 0)
                    {
                        return null;
                    }
                }

            }
        }

        private List<IWebElement> findElementsBy(IWebDriver driver, By by, int timeout)
        {
            List<IWebElement> elements = null;
            while (true)
            {
                try
                {
                    elements = driver.FindElements(by).ToList();
                    if (elements != null)
                    {
                        return elements;
                    }
                    Thread.Sleep(1000);
                    timeout--;
                    if (timeout == 0)
                    {
                        return null;
                    }
                }
                catch (Exception)
                {
                    Thread.Sleep(1000);
                    timeout--;
                    if (timeout == 0)
                    {
                        return null;
                    }
                }

            }

        }
        public void stopThreadMain()
        {
            if (threadMain != null)
            {
                try
                {
                    threadMain.Abort();
                }
                catch (Exception)
                {
                }
            }
        }
    }
    public class AllCookie
    {
        public List<CookieP> AllCookies { get; set; }
    }
    public class CookieP
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Domain { get; set; }
        public string Path { get; set; }
        public bool Secure { get; set; }
        public bool IsHttpOnly { get; set; }
        public DateTime? Expiry { get; set; }

    }

    public class ResultGetHistory
    {
        public string resultLimit { get; set; }
        public string history { get; set; }
        public string amount { get; set; }
        public string countTransaction { get; set; }
        public string idTransaction { get; set; }
        public string mailReceive { get; set; }

    }

    public class ResultGetInfoAccout
    {
        public string name { get; set; }
        public string dateCreate { get; set; }
        public string address { get; set; }
        public string ttVeryMail { get; set; }
        public string birthday { get; set; }
        public string emailPhu { get; set; }
        public string KQKTDG { get; set; }
    }

    public class ResultCreateSecureAnd2FA
    {
        public string securityQuestion { get; set; }
        public string twoFA { get; set; }
    }
}
