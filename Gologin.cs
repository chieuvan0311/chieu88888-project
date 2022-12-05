using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using xNet;

namespace PaypalAccountManager
{
    public class Gologin
    {
        public string access_token = "";
        public string tmpdir = "";
        public string address = "";
        public List<string> extra_params;
        public int port = 0;
        public bool spawn_browser = true;
        public string userPath = "";
        public string executablePath = "";
        public string profile_id = "";
        public string profile_path = "";
        public string profile_zip_path = "";
        public string profile_zip_path_upload = "";
        public dynamic profile = "";
        public string API_URL = "https://api.gologin.com";
        public dynamic proxy = null;
        public string profile_name = "";
        public dynamic tz = null;
        public bool local = false;

        HttpRequest http = new HttpRequest();

        public Gologin(string token, string profile_id, bool local, ProxyGologin proxy, int port, List<string> extra_params)
        {
            this.http.AddHeader("Authorization", "Bearer " + token);
            this.http.AddHeader("User-Agent", "Selenium-API");

            //=============
            this.local = local;
            this.tmpdir = Path.GetTempPath();
            this.spawn_browser = true;
            this.address = "127.0.0.1";
            this.port = port;
            this.userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            this.executablePath = Path.Combine(this.userPath, ".gologin/browser/orbita-browser/chrome");
            this.extra_params = extra_params;
            this.profile_id = profile_id;
            this.access_token = token;
            this.profile_path = Path.Combine(this.tmpdir, "gologin_" + this.profile_id);
            if (Directory.Exists(this.profile_path))
            {
                Directory.Delete(this.profile_path, true);
            }
            this.profile_zip_path = Path.Combine(this.tmpdir, "gologin_" + this.profile_id + ".zip");
            this.profile_zip_path_upload = Path.Combine(this.tmpdir, "gologin_" + this.profile_id + "_upload.zip");
            this.proxy = proxy;

        }

        public dynamic getProfile()
        {
            string response = this.http.Get(this.API_URL + "/browser/" + this.profile_id).ToString();
            dynamic data = JsonConvert.DeserializeObject<Object>(response);
            return data;
        }
        public void createEmptyProfile()
        {
            Console.WriteLine("createEmptyProfile");
            string empty_profile = "gologin_zeroprofile.zip";
            File.Copy(empty_profile, this.profile_zip_path, true);
        }

        public void extractProfileZip()
        {
            ZipFile.ExtractToDirectory(this.profile_zip_path, this.profile_path);
        }
        public void downloadProfileZip()
        {


            string s3path = this.profile.s3Path;
            byte[] data = null;
            if (s3path == "")
            {
                Console.WriteLine("downloading profile direct");
                data = this.http.Get(API_URL + "/browser/" + this.profile_id).ToBytes();
            }
            else
            {
                WebClient webClient = new WebClient();

                Console.WriteLine("downloading profile s3");
                string s3url = "https://gprofiles.gologin.com/" + s3path.Replace(" ", "+");

                data = webClient.DownloadData(s3url);

                //data = this.http.Get(s3url).ToBytes();
            }

            if (data.Length == 0)
            {
                this.createEmptyProfile();
            }
            else
            {
                File.WriteAllBytes(this.profile_zip_path, data);
            }

            try
            {
                this.extractProfileZip();
            }
            catch (Exception ex)
            {
                this.createEmptyProfile();
                this.extractProfileZip();
            }

            if (!File.Exists(Path.Combine(this.profile_path, "Default/Preferences")))
            {
                this.createEmptyProfile();
                this.extractProfileZip();
            }

        }

        public string formatProxyUrlPassword(dynamic proxy)
        {
            if (proxy.username == "")
            {
                return proxy.mode + "://" + proxy.host + ":" + proxy.port;
            }
            else
            {
                return proxy.mode + "://" + proxy.username + ':' + proxy.password + "@" + proxy.host + ":" + proxy.port;
            }
        }

        public dynamic getTimeZone()
        {
            HttpRequest request = new HttpRequest();

            var proxy = this.proxy;
            string data = null;
            if (proxy != null)
            {

                string h = proxy.host;
                int p = proxy.port;
                string u = proxy.username;
                string up = proxy.password;

                HttpWebRequest requests = (HttpWebRequest)WebRequest.Create("https://time.gologin.com/timezone");
                WebProxy myproxy = new WebProxy(h, p);
                myproxy.BypassProxyOnLocal = false;

                myproxy.Credentials = new NetworkCredential(u, up);
                requests.Proxy = myproxy;
                requests.Method = "GET";

                using (var twitpicResponse = (HttpWebResponse)requests.GetResponse())
                {

                    using (var reader = new StreamReader(twitpicResponse.GetResponseStream()))
                    {
                        var objText = reader.ReadToEnd();
                        return JsonConvert.DeserializeObject(objText);
                    }

                }
            }
            else
            {
                data = request.Get("https://time.gologin.com/timezone").ToString();
            }
            return JsonConvert.DeserializeObject(data);
        }

        public dynamic getGeolocationParams(dynamic profileGeolocationParams, dynamic tzGeolocationParams)
        {
            dynamic data = new ExpandoObject();

            if (profileGeolocationParams.fillBasedOnIp == true)
            {
                data.mode = profileGeolocationParams.mode;
                data.latitude = tzGeolocationParams.latitude;
                data.longitude = tzGeolocationParams.longitude;
                data.accuracy = tzGeolocationParams.accuracy;
            }
            else
            {
                data.mode = profileGeolocationParams.mode;
                data.latitude = profileGeolocationParams.latitude;
                data.longitude = profileGeolocationParams.longitude;
                data.accuracy = profileGeolocationParams.accuracy;
            }
            return data;
        }
        public dynamic convertPreferences(dynamic preferences)
        {
            string resolution = preferences.resolution != null ? preferences.resolution : "1920x1080";
            preferences["screenWidth"] = Int32.Parse(resolution.Split('x')[0]);
            preferences["screenHeight"] = Int32.Parse(resolution.Split('x')[1]);

            this.tz = this.getTimeZone();

            dynamic tzGeoLocation = new ExpandoObject();
            tzGeoLocation.latitude = this.tz.ll[0] != null ? this.tz.ll[0] : 0;
            tzGeoLocation.longitude = this.tz.ll[1] != null ? this.tz.ll[1] : 0;
            tzGeoLocation.accuracy = this.tz.accuracy != null ? this.tz.accuracy : 0;

            dynamic geoLocation = this.getGeolocationParams(preferences.geolocation, tzGeoLocation);
            preferences.geoLocation = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(geoLocation));

            dynamic webRtc = new ExpandoObject();
            webRtc.mode = preferences.webRTC.mode == "alerted" ? "public" : preferences.webRTC.mode;
            webRtc.publicIP = preferences.webRTC.fillBasedOnIp == "True" ? this.tz.ip : preferences.webRTC.publicIp;
            webRtc.localIps = preferences.webRTC.localIps != null ? preferences.webRTC.localIps : new string[0];
            preferences.webRtc = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(webRtc));


            dynamic timezone = new ExpandoObject();
            timezone.id = this.tz.timezone;
            preferences.timezone = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(timezone));

            preferences.proxy = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(this.proxy));

            preferences.webgl_noise_value = preferences.webGL.noise;
            preferences.get_client_rects_noise = preferences.webGL.getClientRectsNoise;
            preferences.canvasMode = preferences.canvas.mode;
            preferences.canvasNoise = preferences.canvas.noise;

            dynamic audioContext = new ExpandoObject();
            audioContext.enable = preferences.audioContext.mode;
            audioContext.noiseValue = preferences.audioContext.noise;
            preferences.audioContext = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(audioContext));


            dynamic metadata = new ExpandoObject();
            metadata.vendor = preferences.webGLMetadata.vendor;
            metadata.renderer = preferences.webGLMetadata.renderer;
            metadata.mode = preferences.webGLMetadata.mode == "mask";
            dynamic webgl = new ExpandoObject();
            webgl.metadata = metadata;
            preferences.webgl = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(webgl));

            if (preferences.navigator.userAgent != null)
            {
                preferences.userAgent = preferences.navigator.userAgent;
            }

            if (preferences.navigator.doNotTrack == "true")
            {
                preferences.doNotTrack = preferences.navigator.doNotTrack;
            }

            if (preferences.navigator.hardwareConcurrency != null)
            {
                preferences.hardwareConcurrency = preferences.navigator.hardwareConcurrency;
            }

            if (preferences.navigator.language != null)
            {
                preferences.language = preferences.navigator.language;
            }
            return preferences;

        }

        public void updatePreferences()
        {
            string pref_file = Path.Combine(this.profile_path, "Default/Preferences");
            string content_pref_file = File.ReadAllText(pref_file);
            dynamic preferences = JsonConvert.DeserializeObject<Object>(content_pref_file);
            dynamic profile = this.profile;
            dynamic proxy = this.proxy == null ? profile.proxy : this.proxy;
            Console.WriteLine(" ### proxy= ", proxy);
            if (proxy != null && (proxy.mode == "gologin" || proxy.mode == "tor"))
            {
                string autoProxyServer = profile.autoProxyServer;
                var splittedAutoProxyServer = autoProxyServer.Replace("://", "|").Split('|');
                var splittedProxyAddress = splittedAutoProxyServer[1].Split(':');
                var port = splittedProxyAddress[1];
                proxy = new ProxyGologin
                {
                    mode = "http",
                    host = splittedProxyAddress[0],
                    port = Int32.Parse(port),
                    username = profile.autoProxyUsername,
                    password = profile.autoProxyPassword,
                    timezone = profile.autoProxyTimezone,

                };
                profile["proxy"]["username"] = profile.autoProxyUsername;
                profile["proxy"]["password"] = profile.autoProxyPassword;
            }

            if (proxy == null || proxy.mode == "none")
            {
                Console.WriteLine("no proxy");
                proxy = null;
            }

            if (proxy != null && proxy.mode == null)
            {
                proxy.mode = "http";
            }
            if (this.proxy == null)
            {
                this.proxy = proxy;
            }

            this.profile_name = profile.name;
            if (this.profile_name == null)
            {
                Console.WriteLine("empty profile name");
                Console.WriteLine("profile=" + profile);
                return;
            }
            var gologin = this.convertPreferences(profile);
            preferences.gologin = gologin;
            File.WriteAllText(pref_file, JsonConvert.SerializeObject(preferences));
        }
        public string createStartup()
        {
            if (!this.local && Directory.Exists(this.profile_path))
            {
                Directory.Delete(this.profile_path, true);
            }
            this.profile = this.getProfile();
            if (!this.local)
            {
                this.downloadProfileZip();
            }

            this.updatePreferences();
            return this.profile_path;
        }

        public string formatProxyUrl(dynamic proxy)
        {
            return proxy.mode + "://" + proxy.host + ':' + proxy.port;
        }
        public string spawnBrowser()
        {
            var proxy = this.proxy;
            string proxy_host = "";
            if (proxy != null)
            {
                if (proxy.mode == null) proxy.mode = "http";
                proxy_host = proxy.host;
                proxy = this.formatProxyUrl(proxy);
            }

            tz = this.tz.timezone;

            List<string> paramss = new List<string>();
            paramss.Add("--remote-debugging-port=" + this.port);
            paramss.Add("--user-data-dir=\"" + this.profile_path + "\"");
            paramss.Add("--password-store=basic");
            paramss.Add("--tz=" + tz);
            paramss.Add("--gologin-profile=\"" + this.profile_name + "\"");
            paramss.Add("--lang=en");
            //============
            //paramss.Add("--allow-http-screen-capture");
            //paramss.Add("--disable-background-networking");
            //paramss.Add("--disable-blink-features=AutomationControlled");
            //paramss.Add("--disable-breakpad");
            //paramss.Add("--disable-cast");
            //paramss.Add("--disable-cast-streaming-hw-encoding");
            //paramss.Add("--disable-client-side-phishing-detection");
            //paramss.Add("--disable-cloud-import");
            //paramss.Add("--disable-default-apps");
            //paramss.Add("--disable-dev-shm-usage");
            //paramss.Add("--disable-geolocation");
            ////paramss.Add("--disable-gpu");
            //paramss.Add("--disable-hang-monitor");
            //paramss.Add("--disable-impl-side-painting");
            //===============
            paramss.Add("--disable-ipv6");
            paramss.Add("--disable-notifications");
            paramss.Add("--enable-popup-blocking");
            paramss.Add("--disable-prompt-on-repost");
            paramss.Add("--disable-seccomp-filter-sandbox");
            paramss.Add("--disable-session-crashed-bubble");
            paramss.Add("--disable-setuid-sandbox");
            paramss.Add("--disable-sync");
            paramss.Add("--disable-web-resources");
            paramss.Add("--enable-application-cache");
            paramss.Add("--enable-automation");
            paramss.Add("--enable-blink-features=ShadowDOMV0");
            //===
            //paramss.Add("--enable-logging");
            //paramss.Add("--excludeswitches");
            //paramss.Add("--false");
            //paramss.Add("--force-fieldtrials=SiteIsolationExtensions/Control");
            //paramss.Add("--ignore-certificate-errors");
            //paramss.Add("--log-level=0");
            //paramss.Add("--no-first-run");
            //paramss.Add("--no-sandbox");
            //paramss.Add("--test-type=webdriver");
            //paramss.Add("--use-mock-keychain");
            //paramss.Add("--useautomationextension");
            //paramss.Add("--flag-switches-begin");
            //paramss.Add("--flag-switches-end");
            paramss.Add("--window-size=900,900");

            if (proxy != null)
            {
                string hr_rules = "\"\\\"MAP * 0.0.0.0 , EXCLUDE " + proxy_host + "\\\"\"";
                paramss.Add("--proxy-server=" + proxy);
                paramss.Add("--host-resolver-rules=" + hr_rules);
            }
            foreach (var param in this.extra_params)
            {
                paramss.Add(param);
            }

            string arg = "";
            foreach (var item in paramss)
            {
                arg += item + " ";
            }

            Process.Start(this.executablePath, arg);

            int try_count = 1;
            string url = this.address + ':' + this.port;
            HttpRequest request = new HttpRequest();
            while (try_count < 100)
            {
                try
                {
                    string data = request.Get("http://" + url + "/json").ToString();
                    break;
                }
                catch (Exception ex)
                {
                }
                try_count += 1;
                Thread.Sleep(1);
            }
            return url;
        }
        public string Start()
        {
            profile_path = this.createStartup();
            Console.WriteLine("profile_path=" + profile_path);
            if (this.spawn_browser)
            {
                return this.spawnBrowser();
            }
            return profile_path;
        }

    }
}
