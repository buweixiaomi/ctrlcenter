using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OE.Service.Utils
{
    public class Utils
    {
        public static Dictionary<string, string> GetDicFromObject(object obj)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            if (obj == null)
                return para;
            Type objtype = obj.GetType();
            if (objtype.IsClass && objtype.Name.ToLower() != "string")
            {
                foreach (System.Reflection.PropertyInfo a in objtype.GetProperties())
                {
                    string key = a.Name;
                    object value = a.GetValue(obj, null);
                    para.Add(key, value == null ? null : value.ToString());
                }
            }
            return para;
        }

        /// <summary>
        /// 解压缩一个 zip 文件。
        /// </summary>
        /// <param name="zipedFile">The ziped file.</param>
        /// <param name="strDirectory">The STR directory.</param>
        /// <param name="password">zip 文件的密码。</param>
        /// <param name="overWrite">是否覆盖已存在的文件。</param>
        public static void UnZip(string zipedFile, string strDirectory, string password, bool overWrite)
        {

            if (strDirectory == "")
                strDirectory = Directory.GetCurrentDirectory();
            if (!strDirectory.EndsWith("\\"))
                strDirectory = strDirectory + "\\";
            if (!Directory.Exists(strDirectory))
                Directory.CreateDirectory(strDirectory);

            using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipedFile)))
            {
                s.Password = password;
                ZipEntry theEntry;

                while ((theEntry = s.GetNextEntry()) != null)
                {
                    string directoryName = "";
                    string pathToZip = "";
                    pathToZip = theEntry.Name;

                    if (pathToZip != "")
                        directoryName = Path.GetDirectoryName(pathToZip) + "\\";

                    string fileName = Path.GetFileName(pathToZip);


                    if (!Directory.Exists(strDirectory + directoryName))
                        Directory.CreateDirectory(strDirectory + directoryName);

                    if (fileName != "")
                    {
                        if ((File.Exists(strDirectory + directoryName + fileName) && overWrite) || (!File.Exists(strDirectory + directoryName + fileName)))
                        {
                            using (FileStream streamWriter = File.Create(strDirectory + directoryName + fileName))
                            {
                                int size = 2048;
                                byte[] data = new byte[2048];
                                while (true)
                                {
                                    size = s.Read(data, 0, data.Length);

                                    if (size > 0)
                                        streamWriter.Write(data, 0, size);
                                    else
                                        break;
                                }
                                streamWriter.Close();
                            }
                        }
                    }
                }

                s.Close();
            }
        }

        public static void CopyFile(string sourcefile, string tarfile)
        {
            System.IO.FileInfo fi = new FileInfo(tarfile);
            if (!fi.Directory.Exists)
                fi.Directory.Create();
            System.IO.File.Copy(sourcefile, tarfile);
        }

        public static int CopyDir(string sourcedir, string tardir, bool containsubdir = true)
        {
            sourcedir = sourcedir.Trim().TrimEnd('\\');
            tardir = tardir.Trim().TrimEnd('\\');

            if (sourcedir.ToLower() == tardir.ToLower())
                return 0;
            int count = 0;
            System.IO.DirectoryInfo dirinfo = new System.IO.DirectoryInfo(sourcedir);
            if (!System.IO.Directory.Exists(tardir))
                System.IO.Directory.CreateDirectory(tardir);
            foreach (var a in dirinfo.GetFiles())
            {
                a.CopyTo(tardir.TrimEnd('\\') + "\\" + a.Name, true);
                count++;
            }
            if (containsubdir)
            {
                foreach (var a in dirinfo.GetDirectories())
                {
                    if (a.FullName.ToLower() == tardir.ToLower())
                        continue;
                    count += CopyDir(a.FullName, tardir.TrimEnd('\\') + "\\" + a.Name);
                }
            }
            return count;
        }


        public static int DeleteFileOrDir(string path)
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
                return 1;
            }
            if (System.IO.Directory.Exists(path))
            {
                System.IO.Directory.Delete(path, true);
                return 1;
            }
            return 0;
        }

        public static string SerializeDataTable(System.Data.DataTable tb)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(tb, Newtonsoft.Json.Formatting.Indented);
            //List<object> obj = new List<object>();
            //foreach(System.Data.DataRow dr in tb.Rows)
            //{
            //    Newtonsoft.Json.JsonConvert.SerializeObject(tb);
            //    var dy = new object();
            //    dy.

            //}
        }

        public static string SerializeObject(object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            //List<object> obj = new List<object>();
            //foreach(System.Data.DataRow dr in tb.Rows)
            //{
            //    Newtonsoft.Json.JsonConvert.SerializeObject(tb);
            //    var dy = new object();
            //    dy.

            //}
        }


        public static T DeserializeObject<T>(string jsonobj)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonobj);
        }

        public static void GetIpsAndMacs(out string[] iparr, out string[] macarr)
        {
            List<string> macAddress = new List<string>();
            List<string> ips = new List<string>();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                IPInterfaceProperties properties = adapter.GetIPProperties();
                // 每个网络接口可能会有多个IP地址 
                foreach (IPAddressInformation address in properties.UnicastAddresses)
                {
                    // 如果此IP不是ipv4，则进行下一次循环
                    if (address.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        if (!System.Net.IPAddress.IsLoopback(address.Address))
                        {
                            ips.Add(address.Address.ToString());
                        }
                    }
                }
                if (!adapter.GetPhysicalAddress().ToString().Equals(""))
                {
                    string tmac = adapter.GetPhysicalAddress().ToString();
                    for (int i = 1; i < 6; i++)
                    {
                        tmac = tmac.Insert(3 * i - 1, ":");
                    }
                    macAddress.Add(tmac);
                }
            }
            macarr = macAddress.Distinct().ToArray();
            iparr = ips.Distinct().ToArray();
        }
    }
}
