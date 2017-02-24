/*----------------------------------------------------------------
// Copyright (C) 2012 通通优品
// 版权所有。
//
// 文件名：Singleton.cs
// 功能描述：采用延迟加载（.NET 4.0）实现泛型模式基类
// 
// 创建标识：Star Gu（古红星） 2012.06.25
// 
// 修改标识：参考（http://www.fascinatedwithsoftware.com/blog/post/2011/07/13/A-Generic-Singleton-Class.aspx）
// 修改描述：
//
// 修改标识：
// 修改描述：
//
//----------------------------------------------------------------*/


using System;
using System.Linq;
using System.Reflection;

namespace Public.Common
{
    /// <summary>
    /// 单例模式基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> where T : class
    {
        private static readonly Lazy<T> _instance
          = new Lazy<T>(() =>
          {
              var ctors = typeof(T).GetConstructors(
                  BindingFlags.Instance
                  | BindingFlags.NonPublic
                  | BindingFlags.Public);
              if (ctors.Count() != 1)
                  throw new InvalidOperationException(String.Format("{0} 必须存在一个私有构造函数！", typeof(T)));
              var ctor = ctors.SingleOrDefault(c => c.GetParameters().Count() == 0 && c.IsPrivate);
              if (ctor == null)
                  throw new InvalidOperationException(String.Format("{0} 必须存在一个私有并且不带参数的构造函数！", typeof(T)));
              return (T)ctor.Invoke(null);
          });

        public static T Instance
        {
            get { return _instance.Value; }
        }
    }
}
