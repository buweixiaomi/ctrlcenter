using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CCF.Utils
{
    public static class Security
    {
        public static string MakeMD5(string oristring)
        {
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(oristring);
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] resultbs = md5.ComputeHash(bs);
            md5.Dispose();
            StringBuilder sb = new StringBuilder();
            foreach (var a in resultbs)
            {
                sb.Append(a.ToString("x2"));
            }
            return sb.ToString();
        }
        public static byte[] EnDES(byte[] source, byte[] key, byte[] iv)
        {
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, iv), CryptoStreamMode.Write))
                {
                    cs.Write(source, 0, source.Length);
                    cs.FlushFinalBlock();
                }
                return ms.ToArray();
            }
        }

        public static byte[] DeDES(byte[] source, byte[] key, byte[] iv)
        {
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, iv), CryptoStreamMode.Write))
                {
                    cs.Write(source, 0, source.Length);
                    cs.FlushFinalBlock();
                }
                return ms.ToArray();
            }
        }

        public static byte[] EnDES(byte[] source, byte[] key)
        {
            return EnDES(source, key.Take(8).ToArray(), key.Skip(8).ToArray());
        }

        public static byte[] DeDES(byte[] source, byte[] key)
        {
            return DeDES(source, key.Take(8).ToArray(), key.Skip(8).ToArray());
        }

        public static string EnDES(string source, string key)
        {
            var bk = System.Text.Encoding.UTF8.GetBytes(key);
            if (bk.Length > 16)
                bk = bk.Take(16).ToArray();
            else if (bk.Length < 16)
            {
                var tbk = new byte[16];
                bk.CopyTo(tbk, 0);
                bk = tbk;
            }
            var bs = EnDES(System.Text.Encoding.UTF8.GetBytes(source), bk.Take(8).ToArray(), bk.Skip(8).ToArray());
            return CCF.Utils.StringHelper.BytesToString(bs);

        }

        public static string DeDES(string source, string key)
        {
            var bk = System.Text.Encoding.UTF8.GetBytes(key);
            if (bk.Length > 16)
                bk = bk.Take(16).ToArray();
            else if (bk.Length < 16)
            {
                var tbk = new byte[16];
                bk.CopyTo(tbk, 0);
                bk = tbk;
            }
            var bs = DeDES(CCF.Utils.StringHelper.ByteStringToBytes(source), bk.Take(8).ToArray(), bk.Skip(8).ToArray());
            return System.Text.Encoding.UTF8.GetString(bs);
        }


        public static string SHA1(byte[] bs)
        {
            SHA1 sha;
            string hash = "";
            try
            {
                sha = new SHA1CryptoServiceProvider();
                byte[] dataToHash = bs;
                byte[] dataHashed = sha.ComputeHash(dataToHash);
                hash = BitConverter.ToString(dataHashed).Replace("-", "");
                hash = hash.ToLower();
                return hash;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string SHA1(string raw)
        {
            return SHA1(new ASCIIEncoding().GetBytes(raw));
        }

    }
}
