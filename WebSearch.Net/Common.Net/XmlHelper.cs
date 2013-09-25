using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections;

namespace WebSearch.Common.Net
{
    public class XmlHelper
    {
        private static HashCodeLock _fileLock = new HashCodeLock();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="parentNode"></param>
        /// <param name="nodeName"></param>
        /// <param name="attributes"></param>
        public static void AddNode(string fileName, string parentNode, 
            string nodeName, Hashtable attributes)
        {
            using (_fileLock.Lock(fileName))
            {
                XmlDocument domWebSetting = new XmlDocument();
                domWebSetting.Load(fileName);
                XmlNode elem = (XmlNode)domWebSetting.CreateElement(nodeName);
                foreach (string key in attributes.Keys)
                {
                    XmlAttribute attr = domWebSetting.CreateAttribute(key);
                    attr.Value = attributes[key].ToString();
                    elem.Attributes.Append(attr);
                }
                domWebSetting.GetElementsByTagName(parentNode)[0].AppendChild(elem);
                domWebSetting.Save(fileName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="parentID"></param>
        /// <param name="nodeName"></param>
        /// <param name="value"></param>
        public static void AddNode(string fileName, string parentID, string nodeName, string value)
        {
            using (_fileLock.Lock(fileName))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(fileName); // load the xml
                XmlNode elem = (XmlNode)doc.CreateElement(nodeName);
                elem.InnerText = value;
                doc.GetElementById(parentID).AppendChild(elem);
                doc.Save(fileName);
            }
        }

        /// <summary>
        /// Modify the value of given node name
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="xPath"></param>
        /// <param name="nodeName"></param>
        /// <param name="strValue"></param>
        public static void ModifyNode(string fileName, string xPath, string nodeName, string strValue)
        {
            using (_fileLock.Lock(fileName))
            {
                string XPath = xPath == null ? "/root/Setting/?" : xPath;
                XmlDocument doc = new XmlDocument();
                doc.Load(fileName); // load the xml

                XPath = nodeName != null ? XPath.Replace("?", nodeName) : XPath;
                XmlNode node = doc.SelectSingleNode(XPath);
                if (node == null)
                    throw new ArgumentException("No Find the node: " + nodeName);

                node.InnerText = strValue;
                doc.Save(fileName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="id"></param>
        /// <param name="nodeName"></param>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public static bool ModifyNode(string fileName, string id, string nodeName, object value)
        {
            using (_fileLock.Lock(fileName))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(fileName); // load the xml
                XmlElement elem = doc.GetElementById(id);
                if (elem == null) return false;
                if (elem.Attributes[nodeName] != null)
                    elem.Attributes[nodeName].Value = value.ToString();
                else
                {
                    elem.Attributes.Append(doc.CreateAttribute(nodeName));
                    elem.Attributes[nodeName].Value = value.ToString();
                }
                doc.Save(fileName);
            }
            return true;
        }

        /// <summary>
        /// Read the value of the given node name
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="xPath"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public static string ReadNode(string fileName, string xPath, string nodeName)
        {
            using (_fileLock.Lock(fileName))
            {
                string XPath = (xPath == null) ? "/root/Setting/?" : xPath;
                XmlDocument doc = new XmlDocument();
                doc.Load(fileName); // load the xml

                XPath = nodeName != null ? XPath.Replace("?", nodeName) : XPath;
                XmlNode node = doc.SelectSingleNode(XPath);
                if (node == null)
                    throw new ArgumentException("No Find the node:" + nodeName);

                return node.InnerText;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static XmlElement ReadNode(string fileName, int id)
        {
            using (_fileLock.Lock(fileName))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(fileName); // load the xml
                return doc.GetElementById("ID" + id.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static XmlElement ReadNode(string fileName, string id)
        {
            using (_fileLock.Lock(fileName))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(fileName); // load the xml
                return doc.GetElementById(id);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="id"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public static bool IncreaseNode(string fileName, string id, string nodeName)
        {
            using (_fileLock.Lock(fileName))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(fileName); // load the xml

                XmlElement elem = doc.GetElementById(id);
                if (elem == null) return false;
                try
                {
                    int value = int.Parse(elem.Attributes[nodeName].Value);
                    value++;
                    elem.Attributes[nodeName].Value = value.ToString();
                }
                catch (Exception ex)
                {
                    return false;
                }
                doc.Save(fileName);
                return true;
            }
        }
    }
}
