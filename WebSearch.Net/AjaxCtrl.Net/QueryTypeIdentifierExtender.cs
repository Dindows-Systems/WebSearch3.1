using System;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.ComponentModel;
using System.ComponentModel.Design;
using AjaxControlToolkit;

[assembly: System.Web.UI.WebResource("AjaxCtrl.Net.QueryTypeIdentifierBehavior.js", "text/javascript")]

namespace AjaxCtrl.Net
{
    [Designer(typeof(QueryTypeIdentifierDesigner))]
    [ClientScriptResource("AjaxCtrl.Net.QueryTypeIdentifierBehavior", "AjaxCtrl.Net.QueryTypeIdentifierBehavior.js")]
    [RequiredScript(typeof(CommonToolkitScripts))]
    [RequiredScript(typeof(TimerScript))]
    [TargetControlType(typeof(Control))]
    public class QueryTypeIdentifierExtender : ExtenderControlBase
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
        [ClientPropertyName("completionInformationalID")]
        [IDReferenceProperty(typeof(WebControl))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase", MessageId = "Member")]
        public virtual string CompletionInformationalID
        {
            get { return GetPropertyValue("CompletionInformationalID", String.Empty); }
            set { SetPropertyValue("CompletionInformationalID", value); }
        }

        [DefaultValue("")]
        [ExtenderControlProperty]
        [ClientPropertyName("completionNavigationalID")]
        [IDReferenceProperty(typeof(WebControl))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase", MessageId = "Member")]
        public virtual string CompletionNavigationalID
        {
            get { return GetPropertyValue("CompletionNavigationalID", String.Empty); }
            set { SetPropertyValue("CompletionNavigationalID", value); }
        }

        [DefaultValue("")]
        [ExtenderControlProperty]
        [ClientPropertyName("completionTransactionalID")]
        [IDReferenceProperty(typeof(WebControl))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase", MessageId = "Member")]
        public virtual string CompletionTransactionalID
        {
            get { return GetPropertyValue("CompletionTransactionalID", String.Empty); }
            set { SetPropertyValue("CompletionTransactionalID", value); }
        }

        [DefaultValue("")]
        [ExtenderControlProperty]
        [ClientPropertyName("progressBarID")]
        [IDReferenceProperty(typeof(Image))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase", MessageId = "Member")]
        public virtual string ProgressBarID
        {
            get { return GetPropertyValue("ProgressBarID", String.Empty); }
            set { SetPropertyValue("ProgressBarID", value); }
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
