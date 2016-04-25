using Gecko;
using System.Web;

namespace Worker
{
    public class WebSocketSIDGenerator
    {
        public delegate void OnNewSIDGeneratedDelegate(WebSocketSIDGenerator generator, string SID);
        public event OnNewSIDGeneratedDelegate OnNewSIDGenerated;

        private GeckoWebBrowser browser;
        private bool generating;

        public WebSocketSIDGenerator()
        {
            Gecko.Xpcom.Initialize("..\\..\\xulrunner");
            browser = new GeckoWebBrowser();

            browser.CreateWindow += Browser_CreateWindow;
            browser.NavigationError += Browser_NavigationError;
            browser.ObserveHttpModifyRequest += Browser_ObserveHttpModifyRequest;
            browser.UseHttpActivityObserver = true;

            generating = false;
        }

        public void SetProxy(string host, int port)
        {
            if (host == null || host == string.Empty)
            {
                GeckoPreferences.Default["network.proxy.type"] = 0;
                GeckoPreferences.Default["network.proxy.http"] = "";
                GeckoPreferences.Default["network.proxy.http_port"] = 80;
                GeckoPreferences.User["network.proxy.type"] = 0;
                GeckoPreferences.User["network.proxy.http"] = "";
                GeckoPreferences.User["network.proxy.http_port"] = 80;
            }
            else
            {
                GeckoPreferences.Default["network.proxy.type"] = 1;
                //GeckoPreferences.Default["network.proxy.http"] = host;
                //GeckoPreferences.Default["network.proxy.http_port"] = port;
                GeckoPreferences.Default["network.proxy.socks"] = host;
                GeckoPreferences.Default["network.proxy.socks_port"] = port;
                GeckoPreferences.User["network.proxy.socks_version"] = 5;
                GeckoPreferences.User["network.proxy.socks"] = host;
                GeckoPreferences.User["network.proxy.socks_port"] = port;
                GeckoPreferences.User["network.proxy.socks_version"] = 5;
                GeckoPreferences.User["network.proxy.type"] = 1;
                //GeckoPreferences.User["network.proxy.http"] = host;
                //GeckoPreferences.User["network.proxy.http_port"] = port;
            }
            GeckoPreferences.Default["browser.xul.error_pages.enabled"] = true;
            GeckoPreferences.User["browser.xul.error_pages.enabled"] = true;
        }

        private void Browser_ObserveHttpModifyRequest(object sender, GeckoObserveHttpModifyRequestEventArgs e)
        {
            if (generating)
            {
                var query = HttpUtility.ParseQueryString(e.Uri.Query);
                if (query.HasKeys() && query.GetValues("transport") != null && query.GetValues("transport")[0].CompareTo("websocket") == 0)
                {
                    string sid = query.GetValues("sid")[0];

                    e.Cancel = true;

                    generating = false;
                    browser.Stop();
                    browser.LoadHtml("");

                    if (OnNewSIDGenerated != null)
                    {
                        OnNewSIDGenerated(this, sid);
                    }
                }
            }
        }

        private void Browser_NavigationError(object sender, Gecko.Events.GeckoNavigationErrorEventArgs e)
        {
            if (generating)
            {
                generating = false;
                browser.Stop();
                browser.LoadHtml("");

                OnNewSIDGenerated(this, string.Empty);
            }
        }

        private void Browser_CreateWindow(object sender, GeckoCreateWindowEventArgs e)
        {
            e.Cancel = true;
        }

        public bool GenerateSid(string proxy_host = null, int proxy_port = 0)
        {
            if(generating)
            {
                return false;
            }

            generating = true;

            SetProxy(proxy_host, proxy_port);
            browser.Navigate("http://www.praatanoniem.nl/");

            return true;
        }

        public bool GeneratingInProgress()
        {
            return generating;
        }
    }
}
