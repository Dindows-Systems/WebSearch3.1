// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

Type.registerNamespace('AjaxCtrl.Net');

AjaxCtrl.Net.ModalPopupBehavior = function(element) {
    /// <summary>
    /// The ModalPopupBehavior is used to display the target element as a modal dialog
    /// </summary>
    /// <param name="element" type="Sys.UI.DomElement" domElement="true">
    /// DOM Element the behavior is associated with
    /// </param>
    AjaxCtrl.Net.ModalPopupBehavior.initializeBase(this, [element]);
    
    // Properties
    this._PopupControlID = null;
    this._PopupDragHandleControlID = null;
    this._BackgroundCssClass = null;
    this._DropShadow = false;
    this._Drag = false;    
    this._OkControlID = null;
    this._CancelControlID = null;
    this._OnOkScript = null;
    this._OnCancelScript = null;
    this._xCoordinate = -1;
    this._yCoordinate = -1;
    this._maskWidth = -1;
    this._maskHeight = -1;

    // Variables
    this._backgroundElement = null;
    this._foregroundElement = null;
    this._popupElement = null;
    this._dragHandleElement = null;
    this._showHandler = null;
    this._okHandler = null;
    this._cancelHandler = null;
    this._scrollHandler = null;
    this._resizeHandler = null;
    this._windowHandlersAttached = false;

    this._dropShadowBehavior = null;
    this._dragBehavior = null;

    this._saveTabIndexes = new Array();
    this._saveDesableSelect = new Array();
    this._tagWithTabIndex = new Array('A','AREA','BUTTON','INPUT','OBJECT','SELECT','TEXTAREA','IFRAME');
}
AjaxCtrl.Net.ModalPopupBehavior.prototype = {
    initialize : function() {
        /// <summary>
        /// Initialize the behavior
        /// </summary>
        
        /*
            <div superpopup - drag container resizable><div -- drag handle\dropshadow foreground></div></div>
        */
        AjaxCtrl.Net.ModalPopupBehavior.callBaseMethod(this, 'initialize');
        if(this._PopupDragHandleControlID)
            this._dragHandleElement = $get(this._PopupDragHandleControlID);

        this._popupElement = $get(this._PopupControlID);
        if(this._DropShadow)
        {
            this._foregroundElement = document.createElement('div');
            this._popupElement.parentNode.appendChild(this._foregroundElement);
            this._foregroundElement.appendChild(this._popupElement);
        }
        else
        {
            this._foregroundElement = $get(this._PopupControlID);
        }
        this._backgroundElement = document.createElement('div');
        this._backgroundElement.style.display = 'none';
        this._backgroundElement.style.position = 'absolute';
        this._backgroundElement.style.left = '0px';
        this._backgroundElement.style.top = '0px';
        // Want zIndex to big enough that the background sits above everything else
        // CSS 2.1 defines no bounds for the <integer> type, so pick arbitrarily
        this._backgroundElement.style.zIndex = 10000;
        if (this._BackgroundCssClass) {
            this._backgroundElement.className = this._BackgroundCssClass;
        }
        this._foregroundElement.parentNode.appendChild(this._backgroundElement);

        this._foregroundElement.style.display = 'none';
        this._foregroundElement.style.position = 'absolute';
        this._foregroundElement.style.zIndex = CommonToolkitScripts.getCurrentStyle(this._backgroundElement, 'zIndex', this._backgroundElement.style.zIndex) + 1;
        
        this._showHandler = Function.createDelegate(this, this._onShow);
        $addHandler(this.get_element(), 'click', this._showHandler);

        if (this._OkControlID) {
            this._okHandler = Function.createDelegate(this, this._onOk);
            $addHandler($get(this._OkControlID), 'click', this._okHandler);
        }

        if (this._CancelControlID) {
            this._cancelHandler = Function.createDelegate(this, this._onCancel);
            $addHandler($get(this._CancelControlID), 'click', this._cancelHandler);
        }

        this._scrollHandler = Function.createDelegate(this, this._onLayout);
        this._resizeHandler = Function.createDelegate(this, this._onLayout);

        // Need to know when partial updates complete
        this.registerPartialUpdateEvents();
    },

    dispose : function() {
        /// <summary>
        /// Dispose the behavior
        /// </summary>
        this._detachPopup();

        if(this._DropShadow)
        {
            // Remove DIV wrapper added in initialize
            this._foregroundElement.parentNode.appendChild(this._popupElement);
            this._foregroundElement.parentNode.removeChild(this._foregroundElement);
        }

        this._scrollHandler = null;
        this._resizeHandler = null;
        if (this._cancelHandler && $get(this._CancelControlID)) {
            $removeHandler($get(this._CancelControlID), 'click', this._cancelHandler);
            this._cancelHandler = null;
        }
        if (this._okHandler && $get(this._OkControlID)) {
            $removeHandler($get(this._OkControlID), 'click', this._okHandler);
            this._okHandler = null;
        }
        if (this._showHandler) {
            $removeHandler(this.get_element(), 'click', this._showHandler);
            this._showHandler = null;
        }
        
        AjaxCtrl.Net.ModalPopupBehavior.callBaseMethod(this, 'dispose');
    },

    _attachPopup : function() {
        /// <summary>
        /// Attach the event handlers for the popup
        /// </summary>

        if (this._DropShadow && !this._dropShadowBehavior) {
            this._dropShadowBehavior = $create(AjaxControlToolkit.DropShadowBehavior, {}, null, null, this._popupElement);
        }
        if (this._dragHandleElement && !this._dragBehavior) {
            this._dragBehavior = $create(AjaxControlToolkit.FloatingBehavior, {"handle" : this._dragHandleElement}, null, null, this._foregroundElement);
        }        
        $addHandler(window, 'resize', this._resizeHandler);
        $addHandler(window, 'scroll', this._scrollHandler);
        this._windowHandlersAttached = true;
    },

    _detachPopup : function() {
        /// <summary>
        /// Detach the event handlers for the popup
        /// </summary>

        if (this._windowHandlersAttached) {
            if (this._scrollHandler) {
                $removeHandler(window, 'scroll', this._scrollHandler);
            }

            if (this._resizeHandler) {
                $removeHandler(window, 'resize', this._resizeHandler);
            }
            this._windowHandlersAttached = false;
        }
        
        if (this._dragBehavior) {
            this._dragBehavior.dispose();
            this._dragBehavior = null;
        }       
        
        if (this._dropShadowBehavior) {
            this._dropShadowBehavior.dispose();
            this._dropShadowBehavior = null;
        }

    },

    _onShow : function(e) {
        /// <summary>
        /// Handler for the target's click event
        /// </summary>
        /// <param name="e" type="Sys.UI.DomEvent">
        /// Event info
        /// </param>

        if (!this.get_element().disabled) {
            this.show();
            e.preventDefault();
            return false;
        }
    },

    _onOk : function(e) {
        /// <summary>
        /// Handler for the modal dialog's OK button click
        /// </summary>
        /// <param name="e" type="Sys.UI.DomEvent">
        /// Event info
        /// </param>

        var element = $get(this._OkControlID);
        if (element && !element.disabled) {
            this.hide();
            e.preventDefault();
            if (this._OnOkScript) {
                window.setTimeout(this._OnOkScript, 0);
            }
            return false;
        }
    },

    _onCancel : function(e) {
        /// <summary>
        /// Handler for the modal dialog's Cancel button click
        /// </summary>
        /// <param name="e" type="Sys.UI.DomEvent">
        /// Event info
        /// </param>

        var element = $get(this._CancelControlID);
        if (element && !element.disabled) {
            this.hide();
            e.preventDefault();
            if (this._OnCancelScript) {
                window.setTimeout(this._OnCancelScript, 0);
            }
            return false;
        }
    },

    _onLayout : function() {
        /// <summary>
        /// Handler for scrolling and resizing events that would require a repositioning of the modal dialog
        /// </summary>
        this._layout();
    },

    show : function() {
        /// <summary>
        /// Display the element referenced by PopupControlID as a modal dialog
        /// </summary>
        AjaxCtrl.Net.ModalPopupBehavior.callBaseMethod(this, 'populate');

        this.raiseShowing();

        this._attachPopup();

        this._backgroundElement.style.display = '';
        this._foregroundElement.style.display = '';
        this._popupElement.style.display = '';

        // Disable TAB
        this.disableTab();

        this._layout();
        // On pages that don't need scrollbars, Firefox and Safari act like
        // one or both are present the first time the layout code runs which
        // obviously leads to display issues - run the layout code a second
        // time to work around this problem
        this._layout();
        
        this.raiseShown();
    },

    disableTab : function() {
        /// <summary>
        /// Change the tab indices so we only tab through the modal popup
        /// (and hide SELECT tags in IE6)
        /// </summary>

        var i = 0;
        var tagElements;
        var tagElementsInPopUp = new Array();
        Array.clear(this._saveTabIndexes);

        //Save all popup's tag in tagElementsInPopUp
        for (var j = 0; j < this._tagWithTabIndex.length; j++) {
            tagElements = this._foregroundElement.getElementsByTagName(this._tagWithTabIndex[j]);
            for (var k = 0 ; k < tagElements.length; k++) {
                tagElementsInPopUp[i] = tagElements[k];
                i++;
            }
        }

        i = 0;
        for (var j = 0; j < this._tagWithTabIndex.length; j++) {
            tagElements = document.getElementsByTagName(this._tagWithTabIndex[j]);
            for (var k = 0 ; k < tagElements.length; k++) {
                if (Array.indexOf(tagElementsInPopUp, tagElements[k]) == -1)  {
                    this._saveTabIndexes[i] = {tag: tagElements[k], index: tagElements[k].tabIndex};
                    tagElements[k].tabIndex="-1";
                    i++;
                }
            }
        }

        //IE6 Bug with SELECT element always showing up on top
        i = 0;
        if ((Sys.Browser.agent === Sys.Browser.InternetExplorer) && (Sys.Browser.version < 7)) {
            //Save SELECT in PopUp
            var tagSelectInPopUp = new Array();
            for (var j = 0; j < this._tagWithTabIndex.length; j++) {
                tagElements = this._foregroundElement.getElementsByTagName('SELECT');
                for (var k = 0 ; k < tagElements.length; k++) {
                    tagSelectInPopUp[i] = tagElements[k];
                    i++;
                }
            }

            i = 0;
            Array.clear(this._saveDesableSelect);
            tagElements = document.getElementsByTagName('SELECT');
            for (var k = 0 ; k < tagElements.length; k++) {
                if (Array.indexOf(tagSelectInPopUp, tagElements[k]) == -1)  {
                    this._saveDesableSelect[i] = {tag: tagElements[k], visib: CommonToolkitScripts.getCurrentStyle(tagElements[k], 'visibility')} ;
                    tagElements[k].style.visibility = 'hidden';
                    i++;
                }
            }
        }
    },

    restoreTab : function() {
        /// <summary>
        /// Restore the tab indices so we tab through the page like normal
        /// (and restore SELECT tags in IE6)
        /// </summary>

        for (var i = 0; i < this._saveTabIndexes.length; i++) {
            this._saveTabIndexes[i].tag.tabIndex = this._saveTabIndexes[i].index;
        }

        //IE6 Bug with SELECT element always showing up on top
        if ((Sys.Browser.agent === Sys.Browser.InternetExplorer) && (Sys.Browser.version < 7)) {
            for (var k = 0 ; k < this._saveDesableSelect.length; k++) {
                this._saveDesableSelect[k].tag.style.visibility = this._saveDesableSelect[k].visib;
            }
        }
    },

    hide : function() {
        /// <summary>
        /// Hide the modal dialog
        /// </summary>

        this.raiseHiding();
        
        this._backgroundElement.style.display = 'none';
        this._foregroundElement.style.display = 'none';

        this.restoreTab();

        this._detachPopup();
        
        this.raiseHidden();
    },

    _layout : function() {
        /// <summary>
        /// Position the modal dialog in the center of the screen
        /// </summary>

        var scrollLeft = (document.documentElement.scrollLeft ? document.documentElement.scrollLeft : document.body.scrollLeft);
        var scrollTop = (document.documentElement.scrollTop ? document.documentElement.scrollTop : document.body.scrollTop);
        var clientBounds = CommonToolkitScripts.getClientBounds();
        var clientWidth = clientBounds.width;
        var clientHeight = clientBounds.height;
        if (this._maskWidth == -1)
            this._backgroundElement.style.width = Math.max(Math.max(document.documentElement.scrollWidth, document.body.scrollWidth), clientWidth)+'px';
        else
            this._backgroundElement.style.width = this._maskWidth + 'px';
        
        if (this._maskHeight == -1)
            this._backgroundElement.style.height = Math.max(Math.max(document.documentElement.scrollHeight, document.body.scrollHeight), clientHeight)+'px';
        else
            this._backgroundElement.style.height = this._maskHeight + 'px';

        var isIE6 = (Sys.Browser.agent == Sys.Browser.InternetExplorer && Sys.Browser.version < 7);
        if(this._xCoordinate < 0)
        {
            this._foregroundElement.style.left = scrollLeft+((clientWidth-this._foregroundElement.offsetWidth)/2)+'px';
        }
        else
        {
            if(isIE6)
            {
                this._foregroundElement.style.position = 'absolute';
                this._foregroundElement.style.left = (this._xCoordinate + scrollLeft) + 'px';
            }
            else
            {
                this._foregroundElement.style.position = 'fixed';
                this._foregroundElement.style.left = this._xCoordinate + 'px';
            }
        }
        if(this._yCoordinate < 0)
        {
            this._foregroundElement.style.top = scrollTop+((clientHeight-this._foregroundElement.offsetHeight)/2)+'px';
        }
        else
        {
            if(isIE6)
            {
                this._foregroundElement.style.position = 'absolute';
                this._foregroundElement.style.top = (this._yCoordinate + scrollTop) + 'px';
            }
            else
            {
                this._foregroundElement.style.position = 'fixed';
                this._foregroundElement.style.top = this._yCoordinate + 'px';
            }
        }
        
        if (this._dropShadowBehavior) {
            this._dropShadowBehavior.setShadow();
            window.setTimeout(Function.createDelegate(this, this._fixupDropShadowBehavior), 0);
        }
    },

    _fixupDropShadowBehavior : function() {
        /// <summary>
        /// Some browsers don't update the location values immediately, so
        /// the location of the drop shadow would always be a step behind
        /// without this method
        /// </summary>

        if (this._dropShadowBehavior) {
            this._dropShadowBehavior.setShadow();
        }
    },

    // Show the popup if asked to
    _partialUpdateEndRequest : function(sender, endRequestEventArgs) {
        AjaxCtrl.Net.ModalPopupBehavior.callBaseMethod(this, '_partialUpdateEndRequest', [sender, endRequestEventArgs]);

        if (this.get_element()) {
            // Look up result by element's ID
            var action = endRequestEventArgs.get_dataItems()[this.get_element().id];
            if ("show" == action) {
                this.show();
            } else if ("hide" == action) {
                this.hide();
            }
        }

        // Async postback may have added content; re-layout to accomodate it
        this._layout();
    },

    _onPopulated : function(sender, eventArgs) {
        AjaxCtrl.Net.ModalPopupBehavior.callBaseMethod(this, '_onPopulated', [sender, eventArgs]);

        // Dynamic populate may have added content; re-layout to accomodate it
        this._layout();
    },
    
    raiseShowing : function() {
        /// <summary>
        /// Raise the <code>showing</code> event
        /// </summary>
        /// <returns />
        var handlers = this.get_events().getHandler('showing');
        if (handlers) {
            handlers(this, Sys.EventArgs.Empty);
        }
    },
    
    add_showing : function(handler) {
        /// <summary>
        /// Adds an event handler for the <code>showiwng</code> event.
        /// </summary>
        /// <param name="handler" type="Function">
        /// The handler to add to the event.
        /// </param>
        /// <returns />
        this.get_events().addHandler("showing", handler);
    },
    
    remove_showing : function(handler) {
        /// <summary>
        /// Removes an event handler for the <code>showing</code> event.
        /// </summary>
        /// <param name="handler" type="Function">
        /// The handler to remove from the event.
        /// </param>
        /// <returns />
        this.get_events().removeHandler("showing", handler);
    },
    
    raiseShown : function() {
        /// <summary>
        /// Raise the <code>shown</code> event
        /// </summary>
        /// <returns />
        var handlers = this.get_events().getHandler('shown');
        if (handlers) {
            handlers(this, Sys.EventArgs.Empty);
        }
    },
    
    add_shown : function(handler) {
        /// <summary>
        /// Adds an event handler for the <code>shown</code> event.
        /// </summary>
        /// <param name="handler" type="Function">
        /// The handler to add to the event.
        /// </param>
        /// <returns />
        this.get_events().addHandler("shown", handler);
    },
    
    remove_shown : function(handler) {
        /// <summary>
        /// Removes an event handler for the <code>shown</code> event.
        /// </summary>
        /// <param name="handler" type="Function">
        /// The handler to remove from the event.
        /// </param>
        /// <returns />
        this.get_events().removeHandler("shown", handler);
    },
    
    raiseHiding : function() {
        /// <summary>
        /// Raise the <code>hiding</code> event
        /// </summary>
        /// <returns />
        var handlers = this.get_events().getHandler('hiding');
        if (handlers) {
            handlers(this, Sys.EventArgs.Empty);
        }
    },
    
    add_hiding : function(handler) {
        /// <summary>
        /// Adds an event handler for the <code>hiding</code> event.
        /// </summary>
        /// <param name="handler" type="Function">
        /// The handler to add to the event.
        /// </param>
        /// <returns />
        this.get_events().addHandler("hiding", handler);
    },
    
    remove_hiding : function(handler) {
        /// <summary>
        /// Removes an event handler for the <code>hiding</code> event.
        /// </summary>
        /// <param name="handler" type="Function">
        /// The handler to remove from the event.
        /// </param>
        /// <returns />
        this.get_events().removeHandler("hiding", handler);
    },
    
    raiseHidden : function() {
        /// <summary>
        /// Raise the <code>hidden</code> event
        /// </summary>
        /// <returns />
        var handlers = this.get_events().getHandler('hidden');
        if (handlers) {
            handlers(this, Sys.EventArgs.Empty);
        }
    },
    
    add_hidden : function(handler) {
        /// <summary>
        /// Adds an event handler for the <code>hidden</code> event.
        /// </summary>
        /// <param name="handler" type="Function">
        /// The handler to add to the event.
        /// </param>
        /// <returns />
        this.get_events().addHandler("hidden", handler);
    },
    
    remove_hidden : function(handler) {
        /// <summary>
        /// Removes an event handler for the <code>hidden</code> event.
        /// </summary>
        /// <param name="handler" type="Function">
        /// The handler to remove from the event.
        /// </param>
        /// <returns />
        this.get_events().removeHandler("hidden", handler);
    },

    get_PopupControlID : function() {
        /// <value type="String">
        /// The ID of the element to display as a modal popup
        /// </value>
        return this._PopupControlID;
    },
    set_PopupControlID : function(value) {
        if (this._PopupControlID != value) {
            this._PopupControlID = value;
            this.raisePropertyChanged('PopupControlID');
        }
    },

    get_X: function() {
        /// <value type="Number" integer="true">
        /// The number of pixels from the left of the browser to position the modal popup.
        /// </value>
        return this._xCoordinate;
    },
    set_X: function(value) {
        if (this._xCoordinate != value) {
            this._xCoordinate = value;
            this.raisePropertyChanged('X');
        }
    },

    get_Y: function() {
        /// <value type="Number" integer="true">
        /// The number of pixels from the top of the browser to position the modal popup.
        /// </value>
        return this._yCoordinate;
    },
    set_Y: function(value) {
        if (this._yCoordinate != value) {
            this._yCoordinate = value;
            this.raisePropertyChanged('Y');
        }
    },
    
    get_MaskWidth: function() {
        return this._maskWidth;
    },
    set_MaskWidth: function(value) {
        if (this._maskWidth != value) {
            this._maskWidth = value;
            this.raisePropertyChanged('MaskWidth');
        }
    },
    
    get_MaskHeight: function() {
        return this._maskHeight;
    },
    set_MaskHeight: function(value) {
        if (this._maskHeight != value) {
            this._maskHeight = value;
            this.raisePropertyChanged('MaskHeight');
        }
    },
       
    get_PopupDragHandleControlID : function() {
        /// <value type="String">
        /// The ID of the element to display as the drag handle for the modal popup
        /// </value>
        return this._PopupDragHandleControlID;
    },
    set_PopupDragHandleControlID : function(value) {
        if (this._PopupDragHandleControlID != value) {
            this._PopupDragHandleControlID = value;
            this.raisePropertyChanged('PopupDragHandleControlID');
        }
    },

    get_BackgroundCssClass : function() {
        /// <value type="String">
        /// The CSS class to apply to the background when the modal popup is displayed
        /// </value>
        return this._BackgroundCssClass;
    },
    set_BackgroundCssClass : function(value) {
        if (this._BackgroundCssClass != value) {
            this._BackgroundCssClass = value;
            this.raisePropertyChanged('BackgroundCssClass');
        }
    },

    get_DropShadow : function() {
        /// <value type="Boolean">
        /// Whether or not a drop-shadow should be added to the modal popup
        /// </value>
        return this._DropShadow;
    },
    set_DropShadow : function(value) {
        if (this._DropShadow != value) {
            this._DropShadow = value;
            this.raisePropertyChanged('DropShadow');
        }
    },

    get_Drag : function() {
        /// <value type="Boolean">
        /// Obsolete: Setting the _Drag property is a noop
        /// </value>
        return this._Drag;
    },
    set_Drag : function(value) {
        if (this._Drag != value) {
            this._Drag = value;
            this.raisePropertyChanged('Drag');
        }
    },

    get_OkControlID : function() {
        /// <value type="String">
        /// The ID of the element that dismisses the modal popup
        /// </value>
        return this._OkControlID;
    },
    set_OkControlID : function(value) {
        if (this._OkControlID != value) {
            this._OkControlID = value;
            this.raisePropertyChanged('OkControlID');
        }
    },

    get_CancelControlID : function() {
        /// <value type="String">
        /// The ID of the element that cancels the modal popup
        /// </value>
        return this._CancelControlID;
    },
    set_CancelControlID : function(value) {
        if (this._CancelControlID != value) {
            this._CancelControlID = value;
            this.raisePropertyChanged('CancelControlID');
        }
    },

    get_OnOkScript : function() {
        /// <value type="String">
        /// Script to run when the modal popup is dismissed with the OkControlID
        /// </value>
        return this._OnOkScript;
    },
    set_OnOkScript : function(value) {
        if (this._OnOkScript != value) {
            this._OnOkScript = value;
            this.raisePropertyChanged('OnOkScript');
        }
    },

    get_OnCancelScript : function() {
        /// <value type="String">
        /// Script to run when the modal popup is dismissed with the CancelControlID
        /// </value>
        return this._OnCancelScript;
    },
    set_OnCancelScript : function(value) {
        if (this._OnCancelScript != value) {
            this._OnCancelScript = value;
            this.raisePropertyChanged('OnCancelScript');
        }
    }
}
AjaxCtrl.Net.ModalPopupBehavior.registerClass('AjaxCtrl.Net.ModalPopupBehavior', AjaxControlToolkit.DynamicPopulateBehaviorBase);
//    getDescriptor : function() {
//        var td = AjaxCtrl.Net.ModalPopupBehavior.callBaseMethod(this, 'getDescriptor');
//        //  Add property declarations
//        td.addProperty('PopupControlID', String);
//        td.addProperty('BackgroundCssClass', String);
//        td.addProperty('DropShadow', Boolean);
//        td.addProperty('OkControlID', String);
//        td.addProperty('CancelControlID', String);
//        td.addProperty('OnOkScript', String);
//        td.addProperty('OnCancelScript', String);
//        return td;
//    },
AjaxCtrl.Net.ModalPopupBehavior.invokeViaServer = function(behaviorID, show) {
    /// <summary>
    /// This static function (that is intended to be called from script emitted
    /// on the server) will show or hide the behavior associated with behaviorID
    /// (i.e. to use this, the ModalPopupExtender must have an ID or BehaviorID) and
    /// will either show or hide depending on the show parameter.
    /// </summary>
    /// <param name="behaviorID" type="String">
    /// ID of the modal popup behavior
    /// </param>
    /// <param name="show" type="Boolean">
    /// Whether to show or hide the modal popup
    /// </param>
    var behavior = $find(behaviorID);
    if (behavior) {
        if (show) {
            behavior.show();
        } else {
            behavior.hide();
        }
    }
}
