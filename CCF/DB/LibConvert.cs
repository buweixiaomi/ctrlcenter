using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace CCF.DB
{
    public static class LibConvert
    {

        /// <summary>返回有关指定对象是否为 System.TypeCode.DBNull 类型的指示。</summary>
        /// <param name="Obj">一个对象</param>
        /// <returns></returns>
        public static bool IsDbNull(object Obj)
        {
            return Convert.IsDBNull(Obj);
        }

        /// <summary>日期型转整型</summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long DateTimeToInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (long)(time - startTime).TotalSeconds;
        }

        /// <summary>整型转日期型</summary>
        /// <param name="Seconds"></param>
        /// <returns></returns>
        public static DateTime IntToDateTime(long Seconds)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return startTime.AddSeconds(Seconds);
        }

        /// <summary>【通用函数】string类型转换为short类型
        /// </summary>
        public static short StrToShort(string aStr)
        {
            short iRet = 0;
            short.TryParse(aStr, out iRet);
            return iRet;
        }

        /// <summary>【通用函数】string类型转换为int类型
        /// </summary>
        public static int StrToInt(string aStr)
        {
            int iRet = 0;
            int.TryParse(aStr, out iRet);
            return iRet;
        }

        /// <summary>【通用函数】string类型转换为Tnt64类型
        /// </summary>
        public static Int64 StrToInt64(string aStr)
        {
            Int64 iRet = 0;
            Int64.TryParse(aStr, out iRet);
            return iRet;
        }

        /// <summary>【通用函数】string类型转换为double类型
        /// </summary>
        public static double StrToDouble(string aStr)
        {
            double dRet = 0;
            double.TryParse(aStr, out dRet);
            if (double.IsNaN(dRet)) return 0;
            return dRet;
        }

        /// <summary>【通用函数】string类型转换为float类型
        /// </summary>
        public static float StrToFloat(string aStr)
        {
            float fRet = 0;
            float.TryParse(aStr, out fRet);
            return fRet;
        }

        /// <summary>【通用函数】string类型转换为decimal类型
        /// </summary>
        public static decimal StrToDecimal(string aStr)
        {
            decimal dRet = 0;
            decimal.TryParse(aStr, out dRet);
            return dRet;
        }

        /// <summary>【通用函数】string类型转换为日期类型,转换失败返回DateTime.Now
        /// </summary>
        public static DateTime StrToDateTime(string aStr)
        {
            DateTime dRet = DateTime.Now;
            if (DateTime.TryParse(aStr, out dRet))
                return dRet;
            else
                return DateTime.Now;
        }
        /// <summary>【通用函数】string类型转换为BOOL类型,转换失败返回False
        /// </summary>
        public static Boolean StrToBoolean(string aStr)
        {
            Boolean result;
            if (!Boolean.TryParse(aStr, out result))
            {
                return false;
            }
            return result;
        }


        /// <summary>【通用函数】string类型转换为日期类型
        /// </summary>
        /// <param name="aStr">要转换的字符串</param>
        /// <param name="aDefault">如果转换失败的默认值</param>
        /// <returns></returns>
        public static DateTime StrToDateTime(string aStr, DateTime aDefault)
        {
            DateTime dRet = aDefault;
            if (DateTime.TryParse(aStr, out dRet))
                return dRet;
            else
                return aDefault;
        }

        /// <summary>【通用函数】int类型转换为bool类型(1为TRUE，其余为FALSE)
        /// </summary>
        public static bool IntToBool(int aInt)
        {
            return (aInt == 1);
        }

        /// <summary>【通用函数】bool类型转换为int类型(TRUE为1，FALSE为0)
        /// </summary>
        public static int BoolToInt(bool aBool)
        {
            if (aBool)
                return 1;
            else
                return 0;
        }

        /// <summary>【通用函数】object类型转换为short类型
        /// </summary>
        public static short ObjToShort(object aObj)
        {
            if (aObj == null) return 0;
            short iRet = 0;
            short.TryParse(aObj.ToString(), out iRet);
            return iRet;
        }

        /// <summary>【通用函数】object类型转换为int类型
        /// </summary>
        public static int ObjToInt(object aObj)
        {
            if (aObj == null) return 0;
            if (aObj.GetType() == typeof(bool))
                return ((bool)aObj) ? 1 : 0;
            int iRet = 0;
            int.TryParse(aObj.ToString(), out iRet);
            return iRet;
        }

        /// <summary>【通用函数】object类型转换为Int64类型
        /// </summary>
        public static Int64 ObjToInt64(object aObj)
        {
            if (aObj == null) return 0;
            Int64 iRet = 0;
            Int64.TryParse(aObj.ToString(), out iRet);
            return iRet;
        }

        /// <summary>【通用函数】object类型转换为double类型
        /// </summary>
        public static double ObjToDouble(object aObj)
        {
            if (aObj == null) return 0;
            double dRet = 0;
            double.TryParse(aObj.ToString(), out dRet);
            if (double.IsNaN(dRet)) return 0;
            return dRet;
        }

        /// <summary>【通用函数】object类型转换为decimal类型
        /// </summary>
        public static decimal ObjToDecimal(object aObj)
        {
            if (aObj == null) return 0;
            decimal dRet = 0;
            decimal.TryParse(aObj.ToString(), out dRet);
            return dRet;
        }

        /// <summary>【通用函数】object类型转换为float类型
        /// </summary>
        public static float ObjToFloat(object aObj)
        {
            if (aObj == null) return 0;
            float dRet = 0;
            float.TryParse(aObj.ToString(), out dRet);
            return dRet;
        }


        /// <summary>【通用函数】object类型转换为datetime类型
        /// </summary>
        public static DateTime ObjToDateTime(object aObj)
        {
            DateTime dRet = new DateTime();
            if (aObj == null) return dRet;

            try
            {
                return Convert.ToDateTime(aObj);
            }
            catch
            {
                return dRet;
            }
        }

        /// <summary>【通用函数】object类型转换为bool类型，直接强制转换(bool)aObj
        /// </summary>
        public static bool ObjToBool(object aObj)
        {
            if (aObj == null) return false;
            if (aObj is bool)
            {
                return (bool)aObj;
            }
            if (aObj.ToString() == "true") return true;
            if (ObjToInt(aObj) == 1)
                return true;
            else
                return false;
        }

        /// <summary>【通用函数】object类型转换为string类型
        /// </summary>
        public static string NullToStr(object aObj)
        {
            if (aObj == null) return "";
            if (Convert.IsDBNull(aObj)) return "";
            return aObj.ToString();
        }
        /// <summary>【通用函数】相当于NullToStr
        /// </summary>
        public static string ObjToStr(object aObj)
        {
            return NullToStr(aObj);
        }

        ///// <summary>【通用函数】FONT转STRING
        ///// </summary>
        //public static string FontToStr(Font aFont)
        //{
        //    FontConverter fc = new FontConverter();
        //    return fc.ConvertToInvariantString(aFont);
        //}

        ///// <summary>【通用函数】STRING转FONT
        ///// </summary>
        //public static Font StrToFont(string aStr)
        //{
        //    FontConverter fc = new FontConverter();
        //    Font f = (Font)fc.ConvertFromString(aStr);
        //    return f;
        //}

        /// <summary>【通用函数】金额小写转换成大写</summary>
        /// <param name="Value">数字</param>
        /// <param name="AState">状态，true表示完整，false表示简写</param>
        /// <returns>返回字符串，如：叁佰贰拾伍元整</returns>
        public static string MoneyToUpper(double Value, bool AState)
        {
            const string NUM = "零壹贰叁肆伍陆柒捌玖";
            const string WEI = "分角元拾佰仟万拾佰仟亿拾佰仟";
            string F = "";
            double d = StrToDouble(Value.ToString("0.00"));
            if (AState)
            {
                if (d < 0)
                {
                    d = -d;
                    F = "负";
                }
                else if (d == 0)
                {
                    F = "零元零角零分";
                }
                else
                {
                    F = "";
                }
                int L = (int)Math.Truncate(Math.Log(d, 10));
                string Str = "";
                for (int i = L; i >= -2; i--)
                {
                    Int64 T = (Int64)(Math.Round(d, 2) / Math.Pow(10, i) + 0.000001);
                    int N = (int)(T % 10);
                    Str = Str + NUM.Substring(N, 1) + WEI.Substring(i + 2, 1);
                }
                Str = F + Str;
                return Str;
            }
            else
            {
                if (d < 0)
                {
                    d = -d;
                    F = "负";
                }
                else if (d == 0)
                {
                    F = "零元整";
                }
                else
                {
                    F = "";
                }
                int L = (int)(Math.Floor(Math.Log(d + 0.0000001, 10)));
                string Str = "";
                bool Zero = false;  //上一位是不是0
                for (int i = L; i >= -2; i--)
                {
                    Int64 T = (Int64)(Math.Round(d, 2) / Math.Pow(10, i) + 0.000001);
                    int N = (int)(T % 10);
                    if (N == 0)
                    {
                        if (i == 0 || i == 4 || i == 8) //碰到元、万、亿必须显示
                        {
                            Str = Str + WEI.Substring(i + 2, 1);
                            Zero = false;
                        }
                        else
                        {
                            Zero = true;
                        }
                    }
                    else
                    {
                        if (Zero)
                        {
                            Str = Str + NUM.Substring(0, 1);
                        }
                        Str = Str + NUM.Substring(N, 1) + WEI.Substring(i + 2, 1);
                        Zero = false;
                    }
                    if (Math.Abs(d - T * Math.Pow(10, i)) < 0.001 && i > -2 && i < 5)
                    {
                        if (i > 0)
                        {
                            Str = Str + "元整";
                        }
                        else
                        {
                            Str = Str + "整";
                        }
                        break;
                    }
                }
                Str = F + Str;
                return Str;
            }
        }

        /// <summary>【通用函数】取得金额某位的大写</summary>
        /// <param name="Value">数字</param>
        /// <param name="B">位置，-2分-1角0元1十2百3千4万，以此类推</param>
        /// <returns>返回数字，如：贰</returns>
        public static string MoneyBitUpper(double Value, int B)
        {
            const string NUM = "零壹贰叁肆伍陆柒捌玖";
            string S = "";
            double d = StrToDouble(Value.ToString("0.00"));
            if (d < 0)
            {
                S = "负";
                d = -d;
            }
            int P = (int)(d / Math.Pow(10, B) + 0.001);
            if (P == 0)
            {
                return "¤";
            }
            else
            {
                if (P >= 10) S = "";
                P = P % 10;
                return S + NUM.Substring(P, 1);
            }
        }

        /// <summary>【通用函数】对象转换成字节数组,自动判断isDbNull,返回null
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>失败返回null</returns>
        public static byte[] ObjToBytes(object obj)
        {
            if (obj == null || Convert.IsDBNull(obj))
            {
                return null;
            }

            using (MemoryStream ms = new MemoryStream())
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Serialize(ms, obj);
                return ms.GetBuffer();
            }

            //if (obj.GetType() == typeof(string))
            //{
            //    return StrToBytes(obj.ToString());
            //}
            //byte[] bytes = (byte[])obj;
            //if (bytes.Length == 0)
            //{
            //    return null;
            //}
            //else
            //{
            //    return bytes;
            //}
        }

        /// <summary>
        /// 【通用函数】字节数组转换成对象,自动判断isDbNull,返回null
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static object BytesToObj(byte[] data)
        {
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            using (MemoryStream rems = new MemoryStream(data))
            {
                rems.Position = 0;
                return formatter.Deserialize(rems);
            }
        }



        /// <summary>【通用函数】字符串转换成字节数组(采用UTF8)</summary>
        /// <param name="Str">字符串</param>
        /// <returns></returns>
        public static byte[] StrToBytes(string Str)
        {
            byte[] result = new byte[Encoding.UTF8.GetByteCount(Str)];
            result = Encoding.UTF8.GetBytes(Str);
            return result;
        }

        /// <summary>【通用函数】字节数组转换成字符串(采用UTF8)（过期函数，建议采用ObjToBytesToStr）</summary>
        /// <param name="bytes">字节数组，DataSet返回的数据可以直接使用,如:(byte[])Ds.Tables[0].Rows[0]["f_fromsql"]</param>
        /// <returns></returns>
        public static string BytesToStr(byte[] bytes)
        {
            if (bytes == null)
            {
                return "";
            }
            else if (bytes.Length == 0)
            {
                return "";
            }
            else
            {
                return System.Text.Encoding.UTF8.GetString(bytes).Replace("\0", string.Empty);
            }
        }

        /// <summary>【通用函数】对象转换成字节数组再转换成字符串(采用UTF8)</summary>
        /// <param name="Obj">对象,如:Ds.Tables[0].Rows[0]["f_image"]</param>
        /// <returns></returns>
        public static string ObjToBytesToStr(object Obj)
        {
            if (Convert.IsDBNull(Obj)) return "";
            return System.Text.Encoding.UTF8.GetString((byte[])Obj).Replace("\0", string.Empty);
        }

        /// <summary>【通用函数】将Dataset对象转换成Xml字符串(采用UTF8)</summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static string DatasetToXmlstr(DataSet ds)
        {
            MemoryStream stream = new MemoryStream();
            //格式化内存流
            stream.Flush();
            //把dataset以xml格式写入流中
            ds.WriteXml(stream, XmlWriteMode.WriteSchema);
            //再用一个字符串来保存上面的xml格式
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        /// <summary>【通用函数】将Xml字符串转换成Dataset对象(采用UTF8)</summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static DataSet XmlstrToDataset(string xml)
        {
            DataSet ds = new DataSet();
            StringReader reader = new StringReader(xml);
            XmlTextReader xmlreader = new XmlTextReader(reader);
            ds.ReadXml(xmlreader);
            return ds;
        }

        /// <summary>【通用函数】填冲c字符n次
        /// </summary>
        public static string DupeString(string aC, int aN)
        {
            string Tmp = "";
            for (int i = 1; i <= aN; i++)
            {
                Tmp = Tmp + aC;
            }
            return Tmp;
        }

        /// <summary>【通用函数】取字符串左边N个字符
        /// </summary>
        public static string LeftStr(string aStr, int aN)
        {
            string Tmp = "";
            if (System.Text.Encoding.Default.GetBytes(aStr).Length < aN)
            {
                aStr = aStr + Tmp.PadRight(aN - System.Text.Encoding.Default.GetBytes(aStr).Length, ' ');
            }
            byte[] bytStr1 = System.Text.Encoding.Default.GetBytes(aStr);
            byte[] bytStr2 = new byte[aN];
            for (int i = 0; i < aN; i++)
            {
                bytStr2[i] = bytStr1[i];
            }
            return System.Text.Encoding.Default.GetString(bytStr2, 0, bytStr2.Length).Trim();
        }

        /// <summary>【通用函数】取字符串右边N个字符
        /// </summary>
        public static string RightStr(string aStr, int aN)
        {
            string Tmp = "";
            if (System.Text.Encoding.Default.GetBytes(aStr).Length < aN)
            {
                aStr = aStr + Tmp.PadLeft(aN - System.Text.Encoding.Default.GetBytes(aStr).Length, ' ');
            }
            byte[] bytStr1 = System.Text.Encoding.Default.GetBytes(aStr);
            byte[] bytStr2 = new byte[aN];
            int j = System.Text.Encoding.Default.GetBytes(aStr).Length - 1;
            for (int i = aN - 1; i >= 0; i--)
            {
                bytStr2[i] = bytStr1[j--];
            }
            return System.Text.Encoding.Default.GetString(bytStr2, 0, bytStr2.Length).Trim();
        }

        /// <summary>【通用函数】取字符串长度(按字节计算)
        /// </summary>
        public static int GetLength(string aStr)
        {
            return System.Text.Encoding.Default.GetBytes(aStr).Length;
        }
        /// <summary>【通用函数】byte数组转16进制字符串，一个字节两个字母
        /// </summary>
        public static string BytesToByteStr(IEnumerable<byte> bs)
        {
            if (bs == null)
                return "";
            StringBuilder sbtobs = new StringBuilder();
            foreach (var a in bs)
            {
                string tss = a.ToString("x2");
                sbtobs.Append(tss);
            }
            return sbtobs.ToString();
        }

        #region 转换扩展方法
        /// <summary>
        /// 字符串数组转换为Long数组
        /// </summary>
        /// <param name="stringList">转换字符串数组</param>
        /// <returns>List{long}</returns>
        public static List<long> ToLongList(this string[] stringList)
        {
            List<long> list = new List<long>();
            foreach (var item in stringList)
            {
                Int64 iRet = 0;
                Int64.TryParse(item, out iRet);
                list.Add(iRet);
            }
            return list;
        }

        /// <summary>
        /// 字符串转int类型
        /// </summary>
        /// <param name="source">源数据</param>
        /// <returns></returns>
        public static List<long> ToLongList(this List<string> source)
        {
            long i = 0;
            List<long> list = new List<long>();
            foreach (var item in source)
            {
                long.TryParse(item, out i);
                list.Add(i);
            }

            return list;
        }
        #endregion

    }
}
