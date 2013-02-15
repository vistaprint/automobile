using System.Threading;
using Automobile.Mobile.Framework.Browser;

namespace Automobile.Mobile.Framework.UnitTests.Helpers
{
    public class MobileBrowserDummy : IMobileBrowser
    {
        public string Url { get; set; }

        public string Js { get; set; }

        public string ExecJavascript(string js)
        {
            Js = js;
            return Js;
        }

        public void Navigate(string url)
        {
            Url = url;
        }

        public void Navigate(string url, bool useBaseUrl)
        {
            Url = url;
        }

        public void Refresh()
		{
		}

        public void WaitForReady()
        {
            Thread.Sleep(1000);
        }
    }
}