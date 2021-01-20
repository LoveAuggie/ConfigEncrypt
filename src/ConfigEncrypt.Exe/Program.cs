using ConfigEncrypt.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ConfigEncrypt.Exe
{
    class Program
    {
        private static ExeConfig config;
        static void Main(string[] args)
        {
            config = ExeConfig.Load(args);
            if (config == null)
            {
                Console.WriteLine("参数配置不正确，请重试!");
                System.Threading.Thread.Sleep(1000);
                return;
            }
            Console.WriteLine("args: " + string.Join(" ", args) + "\r\n");

            if (args.Length == 0 || config.help)
            {
                Console.WriteLine("-h   : 打开帮助文档");
                Console.WriteLine("-r   : 注册（管理员权限）");
                Console.WriteLine("-des : 使用DES加解密算法");
                Console.WriteLine("-sm4 : 使用SM4加解密算法");
                Console.WriteLine("-k   : 加密字符串，不配置则使用默认加密字符串");
                Console.WriteLine("-d   : 加密配置所在文件夹");
                Console.WriteLine("-f   : 需要加密的程序，可支持多个(例如：demo.exe,demo.dll)");
                System.Threading.Thread.Sleep(2000);
                return;
            }
            if (config.register)
            {
                // 将当前文件夹设置到Path类
                var path1 = Environment.GetEnvironmentVariable("path",EnvironmentVariableTarget.Machine);
                if (!path1.Contains(System.Environment.CurrentDirectory))
                {
                    var new_path = path1 + ";" + System.Environment.CurrentDirectory;
                    Environment.SetEnvironmentVariable("path", new_path, EnvironmentVariableTarget.Machine);
                }
                var path2 = Environment.GetEnvironmentVariable("path", EnvironmentVariableTarget.User);
                if (!path2.Contains(System.Environment.CurrentDirectory))
                {
                    var new_path = path2 + ";" + System.Environment.CurrentDirectory;
                    Environment.SetEnvironmentVariable("path", new_path, EnvironmentVariableTarget.User);
                }
                Console.WriteLine("环境设置成功！");
                return;
            }
            Console.WriteLine("开始执行程序加密...");

            foreach(var ef in config.Exes)
            {
                var file = Path.Combine(config.Directory, ef+".config");
                Console.WriteLine("加密文件：" + file);
                if (!File.Exists(file))
                {
                    Console.WriteLine("    => 失败：配置文件不存在！");
                    continue;
                }
                XmlDocument document = new XmlDocument();
                document.Load(file);

                XmlDocument desDoc = new XmlDocument();
                var root = desDoc.CreateElement("ConfigEncrypt");
                desDoc.AppendChild(root);

                var ConfigSettings = document.SelectSingleNode("*/connectionStrings");
                if (ConfigSettings != null)
                {
                    root.AppendChild(EncryptConfig(ConfigSettings, "connectionStrings", document, desDoc));
                }

                var AppSettings = document.SelectSingleNode("*/appSettings");
                if (AppSettings != null)
                {
                    root.AppendChild(EncryptConfig(AppSettings, "appSettings", document, desDoc));
                }

                document.Save(file);
                //desDoc.Save(file + ".desc");
                string enType;
                byte[] bytes = null;
                switch (config.EnType)
                {
                    case EnType.SM4:
                        enType = "-sm4";
                        bytes = Encoding.UTF8.GetBytes(SM4Utils.Encrypt_ECB(desDoc.OuterXml, config.EnKey));
                        break;
                    case EnType.DESC:
                    default:
                        enType = "-des";
                        bytes = Encoding.UTF8.GetBytes(DesHelper.ToDESEncrypt(desDoc.OuterXml, config.EnKey));
                        break;
                }

                byte[] result = new byte[bytes.Length + 30];
                var deByte = Encoding.UTF8.GetBytes(enType + "|" + config.EnKey);

                deByte.CopyTo(result, 0);
                bytes.CopyTo(result, 30);
                File.WriteAllBytes(config.Directory + "configEncrypt.desc", result);
                Console.WriteLine("    => 成功！");
            }
            Console.WriteLine("加密完成!");
            System.Threading.Thread.Sleep(1000);
            System.Environment.Exit(0);
        }

        private static XmlNode EncryptConfig(XmlNode configNode, string configDesc, XmlDocument configDoc, XmlDocument desDoc)
        {
            var csNode = desDoc.CreateElement(configDesc);
            if (configNode.ChildNodes.Count > 0)
            {
                var xml = configNode.InnerXml;
                // 原数据保存
                csNode.InnerXml = xml;
            }
            else
            {
                var cs = configNode.Attributes["configSource"];
                if (cs == null)
                    return csNode;
                var path = cs.Value;
                var cPath = Path.Combine(config.Directory, path);
                if (!File.Exists(cPath))
                    return csNode;

                var childDoc = new XmlDocument();
                childDoc.Load(cPath);
                var childNode = childDoc.SelectSingleNode(configDesc);
                if (childNode == null)
                    return csNode;

                csNode.InnerXml = childNode.InnerXml;
                configNode.Attributes.Remove(cs);
                File.Delete(cPath); // 配置文件删除
             }

            // 删除原数据
            configNode.RemoveAll();
            // 添加加密标识
            //var node = configDoc.CreateElement("csEnrypt");
            //node.SetAttribute("desc", $"{configDesc}");
            //configNode.AppendChild(node);

            return csNode;
        }
    }
}
