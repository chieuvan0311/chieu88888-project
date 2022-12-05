using GPMSharedLibrary.V2.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using xNet;

namespace PaypalAccountManager.Entity
{
    public class ObjectResultResetProxy
    {
        public string http_ipv4 { get; set; }
        public string http_ipv6 { get; set; }
        public string socks_ipv4 { get; set; }
        public string http_ipv6_ipv4 { get; set; }
        public string public_ip { get; set; }
        public string public_ip_ipv6 { get; set; }
        public string expired_at { get; set; }
        public string timeout { get; set; }
        public string next_request { get; set; }
        public ObjectAuthentication authentication { get; set; }
        public string ip_allow { get; set; }
        public string your_ip { get; set; }
        public string location { get; set; }

    }
    public class ObjectAuthentication
    {
        public string username { get; set; }
        public string password { get; set; }
    }
    public class ResultResetProxy
    {
        public int code { get; set; }
        public string message { get; set; }
        public ObjectResultResetProxy data { get; set; }

    }
    public class ProxyObject
    {
        public DateTime Time { get; set; }
        public string ProxyString { get; set; }
        public string License { get; set; }
        public string HOST { get; set; }
        public int PORT { get; set; }
        public string USERNAME { get; set; }
        public string PASSWORD { get; set; }
        public bool Running { get; set; }

        public ProxyObject(string proxyString)
        {
            this.License = proxyString;
            this.Running = false;

        }


        public bool resetProxy(string publicIp)
        {
            var urlReset = "https://app.dtproxy.vn/txproxy/v1/api/proxy/get-new-proxy?license=" + this.License + "&authen_ips=" + publicIp;
            HttpRequest httpRequest = new HttpRequest();
            var resultReset = httpRequest.Get(urlReset).ToString();
            if (resultReset.Contains("Success"))
            {
                ResultResetProxy resultResetProxy = JsonConvert.DeserializeObject<ResultResetProxy>(resultReset);
                this.HOST = resultResetProxy.data.public_ip;
                this.PORT = Int32.Parse(resultResetProxy.data.http_ipv4.Split(':')[1]);
                this.USERNAME = resultResetProxy.data.authentication.username;
                this.PASSWORD = resultResetProxy.data.authentication.password;
                if (resultResetProxy.data.authentication.username != null)
                {
                    this.ProxyString = this.USERNAME + ":" + this.PASSWORD + "@" + this.HOST + ":" + this.PORT;
                }else
                {
                    this.ProxyString =  this.HOST + ":" + this.PORT;
                }

                return true;
            }
            else
            {
                throw new Exception("Error Reset proxy: " + resultReset + ": " + this.ProxyString);
            }
        }

        public bool checkProxy()
        {
            try
            {
                IPInfo info = new IPInfo();

                xNet.HttpRequest request = new xNet.HttpRequest();
                if (USERNAME != null)
                {
                    request.Proxy = new xNet.HttpProxyClient(HOST, this.PORT, USERNAME, PASSWORD);

                }
                else
                {
                    request.Proxy = new xNet.HttpProxyClient(HOST, this.PORT);
                }

                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.93 Safari/537.36";
                request.AddHeader(xNet.HttpHeader.Accept, "application/json");

                string html = request.Get("https://time.gologin.com/timezone").ToString(); // https://time.gologin.com/timezone http://ip-api.com/json
                dynamic obj = JsonConvert.DeserializeObject<dynamic>(html);
                info.IP = Convert.ToString(obj.query ?? obj.ip);
                info.Country = Convert.ToString(obj.country);
                info.Region = Convert.ToString(obj.region ?? obj.city);
                info.City = Convert.ToString(obj.city);
                info.Timezone = Convert.ToString(obj.timezone);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Proxy Error: " + ex.Message + ": " + this.ProxyString);
            }

        }

    }
}
