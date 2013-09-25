// README
//
// There are two steps to adding a property:
//
// 1. Create a member variable to store your property
// 2. Add the get_ and set_ accessors for your property.
//
// Remember that both are case sensitive!
//

Type.registerNamespace('AjaxCtrl.Net');

AjaxCtrl.Net.QueryPopularityBehavior = function(element) {

    AjaxCtrl.Net.QueryPopularityBehavior.initializeBase(this, [element]);

    // TODO : (Step 1) Add your property variables here
    //
    this._servicePath = null;
    this._serviceMethod = null;
    this._minimumPrefixLength = 2;
    this._completionInterval = 1000;
    this._thaiRatingPopularityElementID = null;
    this._thaiRatingPopularityElement = null;
    this._timer = null;
    this._cache = null;
    this._currentPrefix = null;
    this._focusHandler = null;
    this._blurHandler = null;
    this._keyDownHandler = null;
    this._ratingBehavior = null;
    this._tickHandler = null;
    this._enableCaching = true;
}

AjaxCtrl.Net.QueryPopularityBehavior.prototype = {

    initialize : function() {
        AjaxCtrl.Net.QueryPopularityBehavior.callBaseMethod(this, 'initialize');

        /// initialize the event handlers
        this._tickHandler = Function.createDelegate(this, this._onTimerTick);
        this._focusHandler = Function.createDelegate(this, this._onGotFocus);
        this._blurHandler = Function.createDelegate(this, this._onLostFocus);
        this._keyDownHandler = Function.createDelegate(this, this._onKeyDown);
        
        /// initialize the timer to tick handler
        this._timer = new Sys.Timer();
        this.initializeTimer(this._timer);
        
        var element = this.get_element();
        this.initializeTextBox(element);
        
        /// initialize the informational control
        if (this._thaiRatingPopularityElementID !== null)
            this._thaiRatingPopularityElement = $get(this._thaiRatingPopularityElementID);
        if (this._thaiRatingPopularityElement == null ) {
            this._thaiRatingPopularityElement = document.createElement('DIV');
            this._thaiRatingPopularityElement.id = this.get_id() + '__thaiRatingPopularityElem';

            // Safari styles the element incorrectly if it's added to the desired location
            if (Sys.Browser.agent == Sys.Browser.Safari) {
                document.body.appendChild(this._thaiRatingPopularityElement);
            } else {
                element.parentNode.appendChild(this._thaiRatingPopularityElement);
            }
        }
        
        this.initializeThaiRatingPopularityElement(this._thaiRatingPopularityElement);
        
        this._ratingBehavior = $create(AjaxControlToolkit.RatingBehavior, 
                { 'id':this.get_id()+'RatingBehavior' }, null, null, this._thaiRatingPopularityElement);
    },

    dispose : function() {
        if (this._ratingBehavior) {
            this._ratingBehavior.dispose();
            this._ratingBehavior = null;
        }
        if(this._timer) {        
            this._timer.dispose();
            this._timer = null;
        }

        var element = this.get_element();
        if(element) {
            $removeHandler(element, "focus", this._focusHandler);
            $removeHandler(element, "blur", this._blurHandler);
            $removeHandler(element, "keydown", this._keyDownHandler);
        }
        
        this._tickHandler = null;
        this._focusHandler = null;
        this._blurHandler = null;
        this._keyDownHandler = null;

        AjaxCtrl.Net.QueryPopularityBehavior.callBaseMethod(this, 'dispose');
    },

    // TODO: (Step 2) Add your property accessors here
    //
    get_completionInterval: function() {
        /// <value type="Number">Auto completion timer interval in milliseconds.</value>
        return this._completionInterval;
    },
    set_completionInterval: function(value) {
        this._completionInterval = value;
    },
    
    get_thaiRatingPopularityElement: function() {
        /// <value domElement="true">List dom element.</value>
        return this._thaiRatingPopularityElement;
    },
    set_thaiRatingPopularityElement: function(value) {
        this._thaiRatingPopularityElement = value;
    },
    
    get_minimumPrefixLength: function() {
        /// <value type="Number">Minimum text prefix length required to perform behavior.</value>
        return this._minimumPrefixLength;
    },
    set_minimumPrefixLength: function(value) {
        this._minimumPrefixLength = value;
    },
    
    get_serviceMethod: function() {
        /// <value type="String">Web service method.</value>
        return this._serviceMethod;
    },
    set_serviceMethod: function(value) {
        this._serviceMethod = value;
    },
    
    get_servicePath: function() {
        /// <value type="String">Web service url.</value>
        return this._servicePath;
    },
    set_servicePath: function(value) {
        this._servicePath = value;
    },
    
    get_enableCaching: function() {
        /// <value type="Boolean">Get or sets whether suggestions retrieved from the webservice should be cached.</value>
        return this._enableCaching;
    },
    set_enableCaching: function(value) {
        this._enableCaching = value;
    },
    
    get_thaiRatingPopularityElementID: function(){
        /// <value type="String>ID of the completion div element. </value>
        return this._thaiRatingPopularityElementID;
    },
    set_thaiRatingPopularityElementID: function(value) {
        this._thaiRatingPopularityElementID = value;  
    },
    
    initializeTimer: function(timer) {
        timer.set_interval(this._completionInterval);
        timer.add_tick(this._tickHandler);
    },
    initializeTextBox: function(element) {
        element.autocomplete = "off";
        $addHandler(element, "focus", this._focusHandler);
        $addHandler(element, "blur", this._blurHandler);
        $addHandler(element, "keydown", this._keyDownHandler);
    },
    initializeThaiRatingPopularityElement: function(element) {
    },
    
    _onGotFocus: function(ev) {
        this._timer.set_enabled(true);
    },
    _onKeyDown: function(ev) {
        var k = ev.keyCode ? ev.keyCode : ev.rawEvent.keyCode;
        if (k === Sys.UI.Key.esc) {
        }
        else if (k === Sys.UI.Key.up) {
        }
        else if (k === Sys.UI.Key.down) {
        }
        else if (k === Sys.UI.Key.enter) {
        }
        else if (k === Sys.UI.Key.tab) {
        }
        else {
            this._timer.set_enabled(true);
        }
    },
    _onLostFocus: function() {
        this._timer.set_enabled(false);
        this._hideCompletionList();
    },
    _onMethodComplete: function(result, context, methodName) {
        this._update(context, result, /* cacheResults */ true);
    },
    _onMethodFailed: function(err, response, context) {
        // no op
    },
    _onTimerTick: function(sender, eventArgs) {
        if (this._servicePath && this._serviceMethod) {
            var text = this.get_element().value;
            
            if (text.trim().length < this._minimumPrefixLength) {
                this._currentPrefix = null;
                this._update('', null, /* cacheResults */ false);
                return;
            }
            
            if (this._currentPrefix !== text) {
                this._currentPrefix = text;
                if (this._cache && this._cache[text]) {
                    this._update(text, this._cache[text], /* cacheResults */ false);
                    return;
            }
            
            Sys.Net.WebServiceProxy.invoke(this.get_servicePath(), this.get_serviceMethod(), false,
                                        { query : this._currentPrefix },
                                        Function.createDelegate(this, this._onMethodComplete),
                                        Function.createDelegate(this, this._onMethodFailed),
                                        text);
            }
        }
    },
    _update: function(prefixText, popularityResult, cacheResults) {
        if (cacheResults && this.get_enableCaching()) {
            if (!this._cache) {
                this._cache = {};
            }
            this._cache[prefixText] = typeResult;
        }
    }
}

AjaxCtrl.Net.QueryPopularityBehavior.registerClass('AjaxCtrl.Net.QueryPopularityBehavior', AjaxControlToolkit.BehaviorBase);
