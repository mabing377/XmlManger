/*----------------------------------------------------------------
// Copyright (C) 2016 通通优品
// 版权所有。
//
// 类名：DesHelper
// 功能描述：DES帮助类
// 
// 创建标识：Ailn(张云超) 2016.06.30

// 修改标识：Ailn(张云超) 2016.06.30
// 修改描述：
// 
//
//----------------------------------------------------------------*/
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Public.Common
{
    /// <summary>
    /// DES 帮助类
    /// </summary>
    public class DesHelper
    {
        /// <summary>
        /// DES 所需的秘钥串
        /// </summary>
        private string _key = string.Empty;

        /// <summary>
        /// DES 所需的秘钥串
        /// </summary>
        public string Key
        {
            get
            {
                return _key;
            }

            set
            {
                _key = value;
            }
        }

        /// <summary>
        /// 使用key实例化一个DesHelper
        /// </summary>
        /// <param name="key">秘钥串</param>
        public DesHelper(string key)
        {
            this._key = key;
        }
        /// <summary>
        /// 对原始encryptBytes 进行DES加密
        /// </summary>
        /// <param name="encryptBytes">原始字节流</param>
        /// <returns>加密后字节流</returns>
        public byte[] Encrypt(byte[] encryptBytes)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(this._key.Substring(0, 8));
            byte[] keyIV = keyBytes;
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, provider.CreateEncryptor(keyBytes, keyIV), CryptoStreamMode.Write);
            cStream.Write(encryptBytes, 0, encryptBytes.Length);
            cStream.FlushFinalBlock();
            return mStream.ToArray();
        }
        /// <summary>
        /// 对decrypBytes 进行解密
        /// </summary>
        /// <param name="decrypBytes">加密的字节流</param>
        /// <returns>解密后的字节流</returns>
        public byte[] Decrypt(byte[] decrypBytes)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(this._key.Substring(0, 8));
            byte[] keyIV = keyBytes;
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, provider.CreateDecryptor(keyBytes, keyIV), CryptoStreamMode.Write);
            cStream.Write(decrypBytes, 0, decrypBytes.Length);
            cStream.FlushFinalBlock();
            return mStream.ToArray();
        }
    }
}
