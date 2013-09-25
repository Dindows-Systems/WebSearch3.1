using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Store;
using WebSearch.Common.Net;
using System.Diagnostics;

namespace WebSearch.DataCenter.Net.Lucene
{
    public static class LuceneIndexer
    {
        #region Settings for ParallelIndexer

        /// <summary>
        /// the place to hold the index
        /// </summary>
        public static string INDEX_STORE_PATH = @"C:\Index\";

        /// <summary>
        /// max document number in ram directory
        /// </summary>
        public static int MAX_DOCS_IN_RAM = 10;

        /// <summary>
        /// max field length, no index for surpass
        /// </summary>
        public static readonly int MAX_FIELD_LENGTH = int.MaxValue;

        /// <summary>
        /// Language used in this indexer
        /// </summary>
        public static SupportedLanguage LANGUAGE = SupportedLanguage.English;

        /// <summary>
        /// Whether use the single thread or multi-thread
        /// </summary>
        public static bool SINGLE_THREAD = true;

        #endregion

        private static Directory _directory = null;

        private static Analyzer _analyzer = null;

        private static List<Thread> _threadList = new List<Thread>();

        private static IndexModifier _modifier = null;

        /// <summary>
        /// Get the instance of the index modifier
        /// </summary>
        /// <returns></returns>
        public static IndexModifier Instance
        {
            get
            {
                lock (_threadList)
                {
                    // initialize the index modifier object
                    if (_modifier == null)
                        Initialize();

                    // add the current thread into thread list
                    if (!_threadList.Contains(Thread.CurrentThread))
                        _threadList.Add(Thread.CurrentThread);

                    return _modifier;
                }
            }
        }

        /// <summary>
        /// Initialize the index modifier object
        /// </summary>
        public static void Initialize()
        {
            #region prepare the analyzer object

            _analyzer = LuceneAnalyzer.Get(LANGUAGE);

            #endregion

            #region prepare the directory and indexer object

            if (INDEX_STORE_PATH == null || INDEX_STORE_PATH == "")
            {
                // ram directory
                _directory = new RAMDirectory();
                // create the index modifier
                _modifier = new IndexModifier(_directory, _analyzer, false);
            }
            else
            {
                if (!System.IO.Directory.Exists(INDEX_STORE_PATH))
                    System.IO.Directory.CreateDirectory(INDEX_STORE_PATH);

                // file system directory
                bool create = (System.IO.File.Exists(System.IO.Path.Combine(
                    INDEX_STORE_PATH, "segments"))) ? false : true;
                _directory = FSDirectory.GetDirectory(INDEX_STORE_PATH, create);
                _modifier = new IndexModifier(_directory, _analyzer, create);
            }
            _modifier.SetMaxFieldLength(MAX_FIELD_LENGTH);
            _modifier.SetMaxBufferedDocs(MAX_DOCS_IN_RAM);

            #endregion
        }

        /// <summary>
        /// Get the thread count of the current indexing threads
        /// </summary>
        /// <returns></returns>
        public static int GetThreadCount()
        {
            return _threadList.Count;
        }

        /// <summary>
        /// Close the current thread in the indexer's thread collection
        /// </summary>
        public static void Close()
        {
            lock (_threadList)
            {
                // remove the current thread
                if (_threadList.Contains(Thread.CurrentThread))
                    _threadList.Remove(Thread.CurrentThread);

                if (_threadList.Count == 0)
                {
                    if (_modifier != null)
                    {
                        _modifier.Close();
                        _modifier = null;
                    }
                }
            }
        }

        /// <summary>
        /// Terminate the index modifier for all threads
        /// </summary>
        public static void Terminate()
        {
            lock (_threadList)
            {
                // remove all the threads
                _threadList.Clear();
                // close the modifier
                if (_modifier != null)
                {
                    _modifier.Close();
                    _modifier = null;
                }
            }
        }

        /// <summary>
        /// Optimize the indexes
        /// </summary>
        public static void Optimize()
        {
            LuceneIndexer.Instance.Optimize();
        }
    }
}
