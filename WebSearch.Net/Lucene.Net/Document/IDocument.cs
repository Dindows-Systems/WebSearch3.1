using System;
using System.Collections.Generic;
using System.Text;
using Lucene.Net.Documents;
using System.Collections;

namespace Lucene.Net.Documents
{
    public interface IDocument
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Document ToDocument();
    }
}
