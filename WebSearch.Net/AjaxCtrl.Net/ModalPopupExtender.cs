// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using AjaxControlToolkit;

[assembly: System.Web.UI.WebResource("AjaxCtrl.Net.ModalPopupBehavior.js", "text/javascript")]

namespace AjaxCtrl.Net
{
    /// <summary>
    /// Extender for the ModalPopup
    /// </summary>
    [Designer("AjaxCtrl.Net.ModalPopupDesigner, AjaxCtrl.Net")]
    [ClientScriptResource("AjaxCtrl.Net.ModalPopupBehavior", "AjaxCtrl.Net.ModalPopupBehavior.js")]
    [RequiredScript(typeof(CommonToolkitScripts))]
    [RequiredScript(typeof(DragPanelExtender))]
    [RequiredScript(typeof(DropShadowExtender))]
    [TargetControlType(typeof(Control))]
    public class ModalPopupExtender : DynamicPopulateExtenderControlBase
    {
        // Property names
        private const string stringPopupControlID = "PopupControlID";
        private const string stringPopupDragHandleControlID = "PopupDragHandleControlID";
        private const string stringBackgroundCssClass = "BackgroundCssClass";
        private const string stringDropShadow = "stringDropShadow";
        private const string stringDrag = "stringDrag";
        private const string stringOkControlID = "OkControlID";
        private const string stringCancelControlID = "CancelControlID";
        private const string stringOnOkScript = "OnOkScript";
        private const string stringOnCancelScript = "OnCancelScript";
        private const string stringX = "X";
        private const string stringY = "Y";
        private const string stringMaskWidth = "MaskWidth";
        private const string stringMaskHeight = "MaskHeight";

        [ExtenderControlProperty()]
        [DefaultValue("")]
        [IDReferenceProperty(typeof(WebControl))]
        [RequiredProperty()]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase", Justification = "Following ASP.NET AJAX pattern")]
        public string PopupControlID
        {
            get { return GetPropertyValue(stringPopupControlID, ""); }
            set { SetPropertyValue(stringPopupControlID, value); }
        }

        [ExtenderControlProperty()]
        [DefaultValue("")]
        public string BackgroundCssClass
        {
            get { return GetPropertyValue(stringBackgroundCssClass, ""); }
            set { SetPropertyValue(stringBackgroundCssClass, value); }
        }

        [ExtenderControlProperty()]
        [DefaultValue(false)]
        public bool DropShadow
        {
            get { return GetPropertyValue(stringDropShadow, false); }
            set { SetPropertyValue(stringDropShadow, value); }
        }

        [ExtenderControlProperty()]
        [DefaultValue(false)]
        [Obsolete("The drag feature on modal popup will be automatically turned on if you specify the PopupDragHandleControlID property. Setting the Drag property is a noop")]
        public bool Drag
        {
            get { return GetPropertyValue(stringDrag, false); }
            set { SetPropertyValue(stringDrag, value); }
        }

        [ExtenderControlProperty()]
        [DefaultValue(-1)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "X", Justification = "Common term")]
        public int X
        {
            get { return GetPropertyValue(stringX, -1); }
            set { SetPropertyValue(stringX, value); }
        }

        [ExtenderControlProperty()]
        [DefaultValue(-1)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Y", Justification = "Common term")]
        public int Y
        {
            get { return GetPropertyValue(stringY, -1); }
            set { SetPropertyValue(stringY, value); }
        }

        [ExtenderControlProperty()]
        [DefaultValue(-1)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "MaskWidth", Justification = "Common term")]
        public int MaskWidth
        {
            get { return GetPropertyValue(stringMaskWidth, -1); }
            set { SetPropertyValue(stringMaskWidth, value); }
        }

        [ExtenderControlProperty()]
        [DefaultValue(-1)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "MaskHeight", Justification = "Common term")]
        public int MaskHeight
        {
            get { return GetPropertyValue(stringMaskHeight, -1); }
            set { SetPropertyValue(stringMaskHeight, value); }
        }

        [ExtenderControlProperty()]
        [IDReferenceProperty(typeof(WebControl))]
        [DefaultValue("")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase", Justification = "Following ASP.NET AJAX pattern")]
        public string PopupDragHandleControlID
        {
            get { return GetPropertyValue(stringPopupDragHandleControlID, ""); }
            set { SetPropertyValue(stringPopupDragHandleControlID, value); }
        }
        [ExtenderControlProperty()]
        [DefaultValue("")]
        [IDReferenceProperty(typeof(WebControl))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase", Justification = "Following ASP.NET AJAX pattern")]
        public string OkControlID
        {
            get { return GetPropertyValue(stringOkControlID, ""); }
            set { SetPropertyValue(stringOkControlID, value); }
        }

        [ExtenderControlProperty()]
        [DefaultValue("")]
        [IDReferenceProperty(typeof(WebControl))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase", Justification = "Following ASP.NET AJAX pattern")]
        public string CancelControlID
        {
            get { return GetPropertyValue(stringCancelControlID, ""); }
            set { SetPropertyValue(stringCancelControlID, value); }
        }

        [ExtenderControlProperty()]
        [DefaultValue("")]
        public string OnOkScript
        {
            get { return GetPropertyValue(stringOnOkScript, ""); }
            set { SetPropertyValue(stringOnOkScript, value); }
        }

        [ExtenderControlProperty()]
        [DefaultValue("")]
        public string OnCancelScript
        {
            get { return GetPropertyValue(stringOnCancelScript, ""); }
            set { SetPropertyValue(stringOnCancelScript, value); }
        }

        /// <summary>
        /// Cause the ModalPopup to be shown by sending script to the client
        /// </summary>
        public void Show()
        {
            ChangeVisibility(true);
        }

        /// <summary>
        /// Cause the ModalPopup to be hidden by sending script to the client
        /// </summary>
        public void Hide()
        {
            ChangeVisibility(false);
        }

        /// <summary>
        /// Emit script to the client that will cause the modal popup behavior
        /// to be shown or hidden
        /// </summary>
        /// <param name="show">True to show the popup, false to hide it</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Justification = "Assembly is not localized")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "Raising for property, not parameter")]
        private void ChangeVisibility(bool show)
        {
            if (TargetControl == null)
            {
                throw new ArgumentNullException("TargetControl", "TargetControl property cannot be null");
            }

            string operation = show ? "show" : "hide";

            if (ScriptManager.GetCurrent(Page).IsInAsyncPostBack)
            {
                // RegisterDataItem is more elegant, but we can only call it during an async postback
                ScriptManager.GetCurrent(Page).RegisterDataItem(TargetControl, operation);
            }
            else
            {
                // Add a load handler to show the popup and then remove itself
                string script = string.Format(CultureInfo.InvariantCulture,
                    "(function() {{" +
                        "var fn = function() {{" +
                            "AjaxControlToolkit.ModalPopupBehavior.invokeViaServer('{0}', {1}); " +
                            "Sys.Application.remove_load(fn);" +
                        "}};" +
                        "Sys.Application.add_load(fn);" +
                    "}})();",
                    BehaviorID,
                    show ? "true" : "false");
                ScriptManager.RegisterStartupScript(this, typeof(ModalPopupExtender), operation + BehaviorID, script, true);
            }
        }
    }
}
