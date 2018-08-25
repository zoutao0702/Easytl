using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;

namespace Easytl
{
    /// <summary>
    /// 各种格式转换类
    /// </summary>
    public static class THelper
    {
        /// <summary>
        /// 类实例化（方便重用）
        /// </summary>
        public static T R<T>(ref T RClass, object[] Params = null)
        {
            if (RClass != null)
            {
                return RClass;
            }
            else
            {
                if (Params != null)
                {
                    RClass = (T)System.Activator.CreateInstance(typeof(T), Params);
                }
                else
                {
                    RClass = System.Activator.CreateInstance<T>();
                }
                return RClass;
            }
        }

        /// <summary>
        /// 类型转换
        /// </summary>
        public static T C<T>(object ChangeValue, T DefaultValue)
        {
            if (ChangeValue != null)
            {
                try
                {
                    Type DType = typeof(T);
                    if (DType.Name.ToLower().Contains("nullable"))
                        DType = Nullable.GetUnderlyingType(DType);
                    if (ChangeValue != null)
                    {
                        if (DType.IsEnum)
                        {
                            int ChangeValueInt;
                            if (int.TryParse(ChangeValue.ToString(), out ChangeValueInt))
                            {
                                if (Enum.IsDefined(typeof(T), ChangeValueInt))
                                    return (T)Enum.Parse(DType, ChangeValueInt.ToString());
                            }
                            else
                            {
                                if (Enum.GetNames(DType).Contains(ChangeValue))
                                    return (T)Enum.Parse(DType, ChangeValue.ToString());
                            }
                        }
                        else
                        {
                            switch (DType.Name.ToLower())
                            {
                                case "guid":
                                    return (T)Convert.ChangeType(Guid.Parse(ChangeValue.ToString()), DType);
                                default:
                                    return (T)Convert.ChangeType(ChangeValue, DType);
                            }
                        }
                    }
                }
                catch
                { }
            }

            return DefaultValue;
        }

        /// <summary>
        /// 返回以分隔符隔开的字符串
        /// </summary>
        public static string ToSplitString<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector, string split)
        {
            string SplitString = string.Empty;
            List<TResult> tlist = source.Select(selector).ToList();
            foreach (var item in tlist)
            {
                if (!string.IsNullOrEmpty(SplitString))
                    SplitString += split;
                SplitString += item.ToString();
            }

            return SplitString;
        }

        /// <summary>
        /// 获取枚举值上的Description特性的说明
        /// </summary>
        /// <param name="EnumValue">枚举值</param>
        /// <returns>特性的说明</returns>
        public static string GetEnumDescription(this Enum EnumValue, bool EmptyGetValue = true)
        {
            if (EnumValue != null)
            {
                System.Reflection.FieldInfo field = EnumValue.GetType().GetField(EnumValue.ToString());
                if (field == null)
                    return string.Empty;

                Attribute DescAttr = Attribute.GetCustomAttribute(field, typeof(System.ComponentModel.DescriptionAttribute));
                if (DescAttr == null)
                {
                    if (EmptyGetValue)
                        return EnumValue.ToString();
                    else
                        return string.Empty;
                }

                return (DescAttr as System.ComponentModel.DescriptionAttribute).Description;
            }
            return null;
        }

        /// <summary>
        /// 获取自定义特性
        /// </summary>
        public static T GetCustomAttribute<T>(System.Reflection.MemberInfo member) where T : System.Attribute
        {
            object[] attributes = member.GetCustomAttributes(typeof(T), false);
            if (attributes != null)
            {
                foreach (object item in attributes)
                {
                    if (item is T)
                    {
                        return (T)item;
                    }
                }
            }
            return default(T);
        }

        /// <summary>
        /// 获取自定义属性的默认值
        /// </summary>
        /// <param name="EnumValue">自定义属性</param>
        /// <returns>特性的说明</returns>
        public static object GetCustomAttribute_FirstValue<T>(System.Reflection.MemberInfo member) where T : System.Attribute
        {
            object[] attributes = member.GetCustomAttributes(typeof(T), false);
            if (attributes != null)
            {
                if (attributes.Length > 0)
                {
                    System.Reflection.PropertyInfo[] Properties = typeof(T).GetProperties();
                    if (Properties.Length > 0)
                    {
                        return Properties[0].GetValue(attributes[0], null);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 将对象的属性和值转化为XML字符串
        /// </summary>
        /// <typeparam name="T">要转化的对象类型</typeparam>
        /// <param name="obj">要转化的对象</param>
        /// <param name="WriteNull">是否写入空值属性</param>
        /// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 System.Reflection.BindingFlags 组成。- 或 -零，以返回 null。</param>
        public static string ToXmlString<T>(this T obj, bool WriteNull = false, string Pattern = null, System.Reflection.BindingFlags bindingAttr = System.Reflection.BindingFlags.Public)
        {
            string XmlString = string.Empty;
            System.Reflection.PropertyInfo[] Properties = typeof(T).GetProperties(bindingAttr);
            if (Properties.Length > 0)
            {
                foreach (System.Reflection.PropertyInfo item in Properties)
                {
                    if (item.CanRead)
                    {
                        object item_value = item.GetValue(obj, null);
                        if (item_value != null)
                        {
                            if (Pattern != null)
                            { 
                                XmlString += "<" + item.Name + ">" + Pattern.Replace("{1}", item_value.ToString()) + "</" + item.Name + ">";
                            }
                            else
                            {
                                XmlString += "<" + item.Name + ">" + item_value.ToString() + "</" + item.Name + ">";
                            }
                        }
                        else
                        {
                            if (WriteNull)
                            {
                                XmlString += "<" + item.Name + ">" + "</" + item.Name + ">";
                            }
                        }
                    }
                }
            }
            XmlString = "<xml>" + XmlString + "</xml>";
            return XmlString;
        }

        /// <summary>
        /// Xml转化为类
        /// </summary>
        public static T XmlToObject<T>(this string XmlString, string Pattern = null, System.Reflection.BindingFlags bindingAttr = System.Reflection.BindingFlags.Public)
        {
            T TObj = System.Activator.CreateInstance<T>();
            System.Reflection.PropertyInfo[] Properties = typeof(T).GetProperties(bindingAttr);
            if (Properties.Length > 0)
            {
                foreach (System.Reflection.PropertyInfo item in Properties)
                {
                    if (item.CanWrite)
                    {
                        System.Text.RegularExpressions.GroupCollection Groups = System.Text.RegularExpressions.Regex.Match(XmlString, "<" + item.Name + ">(.*)</" + item.Name + ">").Groups;
                        if (Groups.Count > 1)
                        {
                            if (Pattern != null)
                            {
                                string GroupsValue = Groups[1].Value;
                                Groups = System.Text.RegularExpressions.Regex.Match(GroupsValue, Pattern.Replace("[", @"\[").Replace("]", @"\]").Replace("(", @"\(").Replace(")", @"\)").Replace("{1}", "(.*)")).Groups;
                                if (Groups.Count > 1)
                                {
                                    item.SetValue(TObj, Convert.ChangeType(Groups[1].Value, item.PropertyType), null);
                                }
                                else
                                {
                                    item.SetValue(TObj, Convert.ChangeType(GroupsValue, item.PropertyType), null);
                                }
                            }
                            else
                            {
                                item.SetValue(TObj, Convert.ChangeType(Groups[1].Value, item.PropertyType), null);
                            }
                        }
                    }
                }
            }
            return TObj;
        }

        /// <summary>
        /// 将一个数据表转换成一个JSON字符串，在客户端可以直接转换成二维数组。
        /// </summary>
        /// <param name="source">需要转换的表。</param>
        /// <returns>Json字符</returns>
        /// <author>william</author>
        /// <createtime>2011-7-1</createtime>
        /// <remarks></remarks>      
        public static string ToJsonString(this DataTable source, bool ContentColumnName)
        {
            if ((source == null) || (source.Rows.Count == 0))
            {
                return string.Empty;
            }
            StringBuilder sbr = new StringBuilder("{\"TotalCount\":\"" + source.Rows.Count + "\",\"Content\":[");
            foreach (DataRow row in source.Rows)
            {
                if (ContentColumnName)
                {
                    sbr.Append("{");
                    for (int i = 0; i < source.Columns.Count; i++)
                    {
                        sbr.Append("\"" + source.Columns[i].ColumnName + "\":\"" + row[i].ToString() + "\",");
                    }
                    sbr.Remove(sbr.Length - 1, 1);
                    sbr.Append("},");
                }
                else
                {
                    sbr.Append("[");
                    for (int i = 0; i < source.Columns.Count; i++)
                    {
                        sbr.Append('"' + row[i].ToString() + "\",");
                    }
                    sbr.Remove(sbr.Length - 1, 1);
                    sbr.Append("],");
                }
            }
            sbr.Remove(sbr.Length - 1, 1);
            sbr.Append("]}");
            return sbr.ToString();
        }

        ///<summary>
        ///生成随机字符串 
        ///</summary>
        ///<param name="length">目标字符串的长度</param>
        ///<param name="useNum">是否包含数字，1=包含，默认为包含</param>
        ///<param name="useLow">是否包含小写字母，1=包含，默认为包含</param>
        ///<param name="useUpp">是否包含大写字母，1=包含，默认为包含</param>
        ///<param name="useSpe">是否包含特殊字符，1=包含，默认为不包含</param>
        ///<param name="custom">要包含的自定义字符，直接输入要包含的字符列表</param>
        ///<returns>指定长度的随机字符串</returns>
        public static string GetRandomString(int length, bool useNum = true, bool useLow = true, bool useUpp = true, bool useSpe = false, string custom = "")
        {
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            Random r = new Random(BitConverter.ToInt32(b, 0));
            string s = null, str = custom;
            if (useNum == true) { str += "0123456789"; }
            if (useLow == true) { str += "abcdefghijklmnopqrstuvwxyz"; }
            if (useUpp == true) { str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; }
            if (useSpe == true) { str += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~"; }
            for (int i = 0; i < length; i++)
            {
                s += str.Substring(r.Next(0, str.Length - 1), 1);
            }
            return s;
        }

        /// <summary>
        /// 检查是否有属性为空
        /// </summary>
        public static bool CheckEmpty<T>(T ObjT, List<string> ExcludeProName = null)
        {
            System.Reflection.PropertyInfo[] properties = typeof(T).GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                System.Reflection.PropertyInfo propertyInfo = properties[i];
                bool CheckE = true;
                if (ExcludeProName != null)
                {
                    if (ExcludeProName.Contains(propertyInfo.Name))
                    {
                        CheckE = false;
                    }
                }
                if (CheckE)
                {
                    object value = propertyInfo.GetValue(ObjT, null);
                    if (value == null)
                    {
                        return true;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(value.ToString().Trim()))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        ///// <summary>
        ///// 获取本地IPv4地址
        ///// </summary>
        ///// <returns></returns>
        //public static string GetHostIPv4()
        //{
        //    System.Net.IPAddress[] IPs = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName());
        //    foreach (var item in IPs)
        //    {
        //        if (System.Text.RegularExpressions.Regex.IsMatch(item.ToString(), "^(1\\d{2}|2[0-4]\\d|25[0-5]|[1-9]\\d|[1-9])\\."
        //                                                                        + "(1\\d{2}|2[0-4]\\d|25[0-5]|[1-9]\\d|\\d)\\."
        //                                                                        + "(1\\d{2}|2[0-4]\\d|25[0-5]|[1-9]\\d|\\d)\\."
        //                                                                        + "(1\\d{2}|2[0-4]\\d|25[0-5]|[1-9]\\d|\\d)$"))
        //        {
        //            return item.ToString();
        //        }
        //    }

        //    return string.Empty;
        //}

        /// <summary>
        /// 向用户显示控件
        /// </summary>
        public static void Show(this System.Windows.Forms.Form MyForm, System.Windows.Forms.IWin32Window Owner = null)
        {
            if (MyForm.Visible)
                MyForm.Focus();
            else
            {
                if (Owner == null)
                    MyForm.Show();
                else
                    MyForm.Show(Owner);
            }
        }

        /// <summary>
        /// 日期转化为(16进制)2字节字符串（默认支持最大日期到2063年）
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="StartYear">计算开始日期</param>
        /// <returns></returns>
        public static string DateToB2Str16(this DateTime date, int StartYear = 2000)
        {
            string YearT2 = Convert.ToString((date.Year - StartYear), 2);
            YearT2 = Str_Add0_Before(YearT2, 6);
            string MonthT2 = Convert.ToString(date.Month, 2);
            MonthT2 = Str_Add0_Before(MonthT2, 4);
            string DayT2 = Convert.ToString(date.Day, 2);
            DayT2 = Str_Add0_Before(DayT2, 5);
            string dateT2 = "0" + YearT2 + MonthT2 + DayT2;
            string date1T16 = Str_Add0_Before(Convert.ToString(Convert.ToByte(dateT2.Substring(0, 8), 2), 16), 2);
            string date2T16 = Str_Add0_Before(Convert.ToString(Convert.ToByte(dateT2.Substring(8, 8), 2), 16), 2);
            return date1T16 + date2T16;
        }

        /// <summary>
        /// 日期和时间转化为(16进制)4字节字符串（默认支持最大日期到2063年）
        /// </summary>
        /// <param name="date">日期和时间</param>
        /// <param name="StartYear">计算开始日期</param>
        /// <returns></returns>
        public static string DateToB4Str16(this DateTime date, int StartYear = 2000)
        {
            string YearT2 = Convert.ToString((date.Year - StartYear), 2);
            YearT2 = Str_Add0_Before(YearT2, 6);
            string MonthT2 = Convert.ToString(date.Month, 2);
            MonthT2 = Str_Add0_Before(MonthT2, 4);
            string DayT2 = Convert.ToString(date.Day, 2);
            DayT2 = Str_Add0_Before(DayT2, 5);
            string HourT2 = Convert.ToString(date.Hour, 2);
            HourT2 = Str_Add0_Before(HourT2, 5);
            string MinuteT2 = Convert.ToString(date.Minute, 2);
            MinuteT2 = Str_Add0_Before(MinuteT2, 6);
            string SecondT2 = Convert.ToString(date.Second, 2);
            SecondT2 = Str_Add0_Before(SecondT2, 6);
            string dateT2 = YearT2 + MonthT2 + DayT2 + HourT2 + MinuteT2 + SecondT2;
            string date1T16 = Str_Add0_Before(Convert.ToString(Convert.ToByte(dateT2.Substring(0, 8), 2), 16), 2);
            string date2T16 = Str_Add0_Before(Convert.ToString(Convert.ToByte(dateT2.Substring(8, 8), 2), 16), 2);
            string date3T16 = Str_Add0_Before(Convert.ToString(Convert.ToByte(dateT2.Substring(16, 8), 2), 16), 2);
            string date4T16 = Str_Add0_Before(Convert.ToString(Convert.ToByte(dateT2.Substring(24, 8), 2), 16), 2);
            return date1T16 + date2T16 + date3T16 + date4T16;
        }

        /// <summary>
        /// (16进制)2字节字符串转化为日期
        /// </summary>
        /// <param name="Bs">16进制2字节字符串</param>
        /// <param name="startpoint">开始截取位置</param>
        /// <returns></returns>
        public static DateTime? ByteToDate(this string Bs, int startpoint)
        {
            string B1 = Convert.ToString(Convert.ToByte(Bs.Substring(startpoint, 2), 16), 2);
            int B1Length = 8 - B1.Length;
            for (int i = 0; i < B1Length; i++)
            {
                B1 = "0" + B1;
            }
            string B2 = Convert.ToString(Convert.ToByte(Bs.Substring(startpoint + 2, 2), 16), 2);
            int B2Length = 8 - B2.Length;
            for (int i = 0; i < B2Length; i++)
            {
                B2 = "0" + B2;
            }
            string BStr = B1 + B2;
            int Year = Convert.ToInt32(BStr.Substring(1, 6), 2);
            int Month = Convert.ToInt32(BStr.Substring(7, 4), 2);
            int Day = Convert.ToInt32(BStr.Substring(11, 5), 2);
            try
            {
                return Convert.ToDateTime("20" + Str_Add0_Before(Year.ToString(), 2) + "-" + Str_Add0_Before(Month.ToString(), 2) + "-" + Str_Add0_Before(Day.ToString(), 2));
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// (16进制)4字节字符串转化为时间
        /// </summary>
        /// <param name="Bs">16进制4字节字符串</param>
        /// <param name="startpoint">开始截取位置</param>
        /// <returns></returns>
        public static DateTime? Byte4ToDate(this string Bs, int startpoint)
        {
            string B1 = Convert.ToString(Convert.ToByte(Bs.Substring(startpoint + 0, 2), 16), 2);
            int B1Length = 8 - B1.Length;
            for (int i = 0; i < B1Length; i++)
            {
                B1 = "0" + B1;
            }
            string B2 = Convert.ToString(Convert.ToByte(Bs.Substring(startpoint + 2, 2), 16), 2);
            int B2Length = 8 - B2.Length;
            for (int i = 0; i < B2Length; i++)
            {
                B2 = "0" + B2;
            }
            string B3 = Convert.ToString(Convert.ToByte(Bs.Substring(startpoint + 4, 2), 16), 2);
            int B3Length = 8 - B3.Length;
            for (int i = 0; i < B3Length; i++)
            {
                B3 = "0" + B3;
            }
            string B4 = Convert.ToString(Convert.ToByte(Bs.Substring(startpoint + 6, 2), 16), 2);
            int B4Length = 8 - B4.Length;
            for (int i = 0; i < B4Length; i++)
            {
                B4 = "0" + B4;
            }
            string BStr = B1 + B2 + B3 + B4;
            int Year = Convert.ToInt32(BStr.Substring(0, 6), 2);
            int Month = Convert.ToInt32(BStr.Substring(6, 4), 2);
            int Day = Convert.ToInt32(BStr.Substring(10, 5), 2);
            int Hour = Convert.ToInt32(BStr.Substring(15, 5), 2);
            int Minite = Convert.ToInt32(BStr.Substring(20, 6), 2);
            int Second = Convert.ToInt32(BStr.Substring(26, 6), 2);
            try
            {
                return Convert.ToDateTime("20" + Str_Add0_Before(Year.ToString(), 2) + "-" + Str_Add0_Before(Month.ToString(), 2) + "-" + Str_Add0_Before(Day.ToString(), 2) + " " + Str_Add0_Before(Hour.ToString(), 2) + ":" + Str_Add0_Before(Minite.ToString(), 2) + ":" + Str_Add0_Before(Second.ToString(), 2));
            }
            catch
            {
                return null; ;
            }
        }

        /// <summary>
        /// 字符串前补0
        /// </summary>
        /// <param name="b"></param>
        /// <param name="blength"></param>
        /// <returns></returns>
        public static string Str_Add0_Before(this string b, int blength)
        {
            blength = blength - b.Length;
            for (int i = 0; i < blength; i++)
            {
                b = "0" + b;
            }
            return b;
        }

        /// <summary>
        /// 字符串后补0
        /// </summary>
        /// <param name="b"></param>
        /// <param name="blength"></param>
        /// <returns></returns>
        public static string Str_Add0_After(this string b, int blength)
        {
            blength = blength - b.Length;
            for (int i = 0; i < blength; i++)
            {
                b = b + "0";
            }
            return b;
        }

        /// <summary>
        /// 字符串前补字符
        /// </summary>
        /// <param name="b"></param>
        /// <param name="blength"></param>
        /// <returns></returns>
        public static string Str_AddChar_Before(this string b, int blength, string CharF)
        {
            blength = blength - b.Length;
            for (int i = 0; i < blength; i++)
            {
                b = CharF + b;
            }
            return b;
        }

        /// <summary>
        /// 字符串后补字符
        /// </summary>
        /// <param name="b"></param>
        /// <param name="blength"></param>
        /// <returns></returns>
        public static string Str_AddChar_After(this string b, int blength, string CharF)
        {
            blength = blength - b.Length;
            for (int i = 0; i < blength; i++)
            {
                b = b + CharF;
            }
            return b;
        }

        /// <summary>
        /// 获取两段字符中间的字符串
        /// </summary>
        /// <param name="b">字符串</param>
        /// <param name="ContentStart">开始字符</param>
        /// <param name="ContentEnd">截止字符</param>
        /// <returns></returns>
        public static string Str_GetContent(this string b, string ContentStart, string ContentEnd)
        {
            int Start = b.IndexOf(ContentStart) + ContentStart.Length;
            int End = b.IndexOf(ContentEnd, Start);
            if ((Start >= 0) && (End > Start))
            {
                return b.Substring(Start, End - Start);
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 16进制字符串转换为2进制字符串
        /// </summary>
        /// <param name="Str16">16进制字符串</param>
        public static string Str16_To_Str2(this string Str16)
        {
            string Str2 = string.Empty;
            if (Str16.Length % 2 != 0)
            {
                return Str2;
            }
            for (int i = 0; i < Str16.Length / 2; i++)
            {
                Str2 += Str_Add0_Before(Convert.ToString(Convert.ToByte(Str16.Substring(i * 2, 2), 16), 2), 8);
            }
            return Str2;
        }

        /// <summary>
        /// 16进制字符串转换为10进制字节数组
        /// </summary>
        /// <param name="Str16">16进制字符串</param>
        public static byte[] Str16_To_Bytes(this string Str16)
        {
            if (Str16.Length % 2 != 0)
            {
                return null;
            }
            byte[] Bytes = new byte[Str16.Length / 2];
            for (int i = 0; i < Str16.Length / 2; i++)
            {
                Bytes[i] = Convert.ToByte(Str16.Substring(i * 2, 2), 16);
            }
            return Bytes;
        }

        /// <summary>
        /// 2进制字符串转换为16进制字符串
        /// </summary>
        /// <param name="Str2">2进制字符串</param>
        public static string Str2_To_Str16(this string Str2)
        {
            string Str16 = string.Empty;
            if (Str2.Length % 8 != 0)
            {
                return Str16;
            }
            for (int i = 0; i < Str2.Length / 8; i++)
            {
                Str16 += Str_Add0_Before(Convert.ToString(Convert.ToByte(Str2.Substring(i * 8, 8), 2), 16), 2);
            }
            return Str16;
        }

        /// <summary>
        /// 获取字节数
        /// </summary>
        public static int GetByteLength(this byte b)
        {
            return 1;
        }

        /// <summary>
        /// 获取字节数
        /// </summary>
        public static int GetByteLength(this char b)
        {
            return 1;
        }

        /// <summary>
        /// 获取字节数
        /// </summary>
        public static int GetByteLength(this int b)
        {
            return 4;
        }

        /// <summary>
        /// 反转字符串
        /// </summary>
        /// <param name="Str">要反转的字符串</param>
        /// <param name="ReverseNum">反转时的字符串个数</param>
        public static string StrReverse(this string Str, int ReverseNum)
        {
            string StrRev = string.Empty;
            if (Str.Length % ReverseNum != 0)
            {
                return StrRev;
            }
            int StrCharCount = Str.Length / ReverseNum;
            for (int i = StrCharCount - 1; i >= 0; i--)
            {
                StrRev += Str.Substring(i * ReverseNum, ReverseNum);
            }
            return StrRev;
        }

        /// <summary>
        /// 日期转换
        /// </summary>
        /// <param name="week"></param>
        /// <returns></returns>
        public static string WeekEnToCh(this DayOfWeek week)
        {
            string WeekCh = string.Empty;
            switch (week)
            {
                case DayOfWeek.Monday:
                    WeekCh = "一";
                    break;
                case DayOfWeek.Tuesday:
                    WeekCh = "二";
                    break;
                case DayOfWeek.Wednesday:
                    WeekCh = "三";
                    break;
                case DayOfWeek.Thursday:
                    WeekCh = "四";
                    break;
                case DayOfWeek.Friday:
                    WeekCh = "五";
                    break;
                case DayOfWeek.Saturday:
                    WeekCh = "六";
                    break;
                case DayOfWeek.Sunday:
                    WeekCh = "日";
                    break;
            }
            return WeekCh;
        }

        /// <summary>
        /// 将类的相同属性值赋予其他类
        /// </summary>
        public static RT GetModel<T, RT>(T model)
        {
            if (model != null)
            {
                RT json_model = Activator.CreateInstance<RT>();

                Type TTtype = typeof(RT);
                System.Reflection.PropertyInfo[] PropertyInfoList = TTtype.GetProperties();
                foreach (System.Reflection.PropertyInfo PropertyInfoI in PropertyInfoList)
                {
                    if (PropertyInfoI.CanWrite)
                    {
                        System.Reflection.PropertyInfo m_Property = typeof(T).GetProperty(PropertyInfoI.Name);
                        if (m_Property != null)
                        {
                            PropertyInfoI.SetValue(json_model, m_Property.GetValue(model, null), null);
                        }
                    }
                }

                return json_model;
            }
            else
                return default(RT);
        }

        /// <summary>
        /// 将类的相同属性值赋予其他类
        /// </summary>
        public static List<RT> GetList<T, RT>(List<T> Tlist)
        {
            if (Tlist != null)
            {
                List<RT> TTlist = new List<RT>();
                foreach (T model in Tlist)
                {
                    TTlist.Add(GetModel<T, RT>(model));
                }
                return TTlist;
            }
            return null;
        }

        /// <summary>
        /// 将整形转为IP地址
        /// </summary>
        public static string IntToIPAdress(int IntIP)
        {
            string IPAdress = string.Empty;
            string Str16IP = Convert.ToString(IntIP, 16).Str_Add0_Before(8);
            for (int i = (Str16IP.Length / 2) - 1; i >= 0; i--)
            {
                if (!string.IsNullOrEmpty(IPAdress))
                    IPAdress += ".";
                IPAdress += Convert.ToInt32(Str16IP.Substring(i * 2, 2), 16).ToString();
            }

            return IPAdress;
        }

        /// <summary>
        /// 将IP地址转为整形
        /// </summary>
        public static int IPAdressToInt(string IPAdress)
        {
            string Str16IP = string.Empty;
            string[] IPAdressCon = IPAdress.Split('.');
            for (int i = IPAdressCon.Length - 1; i >= 0; i--)
            {
                Str16IP += Convert.ToString(Convert.ToInt32(IPAdressCon[i]), 16).Str_Add0_Before(2);
            }

            return Convert.ToInt32(Str16IP, 16);
        }

        /// <summary>
        /// 获取本地IPv4地址
        /// </summary>
        public static string GetHostIPv4()
        {
            foreach (var item in System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName()))
            {
                if (item.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    return item.ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        public static long GetTimestamp(bool UTC = false, string StartDateTime = "1970/01/01 00:00:00")
        {
            return Convert.ToInt64((((UTC) ? DateTime.UtcNow : DateTime.Now) - Convert.ToDateTime(StartDateTime)).TotalSeconds);
        }

        /// <summary>
        /// 获取ConnectionStrings连接字符串
        /// </summary>
        public static string GetConnectionStrings(string Name)
        {
            System.Configuration.ConnectionStringSettings ConnStrSettings = System.Configuration.ConfigurationManager.ConnectionStrings[Name];
            if (ConnStrSettings != null)
                return ConnStrSettings.ConnectionString;
            return null;
        }

        /// <summary>
        /// 设置ConnectionString连接字符串
        /// </summary>
        public static void SetConnectionString(string Name, string ConnectionString)
        {
            SetConnectionString(Name, ConnectionString, System.Configuration.ConfigurationManager.ConnectionStrings[Name].ProviderName);
        }

        /// <summary>
        /// 设置ConnectionString连接字符串
        /// </summary>
        public static void SetConnectionString(string Name, string ConnectionString, string ProviderName)
        {
            System.Configuration.Configuration Config = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None);
            System.Configuration.ConnectionStringSettings ConnStrSettings = Config.ConnectionStrings.ConnectionStrings[Name];
            if (ConnStrSettings != null)
            {
                ConnStrSettings.ConnectionString = ConnectionString;
                ConnStrSettings.ProviderName = ProviderName;
            }
            else
                Config.ConnectionStrings.ConnectionStrings.Add(new System.Configuration.ConnectionStringSettings(Name, ConnectionString, ProviderName));

            Config.Save(System.Configuration.ConfigurationSaveMode.Modified);
            System.Configuration.ConfigurationManager.RefreshSection("connectionStrings");
        }

        /// <summary>
        /// 获取AppSetting配置字符串
        /// </summary>
        public static string GetAppSettingValue(string Name)
        {
            return System.Configuration.ConfigurationManager.AppSettings[Name];
        }

        /// <summary>
        /// 设置AppSetting配置字符串
        /// </summary>
        public static void SetAppSettingValue(string Name, string Value)
        {
            System.Configuration.Configuration Config = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None);
            if (Config.AppSettings.Settings[Name] != null)
                Config.AppSettings.Settings[Name].Value = Value;
            else
                Config.AppSettings.Settings.Add(new System.Configuration.KeyValueConfigurationElement(Name, Value));

            Config.Save(System.Configuration.ConfigurationSaveMode.Modified);
            System.Configuration.ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
