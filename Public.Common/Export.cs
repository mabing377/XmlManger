using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace Public.Common
{
    public class Export
    {
        public delegate void ColumnsReplace(int row, int col);
        //1.表头需要解决，2.长数字传文本需要解决
        public static void DataTable2Excel2(DataTable dtData, string name, ColumnsReplace cr)
        {
            if (dtData == null) return;
            HttpContext curContext = HttpContext.Current;
            // 设置编码 
            curContext.Response.ContentType = "application/vnd.ms-excel";
            curContext.Response.ContentEncoding = System.Text.Encoding.UTF8;
            curContext.Response.Charset = "GB2312";
            curContext.Response.AppendHeader("Content-Disposition", "attachment;filename=" + System.Web.HttpUtility.UrlEncode(name) + ".xls");

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<html xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:x='urn:schemas-microsoft-com:office:excel'>
                                    <head><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>Sheet1</x:Name>
                                        <x:WorksheetOptions><x:DoNotDisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet>
                                      </x:ExcelWorksheets></x:ExcelWorkbook></xml></head>
                                <body><div><table>");

            sb.Append("<tr>");
            for (int k = 0; k < dtData.Columns.Count; k++)
            {
                sb.Append("<td>");
                sb.Append(dtData.Columns[k].ColumnName);
                sb.Append("</td>");
            }
            sb.Append("</tr>");

            for (int i = 0; i < dtData.Rows.Count; i++)
            {
                sb.Append("<tr>");
                for (int j = 0; j < dtData.Columns.Count; j++)
                {
                    sb.Append("<td>");
                    sb.Append(dtData.Rows[i][j].ToString());
                    cr.Invoke(i, j);
                    sb.Append("</td>");
                }
                sb.Append("</tr>");
            }
            sb.Append("</table></div></body></html>");
            curContext.Response.Write(sb.ToString());
            curContext.Response.End();
        }

        public static byte[] Ilist2Excel(object IlistData, GridViewRowEventHandler gvExport_RowDataBound)
        {
            byte[] RetByteStr = null;
            if (IlistData == null) return RetByteStr;
            GridView dgExport = null;
            // 当前对话 
            System.Web.HttpContext curContext = System.Web.HttpContext.Current;
            // IO用于导出并返回excel文件 
            System.IO.StringWriter strWriter = null;
            System.Web.UI.HtmlTextWriter htmlWriter = null;

            if (IlistData != null)
            {
                // 导出excel文件 
                strWriter = new System.IO.StringWriter();
                htmlWriter = new System.Web.UI.HtmlTextWriter(strWriter);

                // 为了解决dgData中可能进行了分页的情况，需要重新定义一个无分页的DataGrid 
                dgExport = new GridView();
                if (dgExport != null)
                    dgExport.RowDataBound += new GridViewRowEventHandler(gvExport_RowDataBound);

                dgExport.DataSource = IlistData;
                dgExport.AllowPaging = false;
                dgExport.DataBind();
                // 返回客户端 
                string headControlStr = @"<html xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:x='urn:schemas-microsoft-com:office:excel'><head>
                                    <xml>
                                     <x:ExcelWorkbook>
                                      <x:ExcelWorksheets>
                                       <x:ExcelWorksheet>
                                        <x:Name>Sheet1</x:Name>
                                        <x:WorksheetOptions>  
                                         <x:DoNotDisplayGridlines/>
                                        </x:WorksheetOptions>
                                       </x:ExcelWorksheet>
                                      </x:ExcelWorksheets>
                                     </x:ExcelWorkbook>
                                    </xml>
                                    </head>
                                    <body>";
                dgExport.RenderControl(htmlWriter);
                RetByteStr = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(headControlStr + strWriter.ToString() + "</body></html>");
            }
            return RetByteStr;
        }
        /// <summary>
        /// 使用方法:return File(Export.DataTable2Excel(dt, "GB2312", null), "application/x-xls", "3G.xls");
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="encoding">其值为"UTF-8"或"GB2312"</param>
        /// <param name="gvExport_RowDataBound"></param>
        public static byte[] DataTable2Excel(DataTable dt, string encoding, GridViewRowEventHandler gvExport_RowDataBound)
        {
            GridView gvExport = null;
            //IO用于导出并返回Excel文件
            System.IO.StringWriter strWriter = null;
            System.Web.UI.HtmlTextWriter htmlWriter = null;
            byte[] str = null;
            if (dt != null)
            {
                strWriter = new System.IO.StringWriter();
                htmlWriter = new System.Web.UI.HtmlTextWriter(strWriter);
                gvExport = new GridView();
                if (gvExport_RowDataBound != null)
                    gvExport.RowDataBound += new GridViewRowEventHandler(gvExport_RowDataBound);
                gvExport.DataSource = dt.DefaultView;
                gvExport.AllowPaging = false;
                gvExport.DataBind();
                //把文件发送到客户端
                gvExport.RenderControl(htmlWriter);
                str = System.Text.Encoding.GetEncoding(encoding).GetBytes("<meta http-equiv='content-type' content='application/ms-excel; charset=" + encoding + "'/>" + strWriter.ToString());
            }
            return str;
        }
    }
}
