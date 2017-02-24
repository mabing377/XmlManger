/*----------------------------------------------------------------
// Copyright (C) 2012 通通优品
// 版权所有。
//
// 文件名：SerializationHelper.cs
// 功能描述：序列化工具类
// 
// 创建标识：Star Gu（古红星） 2012.06.25
// 
// 修改标识：
// 修改描述：
//
// 修改标识：
// 修改描述：
//
//----------------------------------------------------------------*/

using System;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Collections.Generic;
using System.Text;

namespace Public.Common
{
    /// <summary>
    /// XML序列化工具类
    /// </summary>
    public static class XmlSerializationHelper
    {
        #region 从XML文件中 反序列化
        /// <summary>
        /// 从XML文件中反序列化
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="fileName">文件绝对路径</param>
        /// <returns></returns>
        public static object Load(Type type, string fileName)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                XmlSerializer serializer = new XmlSerializer(type);
                return serializer.Deserialize(fs);
            }
            catch (Exception ex)
            {
                throw ex;
                //throw new Exception(string.Format("type:{0} fileName:{1}", type, fileName), ex);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }
        #endregion

        #region 序列化为XML文件
        /// <summary>
        /// 序列化为XML文件
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="fileName">文件绝对路径</param>
        public static void Save(object obj, string fileName)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(fs, obj);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }
        #endregion

        #region 将XML序列化成字符串
        /// <summary>
        /// 将XML序列化成字符串
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>XML字符串</returns>
        public static string Serialize(object obj)
        {
            string resultValue = "";
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            MemoryStream ms = new MemoryStream();
            XmlTextWriter xtw = null;
            StreamReader sr = null;
            try
            {
                xtw = new System.Xml.XmlTextWriter(ms, Encoding.UTF8);
                xtw.Formatting = System.Xml.Formatting.Indented;
                serializer.Serialize(xtw, obj);
                ms.Seek(0, SeekOrigin.Begin);
                sr = new StreamReader(ms);
                resultValue = sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (xtw != null)
                {
                    xtw.Close();
                }
                if (sr != null)
                {
                    sr.Close();
                }
                ms.Close();
            }
            return resultValue;
        }
        #endregion

        #region 将xml字符串序列化为对象
        /// <summary>
        /// 将xml字符串序列化为对象
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="s">待序列化xml字符串</param>
        /// <returns>对象</returns>
        public static object DeSerialize(Type type, string input)
        {
            try
            {
                byte[] b = System.Text.Encoding.UTF8.GetBytes(input);
                XmlSerializer serializer = new XmlSerializer(type);
                return serializer.Deserialize(new MemoryStream(b));
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("反序列化失败 Type：{0}\r\ninput:{1}", type, input), ex);
            }
        }
        /// <summary>
        /// 将xml字符串序列化为对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="s">待序列化xml字符串</param>
        /// <returns></returns>
        public static T DeSerialize<T>(string s)
        {
            return DeSerialize(typeof(T), s).ConvertTo<T>();
        }
        #endregion
    }
}
