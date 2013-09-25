using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Globalization;
using Lucene.Net.Analysis;
using WebSearch.Common.Net;
using WebSearch.Linguistics.Net;

namespace WebSearch.DataCenter.Net.Lucene.CN
{
    /*
     * Title: ChineseTokenizer
     * Description: Extract tokens from the Stream using Character.getType()
     *              Rule: A Chinese character as a single token
     * Copyright:   Copyright (c) 2001
     * Company:
     *
     * The difference between thr ChineseTokenizer and the
     * CJKTokenizer (id=23545) is that they have different
     * token parsing logic.
     * 
     * Let me use an example. If having a Chinese text
     * "C1C2C3C4" to be indexed, the tokens returned from the
     * ChineseTokenizer are C1, C2, C3, C4. And the tokens
     * returned from the CJKTokenizer are C1C2, C2C3, C3C4.
     *
     * Therefore the index the CJKTokenizer created is much
     * larger.
     *
     * The problem is that when searching for C1, C1C2, C1C3,
     * C4C2, C1C2C3 ... the ChineseTokenizer works, but the
     * CJKTokenizer will not work.
     *
     * @author Jialiang Ge
     * @version $Id: ChineseTokenizer, v 1.4 2007/04/22 13:56:03 otis Exp $
     */
    public class ChineseTokenizer : Tokenizer
    {
        protected string _originalText;
        protected int _offset = 0;
        protected int _index = 0;
        protected string[] _tokenStrs;

        public ChineseTokenizer(TextReader _in) : base(_in)
        {
            this._originalText = _in.ReadToEnd();
            // for html text, filter all the tags
            string plainText = HtmlParser.Filter(_originalText, true);
            // tokenize the plain text, do the filter and recognize name
            this._tokenStrs = ChineseHelper.Segment(plainText);
        }

        public override Token Next()
        {
            int start = 0; string text;
            do
            {
                if (_index >= _tokenStrs.Length)
                    return null;

                // get the next token text, (update index)
                text = this._tokenStrs[this._index++].Trim();
                if (text == "") { start = -1; continue; }
                // try to find the token in the orginal text
                start = _originalText.IndexOf(text, _offset);

            } while (start < 0);

            this._offset = start + text.Length;
            if (_offset > _originalText.Length)
                return null;
            return new Token(text, start, _offset);
        }
    }
}
