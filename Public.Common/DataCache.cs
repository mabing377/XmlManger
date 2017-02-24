using System;
using System.Collections;
using System.Web;
using System.Web.Caching;

namespace Public.Common
{
    /// <summary>
    /// 缓存相关的操作类
    /// </summary>
    public class DataCache
    {
        /// <summary>
        /// 获取当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="CacheKey"></param>
        /// <returns></returns>
        public static object GetCache(string CacheKey)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            return objCache[CacheKey];
        }

        /// <summary>
        /// 设置当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="CacheKey"></param>
        /// <param name="objObject"></param>
        public static void SetCache(string CacheKey, object objObject)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            RemoveCache(CacheKey);
            objCache.Insert(CacheKey, objObject, null, DateTime.Now.AddHours(30), TimeSpan.Zero, CacheItemPriority.High, null);
        }
        public static void SetCache(string CacheKey, object objObject, int addMin)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            RemoveCache(CacheKey);
            objCache.Insert(CacheKey, objObject, null, DateTime.Now.AddMinutes(addMin), TimeSpan.Zero, CacheItemPriority.High, null);
        }

        /// <summary>
        /// 去除当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="CacheKey"></param>
        public static void RemoveCache(string CacheKey)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            if (GetCache(CacheKey) != null)
                objCache.Remove(CacheKey);
        }
        /// <summary>
        /// 删除 CacheKey中包含参数key指定内容的所有缓存
        /// </summary>
        /// <param name="key"></param>
        public static void RemoveCacheLike(string key)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            IDictionaryEnumerator enumtor = objCache.GetEnumerator();
            while (enumtor.MoveNext())
            {
                if (enumtor.Key.ToString().Contains(key))
                    objCache.Remove(enumtor.Key.ToString());
            }
        }
        /// <summary>
        /// 删除所有缓存
        /// </summary>
        public static void RemoveAllCache()
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            IDictionaryEnumerator enumtor = objCache.GetEnumerator();
            while (enumtor.MoveNext())
            {
                objCache.Remove(enumtor.Key.ToString());
            }
        }
    }
}
