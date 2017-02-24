using System;
using System.Collections.Generic;
using System.Web;

namespace Public.Common
{
    [Serializable]
    public class UploadInfo
    {

        #region 构造函数

        public UploadInfo() { }

        #endregion

        #region 私有字段

        private string _originalImageName = "";                 //原始图片名称
        private string _folderPath = "UploadImages/Default/Default";         //图片保存的文件夹(多个系统共用时为每个不同的系统分配不同的文件夹)
        private bool _isReName = true;                          //是否按照Guid码来重命名图片
        private string _sizes = "50*80,100*120,130*150,200*250";                             //生成缩略图的尺寸(格式:60*90,100*120,130*150,200*300,)

        private bool _isWaterImage = true;                      //是否生成图片水印图片
        private string _waterImageName = "WaterImage.gif";      //图片水印的图片名称(可以是URL地址)
        private string _waterImagePosition = "MP_Left_Top";     //图片水印的位置 可选值(左上角:MP_Left_Top,左下角:MP_Left_Bottom,右上角:MP_Right_Top,右下角:MP_Right_Bottom,中间:MP_Center_Middle)
        private string _waterImageTransparency = "0.3";         //图片水印的透明度

        private bool _isWaterText = true;                       //是否生成文字水印图片
        private string _textImageName = "www.xaiy.net";        //文字水印的文字
        private string _textImagePosition = "MP_Right_Bottom";  //文字水印的位置
        private string _textImageTransparency = "0.3";          //文字水印的透明度
        private string _textImageFontFamily = "Arial";          //文字的字体
        private string _textImageFontStyle = "Bold";            //文字的样式 可选样式:(普通文本:Regular,加粗文本:Bold,倾斜文本:Italic,带下划线的文本:Underline,中间有直线通过的文本:Strikeout)

        #endregion

        #region 公共属性

        /// <summary>
        /// 原始图片名称
        /// </summary>
        public string OriginalImageName
        {
            get { return _originalImageName; }
            set { _originalImageName = value; }
        }
        /// <summary>
        /// 图片保存的文件夹(多个系统共用时为每个不同的系统分配不同的文件夹)
        /// </summary>
        public string FolderPath
        { 
            get { return _folderPath; } 
            set { _folderPath = value; }
        }
        /// <summary>
        /// 是否按照Guid码来重命名图片
        /// </summary>
        public bool IsReName
        {
            get { return _isReName; }
            set { _isReName = value; }
        }
        /// <summary>
        /// 生成缩略图的尺寸(格式:60*90,100*120,130*150,200*300,)
        /// </summary>
        public string Sizes
        {
            get{ return _sizes; }
            set{ _sizes = value; }
        }
        /// <summary>
        /// 是否生成图片水印图片
        /// </summary>
        public bool IsWaterImage
        {
            get { return _isWaterImage; }
            set { _isWaterImage = value; }
        }
        /// <summary>
        /// 图片水印的图片名称(可以是URL地址)
        /// </summary>
        public string WaterImageName
        {
            get{ return _waterImageName; }
            set{ _waterImageName = value; }
        }
        /// <summary>
        /// 图片水印的位置 可选值(左上角:MP_Left_Top,左下角:MP_Left_Bottom,右上角:MP_Right_Top,右下角:MP_Right_Bottom,中间:MP_Center_Middle)
        /// </summary>
        public string WaterImagePosition
        {
            get{ return _waterImagePosition; }
            set{ _waterImagePosition = value; }
        }
        /// <summary>
        /// 图片水印的透明度
        /// </summary>
        public string WaterImageTransparency
        {
            get{ return _waterImageTransparency; }
            set{ _waterImageTransparency = value; }
        }
        /// <summary>
        /// 是否生成文字水印图片
        /// </summary>
        public bool IsWaterText
        {
            get { return _isWaterText; }
            set { _isWaterText = value; }
        }
        /// <summary>
        /// 文字水印的文字
        /// </summary>
        public string TextImageName
        {
            get{ return _textImageName; }
            set { _textImageName = value; }
        }
        /// <summary>
        /// 文字水印的位置
        /// </summary>
        public string TextImagePosition
        {
            get { return _textImagePosition; }
            set { _textImagePosition = value; }
        }
        /// <summary>
        /// 文字水印的透明度
        /// </summary>
        public string TextImageTransparency
        {
            get { return _textImageTransparency; }
            set { _textImageTransparency = value; }
        }
        /// <summary>
        /// 文字的字体
        /// </summary>
        public string TextImageFontFamily
        {
            get { return _textImageFontFamily; }
            set { _textImageFontFamily = value; }
        }
        /// <summary>
        /// 文字的样式 可选样式:(普通文本:Regular,加粗文本:Bold,倾斜文本:Italic,带下划线的文本:Underline,中间有直线通过的文本:Strikeout)
        /// </summary>
        public string TextImageFontStyle
        {
            get { return _textImageFontStyle; }
            set { _textImageFontStyle = value; }
        }

        #endregion
        
    }
}
