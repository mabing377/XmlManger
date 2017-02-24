/*----------------------------------------------------------------
// Copyright (C) 2012 通通优品
// 版权所有。
//
// 文件名：UtilityHelper.cs
// 功能描述：工具库
// 
// 创建标识：Public 2012.06.01
// 
//
//----------------------------------------------------------------*/

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Data;

namespace Public.Common
{
    /// <summary>
    /// 工具库
    /// </summary>
    public static class UtilityHelper
    {
        #region 类型转换

        #region 引用或值类型的值转换(得支持 IConvertible 接口)
        /// <summary>
        /// 引用或值类型的值转换(得支持 IConvertible 接口)
        /// </summary>
        /// <typeparam name="T">要转换后的类型</typeparam>
        /// <param name="convertibleValue">要转换的值</param>
        /// <returns>返回转换后的值</returns>
        /// <param name="commandParameters">查询参数</param>
        /// <example> 示例
        /// <code>
        ///var numvar = "123".ConvertTo&lt;int&gt;(0);
        ///float numfloat = "1.23".ConvertTo(0F);//如果包括默认值可省略类型T
        ///int numint = "123".ConvertTo&lt;int&gt;();//如果不填写默认值默认值为int的默认值
        /// </code>
        /// </example>
        public static T ConvertTo<T>(this IConvertible convertibleValue)
        {
            return ConvertTo<T>(convertibleValue, default(T));
        }
        #endregion

        #region 引用或值类型的值转换(得支持 IConvertible 接口，不支持则返回默认值)
        /// <summary>
        /// 引用或值类型的值转换(得支持 IConvertible 接口，不支持则返回默认值)
        /// </summary>
        /// <typeparam name="T">要转换后的类型</typeparam>
        /// <param name="convertibleValue">要转换的值</param>
        /// <returns>返回转换后的值</returns>
        /// <example> 示例
        /// <code>
        ///var numvar = "123".ConvertTo&lt;int&gt;(0);
        ///float numfloat = "1.23".ConvertTo(0F);//如果包括默认值可省略类型T
        ///int numint = "123".ConvertTo&lt;int&gt;();//如果不填写默认值默认值为int的默认值
        /// </code>
        /// </example>
        public static T ConvertTo<T>(this object convertibleValue)
        {
            return ConvertTo<T>(convertibleValue, default(T));
        }
        #endregion

        #region 引用或值类型的值转换(得支持 IConvertible 接口，不支持则返回默认值)
        /// <summary>
        /// 引用或值类型的值转换(得支持 IConvertible 接口，不支持则返回默认值)
        /// </summary>
        /// <typeparam name="T">要转换后的类型</typeparam>
        /// <param name="convertibleValue">要转换的值</param>
        /// <param name="defaultValue">转换失败后，返回的默认值</param>
        /// <returns>转换成功，返回转换后的值；转换失败，返回参数中设置的默认值</returns>
        /// <example> 示例
        /// <code>
        ///var numvar = "123".ConvertTo&lt;int&gt;(0);
        ///float numfloat = "1.23".ConvertTo(0F);//如果包括默认值可省略类型T
        ///int numint = "123".ConvertTo&lt;int&gt;();//如果不填写默认值默认值为int的默认值
        /// </code>
        /// </example>
        public static T ConvertTo<T>(this object convertibleValue, T defaultValue)
        {
            if (convertibleValue is IConvertible)
            {
                return ConvertTo<T>(convertibleValue as IConvertible, defaultValue);
            }
            else if (convertibleValue is System.DBNull)
            {
                return defaultValue;
            }
            else if (convertibleValue is T)
            {
                return (T)convertibleValue;
            }
            else
            {
                return defaultValue;
            }
            //return convertibleValue is IConvertible ? ConvertTo<T>(convertibleValue as IConvertible, defaultValue) : defaultValue;
        }
        #endregion

        #region 引用或值类型的值转换(得支持 IConvertible 接口)
        /// <summary>
        /// 引用或值类型的值转换(得支持 IConvertible 接口)
        /// </summary>
        /// <typeparam name="T">要转换后的类型</typeparam>
        /// <param name="convertibleValue">要转换的值</param>
        /// <param name="defaultValue">转换失败后，返回的默认值</param>
        /// <returns>转换成功，返回转换后的值；转换失败，返回参数中设置的默认值</returns>
        /// <example> 示例
        /// <code>
        ///var numvar = "123".ConvertTo&lt;int&gt;(0);
        ///float numfloat = "1.23".ConvertTo(0F);//如果包括默认值可省略类型T
        ///int numint = "123".ConvertTo&lt;int&gt;();//如果不填写默认值默认值为int的默认值
        /// </code>
        /// </example> 
        public static T ConvertTo<T>(this IConvertible convertibleValue, T defaultValue)
        {
            if (null != convertibleValue)
            {
                try
                {
                    Type typeT = typeof(T);
                    if (!typeT.IsGenericType)
                    {
                        if (typeT.IsEnum)
                        {
                            if (typeT.IsEnumDefined(convertibleValue))
                            {
                                return (T)Enum.Parse(typeT, convertibleValue.ToString(), true);
                            }
                        }
                        else if (typeT == typeof(Guid) && convertibleValue is String)
                        {
                            Guid v;
                            if (Guid.TryParse((convertibleValue ?? string.Empty).ToString(), out v))
                            {
                                return (T)(object)(v);
                            }
                            return defaultValue;
                        }
                        else
                        {
                            return (T)Convert.ChangeType(convertibleValue, typeT);
                        }
                    }
                    else
                    {
                        Type genericTypeDefinition = typeT.GetGenericTypeDefinition();
                        if (genericTypeDefinition == typeof(Nullable<>))
                        {
                            Type underlyingType = Nullable.GetUnderlyingType(typeT);

                            if (underlyingType.IsEnum)
                            {
                                if (underlyingType.IsEnumDefined(convertibleValue))
                                    return (T)Enum.Parse(underlyingType, convertibleValue.ToString(), true);
                            }
                            else if (typeT == typeof(Guid?) && convertibleValue is String)
                            {
                                Guid v;
                                if (Guid.TryParse((convertibleValue ?? string.Empty).ToString(), out v))
                                {
                                    return (T)(object)(v);
                                }
                                return defaultValue;
                            }
                            else
                            {
                                return (T)Convert.ChangeType(convertibleValue, Nullable.GetUnderlyingType(typeT));
                            }
                        }
                    }
                }
                catch
                {
                    return defaultValue;
                }
            }
            return defaultValue;
        }
        #endregion

        #region 可空对象没有值时，返回替代值
        /// <summary>
        /// 可空对象没有值时，返回替代值
        /// </summary>
        /// <typeparam name="T">可空对象类型</typeparam>
        /// <param name="input">可空对象</param>
        /// <param name="instead">替代值</param>
        /// <returns></returns>
        public static T ReturnIsNullValue<T>(this Nullable<T> input, T instead) where T : struct
        {
            return input.HasValue ? input.Value : instead;
        }
        #endregion

        #endregion

        #region 枚举扩展方法
        /// <summary>
        /// 获取枚举值的描述信息
        /// </summary>
        /// <param name="enumValue">枚举值</param>
        /// <returns>返回此枚举值没有Description属性标记，或其值的字符串表示形式</returns>
        public static string GetEnumDescription(this Enum enumValue)
        {
            if (enumValue == null) return string.Empty;
            string valueString = enumValue.ToString();
            System.Reflection.FieldInfo fieldinfo = enumValue.GetType().GetField(valueString);
            var attributes = fieldinfo.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            if (attributes != null && attributes.Length == 1)
            {
                var attribute = attributes[0] as System.ComponentModel.DescriptionAttribute;
                if (attribute != null)
                {
                    valueString = attribute.Description;
                }
            }
            return valueString;
        }
        #endregion

        #region 转换人民币大小金额
        /// <summary>
        /// 转换人民币大小金额
        /// </summary>
        /// <param name="inputNumber">要转换的金额</param>
        /// <returns>返回大写形式</returns>
        public static string ConvertToRMBAmounts(decimal inputNumber)
        {
            string chineseCharacter = "零壹贰叁肆伍陆柒捌玖";      //0-9所对应的汉字
            string rmbUnits = "万仟佰拾亿仟佰拾万仟佰拾元角分";    //人民币单位
            string stringFormInputNumber = "";        //从输入的inputNumber中取出的值
            string inputNumberToString = "";          //输入的数字的字符串形式
            string rmbToUper = "";                    //人民币大写金额形式
            int i;               //循环变量
            int intputStringLength;    //输入的inputNumber乘以100的字符串长度
            string chineseReading1 = "";             //数字的汉语读法
            string chineseReading2 = "";             //数字的汉字读法 
            int zeroNunmber = 0;                     //用来计算连续的零值是几个
            int temp;           //从输入的inputNumber中取出的值

            inputNumber = Math.Round(Math.Abs(inputNumber), 2);              //将inputNumber取绝对值并四舍五入取2位小数
            inputNumberToString = ((long)(inputNumber * 100)).ToString();    //将inputNumber乘100并转换成字符串形式 
            intputStringLength = inputNumberToString.Length;                 //找出最高位 
            if (intputStringLength > 15)
            {
                return "溢出";
            }
            rmbUnits = rmbUnits.Substring(15 - intputStringLength);          //取出对应位数的rmbUnits的值。如：200.55,intputStringLength为5时rmbUnits=佰拾元角分 

            //循环取出每一位需要转换的值 
            for (i = 0; i < intputStringLength; i++)
            {
                stringFormInputNumber = inputNumberToString.Substring(i, 1);          //取出需转换的某一位的值 
                temp = Convert.ToInt32(stringFormInputNumber);      //转换为数字 
                if (i != (intputStringLength - 3) && i != (intputStringLength - 7) && i != (intputStringLength - 11) && i != (intputStringLength - 15))
                {
                    //当所取位数不为元、万、亿、万亿上的数字时 
                    if (stringFormInputNumber == "0")
                    {
                        chineseReading1 = "";
                        chineseReading2 = "";
                        zeroNunmber = zeroNunmber + 1;
                    }
                    else
                    {
                        if (stringFormInputNumber != "0" && zeroNunmber != 0)
                        {
                            chineseReading1 = "零" + chineseCharacter.Substring(temp * 1, 1);
                            chineseReading2 = rmbUnits.Substring(i, 1);
                            zeroNunmber = 0;
                        }
                        else
                        {
                            chineseReading1 = chineseCharacter.Substring(temp * 1, 1);
                            chineseReading2 = rmbUnits.Substring(i, 1);
                            zeroNunmber = 0;
                        }
                    }
                }
                else
                {
                    //该位是万亿，亿，万，元位等关键位 
                    if (stringFormInputNumber != "0" && zeroNunmber != 0)
                    {
                        chineseReading1 = "零" + chineseCharacter.Substring(temp * 1, 1);
                        chineseReading2 = rmbUnits.Substring(i, 1);
                        zeroNunmber = 0;
                    }
                    else
                    {
                        if (stringFormInputNumber != "0" && zeroNunmber == 0)
                        {
                            chineseReading1 = chineseCharacter.Substring(temp * 1, 1);
                            chineseReading2 = rmbUnits.Substring(i, 1);
                            zeroNunmber = 0;
                        }
                        else
                        {
                            if (stringFormInputNumber == "0" && zeroNunmber >= 3)
                            {
                                chineseReading1 = "";
                                chineseReading2 = "";
                                zeroNunmber = zeroNunmber + 1;
                            }
                            else
                            {
                                if (intputStringLength >= 11)
                                {
                                    chineseReading1 = "";
                                    zeroNunmber = zeroNunmber + 1;
                                }
                                else
                                {
                                    chineseReading1 = "";
                                    chineseReading2 = rmbUnits.Substring(i, 1);
                                    zeroNunmber = zeroNunmber + 1;
                                }
                            }
                        }
                    }
                }
                if (i == (intputStringLength - 11) || i == (intputStringLength - 3))
                {
                    //如果该位是亿位或元位，则必须写上 
                    chineseReading2 = rmbUnits.Substring(i, 1);
                }
                rmbToUper = rmbToUper + chineseReading1 + chineseReading2;

                if (i == intputStringLength - 1 && stringFormInputNumber == "0")
                {
                    //最后一位（分）为0时，加上“整” 
                    rmbToUper = rmbToUper + '整';
                }
            }
            if (inputNumber == 0)
            {
                rmbToUper = "零元整";
            }

            return rmbToUper;
        }
        #endregion

        #region 日期时间

        #region 获取一周的起止范围
        /// <summary>
        /// 获取一周的起止范围
        /// </summary>
        /// <param name="startTime">周一零点，开始时间</param>
        /// <param name="endTime">下周一零点，结束时间</param>
        /// <param name="offset">相对于本周的偏移值（单位周），为0则为本周，负数则上周×Offset，正数则下周×Offset</param>
        public static void GetWeekRange(int offset, out DateTime startTime, out DateTime endTime)
        {
            DateTime Now = DateTime.Now;
            int Day = Convert.ToInt32(Now.DayOfWeek);
            TimeSpan Time = Now.TimeOfDay;

            startTime = Now.AddDays(-Day + 1).Add(-Time);

            startTime = startTime.AddDays(offset * 7);
            endTime = startTime.AddDays(7);
        }
        #endregion 获取一周的起止范围

        #region 计算年龄

        /// <summary>
        /// 计算指定日期出生的人的年龄（周岁）
        /// </summary>
        /// <param name="birthday">出生日期时间</param>
        /// <returns></returns>
        public static int ComputeFullAge(DateTime birthday)
        {
            return ComputeFullAge(birthday, DateTime.Now);
        }

        /// <summary>
        /// 计算指定日期出生的人的年龄（周岁）
        /// </summary>
        /// <param name="birthday">出生日期时间</param>
        /// <param name="finishday">现在的日期时间</param>
        /// <returns></returns>
        public static int ComputeFullAge(DateTime birthday, DateTime finishday)
        {
            //int ages = 0;
            //DateTime theday = birthday.Date.AddYears(1);
            //while (theday <= finishday)
            //{
            //    ages++;
            //    //Console.WriteLine("到{0:yyyy-MM-dd} 满{1:00}周岁", theday, ages);
            //    theday = theday.AddYears(1);
            //}
            //return ages;
            int ages = finishday.Year - birthday.Year;
            if (ages < 1) return 0;
            //如果已经过生日
            if (finishday.AddYears(-ages) >= birthday)
            {
                return ages;
            }
            else
            {
                return ages - 1;
            }
        }

        /// <summary>
        /// 计算指定日期出生的人的月数
        /// </summary>
        /// <param name="birthday">出生日期时间</param>
        /// <returns></returns>
        public static int ComputeFullMonth(DateTime birthday)
        {
            return ComputeFullMonth(birthday, DateTime.Now);
        }

        /// <summary>
        /// 计算指定日期出生的人的月数
        /// </summary>
        /// <param name="birthday"></param>
        /// <returns></returns>
        public static int ComputeFullMonth(DateTime birthday, DateTime finishday)
        {
            //int months = 0;
            //DateTime theday = birthday.Date.AddMonths(1);
            //while (theday <= finishday)
            //{
            //    months++;
            //    //Console.WriteLine("到{0:yyyy-MM-dd} 满{1:00}个月", theday, months);
            //    theday = theday.AddMonths(1);
            //}
            //return months;
            int ages = finishday.Year - birthday.Year;
            if (ages < 0) return 0;
            int months = ages * 12 + (finishday.Month - birthday.Month);//计算整月数，尚未比较时间
            if (months < 1) return 0;
            if (finishday.AddYears(-ages) >= birthday) //如果已经过生日（比较时间，计算最后一个月是否已满）
            {
                return months;//如果已满
            }
            else
            {
                return months - 1;
            }
        }

        #endregion

        #region 计算操作距离当前的时间差
        /// <summary>
        /// 计算操作距离当前的时间差
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string DateStringFromNow(DateTime dt)
        {
            TimeSpan span = DateTime.Now - dt;
            if (span.TotalDays > 60)
            {
                return dt.ToShortDateString();
            }
            else
            {
                if (span.TotalDays > 30)
                {
                    return "1个月前";
                }
                else
                {
                    if (span.TotalDays > 14)
                    {
                        return "2周前";
                    }
                    else
                    {
                        if (span.TotalDays > 7)
                        {
                            return "1周前";
                        }
                        else
                        {
                            if (span.TotalDays > 1)
                            {
                                return string.Format("{0}天前", (int)Math.Floor(span.TotalDays));
                            }
                            else
                            {
                                if (span.TotalHours > 1)
                                {
                                    return string.Format("{0}小时前", (int)Math.Floor(span.TotalHours));
                                }
                                else
                                {
                                    if (span.TotalMinutes > 1)
                                    {
                                        return string.Format("{0}分钟前", (int)Math.Floor(span.TotalMinutes));
                                    }
                                    else
                                    {
                                        if (span.TotalSeconds >= 1)
                                        {
                                            return string.Format("{0}秒前", (int)Math.Floor(span.TotalSeconds));
                                        }
                                        else
                                        {
                                            return "1秒前";
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region 计算两个时间的差值
        /// <summary>
        /// 计算两个时间的差值(4维数组，索引 0：n天  1：n小时  2：n分钟  3：n秒)
        /// </summary>
        /// <param name="before">前日期时间</param>
        /// <param name="after">后日期时间</param>
        /// <param name="separator">分割符</param>
        /// <returns>4维数组，索引 0：n天  1：n小时  2：n分钟  3：n秒</returns>
        public static string[] DateDiff(DateTime before, DateTime after)
        {
            //string dateDiff = null;
            //TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
            //TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
            //TimeSpan ts = ts1.Subtract(ts2).Duration();
            TimeSpan ts = after - before;
            //dateDiff = ts.Days.ToString() + "天" + ts.Hours.ToString() + "小时" + ts.Minutes.ToString() + "分钟" + ts.Seconds.ToString() + "秒";
            //return dateDiff;
            return new string[] { ts.Days.ToString(), ts.Hours.ToString(), ts.Minutes.ToString(), ts.Seconds.ToString() };
        }
        #endregion

        #region 计算宝宝的年龄、星座

        #region 计算宝宝的岁数
        /// <summary>
        /// 计算宝宝的岁数
        /// </summary>
        /// <param name="birthday">宝宝的出生日期</param>
        /// <returns></returns>
        public static string GetBabyAge(DateTime birthday)
        {
            string age = String.Empty;
            if (birthday > DateTime.Now)
            {
                age = "未出生";
            }
            else
            {
                // 计算总月数
                int month = UtilityHelper.ComputeFullMonth(birthday);
                // 计算总页面数量
                if (month % 12 == 0)
                {
                    age = (month / 12).ToString() + "岁";
                }
                else
                {
                    age = (month / 12).ToString() + "岁" + (month % 12).ToString() + "个月";
                }
            }
            return age;
        }
        #endregion

        #region 得到用户生肖
        /// <summary>
        /// 得到用户生肖
        /// </summary>
        /// <param name="birthday"></param>
        /// <returns></returns>
        public static string GetShengXiao(DateTime birthday)
        {
            System.Globalization.ChineseLunisolarCalendar chinseCaleander = new System.Globalization.ChineseLunisolarCalendar();

            string TreeYear = "鼠牛虎兔龙蛇马羊猴鸡狗猪";

            int intYear = chinseCaleander.GetSexagenaryYear(birthday);

            string Tree = TreeYear.Substring(chinseCaleander.GetTerrestrialBranch(intYear) - 1, 1);

            return Tree;
        }
        #endregion

        #region 计算给定时间的星座信息
        /// <summary>
        /// 计算给定时间的星座信息
        /// </summary>
        /// <param name="birthday">出生日期</param>
        /// <returns>返回相应的星座</returns>
        public static string GetAtomFromBirthday(DateTime birthday)
        {
            float birthdayF = 0.00F;
            string month = birthday.Month.ToString().PadLeft(2, '0');
            string day = birthday.Day.ToString().PadLeft(2, '0');
            if (birthday.Month == 1 && birthday.Day < 20)
            {
                birthdayF = float.Parse(string.Format("13.{0}", day));
            }
            else
            {
                birthdayF = float.Parse(string.Format("{0}.{1}", month, day));
            }
            float[] atomBound = { 1.20F, 2.20F, 3.21F, 4.21F, 5.21F, 6.22F, 7.23F, 8.23F, 9.23F, 10.23F, 11.21F, 12.22F, 13.20F };
            string[] atoms = { "水瓶座", "双鱼座", "白羊座", "金牛座", "双子座", "巨蟹座", "狮子座", "处女座", "天秤座", "天蝎座", "射手座", "魔羯座" };

            string ret = "未知星座";
            for (int i = 0; i < atomBound.Length - 1; i++)
            {
                if (atomBound[i] <= birthdayF && atomBound[i + 1] > birthdayF)
                {
                    ret = atoms[i];
                    break;
                }
            }
            return ret;
        }
        #endregion

        #endregion

        #region 将时间间隔(TimeSpan)格式化输出
        /// <summary>
        /// 将时间间隔(TimeSpan)格式化输出，如：12天12小时6分6秒
        /// </summary>
        /// <param name="span"></param>
        /// <returns></returns>
        public static string ConvertTimeSpanToFormatString(TimeSpan span)
        {
            string timeString = null;
            if (span.TotalMilliseconds <= 0)
                return "0秒";
            if (span.Days > 0)
            {
                timeString = (span.Days + "天");
            }
            if (span.Hours > 0)
            //if ((span.Days > 0) || (span.Hours > 0))
            {
                int hours = span.Hours;
                //int hours = ((0x18 * span.Days) + span.Hours);
                timeString = (timeString + hours + "小时");
            }
            if (span.Minutes > 0)
            {
                timeString = (timeString + span.Minutes + "分");
            }
            if (span.Seconds > 0)
            {
                timeString = (timeString + span.Seconds + "秒");
            }
            return timeString;
        }
        #endregion

        #endregion

        #region 随机操作

        #region 生成随机字符串
        /// <summary>
        /// 解决在同一个时间点生成相同随机字符bug
        /// </summary>
        private static int _Zseed = new Random().Next();
        /// <summary>
        /// 生成随机字符串
        /// </summary>
        /// <param name="len">总长度</param>
        /// <param name="addLowerLetter">包括小写字母</param>
        /// <param name="addUpperLetter">包括大写字母</param>
        /// <param name="addNumStr">包括数字</param>
        /// <param name="addOtherStr">包括其他特殊字母<![CDATA[~!@#$%^&*_+:;?,./]]></param>
        /// <returns></returns>
        public static string GetRandomString(int len, bool addLowerLetter, bool addUpperLetter, bool addNumStr, bool addOtherStr)
        {
            StringBuilder value = new StringBuilder(len);
            if (len > 0)
            {
                string a_z = "abcdefghigklmnopqrstuvwxyz";
                string A_Z = a_z.ToUpper();
                string numericStr = "0123456789";
                string otherStr = "~!@#$%^&*_+:;?,./";

                StringBuilder doStr = new StringBuilder(a_z.Length * 2 + numericStr.Length + otherStr.Length);
                if (addLowerLetter)
                {
                    doStr.Append(a_z);
                }
                if (addUpperLetter)
                {
                    doStr.Append(A_Z);
                }
                if (addNumStr)
                {
                    doStr.Append(numericStr);
                }
                if (addOtherStr)
                {
                    doStr.Append(otherStr);
                }
                if (doStr.Length > 0)
                {
                    for (int i = 0; i < len; i++)
                    {
                        unchecked
                        {
                            _Zseed++;
                        }
                        int seed = unchecked(_Zseed);
                        Random rnd = new Random(seed);
                        value.Append(doStr[rnd.Next(0, doStr.Length - 1)]);
                    }
                }

            }
            return value.ToString();
        }
        #endregion

        #endregion

        #region 工具函数

        #region 格式化字节数字符串
        /// <summary>
        /// 格式化字节数字符串，输出文件大小的最大单位并四舍五入小数点两位。（如:1073741824B => 1GB）
        /// </summary>
        /// <param name="size">单位为字节(B)</param>
        /// <returns></returns>
        public static string ConvertBytesFormatString(double size)
        {
            string sizeString;
            if (size >= 1073741824)
            {
                sizeString = (Math.Round(size / 1073741824, 2).ToString("#,#", System.Globalization.CultureInfo.InvariantCulture) + "GB");
            }
            else if (size >= 1048576)
            {
                sizeString = (Math.Round(size / 1048576, 2) + "MB");
            }
            else if (size >= 1024)
            {
                sizeString = (Math.Round(size / 1024, 2) + "KB");
            }
            else
            {
                sizeString = (size + "B");
            }
            return sizeString;
        }
        #endregion

        #region JSON序列化


        /// <summary>
        /// 将对象转换为 JSON 字符串。
        /// </summary>
        /// <param name="instance">要序列化的对象。</param>
        /// <param name="maxJsonLength">接受的 JSON 字符串的最大长度。</param>
        /// <param name="recursionLimit">用于约束要处理的对象级别的数目的限制。</param>
        /// <returns>序列化的 JSON 字符串。</returns>
        public static string JsonSerialize(this object instance, int? maxJsonLength = null, int? recursionLimit = null)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            if (maxJsonLength.HasValue)
            {
                serializer.MaxJsonLength = maxJsonLength.Value;
            }
            if (recursionLimit.HasValue)
            {
                serializer.RecursionLimit = recursionLimit.Value;
            }
            return serializer.Serialize(instance);
        }

        /// <summary>
        /// 序列化对象并将生成的 JSON 字符串写入指定的 System.Text.StringBuilder 对象。
        /// </summary>
        /// <param name="instance">要序列化的对象。</param>
        /// <param name="maxJsonLength">接受的 JSON 字符串的最大长度。</param>
        /// <param name="recursionLimit">用于约束要处理的对象级别的数目的限制。</param>
        /// <param name="output">用于写 JSON 字符串的 System.Text.StringBuilder 对象。</param>
        public static void JsonSerialize(this object instance, StringBuilder output, int? maxJsonLength = null, int? recursionLimit = null)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            if (maxJsonLength.HasValue)
            {
                serializer.MaxJsonLength = maxJsonLength.Value;
            }
            if (recursionLimit.HasValue)
            {
                serializer.RecursionLimit = recursionLimit.Value;
            }
            serializer.Serialize(instance, output);

        }

        /// <summary>
        /// 将指定的 JSON 字符串转换为 T 类型的对象。
        /// </summary>
        /// <typeparam name="T">所生成对象的类型。</typeparam>
        /// <param name="input">要进行反序列化的 JSON 字符串。</param>
        /// <param name="def">转换失败默认值</param>
        ///<param name="isThrowCatch">转换失败是否抛异常</param>
        /// <param name="maxJsonLength">接受的 JSON 字符串的最大长度。</param>
        /// <param name="recursionLimit">用于约束要处理的对象级别的数目的限制。</param>
        /// <returns>反序列化的对象。</returns>
        public static T JsonDeserialize<T>(this string input, T def = default(T), bool isThrowCatch = true, int? maxJsonLength = null, int? recursionLimit = null)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                if (maxJsonLength.HasValue)
                {
                    serializer.MaxJsonLength = maxJsonLength.Value;
                }
                if (recursionLimit.HasValue)
                {
                    serializer.RecursionLimit = recursionLimit.Value;
                }
                return serializer.Deserialize<T>(input);
            }
            catch (Exception ex)
            {
                if (isThrowCatch) throw new Exception(string.Format("反序列化失败 Type：{0}\r\ninput:{1}", typeof(T), input), ex);
                return def;
            }

        }
        #endregion

        #region 获取本机IP
        /// <summary>
        /// 获取本机IP
        /// </summary>
        /// <param name="hostname">主机名</param>
        /// <returns></returns>
        public static string GetTheMachineIP(string hostname = null)
        {
            string ipv4 = String.Empty;
            foreach (System.Net.IPAddress IPA in System.Net.Dns.GetHostAddresses(hostname != null ? hostname : System.Web.HttpContext.Current.Request.UserHostAddress))
            {
                if (IPA.AddressFamily.ToString() == "InterNetwork")
                {
                    ipv4 = IPA.ToString();
                    break;
                }
            }
            if (ipv4 != String.Empty)
            {
                return ipv4;
            }
            foreach (System.Net.IPAddress IPA in System.Net.Dns.GetHostAddresses(hostname != null ? hostname : System.Net.Dns.GetHostName()))
            {
                if (IPA.AddressFamily.ToString() == "InterNetwork")
                {
                    ipv4 = IPA.ToString();
                    break;
                }
            }
            return ipv4;
        }
        #endregion

        #endregion

        #region 检查是否允许的 IP
        /// <summary>
        /// 检查是否允许的 IP
        /// </summary>
        /// <param name="allowIps">允许的IP列表，支持星号“192.168.*.*”</param>
        /// <param name="ip">ip地址</param>
        /// <returns></returns>
        public static bool IsAllowIP(HashSet<string> allowIps, string ip)
        {
            if (allowIps.Count > 0)
            {
                bool result = allowIps.Contains(ip);
                if (!result)
                {
                    foreach (var item in allowIps)
                    {
                        if (item.Contains("*"))
                        {
                            if (System.Text.RegularExpressions.Regex.IsMatch(ip, "^" + item.Replace(".", "\\.").Replace("*", "[0-9]+") + "$")) return true;
                        }
                    }
                }

                return result;
            }
            return false;
        }
        #endregion

        #region MachineKey生成方法
        /// <summary>
        /// MachineKey生成方法 Generate Cryptographically Random Keys
        /// </summary>
        /// <param name="length">
        /// For SHA1, set the validationKey to 64 bytes (128 hexadecimal characters).
        /// For AES, set the decryptionKey to 32 bytes (64 hexadecimal characters).
        /// For 3DES, set the decryptionKey to 24 bytes (48 hexadecimal characters).
        /// </param>
        /// <returns></returns>
        public static string CreateMachineKey(int length)
        {
            // 要返回的字符格式为16进制,byte最大值255
            // 需要2个16进制数保存1个byte,因此除2
            byte[] random = new byte[length / 2];

            // 使用加密服务提供程序 (CSP) 提供的实现来实现加密随机数生成器 (RNG)
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

            // 用经过加密的强随机值序列填充字节数组
            rng.GetBytes(random);

            StringBuilder machineKey = new StringBuilder(length);
            for (int i = 0; i < random.Length; i++)
            {
                machineKey.Append(string.Format("{0:X2}", random[i]));
            }
            return machineKey.ToString();
        }
        #endregion

        #region 数据库相关
        /// <summary>
        /// 读取列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static List<T> DataReaderToList<T>(this IDataReader dr)
        {
            return dr.ReaderToList<T>();
        }
        /// <summary>
        /// 读取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static T DataReaderToModel<T>(this IDataReader dr)
        {
            return dr.ReaderToModel<T>();
        }
        /// <summary>
        /// 读取列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static List<T> ReaderToList<T>(this IDataReader dr)
        {
            List<T> list = new List<T>();
            T obj = default(T);
            using (dr)
            {
                HashSet<string> set = new HashSet<string>();
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    set.Add(dr.GetName(i));
                }
                while (dr.Read())
                {
                    obj = Activator.CreateInstance<T>();
                    foreach (System.Reflection.PropertyInfo prop in obj.GetType().GetProperties())
                    {
                        if (set.Contains(prop.Name) && !object.Equals(dr[prop.Name], DBNull.Value))
                        {
                            //prop.SetValue(obj, dr[prop.Name], null);
                            prop.SetValue(obj, ConvertToType(dr[prop.Name], prop.PropertyType), null);
                        }
                    }
                    list.Add(obj);
                }
            }

            return list;
        }
        /// <summary>
        /// 读取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static T ReaderToModel<T>(this IDataReader dr)
        {
            var list = ReaderToList<T>(dr);
            if (list.Count > 0)
            {
                return list[0];
            }
            return default(T);
        }
        /// <summary>
        /// 转换成制定类型对象
        /// </summary>
        /// <param name="value"></param>
        /// <param name="conversionType"></param>
        /// <returns></returns>
        public static object ConvertToType(this object value, Type conversionType)
        {
            //这个类对可空类型进行判断转换，要不然会报错 
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                    return null;

                System.ComponentModel.NullableConverter nullableConverter = new System.ComponentModel.NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            }
            if (conversionType == typeof(Guid) && value is String)
            {
                return (object)Guid.Parse((value ?? string.Empty).ToString());
            }
            else if (conversionType.IsEnum)
            {
                return Enum.Parse(conversionType, (value ?? string.Empty).ToString(), true);
            }
            return Convert.ChangeType(value, conversionType);
        }
        #endregion


        #region 交叉数组组合
        /// <summary>
        /// 交叉数组组合 [[A,B,C],[a,b],[1,2,3,4]] => Aa1 Aa2 Aa3 Aa4 Ab1 Ab2 Ab3 Ab4 Ba1 Ba2 Ba3 Ba4 Bb1 Bb2 Bb3 Bb4 Ca1 Ca2 Ca3 Ca4 Cb1 Cb2 Cb3 Cb4
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="crossArr">交叉数组</param>
        /// <param name="separator">分割符</param>
        /// <returns></returns>
        /// <example>
        /// <code>
        /// string[][] crossArr = new string[][] { new string[] { "A", "B", "C" }, new string[] { "a", "b" }, new string[] { "1", "2", "3", "4" } };  
        ///  var list = JoinCrossArray(crossArr, ",");
        /// </code>
        /// </example>
        public static string[] JoinCrossArray<T>(this T[][] crossArr, string separator)
        {
            T[] name = new T[crossArr.Length];
            List<string> names = new List<string>();
            _JoinCrossArray<T>(name, ref names, 0, crossArr[0], crossArr, separator);
            return names.ToArray();
            #region 原理
            //foreach (var sarr in arr[0])
            //{
            //    foreach (var _sarr in arr[1])
            //    {
            //        foreach (var __sarr in arr[2])
            //        {
            //            Console.WriteLine(sarr+_sarr+__sarr);
            //        }
            //    }
            //}
            #endregion

        }
        private static void _JoinCrossArray<T>(T[] name, ref List<string> names, int index, T[] sarr, T[][] arr, string separator)
        {
            for (int i = 0; i < sarr.Length; i++)
            {
                name[index] = sarr[i];
                if (index + 1 == arr.Length)
                {
                    names.Add(string.Join<T>(separator, name));
                }
                else
                {
                    index++;
                    _JoinCrossArray(name, ref names, index, arr[index], arr, separator);
                    index--;

                }
            }
        }
        #endregion

    }
}