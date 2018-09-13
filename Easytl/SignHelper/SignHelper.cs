using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Easytl.SignHelper.Attribute;
using System.Reflection;
using System.Collections.Specialized;
using System.Security.Cryptography;

namespace Easytl.SignHelper
{
    /// <summary>
    /// 签名帮助类
    /// </summary>
    public class SignHelper
    {
        /// <summary>
        /// 获取签名前拼接字符串
        /// </summary>
        public string GetStringSignTemp<T>(T model, BindingFlags bindingAttr, string SignKey, bool ParaAsc = true, StringComparison StrCompar = StringComparison.Ordinal)
        {
            StringBuilder stringSignTemp = new StringBuilder();
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
                        KeyValueJoin(ref stringSignTemp, ProList[i].Name, ProIValue);
                    }
                }
            }

            if (!string.IsNullOrEmpty(SignKey))
                stringSignTemp.Append(SignKey);

            return stringSignTemp.ToString();
        }

        /// <summary>
        /// 获取签名前拼接字符串
        /// </summary>
        public string GetStringSignTemp(NameValueCollection ParamList, string SignKey, bool ParaAsc = true, StringComparison StrCompar = StringComparison.Ordinal)
        {
            StringBuilder stringSignTemp = new StringBuilder();
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
                    KeyValueJoin(ref stringSignTemp, keyv.Key, keyv.Value);
                }
            }

            if (!string.IsNullOrEmpty(SignKey))
                stringSignTemp.Append(SignKey);

            return stringSignTemp.ToString();
        }

        /// <summary>
        /// 签名参数键值对的拼接方式
        /// </summary>
        protected virtual void KeyValueJoin(ref StringBuilder StringSignTemp, string key, object value)
        {
            if (value != null)
            {
                if (!string.IsNullOrEmpty(value.ToString().Trim()))
                {
                    if (StringSignTemp.Length > 0)
                        StringSignTemp.Append("&");

                    StringSignTemp.Append(key + "=" + value);
                }
            }
        }

        /// <summary>
        /// RSA签名的哈希算法
        /// </summary>
        public enum RSASignHashAlgorithmType
        {
            MD5 =0,
            SHA1 = 1,
            SHA256 = 2
        }

        /// <summary>
        /// RSA签名
        /// </summary>
        /// <param name="InOutParaType">输出的字符串类型</param>
        /// <param name="PrivateKey">私钥</param>
        /// <param name="HashbyteSignStr">待签名Hash字符串值</param>
        /// <param name="HashAlgorithmType">签名算法</param>
        /// <param name="Encode">字符串编码</param>
        public string CreateRSASign(SafeHelper.EncryptionHelper.InOutParaType InOutParaType, string HashbyteSignStr, string PrivateKey, RSASignHashAlgorithmType HashAlgorithmType, Encoding Encode)
        {
            try
            {
                byte[] Buffer = Encode.GetBytes(HashbyteSignStr);
                byte[] HashbyteSign;
                switch (HashAlgorithmType)
                {
                    case RSASignHashAlgorithmType.SHA1:
                        SHA1Managed sha1 = new SHA1Managed();
                        HashbyteSign = sha1.ComputeHash(Buffer);
                        sha1.Dispose();
                        break;
                    case RSASignHashAlgorithmType.SHA256:
                        SHA256Managed sha2 = new SHA256Managed();
                        HashbyteSign = sha2.ComputeHash(Buffer);
                        sha2.Dispose();
                        break;
                    default:
                        HashAlgorithm MD5 = HashAlgorithm.Create("MD5");
                        HashbyteSign = MD5.ComputeHash(Buffer);
                        MD5.Dispose();
                        break;
                }

                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();

                RSA.FromXmlString(PrivateKey);
                RSAPKCS1SignatureFormatter RSAFormatter = new RSAPKCS1SignatureFormatter(RSA);
                //设置签名的算法
                RSAFormatter.SetHashAlgorithm(HashAlgorithmType.ToString());
                //执行签名
                byte[] RSASignbyte = RSAFormatter.CreateSignature(HashbyteSign);

                RSA.Dispose();

                switch (InOutParaType)
                {
                    case SafeHelper.EncryptionHelper.InOutParaType.Str16:
                        return BitConverter.ToString(RSASignbyte).Replace("-", string.Empty).ToUpper();
                    case SafeHelper.EncryptionHelper.InOutParaType.Base64:
                        return Convert.ToBase64String(RSASignbyte);
                    default:
                        return string.Empty;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    /// <summary>
    /// 一个用于比较键的 System.Collections.Generic.IComparer`1。
    /// </summary>
    public class StringComparer : IComparer<string>
    {
        /// <summary>
        /// 
        /// </summary>
        public StringComparison StrCompar { get; set; } = StringComparison.Ordinal;

        /// <summary>
        /// 
        /// </summary>
        public int Compare(string x, string y)
        {
            return string.Compare(x, y, StrCompar);
        }
    }
}
