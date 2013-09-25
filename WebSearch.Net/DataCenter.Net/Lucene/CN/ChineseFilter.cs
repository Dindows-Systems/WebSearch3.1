using System;
using System.IO;
using System.Collections;
using System.Globalization;

using Lucene.Net.Analysis;

namespace WebSearch.DataCenter.Net.Lucene.CN
{
    /// <summary>
    /// Title: ChineseFilter
    /// Description: Filter with a stop word table
    ///              Rule: No digital is allowed.
    ///                    English word/token should larger than 1 character.
    ///                    One Chinese character as one Chinese word.
    /// TO DO:
    ///   1. Add Chinese stop words, such as \ue400
    ///   2. Dictionary based Chinese word extraction
    ///   3. Intelligent Chinese word extraction
    /// 
    /// Copyright:    Copyright (c) 2007
    /// Company:
    /// @author Jialiang Ge
    /// @version $Id: ChineseFilter.java, v 1.4 2007/04/22 12:49:33 ehatcher Exp $
    /// </summary>
    public sealed class ChineseFilter : TokenFilter
    {
        #region Chinese Stop Words

        public static String[] STOP_WORDS = 
		{
            "and", "are", "as", "at", "be", "but", "by", "for", "if", "in", "into", "is", "it",
            "no", "not", "of", "on", "or", "such", "that", "the", "their", "then", "there", "these",
            "they", "this", "to", "was", "will", "with", "的", "地"
            //,"啊", "鄙人", "不怕", "从", "多少", "各", "和", "及其", "将", "开外", "另外", "哪个", "乃至",
            //"阿", "彼", "不然", "从而", "而", "各个", "何", "及至", "较", "靠", "另一方面", "哪里", "呢",
            //"哎", "彼此", "不如", "打", "而况", "各位", "何处", "即", "较之", "咳", "论", "哪年", "能",
            //"哎呀", "边", "不特", "待", "而且", "各种", "何况", "即便", "叫", "可", "嘛", "哪怕", "你",
            //"哎哟", "别", "不惟", "但", "而是", "各自", "何时", "即或", "接着", "可见", "吗", "哪天", "你们",
            //"唉", "别的", "不问", "但是", "而外", "给", "嘿", "即令", "结果", "可是", "慢说", "哪些", "您",
            //"俺", "别说", "不只", "当", "而言", "根据", "哼", "即若", "借", "可以", "漫说", "哪样", "宁",
            //"俺们", "并", "朝", "当着", "而已", "跟", "哼唷", "即使", "紧接着", "况且", "冒", "那", "宁可",
            //"按", "并且", "朝着", "到", "尔后", "故", "呼哧", "几", "进而", "啦", "么", "那边", "宁肯",
            //"按照", "不比", "趁", "得", "反过来", "故此", "乎", "几时", "尽", "来", "每", "那儿", "宁愿",
            //"吧", "不成", "趁着", "的", "反过来说", "固然", "哗", "己", "尽管", "来着", "每当", "那个", "哦",
            //"吧哒", "不单", "乘", "的话", "反之", "关于", "还是", "既", "经", "离", "们", "那会儿", "呕",
            //"把", "不但", "冲", "等", "非但", "管", "还有", "既然", "经过", "例如", "莫若", "那里", "啪达",
            //"罢了", "不独", "除", "等等", "非徒", "归", "换句话说", "既是", "就", "哩", "某", "那么", "旁人",
            //"被", "不管", "除此之外", "地", "否则", "果然", "换言之", "继而", "就是", "连", "某个", "那么些", "呸",
            //"本", "不光", "除非", "第", "嘎", "果真", "或", "加之", "就是说", "连同", "某些", "那么样", "凭",
            //"本着", "不过", "除了", "叮咚", "嘎登", "过", "或是", "假如", "据", "两者", "拿", "那时", "凭借",
            //"比", "不仅", "此", "对", "该", "哈", "或者", "假若", "具体地说", "了", "哪", "那些", "其",
            //"比方", "不拘", "此间", "对于", "赶", "哈哈", "极了", "假使", "具体说来", "临", "哪边", "那样", "其次",
            //"比如", "不论", "此外", "多", "个", "呵", "及", "鉴于", "开始", "另", "哪儿", "乃", "其二",
            //"其他", "任凭", "时候", "他", "望", "向", "一来", "用", "再者", "这就是说", "至于", "纵使",
            //"其它", "如", "什么", "他们", "为", "向着", "一切", "由", "在", "这里", "诸位", "遵照",
            //"其一", "如此", "什么样", "他人", "为何", "嘘", "一样", "由此可见", "在下", "这么", "着", "作为",
            //"其余", "如果", "使得", "它", "为了", "呀", "一则", "由于", "咱", "这么点儿", "着呢", "兮",
            //"其中", "如何", "是", "它们", "为什么", "焉", "依", "有", "咱们", "这么些", "自", "呃",
            //"起", "如其", "是的", "她", "为着", "沿", "依照", "有的", "则", "这么样", "自从", "呗",
            //"一方面", "如若", "首先", "她们", "喂", "沿着", "矣", "有关", "怎", "这时", "自个儿", "咚",
            //"起见", "如上所述", "谁", "倘", "嗡嗡", "要", "以", "有些", "怎么", "这些", "自各儿", "咦",
            //"岂但", "若", "谁知", "倘或", "我", "要不", "以便", "又", "怎么办", "这样", "自己", "喏",
            //"恰恰相反", "若非", "顺", "倘然", "我们", "要不然", "以及", "于", "怎么样", "正如", "自家", "啐",
            //"前后", "若是", "顺着", "倘若", "呜", "要不是", "以免", "于是", "怎样", "吱", "自身", "喔唷",
            //"前者", "啥", "似的", "倘使", "呜呼", "要么", "以至", "于是乎", "咋", "之", "综上所述", "嗬",
            //"且", "上下", "虽", "腾", "乌乎", "要是", "以至于", "与", "照", "之类", "总的来看", "嗯",
            //"然而", "尚且", "虽然", "替", "无论", "也", "以致", "与此同时", "照着", "之所以", "总的来说", "嗳",
            //"然后", "设若", "虽说", "通过", "无宁", "也罢", "抑或", "与否", "者", "之一", "总的说来", "这会儿",
            //"然则", "设使", "虽则", "同", "毋宁", "也好", "因", "与其", "这", "只是", "总而言之", "纵然",
            //"让", "甚而", "随", "同时", "嘻", "一", "因此", "越是", "这边", "只限", "总之", "至",
            //"人家", "甚么", "随着", "哇", "吓", "一般", "因而", "云云", "这儿", "只要", "纵", "再说",
            //"任", "甚至", "所", "万一", "相对而言", "一旦", "因为", "哉", "这个", "只有", "纵令", "哟",
            //"任何", "省得", "所以", "往", "像"
		};

        private static Hashtable _stopWords = null;

        public static Hashtable StopWords
        {
            get
            {
                if (_stopWords == null)
                    _stopWords = StopFilter.MakeStopSet(STOP_WORDS);
                return _stopWords;
            }
        }

        #endregion

        public ChineseFilter(TokenStream _in)
            : base(_in)
        {
        }

        public override Token Next()
        {
            for (Token token = input.Next(); token != null; token = input.Next())
            {
                string text = token.TermText().ToLower();

                // why not key off token type here assuming ChineseTokenizer comes first?
                if (StopWords[text] == null)
                {
                    switch (Char.GetUnicodeCategory(text[0]))
                    {
                        case UnicodeCategory.LowercaseLetter:
                        case UnicodeCategory.UppercaseLetter:
                            // English word/token should larger than 1 character.
                            if (text.Length > 1)
                                return token;
                            break;
                        case UnicodeCategory.OtherLetter:
                            // One Chinese character as one Chinese word.
                            // Chinese word extraction to be added later here.
                            return token;
                    }
                }
            }
            return null;
        }
    }
}
