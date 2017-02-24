/*----------------------------------------------------------------
// Copyright (C) 2012 通通优品
// 版权所有。
//
// 文件名：BadWordsFilter
// 功能描述：特殊词判断与过虑
// 
// 创建标识：Roc.Lee(李鹏鹏) 2011.06.27
// 
// 修改标识：
// 修改描述：
//
// 修改标识：
// 修改描述：
//
//----------------------------------------------------------------*/

using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Public.Common
{
    /// <summary>
    /// 特殊词判断与过虑
    /// </summary>
    public class WordsFilter
    {
        /// <summary>
        /// 多于一个字符的脏词词典
        /// </summary>
        private HashSet<string> wordsHashList = new HashSet<string>();
        /// <summary>
        /// 将所有词典中的所有字符的unicode作为key记录在这，代表存在
        /// </summary>
        private byte[] fastCheck = new byte[char.MaxValue];
        /// <summary>
        /// 采样长度集合（所有词的的第一个相同字符的unicode作为key记录这个词的长度，采样长度最长为8个）
        /// </summary>
        private byte[] fastLength = new byte[char.MaxValue];
        /// <summary>
        /// 记录这个词为单个字符的集合
        /// </summary>
        private BitArray charCheck = new BitArray(char.MaxValue);
        /// <summary>
        /// 记录这个词的最后一个字符集合
        /// </summary>
        private BitArray endCheck = new BitArray(char.MaxValue);
        /// <summary>
        /// 词典中字符长度最长为多少
        /// </summary>
        private int maxWordLength = 0;
        /// <summary>
        /// 词典中字符长度最短为多少
        /// </summary>
        private int minWordLength = int.MaxValue;


        delegate string ReplaceDelegate(string text, int index, int length, string mask);
        ReplaceDelegate replaceDelegate;

        /// <summary>
        /// 是否将过滤的字符串替换为等数量的指定字符(在初始化Init(）函数之前赋值才有效)
        /// </summary>
        public bool IsFill { get; set; }

        /// <summary>
        ///  替换相应的字符串
        /// </summary>
        /// <param name="text"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        string ReplaceStr(string text, int index, int length, string mask)
        {
            return text.Remove(index, length).Insert(index, mask);
        }
        /// <summary>
        /// 将过滤的字符串替换为等数量的指定字符
        /// </summary>
        /// <param name="text"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        string ReplaceFill(string text, int index, int length, string mask)
        {
            if (!string.IsNullOrEmpty(mask))
                return text.Remove(index, length).Insert(index, "".PadLeft(length, mask[0]));
            else
                return ReplaceStr(text, index, length, mask);
        }
        public WordsFilter()
            : this(false)
        {

        }
        /// <summary>
        /// (bool)重载构造函数
        /// </summary>
        /// <param name="isFill">是否将过滤的字符串替换为等数量的指定字符</param>
        public WordsFilter(bool isFill)
        {
            IsFill = true;
        }
        /// <summary>
        /// 初始化脏词词典
        /// </summary>
        /// <param name="badwords">词典数组</param>
        public void Init(string[] badwords)
        {
            if (IsFill)
                replaceDelegate = ReplaceFill;
            else
                replaceDelegate = ReplaceStr;

            foreach (string word in badwords)
            {
                maxWordLength = Math.Max(maxWordLength, word.Length);
                minWordLength = Math.Min(minWordLength, word.Length);

                for (int i = 0; i < 7 && i < word.Length; i++)
                {
                    fastCheck[word[i]] |= (byte)(1 << i);//（二进制1）首字符 （二进制10）第2个字符 （二进制100）第3个字符……
                }

                for (int i = 7; i < word.Length; i++)
                {
                    fastCheck[word[i]] |= 0x80;//第7个以后的初始化为128（1000 0000）
                }

                if (word.Length == 1)//如果这个词是单个字符
                {
                    charCheck[word[0]] = true;//以这个字符的unicode作为key赋值true
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(word))
                    {
                        fastLength[word[0]] |= (byte)(1 << (Math.Min(7, word.Length - 2)));//以这个词的第一个字符的unicode作为key或运算二进制1向左移动到 倒数第1个所在位置不能超过第7位
                        endCheck[word[word.Length - 1]] = true;//这个词的最后一个字符的unicode作为key赋值true

                        wordsHashList.Add(word);
                    }
                }
            }
        }

        /// <summary>
        /// 特殊词过虑并替换
        /// </summary>
        /// <param name="text">原文本</param>
        /// <param name="mask">替换字符</param>
        /// <returns>过虑后的文本</returns>
        public string Filter(string text, string mask)
        {
            int index = 0;

            while (index < text.Length - 1)
            {
                if (index > 0 || (fastCheck[text[index]] & 1) == 0)//如果不是第一字符匹配或者不包含在词典中
                {
                    //如果不是第一字符匹配或者不包含在词典中 则 index累加
                    while (index < text.Length - 1 && (fastCheck[text[index]] & 1) == 0) index++;
                }
                //词典中某个词的第一个字符
                char begin = text[index];

                if (minWordLength == 1 && charCheck[begin])//如果这个词是一个字符
                {
                    text = replaceDelegate(text, index, 1, mask);
                    //text.Remove(index, 1).Insert(index, mask);
                    continue;
                }
                int count = Math.Min(maxWordLength, Math.Abs(text.Length - index) - 1);//最后剩下字符数

                for (int j = count; j > 0; j--)
                {
                    char current = text[index + j];

                    if (count > 1)
                        count--;

                    if (j + 1 >= minWordLength)//如果判断的词长度大于等于词典中的最小长度
                    {
                        if ((fastLength[begin] & (1 << Math.Min(j - 1, 7))) > 0 && endCheck[current])//如果长度符合，最后字符也符合
                        {
                            string sub = text.Substring(index, j + 1);//取出这个完整的词

                            if (wordsHashList.Contains(sub))//如果在这个词典中
                            {
                                //text = text.Remove(index, j + 1).Insert(index, mask);
                                text = replaceDelegate(text, index, j + 1, mask);

                                if (mask.Length > 0)
                                    count = mask.Length - 1;
                                else
                                    count = 0;
                                break;
                            }
                        }
                    }
                }

                index += count;
            }
            return text;
        }
        /// <summary>
        /// 判断文本中是否包含特殊词
        /// </summary>
        /// <param name="text">特殊词</param>
        /// <returns></returns>
        public bool HasBadWord(string text)
        {
            int index = 0;

            while (index < text.Length)
            {
                int count = 1;

                if (index > 0 || (fastCheck[text[index]] & 1) == 0)//如果不是第一字符匹配或者不包含在词典中
                {
                    //如果不是第一字符匹配或者不包含在词典中 则 index累加
                    while (index < text.Length - 1 && (fastCheck[text[++index]] & 1) == 0) ;
                }
                //词典中某个词的第一个字符
                char begin = text[index];

                if (minWordLength == 1 && charCheck[begin])//如果这个词是一个字符
                {
                    return true;
                }

                for (int j = 1; j <= Math.Min(maxWordLength, text.Length - index - 1); j++)
                {
                    char current = text[index + j];

                    if ((fastCheck[current] & 1) == 0 && count == j)//如果不是第一个字符
                    {
                        ++count;
                    }

                    if ((fastCheck[current] & (1 << Math.Min(j, 7))) == 0)//第j个字符如果不在词典的前7个字符中则跳过判断下个字符
                    {
                        break;
                    }

                    if (j + 1 >= minWordLength)//如果text不是一个字符
                    {
                        if ((fastLength[begin] & (1 << Math.Min(j - 1, 7))) > 0 && endCheck[current])//如果长度符合，最后字符也符合
                        {
                            string sub = text.Substring(index, j + 1);//取出这个完整的词

                            if (wordsHashList.Contains(sub))//如果在这个词典中
                            {
                                return true;
                            }
                        }
                    }
                }

                index += count;
            }

            return false;
        }
    }

    class InnerWordDemo
    {


        private static InnerWordDemo instance;

        private static WordsFilter wordFilter = new WordsFilter();
        private static WordsFilter wordFilter2 = new WordsFilter();
        private static object locker = new object();

        private InnerWordDemo() { }
        public static InnerWordDemo Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                        {
                            instance = new InnerWordDemo();
                        }
                    }
                }
                return instance;
            }
        }
        public void Init()
        {
            wordFilter.IsFill = true;
            wordFilter.Init(null);
            wordFilter2.Init(null);
        }

        public void Reset()
        {
            Init();
        }

        public string Filter1(string text)
        {
            return wordFilter.Filter(text, "");
        }
        public string Filter2(string text)
        {
            return wordFilter2.Filter(text, "");
        }
    }
}
