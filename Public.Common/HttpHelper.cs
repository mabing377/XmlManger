/*----------------------------------------------------------------
// Copyright (C) 2012 通通优品
// 版权所有。
//
// 文件名：HttpHelper.cs
// 功能描述：HTTP 工具库
// 
// 创建标识：Public 2012.06.01
// 
// 修改标识：Star.Gu(古红星) 2012.09.10
// 修改描述：
//
// 修改标识：
// 修改描述：
//
//----------------------------------------------------------------*/

using System;
using System.Web;
using System.Net;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Public.Common
{
    /// <summary>
    /// HTTP 工具库
    /// </summary>
    public class HttpHelper
    {
        #region 获取 HTTP 查询字符串变量值

        #region 获取查询字符串数据
        /// <summary>
        /// 获取查询字符串数据
        /// </summary>
        /// <typeparam name="T">转换后的数据类型</typeparam>
        /// <param name="queryKey">变量名称</param>
        /// <param name="defaultValue">转换或获取失败后的默认值</param>
        /// <returns></returns>
        public static T GetQuery<T>(string queryKey, T defaultValue)
        {
            return GetContext().Request.QueryString[queryKey].ConvertTo<T>(defaultValue);
        }
        #endregion

        #region 获取窗体变量数据
        /// <summary>
        /// 获取窗体变量数据
        /// </summary>
        /// <typeparam name="T">转换后的数据类型</typeparam>
        /// <param name="queryKey">变量名称</param>
        /// <param name="defaultValue">转换或获取失败后的默认值</param>
        /// <returns></returns>
        public static T GetForm<T>(string queryKey, T defaultValue)
        {
            return GetContext().Request.Form[queryKey].ConvertTo<T>(defaultValue);
        }
        #endregion

        #endregion

        #region 获得访问当前页面客户端的IP，获取失败返回“errorIP:{ip}”。
        /// <summary>
        /// 获得访问当前页面客户端的IP，获取失败返回“errorIP:{ip}”。
        /// </summary>
        /// <returns>字符串格式的IP值，形式如“127.0.0.1”</returns>
        public static string GetClientIP()
        {
            string result = String.Empty;
            result = GetServerVariableString("HTTP_X_FORWARDED_FOR");
            if (string.IsNullOrEmpty(result))
            {
                result = GetServerVariableString("REMOTE_ADDR");
            }
            if (string.IsNullOrEmpty(result))
            {
                result = GetContext().Request.UserHostAddress;
            }
            if (string.IsNullOrEmpty(result) || !StringHelper.IsIP(result))
            {
                return string.Format("errorIP:{0}", result);
            }
            return result;
        }
        #endregion

        #region 返回指定的服务器变量信息
        /// <summary>
        /// 返回指定的服务器变量信息
        /// </summary>
        /// <param name="serverKey">服务器变量名</param>
        /// <returns>服务器变量信息</returns>
        public static string GetServerVariableString(string serverKey)
        {
            return GetContext().Request.ServerVariables[serverKey];
        }
        #endregion

        #region 获取有关客户端上次请求的 URL 的信息，该请求链接到当前的 URL。
        /// <summary>
        /// 获取有关客户端上次请求的 URL 的信息，该请求链接到当前的 URL。
        /// </summary>
        /// <returns>上一个页面的地址,没有则返回null</returns>
        public static string GetUrlReferrer()
        {
            return GetContext().Request.Headers["Referer"] ?? string.Empty;
        }
        #endregion

        #region 判断访问着是否来自搜索引擎
        /// <summary>
        /// 判断访问着是否来自搜索引擎
        /// 目前判断的搜索引擎有："google", "yahoo", "msn", "baidu", "sogou", "sohu", "sina", "163", "lycos", "tom", "yisou", "iask", "soso", "gougou", "zhongsou","bing","youdao"
        /// </summary>
        /// <returns>【true】为来自搜索引擎，【false】为非来自搜索引擎</returns>
        public static bool IsVisitFromSearchEngines()
        {
            string tmpReferrer = GetUrlReferrer();
            if (tmpReferrer == null)
            {
                return false;
            }
            tmpReferrer = tmpReferrer.ToLower();
            string[] SearchEngine = { "google", "yahoo", "msn", "baidu", "sogou", "sohu", "sina", "163", "lycos", "tom", "yisou", "iask", "soso", "gougou", "zhongsou", "bing", "youdao" };

            for (int i = 0; i < SearchEngine.Length; i++)
            {
                if (tmpReferrer.IndexOf(SearchEngine[i]) >= 0)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region 判断访问着是否来自网络爬虫
        /// <summary>
        /// 判断访问着是否来自网络爬虫
        /// </summary>
        /// <returns>【true】为来自网络爬虫，【false】为非来自网络爬虫</returns>
        public static bool IsVisitFromNetSpider()
        {
            string userAgent = GetContext().Request.UserAgent;
            if (!string.IsNullOrWhiteSpace(userAgent))
            {
                string[] keywords = { "spider", "bot"/*, "Baiduspider", "Googlebot","iaskspider" */};
                for (int i = 0; i < keywords.Length; i++)
                {
                    if (userAgent.IndexOf(keywords[i]) >= 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion

        #region 获取 HttpContext
        /// <summary>
        /// 获取当前访问请求的HttpContext
        /// </summary>
        /// <returns></returns>
        public static HttpContext GetContext()
        {
            HttpContext context = HttpContext.Current;
            if (context == null)
            {
                throw new Exception("HttpContext 未找到");
            }
            return context;
        }
        #endregion

        #region 获取web服务器上的虚拟路径，如果是物理路径（包括右斜杠“\”）则不转换
        /// <summary>
        /// 获取web服务器上的虚拟路径，如果是物理路径（包括右斜杠“\”）则不转换
        /// </summary>
        /// <returns></returns>
        public static string GetPhyPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                path = GetContext().Server.MapPath("~/");
            }
            else if (!path.Contains("\\"))
            {
                path = GetContext().Server.MapPath(path);
            }
            return path;
        }
        #endregion

        #region 获得当前URL第一部分
        /// <summary>
        /// 获得当前URL第一部分(如：Http://www.baidu.com)
        /// </summary>
        /// <returns>http://www.xxx.com</returns>
        public static string GetUrlBefore()
        {
            return GetContext().Request.Url.GetLeftPart(UriPartial.Authority);
        }
        #endregion

        #region 获得当前URL第一个问号后的内容
        /// <summary>
        /// 获得当前URL第一个问号后的内容"http://www.xx.com/a.aspx?a=b&c=d">>"a=b&c=d"
        /// </summary>
        /// <returns>参数信息</returns>
        public static string GetUrlParameter()
        {
            string rawUrl = GetUrlReferrer();
            if (!string.IsNullOrEmpty(rawUrl) && rawUrl.IndexOf('?') > 0)
            {
                return rawUrl.Substring(rawUrl.IndexOf('?') + 1);
            }
            return null;
        }
        #endregion

        #region 获得当前URL页面的名称(最后一个"/"后面的内容)
        /// <summary>
        /// 获得当前URL页面的名称(最后一个"/"后面的内容)
        /// </summary>
        /// <returns>当前页面的名称</returns>
        public static string GetUrlPageName()
        {
            string[] urlArr = GetContext().Request.Url.AbsolutePath.Split('/');
            return urlArr[urlArr.Length - 1].ToLower();
        }
        #endregion

        #region 获取当前请求的原始 URL(URL 中域信息之后的部分,包括查询字符串(如果存在))
        /// <summary>
        /// 获取当前请求的原始 URL中域信息之后的部分
        /// 未处理过的URL,相对路径。如/home?id=8
        /// </summary>
        /// <returns>原始 URL</returns>
        public static string GetRawUrl()
        {
            return GetContext().Request.RawUrl;
        }
        #endregion

        #region 客户端缓存

        #region GetSetClientCache
        /// <summary>
        /// 检查设置客户端缓存Last-Modified
        /// </summary>
        /// <param name="lastModified">设置 Last-Modified HTTP 标头</param>
        /// <returns>true:已缓存（返回304）</returns>
        public static bool GetSetClientCache(DateTime lastModified)
        {
            HttpContext httpContext = GetContext();

            //判断最后修改时间是否在要求的时间内 
            //如果服务器端的文件没有被修改过，则返回状态是304，内容为空，这样就节省了传输数据量。如果服务器端的文件被修改过，则返回和第一次请求时类似。 
            //if (httpContext.Request.Headers["If-Modified-Since"] != null && TimeSpan.FromTicks(DateTime.Now.Ticks - DateTime.Parse(httpContext.Request.Headers["If-Modified-Since"]).Ticks).Seconds < secondsTime)
            if (httpContext.Request.Headers["If-Modified-Since"] != null && httpContext.Request.Headers["If-Modified-Since"].ConvertTo<DateTime>() == lastModified)
            {
                httpContext.Response.StatusCode = 304;
                //filterContext.HttpContext.Response.Headers.Add("Content-Encoding", "gzip");
                httpContext.Response.StatusDescription = "Not Modified";
                return true;
            }
            else
            {
                //设置客户端缓存状态 1 
                SetClientCaching(lastModified);
            }
            return false;
        }

        /// <summary>
        /// 检查设置客户端缓存 ETag
        /// </summary>
        /// <param name="etag">etag字符串</param>
        /// <returns>true:已缓存（返回304）</returns>
        public static bool GetSetClientCacheByETag(string etag)
        {
            if (string.IsNullOrWhiteSpace(etag))
                return false;
            HttpContext httpContext = GetContext();

            //ETag/If-None-Match
            if (httpContext.Request.Headers["If-None-Match"] != null && httpContext.Request.Headers["If-None-Match"] == etag)
            {
                httpContext.Response.StatusCode = 304;
                //httpContext.Response.Headers.Add("Content-Encoding", "gzip");
                httpContext.Response.StatusDescription = "Not Modified";
                return true;
            }
            else
            {
                //设置客户端缓存状态 1 
                SetClientCachingByETag(etag);
            }
            return false;
        }
        #endregion

        #region SetClientCaching
        /// <summary>
        /// 设置lastModified值
        /// </summary>
        /// <param name="lastModified">lastModified</param>
        public static void SetClientCaching(DateTime lastModified)
        {
            HttpResponse response = GetContext().Response;
            response.Cache.SetLastModified(lastModified);
            //public以指定响应能由客户端和共享（代理）缓存进行缓存。 
            response.Cache.SetCacheability(HttpCacheability.Public);
        }
        /// <summary>
        /// 设置ETag值
        /// </summary>
        /// <param name="etag">ETag</param>
        public static void SetClientCachingByETag(string etag)
        {
            if (string.IsNullOrWhiteSpace(etag))
                return;
            HttpResponse response = GetContext().Response;
            response.Cache.SetETag(etag);
            //public 以指定响应能由客户端和共享（代理）缓存进行缓存。 
            response.Cache.SetCacheability(HttpCacheability.Public);
        }
        /// <summary>
        /// 设置maxAge
        /// </summary>
        /// <param name="maxAge">maxAge</param>
        public static void SetClientMaxAge(TimeSpan maxAge)
        {
            HttpResponse response = GetContext().Response;
            response.Cache.SetMaxAge(maxAge);
            //public 以指定响应能由客户端和共享（代理）缓存进行缓存。 
            response.Cache.SetCacheability(HttpCacheability.Public);
        }
        #endregion

        #region SetFileCaching
        /// <summary> 
        /// 基于文件方式设置客户端缓存 
        /// </summary> 
        /// <param name="fileName"></param> 
        public static void SetFileCaching(string fileName)
        {
            HttpResponse response = GetContext().Response;
            response.AddFileDependency(fileName);
            //基于处理程序文件依赖项的时间戳设置 ETag HTTP 标头。 
            response.Cache.SetETagFromFileDependencies();
            //基于处理程序文件依赖项的时间戳设置 Last-Modified HTTP 标头。 
            response.Cache.SetLastModifiedFromFileDependencies();
            response.Cache.SetCacheability(HttpCacheability.Public);
            response.Cache.SetMaxAge(new TimeSpan(7, 0, 0, 0));
            response.Cache.SetSlidingExpiration(true);
        }
        #endregion

        #endregion

        #region 构造HTTP请求

        #region POST
        /// <summary>
        /// HTTP-POST方式请求数据
        /// 等同于表单属性enctype设置为application/x-www-form-urlencoded
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="param">key1=value1</param>
        /// <returns>响应的body部分</returns>
        public static string HttpPost(string url, string param)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 150000;
            request.AllowAutoRedirect = false;

            StreamWriter requestStream = null;
            WebResponse response = null;
            string responseStr = null;

            try
            {
                requestStream = new StreamWriter(request.GetRequestStream());
                requestStream.Write(param);
                requestStream.Close();

                response = request.GetResponse();
                if (response != null)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    responseStr = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch (WebException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                request = null;
                requestStream = null;
                response = null;
            }
            return responseStr;
        }

        public static string HttpPost(string url, byte[] bData)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 150000;
            request.AllowAutoRedirect = false;
            request.ContentLength = bData.Length;

            //StreamWriter requestStream = null;
            WebResponse response = null;
            string responseStr = null;

            try
            {
                System.IO.Stream smWrite = request.GetRequestStream();
                smWrite.Write(bData, 0, bData.Length);
                smWrite.Close();

                response = request.GetResponse();
                if (response != null)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    responseStr = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch (WebException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                request = null;
                //requestStream = null;
                response = null;
            }
            return responseStr;
        }
        #endregion

        #region Get
        /// <summary>
        /// HTTP GET方式请求数据.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <returns>响应的body部分</returns>
        public static string HttpGet(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            request.Accept = "*/*";
            request.Timeout = 150000;
            request.AllowAutoRedirect = false;

            WebResponse response = null;
            string responseStr = null;

            try
            {
                response = request.GetResponse();

                if (response != null)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    responseStr = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch (WebException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                request = null;
                response = null;
            }
            return responseStr;
        }
        #endregion

        #region Post By multipart/form-data
        /// <summary>
        /// HTTP POST方式请求数据(包含文件)
        /// 等同于表单属性enctype设置为multipart/form-data
        /// </summary>
        /// <param name="url">URL</param>        
        /// <param name="param">POST的数据</param>
        /// <param name="fileByte">文件二进制数据</param>
        /// <param name="fileName">上传的文件名，默认为"pic"</param>
        /// <param name="contentType">上传文件的MIME信息，默认为"text/plain"</param>
        /// <returns>响应的body部分</returns>
        public static string HttpPost(string url, IDictionary<string, string> param, byte[] fileByte, string fileParamName = "pic", string fileName = "postpic", string contentType = "text/plain")
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = System.Net.CredentialCache.DefaultCredentials;

            Stream rs = wr.GetRequestStream();
            string responseStr = null;

            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in param.Keys)
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, key, param[key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }
            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = string.Format(headerTemplate, fileParamName, fileName, contentType);
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            rs.Write(fileByte, 0, fileByte.Length);

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            WebResponse wresp = null;
            try
            {
                wresp = wr.GetResponse();
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                responseStr = reader2.ReadToEnd();
            }
            catch (WebException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                wr = null;
                wresp = null;
            }
            return responseStr;
        }
        #endregion

        #endregion

        #region 获取或生成抵御CSRF攻击的Token并自动放入cookie
        private const string CsrfCookieName = "csrf-token";

        /// <summary>
        /// 获取或生成抵御CSRF攻击的Token并自动放入cookie
        /// </summary>
        /// <param name="cookieDomain">用于存放token的cookie的domain，为空表示当前域</param>
        /// <returns></returns>
        public static string GetCsrfToken(string cookieDomain)
        {
            HttpContext httpContext = GetContext();
            var csrfCookie = httpContext.Request.Cookies[CsrfCookieName];
            if (csrfCookie == null || string.IsNullOrWhiteSpace(csrfCookie.Value))
            {
                csrfCookie = httpContext.Response.Cookies[CsrfCookieName];
                csrfCookie = csrfCookie != null ? csrfCookie : new HttpCookie(CsrfCookieName);
                if (string.IsNullOrWhiteSpace(csrfCookie.Value))
                {
                    csrfCookie.Value = Guid.NewGuid().ToString("N").GetMD5String();//token保证足够随机即可
                    csrfCookie.HttpOnly = true;
                    if (!string.IsNullOrWhiteSpace(cookieDomain))
                    {
                        csrfCookie.Domain = cookieDomain;
                    }
                }
            }
            return csrfCookie.Value;
        }

        /// <summary>
        /// 用于校验请求的CSRF-Token值是否正确。
        /// 比对请求Header["X-CSRF-Token"]或Form["csrf-token"]的值是否与Cookie中的Token值一致
        /// </summary>
        /// <param name="cookieDomain">用于存放token的cookie的domain</param>
        /// <returns></returns>
        public static bool ValidateCsrfToken(string cookieDomain,  string headkey = "X-CSRF-Token", string formkey = "csrf-token", string queryKey = "token")
        {
            HttpContext  httpContext = GetContext();
            string csrfToken = GetCsrfToken(cookieDomain);
            string requestCsrfToken = httpContext.Request.Headers[headkey];
            if (string.IsNullOrWhiteSpace(requestCsrfToken))
            {
                requestCsrfToken = httpContext.Request.Form[formkey];
            } 
            
            if (string.IsNullOrWhiteSpace(requestCsrfToken))
            {
                requestCsrfToken = httpContext.Request.QueryString[queryKey];
            }
            return string.Compare(csrfToken, requestCsrfToken, true) == 0;
        }
        #endregion

        #region 获取或生成跟踪标识并自动放入Cookie

        private const string TraceCodeCookieName = "UID";
        /// <summary>
        /// 获取或生成跟踪标识并自动放入Cookie
        /// 跟踪标识有效期为永久，只要用户不清楚缓存则一直有效
        /// </summary>
        /// <param name="cookieDomain">用于存放token的cookie的domain，为空表示当前域</param>
        /// <returns></returns>
        public static Guid GetTraceCode(string cookieDomain)
        {
            HttpContext httpContext = GetContext();
            Guid traceCode;
            var traceCookie = httpContext.Request.Cookies[TraceCodeCookieName];
            if (traceCookie == null
                || string.IsNullOrWhiteSpace(traceCookie.Value)
                || !Guid.TryParse(traceCookie.Value, out traceCode)
            )
            {
                traceCookie = httpContext.Response.Cookies[TraceCodeCookieName];
                traceCookie = traceCookie != null ? traceCookie : new HttpCookie(TraceCodeCookieName);
                if (string.IsNullOrWhiteSpace(traceCookie.Value) || !Guid.TryParse(traceCookie.Value, out traceCode))
                {
                    traceCode = Guid.NewGuid();
                    traceCookie.Value = traceCode.ToString("N");//token保证足够随机即可
                    traceCookie.HttpOnly = true;
                    traceCookie.Expires = DateTime.Now.AddYears(1);
                    if (!string.IsNullOrWhiteSpace(cookieDomain))
                    {
                        traceCookie.Domain = cookieDomain;
                    }
                }
            }
            return traceCode;
        }
        #endregion
    }
}
