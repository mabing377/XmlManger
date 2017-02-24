/*----------------------------------------------------------------
// Copyright (C) 2016 通通优品
// 版权所有。
//
// 类名：Util
// 功能描述：杂项
// 
// 创建标识：Ailn(张云超) 2016.07.01

// 修改标识：Ailn(张云超) 2016.07.01
// 修改描述：
// 
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Common
{
    /// <summary>
    /// 杂项类
    /// </summary>
    public class Util
    {
        /// <summary>
        /// 将字节转换为 16 进制
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string HexEncode(byte[] bytes)
        {
            string result = "";
            foreach (var i in bytes)
            {
                result += i.ToString("X").PadLeft(2,'0');
            }
            return result;
        }
        /// <summary>
        /// 生成数字验证码
        /// </summary>
        /// <param name="length">长度</param>
        /// <returns>验证码</returns>
        public static string CreateNumVCode(int length)
        {
            string vcode = string.Empty;
            Random r = new Random();
            do
            {
                vcode += r.Next(0, 10).ToString();
            } while (vcode.Length < length);
            return vcode;
        }
    }
}
