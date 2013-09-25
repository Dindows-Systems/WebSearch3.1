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

AjaxCtrl.Net.DisableButtonBehavior = function(element) {

    AjaxCtrl.Net.DisableButtonBehavior.initializeBase(this, [element]);

    // TODO : (Step 1) Add your property variables here
    //
    this._TargetButtonID = null;
    this._DisabledTextValue = null;
    this._oldValue = "";
}

AjaxCtrl.Net.DisableButtonBehavior.prototype = {

    initialize : function() {
        AjaxCtrl.Net.DisableButtonBehavior.callBaseMethod(this, 'initialize');

        $addHandler(this.get_element(), 'keyup', Function.createDelegate(this, this._onkeyup));
        this._onkeyup();
    },

    dispose : function() {
        // TODO: add your cleanup code here

        AjaxCtrl.Net.DisableButtonBehavior.callBaseMethod(this, 'dispose');
    },

    // TODO: (Step 2) Add your property accessors here
    //
    _onkeyup : function() {
        var e = $get(this._TargetButtonID);
        if (e) {
            var disabled = ("" == this.get_element().value);
            e.disabled = disabled;
            if (this._DisabledTextValue) {
                if (disabled) {
                    this._oldValue = e.value;
                    e.value = this._DisabledTextValue;
                } else {
                    if (this._oldValue) {
                        e.value = this._oldValue;
                    }
                }
            }
        }
    },
    
    get_DisabledText : function() {
        return this._DisabledTextValue;
    },
    set_DisabledText : function(value) {
        this._DisabledTextValue = value;
    },
    
    get_TargetButtonID : function() {
        return this._TargetButtonID;
    },

    set_TargetButtonID : function(value) {
        this._TargetButtonID = value;
    }
}

AjaxCtrl.Net.DisableButtonBehavior.registerClass('AjaxCtrl.Net.DisableButtonBehavior', AjaxControlToolkit.BehaviorBase);
