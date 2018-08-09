using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Easytl.SignHelper.Attribute;
using System.Reflection;
using Easytl.SafeHelper;
using System.Collections.Specialized;

namespace Easytl.SignHelper
{
    public class SignHelper
    {
        /// <summary>
        /// 获取签名
        /// </summary>
        public static string GetSign<T>(T model, BindingFlags bindingAttr, string SignKey, Encoding encrypt_encoding, EncryptionHelper.Encrypt_RPType encrypt_RPType, EncryptionHelper.Encrypt_Type encrypt_Type, string EncryptKey = "", string EncryptIV = "", bool ParaAsc = true, StringComparison StrCompar = StringComparison.Ordinal)
        {
            string stringA = string.Empty;
            if (model != null)
            {
                List<PropertyInfo> ProList = model.GetType().GetProperties(bindingAttr).ToList();
                if (ParaAsc)
                    ProList.Sort((x, y) => string.Compare(x.Name, y.Name, StrCompar));
                else
                    ProList.Sort((x, y) => -(string.Compare(x.Name, y.Name, StrCompar))); 

                for (int i = 0; i < ProList.Count; i++)
                {
                    if (THelper.GetCustomAttribute<NoSignAttribute>(ProList[i]) == null)
                    {
                        object ProIValue = ProList[i].GetValue(model, null);
                        if (ProIValue != null)
                        {
                            if (!string.IsNullOrEmpty(ProIValue.ToString().Trim()))
                            {
                                if (!string.IsNullOrEmpty(stringA))
                                {
                                    stringA += "&";
                                }
                                stringA += ProList[i].Name + "=" + ProIValue.ToString();
                            }
                        }
                    }
                }
            }

            string stringSignTemp = SignKey;
            if (!string.IsNullOrEmpty(stringA))
                stringSignTemp = stringA + "&key=" + stringSignTemp;

            return GetSign(stringSignTemp, encrypt_encoding, encrypt_RPType, encrypt_Type, EncryptKey, EncryptIV);
        }

        /// <summary>
        /// 获取签名
        /// </summary>
        public static string GetSign(NameValueCollection ParamList, string SignKey, Encoding encrypt_encoding, EncryptionHelper.Encrypt_RPType encrypt_RPType, EncryptionHelper.Encrypt_Type encrypt_Type, string EncryptKey = "", string EncryptIV = "", bool ParaAsc = true, StringComparison StrCompar = StringComparison.Ordinal)
        {
            return GetSign(GetSignBefore(ParamList, SignKey, ParaAsc, StrCompar), encrypt_encoding, encrypt_RPType, encrypt_Type, EncryptKey, EncryptIV);
        }

        /// <summary>
        /// 获取签名（加密前）
        /// </summary>
        public static string GetSignBefore(NameValueCollection ParamList, string SignKey, bool ParaAsc = true, StringComparison StrCompar = StringComparison.Ordinal)
        {
            string stringA = string.Empty;
            if (ParamList.Count > 0)
            {
                List<KeyValuePair<string, string>> ParamDic = new List<KeyValuePair<string, string>>();
                foreach (string key in ParamList.Keys)
                {
                    ParamDic.Add(new KeyValuePair<string, string>(key, ParamList[key]));
                }

                StringComparer _StringComparer = new StringComparer() { StrCompar = StrCompar };
                if (ParaAsc)
                    ParamDic = ParamDic.OrderBy(x => x.Key, _StringComparer).ToList();
                else
                    ParamDic = ParamDic.OrderByDescending(x => x.Key, _StringComparer).ToList();

                foreach (KeyValuePair<string, string> keyv in ParamDic)
                {
                    if (!string.IsNullOrEmpty(keyv.Value))
                    {
                        if (!string.IsNullOrEmpty(keyv.Value.Trim()))
                        {
                            if (!string.IsNullOrEmpty(stringA))
                            {
                                stringA += "&";
                            }
                            stringA += keyv.Key + "=" + keyv.Value;
                        }
                    }
                }
            }

            string stringSignTemp = SignKey;
            if (!string.IsNullOrEmpty(stringA))
                stringSignTemp = stringA + "&key=" + stringSignTemp;

            return stringSignTemp;
        }

        /// <summary>
        /// 获取签名
        /// </summary>
        public static string GetSign(string stringSignTemp, Encoding encrypt_encoding, EncryptionHelper.Encrypt_RPType encrypt_RPType, EncryptionHelper.Encrypt_Type encrypt_Type, string EncryptKey = "", string EncryptIV = "")
        {
            string sign = EncryptionHelper.Encrypt(encrypt_RPType, encrypt_Type, stringSignTemp, EncryptKey, EncryptIV, Encoding_Mode: encrypt_encoding).ToUpper();

            return sign;
        }
    }

    public class StringComparer : IComparer<string>
    {
        StringComparison _StrCompar = StringComparison.Ordinal;

        public StringComparison StrCompar
        {
            get { return _StrCompar; }
            set { _StrCompar = value; }
        }


        public int Compare(string x, string y)
        {
            return string.Compare(x, y, StrCompar);
        }
    }
}
