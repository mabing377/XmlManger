/*----------------------------------------------------------------
// Copyright (C) 2012 通通优品
// 版权所有。
//
// 文件名：CollectionExtensions.cs
// 功能描述：集合扩展方法
// 
// 创建标识：Public 2012.06.01
// 
// 修改标识：Star.Gu(古红星) 2012.09.25
// 修改描述：
//
// 修改标识：
// 修改描述：
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Public.Common
{
    /// <summary>
    /// 集合扩展方法
    /// </summary>
    public static class CollectionExtensions
    {
        #region 集合操作

        #region 用指定分隔符将 集合 拼接字符串输出
        /// <summary>
        /// 用指定分隔符将 集合 拼接字符串输出
        /// </summary>
        /// <typeparam name="T">集合所包含的类型</typeparam>
        /// <param name="parameters">集合</param>
        /// <param name="valueFormatFun">值的复合格式输出方式</param>
        /// <param name="separator">分隔符</param>
        /// <returns></returns>
        /// <example>示例
        /// <code>
        ///     parameters.JoinSeparatorToString((item) => string.Format("【{0}】{1}", item.Key, item.Value), "\r\n");
        /// </code>
        /// </example>
        public static string JoinSeparatorToString<T>(this ICollection<T> parameters, Func<T, string> valueFormatFun, string separator = "\r\n")
        {
            if (parameters != null && parameters.Count > 0)
            {
                var list = new List<string>(parameters.Count);
                foreach (var param in parameters)
                {
                    list.Add(valueFormatFun(param));
                }
                return string.Join(separator, list);
            }
            return string.Empty;
        }
        #endregion

        #region 用指定分隔符将 键值/对集合 拼接字符串输出
        /// <summary>
        /// 用指定分隔符将 键值/对集合 拼接字符串输出
        /// </summary>
        /// <param name="parameters">键值/对集合</param>
        /// <param name="separator">分隔符</param>
        /// <param name="keyValueformat">键与值之间的复合格式字符串</param>
        /// <returns></returns>
        public static string JoinSeparatorToString(this IDictionary<string, dynamic> parameters, string separator = "\r\n", string keyValueformat = "【{0}】{1}")
        {
            return parameters.JoinSeparatorToString((item) => string.Format(keyValueformat, item.Key, item.Value), separator);
        }
        #endregion

        #region 链式的语法结构添加到集合中
        /// <summary>
        /// 链式的语法结构添加到集合中
        /// </summary>
        /// <typeparam name="T">集合所包含的类型</typeparam>
        /// <param name="parameters">集合</param>
        /// <param name="value">添加的值</param>
        /// <returns></returns>
        public static ICollection<T> AddCollectionValue<T>(this ICollection<T> parameters, T value)
        {
            parameters.Add(value);
            return parameters;
        }
        #endregion

        #endregion

        #region 数组操作
        /// <summary>
        /// 获取指定索引数组的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="index">索引</param>
        /// <param name="def">如果未找到的默认值</param>
        /// <returns></returns>
        public static T GetArrayIndexOfValue<T>(this T[] arr, int index, T def = default(T))
        {
            if (arr != null && arr.Length > index)
                return arr[index];
            else
                return def;
        }

        #region 数组随机排序
        /// <summary>
        /// 数组随机排序
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="arr">操作数组</param>
        /// <returns>处理过的</returns>
        public static T[] GetArrayRandomSort<T>(this T[] arr)
        {
            T[] output = new T[arr.Length];
            Random random = new Random();
            int end = arr.Length - 1;
            for (int i = 0; i < arr.Length; i++)
            {
                int num = random.Next(0, end + 1);
                output[i] = arr[num];
                arr[num] = arr[end];
                end--;
            }
            return output;
        }
        #endregion

        #region 删除字符串数组中的重复元素
        /// <summary>
        /// 删除字符串数组中的重复元素
        /// </summary>
        /// <param name="array">字符串数组</param>
        /// <returns>字符串数组</returns>
        public static T[] DelArrayRepeatItem<T>(this T[] array)
        {
            if (array == null || array.Length == 0) return array;
            HashSet<T> list = new HashSet<T>(array);
            return list.ToArray();
        }
        #endregion

        #endregion

        #region 数据表转换
        /// <summary>
        /// 数据表转换
        /// </summary>
        /// <param name="tb"></param>
        /// <returns></returns>
        public static IList<IDictionary<string, object>> ToDictionaryList(this DataTable tb)
        {
            if (tb == null || tb.Rows.Count == 0) return null;
            var list = new List<IDictionary<string, object>>(tb.Rows.Count);
            foreach (DataRow dr in tb.Rows)
            {
                var dic = new Dictionary<string, object>(tb.Columns.Count);
                foreach (DataColumn item in tb.Columns)
                {
                    dic[item.ColumnName] = dr[item];
                }
                list.Add(dic);
            }
            return list;
        }
        #endregion


    }
}
