using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigEncrypt.Exe
{
    /// <summary>
    /// 
    /// </summary>
    internal class ExeConfig
    {
        /* -h : 打开帮助文档
         * -r : 注册当前程序
         * -d : 运行目录
         * -f : 待加密的程序
         * 示例：cec.exe -d D:\test\bin -f a.exe b.exe
         */
        internal bool help { get; set; }

        internal bool register { get; set; }

        internal string Directory { get; set; }

        internal string Exe { get; set; }

        internal EnType EnType { get; set; } = EnType.SM4;

        internal string EnKey { get; set; }

        internal string Enviroment { get; set; }

        internal static ExeConfig Load(string[] args)
        {
            try
            {
                List<string> aList = args.ToList();
                ExeConfig config = new ExeConfig();
                if (aList.Contains("-h"))
                {
                    config.help = true;
                    return config;
                }
                if (aList.Contains("-r"))
                {
                    config.register = true;
                    return config;
                }
                for (int i = 0; i < aList.Count; i++)
                {
                    var arg = aList[i];
                    switch (arg)
                    {
                        case "-des":
                            config.EnType = EnType.DESC;
                            break;
                        case "-sm4":
                            config.EnType = EnType.SM4;
                            break;
                        case "-k":
                            config.EnKey = aList[i + 1];
                            i++;
                            break;
                        case "-d":
                            config.Directory = aList[i + 1];
                            i++;
                            break;
                        case "-f":
                            config.Exe = aList[i + 1];
                            i++;
                            break;
                        case "-e":
                            config.Enviroment = aList[i + 1];
                            i++;
                            break;
                        default:
                            break;
                    }
                }
                return config;
            }
            catch
            {
                return null;
            }
        }
    }

    internal enum EnType
    {
        DESC,
        SM4,
    }
}
