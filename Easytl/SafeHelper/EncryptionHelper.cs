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
        /// <summary>
        /// 获取或设置加密操作的加密模式
        /// </summary>
        public CipherMode Mode { get; set; } = CipherMode.CBC;

        /// <summary>
        /// 获取或设置加密操作的填充方式
        /// </summary>
        public PaddingMode Padding { get; set; } = PaddingMode.PKCS7;

        /// <summary>
        /// 获取或设置加密操作的编码模式
        /// </summary>
        public Encoding Encode { get; set; } = Encoding.Default;

        /// <summary>
        /// 获取或设置加密操作的块大小（以位为单位）。
        /// </summary>
        public int BlockSize { get; set; } = 128;

        /// <summary>
        /// RSA加密秘钥大小
        /// </summary>
        public int dwKeySize { get; set; } = 1024;

        /// <summary>
        /// 加解密有误时抛出异常
        /// </summary>
        public bool throwEx { get; set; } = true;

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
            /// 【非对称算法】RSA加密
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
        /// 输入或输出字符串类型
        /// </summary>
        public enum InOutParaType
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
        byte[] Encrypt(Encrypt_Type EncryptType, string EncryptString, string Key = "", string IV = "")
        {
            try
            {
                SymmetricAlgorithm ect = null;
                switch (EncryptType)
                {
                    case Encrypt_Type.DES_Base64:
                        ect = new DESCryptoServiceProvider();
                        if (string.IsNullOrEmpty(IV))
                            IV = Key.Substring(0, 8);
                        break;
                    case Encrypt_Type.DES3:
                        ect = new TripleDESCryptoServiceProvider();
                        if (string.IsNullOrEmpty(IV))
                            IV = Key.Substring(0, 8);
                        ect.BlockSize = BlockSize;
                        break;
                    case Encrypt_Type.AES:
                        ect = new AesCryptoServiceProvider();
                        if (string.IsNullOrEmpty(IV))
                            IV = Key.Substring(0, 16);
                        ect.BlockSize = BlockSize;
                        break;
                    case Encrypt_Type.DES_MD5:
                        if (!string.IsNullOrEmpty(Key))
                        {
                            ect = new DESCryptoServiceProvider();
                            if (string.IsNullOrEmpty(IV))
                                IV = Key;
                            byte[] inputByteArray = Encode.GetBytes(EncryptString);
                            ect.Key = Encode.GetBytes(Key);
                            ect.IV = Encode.GetBytes(IV);
                            ect.Mode = Mode;
                            ect.Padding = Padding;
                            MemoryStream ms = new MemoryStream();
                            CryptoStream cs = new CryptoStream(ms, ect.CreateEncryptor(), CryptoStreamMode.Write);
                            cs.Write(inputByteArray, 0, inputByteArray.Length);
                            cs.FlushFinalBlock();
                            return ms.ToArray();
                        }
                        else goto default;
                    case Encrypt_Type.RC2:
                        ect = new RC2CryptoServiceProvider();
                        if (string.IsNullOrEmpty(IV))
                            IV = Key.Substring(0, 8);
                        ect.BlockSize = BlockSize;
                        break;
                    case Encrypt_Type.RC4:
                        if (!string.IsNullOrEmpty(Key))
                        {
                            RC4CryptoServiceProvider rc4 = new RC4CryptoServiceProvider();
                            rc4.Encode = Encode; 
                            return rc4.Encrypt(EncryptString, Key);
                        }
                        else goto default;
                    case Encrypt_Type.RSA:
                        if (!string.IsNullOrEmpty(Key))
                        {
                            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(dwKeySize);
                            RSA.FromXmlString(Key);
                            byte[] dataToEncrypt = Encode.GetBytes(EncryptString);
                            byte[] encryptedData = RSA.Encrypt(dataToEncrypt, false);
                            return encryptedData;
                        }
                        else goto default;
                    case Encrypt_Type.MD5:
                        MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
                        return Hash(md5Hasher, EncryptString);
                    case Encrypt_Type.SHA1:
                        SHA1 sha1Hasher = new SHA1CryptoServiceProvider();
                        return Hash(sha1Hasher, EncryptString);
                    case Encrypt_Type.HMACSHA1:
                        if (!string.IsNullOrEmpty(Key))
                        {
                            HMACSHA1 hmacsha1 = new HMACSHA1(Encode.GetBytes(Key));
                            return Hash(hmacsha1, EncryptString);
                        }
                        else goto default;
                    default:
                        return null;
                }

                ect.Key = Encode.GetBytes(Key);
                ect.IV = Encode.GetBytes(IV);
                ect.Mode = Mode;
                ect.Padding = Padding;
                ICryptoTransform Encrypt = ect.CreateEncryptor();
                byte[] data = Encode.GetBytes(EncryptString);
                byte[] result = Encrypt.TransformFinalBlock(data, 0, data.Length);
                return result;
            }
            catch (Exception ex) { if (throwEx) throw ex; }
            return null;
        }

        /// <summary>
        /// 加密（输出16进制加密字符串）
        /// </summary>
        public string Encrypt(InOutParaType Encrypt_RPType, Encrypt_Type EncryptType, string EncryptString, string Key = "", string IV = "")
        {
            switch (Encrypt_RPType)
            {
                case EncryptionHelper.InOutParaType.Str16:
                    return BitConverter.ToString(Encrypt(EncryptType, EncryptString, Key, IV)).Replace("-", string.Empty).ToUpper();
                case EncryptionHelper.InOutParaType.Base64:
                    return Convert.ToBase64String(Encrypt(EncryptType, EncryptString, Key, IV));
                default:
                    return string.Empty;
            }
        }


        /// <summary>
        /// 解密
        /// </summary>
        string Decrypt(Encrypt_Type DecryptType, byte[] DecryptBytes, string Key, string IV = "")
        {
            try
            {
                SymmetricAlgorithm ect = null;
                switch (DecryptType)
                {
                    case Encrypt_Type.DES_Base64:
                        ect = new DESCryptoServiceProvider();
                        if (string.IsNullOrEmpty(IV))
                            IV = Key.Substring(0, 8);
                        break;
                    case Encrypt_Type.DES3:
                        ect = new TripleDESCryptoServiceProvider();
                        if (string.IsNullOrEmpty(IV))
                            IV = Key.Substring(0, 8);
                        ect.BlockSize = BlockSize;
                        break;
                    case Encrypt_Type.AES:
                        ect = new RijndaelManaged();
                        if (string.IsNullOrEmpty(IV))
                            IV = Key.Substring(0, 16);
                        ect.BlockSize = BlockSize;
                        break;
                    case Encrypt_Type.DES_MD5:
                        ect = new DESCryptoServiceProvider();
                        if (string.IsNullOrEmpty(IV))
                            IV = Key;
                        ect.Key = Encode.GetBytes(Key);
                        ect.IV = Encode.GetBytes(IV);
                        ect.Mode = Mode;
                        ect.Padding = Padding;
                        MemoryStream ms = new MemoryStream();
                        CryptoStream cs = new CryptoStream(ms, ect.CreateDecryptor(), CryptoStreamMode.Write);
                        cs.Write(DecryptBytes, 0, DecryptBytes.Length);
                        cs.FlushFinalBlock();
                        return Encode.GetString(ms.ToArray());
                    case Encrypt_Type.RC2:
                        ect = new RC2CryptoServiceProvider();
                        if (string.IsNullOrEmpty(IV))
                            IV = Key.Substring(0, 8);
                        ect.BlockSize = BlockSize;
                        break;
                    case Encrypt_Type.RC4:
                        RC4CryptoServiceProvider rc4 = new RC4CryptoServiceProvider();
                        rc4.Encode = Encode;
                        return rc4.Decrypt(DecryptBytes, Key);
                    case Encrypt_Type.RSA:
                        RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                        RSA.FromXmlString(Key);
                        byte[] decryptedData = RSA.Decrypt(DecryptBytes, false);
                        return Encode.GetString(decryptedData);
                    default:
                        return string.Empty;
                }

                ect.Key = Encode.GetBytes(Key);
                ect.IV = Encode.GetBytes(IV);
                ect.Mode = Mode;
                ect.Padding = Padding;
                ICryptoTransform Decrypt = ect.CreateDecryptor();
                byte[] result = Decrypt.TransformFinalBlock(DecryptBytes, 0, DecryptBytes.Length);
                return Encode.GetString(result);
            }
            catch (Exception ex) { if (throwEx) throw ex; }
            return null;
        }

        /// <summary>
        /// 解密（输入16进制解密字符串）
        /// </summary>
        public string Decrypt(InOutParaType Encrypt_RPType, Encrypt_Type DecryptType, string DecryptString, string Key, string IV = "")
        {
            switch (Encrypt_RPType)
            {
                case EncryptionHelper.InOutParaType.Str16:
                    return Decrypt(DecryptType, DecryptString.Str16_To_Bytes(), Key, IV);
                case EncryptionHelper.InOutParaType.Base64:
                    return Decrypt(DecryptType, Convert.FromBase64String(DecryptString), Key, IV);
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// 哈希算法
        /// </summary>
        byte[] Hash(HashAlgorithm Hasher, string EncryptString)
        {
            return Hasher.ComputeHash(Encode.GetBytes(EncryptString));
        }
    }
}
