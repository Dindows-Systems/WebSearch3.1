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

AjaxCtrl.Net.QueryTypeIdentifierBehavior = function(element) {

    AjaxCtrl.Net.QueryTypeIdentifierBehavior.initializeBase(this, [element]);

    // TODO : (Step 1) Add your property variables here
    //
    this._servicePath = null;
    this._serviceMethod = null;
    this._minimumPrefixLength = 2;
    this._completionInterval = 1000;        
    this._completionInformationalID = null;
    this._completionNavigationalID = null;
    this._completionTransactionalID = null;
    this._progressBarID = null;
    this._completionInformational = null;
    this._completionNavigational = null;
    this._completionTransactional = null;
    this._progressBar = null;
    this._timer = null;
    this._cache = null;
    this._currentPrefix = null;
    this._focusHandler = null;
    this._blurHandler = null;
    this._keyDownHandler = null;
    this._tickHandler = null;
    this._enableCaching = true;
}

AjaxCtrl.Net.QueryTypeIdentifierBehavior.prototype = {

    initialize : function() {
        AjaxCtrl.Net.QueryTypeIdentifierBehavior.callBaseMethod(this, 'initialize');

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
        if (this._completionInformationalID !== null)
            this._completionInformational = $get(this._completionInformationalID);
        if (this._completionInformational == null ) {
            this._completionInformational = document.createElement('DIV');
            this._completionInformational.id = this.get_id() + '_completionInformational';

            // Safari styles the element incorrectly if it's added to the desired location
            if (Sys.Browser.agent === Sys.Browser.Safari) {
                document.body.appendChild(this._completionInformational);
            } else {
                element.parentNode.appendChild(this._completionInformational);
            }
        }
        
        this.initializeCompletionInformational(this._completionInformational);
        
        /// initialize the navigational control
        if (this._completionNavigationalID !== null)
            this._completionNavigational = $get(this._completionNavigationalID);
        if (this._completionNavigational == null ) {
            this._completionNavigational = document.createElement('DIV');
            this._completionNavigational.id = this.get_id() + '_completionNavigational';

            // Safari styles the element incorrectly if it's added to the desired location
            if (Sys.Browser.agent === Sys.Browser.Safari) {
                document.body.appendChild(this._completionNavigational);
            } else {
                element.parentNode.appendChild(this._completionNavigational);
            }
        }
        
        this.initializeCompletionNavigational(this._completionNavigational);
        
        /// initialize the transactional control
        if (this._completionTransactionalID !== null)
            this._completionTransactional = $get(this._completionTransactionalID);
        if (this._completionTransactional == null ) {
            this._completionTransactional = document.createElement('DIV');
            this._completionTransactional.id = this.get_id() + '_completionTransactional';

            // Safari styles the element incorrectly if it's added to the desired location
            if (Sys.Browser.agent === Sys.Browser.Safari) {
                document.body.appendChild(this._completionTransactional);
            } else {
                element.parentNode.appendChild(this._completionTransactional);
            }
        }
        
        this.initializeCompletionTransactional(this._completionTransactional);
        
        /// initialize the progress bar control
        if (this._progressBarID != null)
            this._progressBar = $get(this._progressBarID);
        if (this._progressBar == null) {
            this._progressBar = document.createElement('DIV');
            this._progressBar.id = this.get_id() + '_progressBar';
            
            // Safari styles the element incorrectly if it's added to the desired location
            if (Sys.Browser.agent === Sys.Browser.Safari) {
                document.body.appendChild(this._progressBar);
            } else {
                element.parentNode.appendChild(this._progressBar);
            }
        }
        
        this.initializeProgressBar(this._progressBar);
    },

    dispose : function() {
        if(this._timer) {        
            this._timer.dispose();
            this._timer = null;
        }

        var element = this.get_element();
        if (element) {
            $removeHandler(element, "focus", this._focusHandler);
            $removeHandler(element, "blur", this._blurHandler);
            $removeHandler(element, "keydown", this._keyDownHandler);
        }
        
        this._tickHandler = null;
        this._focusHandler = null;
        this._blurHandler = null;
        this._keyDownHandler = null;
        
        AjaxCtrl.Net.QueryTypeIdentifierBehavior.callBaseMethod(this, 'dispose');
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
    
    get_completionInformational: function() {
        /// <value domElement="true"> dom element.</value>
        return this._completionInformational;
    },
    set_completionInformational: function(value) {
        this._completionInformational = value;
    },
    
    get_completionNavigational: function() {
        /// <value domElement="true"> dom element.</value>
        return this._completionNavigational;
    },
    set_completionNavigational: function(value) {
        this._completionNavigational = value;
    },
    
    get_completionTransactional: function() {
        /// <value domElement="true"> dom element.</value>
        return this._completionTransactional;
    },
    set_completionTransactional: function(value) {
        this._completionTransactional = value;
    },
    
    get_progressBar: function() {
        /// <value domElement="true"> dom element.</value>
        return this._progressBar;
    },
    set_progressBar: function(value) {
        this._progressBar = value;
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
    
    get_completionInformationalID: function(){
        /// <value type="String>ID of the completion div element. </value>
        return this._completionInformationalID;
    },
    set_completionInformationalID: function(value) {
        this._completionInformationalID = value;  
    },
    
    get_completionNavigationalID: function(){
        /// <value type="String>ID of the completion div element. </value>
        return this._completionNavigationalID;
    },
    set_completionNavigationalID: function(value) {
        this._completionNavigationalID = value;  
    },
    
    get_completionTransactionalID: function(){
        /// <value type="String>ID of the completion div element. </value>
        return this._completionTransactionalID;
    },
    set_completionTransactionalID: function(value) {
        this._completionTransactionalID = value;  
    },
    
    get_progressBarID: function() {
        /// <value domElement="true"> dom element.</value>
        return this._progressBarID;
    },
    set_progressBarID: function(value) {
        this._progressBarID = value;
    },
    
    _onMethodComplete: function(result, context, methodName) {
        this._update(context, result, /* cacheResults */ true);
        this._progressBar.src = "images/free.gif";
    },
    _onMethodFailed: function(err, response, context) {
        this._progressBar.src = "images/error.gif";
    },
    _onTimerTick: function(sender, eventArgs) {
        if (this._servicePath && this._serviceMethod) {
            var text = this.get_element().value;
            
            if (text.trim().length < this._minimumPrefixLength) {
                this._currentPrefix = null;
                this._update('', null, /* cacheResults */ false);
                return;
            }
            
            if (this._currentPrefix != text) {
                this._currentPrefix = text;
                if (this._cache && this._cache[text]) {
                    this._update(text, this._cache[text], /* cacheResults */ false);
                    return;
                }

                Sys.Net.WebServiceProxy.invoke(this.get_servicePath(), this.get_serviceMethod(), false,
                    { query : this._currentPrefix }, Function.createDelegate(this, this._onMethodComplete),
                    Function.createDelegate(this, this._onMethodFailed), text);

                // make the progress bar visible
                this._progressBar.src = "images/loading.gif";
            }
        }
    },
    _onGotFocus: function(ev) {
        this._timer.set_enabled(true);
    },
    _onLostFocus: function() {
        this._timer.set_enabled(false);
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
    
    initializeTimer: function(timer) {
        timer.set_interval(this._completionInterval);
        timer.add_tick(this._tickHandler);
    },
    initializeTextBox: function(element) {
        $addHandler(element, "focus", this._focusHandler);
        $addHandler(element, "blur", this._blurHandler);
        $addHandler(element, "keydown", this._keyDownHandler);
    },
    initializeCompletionInformational: function(element) {
    },
    initializeCompletionNavigational: function(element) {
    },
    initializeCompletionTransactional: function(element) {
    },
    initializeProgressBar: function(element) {
    },
    
    _update: function(prefixText, typeResult, cacheResults) {
        if (cacheResults && this.get_enableCaching()) {
            if (!this._cache) {
                this._cache = {};
            }
            this._cache[prefixText] = typeResult;
        }

        if (typeResult == 1)
            this._completionInformational.checked="checked";
        else if (typeResult == 2)
            this._completionNavigational.checked="checked";
        else if (typeResult == 3)
            this._completionTransactional.checked="checked";
        else
        {
            this._completionInformational.checked="";
            this._completionNavigational.checked="";
            this._completionTransactional.checked="";
        }
    }
}

AjaxCtrl.Net.QueryTypeIdentifierBehavior.registerClass('AjaxCtrl.Net.QueryTypeIdentifierBehavior', AjaxControlToolkit.BehaviorBase);
