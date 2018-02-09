using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace CCF.DB
{
    public enum DbType
    {
        SQLSERVER = 0,
        MYSQL = 1
    }


    /// <summary>存储过程参数类型</summary>
    public enum ProcParType
    {
        /// <summary>
        /// 默认无参 车毅
        /// </summary>
        Default = 0,
        /// <summary>16位的有符号整数，相当于Sql中的SmallInt、Oracle中的Int16、Oledb中的SmallInt、.net中的System.Int16</summary>
        Int16 = 1,
        /// <summary>32位的有符号整数，相当于Sql中的Int、Oracle中的Int32、Oledb中的Integer、.net中的System.Int32</summary>
        Int32 = 2,
        /// <summary>64位的有符号整数，相当于Sql中的BigInt、Oracle中无此类型可用Number代替、Oledb中的BigInt、.net中的System.Int64</summary>
        Int64 = 3,
        /// <summary>单精度浮点值，相当于Sql中的Real、Oracle中的Float、Oledb中的Single、.net中的System.Single</summary>
        Single = 11,
        /// <summary>双精度浮点值，相当于Sql中的Float、Oracle中的Double、Oledb中的Double、.net中的System.Double</summary>
        Double = 12,
        /// <summary>定点精度和小数位数数值，相当于Sql中的Decimal、Oracle中的Number、Oledb中的Decimal、.net中的System.Decimal</summary>
        Decimal = 13,
        /// <summary>固定长度字符串，相当于Sql中的Char、Oracle中的Char、Oledb中的Char、.net中的System.String</summary>
        Char = 21,
        /// <summary>可变长度字符串，相当于Sql中的VarChar、Oracle中的VarChar、Oledb中的VarChar、.net中的System.String</summary>
        VarChar = 22,
        /// <summary>双字节char</summary>
        NVarchar = 23,
        /// <summary>二进制数据，相当于Sql中的Binary、Oracle中的Blob、Oledb中的Binary、.net中的System.Byte[]</summary>
        Image = 31,
        /// <summary>日期时间类型，相当于Sql中的DateTime、Oracle中的DateTime、Oledb中的DBDate、.net中的System.DateTime</summary>
        DateTime = 41
    }


    /// <summary>存储过程参数</summary>
    public class ProcedureParameter
    {
        /// <summary>参数名称</summary>
        public string Name;
        /// <summary>参数类型</summary>
        public ProcParType ParType;
        /// <summary>参数大小</summary>
        public int Size;
        /// <summary>参数方向</summary>
        public ParameterDirection Direction = ParameterDirection.Input;
        /// <summary>参数值</summary>
        public object Value;


        /// <summary>默认构造函数</summary>
        public ProcedureParameter()
        {

        }

        /// <summary>image型构造函数</summary>
        /// <param name="AName"></param>
        /// <param name="AImage"></param>
        public ProcedureParameter(string AName, byte[] AImage)
        {
            Name = AName;
            ParType = ProcParType.Image;
            Size = AImage.Length;
            Direction = ParameterDirection.Input;
            Value = AImage;
        }

        /// <summary>构造函数</summary>
        /// <param name="_name">参数名称</param>
        /// <param name="_partype">参数类型</param>
        /// <param name="_size">参数大小</param>
        /// <param name="_direction">参数方向</param>
        /// <param name="_value">参数值</param>
        public ProcedureParameter(string _name, ProcParType _partype, int _size, ParameterDirection _direction, object _value)
        {
            Name = _name;
            ParType = _partype;
            Size = _size;
            Direction = _direction;
            Value = _value;
        }

        /// <summary>构造函数</summary>
        /// <param name="_name">参数名称</param>
        /// <param name="_partype">参数类型</param>
        /// <param name="_size">参数大小</param>
        /// <param name="_direction">参数方向</param>
        /// <param name="_value">参数值</param>
        public ProcedureParameter(string _name, ProcParType _partype, int _size, object _value)
        {
            Name = _name;
            ParType = _partype;
            Size = _size;
            Direction = ParameterDirection.Input;
            Value = _value;
        }

        /// <summary>构造函数 车毅修改支持无类型参数</summary>
        /// <param name="_name">参数名称</param>
        /// <param name="_partype">参数类型</param>
        /// <param name="_size">参数大小</param>
        /// <param name="_direction">参数方向</param>
        /// <param name="_value">参数值</param>
        public ProcedureParameter(string _name, object _value)
        {
            Name = _name;
            ParType = ProcParType.Default;
            Value = _value;
        }
    }
}
