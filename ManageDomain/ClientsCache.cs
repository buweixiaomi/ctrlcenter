using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageDomain
{
    public class ClientsCache
    {
        private ClientsCache() { }
        private static Dictionary<string, string> clients = new Dictionary<string, string>();
        private static object Locker = new object();
        public const int MaxLength = 1000;
       private static DateTime lastcleartime = DateTime.Now;
        public static bool AddClientInfo(string clientid, string msg)
        {
            if (clients.Count >= MaxLength)
            {
                lock (Locker)
                {
                    clients.Clear();
                }
            }

            if ((DateTime.Now - lastcleartime).TotalHours > 24)
            {
                lock (Locker)
                {
                    lastcleartime = DateTime.Now;
                    clients.Clear();
                }
            }


            lock (Locker)
            {
                if (clients.Count >= MaxLength)
                {
                    return false;
                }
                clients[clientid] = msg;
                return true;
            }
        }

        public static Dictionary<string, string> GetAll()
        {
            Dictionary<string, string> r = new Dictionary<string, string>();
            foreach (var a in clients)
            {
                r[a.Key] = a.Value;
            }
            return r;
        }

        public static void Remove(string clientid)
        {
            if (!clients.ContainsKey(clientid))
                return;
            lock (Locker)
            {
                if (clients.ContainsKey(clientid))
                    clients.Remove(clientid);
            }
        }
    }
}
