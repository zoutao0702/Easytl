using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Globalization;
using System.IO;

namespace Easytl.SafeHelper
{
    /// <summary>
    /// 各种加密方法类
    /// </summary>
    public class EncryptionHelper
    {
        //CipherMode _Encrypt_Mode = CipherMode.CBC;
        ///// <summary>
        ///// 加密模式
        ///// </summary>
        //public CipherMode Encrypt_Mode
        //{
        //    get { return _Encrypt_Mode; }
        //    set { _Encrypt_Mode = value; }
        //}

        //PaddingMode _Encrypt_Pad = PaddingMode.PKCS7;
        ///// <summary>
        ///// 填充方式
        ///// </summary>
        //public PaddingMode Encrypt_Pad
        //{
        //    get { return _Encrypt_Pad; }
        //    set { _Encrypt_Pad = value; }
        //}

        //Encoding _Encoding_Mode = Encoding.Default;
        ///// <summary>
        ///// 编码模式
        ///// </summary>
        //public Encoding Encoding_Mode
        //{
        //    get { return _Encoding_Mode; }
        //    set { _Encoding_Mode = value; }
        //}

        /// <summary>
        /// 加密类型
        /// </summary>
        public enum Encrypt_Type
        {
            /// <summary>
            /// 【对称算法】DES加密，密钥长度64位
            /// </summary>
            DES_Base64,
            /// <summary>
            /// 【对称算法】DES加密，密钥长度64位
            /// </summary>
            DES_MD5,
            /// <summary>
            /// 【对称算法】3DES加密，密钥长度128、192位
            /// </summary>
            DES3,
            /// <summary>
            /// 【对称算法】AES加密，密钥长度128、192、256位
            /// </summary>
            AES,
            /// <summary>
            /// 【对称算法】RC2加密，密钥长度64、128位
            /// </summary>
            RC2,
            /// <summary>
            /// 【对称算法】RC4加密，密钥长度可变
            /// </summary>
            RC4,
            /// <summary>
            /// 【非对称算法】RAS加密
            /// </summary>
            RSA,
            /// <summary>
            /// 【散列算法】MD5加密(输出为16进制字符串)
            /// </summary>
            MD5,
            /// <summary>
            /// 【散列算法】SHA1加密(输出为16进制字符串)
            /// </summary>
            SHA1,
            /// <summary>
            /// 【散列算法】HMAC-SHA1加密(输出为16进制字符串)
            /// </summary>
            HMACSHA1
        }

        /// <summary>
        /// 加密结果或解密参数类型
        /// </summary>
        public enum Encrypt_RPType
        { 
            /// <summary>
            /// 输入或输出16进制加密字符串
            /// </summary>
            Str16 = 1,
            /// <summary>
            /// 输入或输出Base64加密字符串
            /// </summary>
            Base64 = 2
        }


        /// <summary>
        /// 加密
        /// </summary>
        public static byte[] Encrypt(Encrypt_Type EncryptType, string EncryptString, string EncryptKey = "", string EncryptIV = "", CipherMode Encrypt_Mode = CipherMode.CBC, PaddingMode Encrypt_Pad = PaddingMode.PKCS7, Encoding Encoding_Mode = null)
        {
            try
            {
                if (Encoding_Mode == null)
                    Encoding_Mode = Encoding.Default;

                SymmetricAlgorithm ect = null;
                switch (EncryptType)
                {
                    case Encrypt_Type.DES_Base64:
                        ect = new DESCryptoServiceProvider();
                        if (string.IsNullOrEmpty(EncryptIV))
                            EncryptIV = EncryptKey.Substring(0, 8);
                        break;
                    case Encrypt_Type.DES3:
                        ect = new TripleDESCryptoServiceProvider();
                        if (string.IsNullOrEmpty(EncryptIV))
                            EncryptIV = EncryptKey.Substring(0, 8);
                        break;
                    case Encrypt_Type.AES:
                        ect = new AesCryptoServiceProvider();
                        if (string.IsNullOrEmpty(EncryptIV))
                            EncryptIV = EncryptKey.Substring(0, 16);
                        ect.BlockSize = 128;
                        break;
                    case Encrypt_Type.DES_MD5:
                        if (!string.IsNullOrEmpty(EncryptKey))
                        {
                            ect = new DESCryptoServiceProvider();
                            if (string.IsNullOrEmpty(EncryptIV))
                                EncryptIV = EncryptKey;
                            byte[] inputByteArray = Encoding_Mode.GetBytes(EncryptString);
                            ect.Key = Encoding_Mode.GetBytes(EncryptKey);
                            ect.IV = Encoding_Mode.GetBytes(EncryptIV);
                            ect.Mode = Encrypt_Mode;
                            ect.Padding = Encrypt_Pad;
                            MemoryStream ms = new MemoryStream();
                            CryptoStream cs = new CryptoStream(ms, ect.CreateEncryptor(), CryptoStreamMode.Write);
                            cs.Write(inputByteArray, 0, inputByteArray.Length);
                            cs.FlushFinalBlock();
                            return ms.ToArray();
                        }
                        else goto default;
                    case Encrypt_Type.RC2:
                        ect = new RC2CryptoServiceProvider();
                        if (string.IsNullOrEmpty(EncryptIV))
                        {
                            EncryptIV = EncryptKey.Substring(0, 8);
                        }
                        break;
                    case Encrypt_Type.RC4:
                        if (!string.IsNullOrEmpty(EncryptKey))
                        {
                            RC4CryptoServiceProvider rc4 = new RC4CryptoServiceProvider();
                            rc4.Encode = Encoding_Mode;
                            return rc4.Encrypt(EncryptString, EncryptKey);
                        }
                        else goto default;
                    case Encrypt_Type.RSA:
                        if (!string.IsNullOrEmpty(EncryptKey))
                        {
                            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                            RSA.FromXmlString(EncryptKey);
                            byte[] dataToEncrypt = Encoding_Mode.GetBytes(EncryptString);
                            byte[] encryptedData = RSA.Encrypt(dataToEncrypt, true);
                            return encryptedData;
                        }
                        else goto default;
                    case Encrypt_Type.MD5:
                        MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
                        return Hash(md5Hasher, EncryptString, Encoding_Mode);
                    case Encrypt_Type.SHA1:
                        SHA1 sha1Hasher = new SHA1CryptoServiceProvider();
                        return Hash(sha1Hasher, EncryptString, Encoding_Mode);
                    case Encrypt_Type.HMACSHA1:
                        if (!string.IsNullOrEmpty(EncryptKey))
                        {
                            HMACSHA1 hmacsha1 = new HMACSHA1(Encoding_Mode.GetBytes(EncryptKey));
                            return Hash(hmacsha1, EncryptString, Encoding_Mode);
                        }
                        else goto default;
                    default:
                        return null;
                }

                ect.Key = Encoding_Mode.GetBytes(EncryptKey);
                ect.IV = Encoding_Mode.GetBytes(EncryptIV);
                ect.Mode = Encrypt_Mode;
                ect.Padding = Encrypt_Pad;
                ICryptoTransform Encrypt = ect.CreateEncryptor();
                byte[] data = Encoding_Mode.GetBytes(EncryptString);
                byte[] result = Encrypt.TransformFinalBlock(data, 0, data.Length);
                return result;
            }
            catch { }
            return null;
        }

        /// <summary>
        /// 加密（输出16进制加密字符串）
        /// </summary>
        public static string Encrypt(Encrypt_RPType Encrypt_RPType, Encrypt_Type EncryptType, string EncryptString, string EncryptKey = "", string EncryptIV = "", CipherMode Encrypt_Mode = CipherMode.CBC, PaddingMode Encrypt_Pad = PaddingMode.PKCS7, Encoding Encoding_Mode = null)
        {
            switch (Encrypt_RPType)
            {
                case EncryptionHelper.Encrypt_RPType.Str16:
                    return BitConverter.ToString(Encrypt(EncryptType, EncryptString, EncryptKey, EncryptIV, Encrypt_Mode, Encrypt_Pad, Encoding_Mode)).Replace("-", string.Empty).ToUpper();
                case EncryptionHelper.Encrypt_RPType.Base64:
                    return Convert.ToBase64String(Encrypt(EncryptType, EncryptString, EncryptKey, EncryptIV, Encrypt_Mode, Encrypt_Pad, Encoding_Mode));
                default:
                    return string.Empty;
            }
        }


        /// <summary>
        /// 解密
        /// </summary>
        public static string Decrypt(Encrypt_Type DecryptType, byte[] DecryptBytes, string DecryptKey, string DecryptIV = "", CipherMode Encrypt_Mode = CipherMode.CBC, PaddingMode Encrypt_Pad = PaddingMode.PKCS7, Encoding Encoding_Mode = null)
        {
            try
            {
                if (Encoding_Mode == null)
                    Encoding_Mode = Encoding.Default;

                SymmetricAlgorithm ect = null;
                switch (DecryptType)
                {
                    case Encrypt_Type.DES_Base64:
                        ect = new DESCryptoServiceProvider();
                        if (string.IsNullOrEmpty(DecryptIV))
                        {
                            DecryptIV = DecryptKey.Substring(0, 8);
                        }
                        break;
                    case Encrypt_Type.DES3:
                        ect = new TripleDESCryptoServiceProvider();
                        if (string.IsNullOrEmpty(DecryptIV))
                        {
                            DecryptIV = DecryptKey.Substring(0, 8);
                        }
                        break;
                    case Encrypt_Type.AES:
                        ect = new RijndaelManaged();
                        if (string.IsNullOrEmpty(DecryptIV))
                        {
                            DecryptIV = DecryptKey.Substring(0, 16);
                        }
                        ect.BlockSize = 128;
                        break;
                    case Encrypt_Type.DES_MD5:
                        ect = new DESCryptoServiceProvider();
                        if (string.IsNullOrEmpty(DecryptIV))
                        {
                            DecryptIV = DecryptKey;
                        }
                        ect.Key = Encoding_Mode.GetBytes(DecryptKey);
                        ect.IV = Encoding_Mode.GetBytes(DecryptIV);
                        ect.Mode = Encrypt_Mode;
                        ect.Padding = Encrypt_Pad;
                        MemoryStream ms = new MemoryStream();
                        CryptoStream cs = new CryptoStream(ms, ect.CreateDecryptor(), CryptoStreamMode.Write);
                        cs.Write(DecryptBytes, 0, DecryptBytes.Length);
                        cs.FlushFinalBlock();
                        return Encoding_Mode.GetString(ms.ToArray());
                    case Encrypt_Type.RC2:
                        ect = new RC2CryptoServiceProvider();
                        if (string.IsNullOrEmpty(DecryptIV))
                        {
                            DecryptIV = DecryptKey.Substring(0, 8);
                        }
                        break;
                    case Encrypt_Type.RC4:
                        RC4CryptoServiceProvider rc4 = new RC4CryptoServiceProvider();
                        rc4.Encode = Encoding_Mode;
                        return rc4.Decrypt(DecryptBytes, DecryptKey);
                    case Encrypt_Type.RSA:
                        RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                        RSA.FromXmlString(DecryptKey);
                        byte[] decryptedData = RSA.Decrypt(DecryptBytes, false);
                        return Encoding_Mode.GetString(decryptedData);
                    default:
                        return string.Empty;
                }

                ect.Key = Encoding_Mode.GetBytes(DecryptKey);
                ect.IV = Encoding_Mode.GetBytes(DecryptIV);
                ect.Mode = Encrypt_Mode;
                ect.Padding = Encrypt_Pad;
                ICryptoTransform Decrypt = ect.CreateDecryptor();
                byte[] result = Decrypt.TransformFinalBlock(DecryptBytes, 0, DecryptBytes.Length);
                return Encoding_Mode.GetString(result);
            }
            catch { }
            return null;
        }

        /// <summary>
        /// 解密（输入16进制解密字符串）
        /// </summary>
        public static string Decrypt(Encrypt_RPType Encrypt_RPType, Encrypt_Type DecryptType, string DecryptString, string DecryptKey, string DecryptIV = "", CipherMode Encrypt_Mode = CipherMode.CBC, PaddingMode Encrypt_Pad = PaddingMode.PKCS7, Encoding Encoding_Mode = null)
        {
            switch (Encrypt_RPType)
            {
                case EncryptionHelper.Encrypt_RPType.Str16:
                    return Decrypt(DecryptType, DecryptString.Str16_To_Bytes(), DecryptKey, DecryptIV, Encrypt_Mode, Encrypt_Pad, Encoding_Mode);
                case EncryptionHelper.Encrypt_RPType.Base64:
                    return Decrypt(DecryptType, Convert.FromBase64String(DecryptString), DecryptKey, DecryptIV, Encrypt_Mode, Encrypt_Pad, Encoding_Mode);
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// 哈希算法
        /// </summary>
        private static byte[] Hash(HashAlgorithm Hasher, string EncryptString, Encoding Encoding_Mode)
        {
            return Hasher.ComputeHash(Encoding_Mode.GetBytes(EncryptString));
        }
    }
}
