/*----------------------------------------------------------------
// Copyright (C) 2012 通通优品
// 版权所有。
//
// 文件名：JscriptOperator.cs
// 功能描述：注册脚本工具库，在服务器端注册客户端的JavaScript脚本。
// 
// 创建标识：Star.Gu(古红星) 2011.04.27
//
// 修改标识：Roc.Lee(李鹏鹏) 2011.05.11
// 修改描述：增加 AjaxAlert 函数
//
// 修改标识：Star Gu(古红星) 2011.07.18
// 修改描述：增加 RegisterStartupScript 函数
// 
// 修改标识：Star Gu(古红星) 2012.06.20
// 修改描述：重构函数名和注释结构
//----------------------------------------------------------------*/
using System;
using System.Text;
using System.Web.UI;
using System.Collections.Generic;

namespace Public.Common
{
    /// <summary>
    /// 注册脚本工具库
    /// </summary>
    public static class JscriptHelper
    {        
        #region 关闭

        #region 关闭当前窗口
        /// <summary>
        /// 关闭当前窗口
        /// </summary>
        /// <param name="page">当前页面的Page对象</param>
        public static void CloseWindow(Page page)
        {
            string strScript = "    top.close();\n";
            page.ClientScript.RegisterClientScriptBlock(page.GetType(), Guid.NewGuid().ToString(), strScript, true);
        }
        #endregion

        #region 关闭当前父窗口
        /// <summary>
        /// 关闭当前父窗口
        /// </summary>
        /// <param name="page">当前页面的Page对象</param>
        public static void CloseParentWindow(Page page)
        {
            string strScript = "    window.parent.close();\n";
            page.ClientScript.RegisterClientScriptBlock(page.GetType(), Guid.NewGuid().ToString(), strScript, true);
        }
        #endregion

        #endregion

        #region 刷新

        #region 回到历史页面
        /// <summary>
        /// 回到历史页面
        /// </summary>
        /// <param name="page">当前页面的Page对象</param>
        /// <param name="value">要访问的 URL 在 History 的 URL 列表中的相对位置。</param>
        public static void RefreshGoHistory(Page page, int value)
        {
            string strScript = "    history.go({0});\n";
            page.ClientScript.RegisterClientScriptBlock(page.GetType(), Guid.NewGuid().ToString(), string.Format(strScript, value), true);
        }
        #endregion

        #region 刷新当前窗口
        /// <summary>
        /// 刷新当前窗口
        /// </summary>
        /// <param name="page">当前页面的Page对象</param>
        public static void RefreshLocation(Page page)
        {
            RefreshLocation(page, null);
        }
        /// <summary>
        /// 刷新当前窗口为指定Url
        /// </summary>
        /// <param name="page">当前页面的Page对象</param>
        public static void RefreshLocation(Page page, string url)
        {
            string strScript = "    opener.location.reload();\n";
            if (url != null)
            {
                strScript = "    top.location='" + url + "';\n";
            }
            page.ClientScript.RegisterClientScriptBlock(page.GetType(), Guid.NewGuid().ToString(), strScript, true);
        }
        #endregion

        #region 刷新父窗口
        /// <summary>
        /// 刷新父窗口
        /// </summary>
        /// <param name="page">当前页面的Page对象</param>
        public static void RefreshParent(Page page)
        {
            RefreshParent(page, null);
        }
        /// <summary>
        /// 刷新父窗口为指定Url
        /// </summary>
        /// <param name="page">当前页面的Page对象</param>
        /// <param name="url">地址</param>
        public static void RefreshParent(Page page, string url)
        {
            string strScript = "    try{top.location.reload()}catch(e){location.reload()}\n";
            if (url != null)
            {
                strScript = "    try{top.location='" + url + "'}catch(e){location='" + url + "'}\n";
            }
            page.ClientScript.RegisterClientScriptBlock(page.GetType(), Guid.NewGuid().ToString(), strScript, true);
        }
        #endregion

        #endregion

        #region 弹出对话框

        #region 弹出消息框
        /// <summary>
        /// 弹出消息框
        /// </summary>
        /// <param name="page">当前页面的Page对象</param>
        /// <param name="description">提示信息</param>
        public static void Alert(Page page, string description)
        {

            description = GetFilterDescription(description);
            string strScript = "    window.alert(\"" + description + "\");\n";
            page.ClientScript.RegisterStartupScript(page.GetType(), Guid.NewGuid().ToString(), strScript, true);
        }

        /// <summary>
        /// 弹出消息框（Asp.net Ajax 脚本注册 ）
        /// </summary>
        /// <param name="page">当前页面的Page对象</param>
        /// <param name="updateControl">UpdatePanel 控件实例</param>
        /// <param name="description">提示信息</param>
        public static void AjaxAlert(Page page, Control updateControl, string description)
        {
            ScriptManager.RegisterStartupScript(updateControl, typeof(UpdatePanel), Guid.NewGuid().ToString(), string.Format("alert('{0}');", GetFilterDescription(description)), true);
        }
        #endregion

        #region 弹出消息框并且转向到新的URL
        /// <summary>
        /// 弹出消息框并且转向到新的URL
        /// </summary>
        /// <param name="page">当前页面的Page对象</param>
        /// <param name="description">提示信息</param>
        /// <param name="url">地址</param>
        public static void AlertAndRedirect(Page page, string description, string url)
        {
            description = GetFilterDescription(description);
            string strScript = "    alert('{0}');window.location.replace('{1}');\n";
            page.ClientScript.RegisterClientScriptBlock(page.GetType(), Guid.NewGuid().ToString(), string.Format(strScript, description, url), true);
        }
        #endregion

        #region 弹出消息框并且转向到新的URL（框架）
        /// <summary>
        /// 弹出消息框并且转向到新的URL（框架）
        /// </summary>
        /// <param name="page">当前页面的Page对象</param>
        /// <param name="description">提示信息</param>
        /// <param name="url">地址</param>
        public static void AlertAndTopRedirect(System.Web.UI.Page page, string description, string url)
        {
            description = GetFilterDescription(description);
            string strScript = "    alert('{0}');try{{window.parent.location='{1}'}}catch(e){{location='{1}'}};\n";
            page.ClientScript.RegisterClientScriptBlock(page.GetType(), Guid.NewGuid().ToString(), string.Format(strScript, description, url), true);
        }
        #endregion

        #region 弹出信息并关闭窗口
        /// <summary>
        /// 弹出信息并关闭窗口
        /// </summary>
        /// <param name="page">当前页面的Page对象</param>
        /// <param name="description">提示信息</param>
        public static void AlertAndClose(System.Web.UI.Page page, string description)
        {
            description = GetFilterDescription(description);
            string strScript = "    alert('{0}');window.close();\n";
            page.ClientScript.RegisterClientScriptBlock(page.GetType(), Guid.NewGuid().ToString(), string.Format(strScript, description), true);

        }
        #endregion

        #endregion

        #region 模式窗口
        /// <summary>
        /// 根据Url打开showModalDialog窗口，需要指定宽和高。
        /// </summary>
        /// <param name="page">当前页面的Page对象</param>
        /// <param name="url">链接</param>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        public static void OpenDialog(Page page, string url, int width, int height)
        {
            string strScript = "    window.showModalDialog(\"" + url + "\",null,\"" + string.Format("dialogWidth:{0}px;dialogHeight:{1}px;help:no;unadorned:yes;resizable:yes;status:no", width, height) + "\");\n";
            page.ClientScript.RegisterClientScriptBlock(page.GetType(), Guid.NewGuid().ToString(), strScript, true);
        }
        #endregion

        #region 打开IE窗口
        /// <summary>
        /// 打开IE窗口(无标题栏、工具栏、地址栏等）
        /// </summary>
        /// <param name="page">当前页面的Page对象</param>
        /// <param name="url">链接</param>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        public static void OpenWindow(Page page, string url, int width, int height)
        {
            string parValues = string.Format("width={0},height={1},directories=no,location=no,menubar=no,status=no,toolbar=no,resizable=yes", width, height);
            string strScript = "    window.open(\"" + url + "\",null,\"" + parValues + "\");\n";
            page.ClientScript.RegisterClientScriptBlock(page.GetType(), Guid.NewGuid().ToString(), strScript, true);
        }
        #endregion

        #region 脚本注册

        #region 在form结束之前注册脚本
        /// <summary>
        /// 在“<form runat= server>”元素的结束标记之前注册该脚本</summary>
        /// <param name="page">当前页面</param>
        /// <param name="strScript">注册脚本，不用包含<script language=javascript>。</param>
        public static void RegisterStartupScript(System.Web.UI.Page page, string strScript)
        {
            page.ClientScript.RegisterStartupScript(page.GetType(), Guid.NewGuid().ToString(), strScript, true);
        }
        #endregion

        #endregion

        #region 内部函数
        /// <summary>
        /// 过滤用户输入的信息
        /// </summary>
        /// <param name="description">输入信息</param>
        /// <returns>过滤后的字符串</returns>
        private static string GetFilterDescription(string description)
        {
            if (string.IsNullOrEmpty(description))
                return string.Empty;
            //先将提示信息中的某些字符做转换，否则会影响脚本的执行。
            description = description.Replace("\"", "\\\"");
            description = description.Replace("\\", "\\\\");
            description = description.Replace("\r", "\\r");
            description = description.Replace("\n", "\\n");
            description = description.Replace("'", "\'");
            return description;
        }
        #endregion
    }
}