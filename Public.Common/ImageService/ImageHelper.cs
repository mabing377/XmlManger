using System;
using System.Collections.Generic;
using System.Web;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Net;

namespace Public.Common
{
    /// <summary>
    /// 图片操作辅助类
    /// 作者：sunshine
    /// 最后修改时间：2009-11-20
    /// </summary>
    public sealed class ImageHelper
    {

        #region Static Instance

        private ImageHelper() { }
        static ImageHelper() { }

        private static object _lockHelper = new object();
        private static ImageHelper _instance;
        public static ImageHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockHelper)
                    {
                        if (_instance == null)
                        {
                            _instance = new ImageHelper();
                        }
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region Size

        /// <summary>
        /// 计算新尺寸
        /// </summary>
        /// <param name="width">原始宽度</param>
        /// <param name="height">原始高度</param>
        /// <param name="maxWidth">最大新宽度</param>
        /// <param name="maxHeight">最大新高度</param>
        /// <returns></returns>
        private Size ResizeImage(int width, int height, int maxWidth, int maxHeight)
        {
            decimal MAX_WIDTH = (decimal)maxWidth;
            decimal MAX_HEIGHT = (decimal)maxHeight;
            decimal ASPECT_RATIO = MAX_WIDTH / MAX_HEIGHT;//纵横比例

            int newWidth, newHeight;

            decimal originalWidth = (decimal)width;
            decimal originalHeight = (decimal)height;

            if (originalWidth > MAX_WIDTH || originalHeight > MAX_HEIGHT)
            {
                decimal factor;
                // determine the largest factor 
                if (originalWidth / originalHeight > ASPECT_RATIO)
                {
                    factor = originalWidth / MAX_WIDTH;
                    newWidth = Convert.ToInt32(originalWidth / factor);
                    newHeight = Convert.ToInt32(originalHeight / factor);
                }
                else
                {
                    factor = originalHeight / MAX_HEIGHT;
                    newWidth = Convert.ToInt32(originalWidth / factor);
                    newHeight = Convert.ToInt32(originalHeight / factor);
                }
            }
            else
            {
                newWidth = width;
                newHeight = height;
            }

            return new Size(newWidth, newHeight);

        }

        #endregion

        #region Path
        /// <summary>
        /// 根据相对路径获取绝对路径
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public string GetPhysicalPath(string virtualPath)
        {
            return virtualPath;
        }

        /// <summary>
        /// 获取图片保存的路径
        /// </summary>
        /// <param name="extensionName">图片的后缀名</param>
        /// <param name="folderName">指定的文件夹</param>
        /// <returns>图片保存的路径</returns>
        private string GetSavePathOriginal(string extensionName, string folderName, bool isReName, string imageName)
        {
            string Path = string.Format("{0}\\{1}\\{2}\\{3}", folderName, DateTime.Now.Year.ToString("0000"), DateTime.Now.Month.ToString("00"), DateTime.Now.Day.ToString("00"));
            if (!System.IO.Directory.Exists(GetPhysicalPath(Path)))
            {
                System.IO.Directory.CreateDirectory(GetPhysicalPath(Path));
            }
            if (!System.IO.Directory.Exists(GetPhysicalPath(Path)))
            {
                return string.Empty;
            }
            return isReName ? string.Format("{0}\\{1}{2}", Path, Guid.NewGuid().ToString("N"), extensionName) : string.Format("{0}\\{1}", Path, imageName);
        }
        
        #endregion

        #region Cut

        /// <summary>
        /// 剪裁图片 -- 用GDI+
        /// </summary>
        /// <param name="imgUrl">原始BitmapURL地址</param>
        /// <param name="savePath">保存路径（虚拟路径）</param>
        /// <param name="StartX">开始坐标X</param>
        /// <param name="StartY">开始坐标Y</param>
        /// <param name="iWidth">宽度</param>
        /// <param name="iHeight">高度</param>
        /// <returns>剪裁后的Bitmap</returns>
        public string KitCut(string imgUrl, string savePath, int StartX, int StartY, int iWidth, int iHeight)
        {
            Regex reg = new Regex("(http://([^/]*))", RegexOptions.IgnoreCase);
            string imgPath = GetPhysicalPath(reg.Replace(imgUrl, ""));
            if (!System.IO.File.Exists(imgPath))
            {
                return null;
            }
            System.Drawing.Image img = System.Drawing.Image.FromFile(imgPath);
            if (img == null)
            {
                return null;
            }

            int w = img.Width;
            int h = img.Height;

            if (StartX >= w || StartY >= h)
            {
                return null;
            }

            if (StartX + iWidth > w)
            {
                iWidth = w - StartX;
            }

            if (StartY + iHeight > h)
            {
                iHeight = h - StartY;
            }

            try
            {
                //新建一个bmp图片
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(iWidth, iHeight);

                //新建一个画板
                Graphics g = System.Drawing.Graphics.FromImage(bitmap);

                //设置高质量插值法
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                //设置高质量,低速度呈现平滑程度
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;


                //清空画布并以透明背景色填充
                g.Clear(Color.Transparent);

                //在指定位置并且按指定大小绘制原图片的指定部分
                g.DrawImage(img, new Rectangle(0, 0, iWidth, iHeight), new Rectangle(StartX, StartY, iWidth, iHeight), GraphicsUnit.Pixel);
                string extensionName = System.IO.Path.GetExtension(imgPath);

                Encoder myEncoder = Encoder.Quality;
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 95L);
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                ImageCodecInfo myImageCodecInfo = ImageCodecInfo.GetImageEncoders()[0];
                // 在这里设置图片的质量等级为95L. 
                myEncoderParameters.Param[0] = myEncoderParameter;//将构建出来的EncoderParameter类赋给

                bitmap.Save(GetPhysicalPath(savePath), myImageCodecInfo, myEncoderParameters);

                img.Dispose();
                g.Dispose();
                bitmap.Dispose();

                if (System.IO.File.Exists(GetPhysicalPath(savePath)))
                {
                    string ImgUrl = string.Format("http://{0}/{1}", HttpContext.Current.Request.Url.Authority, savePath);
                    return ImgUrl;
                }
                return null;

            }
            catch
            {
                return null;
            }
            finally
            {

            }
        }

        #endregion

        #region 保存图片操作
        
        /// <summary>
        /// 保存图片返回原始图片URL
        /// </summary>
        /// <param name="imgData">图片数据</param>
        /// <param name="extensionName">图片扩展名</param>
        /// <param name="folderName"></param>
        /// <returns>图片存储地址（URL）</returns>
        public string SaveImageToOriginal(byte[] imgData, string extensionName, string folderName, bool isReName, string imageName, string sizes)
        {
            MemoryStream stream = new MemoryStream(imgData);
            Image originalImage = Image.FromStream(stream);
            try
            {
                //保存原始图片
                string savePath = GetSavePathOriginal(extensionName, folderName, isReName, imageName);
                originalImage.Save(GetPhysicalPath(savePath), ImageUtility.GetFormat(extensionName));
                //保存指定尺寸的缩略图
                string thumbnailPath = savePath.Substring(0, savePath.LastIndexOf("."));
                if (sizes.Length > 0)
                {
                    sizes = sizes.EndsWith(",") ? sizes.Remove(sizes.Length - 1) : sizes;
                    string[] tempSizes = sizes.Split(',');
                    if (tempSizes.Length > 0)
                    {
                        foreach (string tempSize in tempSizes)
                        {
                            int width;
                            int height;
                            string newImageName = thumbnailPath + "_" + tempSize.Replace('*', '-') + extensionName;
                            if (int.TryParse(tempSize.Substring(0, tempSize.IndexOf("*")), out width) && int.TryParse(tempSize.Substring(tempSize.IndexOf("*") + 1), out height))
                            {
                                Size size = ResizeImage(originalImage.Width, originalImage.Height, width, height);
                                ImageUtility.ThumbAsJPG(stream, GetPhysicalPath(newImageName), size.Width, size.Height);
                            }
                        }
                    }
                }
                if (System.IO.File.Exists(GetPhysicalPath(savePath)))
                {
                    //http://localhost:29444/\Admin\MyCenter\Framework\BackgroundDev\3_Project\8.JiaHua\Source\ProjectCase\MIS\Module\Seal\Public.MIS.Seal.Web\UpFiles\Img\\2015\04\10\5266d522ffc64575a26c0c8680fc431e.png
                    //http://localhost:29444/UpFiles\Img\\2015\04\10\3aff93fb19344893849bf9d33626c420.png
                    string ImgUrl = string.Format("/{0}", savePath.Substring(savePath.IndexOf("UpFiles\\Img\\")).Replace("\\", "/").Replace("//","/"));
                    //return ImgUrl;
                    //string ImgUrl = string.Format("http://{0}/{1}", HttpContext.Current.Request.Url.Authority, savePath.Substring(2));
                    return ImgUrl;
                }
                return "";
            }
            catch(Exception ex)
            {
                return ex.ToString();
            }
            finally
            {
                originalImage.Dispose();
                stream.Close();
                stream.Dispose();
            }

        }

        /// <summary>
        /// 保存裁切图片
        /// </summary>
        /// <param name="imgUrl">原始图片URL地址</param>
        /// <param name="StartX">开始坐标X</param>
        /// <param name="StartY">开始坐标Y</param>
        /// <param name="iWidth">宽度</param>
        /// <param name="iHeight">高度</param>
        /// <returns>裁切过的图片URL地址</returns>
        public string SaveImageToCut(string imgUrl, int StartX, int StartY, int iWidth, int iHeight)
        {
            //Regex reg = new Regex("(http://([^/]*))", RegexOptions.IgnoreCase);
            //string imgPath = GetPhysicalPath(reg.Replace(imgUrl, ""));
            //string extensionName = System.IO.Path.GetExtension(imgPath);
            //string savePath = GetSavePathCut(extensionName);
            //return KitCut(imgUrl, savePath, StartX, StartY, iWidth, iHeight);
            return "";
        }

        /// <summary>
        /// 保存水印图片（图片水印）
        /// </summary>
        /// <param name="imgData">原始图片数据</param>
        /// <param name="savePath">原始图片的相对路径</param>
        /// <param name="extensionName">图片扩展名</param>
        /// <param name="waterImageName">水印图片地址</param>
        /// <param name="waterImagePosition">水印图片位置</param>
        /// <param name="waterImageTransparency">透明度</param>
        /// <returns>是否生成成功</returns>
        public bool SaveImageToWaterImage(byte[] imgData, string savePath, string extensionName, string waterImageName, string waterImagePosition, string waterImageTransparency)
        {
            MemoryStream stream = new MemoryStream(imgData);
            try
            {
                //保存水印图片
                //保存路径（相对路径）
                string saveWaterImagePath = savePath.Substring(0, savePath.LastIndexOf(".")) + "_WaterImage" + savePath.Substring(savePath.LastIndexOf("."));
                //位置
                ImageUtility.MarkPosition mp = (ImageUtility.MarkPosition)Enum.Parse(typeof(ImageUtility.MarkPosition), waterImagePosition);
                //透明度
                float waterTransparency;
                waterTransparency = float.TryParse(waterImageTransparency, out waterTransparency) ? waterTransparency : (float)0.3;
                if (waterTransparency > 1) waterTransparency = 1;
                ImageUtility.AddPicWatermarkAsJPG(stream, waterImageName, GetPhysicalPath(saveWaterImagePath), mp, waterTransparency);
                if (System.IO.File.Exists(GetPhysicalPath(saveWaterImagePath)))
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }

        }

        /// <summary>
        /// 保存水印图片（文字水印）
        /// </summary>
        /// <param name="imgData">原始图片数据</param>
        /// <param name="savePath">原始图片的相对路径</param>
        /// <param name="extensionName">图片扩展名</param>
        /// <param name="textImageName">水印文字</param>
        /// <param name="textImagePosition">水印文字位置</param>
        /// <param name="textImageTransparency">透明度</param>
        /// <param name="textImageFontFamily">字体</param>
        /// <param name="textImageFontStyle">样式</param>
        /// <returns>是否生成成功</returns>
        public bool SaveImageToWaterText(byte[] imgData, string savePath, string extensionName, string textImageName, string textImagePosition, string textImageTransparency, string textImageFontFamily, string textImageFontStyle)
        {
            MemoryStream stream = new MemoryStream(imgData);
            try
            {
                //保存水印图片
                //保存路径
                string saveWaterImagePath = savePath.Substring(0, savePath.LastIndexOf(".")) + "_WaterText" + savePath.Substring(savePath.LastIndexOf("."));
                //位置
                ImageUtility.MarkPosition mp = (ImageUtility.MarkPosition)Enum.Parse(typeof(ImageUtility.MarkPosition), textImagePosition);
                //透明度
                
                float tempTransparency;
                int waterTransparency = float.TryParse(textImageTransparency, out tempTransparency) ? (int)(tempTransparency * 255) : (int)(0.3 * 255);
                if (waterTransparency > 255) waterTransparency = 255;
                ImageUtility.AddTextWatermarkAsJPG(stream, textImageName, GetPhysicalPath(saveWaterImagePath), mp, textImageFontFamily, textImageFontStyle, waterTransparency);

                if (System.IO.File.Exists(GetPhysicalPath(saveWaterImagePath)))
                {
                    return true;
                }
                return false;
            }
            catch
            {
               return false;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }

        #endregion

        #region 将网上的图片下载到本地
        /// 
        /// 从图片地址下载图片到本地磁盘
        /// 
        /// 图片本地磁盘地址
        /// 图片网址
        /// 
        public bool SaveImageFromUrl(string FileName, string Url)
        {
            bool value = false;
            WebResponse response = null;
            Stream stream = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);

                response = request.GetResponse();
                stream = response.GetResponseStream();

                if (!response.ContentType.ToLower().StartsWith("text/"))
                {
                    value = SaveBinaryFile(response, FileName);
                }

            }
            catch (Exception e)
            {
                string aa = e.ToString();
            }
            return value;
        }

        // 将二进制文件保存到磁盘
        private bool SaveBinaryFile(WebResponse response, string FileName)
        {
            bool Value = true;
            byte[] buffer = new byte[1024];

            try
            {
                if (File.Exists(FileName))
                    return true;
                Stream outStream = System.IO.File.Create(FileName);
                Stream inStream = response.GetResponseStream();

                int l;
                do
                {
                    l = inStream.Read(buffer, 0, buffer.Length);
                    if (l > 0)
                        outStream.Write(buffer, 0, l);
                }
                while (l > 0);

                outStream.Close();
                inStream.Close();
            }
            catch
            {
                Value = false;
            }
            return Value;
        }
 
        #endregion
    }
}
