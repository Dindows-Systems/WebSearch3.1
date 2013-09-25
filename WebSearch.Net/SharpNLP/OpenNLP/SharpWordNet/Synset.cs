//Copyright (C) 2006 Richard J. Northedge
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

using System;

namespace SharpWordNet
{
    /// <summary>
    /// Summary description for Synset.
    /// </summary>
    public class Synset
    {
        private int _offset;
        private string _gloss;
        private string[] _wordList;
        private string _lexicographerFile;
        private Relation[] _relations;

        private Synset()
        {
        }

        internal Synset(int offset, string gloss, string[] wordList,
            string lexicographerFile, Relation[] relations)
        {
            _offset = offset;
            _gloss = gloss;
            _wordList = wordList;
            _lexicographerFile = lexicographerFile;
            _relations = relations;
        }

        public int Offset
        {
            get
            {
                return _offset;
            }
        }

        public string Gloss
        {
            get
            {
                return _gloss;
            }
        }

        public string GetWord(int wordIndex)
        {
            return _wordList[wordIndex];
        }

        public int WordCount
        {
            get
            {
                return _wordList.Length;
            }
        }

        public string LexicographerFile
        {
            get
            {
                return _lexicographerFile;
            }
        }

        public Relation GetRelation(int relationIndex)
        {
            return _relations[relationIndex];
        }

        public int RelationCount
        {
            get
            {
                return _relations.Length;
            }
        }

        public override string ToString()
        {
            System.Text.StringBuilder oOutput = new System.Text.StringBuilder();

            for (int iCurrentWord = 0; iCurrentWord < _wordList.Length; iCurrentWord++)
            {
                oOutput.Append(_wordList[iCurrentWord]);
                if (iCurrentWord < _wordList.Length - 1)
                {
                    oOutput.Append(", ");
                }
            }

            oOutput.Append("  --  ").Append(_gloss);

            return oOutput.ToString();
        }
    }
}
