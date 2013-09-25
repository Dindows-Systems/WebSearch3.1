using System;
using System.Collections.Generic;
using System.Text;

namespace WebSearch.Model.Net
{
    public class WebSite : BaseModel
    {
        #region Properties

        private string _homeUrl;

        /// <summary>
        /// Web site's home url
        /// </summary>
        public string HomeUrl
        {
            get { return _homeUrl; }
            set { _homeUrl = value; }
        }

        private SiteType _type;

        /// <summary>
        /// Web site type
        /// </summary>
        public SiteType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private List<WebClassification> _focuses = new List<WebClassification>();

        /// <summary>
        /// The site's focus
        /// </summary>
        public List<WebClassification> Focuses
        {
            get { return _focuses; }
            set { _focuses = value; }
        }

        private float _traffic;

        /// <summary>
        /// Site traffic
        /// </summary>
        public float Traffic
        {
            get { return _traffic; }
            set { _traffic = value; }
        }

        private int _clusterNum;

        /// <summary>
        /// Mirror site number
        /// </summary>
        public int ClusterNum
        {
            get { return _clusterNum; }
            set { _clusterNum = value; }
        }

        private int _size;

        /// <summary>
        /// Size of the site
        /// </summary>
        private int Size
        {
            get { return _size; }
            set { _size = value; }
        }

        #endregion
    }

    /// <summary>
    /// Web site type
    /// </summary>
    public enum SiteType
    {
        Portal,         // 门户网站 (http://www.yahoo.com/)
        SearchEngine,   // 搜索引擎 (http://www.live.com/)
        B2B,            // e.g.商业网 (http://china.alibaba.com/)
        C2C,            // e.g.拍卖网 (http://www.eachnet.com/)
        B2C,            // e.g.购物网 (http://www.amazon.com/)
        Other           // 其他网站 (http://news.enet.com.cn/)
    };

    public enum WebClassification
    {
        Invalid = -1, News = 0, Sports, Email,
        Search, Education, Abroad, Jobbing, IT,
        Travel, Living, Digital, Music, Star, 
        Military, Art, Game, Cartoon, Furnishings,
        Finance, Culture, Health
    };
}
