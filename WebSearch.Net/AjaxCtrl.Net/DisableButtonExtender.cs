using System;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.ComponentModel;
using System.ComponentModel.Design;
using AjaxControlToolkit;

[assembly: System.Web.UI.WebResource("AjaxCtrl.Net.DisableButtonBehavior.js", "text/javascript")]

namespace AjaxCtrl.Net
{
    [Designer(typeof(DisableButtonDesigner))]
    [ClientScriptResource("AjaxCtrl.Net.DisableButtonBehavior", "AjaxCtrl.Net.DisableButtonBehavior.js")]
    [TargetControlType(typeof(TextBox))]
    public class DisableButtonExtender : ExtenderControlBase
    {
        // TODO: Add your property accessors here.
        //
        [ExtenderControlProperty]
        [DefaultValue("")]
        [IDReferenceProperty(typeof(Button))]
        public string TargetButtonID
        {
            get
            {
                return GetPropertyValue("TargetButtonID", "");
            }
            set
            {
                SetPropertyValue("TargetButtonID", value);
            }
        }

        [ExtenderControlProperty]
        public string DisabledText
        {
            get
            {
                return GetPropertyValue("DisabledText", "");
            }
            set
            {
                SetPropertyValue("DisabledText", value);
            }
        }
    }
}
