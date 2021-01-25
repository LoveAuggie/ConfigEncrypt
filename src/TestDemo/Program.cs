using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace TestDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var keys = ConfigHelper.AppSettings.AllKeys;
            foreach (var key in keys)
            {
                Console.WriteLine(key + " => " + ConfigHelper.AppSettings[key]);
            }
            foreach (ConnectionStringSettings cs in ConfigHelper.ConnectionStrings)
            {
                Console.WriteLine(cs.Name + " => " + cs.ConnectionString);
            }

            Console.ReadKey();
        }
    }
}
