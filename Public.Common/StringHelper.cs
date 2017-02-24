/*----------------------------------------------------------------
// Copyright (C) 2012 通通优品
// 版权所有。
//
// 文件名：StringHelper.cs
// 功能描述：字符串扩展工具库
// 
// 创建标识：Public 2012.06.01
// 
// 修改标识：Star.Gu(古红星) 2012.09.17
// 修改描述：增加 FilterUnsafeSQL 函数
//
// 修改标识： 2012.11.08
// 修改描述：增加 Template函数，用于模板解析处理
//
// 修改标识： 2012.11.28
// 修改描述：增加 DateStringFromNow和ReadFile函数.
// DateStringFromNow：格式化时间
// ReadFile：读取文件
// 
// 修改标识：Roc.Lee(李鹏鹏) 2013.01.29
// 修改描述：增加 UrlDecode UrlEncode 。在 Html脚本处理操作 中 加入UrlDecode 先解码再处理
//
// 修改标识： 2013.02.22
// 修改描述：增加 HtmlEncode HtmlDecode 。字符类型输入数据必须经过编码或过滤：
// 如果输入位置要求纯文本，采用编码策略（编码后长度增长）；如果输入位置要求HTML，采用过滤策略。
//
//----------------------------------------------------------------*/

using System;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Microsoft.International.Converters.PinYinConverter;
using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;
using System.IO;
namespace Public.Common
{
    /// <summary>
    /// 字符串扩展工具库
    /// </summary>
    public static class StringHelper
    {

        #region 验证处理操作

        #region 检测是否存在中文字符
        /// <summary>
        /// 检测是否存在中文字符
        /// </summary>
        /// <param name="strData">输入要验证的数据(utf-8)</param>
        /// <returns>bool</returns>
        public static bool IsHasCHZN(this string input)
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                Regex RegCHZN = new Regex("[\u4e00-\u9fa5]");//[\u2E80-\uFE4F]
                Match m = RegCHZN.Match(input);
                return m.Success;
            }
            return false;
        }
        #endregion

        #region 验证正则是否通过
        /// <summary>
        /// 验证正则是否通过
        /// </summary>
        /// <param name="strData">待验证字符串</param>
        /// <param name="strReg">正则式</param>
        /// <param name="options">提供用于设置正则表达式选项的枚举值</param>
        /// <param name="isTrim">验证时是否去除首尾空格</param>
        /// <returns>bool</returns>
        public static bool IsRegexMatch(this string strData, string strReg, RegexOptions options = RegexOptions.None, bool isTrim = true)
        {
            //如果为空，认为验证失败
            if (string.IsNullOrWhiteSpace(strData))
            {
                return false;
            }
            return Regex.IsMatch(isTrim ? strData.Trim() : strData, strReg, options);
        }
        #endregion

        #region 是否为逗号分隔的数字串
        /// <summary>
        /// 是否为逗号分隔的数字串
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <returns>bool</returns>
        public static bool IsNumStringSplitByComma(this string input)
        {
            return IsRegexMatch(input, @"^\d+(,\d+)*$");
        }
        #endregion

        #region 验证IP是否正确
        /// <summary>
        /// 验证IP是否正确
        /// </summary>
        /// <param name="ip">单个IP</param>
        /// <returns>bool</returns>
        public static bool IsIP(this string ip)
        {
            System.Net.IPAddress ipads = null;
            return System.Net.IPAddress.TryParse(ip, out ipads);
            //string reg = @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$";
            //System.Net.IPAddress.TryParse(ip, out null);
            //return IsRegexMatch(ip, reg);
        }

        /// <summary>
        /// 验证IP段是否合法
        /// </summary>
        /// <param name="ip">192.168.1.*，用*号来代替某个IP段。</param>
        /// <returns>bool</returns>
        public static bool IsIPSection(this string ip)
        {
            return IsRegexMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){2}((2[0-4]\d|25[0-5]|[01]?\d\d?|\*)\.)(2[0-4]\d|25[0-5]|[01]?\d\d?|\*)$");
        }

        /// <summary>
        /// 验证IP是否在IP数组中，IP数组中的项可以为IP段形式，如：192.168.16.*
        /// </summary>
        /// <param name="ip">IP</param>
        /// <param name="ips">IP数组</param>
        /// <returns>bool</returns>
        public static bool IsInIPSection(this string ip, string[] ips)
        {
            if (string.IsNullOrWhiteSpace(ip))
            {
                return false;
            }
            string[] userip = ip.Split('.');
            for (int ipIndex = 0; ipIndex < ips.Length; ipIndex++)
            {
                string[] tmpip = ips[ipIndex].Split('.');
                int r = 0;
                for (int i = 0; i < tmpip.Length; i++)
                {
                    if (tmpip[i] == "*")
                    {
                        return true;
                    }
                    if (userip.Length > i)
                    {
                        if (tmpip[i] == userip[i])
                        {
                            r++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                if (r == 4)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region 验证Email是否正确
        /// <summary>
        /// 验证Email是否正确
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>bool</returns>
        public static bool IsEmail(this string email)
        {
            string reg = @"^([0-9a-zA-Z]+[-._+&])*[0-9a-zA-Z]+@([-0-9a-zA-Z]+[.])+[a-zA-Z]{2,6}$";
            //string reg = @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
            return IsRegexMatch(email, reg);
        }
        #endregion

        #region 验证网址是否正确
        /// <summary>
        /// 验证网址是否正确
        /// </summary>
        /// <param name="url">要验证的网址</param>
        /// <returns>bool</returns>
        public static bool IsURL(this string url)
        {
            //http https
            string reg = @"^(http|https)\://([a-zA-Z0-9\.\-]+(\:[a-zA-Z0-9\.&%\$\-]+)*@)*((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|localhost|([a-zA-Z0-9\-]+\.)*[a-zA-Z0-9\-]+\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{1,10}))(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\?\'\\\+&%\$#\=~_\-]+))*$";
            //string reg = @"[a-zA-z]+://[^\s]*";   //所有协议
            return IsRegexMatch(url, reg);
        }
        #endregion

        #region 验证邮政编码是否正确
        /// <summary>
        /// 验证邮政编码是否正确
        /// </summary>
        /// <param name="email">邮政编码</param>
        /// <returns>bool</returns>
        public static bool IsPostCode(this string postCode)
        {
            string reg = @"^[1-9]\d{5}(?!\d)$";
            return IsRegexMatch(postCode, reg);
        }
        #endregion

        #region 验证是否为实数
        /// <summary>
        /// 验证是否为实数
        /// </summary>
        /// <param name="value">验证值</param>
        /// <returns>bool</returns>
        public static bool IsFloat(this string value)
        {
            string reg = @"^-?([1-9]\d*\.\d*|0\.\d*[1-9]\d*|0?\.0+|0)$";
            return IsRegexMatch(value, reg);
        }
        #endregion

        #region 验证是否为整数
        /// <summary>
        /// 验证是否为整数
        /// </summary>
        /// <param name="value">验证值</param>
        /// <returns>bool</returns>
        public static bool IsInteger(this string value)
        {
            string reg = @"^-?[1-9]\d*$";
            return IsRegexMatch(value, reg);
        }
        #endregion

        #region 验证是否为正整数，包含零。
        /// <summary>
        /// 验证是否为正整数，包含零。
        /// </summary>
        /// <param name="value">验证值</param>
        /// <returns>bool</returns>
        public static bool IsPositiveInteger(this string value)
        {
            string reg = @"^[0-9]*$";
            return Regex.IsMatch(value, reg);
        }
        #endregion

        #region 验证是否为数字串,不包括负号和小数点
        /// <summary>
        /// 验证是否为数字串,不包括负号和小数点
        /// </summary>
        /// <param name="value">验证值</param>
        /// <returns>bool</returns>
        public static bool IsNumber(this string value)
        {
            string reg = @"^[0-9]+[0-9]*$";
            return IsRegexMatch(value, reg);
        }
        #endregion

        #endregion

        #region 将字符串转布尔型
        /// <summary>
        /// 将字符串转换为Bool类型，如果转换失败，则返回参数中设置的默认值
        /// 为True的字符串有 “true”、“1”、“yes”
        /// </summary>
        /// <param name="inputString">要转换的字符串</param>
        /// <param name="defaultValue">转换失败时，要返回的默认值</param>
        /// <returns>转换成功，则返回转换后的Bool值；转换失败，则返回参数中设置的默认值</returns>
        public static bool ConvertToBool(this string inputString, bool defaultValue = false)
        {
            if (!string.IsNullOrWhiteSpace(inputString))
            {
                if (string.Compare(inputString, "true", true) == 0)
                {
                    return true;
                }
                else if (string.Compare(inputString, "1", true) == 0)
                {
                    return true;
                }
                else if (string.Compare(inputString, "yes", true) == 0)
                {
                    return true;
                }
                else if (string.Compare(inputString, "false", true) == 0)
                {
                    return false;
                }
                else if (string.Compare(inputString, "0", true) == 0)
                {
                    return false;
                }
                else if (string.Compare(inputString, "no", true) == 0)
                {
                    return false;
                }
                else
                {
                    return defaultValue;
                }
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 将object转换为Bool类型
        /// </summary>
        /// <param name="inputString">要转换的字符串</param>
        /// <param name="defaultValue">转换失败时，要返回的默认值</param>
        /// <returns>转换成功，则返回转换后的Bool值；转换失败，则返回参数中设置的默认值</returns>
        public static bool ConvertToBool(this object inputString, bool defaultValue = false)
        {
            if (inputString != null)
            {
                return ConvertToBool(inputString.ToString(), defaultValue);
            }
            else
            {
                return defaultValue;
            }
        }
        #endregion

        #region 字符串处理

        #region 返回字符串真实长度(字节数), 1个汉字长度为2个字节
        /// <summary>
        /// 返回字符串指定编码格式的字节数
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <param name="encoding">字符串以何种编码来计算长度</param>
        /// <returns>如果字符串是 null或空，那么返回0</returns>
        public static int GetRealLength(this string input, Encoding encoding)
        {
            if (!string.IsNullOrEmpty(input))
            {
                return encoding.GetByteCount(input);
            }
            return 0;
        }

        /// <summary>
        /// GB2312编码
        /// </summary>
        private static Encoding Encoding_GB2312 = Encoding.GetEncoding("gb2312");

        /// <summary>
        /// 返回字符串GB2312编码格式的字节数, 1个汉字长度为2个字节
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <returns>如果字符串是 null或空，那么返回0</returns>
        public static int GetRealLength(this string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                return Encoding_GB2312.GetByteCount(input);
            }
            return 0;
        }

        /// <summary>
        /// 返回字符串ANSI编码下的真实长度（在简体中文系统下，ANSI 编码代表 GB2312 编码）
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <returns>如果字符串是 null或空，那么返回0</returns>
        public static int GetANSILength(this string input)
        {
            return input.GetRealLength(Encoding.Default);
        }
        #endregion

        #region  如果为空，则返回替代值
        /// <summary>
        /// 如果为空，则返回替代字符串
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <param name="insteadStr">替代字符串</param>
        /// <returns>非空源字符串或替代字符串</returns>
        public static string IsNullOrWhiteSpaceStringValue(this string input, string insteadStr)
        {
            return string.IsNullOrWhiteSpace(input) ? insteadStr : input;
        }


        /// <summary>
        /// 字符串是null时，返回替代值
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <param name="instead">替代值</param>
        /// <returns></returns>
        public static string ReturnIsNullStringValue(this string input, string instead)
        {
            return input == null ? instead : input;
        }

        #endregion

        #region 清除连续多于2个空白字符为1个
        /// <summary>
        /// <![CDATA[ 清除连续多于2个空白字符为1个（包括空格、制表符、换页符、&nbsp;（默认忽略）等等。等价于 [ \f\n\r\t\v]。）]]>
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <param name="andHtmlSpace"><![CDATA[ 是包括html空格（&nbsp;）]]></param>
        /// <returns>清除后返回的字符串</returns>
        public static string RemoveConsecutiveWhitespaceCharToOne(this string input, bool andHtmlSpace = false)
        {
            input = input != null ? Regex.Replace(input, @"(\s){2,}", "$1", RegexOptions.Singleline) : string.Empty;
            if (andHtmlSpace) return Regex.Replace(input, "(&nbsp;){2,}", "&nbsp;", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            return input;
        }
        #endregion

        #region 生成指定数量的字符串
        /// <summary>
        ///  生成指定数量的字符串
        /// </summary>
        /// <param name="str">被生成的字符串</param>
        /// <param name="number">生成的数量</param>
        /// <returns></returns>
        public static string GetSpecifiedNumberOfStrings(this string str, int number)
        {
            if (!String.IsNullOrEmpty(str))
            {
                StringBuilder strs = new StringBuilder(number * str.Length);
                for (int i = 0; i < number; i++)
                {
                    strs.Append(str);
                }
                return strs.ToString();
            }
            return string.Empty;
        }
        #endregion

        #region 统计字符串出现的次数
        /// <summary>
        /// 统计字符串出现的次数
        /// </summary>
        /// <param name="input">原字符串</param>
        /// <param name="findedStr">要查找的字符串</param>
        /// <returns>出现的次数</returns>
        public static int GetCountOfRepeatString(this string input, string findedStr)
        {
            return GetCountOfRepeatString(input, findedStr, false);
        }
        #endregion

        #region 统计字符串出现的次数（可选择支持正则查找）
        /// <summary>
        /// 统计字符串出现的次数（可选择支持正则查找）
        /// </summary>
        /// <param name="input">原字符串</param>
        /// <param name="findedStr">要查找的字符串</param>
        /// <param name="isRegex">查找串是否为正则表达式</param>
        /// <returns>出现的次数</returns>
        public static int GetCountOfRepeatString(this string input, string findedStr, bool isRegex)
        {
            if (!string.IsNullOrWhiteSpace(input) && !string.IsNullOrEmpty(findedStr))
            {
                return Regex.Matches(input, isRegex ? findedStr : Regex.Escape(findedStr)).Count;
            }
            return 0;

        }
        #endregion

        #region 报告指定字符串在此实例中的第n个匹配项的索引。
        /// <summary>
        /// 报告指定字符串在此实例中的第n个匹配项的索引。
        /// </summary>
        /// <param name="input">原字符串</param>
        /// <param name="location">位置</param>
        /// <param name="matchStr">要匹配的字符串</param>
        /// <returns>
        /// 如果找到该字符串，则为 matchStr 的从零开始的索引位置；如果未找到该字符串，则为 -1。 如果 matchStr 为 String.Empty ，则返回值为 0；如果 location 超出所在位置则返回最后一次匹配项的索引。
        ///</returns>
        public static int IndexOfMatchLocation(this string input, int location, string matchStr)
        {
            int index = input.IndexOf(matchStr, 0);

            while (location > 1)
            {
                index++;
                index = input.IndexOf(matchStr, index);
                if (index < 0) return input.LastIndexOf(matchStr);
                location--;
            }
            return index;
        }
        #endregion

        #region 报告指定 Unicode 字符在此实例中的第n个匹配项的索引。
        /// <summary>
        /// 报告指定 Unicode 字符在此实例中的第n个匹配项的索引。
        /// </summary>
        /// <param name="input">原字符串</param>
        /// <param name="location">位置</param>
        /// <param name="matchChar">要匹配的 Unicode 字符</param>
        /// <returns>
        /// 如果找到该字符，则为 matchChar 的从零开始的索引位置；如果未找到，则为 -1。如果 location 超出所在位置则返回最后一次匹配项的索引。
        ///</returns>
        public static int IndexOfMatchLocation(this string input, int location, char matchChar)
        {
            int index = input.IndexOf(matchChar, 0);

            while (location > 1)
            {
                index++;
                index = input.IndexOf(matchChar, index);
                if (index < 0) return input.LastIndexOf(matchChar);
                location--;
            }
            return index;
        }
        #endregion

        #region 截取字符串指定的长度

        #region 截取字符串指定的长度(从头截取，以字符数量计算)
        /// <summary>
        ///  截取字符串指定的长度(从头截取，以字符数量计算)
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <param name="len">字符长度</param>
        /// <param name="replaceStr">替代字符串，默认为null</param>
        /// <returns>截取又替换后的字符串</returns>
        public static string GetCutStringByCharCount(this string input, int len, string replaceStr = null)
        {
            if (len > 0 && !string.IsNullOrWhiteSpace(input) && input.Length > len)
            {
                return string.Concat(input.Remove(len), replaceStr);
            }
            return input;
        }
        #endregion

        #region 截取字符串指定的长度(以字节形式计算)
        /// <summary>
        /// 截取ANSI编码下字符串指定的长度(从头截取，以字节形式计算)
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <param name="len">截取长度,以字节形式计算</param>
        /// <param name="replaceStr">替换字符串</param>
        /// <returns></returns>
        public static string GetCutStringByANSILength(this string input, int len, string replaceStr = null)
        {
            return input.GetCutString(len, replaceStr, Encoding.Default);
        }

        /// <summary>
        /// 截取字符串指定的长度(以字节形式计算)
        /// 不同编码下字符串长度可能有差别
        /// </summary>
        /// <param name="input">字符串</param>
        /// <param name="len">截取长度,以字节形式计算</param>
        /// <param name="replaceStr">替换字符串</param>
        /// <param name="encoding">字符串的编码格式</param>
        /// <returns></returns>
        public static string GetCutString(this string input, int len, string replaceStr, Encoding encoding)
        {
            if (len > 0 && !string.IsNullOrWhiteSpace(input))
            {
                byte[] s = encoding.GetBytes(input);
                if (s.Length > len)
                {
                    string temp = string.Empty;
                    temp = encoding.GetString(s.Take(len).ToArray()).TrimEnd('?');
                    return string.Concat(temp, replaceStr);
                }
            }
            return input;
        }
        #endregion

        #endregion

        #region 按特定字符分割字符串为数组
        /// <summary>
        /// 按特定字符分割字符串为数组
        /// </summary>
        /// <param name="input">字符串</param>
        /// <param name="split">分割字符串</param>
        /// <returns>字符串数组</returns>
        public static string[] GetSplitStrings(this string input, string split)
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                if (input.IndexOf(split) < 0)
                {
                    string[] temp = { input };
                    return temp;
                }
                return Regex.Split(input, Regex.Escape(split), RegexOptions.IgnoreCase);
            }
            else
            {
                return new string[0] { };
            }
        }
        #endregion

        #region 删除指定字符串最后结尾的后的指定字符串
        /// <summary>
        /// 删除指定字符串最后结尾的后的指定字符
        /// </summary>
        /// <param name="input">字符串</param>
        /// <param name="strchar">除掉尾部的字符</param>
        /// <returns>修改后的字符串</returns>
        public static string GetDelLastString(this string input, string str)
        {
            if (input.EndsWith(str))
            {
                return input.Substring(0, input.LastIndexOf(str));
            }
            else
            {
                return input;
            }
        }
        #endregion

        #region 判断指定字符串在指定字符串数组中的位置
        /// <summary>
        /// 判断指定字符串在指定字符串数组中的位置
        /// </summary>
        /// <param name="input">字符串</param>
        /// <param name="inputArray">字符串数组</param>
        /// <param name="caseInsensetive">是否不区分大小写, true为不区分, false为区分</param>
        /// <returns>字符串在指定字符串数组中的位置, 如不存在则返回-1</returns>
        public static int GetStringInArrayLocation(this string input, string[] inputArray, bool caseInsensetive)
        {
            if (inputArray == null)
            {
                return -1;
            }
            for (int i = 0; i < inputArray.Length; i++)
            {
                if (string.Compare(input, inputArray[i], caseInsensetive) == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 判断指定字符串在指定字符串数组中的位置(不区分大小写)
        /// </summary>
        /// <param name="input">字符串</param>
        /// <param name="inputArrayArray">字符串数组</param>
        /// <returns>字符串在指定字符串数组中的位置, 如不存在则返回-1</returns>		
        public static int GetStringInArrayLocation(this string input, string[] inputArrayArray)
        {
            return GetStringInArrayLocation(input, inputArrayArray, true);
        }
        #endregion

        #region 获取内容分页
        /// <summary>
        /// 获取内容分页
        /// </summary>
        /// <remarks>
        /// 分页标记为：
        /// &lt;div id=&quot;pageline&quot;&gt;标题1&lt;/div&gt;</remarks>       
        /// <param name="text"></param>
        /// <returns>所有分页内容数组</returns>
        public static string[] GetPageContent(this string text)
        {
            IDictionary<string, int> titles = null;
            return GetPageContent(text, out titles);
        }
        /// <summary>
        /// 获取内容分页 及 标题与数组索引集合
        /// 分页标记为：
        /// [[--标题--]]
        /// </summary>
        /// <param name="text"></param>
        /// <param name="titles">标题与数组索引集合</param>
        /// <returns>所有分页内容数组</returns>
        public static string[] GetPageContent(this string text, out IDictionary<string, int> titles)
        {
            IList<string> contents = new List<string>();
            titles = new Dictionary<string, int>();
            IDictionary<string, int> existKey = new Dictionary<string, int>();
            string[] arr = Regex.Split(text, @"(\[\[--[^\]]+--\]\])", RegexOptions.Singleline);
            if (arr.Length > 1)
            {
                int index = 0;
                int page = 1;
                while (arr.Length > index)
                {
                    contents.Add(Regex.Replace(arr[index], @"(\s*<[^>]*?>\s*$)|(^\s*</[^>]*?>\s*)|(^\s*)|(\s*$)", string.Empty, RegexOptions.Singleline));
                    string key = null;
                    if (index + 1 < arr.Length)
                    {
                        key = arr[index + 1].Substring(4, arr[index + 1].Length - 8);
                    }
                    if (string.IsNullOrWhiteSpace(key))
                    {
                        key = null;
                    }
                    if (key != null)
                    {
                        if (titles.ContainsKey(key))
                        {
                            if (existKey.ContainsKey(key))
                            {
                                key = string.Concat(key, string.Format("({0})", ++existKey[key]));
                            }
                            else
                            {
                                existKey.Add(key, 1);
                                key = string.Concat(key, "(1)");
                            }

                        }
                        titles.Add(key, page);
                    }
                    index += 2;
                    page++;
                }
                if (string.IsNullOrWhiteSpace(contents[contents.Count - 1]))
                {
                    contents.RemoveAt(contents.Count - 1);
                }
            }
            else
            {
                contents.Add(text);
            }
            return contents.ToArray();

            #region <div id="pageline">标题1</div>
            //&lt;div id=&quot;pageline&quot;&gt;标题1&lt;/div&gt;
            //IList<string> contents = new List<string>();
            //titles = new Dictionary<string, int>();
            //IDictionary<string, int> existKey = new Dictionary<string, int>();
            //string[] arr = Regex.Split(text, @"(<div[^>]*?id=""pageline""[^>]*?>.*?</div>)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            //int index = 0;
            //int page = 1;
            //while (arr.Length > index)
            //{
            //    //contents.Insert(page, arr[index]);
            //    contents.Add(Regex.Replace(arr[index], @"(<\w+>\s*$)|(^</\w+>)", string.Empty, RegexOptions.Singleline));
            //    string key = null;
            //    if (index + 1 < arr.Length)
            //        key = Regex.Match(arr[index + 1], @"(?<=>).*?(?=</)", RegexOptions.IgnoreCase | RegexOptions.Singleline).Value;
            //    if (string.IsNullOrWhiteSpace(key))
            //        key = null;
            //    if (key != null)
            //    {
            //        if (titles.ContainsKey(key))
            //        {
            //            if (existKey.ContainsKey(key))
            //            {
            //                key = string.Concat(key, string.Format("({0})", ++existKey[key]));
            //            }
            //            else
            //            {
            //                existKey.Add(key, 1);
            //                key = string.Concat(key, "(1)");
            //            }

            //        }
            //        titles.Add(key, page);

            //    }
            //    index += 2;
            //    page++;
            //}
            //if (string.IsNullOrWhiteSpace(contents[contents.Count - 1]))
            //    contents.RemoveAt(contents.Count - 1);
            //return contents.ToArray();

            #endregion
        }
        #endregion

        #region 转全角的函数(SBC)
        /// <summary>
        /// 转全角的函数(SBC case)
        /// </summary>
        /// <param name="inputString">要转换的字符串</param>
        /// <returns>返回转换后的全角字符串</returns>
        ///<remarks>
        ///全角空格为12288，半角空格为32
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        ///</remarks> 
        public static string ConvertStringToSBC(this string inputString)
        {
            char[] charArray = inputString.ToCharArray();
            for (int i = 0; i < charArray.Length; i++)
            {
                if (charArray[i] == 32)
                {
                    charArray[i] = (char)12288;
                    continue;
                }
                if (charArray[i] < 127)
                {
                    charArray[i] = (char)(charArray[i] + 65248);
                }
            }
            return new string(charArray);
        }
        #endregion

        #endregion

        #region 安全处理操作
        /// <summary>
        /// 对字符串进行SHA1加密
        /// </summary>
        /// <param name="strIN">需要加密的字符串</param>
        /// <returns>密文</returns>
        public static string GetSHA1String(this string inputString)
        {
            byte[] StrRes = Encoding.Default.GetBytes(inputString);
            System.Security.Cryptography.HashAlgorithm iSHA = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            StrRes = iSHA.ComputeHash(StrRes);
            StringBuilder EnText = new StringBuilder();
            foreach (byte iByte in StrRes)
            {
                EnText.AppendFormat("{0:x2}", iByte);
            }
            return EnText.ToString();
        }

        #region 以MD5方式加密字符串
        /// <summary>
        /// 以MD5方式加密字符串
        /// </summary>
        /// <param name="inputString">要加密的字符串</param>
        /// <returns>返回加密后的字符串</returns>
        public static string GetMD5String(this string inputString)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(inputString, "MD5");
        }
        #endregion

        #region 以SHA512方式加密字符串
        /// <summary>
        /// 以SHA512方式加密字符串
        /// </summary>
        /// <param name="inputString">要加密的字符串</param>
        /// <returns>返回加密后的字符串</returns>
        public static string GetSHA512String(this string inputString)
        {
            System.Security.Cryptography.SHA512 s512 = new System.Security.Cryptography.SHA512Managed();
            byte[] result;
            result = s512.ComputeHash(Encoding.Default.GetBytes(inputString));
            s512.Clear();
            return Convert.ToBase64String(result);
        }
        #endregion

        #endregion

        #region 特殊字符过滤

        #region SQL 注入特殊关键词过滤
        /// <summary>
        /// SQL 注入特殊关键词过滤
        /// </summary>
        /// <param name="inputString">输入的字符串</param>
        /// <returns>返回处理后的字符串</returns>
        public static string FilterUnsafeSQL(string inputString)
        {
            if (string.IsNullOrWhiteSpace(inputString)) return inputString;
            inputString = inputString.Replace("&", "&amp;");
            inputString = inputString.Replace("<", "&lt;");
            inputString = inputString.Replace(">", "&gt");
            inputString = inputString.Replace("'", "''");
            inputString = inputString.Replace("*", "");
            inputString = inputString.Replace("\n", "<br/>");
            inputString = inputString.Replace("\r\n", "<br/>");
            inputString = inputString.Replace("select", "");
            inputString = inputString.Replace("insert", "");
            inputString = inputString.Replace("update", "");
            inputString = inputString.Replace("delete", "");
            inputString = inputString.Replace("create", "");
            inputString = inputString.Replace("drop", "");
            inputString = inputString.Replace("delcare", "");
            if (inputString.Trim().ToString() == "")
            {
                inputString = "";
            }
            return inputString.Trim();
        }
        #endregion

        #endregion

        #region Html脚本处理操作

        #region URL 解码
        /// <summary>
        /// URL 解码
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string UrlDecode(this string html)
        {
            if (html != null)
            {
                if (System.Web.HttpContext.Current != null)
                {
                    return System.Web.HttpContext.Current.Server.UrlDecode(html);
                }
                else
                {
                    return System.Web.HttpUtility.UrlEncodeUnicode(html);
                }
            }
            return html;
        }
        #endregion

        #region URL 编码
        /// <summary>
        /// URL 编码
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string UrlEncode(this string html)
        {
            if (html != null)
            {
                if (System.Web.HttpContext.Current != null)
                {
                    return System.Web.HttpContext.Current.Server.UrlEncode(html);
                }
                else
                {
                    return System.Web.HttpUtility.UrlEncodeUnicode(html);
                }
            }
            return html;
        }
        #endregion

        #region HTML 编码

        /// <summary>
        /// HTML编码
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string HtmlEncode(this string html)
        {
            if (!string.IsNullOrWhiteSpace(html))
            {
                return System.Web.HttpUtility.HtmlEncode(html);
            }
            return html;
        }

        #endregion

        #region HTML 解码

        /// <summary>
        /// HTML解码
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string HtmlDecode(this string html)
        {
            if (!string.IsNullOrWhiteSpace(html))
            {
                return System.Web.HttpUtility.HtmlDecode(html);
            }
            return html;
        }
        #endregion

        #region 破坏Html标签
        /// <summary>
        /// 转义左右尖括号，破坏html形成标签
        /// </summary>
        /// <param name="html">html</param>
        /// <returns></returns>
        public static string DestroyHtml(this string html)
        {
            return html != null ? html.UrlDecode().Replace("<", "&lt;").Replace(">", "&gt;") : string.Empty;
        }
        #endregion

        #region 过滤html标签
        /// <summary>
        /// 过滤html标签
        /// </summary>
        /// <param name="html">html</param>
        /// <param name="removeBlankCharToOne"><![CDATA[是否 清除连续多于2个空白字符为1个（包括空格、制表符、换页符、&nbsp;(替换为空格“ ”)等等。等价于 [ \f\n\r\t\v]。） ]]></param>
        /// <returns></returns>
        public static string FilterHtml(this string html, bool removeBlankCharToOne = true)
        {
            html = html != null ? Regex.Replace(html.UrlDecode(), "<[^<>]*?>", string.Empty, RegexOptions.Singleline) : string.Empty;
            if (removeBlankCharToOne) return html.RemoveConsecutiveWhitespaceCharToOne(true).Replace("&nbsp;", " ");
            return html;
        }
        #endregion

        #region 过滤Html中的不安全标签
        /// <summary>
        /// 过滤Html中的不安全标签
        /// </summary>
        /// <param name="html">html</param>
        /// <param name="removeBlankCharToOne">是否 清除连续多于2个空白字符为1个（包括空格、制表符、换页符等等。等价于 [ \f\n\r\t\v]。） </param>
        /// <returns></returns>
        public static string FilterUnsafeHtml(this string html, bool removeBlankCharToOne = true)
        {
            if (string.IsNullOrWhiteSpace(html)) return string.Empty;
            html = html.UrlDecode();//先把URL中的字符解码很重要
            Regex regex3 = new Regex(@"(<(style|script|object|iframe|frameset|form|meta|link).*?>.*?</\2.*?>)|(<(style|script|object|iframe|frameset|form|meta|link).*/?>)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            html = regex3.Replace(html, ""); //过滤
            regex3 = new Regex("<(style|script|object|iframe|frameset|form|meta|link)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            html = regex3.Replace(html, ""); //破坏不完整的标签

            regex3 = new Regex(@"(<[^>]*?)([^\w]+?on.+?=)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            html = regex3.Replace(html, "$1 title=");//破坏on开头的元素js事件，如:“onclick、<img data-che-pa-onerror="alert(document.cookie);/</p"”


            regex3 = new Regex(@"(<[^>]*?(?<mark1>['""]))((javascript|livescript|behavior|vbscript|jscript):.*?)(\<mark1>)", RegexOptions.IgnoreCase);
            html = regex3.Replace(html, "$1#$4");
            regex3 = new Regex(@"((<[^>]*?[^\w]+?)style=(['""""]))(\w+:expression\(.*?)(\3)", RegexOptions.IgnoreCase);
            html = regex3.Replace(html, "$2");

            if (removeBlankCharToOne) return html.RemoveConsecutiveWhitespaceCharToOne();
            return html;
        }
        #endregion

        #region 替换“\r\n”、“\n”为“<br />”
        /// <summary>
        /// 替换“\r\n”、“\n”为“《br /》”
        /// </summary>
        /// <param name="input">输入要处理的数据</param>
        /// <param name="reomveConsecutiveWhitespace">清楚连续的空白字符（包括空格、制表符、换页符等等。等价于 [ \f\n\r\t\v]。）</param>
        /// <returns></returns>
        public static string ReplaceRnToBr(this string input, bool reomveConsecutiveWhitespace = true)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            else
            {
                if (reomveConsecutiveWhitespace)
                {
                    return RemoveConsecutiveWhitespaceCharToOne(input).Replace("\r\n", "<br />").Replace("\n", "<br />");
                }
                else
                {
                    return input.Replace("\r\n", "<br />").Replace("\n", "<br />");
                }
            }
        }
        #endregion

        /// <summary>
        /// 正则替换
        /// </summary>
        /// <param name="strData"></param>
        /// <param name="strReg"></param>
        /// <param name="replacement"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string ReplaceByRegex(this string strData, string strReg, string replacement, RegexOptions options = RegexOptions.None)
        {
            if (string.IsNullOrWhiteSpace(strData))
            {
                return strData;
            }
            return Regex.Replace(strData, strReg, replacement, options);
        }

        #endregion

        #region 将IP地址转为long类型
        /// <summary>
        /// 将IP地址转为long类型
        /// 位操作，不会冲突。
        /// </summary>
        /// <param name="IPAddress">字符串格式的IP地址，形式如"192.168.0.1"</param>
        /// <returns></returns>
        public static long IPToInt(string IPAddress)
        {
            long tmpIpNumber = -1;

            if (IPAddress.IsIP())
            {
                //将目标IP地址字符串strIPAddress转换为数字    
                string[] arrayIP = IPAddress.Split('.');
                long sip1 = Convert.ToInt64(arrayIP[0]);
                long sip2 = Convert.ToInt64(arrayIP[1]);
                long sip3 = Convert.ToInt64(arrayIP[2]);
                long sip4 = Convert.ToInt64(arrayIP[3]);
                tmpIpNumber = (sip1 << 24) + (sip2 << 16) + (sip3 << 8) + sip4;

                //long p1 = long.Parse(arrayIP[0]) * 256 * 256 * 256;
                //long p2 = long.Parse(arrayIP[1]) * 256 * 256;
                //long p3 = long.Parse(arrayIP[2]) * 256;
                //long p4 = long.Parse(arrayIP[3]);
                //tmpIpNumber = p1 + p2 + p3 + p4;
            }

            return tmpIpNumber;
        }
        #endregion

        #region 汉字拼音相关

        #region 汉字转拼音

        /// <summary>
        /// 将汉字转换为拼音;如果参数不是有效汉字，则直接返回该字符的字符串形式
        /// </summary>
        /// <param name="_char">待转换的字符</param>
        /// <returns>拼音</returns>
        public static string ChineseCharToPinYin(char _char, bool isToLower = true)
        {
            bool isChineseChar;
            return ChineseCharToPinYin(_char, out isChineseChar, isToLower);
        }

        /// <summary>
        /// 将汉字转换为拼音;如果参数不是有效汉字，则直接返回该字符的字符串形式
        /// </summary>
        /// <param name="_char">待转换的字符</param>
        /// <param name="isToLower">是否将拼音转为小写形式，默认为小写</param>
        /// <param name="IsChineseChar">表示该字符是不是有效汉字</param>
        /// <returns>拼音</returns>
        public static string ChineseCharToPinYin(char _char, out bool isChineseChar, bool isToLower = true)
        {
            isChineseChar = ChineseChar.IsValidChar(_char);
            if (isChineseChar)
            {
                ChineseChar chineseChar = new ChineseChar(_char);
                string pinyin = chineseChar.Pinyins[0];		//原类库支持多音字，但这里无法根据上下文来决定使用哪一个拼音，所以统一取该汉字的第一个发音。
                pinyin = pinyin.Remove(pinyin.Length - 1);	//拼音的最后一个字符是一个数字，表示该汉字的声调，这里不需要使用，删除
                return isToLower ? pinyin.ToLower() : pinyin;
            }
            else
            {
                return _char.ToString();
            }
        }

        /// <summary>
        /// 将中文句子转换为拼音;对句子中的非汉字保留原样，每个汉字拼音与其先后字符用一个分隔符分开
        /// </summary>
        /// <param name="chineseString">待转换的中文句子</param>
        /// <param name="isToLower">是否转换为小写形式；默认为小写</param>
        /// <returns>转换后的拼音句子</returns>
        public static string ChineseStringToPinYin(string chineseString, bool isToLower = true)
        {
            StringBuilder pinyins = new StringBuilder();
            const char WhiteSpace = ' ';
            char lastChar = WhiteSpace;
            bool lastCharIsChineseChar = true;
            foreach (char _char in chineseString)
            {
                bool currentLetterisChineseChar = false;	//表示当前字符是否汉字
                string pinyin = ChineseCharToPinYin(_char, out currentLetterisChineseChar);

                if (currentLetterisChineseChar && lastChar != WhiteSpace)//如果当前字符是汉字，且其前一个字符不是空白，则在拼音前添加一个空白字符用于隔开拼音和其前边的字符
                {
                    pinyins.Append(' ');
                }
                else if (lastCharIsChineseChar && _char != WhiteSpace && !currentLetterisChineseChar)
                {
                    pinyins.Append(' ');
                }
                pinyins.Append(pinyin);

                lastChar = _char;
                lastCharIsChineseChar = currentLetterisChineseChar;
            }
            return pinyins.ToString();
        }
        #endregion

        #region 获取汉字拼音首字母

        /// <summary>
        /// 获取一个汉字的拼音首字母；如果参数不是有效汉字，则直接返回该字符
        /// </summary>
        /// <param name="_char">需要取其拼音首字母的汉字</param>
        /// <returns>拼音首字母</returns>
        public static char GetTheFirstLetterOfPinYin(char _char)
        {
            if (ChineseChar.IsValidChar(_char))
            {
                return new ChineseChar(_char)
                    .Pinyins[0]
                    .FirstOrDefault();
            }
            else
            {
                return _char;
            }
        }

        /// <summary>
        /// 获取一个汉字词组或句子中各汉字的拼音首字母;对句子中的非汉字保留原样
        /// </summary>
        /// <param name="chineseString">待处理汉字</param>
        /// <returns></returns>
        public static string GetTheFirstLettersOfPinYin(string chineseString)
        {
            StringBuilder letters = new StringBuilder();
            foreach (char c in chineseString)
            {
                letters.Append(GetTheFirstLetterOfPinYin(c));
            }
            return letters.ToString();
        }

        #endregion

        #endregion

        #region 简繁体互转
        /// <summary>
        /// 将繁体汉字转为简体汉字
        /// （注：如果服务器上安装有office2007+，转换效果将更好）
        /// </summary>
        /// <param name="chineseString">待转换的汉字</param>
        /// <returns>转换后的简体字</returns>
        public static string TraditionalToSimplified(this string chineseString)
        {
            return ChineseConverter.Convert(chineseString, ChineseConversionDirection.TraditionalToSimplified);
        }

        /// <summary>
        /// 将简体汉字转为繁体汉字
        /// </summary>
        /// <param name="chineseString">待转换的汉字</param>
        /// <returns>转换后的繁体字</returns>
        public static string SimplifiedToTraditional(this string chineseString)
        {
            return ChineseConverter.Convert(chineseString, ChineseConversionDirection.SimplifiedToTraditional);
        }
        #endregion

        #region 文本模板处理

        private static Regex TemplateSyntaxReg = new Regex("{#(?<var>[^}]+)}");
        /// <summary>
        /// 模板解析辅助方法
        /// </summary>
        /// <param name="template">模板。模板示例：hello {#user.name}. <a href="{#link.href}">{#link.text}</a>
        /// </param>
        /// <param name="context">模板中使用的变量环境。
        /// <returns>解析后的文本内容</returns>
        public static string Template(this string template, Dictionary<string, string> context, bool isThrowExceptionWhenNotExistVariable = true)
        {
            return TemplateSyntaxReg.Replace(template, m =>
            {
                string varName = m.Groups["var"].Value;
                if (context.ContainsKey(varName))
                {
                    return context[varName];
                }
                else
                {
                    if (isThrowExceptionWhenNotExistVariable)
                    {
                        throw new ArgumentOutOfRangeException(string.Format("Context字典中不存在模板中使用的变量“{0}”", varName));
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            });
        }

        #endregion

        #region 格式化时间
        /// <summary>
        /// 格式化时间 1秒前，1分钟前，59分钟前，今天 12:30(大于一小时)，5月12日 12:20(大于一天)，2011年5月20日(大于一年)
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string DateStringFromNow(DateTime time)
        {
            //年的判断
            if (DateTime.Now.Year - time.Year > 0)
            {
                return time.ToString("yyyy年MM月dd日 HH:mm");
            }

            TimeSpan span = DateTime.Today.Subtract(time.Date);
            //天的判断(是否大于一天)  5月23 23:59 与 5月24 00:01 也算相差一天 
            if (span.TotalDays >= 1)
            {
                return time.ToString("MM月dd日 HH:mm");
            }
            //不到一天 就重新赋值
            span = DateTime.Now.Subtract(time);
            if (span.TotalHours >= 1)
            {
                return "今天 " + time.ToString("HH:mm");
            }
            else if (span.TotalMinutes >= 1)
            {
                return string.Format("{0}分钟前", Math.Floor(span.TotalMinutes).ConvertTo<int>());
            }
            else if (span.TotalSeconds >= 1)
            {
                return string.Format("{0}秒前", Math.Floor(span.TotalSeconds).ConvertTo<int>());
            }
            else
            {
                return "刚刚";
            }
        }
        #endregion

        #region 读取文件
        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="filePath">从启示项目的根目录开始</param>
        /// <returns></returns>
        public static string ReadFile(string filePath)
        {
            StringBuilder Content = new StringBuilder();
            using (StreamReader strReader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + (filePath), Encoding.Default))
            {
                Content.Append(strReader.ReadToEnd());
                strReader.Close();
                strReader.Dispose();
                return Content.ToString();
            }

        }
        #endregion
    }
}
