using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace OE.Service.Utils
{
    public class ServiceHelper
    {

        private static Hashtable mySavedState = new Hashtable();
        public static void Install(string ServiceName, string filepath)
        {
            if (ServiceIsExisted(ServiceName))
            {
                Console.WriteLine("服务已存在！");
                return;
            }
            //Uninstall(ServiceName, filepath);
            using (var myAssemblyInstaller = new AssemblyInstaller())
            {
                myAssemblyInstaller.UseNewContext = true;
                myAssemblyInstaller.Path = filepath;
                myAssemblyInstaller.Install(mySavedState);
                myAssemblyInstaller.Commit(mySavedState);
            };
        }

        public static void Uninstall(string ServiceName, string filepath)
        {
            if (ServiceIsExisted(ServiceName))
            {
                using (var myAssemblyInstaller = new AssemblyInstaller())
                {
                    myAssemblyInstaller.UseNewContext = true;
                    myAssemblyInstaller.Path = filepath;
                    myAssemblyInstaller.Uninstall(null);
                }
            }
            else
            {
                Console.WriteLine("服务不存在！");
            }
        }

        public static bool ServiceIsExisted(string serviceName)
        {
            return ServiceController.GetServices().Any(sv => sv.ServiceName == serviceName);
        }


    }
}
