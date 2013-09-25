using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace WebSearch.Model.Net
{
    public abstract class BaseModel
    {
        #region Properties

        protected Hashtable _otherInfos = new Hashtable();

        public Hashtable OtherInfos
        {
            get { return _otherInfos; }
            set { _otherInfos = value; }
        }

        #endregion
    }
}
