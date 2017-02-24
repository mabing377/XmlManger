using System;
using System.Collections.Generic;
using System.Web;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Configuration;

namespace Public.Common
{
    public class ImageUtility
    {
        #region 工具

        /// <summary>
        /// 获取指定mimeType的ImageCodecInfo
        /// </summary>
        private static ImageCodecInfo GetImageCodecInfo(string mimeType)
        {
            ImageCodecInfo[] CodecInfo = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo ici in CodecInfo)
            {
                if (ici.MimeType == mimeType) return ici;
            }
            return null;
        }

        /// <summary>
        ///  获取inputStream中的Bitmap对象
        /// </summary>
        public static Bitmap GetBitmapFromStream(Stream inputStream)
        {
            Bitmap bitmap = new Bitmap(inputStream);
            return bitmap;
        }

        /// <summary>
        /// 得到图片格式
        /// </summary>
        /// <param name="name">文件名称</param>
        /// <returns></returns>
        public static ImageFormat GetFormat(string name)
        {
            switch (name.ToLower().Replace(".", ""))
            {
                case "jpg":
                case "jpeg":
                    return ImageFormat.Jpeg;
                case "bmp":
                    return ImageFormat.Bmp;
                case "png":
                    return ImageFormat.Png;
                case "gif":
                    return ImageFormat.Gif;
                default:
                    return ImageFormat.Jpeg;
            }
        }

        /// <summary>
        /// 将Bitmap对象压缩为JPG图片类型
        /// </summary>
        /// <param name="bmp">源bitmap对象</param>
        /// <param name="saveFilePath">目标图片的存储地址</param>
        /// <param name="quality">压缩质量，越大照片越清晰，推荐80</param>
        public static void CompressAsJPG(Bitmap bmp, string saveFilePath, int quality)
        {
            EncoderParameter p = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality); ;
            EncoderParameters ps = new EncoderParameters(1);
            ps.Param[0] = p;
            bmp.Save(saveFilePath, GetImageCodecInfo("image/jpeg"), ps);
            bmp.Dispose();
        }

        /// <summary>
        /// 将inputStream中的对象压缩为JPG图片类型
        /// </summary>
        /// <param name="inputStream">源Stream对象</param>
        /// <param name="saveFilePath">目标图片的存储地址</param>
        /// <param name="quality">压缩质量，越大照片越清晰，推荐80</param>
        public static void CompressAsJPG(Stream inputStream, string saveFilePath, int quality)
        {
            Bitmap bmp = GetBitmapFromStream(inputStream);
            CompressAsJPG(bmp, saveFilePath, quality);
        }

        #endregion
        
        #region 图片生成缩略图

        /// <summary>
        /// 生成缩略图（JPG 格式）
        /// </summary>
        /// <param name="inputStream">包含图片的Stream</param>
        /// <param name="saveFilePath">目标图片的存储地址</param>
        /// <param name="width">缩略图的宽</param>
        /// <param name="height">缩略图的高</param>
        public static void ThumbAsJPG(Stream inputStream, string saveFilePath, int width, int height)
        {
            #region 按要求的尺寸生成缩略图(如果图片宽高没有达到预定宽高，填充背景颜色为白色)

            //Image imageFrom = Image.FromStream(inputStream);
            //if (imageFrom.Width == width && imageFrom.Height == height)
            //{
            //    CompressAsJPG(inputStream, saveFilePath, 80);
            //}
            //// 源图宽度及高度
            //int imageFromWidth = imageFrom.Width;
            //int imageFromHeight = imageFrom.Height;
            //// 生成的缩略图实际宽度及高度
            //int bitmapWidth = width;
            //int bitmapHeight = height;
            //// 生成的缩略图在上述"画布"上的位置
            //int X = 0;
            //int Y = 0;
            //// 根据源图及欲生成的缩略图尺寸,计算缩略图的实际尺寸及其在"画布"上的位置
            //if (bitmapHeight * imageFromWidth > bitmapWidth * imageFromHeight)
            //{
            //    bitmapHeight = imageFromHeight * width / imageFromWidth;
            //    Y = (height - bitmapHeight) / 2;
            //}
            //else
            //{
            //    bitmapWidth = imageFromWidth * height / imageFromHeight;
            //    X = (width - bitmapWidth) / 2;
            //}
            //// 创建画布
            //Bitmap bmp = new Bitmap(width, height);
            //Graphics g = Graphics.FromImage(bmp);
            //// 用白色清空
            //g.Clear(Color.White);
            //// 指定高质量的双三次插值法。执行预筛选以确保高质量的收缩。此模式可产生质量最高的转换图像。
            //g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //// 指定高质量、低速度呈现。
            //g.SmoothingMode = SmoothingMode.HighQuality;
            //// 在指定位置并且按指定大小绘制指定的 Image 的指定部分。
            //g.DrawImage(imageFrom, new Rectangle(X, Y, bitmapWidth, bitmapHeight), new Rectangle(0, 0, imageFromWidth, imageFromHeight), GraphicsUnit.Pixel);
            //try
            //{
            //    //经测试 .jpg 格式缩略图大小与质量等最优
            //    CompressAsJPG(bmp, saveFilePath, 80);
            //}
            //catch
            //{
            //}
            //finally
            //{
            //    //显示释放资源
            //    imageFrom.Dispose();
            //    bmp.Dispose();
            //    g.Dispose();
            //}

            #endregion

            #region 按要求的尺寸生成缩略图(如果图片宽高没有达到预定宽高，不填充背景颜色)

            Image image = Image.FromStream(inputStream);
            if (image.Width == width && image.Height == height)
            {
                CompressAsJPG(inputStream, saveFilePath, 80);
            }
            int tWidth, tHeight, tLeft, tTop;
            double fScale = (double)height / (double)width;
            if (((double)image.Width * fScale) > (double)image.Height)
            {
                tWidth = width;
                tHeight = (int)((double)image.Height * (double)tWidth / (double)image.Width);
                tLeft = 0;
                tTop = (height - tHeight) / 2;
            }
            else
            {
                tHeight = height;
                tWidth = (int)((double)image.Width * (double)tHeight / (double)image.Height);
                tLeft = (width - tWidth) / 2;
                tTop = 0;
            }
            if (tLeft < 0) tLeft = 0;
            if (tTop < 0) tTop = 0;

            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(bitmap);

            //可以在这里设置填充背景颜色
            graphics.Clear(Color.White);
            // 指定高质量的双三次插值法。执行预筛选以确保高质量的收缩。此模式可产生质量最高的转换图像。
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            // 指定高质量、低速度呈现。
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.DrawImage(image, new Rectangle(tLeft, tTop, tWidth, tHeight));
            image.Dispose();
            try
            {
                CompressAsJPG(bitmap, saveFilePath, 80);
            }
            catch
            {
                ;
            }
            finally
            {
                bitmap.Dispose();
                graphics.Dispose();
            }

            #endregion
            
        }

        /// <summary>
        /// 生成缩略图（JPG 格式）
        /// </summary>
        /// <param name="pathImageFrom">原图片的存储地址</param>
        /// <param name="pathImageTo">目标图片的存储地址</param>
        /// <param name="width">缩略图的宽</param>
        /// <param name="height">缩略图的高</param>
        public static void ThumbAsJPG(string pathImageFrom, string pathImageTo, int width, int height)
        {
            if (File.Exists(pathImageFrom))
            {
                using (StreamReader sr = new StreamReader(pathImageFrom))
                {
                    ThumbAsJPG(sr.BaseStream, pathImageTo, width, height);
                }
            }
        }

        #endregion

        #region 裁切图片

        /// <summary>
        /// 将Bitmap对象裁剪为指定JPG文件
        /// </summary>
        /// <param name="bmp">源bmp对象</param>
        /// <param name="saveFilePath">目标图片的存储地址</param>
        /// <param name="x">开始坐标x，单位：像素</param>
        /// <param name="y">开始坐标y，单位：像素</param>
        /// <param name="width">宽度：像素</param>
        /// <param name="height">高度：像素</param>
        public static void CutAsJPG(Bitmap bmp, string saveFilePath, int x, int y, int width, int height)
        {
            int bmpW = bmp.Width;
            int bmpH = bmp.Height;

            if (x >= bmpW || y >= bmpH)
            {
                CompressAsJPG(bmp, saveFilePath, 80);
                return;
            }

            if (x + width > bmpW)
            {
                width = bmpW - x;
            }

            if (y + height > bmpH)
            {
                height = bmpH - y;
            }

            Bitmap bmpOut = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(bmpOut);
            g.DrawImage(bmp, new Rectangle(0, 0, width, height), new Rectangle(x, y, width, height), GraphicsUnit.Pixel);
            g.Dispose();
            bmp.Dispose();
            CompressAsJPG(bmpOut, saveFilePath, 80);
        }

        /// <summary>
        /// 裁剪指定的图片为指定的JPG文件
        /// </summary>
        /// <param name="inputStream">源bmp对象</param>
        /// <param name="saveFilePath">目标图片的存储地址</param>
        /// <param name="x">开始坐标x，单位：像素</param>
        /// <param name="y">开始坐标y，单位：像素</param>
        /// <param name="width">宽度：像素</param>
        /// <param name="height">高度：像素</param>
        public static void CutAsJPG(Stream inputStream, string saveFilePath, int x, int y, int width, int height)
        {
            Bitmap bmp = GetBitmapFromStream(inputStream);
            CutAsJPG(bmp, saveFilePath, x, y, width, height);
        }

        /// <summary>
        /// 裁剪指定的图片为指定的JPG文件
        /// </summary>
        /// <param name="pathImageFrom">原图片的存储地址</param>
        /// <param name="saveFilePath">目标图片的存储地址</param>
        /// <param name="x">开始坐标x，单位：像素</param>
        /// <param name="y">开始坐标y，单位：像素</param>
        /// <param name="width">宽度：像素</param>
        /// <param name="height">高度：像素</param>
        public static void CutAsJPG(string pathImageFrom, string saveFilePath, int x, int y, int width, int height)
        {
            if (File.Exists(pathImageFrom))
            {
                using (StreamReader sr = new StreamReader(pathImageFrom))
                {
                    CutAsJPG(sr.BaseStream, saveFilePath, x, y, width, height);
                }
            }
        }

        #endregion

        #region 图片水印操作

        /// <summary>
        /// 给图片添加图片水印
        /// </summary>
        /// <param name="inputStream">包含要源图片的流</param>
        /// <param name="watermarkPath">水印图片的物理地址</param>
        /// <param name="saveFilePath">目标图片的存储地址</param>
        /// <param name="mp">水印位置</param>
        /// <param name="waterTransparency">水印图片的透明度（为null时表示默认的0.3）</param>
        public static void AddPicWatermarkAsJPG(Stream inputStream, string watermarkPath, string saveFilePath, MarkPosition mp, float? waterTransparency)
        {

            Image image = Image.FromStream(inputStream);
            Bitmap b = new Bitmap(image.Width, image.Height, PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(b);
            g.Clear(Color.White);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(image, 0, 0, image.Width, image.Height);

            AddWatermarkImage(g, watermarkPath, mp, image.Width, image.Height, waterTransparency);

            try
            {
                CompressAsJPG(b, saveFilePath, 80);
            }
            catch { ;}
            finally
            {
                b.Dispose();
                image.Dispose();
            }
        }

        /// <summary>
        /// 给图片添加图片水印
        /// </summary>
        /// <param name="sourcePath">源图片的存储地址</param>
        /// <param name="watermarkPath">水印图片的物理地址</param>
        /// <param name="saveFilePath">目标图片的存储地址</param>
        /// <param name="mp">水印位置</param>
        /// <param name="waterTransparency">水印图片的透明度（为null时表示默认的0.3）</param>
        public static void AddPicWatermarkAsJPG(string sourcePath, string watermarkPath, string saveFilePath, MarkPosition mp, float? waterTransparency)
        {
            if (File.Exists(sourcePath))
            {
                using (StreamReader sr = new StreamReader(sourcePath))
                {
                    AddPicWatermarkAsJPG(sr.BaseStream, watermarkPath, saveFilePath, mp, waterTransparency);
                }
            }
        }

        /// <summary>
        /// 给图片添加文字水印
        /// </summary>
        /// <param name="inputStream">包含要源图片的流</param>
        /// <param name="text">水印文字</param>
        /// <param name="saveFilePath">目标图片的存储地址</param>
        /// <param name="mp">水印位置</param>
        public static void AddTextWatermarkAsJPG(Stream inputStream, string text, string saveFilePath, MarkPosition mp, string textImageFontFamily, string textImageFontStyle, int waterTransparency)
        {

            Image image = Image.FromStream(inputStream);
            Bitmap b = new Bitmap(image.Width, image.Height, PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(b);
            g.Clear(Color.White);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.High;
            g.DrawImage(image, 0, 0, image.Width, image.Height);

            AddWatermarkText(g, text, mp, image.Width, image.Height, textImageFontFamily, textImageFontStyle, waterTransparency);

            try
            {
                CompressAsJPG(b, saveFilePath, 80);
            }
            catch  { ; }
            finally
            {
                b.Dispose();
                image.Dispose();
            }
        }

        /// <summary>
        /// 给图片添加文字水印
        /// </summary>
        /// <param name="sourcePath">源图片的存储地址</param>
        /// <param name="text">水印文字</param>
        /// <param name="saveFilePath">目标图片的存储地址</param>
        /// <param name="mp">水印位置</param>
        public static void AddTextWatermarkAsJPG(string sourcePath, string text, string saveFilePath, MarkPosition mp, string textImageFontFamily, string textImageFontStyle, int waterTransparency)
        {
            if (File.Exists(sourcePath))
            {
                using (StreamReader sr = new StreamReader(sourcePath))
                {
                    AddTextWatermarkAsJPG(sr.BaseStream, text, saveFilePath, mp, textImageFontFamily, textImageFontStyle, waterTransparency);
                }
            }
        }

        /// <summary>
        /// 添加文字水印
        /// </summary>
        /// <param name="picture">要加水印的原图像</param>
        /// <param name="text">水印文字</param>
        /// <param name="mp">添加的位置</param>
        /// <param name="width">文字的最大宽度</param>
        /// <param name="height">文字的最大高度</param>
        /// <param name="textImageFontFamily">字体</param>
        /// <param name="textImageFontStyle">样式</param>
        /// <param name="waterTransparency">透明度</param>
        private static void AddWatermarkText(Graphics picture, string text, MarkPosition mp, int width, int height, string textImageFontFamily, string textImageFontStyle, int waterTransparency)
        {
            int[] sizes = new int[] { 16, 14, 12, 10, 8, 6, 4 };
            Font crFont = null;
            SizeF crSize = new SizeF();
            for (int i = 0; i < 7; i++)
            {
                FontFamily fontFamily = new FontFamily(textImageFontFamily);
                FontStyle fontStyle = (FontStyle)Enum.Parse(typeof(FontStyle), textImageFontStyle);
                crFont = new Font(fontFamily, sizes[i], fontStyle);
                crSize = picture.MeasureString(text, crFont);

                if ((ushort)crSize.Width < (ushort)width)
                    break;
            }

            float xpos = 0;
            float ypos = 0;

            switch (mp)
            {
                case MarkPosition.MP_Left_Top:
                    xpos = ((float)width * (float).01) + (crSize.Width / 2);
                    ypos = (float)height * (float).01;
                    break;
                case MarkPosition.MP_Right_Top:
                    xpos = ((float)width * (float).99) - (crSize.Width / 2);
                    ypos = (float)height * (float).01;
                    break;
                case MarkPosition.MP_Right_Bottom:
                    xpos = ((float)width * (float).99) - (crSize.Width / 2);
                    ypos = ((float)height * (float).99) - crSize.Height;
                    break;
                case MarkPosition.MP_Left_Bottom:
                    xpos = ((float)width * (float).01) + (crSize.Width / 2);
                    ypos = ((float)height * (float).99) - crSize.Height;
                    break;
                case MarkPosition.MP_Center_Middle:
                    xpos = ((float)width * (float).50);
                    ypos = ((float)height * (float).50) - crSize.Height;
                    break;
            }

            StringFormat StrFormat = new StringFormat();
            StrFormat.Alignment = StringAlignment.Center;

            SolidBrush semiTransBrush2 = new SolidBrush(Color.FromArgb(waterTransparency, 0, 0, 0));
            picture.DrawString(text, crFont, semiTransBrush2, xpos + 1, ypos + 1, StrFormat);

            SolidBrush semiTransBrush = new SolidBrush(Color.FromArgb(waterTransparency, 255, 255, 255));
            picture.DrawString(text, crFont, semiTransBrush, xpos, ypos, StrFormat);

            semiTransBrush2.Dispose();
            semiTransBrush.Dispose();
        }

        /// <summary>
        /// 添加图片水印
        /// </summary>
        /// <param name="picture">要加水印的原图像</param>
        /// <param name="waterMarkPath">水印文件的物理地址</param>
        /// <param name="mp">添加的位置</param>
        /// <param name="width">原图像的宽度</param>
        /// <param name="height">原图像的高度</param>
        /// <param name="waterTransparency">水印图片的透明度</param>
        private static void AddWatermarkImage(Graphics picture, string waterMarkPath, MarkPosition mp, int width, int height, float? waterTransparency)
        {
            Image watermark = new Bitmap(waterMarkPath);

            ImageAttributes imageAttributes = new ImageAttributes();
            ColorMap colorMap = new ColorMap();

            colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
            colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
            ColorMap[] remapTable = { colorMap };

            imageAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

            waterTransparency = waterTransparency == null ? (float)0.3 : waterTransparency;
            if (waterTransparency > 1 || waterTransparency < 0)
            { waterTransparency = (float)0.3; }//默认透明度为0.3
            float[][] colorMatrixElements = {
                                                 new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
                                                 new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
                                                 new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
                                                 new float[] {0.0f,  0.0f,  0.0f,  (float)waterTransparency, 0.0f},
                                                 new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
                                             };

            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);

            imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            int xpos = 0;
            int ypos = 0;
            int WatermarkWidth = 0;
            int WatermarkHeight = 0;
            double bl = 1d;
            if ((width > watermark.Width * 4) && (height > watermark.Height * 4))
            {
                bl = 1;
            }
            else if ((width > watermark.Width * 4) && (height < watermark.Height * 4))
            {
                bl = Convert.ToDouble(height / 4) / Convert.ToDouble(watermark.Height);

            }
            else if ((width < watermark.Width * 4) && (height > watermark.Height * 4))
            {
                bl = Convert.ToDouble(width / 4) / Convert.ToDouble(watermark.Width);
            }
            else
            {
                if ((width * watermark.Height) > (height * watermark.Width))
                {
                    bl = Convert.ToDouble(height / 4) / Convert.ToDouble(watermark.Height);

                }
                else
                {
                    bl = Convert.ToDouble(width / 4) / Convert.ToDouble(watermark.Width);

                }

            }

            WatermarkWidth = Convert.ToInt32(watermark.Width * bl);
            WatermarkHeight = Convert.ToInt32(watermark.Height * bl);


            switch (mp)
            {
                case MarkPosition.MP_Left_Top:
                    xpos = 10;
                    ypos = 10;
                    break;
                case MarkPosition.MP_Right_Top:
                    xpos = width - WatermarkWidth - 10;
                    ypos = 10;
                    break;
                case MarkPosition.MP_Right_Bottom:
                    xpos = width - WatermarkWidth - 10;
                    ypos = height - WatermarkHeight - 10;
                    break;
                case MarkPosition.MP_Left_Bottom:
                    xpos = 10;
                    ypos = height - WatermarkHeight - 10;
                    break;
                case MarkPosition.MP_Center_Middle:
                    xpos = (width - WatermarkWidth) / 2;
                    ypos = (height - WatermarkHeight) / 2;
                    break;
            }

            picture.DrawImage(watermark, new Rectangle(xpos, ypos, WatermarkWidth, WatermarkHeight), 0, 0, watermark.Width, watermark.Height, GraphicsUnit.Pixel, imageAttributes);


            watermark.Dispose();
            imageAttributes.Dispose();
        }

        /// <summary>
        /// 水印的位置
        /// </summary>
        public enum MarkPosition
        {
            /// <summary>
            /// 左上角
            /// </summary>
            MP_Left_Top,

            /// <summary>
            /// 左下角
            /// </summary>
            MP_Left_Bottom,

            /// <summary>
            /// 右上角
            /// </summary>
            MP_Right_Top,

            /// <summary>
            /// 右下角
            /// </summary>
            MP_Right_Bottom,

            /// <summary>
            /// 中间
            /// </summary>
            MP_Center_Middle
        }


        #endregion

        #region 图片加水印原理性示例方法

        /// <summary>
        /// 加水印，对传入的Image对象操作
        /// </summary>
        /// <param name="watermarkFullPath">水印图片的完整路径</param>
        /// <param name="imgPhoto">要加水印的Bitmap对象，直接在上面加水印(ref 引用传递)</param>
        private void AddWaterMark(string watermarkFullPath, ref Bitmap imgPhoto)
        {
            string Copyright = "Copyright ? 2005 - 2006 Lotour.com";

            int phWidth = imgPhoto.Width;
            int phHeight = imgPhoto.Height;

            //创建一个与原图尺寸相同的位图
            Bitmap bmPhoto = new Bitmap(phWidth, phHeight, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            //位图装载到一个Graphics对象
            Graphics grPhoto = Graphics.FromImage(bmPhoto);

            //用水印BMP文件创建一个image对象
            Image imgWatermark = new Bitmap(watermarkFullPath);
            int wmWidth = imgWatermark.Width;
            int wmHeight = imgWatermark.Height;

            //设置图片质量
            grPhoto.SmoothingMode = SmoothingMode.AntiAlias;

            //以原始尺寸把照片图像画到此graphics对象
            grPhoto.DrawImage(
            imgPhoto, // 要绘制的 System.Drawing.Image
            new Rectangle(0, 0, phWidth, phHeight), // System.Drawing.Rectangle 结构，它指定所绘制图像的位置和大小。将图像进行缩放以适合该矩形。
            0, // 要绘制的源图像部分的左上角的 x 坐标。
            0, // 要绘制的源图像部分的左上角的 y 坐标。
            phWidth, // 要绘制的源图像部分的宽度。
            phHeight, // 要绘制的源图像部分的高度。
            GraphicsUnit.Pixel); // System.Drawing.GraphicsUnit 枚举的成员，它指定用于确定源矩形的度量单位。

            //基于前面已修改的Bitmap创建一个新Bitmap
            Bitmap bmWatermark = new Bitmap(bmPhoto);
            bmWatermark.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);
            //Load this Bitmap into a new Graphic Object
            Graphics grWatermark = Graphics.FromImage(bmWatermark);

            ImageAttributes imageAttributes = new ImageAttributes();

            //第一步是以透明色(Alpha=0, R=0, G=0, B=0)来替换背景色
            //为此我们将使用一个Colormap并用它来定义一个RemapTable
            ColorMap colorMap = new ColorMap();

            //水印被定义为一个100%的绿色背景
            //这将是我们以transparency来查找并替换的颜色
            colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
            colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);

            ColorMap[] remapTable = { colorMap };

            imageAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

            //第二个颜色操作是用来改变水印的透明度
            //用包涵the coordinates for the RGBA space的一个5x5 的矩阵
            //设置第三行第三列to 0.3f 我们才能实现透明度水平
            float[][] colorMatrixElements = {
                new float[] {1.0f, 0.0f, 0.0f, 0.0f, 0.0f}, 
                new float[] {0.0f, 1.0f, 0.0f, 0.0f, 0.0f}, 
                new float[] {0.0f, 0.0f, 1.8f, 0.0f, 0.0f}, 
                new float[] {0.0f, 0.0f, 0.0f, 0.3f, 0.0f}, 
                new float[] {0.0f, 0.0f, 0.0f, 0.0f, 1.0f}};
            ColorMatrix wmColorMatrix = new ColorMatrix(colorMatrixElements);
            imageAttributes.SetColorMatrix(wmColorMatrix, ColorMatrixFlag.Default,
            ColorAdjustType.Bitmap);

            //水印放在图像的右上角
            //向下10像素，向左10像素
            int xPosOfWm = ((phWidth - wmWidth) - 10);
            int yPosOfWm = 10;

            //水印放在图像的右下角
            //int xPosOfWm = phWidth - wmWidth;
            //int yPosOfWm = phHeight - wmHeight;

            grWatermark.DrawImage(imgWatermark,
            new Rectangle(xPosOfWm, yPosOfWm, wmWidth, wmHeight), //Set the detination Position
            0, // 源图的横坐标位置
            0, // 源图的纵坐标位置
            wmWidth, // 水印宽度
            wmHeight, // 水印高度
            GraphicsUnit.Pixel, // Unit of measurment
            imageAttributes); //ImageAttributes Object

            //以新图替换原始图
            imgPhoto = bmWatermark;
            grPhoto.Dispose();
            grWatermark.Dispose();
            imgWatermark.Dispose();
        }

        #endregion
        
    }

}