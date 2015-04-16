/* Office Web Widgets - Experimental */
/* Version: 0.1 */
/*
	Copyright (c) Microsoft Corporation.  All rights reserved.
*/

/*
	Your use of this file is governed by the following license http://go.microsoft.com/fwlink/?LinkId=392925
*/
if (window.Type && window.Type.registerNamespace) {
    Type.registerNamespace('Office.Controls');
}
else {
    if (typeof window['Office'] == 'undefined') {
        window['Office'] = new Object();
        window['Office'].__namespace = true;
    }
    if (typeof window['Office']['Controls'] == 'undefined') {
        window['Office']['Controls'] = new Object();
        window['Office']['Controls'].__namespace = true;
    }
}
Office.Controls._principalSource = function() {
};
Office.Controls._principalSource.prototype = {
    cache: 1,
    server: 0
};
if (Office.Controls._principalSource.registerEnum)
    Office.Controls._principalSource.registerEnum('Office.Controls._principalSource', false);
Office.Controls.PrincipalInfo = function Office_Controls_PrincipalInfo() {
};
Office.Controls.PeoplePickerRecord = function Office_Controls_PeoplePickerRecord() {
};
Office.Controls.PeoplePickerRecord.prototype = {
    isResolved: false,
    text: null,
    department: null,
    displayName: null,
    email: null,
    jobTitle: null,
    loginName: null,
    mobile: null,
    principalId: 0,
    principalType: 0,
    sipAddress: null
};
Office.Controls._keyCodes = function Office_Controls__keyCodes() {
};
Office.Controls.PeoplePicker = function Office_Controls_PeoplePicker(root, parameterObject, dataProvider) {
    this._currentTimerId$p$0 = -1;
    this.selectedItems = new Array(0);
    this._internalSelectedItems$p$0 = new Array(0);
    this.errors = new Array(0);
    this._cache$p$0 = Office.Controls.PeoplePicker._mruCache.getInstance();
    if (typeof root !== 'object' || typeof parameterObject !== 'object') {
        Office.Controls.Utils.errorConsole('Invalid parameters type');
        return;
    }
    Office.Controls.Runtime._registerControl(root, this);
    this._root$p$0 = root;
    this._allowMultiple$p$0 = parameterObject.allowMultipleSelections;
    this._groupName$p$0 = parameterObject.groupName;
    this._onAdded$p$0 = parameterObject.onAdded;
    if (Office.Controls.Utils.isNullOrUndefined(this._onAdded$p$0)) {
        this._onAdded$p$0 = Office.Controls.PeoplePicker._nopAddRemove$p;
    }
    this._onRemoved$p$0 = parameterObject.onRemoved;
    if (Office.Controls.Utils.isNullOrUndefined(this._onRemoved$p$0)) {
        this._onRemoved$p$0 = Office.Controls.PeoplePicker._nopAddRemove$p;
    }
    this._onChange$p$0 = parameterObject.onChange;
    if (Office.Controls.Utils.isNullOrUndefined(this._onChange$p$0)) {
        this._onChange$p$0 = Office.Controls.PeoplePicker._nopChange$p;
    }
    if (!dataProvider) {
        this._dataProvider$p$0 = new Office.Controls.PeoplePicker._searchPrincipalServerDataProvider();
    }
    else {
        this._dataProvider$p$0 = dataProvider;
    }
    if (Office.Controls.Utils.isNullOrUndefined(parameterObject.displayErrors)) {
        this._showValidationErrors$p$0 = true;
    }
    else {
        this._showValidationErrors$p$0 = parameterObject.displayErrors;
    }
    if (!Office.Controls.Utils.isNullOrEmptyString(parameterObject.placeholder)) {
        this._defaultTextOverride$p$0 = parameterObject.placeholder;
    }
    this._renderControl$p$0(parameterObject.inputName);
    this._autofill$p$0 = new Office.Controls.PeoplePicker._autofillContainer(this);
};
Office.Controls.PeoplePicker._copyToRecord$i = function Office_Controls_PeoplePicker$_copyToRecord$i(record, info) {
    record.department = info.Department;
    record.displayName = info.DisplayName;
    record.email = info.Email;
    record.jobTitle = info.JobTitle;
    record.loginName = info.LoginName;
    record.mobile = info.Mobile;
    record.principalId = info.PrincipalId;
    record.principalType = info.PrincipalType;
    record.sipAddress = info.SIPAddress;
};
Office.Controls.PeoplePicker._getPrincipalFromRecord$i = function Office_Controls_PeoplePicker$_getPrincipalFromRecord$i(record) {
    var info = new Office.Controls.PrincipalInfo();

    info.Department = record.department;
    info.DisplayName = record.displayName;
    info.Email = record.email;
    info.JobTitle = record.jobTitle;
    info.LoginName = record.loginName;
    info.Mobile = record.mobile;
    info.PrincipalId = record.principalId;
    info.PrincipalType = record.principalType;
    info.SIPAddress = record.sipAddress;
    return info;
};
Office.Controls.PeoplePicker._parseUserPaste$p = function Office_Controls_PeoplePicker$_parseUserPaste$p(content) {
    var openBracket = content.indexOf('<');
    var emailSep = content.indexOf('@', openBracket);
    var closeBracket = content.indexOf('>', emailSep);

    if (openBracket !== -1 && emailSep !== -1 && closeBracket !== -1) {
        return content.substring(openBracket + 1, closeBracket);
    }
    return content;
};
Office.Controls.PeoplePicker._nopAddRemove$p = function Office_Controls_PeoplePicker$_nopAddRemove$p(p1, p2) {
};
Office.Controls.PeoplePicker._nopChange$p = function Office_Controls_PeoplePicker$_nopChange$p(p1) {
};
Office.Controls.PeoplePicker.create = function Office_Controls_PeoplePicker$create(root, parameterObject) {
    return new Office.Controls.PeoplePicker(root, parameterObject);
};
Office.Controls.PeoplePicker.prototype = {
    _allowMultiple$p$0: false,
    _groupName$p$0: null,
    _defaultTextOverride$p$0: null,
    _onAdded$p$0: null,
    _onRemoved$p$0: null,
    _onChange$p$0: null,
    _dataProvider$p$0: null,
    _showValidationErrors$p$0: false,
    _actualRoot$p$0: null,
    _textInput$p$0: null,
    _inputData$p$0: null,
    _defaultText$p$0: null,
    _resolvedListRoot$p$0: null,
    _autofillElement$p$0: null,
    _errorMessageElement$p$0: null,
    _root$p$0: null,
    _alertDiv$p$0: null,
    _lastSearchQuery$p$0: '',
    _currentToken$p$0: null,
    _widthSet$p$0: false,
    _currentPrincipalsChoices$p$0: null,
    hasErrors: false,
    _errorDisplayed$p$0: null,
    _hasMultipleEntryValidationError$p$0: false,
    _hasMultipleMatchValidationError$p$0: false,
    _hasNoMatchValidationError$p$0: false,
    _autofill$p$0: null,
    remove: function Office_Controls_PeoplePicker$remove(entryToRemove) {
        var record = this._internalSelectedItems$p$0;

        for (var i = 0; i < record.length; i++) {
            if (record[i].get_record() === entryToRemove) {
                record[i]._remove$i$0();
                break;
            }
        }
    },
    add: function Office_Controls_PeoplePicker$add(p1, resolve) {
        if (typeof p1 === 'string') {
            this._addThroughString$p$0(p1);
        }
        else {
            if (Office.Controls.Utils.isNullOrUndefined(resolve)) {
                this._addThroughRecord$p$0(p1, false);
            }
            else {
                this._addThroughRecord$p$0(p1, resolve);
            }
        }
    },
    _addThroughString$p$0: function Office_Controls_PeoplePicker$_addThroughString$p$0(input) {
        if (Office.Controls.Utils.isNullOrEmptyString(input)) {
            Office.Controls.Utils.errorConsole('Input can\'t be null or empty string. PeoplePicker Id : ' + this._root$p$0.id);
            return;
        }
        this._addUnresolvedPrincipal$p$0(input);
    },
    _addThroughRecord$p$0: function Office_Controls_PeoplePicker$_addThroughRecord$p$0(info, resolve) {
        if (resolve) {
            this._addUncertainPrincipal$p$0(info);
        }
        else {
            this._addResolvedRecord$p$0(info);
        }
    },
    _renderControl$p$0: function Office_Controls_PeoplePicker$_renderControl$p$0(inputName) {
        this._root$p$0.innerHTML = Office.Controls._peoplePickerTemplates.generateControlTemplate(inputName, this._allowMultiple$p$0, this._defaultTextOverride$p$0);
        if (this._root$p$0.className.length > 0) {
            this._root$p$0.className += ' ';
        }
        this._root$p$0.className += Office.Controls.PeoplePicker.rootClassName;
        this._actualRoot$p$0 = this._root$p$0.querySelector('div.' + Office.Controls._peoplePickerTemplates._actualControlClass$i);
        var $$t_6 = this;

        Office.Controls.Utils.addEventListener(this._actualRoot$p$0, 'click', function(e) {
            return $$t_6._onPickerClick$p$0(e);
        });
        this._inputData$p$0 = this._actualRoot$p$0.querySelector('input[type=\"hidden\"]');
        this._textInput$p$0 = this._actualRoot$p$0.querySelector('input[type=\"text\"]');
        var $$t_7 = this;

        Office.Controls.Utils.addEventListener(this._textInput$p$0, 'focus', function(e) {
            return $$t_7._onInputFocus$p$0(e);
        });
        var $$t_8 = this;

        Office.Controls.Utils.addEventListener(this._textInput$p$0, 'blur', function(e) {
            return $$t_8._onInputBlur$p$0(e);
        });
        var $$t_9 = this;

        Office.Controls.Utils.addEventListener(this._textInput$p$0, 'keydown', function(e) {
            return $$t_9._onInputKeyDown$p$0(e);
        });
        var $$t_A = this;

        Office.Controls.Utils.addEventListener(this._textInput$p$0, 'keyup', function(e) {
            return $$t_A._onInputKeyUp$p$0(e);
        });
        this._defaultText$p$0 = this._actualRoot$p$0.querySelector('span.' + Office.Controls._peoplePickerTemplates._defaultTextClass$i);
        this._resolvedListRoot$p$0 = this._actualRoot$p$0.querySelector('span.' + Office.Controls._peoplePickerTemplates._resolvedListClass$i);
        this._autofillElement$p$0 = this._actualRoot$p$0.querySelector('.' + Office.Controls._peoplePickerTemplates._autofillContainerClass$i);
        this._alertDiv$p$0 = this._actualRoot$p$0.querySelector('.' + Office.Controls._peoplePickerTemplates._alertDivClass$i);
    },
    _onInputKeyDown$p$0: function Office_Controls_PeoplePicker$_onInputKeyDown$p$0(e) {
        var keyEvent = Office.Controls.Utils.getEvent(e);

        if (keyEvent.keyCode === Office.Controls._keyCodes.tab) {
            if (this._autofill$p$0.get_isDisplayed()) {
                return true;
            }
            else {
                this._cancelLastRequest$p$0();
                this._attemptResolveInput$p$0();
                return true;
            }
        }
        else if (keyEvent.keyCode === Office.Controls._keyCodes.escape) {
            this._autofill$p$0.close();
        }
        else if (keyEvent.keyCode === Office.Controls._keyCodes.downArrow && this._autofill$p$0.get_isDisplayed()) {
            var firstElement = this._autofillElement$p$0.querySelector('a');

            if (firstElement) {
                firstElement.focus();
                Office.Controls.Utils.cancelEvent(e);
            }
        }
        else if (keyEvent.keyCode === Office.Controls._keyCodes.backspace) {
            var shouldRemove = false;

            if (!Office.Controls.Utils.isNullOrUndefined(document.selection)) {
                var range = document.selection.createRange();
                var selectedText = range.text;

                range.moveStart('character', -this._textInput$p$0.value.length);
                var caretPos = range.text.length;

                if (!selectedText.length && !caretPos) {
                    shouldRemove = true;
                }
            }
            else {
                var selectionStart = this._textInput$p$0.selectionStart;
                var selectionEnd = this._textInput$p$0.selectionEnd;

                if (!selectionStart && selectionStart === selectionEnd) {
                    shouldRemove = true;
                }
            }
            if (shouldRemove && this._internalSelectedItems$p$0.length) {
                this._internalSelectedItems$p$0[this._internalSelectedItems$p$0.length - 1]._remove$i$0();
            }
        }
        else if (keyEvent.keyCode === Office.Controls._keyCodes.k && keyEvent.ctrlKey || keyEvent.keyCode === Office.Controls._keyCodes.semiColon) {
            this._cancelLastRequest$p$0();
            this._attemptResolveInput$p$0();
            Office.Controls.Utils.cancelEvent(e);
            return false;
        }
        else if (keyEvent.keyCode === Office.Controls._keyCodes.v && keyEvent.ctrlKey || keyEvent.keyCode === Office.Controls._keyCodes.semiColon) {
            this._cancelLastRequest$p$0();
            var $$t_A = this;

            window.setTimeout(function() {
                $$t_A._textInput$p$0.value = Office.Controls.PeoplePicker._parseUserPaste$p($$t_A._textInput$p$0.value);
                $$t_A._attemptResolveInput$p$0();
            }, 0);
            return true;
        }
        else if (keyEvent.keyCode === Office.Controls._keyCodes.enter && keyEvent.shiftKey) {
            var $$t_B = this;

            this._autofill$p$0.open(function(selectedPrincipal) {
                $$t_B._addResolvedPrincipal$p$0(selectedPrincipal);
            });
        }
        else {
            this._resizeInputField$p$0();
        }
        return true;
    },
    _cancelLastRequest$p$0: function Office_Controls_PeoplePicker$_cancelLastRequest$p$0() {
        window.clearTimeout(this._currentTimerId$p$0);
        if (!Office.Controls.Utils.isNullOrUndefined(this._currentToken$p$0)) {
            this._hideLoadingIcon$p$0();
            this._currentToken$p$0.cancel();
            this._currentToken$p$0 = null;
        }
    },
    _onInputKeyUp$p$0: function Office_Controls_PeoplePicker$_onInputKeyUp$p$0(e) {
        this._startQueryAfterDelay$p$0();
        this._resizeInputField$p$0();
        if (!this._textInput$p$0.value.length) {
            this._autofill$p$0.close();
        }
        return true;
    },
    _displayCachedEntries$p$0: function Office_Controls_PeoplePicker$_displayCachedEntries$p$0() {
        var cachedEntries = this._cache$p$0.get(this._textInput$p$0.value, Office.Controls.PeoplePicker._maxCacheEntries$p);

        this._autofill$p$0.setCachedEntries(cachedEntries);
        if (!cachedEntries.length && !this._autofill$p$0.get_isDisplayed()) {
            return;
        }
        var $$t_2 = this;

        this._autofill$p$0.open(function(selectedPrincipal) {
            $$t_2._addResolvedPrincipal$p$0(selectedPrincipal);
        });
    },
    _resizeInputField$p$0: function Office_Controls_PeoplePicker$_resizeInputField$p$0() {
        var size = Math.max(this._textInput$p$0.value.length + 1, 1);

        this._textInput$p$0.size = size;
    },
    _clearInputField$p$0: function Office_Controls_PeoplePicker$_clearInputField$p$0() {
        this._textInput$p$0.value = '';
        this._resizeInputField$p$0();
    },
    _startQueryAfterDelay$p$0: function Office_Controls_PeoplePicker$_startQueryAfterDelay$p$0() {
        this._cancelLastRequest$p$0();
        var $$t_3 = this;

        this._currentTimerId$p$0 = window.setTimeout(function() {
            if ($$t_3._textInput$p$0.value !== $$t_3._lastSearchQuery$p$0) {
                $$t_3._lastSearchQuery$p$0 = $$t_3._textInput$p$0.value;
                if ($$t_3._textInput$p$0.value.length >= Office.Controls.PeoplePicker._minimumNumberOfLettersToQuery$p) {
                    $$t_3._displayLoadingIcon$p$0();
                    $$t_3._removeValidationError$p$0(Office.Controls.PeoplePicker.ValidationError.serverProblemName);
                    var token = new Office.Controls.PeoplePicker._cancelToken();

                    $$t_3._currentToken$p$0 = token;
                    $$t_3._dataProvider$p$0.getPrincipals($$t_3._textInput$p$0.value, 15, 15, $$t_3._groupName$p$0, Office.Controls.PeoplePicker._numberOfResults$p, function(principalsReceived) {
                        if (!token.get_isCanceled()) {
                            $$t_3._onDataReceived$p$0(principalsReceived);
                        }
                        else {
                            $$t_3._hideLoadingIcon$p$0();
                        }
                    }, function(error) {
                        $$t_3._onDataFetchError$p$0(error);
                    });
                }
                else {
                    $$t_3._autofill$p$0.close();
                }
                $$t_3._autofill$p$0.flushContent();
                $$t_3._displayCachedEntries$p$0();
            }
        }, Office.Controls.PeoplePicker._autofillWait$p);
    },
    _onDataFetchError$p$0: function Office_Controls_PeoplePicker$_onDataFetchError$p$0(message) {
        this._hideLoadingIcon$p$0();
        this._addValidationError$p$0(Office.Controls.PeoplePicker.ValidationError._createServerProblemError$i());
    },
    _onDataReceived$p$0: function Office_Controls_PeoplePicker$_onDataReceived$p$0(principalsReceived) {
        this._currentPrincipalsChoices$p$0 = {};
        for (var i = 0; i < principalsReceived.length; i++) {
            var principal = principalsReceived[i];

            this._currentPrincipalsChoices$p$0[principal.LoginName] = principal;
        }
        this._autofill$p$0.setServerEntries(principalsReceived);
        this._hideLoadingIcon$p$0();
        var $$t_4 = this;

        this._autofill$p$0.open(function(selectedPrincipal) {
            $$t_4._addResolvedPrincipal$p$0(selectedPrincipal);
        });
    },
    _onPickerClick$p$0: function Office_Controls_PeoplePicker$_onPickerClick$p$0(e) {
        this._textInput$p$0.focus();
        e = Office.Controls.Utils.getEvent(e);
        var element = Office.Controls.Utils.getTarget(e);

        if (element.nodeName.toLowerCase() !== 'input') {
            this._focusToEnd$p$0();
        }
        return true;
    },
    _focusToEnd$p$0: function Office_Controls_PeoplePicker$_focusToEnd$p$0() {
        var endPos = this._textInput$p$0.value.length;

        if (!Office.Controls.Utils.isNullOrUndefined(this._textInput$p$0.createTextRange)) {
            var range = this._textInput$p$0.createTextRange();

            range.collapse(true);
            range.moveStart('character', endPos);
            range.moveEnd('character', endPos);
            range.select();
        }
        else {
            this._textInput$p$0.focus();
            this._textInput$p$0.setSelectionRange(endPos, endPos);
        }
    },
    _onInputFocus$p$0: function Office_Controls_PeoplePicker$_onInputFocus$p$0(e) {
        this._defaultText$p$0.style.display = 'none';
        if (Office.Controls.Utils.isNullOrEmptyString(this._actualRoot$p$0.className)) {
            this._actualRoot$p$0.className = Office.Controls.PeoplePicker._focusClassName$i;
        }
        else {
            this._actualRoot$p$0.className += ' ' + Office.Controls.PeoplePicker._focusClassName$i;
        }
        if (!this._widthSet$p$0) {
            this._setInputMaxWidth$p$0();
        }
        return true;
    },
    _setInputMaxWidth$p$0: function Office_Controls_PeoplePicker$_setInputMaxWidth$p$0() {
        var maxwidth = this._actualRoot$p$0.clientWidth - 25;

        if (maxwidth <= 0) {
            maxwidth = 20;
        }
        this._textInput$p$0.style.maxWidth = maxwidth.toString() + 'px';
        this._widthSet$p$0 = true;
    },
    _onInputBlur$p$0: function Office_Controls_PeoplePicker$_onInputBlur$p$0(e) {
        Office.Controls.Utils.removeClass(this._actualRoot$p$0, Office.Controls.PeoplePicker._focusClassName$i);
        if (this._textInput$p$0.value.length > 0) {
            return true;
        }
        if (this.selectedItems.length > 0) {
            return true;
        }
        this._defaultText$p$0.style.display = 'inline';
        return true;
    },
    _onDataSelected$p$0: function Office_Controls_PeoplePicker$_onDataSelected$p$0(selectedPrincipal) {
        this._lastSearchQuery$p$0 = '';
        this._validateMultipleEntryAllowed$p$0();
        this._clearInputField$p$0();
        this._refreshInputField$p$0();
        this._onAdded$p$0(this, selectedPrincipal);
        this._onChange$p$0(this);
    },
    _onDataRemoved$p$0: function Office_Controls_PeoplePicker$_onDataRemoved$p$0(selectedPrincipal) {
        this.selectedItems.splice(this.selectedItems.indexOf(selectedPrincipal), 1);
        this._refreshInputField$p$0();
        this._validateMultipleMatchError$p$0();
        this._validateMultipleEntryAllowed$p$0();
        this._validateNoMatchError$p$0();
        this._onRemoved$p$0(this, selectedPrincipal);
        this._onChange$p$0(this);
    },
    _addToCache$p$0: function Office_Controls_PeoplePicker$_addToCache$p$0(entry) {
        if (!this._cache$p$0.isCacheAvailable) {
            return;
        }
        this._cache$p$0.set(entry);
    },
    _refreshInputField$p$0: function Office_Controls_PeoplePicker$_refreshInputField$p$0() {
        this._inputData$p$0.value = Office.Controls.Utils.serializeJSON(this.selectedItems);
    },
    _changeAlertMessage$p$0: function Office_Controls_PeoplePicker$_changeAlertMessage$p$0(message) {
        this._alertDiv$p$0.innerHTML = Office.Controls.Utils.htmlEncode(message);
    },
    _displayLoadingIcon$p$0: function Office_Controls_PeoplePicker$_displayLoadingIcon$p$0() {
        this._actualRoot$p$0.style.backgroundPosition = (this._actualRoot$p$0.clientWidth - 20).toString() + 'px';
        Office.Controls.Utils.addClass(this._actualRoot$p$0, Office.Controls._peoplePickerTemplates._loadingDataClass$i);
        this._changeAlertMessage$p$0(Office.Controls._peoplePickerTemplates.getString(Office.Controls._peoplePickerResourcesStrings.pP_Searching));
    },
    _hideLoadingIcon$p$0: function Office_Controls_PeoplePicker$_hideLoadingIcon$p$0() {
        Office.Controls.Utils.removeClass(this._actualRoot$p$0, Office.Controls._peoplePickerTemplates._loadingDataClass$i);
    },
    _attemptResolveInput$p$0: function Office_Controls_PeoplePicker$_attemptResolveInput$p$0() {
        this._autofill$p$0.close();
        if (this._textInput$p$0.value.length > 0) {
            this._lastSearchQuery$p$0 = '';
            this._addUnresolvedPrincipal$p$0(this._textInput$p$0.value);
            this._clearInputField$p$0();
        }
    },
    _onDataReceivedForResolve$p$0: function Office_Controls_PeoplePicker$_onDataReceivedForResolve$p$0(principalsReceived, internalRecordToResolve) {
        this._hideLoadingIcon$p$0();
        if (principalsReceived.length === 1) {
            internalRecordToResolve._resolveTo$i$0(principalsReceived[0]);
        }
        else {
            internalRecordToResolve._setResolveOptions$i$0(principalsReceived);
        }
        this._refreshInputField$p$0();
        this._onAdded$p$0(this, internalRecordToResolve.get_record());
        this._onChange$p$0(this);
    },
    _onDataReceivedForStalenessCheck$p$0: function Office_Controls_PeoplePicker$_onDataReceivedForStalenessCheck$p$0(principalsReceived, internalRecordToCheck) {
        if (principalsReceived.length === 1) {
            internalRecordToCheck._refresh$i$0(principalsReceived[0]);
        }
        else {
            internalRecordToCheck._unresolve$i$0();
            internalRecordToCheck._setResolveOptions$i$0(principalsReceived);
        }
        this._refreshInputField$p$0();
        this._onAdded$p$0(this, internalRecordToCheck.get_record());
        this._onChange$p$0(this);
    },
    _addResolvedPrincipal$p$0: function Office_Controls_PeoplePicker$_addResolvedPrincipal$p$0(principal) {
        var record = new Office.Controls.PeoplePickerRecord();

        Office.Controls.PeoplePicker._copyToRecord$i(record, principal);
        record.text = principal.DisplayName;
        record.isResolved = true;
        this.selectedItems.push(record);
        var internalRecord = new Office.Controls.PeoplePicker._internalPeoplePickerRecord(this, record);

        internalRecord._add$i$0();
        this._internalSelectedItems$p$0.push(internalRecord);
        this._onDataSelected$p$0(record);
        this._addToCache$p$0(principal);
        this._currentPrincipalsChoices$p$0 = null;
        this._autofill$p$0.close();
    },
    _addResolvedRecord$p$0: function Office_Controls_PeoplePicker$_addResolvedRecord$p$0(record) {
        this.selectedItems.push(record);
        var internalRecord = new Office.Controls.PeoplePicker._internalPeoplePickerRecord(this, record);

        internalRecord._add$i$0();
        this._internalSelectedItems$p$0.push(internalRecord);
        this._onDataSelected$p$0(record);
        this._currentPrincipalsChoices$p$0 = null;
    },
    _addUncertainPrincipal$p$0: function Office_Controls_PeoplePicker$_addUncertainPrincipal$p$0(record) {
        this.selectedItems.push(record);
        var internalRecord = new Office.Controls.PeoplePicker._internalPeoplePickerRecord(this, record);

        internalRecord._add$i$0();
        this._internalSelectedItems$p$0.push(internalRecord);
        var $$t_4 = this, $$t_5 = this;

        this._dataProvider$p$0.getPrincipals(record.email, 15, 15, this._groupName$p$0, Office.Controls.PeoplePicker._numberOfResults$p, function(ps) {
            $$t_4._onDataReceivedForStalenessCheck$p$0(ps, internalRecord);
        }, function(message) {
            $$t_5._onDataFetchError$p$0(message);
        });
        this._validateMultipleEntryAllowed$p$0();
    },
    _addUnresolvedPrincipal$p$0: function Office_Controls_PeoplePicker$_addUnresolvedPrincipal$p$0(input) {
        var record = new Office.Controls.PeoplePickerRecord();

        record.text = input;
        record.isResolved = false;
        var internalRecord = new Office.Controls.PeoplePicker._internalPeoplePickerRecord(this, record);

        internalRecord._add$i$0();
        this.selectedItems.push(record);
        this._internalSelectedItems$p$0.push(internalRecord);
        this._displayLoadingIcon$p$0();
        var $$t_5 = this, $$t_6 = this;

        this._dataProvider$p$0.getPrincipals(input, 15, 15, this._groupName$p$0, Office.Controls.PeoplePicker._numberOfResults$p, function(ps) {
            $$t_5._onDataReceivedForResolve$p$0(ps, internalRecord);
        }, function(message) {
            $$t_6._onDataFetchError$p$0(message);
        });
        this._validateMultipleEntryAllowed$p$0();
    },
    _addValidationError$p$0: function Office_Controls_PeoplePicker$_addValidationError$p$0(err) {
        this.hasErrors = true;
        this.errors.push(err);
        this._displayValidationErrors$p$0();
    },
    _removeValidationError$p$0: function Office_Controls_PeoplePicker$_removeValidationError$p$0(errorName) {
        for (var i = 0; i < this.errors.length; i++) {
            if (this.errors[i].errorName === errorName) {
                this.errors.splice(i, 1);
                break;
            }
        }
        if (!this.errors.length) {
            this.hasErrors = false;
        }
        this._displayValidationErrors$p$0();
    },
    _validateMultipleEntryAllowed$p$0: function Office_Controls_PeoplePicker$_validateMultipleEntryAllowed$p$0() {
        if (!this._allowMultiple$p$0) {
            if (this.selectedItems.length > 1) {
                if (!this._hasMultipleEntryValidationError$p$0) {
                    this._addValidationError$p$0(Office.Controls.PeoplePicker.ValidationError._createMultipleEntryError$i());
                    this._hasMultipleEntryValidationError$p$0 = true;
                }
            }
            else if (this._hasMultipleEntryValidationError$p$0) {
                this._removeValidationError$p$0(Office.Controls.PeoplePicker.ValidationError.multipleEntryName);
                this._hasMultipleEntryValidationError$p$0 = false;
            }
        }
    },
    _validateMultipleMatchError$p$0: function Office_Controls_PeoplePicker$_validateMultipleMatchError$p$0() {
        var oldStatus = this._hasMultipleMatchValidationError$p$0;
        var newStatus = false;

        for (var i = 0; i < this._internalSelectedItems$p$0.length; i++) {
            if (!Office.Controls.Utils.isNullOrUndefined(this._internalSelectedItems$p$0[i]._optionsList$i$0) && this._internalSelectedItems$p$0[i]._optionsList$i$0.length > 0) {
                newStatus = true;
                break;
            }
        }
        if (!oldStatus && newStatus) {
            this._addValidationError$p$0(Office.Controls.PeoplePicker.ValidationError._createMultipleMatchError$i());
        }
        if (oldStatus && !newStatus) {
            this._removeValidationError$p$0(Office.Controls.PeoplePicker.ValidationError.multipleMatchName);
        }
        this._hasMultipleMatchValidationError$p$0 = newStatus;
    },
    _validateNoMatchError$p$0: function Office_Controls_PeoplePicker$_validateNoMatchError$p$0() {
        var oldStatus = this._hasNoMatchValidationError$p$0;
        var newStatus = false;

        for (var i = 0; i < this._internalSelectedItems$p$0.length; i++) {
            if (!Office.Controls.Utils.isNullOrUndefined(this._internalSelectedItems$p$0[i]._optionsList$i$0) && !this._internalSelectedItems$p$0[i]._optionsList$i$0.length) {
                newStatus = true;
                break;
            }
        }
        if (!oldStatus && newStatus) {
            this._addValidationError$p$0(Office.Controls.PeoplePicker.ValidationError._createNoMatchError$i());
        }
        if (oldStatus && !newStatus) {
            this._removeValidationError$p$0(Office.Controls.PeoplePicker.ValidationError.noMatchName);
        }
        this._hasNoMatchValidationError$p$0 = newStatus;
    },
    _displayValidationErrors$p$0: function Office_Controls_PeoplePicker$_displayValidationErrors$p$0() {
        if (!this._showValidationErrors$p$0) {
            return;
        }
        if (!this.errors.length) {
            if (!Office.Controls.Utils.isNullOrUndefined(this._errorMessageElement$p$0)) {
                this._errorMessageElement$p$0.parentNode.removeChild(this._errorMessageElement$p$0);
                this._errorMessageElement$p$0 = null;
                this._errorDisplayed$p$0 = null;
            }
        }
        else {
            if (this._errorDisplayed$p$0 !== this.errors[0]) {
                if (!Office.Controls.Utils.isNullOrUndefined(this._errorMessageElement$p$0)) {
                    this._errorMessageElement$p$0.parentNode.removeChild(this._errorMessageElement$p$0);
                }
                var holderDiv = document.createElement('div');

                holderDiv.innerHTML = Office.Controls._peoplePickerTemplates.generateErrorTemplate(this.errors[0].localizedErrorMessage);
                this._errorMessageElement$p$0 = holderDiv.firstChild;
                this._root$p$0.appendChild(this._errorMessageElement$p$0);
                this._errorDisplayed$p$0 = this.errors[0];
            }
        }
    },
    setDataProvider: function Office_Controls_PeoplePicker$setDataProvider(newProvider) {
        this._dataProvider$p$0 = newProvider;
    }
};
Office.Controls.PeoplePicker._internalPeoplePickerRecord = function Office_Controls_PeoplePicker__internalPeoplePickerRecord(parent, record) {
    this._parent$i$0 = parent;
    this.set_record(record);
};
Office.Controls.PeoplePicker._internalPeoplePickerRecord.prototype = {
    _$$pf_Record$p$0: null,
    get_record: function Office_Controls_PeoplePicker__internalPeoplePickerRecord$get_record() {
        return this._$$pf_Record$p$0;
    },
    set_record: function Office_Controls_PeoplePicker__internalPeoplePickerRecord$set_record(value) {
        this._$$pf_Record$p$0 = value;
        return value;
    },
    _principalOptions$i$0: null,
    _optionsList$i$0: null,
    _$$pf_Node$p$0: null,
    get_node: function Office_Controls_PeoplePicker__internalPeoplePickerRecord$get_node() {
        return this._$$pf_Node$p$0;
    },
    set_node: function Office_Controls_PeoplePicker__internalPeoplePickerRecord$set_node(value) {
        this._$$pf_Node$p$0 = value;
        return value;
    },
    _parent$i$0: null,
    _onRecordRemovalClick$p$0: function Office_Controls_PeoplePicker__internalPeoplePickerRecord$_onRecordRemovalClick$p$0(e) {
        var recordRemovalEvent = Office.Controls.Utils.getEvent(e);
        var target = Office.Controls.Utils.getTarget(recordRemovalEvent);

        this._remove$i$0();
        Office.Controls.Utils.cancelEvent(e);
        this._parent$i$0._autofill$p$0.close();
        return false;
    },
    _onRecordRemovalKeyDown$p$0: function Office_Controls_PeoplePicker__internalPeoplePickerRecord$_onRecordRemovalKeyDown$p$0(e) {
        var recordRemovalEvent = Office.Controls.Utils.getEvent(e);
        var target = Office.Controls.Utils.getTarget(recordRemovalEvent);

        if (recordRemovalEvent.keyCode === Office.Controls._keyCodes.backspace || recordRemovalEvent.keyCode === Office.Controls._keyCodes.enter || recordRemovalEvent.keyCode === Office.Controls._keyCodes.deleteKey) {
            this._remove$i$0();
            Office.Controls.Utils.cancelEvent(e);
            this._parent$i$0._autofill$p$0.close();
        }
        return false;
    },
    _add$i$0: function Office_Controls_PeoplePicker__internalPeoplePickerRecord$_add$i$0() {
        var holderDiv = document.createElement('div');

        holderDiv.innerHTML = Office.Controls._peoplePickerTemplates.generateRecordTemplate(this.get_record());
        var recordElement = holderDiv.firstChild;
        var removeButtonElement = recordElement.querySelector('a.' + Office.Controls._peoplePickerTemplates._recordRemoverClass$i);
        var $$t_5 = this;

        Office.Controls.Utils.addEventListener(removeButtonElement, 'click', function(e) {
            return $$t_5._onRecordRemovalClick$p$0(e);
        });
        var $$t_6 = this;

        Office.Controls.Utils.addEventListener(removeButtonElement, 'keydown', function(e) {
            return $$t_6._onRecordRemovalKeyDown$p$0(e);
        });
        this._ensureNoBiggerThanParent$p$0(recordElement.firstChild);
        this._parent$i$0._resolvedListRoot$p$0.appendChild(recordElement);
        this._parent$i$0._defaultText$p$0.style.display = 'none';
        this.set_node(recordElement);
    },
    _remove$i$0: function Office_Controls_PeoplePicker__internalPeoplePickerRecord$_remove$i$0() {
        this._parent$i$0._resolvedListRoot$p$0.removeChild(this.get_node());
        this._parent$i$0._textInput$p$0.focus();
        for (var i = 0; i < this._parent$i$0._internalSelectedItems$p$0.length; i++) {
            if (this._parent$i$0._internalSelectedItems$p$0[i] === this) {
                this._parent$i$0._internalSelectedItems$p$0.splice(i, 1);
            }
        }
        this._parent$i$0._focusToEnd$p$0();
        this._parent$i$0._onDataRemoved$p$0(this.get_record());
    },
    _ensureNoBiggerThanParent$p$0: function Office_Controls_PeoplePicker__internalPeoplePickerRecord$_ensureNoBiggerThanParent$p$0(userLabel) {
        userLabel.style.maxWidth = (this._parent$i$0._actualRoot$p$0.clientWidth - 36).toString() + 'px';
    },
    _setResolveOptions$i$0: function Office_Controls_PeoplePicker__internalPeoplePickerRecord$_setResolveOptions$i$0(options) {
        this._optionsList$i$0 = options;
        this._principalOptions$i$0 = {};
        for (var i = 0; i < options.length; i++) {
            this._principalOptions$i$0[options[i].LoginName] = options[i];
        }
        var $$t_3 = this;

        Office.Controls.Utils.addEventListener((this.get_node()).querySelector('a.' + Office.Controls._peoplePickerTemplates._unresolvedUserClass$i), 'click', function(e) {
            return $$t_3._onUnresolvedUserClick$i$0(e);
        });
        this._parent$i$0._validateMultipleMatchError$p$0();
        this._parent$i$0._validateNoMatchError$p$0();
    },
    _onUnresolvedUserClick$i$0: function Office_Controls_PeoplePicker__internalPeoplePickerRecord$_onUnresolvedUserClick$i$0(e) {
        e = Office.Controls.Utils.getEvent(e);
        this._parent$i$0._autofill$p$0.flushContent();
        this._parent$i$0._autofill$p$0.setServerEntries(this._optionsList$i$0);
        var $$t_2 = this;

        this._parent$i$0._autofill$p$0.open(function(selectedPrincipal) {
            $$t_2._onAutofillClick$i$0(selectedPrincipal);
        });
        this._parent$i$0._autofill$p$0.focusOnFirstElement();
        Office.Controls.Utils.cancelEvent(e);
        return false;
    },
    _resolveTo$i$0: function Office_Controls_PeoplePicker__internalPeoplePickerRecord$_resolveTo$i$0(principal) {
        Office.Controls.PeoplePicker._copyToRecord$i(this.get_record(), principal);
        (this.get_record()).text = principal.DisplayName;
        (this.get_record()).isResolved = true;
        this._parent$i$0._addToCache$p$0(principal);
        var linkNode = (this.get_node()).querySelector('a.' + Office.Controls._peoplePickerTemplates._unresolvedUserClass$i);
        var newSpan = document.createElement('span');

        newSpan.className = Office.Controls._peoplePickerTemplates._resolvedUserClass$i;
        this._updateHoverText$p$0(newSpan);
        this._ensureNoBiggerThanParent$p$0(newSpan);
        newSpan.innerHTML = Office.Controls.Utils.htmlEncode(principal.DisplayName);
        linkNode.parentNode.insertBefore(newSpan, linkNode);
        linkNode.parentNode.removeChild(linkNode);
    },
    _refresh$i$0: function Office_Controls_PeoplePicker__internalPeoplePickerRecord$_refresh$i$0(principal) {
        Office.Controls.PeoplePicker._copyToRecord$i(this.get_record(), principal);
        (this.get_record()).text = principal.DisplayName;
        var spanNode = (this.get_node()).querySelector('span.' + Office.Controls._peoplePickerTemplates._resolvedUserClass$i);

        spanNode.innerHTML = Office.Controls.Utils.htmlEncode(principal.DisplayName);
    },
    _unresolve$i$0: function Office_Controls_PeoplePicker__internalPeoplePickerRecord$_unresolve$i$0() {
        (this.get_record()).isResolved = false;
        var spanNode = (this.get_node()).querySelector('span.' + Office.Controls._peoplePickerTemplates._resolvedUserClass$i);
        var newLink = document.createElement('a');

        newLink.className = Office.Controls._peoplePickerTemplates._unresolvedUserClass$i;
        this._ensureNoBiggerThanParent$p$0(newLink);
        this._updateHoverText$p$0(newLink);
        newLink.innerHTML = Office.Controls.Utils.htmlEncode((this.get_record()).text);
        spanNode.parentNode.insertBefore(newLink, spanNode);
        spanNode.parentNode.removeChild(spanNode);
    },
    _updateHoverText$p$0: function Office_Controls_PeoplePicker__internalPeoplePickerRecord$_updateHoverText$p$0(userLabel) {
        userLabel.title = Office.Controls.Utils.htmlEncode((this.get_record()).text);
        ((this.get_node()).querySelector('a.' + Office.Controls._peoplePickerTemplates._recordRemoverClass$i)).title = Office.Controls.Utils.formatString(Office.Controls._peoplePickerTemplates.getString(Office.Controls.Utils.htmlEncode(Office.Controls._peoplePickerResourcesStrings.pP_RemovePerson)), (this.get_record()).text);
    },
    _onAutofillClick$i$0: function Office_Controls_PeoplePicker__internalPeoplePickerRecord$_onAutofillClick$i$0(selectedPrincipal) {
        this._parent$i$0._onRemoved$p$0(this._parent$i$0, this.get_record());
        this._resolveTo$i$0(selectedPrincipal);
        this._parent$i$0._refreshInputField$p$0();
        this._principalOptions$i$0 = null;
        this._optionsList$i$0 = null;
        this._parent$i$0._addToCache$p$0(selectedPrincipal);
        this._parent$i$0._validateMultipleMatchError$p$0();
        this._parent$i$0._autofill$p$0.close();
        this._parent$i$0._onAdded$p$0(this._parent$i$0, this.get_record());
        this._parent$i$0._onChange$p$0(this._parent$i$0);
    }
};
Office.Controls.PeoplePicker._autofillContainer = function Office_Controls_PeoplePicker__autofillContainer(parent) {
    this._entries$p$0 = {};
    this._cachedEntries$p$0 = new Array(0);
    this._serverEntries$p$0 = new Array(0);
    this._parent$p$0 = parent;
    this._root$p$0 = parent._autofillElement$p$0;
    if (!Office.Controls.PeoplePicker._autofillContainer._boolBodyHandlerAdded$p) {
        var $$t_2 = this;

        Office.Controls.Utils.addEventListener(document.body, 'click', function(e) {
            return Office.Controls.PeoplePicker._autofillContainer._bodyOnClick$p(e);
        });
        Office.Controls.PeoplePicker._autofillContainer._boolBodyHandlerAdded$p = true;
    }
};
Office.Controls.PeoplePicker._autofillContainer._getControlRootFromSubElement$p = function Office_Controls_PeoplePicker__autofillContainer$_getControlRootFromSubElement$p(element) {
    while (element && element.nodeName.toLowerCase() !== 'body') {
        if (element.className.indexOf(Office.Controls.PeoplePicker.rootClassName) !== -1) {
            return element;
        }
        element = element.parentNode;
    }
    return null;
};
Office.Controls.PeoplePicker._autofillContainer._bodyOnClick$p = function Office_Controls_PeoplePicker__autofillContainer$_bodyOnClick$p(e) {
    if (!Office.Controls.PeoplePicker._autofillContainer._currentOpened$p) {
        return true;
    }
    var click = Office.Controls.Utils.getEvent(e);
    var target = Office.Controls.Utils.getTarget(click);
    var controlRoot = Office.Controls.PeoplePicker._autofillContainer._getControlRootFromSubElement$p(target);

    if (!target || controlRoot !== Office.Controls.PeoplePicker._autofillContainer._currentOpened$p._parent$p$0._root$p$0) {
        Office.Controls.PeoplePicker._autofillContainer._currentOpened$p.close();
    }
    return true;
};
Office.Controls.PeoplePicker._autofillContainer.prototype = {
    _parent$p$0: null,
    _root$p$0: null,
    _$$pf_IsDisplayed$p$0: false,
    get_isDisplayed: function Office_Controls_PeoplePicker__autofillContainer$get_isDisplayed() {
        return this._$$pf_IsDisplayed$p$0;
    },
    set_isDisplayed: function Office_Controls_PeoplePicker__autofillContainer$set_isDisplayed(value) {
        this._$$pf_IsDisplayed$p$0 = value;
        return value;
    },
    setCachedEntries: function Office_Controls_PeoplePicker__autofillContainer$setCachedEntries(entries) {
        this._cachedEntries$p$0 = entries;
        this._entries$p$0 = {};
        var length = entries.length;

        for (var i = 0; i < length; i++) {
            this._entries$p$0[entries[i].LoginName] = entries[i];
        }
    },
    setServerEntries: function Office_Controls_PeoplePicker__autofillContainer$setServerEntries(entries) {
        var newServerEntries = new Array(0);
        var length = entries.length;

        for (var i = 0; i < length; i++) {
            var currentEntry = entries[i];

            if (Office.Controls.Utils.isNullOrUndefined(this._entries$p$0[currentEntry.LoginName])) {
                this._entries$p$0[entries[i].LoginName] = entries[i];
                newServerEntries.push(currentEntry);
            }
        }
        this._serverEntries$p$0 = newServerEntries;
    },
    _renderList$p$0: function Office_Controls_PeoplePicker__autofillContainer$_renderList$p$0(handler) {
        this._root$p$0.innerHTML = Office.Controls._peoplePickerTemplates.generateAutofillListTemplate(this._cachedEntries$p$0, this._serverEntries$p$0, Office.Controls.PeoplePicker._numberOfResults$p);
        var autofillElementsLinkTags = this._root$p$0.querySelectorAll('a');

        for (var i = 0; i < autofillElementsLinkTags.length; i++) {
            var link = autofillElementsLinkTags[i];
            var $$t_8 = this;

            Office.Controls.Utils.addEventListener(link, 'click', function(e) {
                return $$t_8._onEntryClick$p$0(e, handler);
            });
            var $$t_9 = this;

            Office.Controls.Utils.addEventListener(link, 'keydown', function(e) {
                return $$t_9._onKeyDown$p$0(e);
            });
            var $$t_A = this;

            Office.Controls.Utils.addEventListener(link, 'focus', function(e) {
                return $$t_A._onEntryFocus$p$0(e);
            });
            var $$t_B = this;

            Office.Controls.Utils.addEventListener(link, 'blur', function(e) {
                return $$t_B._onEntryBlur$p$0(e);
            });
        }
    },
    flushContent: function Office_Controls_PeoplePicker__autofillContainer$flushContent() {
        var entry = this._root$p$0.querySelectorAll('li');

        for (var i = 0; i < entry.length; i++) {
            this._root$p$0.removeChild(entry[i]);
        }
        this._entries$p$0 = {};
        this._serverEntries$p$0 = new Array(0);
        this._cachedEntries$p$0 = new Array(0);
    },
    open: function Office_Controls_PeoplePicker__autofillContainer$open(handler) {
        this._root$p$0.style.top = (this._parent$p$0._actualRoot$p$0.clientHeight + 2).toString() + 'px';
        this._renderList$p$0(handler);
        this.set_isDisplayed(true);
        Office.Controls.PeoplePicker._autofillContainer._currentOpened$p = this;
        if (!Office.Controls.Utils.containClass(this._parent$p$0._actualRoot$p$0, Office.Controls._peoplePickerTemplates._autofillOpenedClass$i)) {
            Office.Controls.Utils.addClass(this._parent$p$0._actualRoot$p$0, Office.Controls._peoplePickerTemplates._autofillOpenedClass$i);
        }
        if (!Office.Controls.Utils.containClass(this._parent$p$0._actualRoot$p$0, Office.Controls._peoplePickerTemplates._loadingDataClass$i)) {
            if (this._cachedEntries$p$0.length + this._serverEntries$p$0.length > 0) {
                this._parent$p$0._changeAlertMessage$p$0(Office.Controls._peoplePickerTemplates.getString(Office.Controls._peoplePickerResourcesStrings.pP_SuggestionsAvailable));
            }
            else {
                this._parent$p$0._changeAlertMessage$p$0(Office.Controls._peoplePickerTemplates.getString(Office.Controls._peoplePickerResourcesStrings.pP_NoSuggestionsAvailable));
            }
        }
    },
    close: function Office_Controls_PeoplePicker__autofillContainer$close() {
        this.set_isDisplayed(false);
        Office.Controls.Utils.removeClass(this._parent$p$0._actualRoot$p$0, Office.Controls._peoplePickerTemplates._autofillOpenedClass$i);
    },
    _onEntryClick$p$0: function Office_Controls_PeoplePicker__autofillContainer$_onEntryClick$p$0(e, handler) {
        var click = Office.Controls.Utils.getEvent(e);
        var target = Office.Controls.Utils.getTarget(click);

        target = this._getParentListItem$p$0(target);
        var loginName = this._getLoginNameFromListElement$p$0(target);

        handler(this._entries$p$0[loginName]);
        this.flushContent();
        return true;
    },
    focusOnFirstElement: function Office_Controls_PeoplePicker__autofillContainer$focusOnFirstElement() {
        var first = this._root$p$0.querySelector('li.' + Office.Controls._peoplePickerTemplates._autofillItemClass$i);

        if (!Office.Controls.Utils.isNullOrUndefined(first)) {
            first.firstChild.focus();
        }
    },
    _onKeyDown$p$0: function Office_Controls_PeoplePicker__autofillContainer$_onKeyDown$p$0(e) {
        var key = Office.Controls.Utils.getEvent(e);
        var target = Office.Controls.Utils.getTarget(key);

        if (key.keyCode === Office.Controls._keyCodes.upArrow || key.keyCode === Office.Controls._keyCodes.tab && key.shiftKey) {
            var previous = target.parentNode.previousSibling;

            if (!previous) {
                this._parent$p$0._focusToEnd$p$0();
            }
            else {
                if (previous.firstChild.tagName.toLowerCase() !== 'a') {
                    previous = previous.previousSibling;
                }
                previous.firstChild.focus();
            }
            Office.Controls.Utils.cancelEvent(e);
            return false;
        }
        else if (key.keyCode === Office.Controls._keyCodes.downArrow) {
            var next = target.parentNode.nextSibling;

            if (next) {
                if (next.firstChild.tagName.toLowerCase() !== 'a') {
                    next = next.nextSibling;
                    if (next) {
                        next.firstChild.focus();
                    }
                }
                else {
                    next.firstChild.focus();
                }
            }
        }
        else if (key.keyCode === Office.Controls._keyCodes.escape) {
            this.close();
        }
        if (key.keyCode !== Office.Controls._keyCodes.tab && key.keyCode !== Office.Controls._keyCodes.enter) {
            Office.Controls.Utils.cancelEvent(key);
        }
        return false;
    },
    _getLoginNameFromListElement$p$0: function Office_Controls_PeoplePicker__autofillContainer$_getLoginNameFromListElement$p$0(listElement) {
        return (listElement.attributes.getNamedItem(Office.Controls._peoplePickerTemplates._autofillItemDataAttribute$i)).value;
    },
    _getParentListItem$p$0: function Office_Controls_PeoplePicker__autofillContainer$_getParentListItem$p$0(element) {
        while (element && element.nodeName.toLowerCase() !== 'li') {
            element = element.parentNode;
        }
        return element;
    },
    _onEntryFocus$p$0: function Office_Controls_PeoplePicker__autofillContainer$_onEntryFocus$p$0(e) {
        var target = Office.Controls.Utils.getTarget(e);

        target = this._getParentListItem$p$0(target);
        if (!Office.Controls.Utils.containClass(target, Office.Controls.PeoplePicker._autofillContainer._focusClassName$p)) {
            Office.Controls.Utils.addClass(target, Office.Controls.PeoplePicker._autofillContainer._focusClassName$p);
        }
        return false;
    },
    _onEntryBlur$p$0: function Office_Controls_PeoplePicker__autofillContainer$_onEntryBlur$p$0(e) {
        var target = Office.Controls.Utils.getTarget(e);

        target = this._getParentListItem$p$0(target);
        Office.Controls.Utils.removeClass(target, Office.Controls.PeoplePicker._autofillContainer._focusClassName$p);
        return false;
    }
};
Office.Controls.PeoplePicker.Parameters = function Office_Controls_PeoplePicker_Parameters() {
};
Office.Controls.PeoplePicker.ISearchPrincipalDataProvider = function() {
};
if (Office.Controls.PeoplePicker.ISearchPrincipalDataProvider.registerInterface)
    Office.Controls.PeoplePicker.ISearchPrincipalDataProvider.registerInterface('Office.Controls.PeoplePicker.ISearchPrincipalDataProvider');
Office.Controls.PeoplePicker._cancelToken = function Office_Controls_PeoplePicker__cancelToken() {
    this._isCanceled$p$0 = false;
};
Office.Controls.PeoplePicker._cancelToken.prototype = {
    _isCanceled$p$0: false,
    get_isCanceled: function Office_Controls_PeoplePicker__cancelToken$get_isCanceled() {
        return this._isCanceled$p$0;
    },
    cancel: function Office_Controls_PeoplePicker__cancelToken$cancel() {
        this._isCanceled$p$0 = true;
    }
};
Office.Controls.PeoplePicker._searchPrincipalServerDataProvider = function Office_Controls_PeoplePicker__searchPrincipalServerDataProvider() {
    this._requestExecutor$p$0 = Office.Controls.Runtime.context.getRequestExecutor();
};
Office.Controls.PeoplePicker._searchPrincipalServerDataProvider._deserializePrincipalsFromResponse$p = function Office_Controls_PeoplePicker__searchPrincipalServerDataProvider$_deserializePrincipalsFromResponse$p(response) {
    return (Office.Controls.Utils.deserializeJSON(response.body)).d.SearchPrincipalsUsingContextWeb.results;
};
Office.Controls.PeoplePicker._searchPrincipalServerDataProvider.prototype = {
    _requestExecutor$p$0: null,
    getPrincipals: function Office_Controls_PeoplePicker__searchPrincipalServerDataProvider$getPrincipals(input, scopes, sources, groupName, maxCount, callback, errorCallback) {
        var requestInfos = new SP.RequestInfo();

        requestInfos.headers = {};
        requestInfos.headers['Accept'] = Office.Controls.PeoplePicker._searchPrincipalServerDataProvider._oDataJSONAcceptHeader$p;
        requestInfos.headers[Office.Controls.Utils.clientTagHeaderName] = 'ClientControls-PeoplePicker';
        requestInfos.method = 'GET';
        var $$t_C = this;

        requestInfos.success = function(infos) {
            if (infos.statusCode === 200) {
                callback(Office.Controls.PeoplePicker._searchPrincipalServerDataProvider._deserializePrincipalsFromResponse$p(infos));
            }
            else {
                Office.Controls.Utils.errorConsole('Bad error code returned by the server : ' + infos.statusCode.toString());
            }
        };
        var $$t_D = this;

        requestInfos.error = function(infos, code, errorMessage) {
            errorCallback(errorMessage);
            Office.Controls.Utils.errorConsole('Error trying to reach the server : ' + errorMessage);
        };
        requestInfos.url = this._buildRequestUrl$p$0(input, scopes, sources, groupName, maxCount);
        this._requestExecutor$p$0.executeAsync(requestInfos);
    },
    _buildRequestUrl$p$0: function Office_Controls_PeoplePicker__searchPrincipalServerDataProvider$_buildRequestUrl$p$0(input, scopes, sources, groupName, maxCount) {
        var url = '_api/';

        url += Office.Controls.PeoplePicker._searchPrincipalServerDataProvider._requestPathEndpoint$p;
        var queryString = '?';

        queryString += 'input=\'' + encodeURIComponent(input) + '\'';
        queryString += '&scopes=' + scopes.toString();
        queryString += '&sources=' + sources.toString();
        if (!Office.Controls.Utils.isNullOrEmptyString(groupName)) {
            queryString += '&groupName=\'' + encodeURIComponent(groupName) + '\'';
        }
        queryString += '&maxCount=' + maxCount.toString();
        return url + queryString;
    }
};
Office.Controls.PeoplePicker.ValidationError = function Office_Controls_PeoplePicker_ValidationError() {
};
Office.Controls.PeoplePicker.ValidationError._createMultipleMatchError$i = function Office_Controls_PeoplePicker_ValidationError$_createMultipleMatchError$i() {
    var err = new Office.Controls.PeoplePicker.ValidationError();

    err.errorName = Office.Controls.PeoplePicker.ValidationError.multipleMatchName;
    err.localizedErrorMessage = Office.Controls._peoplePickerTemplates.getString(Office.Controls._peoplePickerResourcesStrings.pP_MultipleMatch);
    return err;
};
Office.Controls.PeoplePicker.ValidationError._createMultipleEntryError$i = function Office_Controls_PeoplePicker_ValidationError$_createMultipleEntryError$i() {
    var err = new Office.Controls.PeoplePicker.ValidationError();

    err.errorName = Office.Controls.PeoplePicker.ValidationError.multipleEntryName;
    err.localizedErrorMessage = Office.Controls._peoplePickerTemplates.getString(Office.Controls._peoplePickerResourcesStrings.pP_MultipleEntry);
    return err;
};
Office.Controls.PeoplePicker.ValidationError._createNoMatchError$i = function Office_Controls_PeoplePicker_ValidationError$_createNoMatchError$i() {
    var err = new Office.Controls.PeoplePicker.ValidationError();

    err.errorName = Office.Controls.PeoplePicker.ValidationError.noMatchName;
    err.localizedErrorMessage = Office.Controls._peoplePickerTemplates.getString(Office.Controls._peoplePickerResourcesStrings.pP_NoMatch);
    return err;
};
Office.Controls.PeoplePicker.ValidationError._createServerProblemError$i = function Office_Controls_PeoplePicker_ValidationError$_createServerProblemError$i() {
    var err = new Office.Controls.PeoplePicker.ValidationError();

    err.errorName = Office.Controls.PeoplePicker.ValidationError.serverProblemName;
    err.localizedErrorMessage = Office.Controls._peoplePickerTemplates.getString(Office.Controls._peoplePickerResourcesStrings.pP_ServerProblem);
    return err;
};
Office.Controls.PeoplePicker.ValidationError.prototype = {
    errorName: null,
    localizedErrorMessage: null
};
Office.Controls.PeoplePicker._mruCache = function Office_Controls_PeoplePicker__mruCache() {
    this.isCacheAvailable = this._checkCacheAvailability$p$0();
    if (!this.isCacheAvailable) {
        return;
    }
    this._initializeCache$p$0();
};
Office.Controls.PeoplePicker._mruCache.getInstance = function Office_Controls_PeoplePicker__mruCache$getInstance() {
    if (!Office.Controls.PeoplePicker._mruCache._instance$p) {
        Office.Controls.PeoplePicker._mruCache._instance$p = new Office.Controls.PeoplePicker._mruCache();
    }
    return Office.Controls.PeoplePicker._mruCache._instance$p;
};
Office.Controls.PeoplePicker._mruCache.prototype = {
    isCacheAvailable: false,
    _localStorage$p$0: null,
    _dataObject$p$0: null,
    get: function Office_Controls_PeoplePicker__mruCache$get(key, maxResults) {
        if (Office.Controls.Utils.isNullOrUndefined(maxResults) || !maxResults) {
            maxResults = Number.maxValue;
        }
        var numberOfResults = 0;
        var results = new Array(0);
        var cache = this._dataObject$p$0.cacheMapping[Office.Controls.Runtime.context.sharePointHostUrl];
        var cacheLength = cache.length;

        for (var i = cacheLength; i > 0 && numberOfResults < maxResults; i--) {
            var candidate = cache[i - 1];

            if (this._entityMatches$p$0(candidate, key)) {
                results.push(candidate);
                numberOfResults += 1;
            }
        }
        return results;
    },
    set: function Office_Controls_PeoplePicker__mruCache$set(entry) {
        var cache = this._dataObject$p$0.cacheMapping[Office.Controls.Runtime.context.sharePointHostUrl];
        var cacheSize = cache.length;
        var alreadyThere = false;

        for (var i = 0; i < cacheSize; i++) {
            var cacheEntry = cache[i];

            if (cacheEntry.LoginName === entry.LoginName) {
                cache.splice(i, 1);
                alreadyThere = true;
                break;
            }
        }
        if (!alreadyThere) {
            if (cacheSize >= Office.Controls.PeoplePicker._mruCache._maxCacheItem$p) {
                cache.splice(0, 1);
            }
        }
        cache.push(entry);
        this._cacheWrite$p$0(Office.Controls.PeoplePicker._mruCache._localStorageKey$p, Office.Controls.Utils.serializeJSON(this._dataObject$p$0));
    },
    _entityMatches$p$0: function Office_Controls_PeoplePicker__mruCache$_entityMatches$p$0(candidate, key) {
        if (Office.Controls.Utils.isNullOrEmptyString(key) || Office.Controls.Utils.isNullOrUndefined(candidate)) {
            return false;
        }
        key = key.toLowerCase();
        var userNameKey = candidate.LoginName;

        if (Office.Controls.Utils.isNullOrUndefined(userNameKey)) {
            userNameKey = '';
        }
        var divideIndex = userNameKey.indexOf('\\');

        if (divideIndex !== -1 && divideIndex !== userNameKey.length - 1) {
            userNameKey = userNameKey.substr(divideIndex + 1);
        }
        var emailKey = candidate.Email;

        if (Office.Controls.Utils.isNullOrUndefined(emailKey)) {
            emailKey = '';
        }
        var atSignIndex = emailKey.indexOf('@');

        if (atSignIndex !== -1) {
            emailKey = emailKey.substr(0, atSignIndex);
        }
        if (Office.Controls.Utils.isNullOrUndefined(candidate.DisplayName)) {
            candidate.DisplayName = '';
        }
        if (!(userNameKey.toLowerCase()).indexOf(key) || !(emailKey.toLowerCase()).indexOf(key) || !(candidate.DisplayName.toLowerCase()).indexOf(key)) {
            return true;
        }
        return false;
    },
    _initializeCache$p$0: function Office_Controls_PeoplePicker__mruCache$_initializeCache$p$0() {
        var cacheData = this._cacheRetreive$p$0(Office.Controls.PeoplePicker._mruCache._localStorageKey$p);

        if (Office.Controls.Utils.isNullOrEmptyString(cacheData)) {
            this._dataObject$p$0 = new Office.Controls.PeoplePicker._mruCache._mruData();
        }
        else {
            var datas = Office.Controls.Utils.deserializeJSON(cacheData);

            if (datas.cacheVersion !== Office.Controls.PeoplePicker._mruCache._currentVersion$p) {
                this._dataObject$p$0 = new Office.Controls.PeoplePicker._mruCache._mruData();
                this._cacheDelete$p$0(Office.Controls.PeoplePicker._mruCache._localStorageKey$p);
            }
            else {
                this._dataObject$p$0 = datas;
            }
        }
        if (Office.Controls.Utils.isNullOrUndefined(this._dataObject$p$0.cacheMapping[Office.Controls.Runtime.context.sharePointHostUrl])) {
            this._dataObject$p$0.cacheMapping[Office.Controls.Runtime.context.sharePointHostUrl] = new Array(0);
        }
    },
    _checkCacheAvailability$p$0: function Office_Controls_PeoplePicker__mruCache$_checkCacheAvailability$p$0() {
        this._localStorage$p$0 = window.self.localStorage;
        if (Office.Controls.Utils.isNullOrUndefined(this._localStorage$p$0)) {
            return false;
        }
        return true;
    },
    _cacheRetreive$p$0: function Office_Controls_PeoplePicker__mruCache$_cacheRetreive$p$0(key) {
        return this._localStorage$p$0.getItem(key);
    },
    _cacheWrite$p$0: function Office_Controls_PeoplePicker__mruCache$_cacheWrite$p$0(key, value) {
        this._localStorage$p$0.setItem(key, value);
    },
    _cacheDelete$p$0: function Office_Controls_PeoplePicker__mruCache$_cacheDelete$p$0(key) {
        this._localStorage$p$0.removeItem(key);
    }
};
Office.Controls.PeoplePicker._mruCache._mruData = function Office_Controls_PeoplePicker__mruCache__mruData() {
    this.cacheMapping = {};
    this.cacheVersion = Office.Controls.PeoplePicker._mruCache._currentVersion$p;
    this.sharePointHost = Office.Controls.Runtime.context.sharePointHostUrl;
};
Office.Controls._peoplePickerTemplates = function Office_Controls__peoplePickerTemplates() {
};
Office.Controls._peoplePickerTemplates.getString = function Office_Controls__peoplePickerTemplates$getString(stringName) {
    return Office.Controls.Utils.getStringFromResource('PeoplePicker', stringName);
};
Office.Controls._peoplePickerTemplates._getDefaultText$i = function Office_Controls__peoplePickerTemplates$_getDefaultText$i(allowMultiple) {
    if (allowMultiple) {
        return Office.Controls._peoplePickerTemplates.getString(Office.Controls._peoplePickerResourcesStrings.pP_DefaultMessagePlural);
    }
    else {
        return Office.Controls._peoplePickerTemplates.getString(Office.Controls._peoplePickerResourcesStrings.pP_DefaultMessage);
    }
};
Office.Controls._peoplePickerTemplates.generateControlTemplate = function Office_Controls__peoplePickerTemplates$generateControlTemplate(inputName, allowMultiple, defaultTextOverride) {
    var defaultText;

    if (Office.Controls.Utils.isNullOrEmptyString(defaultTextOverride)) {
        defaultText = Office.Controls.Utils.htmlEncode(Office.Controls._peoplePickerTemplates._getDefaultText$i(allowMultiple));
    }
    else {
        defaultText = Office.Controls.Utils.htmlEncode(defaultTextOverride);
    }
    var body = '<div class=\"' + Office.Controls._peoplePickerTemplates._actualControlClass$i + '\" title=\"' + defaultText + '\">';

    body += '<input type=\"hidden\"';
    if (!Office.Controls.Utils.isNullOrEmptyString(inputName)) {
        body += ' name=\"' + Office.Controls.Utils.htmlEncode(inputName) + '\"';
    }
    body += '/>';
    body += '<span class=\"' + Office.Controls._peoplePickerTemplates._defaultTextClass$i + ' ' + Office.Controls._peoplePickerTemplates._helperTextClass$i + '\">' + defaultText + '</span>';
    body += '<span class=\"' + Office.Controls._peoplePickerTemplates._resolvedListClass$i + '\"></span>';
    body += '<input type=\"text\" class=\"' + Office.Controls._peoplePickerTemplates._inputClass$i + '\" size=\"1\" autocorrect=\"off\" autocomplete=\"off\" autocapitalize=\"off\" title=\"' + defaultText + '\"/>';
    body += '<ul class=\"' + Office.Controls._peoplePickerTemplates._autofillContainerClass$i + '\"></ul>';
    body += Office.Controls._peoplePickerTemplates.generateAlertNode();
    body += '</div>';
    return body;
};
Office.Controls._peoplePickerTemplates.generateErrorTemplate = function Office_Controls__peoplePickerTemplates$generateErrorTemplate(ErrorMessage) {
    var innerHtml = '<span class=\"' + Office.Controls._peoplePickerTemplates._errorMessageClass$i + ' ' + Office.Controls._peoplePickerTemplates._controlErrorClass$i + '\">';

    innerHtml += Office.Controls.Utils.htmlEncode(ErrorMessage);
    innerHtml += '</span>';
    return innerHtml;
};
Office.Controls._peoplePickerTemplates.generateAutofillListItemTemplate = function Office_Controls__peoplePickerTemplates$generateAutofillListItemTemplate(principal, source) {
    var elementClass = Office.Controls._peoplePickerTemplates._autofillItemClass$i + ' ' + (source === Office.Controls._principalSource.cache ? Office.Controls._peoplePickerTemplates._autofillMRUClass$i : Office.Controls._peoplePickerTemplates._autofillServerClass$i);
    var titleText = Office.Controls.Utils.htmlEncode(Office.Controls.Utils.isNullOrEmptyString(principal.Email) ? '' : principal.Email);
    var itemHtml = '<li class=\"' + elementClass + '\" ' + Office.Controls._peoplePickerTemplates._autofillItemDataAttribute$i + '=\"' + Office.Controls.Utils.htmlEncode(principal.LoginName) + '\" title=\"' + titleText + '\">';

    itemHtml += '<a onclick=\"return false;\" href=\"#\">';
    itemHtml += '<div class=\"' + Office.Controls._peoplePickerTemplates._autofillMenuLabelClass$i + '\" unselectable=\"on\">' + Office.Controls.Utils.htmlEncode(principal.DisplayName) + '</div>';
    if (!Office.Controls.Utils.isNullOrEmptyString(principal.JobTitle)) {
        itemHtml += '<div class=\"' + Office.Controls._peoplePickerTemplates._autofillMenuSublabelClass$i + '\" unselectable=\"on\">' + Office.Controls.Utils.htmlEncode(principal.JobTitle) + '</div>';
    }
    itemHtml += '</a></li>';
    return itemHtml;
};
Office.Controls._peoplePickerTemplates.generateAutofillListTemplate = function Office_Controls__peoplePickerTemplates$generateAutofillListTemplate(cachedEntries, serverEntries, maxCount) {
    var html = '';

    if (Office.Controls.Utils.isNullOrUndefined(cachedEntries)) {
        cachedEntries = new Array(0);
    }
    if (Office.Controls.Utils.isNullOrUndefined(serverEntries)) {
        serverEntries = new Array(0);
    }
    html += Office.Controls._peoplePickerTemplates._generateAutofillListTemplatePartial$p(cachedEntries, Office.Controls._principalSource.cache);
    if (cachedEntries.length > 0) {
        html += Office.Controls._peoplePickerTemplates._autofillListSeparator$p;
    }
    html += Office.Controls._peoplePickerTemplates._generateAutofillListTemplatePartial$p(serverEntries, 0);
    if (serverEntries.length > 0) {
        html += Office.Controls._peoplePickerTemplates._autofillListSeparator$p;
    }
    html += Office.Controls._peoplePickerTemplates.generateAutofillFooterTemplate(cachedEntries.length + serverEntries.length, maxCount);
    return html;
};
Office.Controls._peoplePickerTemplates._generateAutofillListTemplatePartial$p = function Office_Controls__peoplePickerTemplates$_generateAutofillListTemplatePartial$p(principals, source) {
    var listHtml = '';

    for (var i = 0; i < principals.length; i++) {
        listHtml += Office.Controls._peoplePickerTemplates.generateAutofillListItemTemplate(principals[i], source);
    }
    return listHtml;
};
Office.Controls._peoplePickerTemplates.generateAutofillFooterTemplate = function Office_Controls__peoplePickerTemplates$generateAutofillFooterTemplate(count, maxCount) {
    var footerHtml = '<li class=\"' + Office.Controls._peoplePickerTemplates._autofillMenuFooterClass$i + '\">';
    var footerText;

    if (count >= maxCount) {
        footerText = Office.Controls.Utils.formatString(Office.Controls._peoplePickerTemplates.getString(Office.Controls._peoplePickerResourcesStrings.pP_ShowingTopNumberOfResults), count.toString());
    }
    else {
        footerText = Office.Controls.Utils.formatString(Office.Controls.Utils.getLocalizedCountValue(Office.Controls._peoplePickerTemplates.getString(Office.Controls._peoplePickerResourcesStrings.pP_Results), Office.Controls._peoplePickerTemplates.getString(Office.Controls._peoplePickerResourcesStrings.pP_ResultsIntervals), count), count.toString());
    }
    footerText = Office.Controls.Utils.htmlEncode(footerText);
    footerHtml += footerText;
    footerHtml += '</li>';
    footerHtml += '<li class=\"' + Office.Controls._peoplePickerTemplates._autofillLoadingClass$i + '\"></li>';
    return footerHtml;
};
Office.Controls._peoplePickerTemplates.generateRecordTemplate = function Office_Controls__peoplePickerTemplates$generateRecordTemplate(record) {
    var recordHtml = '<span class=\"' + Office.Controls._peoplePickerTemplates._userRecordClass$i + '\">';

    if (record.isResolved) {
        recordHtml += '<span class=\"' + Office.Controls._peoplePickerTemplates._resolvedUserClass$i + '\" title=\"' + Office.Controls.Utils.htmlEncode(record.text) + '\">' + Office.Controls.Utils.htmlEncode(record.displayName) + '</span>';
    }
    else {
        recordHtml += '<a class=\"' + Office.Controls._peoplePickerTemplates._unresolvedUserClass$i + '\" onclick=\"return false;\" href=\"#\" title=\"' + Office.Controls.Utils.htmlEncode(record.text) + '\">' + Office.Controls.Utils.htmlEncode(record.text) + '</a>';
    }
    recordHtml += '<a class=\"' + Office.Controls._peoplePickerTemplates._recordRemoverClass$i + '\" onclick=\"return false;\" href=\"#\" title=\"' + Office.Controls.Utils.formatString(Office.Controls._peoplePickerTemplates.getString(Office.Controls._peoplePickerResourcesStrings.pP_RemovePerson), Office.Controls.Utils.htmlEncode(record.text)) + '\">' + 'x' + '</a>';
    recordHtml += '</span>';
    return recordHtml;
};
Office.Controls._peoplePickerTemplates.generateAlertNode = function Office_Controls__peoplePickerTemplates$generateAlertNode() {
    var alertHtml = '<div role=\"alert\" class=\"' + Office.Controls._peoplePickerTemplates._alertDivClass$i + '\">';

    alertHtml += '</div>';
    return alertHtml;
};
Office.Controls.PeoplePickerResourcesDefaults = function Office_Controls_PeoplePickerResourcesDefaults() {
};
Office.Controls._peoplePickerResourcesStrings = function Office_Controls__peoplePickerResourcesStrings() {
};
if (Office.Controls.PrincipalInfo.registerClass)
    Office.Controls.PrincipalInfo.registerClass('Office.Controls.PrincipalInfo');
if (Office.Controls.PeoplePickerRecord.registerClass)
    Office.Controls.PeoplePickerRecord.registerClass('Office.Controls.PeoplePickerRecord');
if (Office.Controls._keyCodes.registerClass)
    Office.Controls._keyCodes.registerClass('Office.Controls._keyCodes');
if (Office.Controls.PeoplePicker.registerClass)
    Office.Controls.PeoplePicker.registerClass('Office.Controls.PeoplePicker');
if (Office.Controls.PeoplePicker._internalPeoplePickerRecord.registerClass)
    Office.Controls.PeoplePicker._internalPeoplePickerRecord.registerClass('Office.Controls.PeoplePicker._internalPeoplePickerRecord');
if (Office.Controls.PeoplePicker._autofillContainer.registerClass)
    Office.Controls.PeoplePicker._autofillContainer.registerClass('Office.Controls.PeoplePicker._autofillContainer');
if (Office.Controls.PeoplePicker.Parameters.registerClass)
    Office.Controls.PeoplePicker.Parameters.registerClass('Office.Controls.PeoplePicker.Parameters');
if (Office.Controls.PeoplePicker._cancelToken.registerClass)
    Office.Controls.PeoplePicker._cancelToken.registerClass('Office.Controls.PeoplePicker._cancelToken');
if (Office.Controls.PeoplePicker._searchPrincipalServerDataProvider.registerClass)
    Office.Controls.PeoplePicker._searchPrincipalServerDataProvider.registerClass('Office.Controls.PeoplePicker._searchPrincipalServerDataProvider', null, Office.Controls.PeoplePicker.ISearchPrincipalDataProvider);
if (Office.Controls.PeoplePicker.ValidationError.registerClass)
    Office.Controls.PeoplePicker.ValidationError.registerClass('Office.Controls.PeoplePicker.ValidationError');
if (Office.Controls.PeoplePicker._mruCache.registerClass)
    Office.Controls.PeoplePicker._mruCache.registerClass('Office.Controls.PeoplePicker._mruCache');
if (Office.Controls.PeoplePicker._mruCache._mruData.registerClass)
    Office.Controls.PeoplePicker._mruCache._mruData.registerClass('Office.Controls.PeoplePicker._mruCache._mruData');
if (Office.Controls._peoplePickerTemplates.registerClass)
    Office.Controls._peoplePickerTemplates.registerClass('Office.Controls._peoplePickerTemplates');
if (Office.Controls.PeoplePickerResourcesDefaults.registerClass)
    Office.Controls.PeoplePickerResourcesDefaults.registerClass('Office.Controls.PeoplePickerResourcesDefaults');
if (Office.Controls._peoplePickerResourcesStrings.registerClass)
    Office.Controls._peoplePickerResourcesStrings.registerClass('Office.Controls._peoplePickerResourcesStrings');
Office.Controls._keyCodes.backspace = 8;
Office.Controls._keyCodes.tab = 9;
Office.Controls._keyCodes.escape = 27;
Office.Controls._keyCodes.upArrow = 38;
Office.Controls._keyCodes.downArrow = 40;
Office.Controls._keyCodes.enter = 13;
Office.Controls._keyCodes.deleteKey = 46;
Office.Controls._keyCodes.k = 75;
Office.Controls._keyCodes.v = 86;
Office.Controls._keyCodes.semiColon = 186;
Office.Controls.PeoplePicker.rootClassName = 'office office-peoplepicker';
Office.Controls.PeoplePicker._focusClassName$i = 'office-peoplepicker-focus';
Office.Controls.PeoplePicker._numberOfResults$p = 30;
Office.Controls.PeoplePicker._autofillWait$p = 250;
Office.Controls.PeoplePicker._minimumNumberOfLettersToQuery$p = 3;
Office.Controls.PeoplePicker._maxCacheEntries$p = 5;
Office.Controls.PeoplePicker._autofillContainer._currentOpened$p = null;
Office.Controls.PeoplePicker._autofillContainer._boolBodyHandlerAdded$p = false;
Office.Controls.PeoplePicker._autofillContainer._focusClassName$p = 'office-peoplepicker-autofill-focus';
Office.Controls.PeoplePicker._searchPrincipalServerDataProvider._requestPathEndpoint$p = 'SP.Utilities.Utility.SearchPrincipalsUsingContextWeb';
Office.Controls.PeoplePicker._searchPrincipalServerDataProvider._oDataJSONAcceptHeader$p = 'application/json;odata=verbose';
Office.Controls.PeoplePicker.ValidationError.multipleMatchName = 'MultipleMatch';
Office.Controls.PeoplePicker.ValidationError.multipleEntryName = 'MultipleEntry';
Office.Controls.PeoplePicker.ValidationError.noMatchName = 'NoMatch';
Office.Controls.PeoplePicker.ValidationError.serverProblemName = 'ServerProblem';
Office.Controls.PeoplePicker._mruCache._instance$p = null;
Office.Controls.PeoplePicker._mruCache._localStorageKey$p = 'Office.PeoplePicker.Cache';
Office.Controls.PeoplePicker._mruCache._maxCacheItem$p = 200;
Office.Controls.PeoplePicker._mruCache._currentVersion$p = 0;
Office.Controls._peoplePickerTemplates._actualControlClass$i = 'office-peoplepicker-main';
Office.Controls._peoplePickerTemplates._helperTextClass$i = 'office-helper';
Office.Controls._peoplePickerTemplates._defaultTextClass$i = 'office-peoplepicker-default';
Office.Controls._peoplePickerTemplates._autofillContainerClass$i = 'office-peoplepicker-menu';
Office.Controls._peoplePickerTemplates._resolvedListClass$i = 'office-peoplepicker-recordList';
Office.Controls._peoplePickerTemplates._inputClass$i = 'office-peoplepicker-input';
Office.Controls._peoplePickerTemplates._loadingDataClass$i = 'office-peoplepicker-loading';
Office.Controls._peoplePickerTemplates._errorMessageClass$i = 'office-peoplepicker-error';
Office.Controls._peoplePickerTemplates._controlErrorClass$i = 'office-error';
Office.Controls._peoplePickerTemplates._autofillOpenedClass$i = 'office-peoplepicker-autofillopened';
Office.Controls._peoplePickerTemplates._autofillItemClass$i = 'office-peoplepicker-menu-item';
Office.Controls._peoplePickerTemplates._autofillMRUClass$i = 'office-peoplepicker-autofill-mru';
Office.Controls._peoplePickerTemplates._autofillServerClass$i = 'office-peoplepicker-autofill-Server';
Office.Controls._peoplePickerTemplates._autofillItemDataAttribute$i = 'data-office-peoplepicker-value';
Office.Controls._peoplePickerTemplates._autofillMenuLabelClass$i = 'office-menu-label';
Office.Controls._peoplePickerTemplates._autofillMenuSublabelClass$i = 'office-menu-sublabel';
Office.Controls._peoplePickerTemplates._autofillMenuFooterClass$i = 'office-menu-footer';
Office.Controls._peoplePickerTemplates._autofillLoadingClass$i = 'office-peoplepicker-autofill-loading';
Office.Controls._peoplePickerTemplates._userRecordClass$i = 'office-peoplepicker-record';
Office.Controls._peoplePickerTemplates._resolvedUserClass$i = 'office-peoplepicker-resolved';
Office.Controls._peoplePickerTemplates._unresolvedUserClass$i = 'office-peoplepicker-unresolved';
Office.Controls._peoplePickerTemplates._recordRemoverClass$i = 'office-peoplepicker-deleterecord';
Office.Controls._peoplePickerTemplates._alertDivClass$i = 'office-peoplepicker-alert';
Office.Controls._peoplePickerTemplates._autofillListSeparator$p = '<li><hr></li>';
Office.Controls.PeoplePickerResourcesDefaults.PP_SuggestionsAvailable = 'Suggestions Available';
Office.Controls.PeoplePickerResourcesDefaults.PP_NoMatch = 'We couldn\'t find an exact match.';
Office.Controls.PeoplePickerResourcesDefaults.PP_ShowingTopNumberOfResults = 'Showing the top {0} results';
Office.Controls.PeoplePickerResourcesDefaults.PP_ServerProblem = 'Sorry, we\'re having trouble reaching the server.';
Office.Controls.PeoplePickerResourcesDefaults.PP_DefaultMessagePlural = 'Enter names or email addresses...';
Office.Controls.PeoplePickerResourcesDefaults.PP_MultipleMatch = 'Multiple entries matched, please click to resolve.';
Office.Controls.PeoplePickerResourcesDefaults.PP_Results = 'No results found||Showing {0} result||Showing {0} results';
Office.Controls.PeoplePickerResourcesDefaults.PP_Searching = 'Searching';
Office.Controls.PeoplePickerResourcesDefaults.PP_ResultsIntervals = '0||1||2-';
Office.Controls.PeoplePickerResourcesDefaults.PP_NoSuggestionsAvailable = 'No Suggestions Available';
Office.Controls.PeoplePickerResourcesDefaults.PP_RemovePerson = 'Remove person or group {0}';
Office.Controls.PeoplePickerResourcesDefaults.PP_DefaultMessage = 'Enter a name or email address...';
Office.Controls.PeoplePickerResourcesDefaults.PP_MultipleEntry = 'You can only enter one name.';
Office.Controls._peoplePickerResourcesStrings.pP_DefaultMessage = 'PP_DefaultMessage';
Office.Controls._peoplePickerResourcesStrings.pP_DefaultMessagePlural = 'PP_DefaultMessagePlural';
Office.Controls._peoplePickerResourcesStrings.pP_MultipleEntry = 'PP_MultipleEntry';
Office.Controls._peoplePickerResourcesStrings.pP_MultipleMatch = 'PP_MultipleMatch';
Office.Controls._peoplePickerResourcesStrings.pP_NoMatch = 'PP_NoMatch';
Office.Controls._peoplePickerResourcesStrings.pP_NoSuggestionsAvailable = 'PP_NoSuggestionsAvailable';
Office.Controls._peoplePickerResourcesStrings.pP_RemovePerson = 'PP_RemovePerson';
Office.Controls._peoplePickerResourcesStrings.pP_Results = 'PP_Results';
Office.Controls._peoplePickerResourcesStrings.pP_ResultsIntervals = 'PP_ResultsIntervals';
Office.Controls._peoplePickerResourcesStrings.pP_Searching = 'PP_Searching';
Office.Controls._peoplePickerResourcesStrings.pP_ServerProblem = 'PP_ServerProblem';
Office.Controls._peoplePickerResourcesStrings.pP_ShowingTopNumberOfResults = 'PP_ShowingTopNumberOfResults';
Office.Controls._peoplePickerResourcesStrings.pP_SuggestionsAvailable = 'PP_SuggestionsAvailable';
