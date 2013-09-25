using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Collections;
using WebSearch.Common.Net;
using System.Xml;

namespace WebSearch.Crawler.Net
{
    public static class WebProxies
    {
        private static Hashtable _cache = new Hashtable();

        public static WebProxy Get(string name)
        {
            if (_cache.ContainsKey(name))
                return (WebProxy)_cache[name];

            // get the proxy object from xml
            XmlElement elem = XmlHelper.ReadNode(Config.SettingPath + "proxies.xml", name);
            WebProxy proxy = new WebProxy(elem.Attributes["_Address"].Value);
            proxy.Credentials = new NetworkCredential(
                elem.Attributes["_UserName"].Value, 
                elem.Attributes["_Password"].Value);

            _cache.Add(name, proxy);
            return proxy;
        }
    }
}
