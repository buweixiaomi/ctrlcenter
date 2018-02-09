using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ManageDomain
{
    public class PermissionProvider
    {
        static PermissionProvider()
        {
            InitKeyTree();
        }
        private static ManageDomain.BLL.PermissionBll permissionbll = new BLL.PermissionBll();
        private static Dictionary<string, List<string>> PermissionCache = new Dictionary<string, List<string>>();
        private static object locker = new object();
        private static readonly List<PermissionItem> PermissionTree = new List<PermissionItem>();
        public static bool Exist(int managerid, SystemPermissionKey key)
        {
            var v = EnumHelper.GetEnumAttr<PermissionKeyAttribute>(key);
            if (v == null)
                return true;
            return Exist(managerid, v.Key);
        }

        public static void ClearCache()
        {
            lock (locker)
            {
                PermissionCache.Clear();
            }
        }
        public static bool Exist(SystemPermissionKey key)
        {
            int id = Pub.CurrUserId();
            if (id <= 0)
                return false;
            var v = EnumHelper.GetEnumAttr<PermissionKeyAttribute>(key);
            if (v == null)
                return true;
            return Exist(id, v.Key);
        }
        public static bool Exist(int managerid, string key)
        {
            return permissionbll.ManagerExistKey(managerid, key);
        }

        public static bool ExistWidthCache(int managerid, string key)
        {
            if (managerid <= 0)
                return false;
            if (!PermissionCache.ContainsKey(managerid.ToString()))
            {
                InitManagerKeys(managerid);
            }
            var keys = PermissionCache[managerid.ToString()];
            if (keys.Contains(key.ToLower()))
                return true;
            return false;
        }

        public static bool ExistWidthCache(SystemPermissionKey key)
        {
            int managerid = Pub.CurrUserId();
            if (managerid <= 0)
                return false;
            var v = EnumHelper.GetEnumAttr<PermissionKeyAttribute>(key);
            if (v == null)
                return false;
            return ExistWidthCache(managerid, v.Key);
        }

        public static void CheckExist(SystemPermissionKey key)
        { 
            int id = Pub.CurrUserId();
            if (id <= 0)
            {
                throw new MException(MExceptionCode.NoPermission, "不允许未登录用户！");
            }
            var v = EnumHelper.GetEnumAttr<PermissionKeyAttribute>(key);
            if (v == null)
                return;
            bool exist = Exist(id, v.Key);
            if (exist == false)
            {
                throw new MPermissionException(key, "你没有操作权限！");
            }
        }

        public static int GetManagerId()
        {
            if (!System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
                return 0;
            var token = Pub.GetTokenModel(System.Web.HttpContext.Current.User.Identity.Name);
            if (token == null)
                return 0;
            return CCF.DB.LibConvert.ObjToInt(token.Id);
        }

        public static void InitManagerKeys(int managerid)
        {
            var keys = permissionbll.GetManagerKeys(managerid);
            if (keys == null)
                keys = new List<string>();
            for (int i = 0; i < keys.Count; i++)
            {
                keys[i] = keys[i].ToLower();
            }
            lock (locker)
            {
                PermissionCache[managerid.ToString()] = keys;
            }
        }
        public static void RemoveManagerKeys(int mangerid)
        {
            lock (locker)
            {
                PermissionCache.Remove(mangerid.ToString());
            }
        }


        public static string GetPermissionKey(SystemPermissionKey key)
        {
            var keys = EnumHelper.GetEnumAttr<PermissionKeyAttribute>(key);
            if (keys == null)
                return "";
            return keys.Key;
        }

        private static void InitKeyTree()
        {
            List<PermissionItem> items = new List<PermissionItem>();
            var types = typeof(SystemPermissionKey);
            var ens = types.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (var a in ens)
            {
                ManageDomain.PermissionKeyAttribute currattr = null;
                var attr = a.GetCustomAttributes(typeof(ManageDomain.PermissionKeyAttribute), false);
                if (attr != null && attr.Length > 0)
                    currattr = attr[0] as ManageDomain.PermissionKeyAttribute;
                if (currattr == null)
                    continue;
                if (!string.IsNullOrEmpty(currattr.Key))
                {
                    if (items.Exists(x => x.Key == currattr.Key.ToLower()))
                        continue;
                }
                items.Add(new PermissionItem()
                {
                    GroupName = currattr.Group ?? "",
                    HasPermission = 0,
                    Key = currattr.Key.ToLower(),
                    Name = currattr.Name,
                    SubPermissions = new List<PermissionItem>()
                });
            }
            for (int i = items.Count - 1; i >= 0; i--)
            {
                var curr = items[i];
                if (!string.IsNullOrEmpty(curr.GroupName))
                {
                    for (int k = 0; k < items.Count; k++)
                    {
                        if (k == i)
                            continue;
                        if (SetSub(items[k], items[i]))
                        {
                            items.Remove(curr);
                            break;
                        }
                    }
                }
            }
            PermissionTree.AddRange(items);
        }

        private static bool SetSub(PermissionItem parent, PermissionItem sub)
        {
            if (!string.IsNullOrEmpty(parent.Name) && !string.IsNullOrEmpty(sub.GroupName))
            {
                if (parent.Name == sub.GroupName)
                {
                    parent.SubPermissions.Add(sub);
                    return true;
                }
            }
            foreach (var a in parent.SubPermissions)
            {
                bool isok = SetSub(a, sub);
                if (isok)
                    return true;
            }
            return false;
        }

        public static List<PermissionItem> GetPermissionTree()
        {
            List<PermissionItem> items = new List<PermissionItem>();
            foreach (var a in PermissionTree)
            {
                items.Add(a.Clone());
            }
            return items;
        }
    }

    public class PermissionItem
    {
        public string Key { get; set; }
        public string Name { get; set; }

        public string GroupName { get; set; }

        public int HasPermission { get; set; }

        public PermissionItem Clone()
        {
            var newmodel = new PermissionItem()
            {
                SubPermissions = new List<PermissionItem>(),
                GroupName = this.GroupName,
                Name = this.Name,
                Key = this.Key,
                HasPermission = this.HasPermission,
            };
            foreach (var a in this.SubPermissions)
                newmodel.SubPermissions.Add(a.Clone());
            return newmodel;
        }

        public List<PermissionItem> SubPermissions { get; set; }
    }
}
