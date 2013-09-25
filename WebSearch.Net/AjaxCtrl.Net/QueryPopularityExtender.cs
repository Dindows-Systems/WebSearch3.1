using System;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.ComponentModel;
using System.ComponentModel.Design;
using AjaxControlToolkit;

[assembly: System.Web.UI.WebResource("AjaxCtrl.Net.QueryPopularityBehavior.js", "text/javascript")]
[assembly: System.Web.UI.WebResource("AjaxControlToolkit.Rating.RatingBehavior.js", "text/javascript")]

namespace AjaxCtrl.Net
{
    [Designer(typeof(QueryPopularityDesigner))]
    [ClientScriptResource("AjaxCtrl.Net.QueryPopularityBehavior", "AjaxCtrl.Net.QueryPopularityBehavior.js")]
    [RequiredScript(typeof(CommonToolkitScripts))]
    [RequiredScript(typeof(TimerScript))]
    [RequiredScript(typeof(Rating))]
    [TargetControlType(typeof(Control))]
    public class QueryPopularityExtender : ExtenderControlBase
    {
        [DefaultValue(2)]
        [ExtenderControlProperty]
        [ClientPropertyName("minimumPrefixLength")]
        public virtual int MinimumPrefixLength
        {
            get { return GetPropertyValue("MinimumPrefixLength", 2); }
            set { SetPropertyValue("MinimumPrefixLength", value); }
        }

        [DefaultValue(1000)]
        [ExtenderControlProperty]
        [ClientPropertyName("completionInterval")]
        public virtual int CompletionInterval
        {
            get { return GetPropertyValue("CompletionInterval", 1000); }
            set { SetPropertyValue("CompletionInterval", value); }
        }

        [DefaultValue("")]
        [ExtenderControlProperty]
        [ClientPropertyName("thaiRatingPopularityElementID")]
        [IDReferenceProperty(typeof(Rating))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase", MessageId = "Member")]
        public virtual string ThaiRatingPopularityElementID
        {
            get { return GetPropertyValue("ThaiRatingPopularityElementID", String.Empty); }
            set { SetPropertyValue("ThaiRatingPopularityElementID", value); }
        }

        [DefaultValue("")]
        [RequiredProperty]
        [ExtenderControlProperty]
        [ClientPropertyName("serviceMethod")]
        public virtual string ServiceMethod
        {
            get { return GetPropertyValue("ServiceMethod", String.Empty); }
            set { SetPropertyValue("ServiceMethod", value); }
        }

        [UrlProperty]
        [ExtenderControlProperty]
        [TypeConverter(typeof(ServicePathConverter))]
        [ClientPropertyName("servicePath")]
        public virtual string ServicePath
        {
            get { return GetPropertyValue("ServicePath", String.Empty); }
            set { SetPropertyValue("ServicePath", value); }
        }

        [DefaultValue(true)]
        [ExtenderControlProperty]
        [ClientPropertyName("enableCaching")]
        public virtual bool EnableCaching
        {
            get { return GetPropertyValue("EnableCaching", true); }
            set { SetPropertyValue("EnableCaching", value); }
        }
    }
}
