using System;
using System.Collections.Generic;
using System.Text;
using Lucene.Net.Index;
using Lucene.Net.Analysis;
using Lucene.Net.Store;

namespace WebSearch.DataCenter.Net.Lucene
{
    public static class LuceneMerger
    {
        /// <summary>
        /// Merge the two indexes into targetPath
        /// </summary>
        /// <param name="targetPath"></param>
        /// <param name="analyzer"></param>
        /// <param name="indexPath1"></param>
        /// <param name="indexPath2"></param>
        /// <returns></returns>
        public static bool Merge(string indexPath1, string indexPath2,
            Analyzer analyzer)
        {
            List<string> otherPaths = new List<string>(1);
            otherPaths.Add(indexPath2);

            return Merge(indexPath1, otherPaths, analyzer);
        }

        /// <summary>
        /// Merge the three indexes into targetPath
        /// </summary>
        /// <param name="targetPath"></param>
        /// <param name="analyzer"></param>
        /// <param name="indexPath1"></param>
        /// <param name="indexPath2"></param>
        /// <param name="indexPath3"></param>
        /// <returns></returns>
        public static bool Merge(string indexPath1, string indexPath2,
            string indexPath3, Analyzer analyzer)
        {
            List<string> otherPaths = new List<string>(2);
            otherPaths.Add(indexPath2);
            otherPaths.Add(indexPath3);

            return Merge(indexPath1, otherPaths, analyzer);
            return true;
        }

        /// <summary>
        /// Merge the indexes in the given index path list into targetPath
        /// </summary>
        /// <param name="targetPath"></param>
        /// <param name="analyzer"></param>
        /// <param name="indexPaths"></param>
        /// <returns></returns>
        public static bool Merge(string resultPath,
            List<string> otherIndexes, Analyzer analyzer)
        {
            // create the target fs index writer.
            IndexWriter fswriter = new IndexWriter(
                resultPath, analyzer, (System.IO.File.Exists(
                System.IO.Path.Combine(resultPath, "segments"))) ? false : true);

            otherIndexes.Remove(resultPath);
            // convert indexPaths into Directory list
            Directory[] dirs = new Directory[otherIndexes.Count];
            int index = 0;
            foreach (string path in otherIndexes)
                dirs[index] = FSDirectory.GetDirectory(path, false);

            fswriter.AddIndexes(dirs);
            fswriter.Close();
            return true;
        }

        //public static bool Merge(string targetPath, 
        // http://blog.csdn.net/lyflower/archive/2007/03/08/1524568.aspx
    }
}
