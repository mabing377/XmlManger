/*----------------------------------------------------------------
// Copyright (C) 2016 通通优品
// 版权所有。
//
// 类名：SHA512Helper
// 功能描述：SHA512帮助类
// 
// 创建标识：Ailn(张云超) 2016.06.30

// 修改标识：Ailn(张云超) 2016.06.30
// 修改描述：
// 
//
//----------------------------------------------------------------*/

using System.Security.Cryptography;

namespace Web.Common
{
    /// <summary>
    /// SHA512帮助类
    /// </summary>
    public class SHA512Helper
    {
        /// <summary>
        /// 获取SHA512摘要
        /// </summary>
        /// <param name="encodeBytes">原始字节流</param>
        /// <returns>摘要字节流</returns>
        public static byte[] SHA512Encode(byte[] encodeBytes)
        {
            SHA512CryptoServiceProvider sha512 = new SHA512CryptoServiceProvider();
            return sha512.ComputeHash(encodeBytes);
        }
    }
}
