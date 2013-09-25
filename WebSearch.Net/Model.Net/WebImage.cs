using System;
using System.Collections.Generic;
using System.Text;

namespace WebSearch.Model.Net
{
    public class WebImage : BaseModel
    {
        #region Properties

        private string _fileName;

        /// <summary>
        /// File Name of the image
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        private string _url;

        /// <summary>
        /// Url of the image
        /// </summary>
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        private string _surroundingText;

        /// <summary>
        /// Surrounding Text
        /// </summary>
        public string SurroundingText
        {
            get { return _surroundingText; }
            set { _surroundingText = value; }
        }

        private string _alt;

        /// <summary>
        /// Alternative Text
        /// </summary>
        public string Alt
        {
            get { return _alt; }
            set { _alt = value; }
        }

        #endregion
    }
}
