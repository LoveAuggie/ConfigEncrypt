using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using ConfigEncrypt.Core;

namespace System.Configuration
{
    public class ConfigHelper
    {
        public static NameValueCollection AppSettings { get; private set; } = new NameValueCollection();
        public static ConnectionStringSettingsCollection ConnectionStrings { get; private set; } = new ConnectionStringSettingsCollection();

        static ConfigHelper()
        {
            // 先将所有的ConfigManager加进来(对于调试模式？）
            foreach(var key in ConfigurationManager.AppSettings.AllKeys)
            {
                AppSettings.Add(key, ConfigurationManager.AppSettings[key]);
            }
            foreach (ConnectionStringSettings cs in ConfigurationManager.ConnectionStrings)
            {
                ConnectionStrings.Add(cs);
            }

            var DescFile = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "configEncrypt.desc");
            if (!File.Exists(DescFile))
            {
                DescFile = Path.Combine(new DirectoryInfo(System.AppDomain.CurrentDomain.BaseDirectory).Parent.FullName, "configEncrypt.desc");
            }
            if (!File.Exists(DescFile))
            {
                return;
            }

            var bytes = File.ReadAllBytes(DescFile);

            var deType = Encoding.UTF8.GetString(bytes.Take(30).ToArray()).Replace("\0", "").Split('|');
            var deMain = Encoding.UTF8.GetString(bytes.Skip(30).ToArray());

            switch (deType[0])
            {
                case "-sm4":
                    deMain = SM4Utils.Decrypt_ECB(deMain, deType[1]);
                    break;
                case "-des":
                default:
                    deMain = DesHelper.ToDESDecrypt(deMain, deType[1]);
                    break;
            }

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(deMain);
            var cS = doc.SelectSingleNode("ConfigEncrypt/connectionStrings");
            if (cS != null)
            {
                foreach (XmlNode cn in cS.ChildNodes)
                {
                    ConnectionStrings.Add(new ConnectionStringSettings(cn.Attributes["name"].Value, cn.Attributes["connectionString"].Value, cn.Attributes["providerName"]?.Value));
                }
            }

            var aS = doc.SelectSingleNode("ConfigEncrypt/appSettings");
            if (aS != null)
            {
                foreach (XmlNode an in aS.ChildNodes)
                {
                    if(an.NodeType!= XmlNodeType.Comment)
                        AppSettings.Add(an.Attributes["key"].Value, an.Attributes["value"].Value);
                }
            }
        }
    }
}
