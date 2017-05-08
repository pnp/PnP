var CAMControl;
$.ajaxSetup({
    cache: true
});
(function (CAMControl) {
    var spContext; //global sharepoint context used throughout the taxonomy picker control (set in the taxpicker constructor)
    var taxIndex = 0; //keeps index of the taxonomy pickers in use
    //********************** START Term Class **********************
    //constructor for Term
    function Term(rawTerm) {
        try {
            if (rawTerm != null) {
                this.Id = rawTerm.get_id().toString(); //Id of the Term from SharePoint
                this.Name = rawTerm.get_name(); //Default label for the term in SharePoint
                this.PathOfTerm = rawTerm.get_pathOfTerm(); //label path of term delimited by semi-colons (ex: World;Europe;Finland)
                this.Children = new Array(); //child terms of the term
                this.Level = rawTerm.get_pathOfTerm().split(';').length - 1; //integer indicating the level of the term
                this.RawTerm = rawTerm;
                this.IsAvailableForTagging = rawTerm.get_isAvailableForTagging();
                this.IsDeprecated = rawTerm.get_isDeprecated();
            }
        } catch (e) {
            ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
        }
    }

    function KeywordsTerm(rawTerm) {
        try {
            if (rawTerm != null) {
                this.Id = rawTerm.Id;//rawTerm.get_id().toString(); //Id of the Term from SharePoint
                this.Name = rawTerm.DefaultLabel;//rawTerm.get_name(); //Default label for the term in SharePoint
                this.PathOfTerm = rawTerm.Paths[0]//rawTerm.PathOfTerm;//rawTerm.get_pathOfTerm(); //label path of term delimited by semi-colons (ex: World;Europe;Finland)
                this.Children = new Array(); //child terms of the term
                this.Level = this.PathOfTerm.split(';').length - 1;//rawTerm.get_pathOfTerm().split(';').length - 1; //integer indicating the level of the term
                this.RawTerm = rawTerm;
                //this.IsAvailableForTagging = rawTerm.IsAvailableForTagging;//rawTerm.get_isAvailableForTagging();
                //this.IsDeprecated = rawTerm.IsDepricated;//rawTerm.get_isDeprecated();
            }
        } catch (e) {
            ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
        }
    }
    $.extend(Term.prototype, {
        //creates a cloned copy of a term to avoid reference issues
        clone: function () {
            return new Term(this.RawTerm);
        },
        //converts a term to an html tree node
        toHtmlLabel: function () {
            var addlClass = (this.Children.length > 0) ? 'collapsed' : '';
            var addlClass = (this.RawTerm.get_termsCount() > 0) ? 'collapsed' : '';
            return $('<li class="cam-taxpicker-treenode-li"><div class="cam-taxpicker-treenode"><div class="cam-taxpicker-expander ' + addlClass + '" id="' + this.Id + '"></div><img src="../content/images/' + (this.IsAvailableForTagging == true ? 'EMMTerm.png' : 'EMMTermDisabled.png') + '" alt=""/><span class="cam-taxpicker-treenode-title" data-item="' + this.Name + '|' + this.Id + '">' + this.Name + '</span></div></li>');
        }
    });

    $.extend(KeywordsTerm.prototype, {
        clone: function () {
            return new KeywordsTerm(this.RawTerm);
        }, toHtmlLabel: function () {
            var addlClass = (this.Children.length > 0) ? 'collapsed' : '';
            return $('<li class="cam-taxpicker-treenode-li"><div class="cam-taxpicker-treenode"><div class="cam-taxpicker-expander ' + addlClass + '"></div><img src="../content/images/' + (this.IsAvailableForTagging == true ? 'EMMTerm.png' : 'EMMTermDisabled.png') + '" alt=""/><span class="cam-taxpicker-treenode-title" data-item="' + this.Name + '|' + this.Id + '">' + this.Name + '</span></div></li>');
        }
    });
    //********************** END Term Class **********************
    //********************** START TermSet Class **********************
    //constructor for TermSet
    function TermSet(options) {
        try {
            this.Id = options.termSetId; //Id of the TermSet in SharePoint
            this.UseHashtags = options.useHashtags; //bool indicating if the Hashtags termset is used during initalization
            this.UseKeywords = options.useKeywords; //bool indicating if the Keywords termset is used during initalization
            this.Terms = new Array(); //Terms of the termset listed in a heirarchy (if applicable)
            this.FlatTerms = new Array(); //Flat representation of terms in the Termset
            this.RawTerms = null; //Raw terms returned from CSOM
            this.TermsLoaded = false; //boolean indicating if the terms have been returned and loaded from CSOM
            this.OnTermsLoaded = null; //optional callback when terms are loaded
            this.Name; //name of the termset
            this.RawTermSet = null; //Raw termset returned from CSOM
            this.TermSetLoaded = false; //boolean indicating if the termset details are loaded
            this.IsOpenForTermCreation = false; //bool indicating if the termset is open for new term creation
            this.NewTerm = null; //the new term being added
            this.Text;
            this.Event;
        } catch (e) {
            ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
        }
    }
    $.extend(TermSet.prototype, {
        //initializes the Termset, including loading all terms using CSOM
        initialize: function () {
            try {
                this._waitingDlg = $('<div class="cam-taxpicker-waiting"><div class="cam-taxpicker-waiting-overlay"></div><div class="cam-taxpicker-waiting-dlg"><div class="cam-taxpicker-waiting-dlg-inner"><img alt="" src="data:image/gif;base64,R0lGODlhEAAQAIAAAFLOQv///yH/C05FVFNDQVBFMi4wAwEAAAAh+QQFCgABACwJAAIAAgACAAACAoRRACH5BAUKAAEALAwABQACAAIAAAIChFEAIfkEBQoAAQAsDAAJAAIAAgAAAgKEUQAh+QQFCgABACwJAAwAAgACAAACAoRRACH5BAUKAAEALAUADAACAAIAAAIChFEAIfkEBQoAAQAsAgAJAAIAAgAAAgKEUQAh+QQFCgABACwCAAUAAgACAAACAoRRACH5BAkKAAEALAIAAgAMAAwAAAINjAFne8kPo5y02ouzLQAh+QQJCgABACwCAAIADAAMAAACF4wBphvID1uCyNEZM7Ov4v1p0hGOZlAAACH5BAkKAAEALAIAAgAMAAwAAAIUjAGmG8gPW4qS2rscRPp1rH3H1BUAIfkECQoAAQAsAgACAAkADAAAAhGMAaaX64peiLJa6rCVFHdQAAAh+QQJCgABACwCAAIABQAMAAACDYwBFqiX3mJjUM63QAEAIfkECQoAAQAsAgACAAUACQAAAgqMARaol95iY9AUACH5BAkKAAEALAIAAgAFAAUAAAIHjAEWqJeuCgAh+QQJCgABACwFAAIAAgACAAACAoRRADs=" style="width: 32px; height: 32px;" /><span class="ms-accentText" style="font-size: 36px;">' + TaxonomyPickerConsts.WORKING_ON_IT + '</span></div></div></div>');
                $('body').append(this._waitingDlg);
                //Get the taxonomy session using CSOM
                var taxSession = SP.Taxonomy.TaxonomySession.getTaxonomySession(spContext);
                //Use the default term store...this could be extended here to support additional term stores
                var termStore = taxSession.getDefaultSiteCollectionTermStore();
                //get the termset based on the properties of the termset
                if (this.Id != null)
                    this.RawTermSet = termStore.getTermSet(this.Id); //get termset by id
                else if (this.UseHashtags)
                    this.RawTermSet = termStore.get_hashTagsTermSet(); //get the hashtags termset
                else if (this.UseKeywords)
                    this.RawTermSet = termStore.get_keywordsTermSet(); //get the keywords termset
                //get ALL terms for the termset and we will organize them in the async callback
                this.RawTerms = this.RawTermSet.getAllTerms();
                spContext.load(this.RawTermSet);
                spContext.load(this.RawTerms);

                if (this.UseKeywords != true) {
                    spContext.executeQueryAsync(Function.createDelegate(this, this.termsLoadedSuccess), Function.createDelegate(this, this.termsLoadedFailed));
                }
            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }
        },
        //internal callback when terms are returned from CSOM
        termsLoadedSuccess: function () {
            try {
                //set termset properties
                $('body').children('.cam-taxpicker-waiting').remove();
                this.Name = this.RawTermSet.get_name();
                this.IsOpenForTermCreation = this.RawTermSet.get_isOpenForTermCreation();
                //get the enumerator for terms list
                var termEnumerator = this.RawTerms.getEnumerator();
                //get flat list of terms
                this.FlatTerms = new Array();
                while (termEnumerator.moveNext()) {
                    var currentTerm = termEnumerator.get_current();
                    var term = new Term(currentTerm);
                    this.FlatTerms.push(term);
                }
                var topLevel = 0;
                //sort by Name that all of the choice will return alphabetically
                this.FlatTerms.sort(function (a, b) {
                    if (a.Level > topLevel) { topLevel = a.Level; }
                    a = a.Name.toLowerCase();
                    b = b.Name.toLowerCase();
                    if (a < b) return -1;
                    if (a > b) return 1;
                    return 0;
                });
                //build a hierarchical representation of Terms by iterating through all of the terms for each level
                for (var currentLevel = 0; currentLevel <= topLevel; currentLevel++) {
                    for (var i = 0; i < this.FlatTerms.length; i++) {
                        var term = this.FlatTerms[i];
                        if (term.Level == currentLevel) {
                            if (currentLevel == 0) {
                                this.Terms.push(term.clone());
                            }
                            else {
                                this.getTermParentCollectionByPath(term.PathOfTerm).push(term);
                            }
                        }
                    }
                }
                //mark as terms loaded
                this.TermsLoaded = true;
                //call OnTermsLoaded event if not null
                if (this.OnTermsLoaded != null)
                    this.OnTermsLoaded();

            } catch (e) {
                $('body').children('.cam-taxpicker-waiting').remove();
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }
        },
        //internal callback when failed CSOM query occurs getting terms
        termsLoadedFailed: function (event, args) {
            //display error message to user
            $('body').children('.cam-taxpicker-waiting').remove();
            alert(TaxonomyPickerConsts.TERMSET_LOAD_FAILED);
            ULSOnError(args.get_message(), document.location.href, 0);
        },
        //gets a term parent collection based on the path passed in (ex: World;Europe;Finland would return the Europe term)
        getTermParentCollectionByPath: function (path) {
            try {
                var term = null;
                var parts = path.split(';');
                var termList = this.Terms;
                for (var i = 0; i < parts.length - 1; i++) {
                    for (var j = 0; j < termList.length; j++) {
                        if (parts[i] == termList[j].Name) {
                            term = termList[j];
                            termList = term.Children;
                            break;
                        }
                    }
                }

            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }
            return termList;
        },
        //get suggestions based on the values typed by user
        getSuggestions: function (text) {
            try {
                var matches = new Array();
                if (this.UseKeywords != true) {
                    $(this.FlatTerms).each(function (i, e) {
                        if (e.Name.toLowerCase().indexOf(text.toLowerCase()) == 0 && e.IsAvailableForTagging == true && e.IsDeprecated == false)
                            matches.push(e);
                    });
                }
                else {
                    $(this.FlatTermsGlobal).each(function (i, e) {
                        if (e.Name.toLowerCase().indexOf(text.toLowerCase()) == 0)
                            matches.push(e);
                    });

                }
            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }

            return matches;
        },
        //get a term by id
        getTermById: function (id) {
            try {
                if (this.UseKeywords) {
                    this.FlatTerms = this.FlatTermsGlobal;
                }
                for (var i = 0; i < this.FlatTerms.length; i++) {
                    if (this.FlatTerms[i].Id == id)
                        return this.FlatTerms[i];
                }
            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }

            return null;
        },
        //get a term by label match
        getTermsByLabel: function (label) {
            try {
                var matches = new Array();
                for (var i = 0; i < this.FlatTerms.length; i++) {
                    if (this.FlatTerms[i].Name.toLowerCase() == label.toLowerCase())
                        matches.push(this.FlatTerms[i]);
                }
            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }

            return matches;
        },
        //adds a new term to the the root of a termset or as a child of another term
        addTerm: function (label, taxpicker, parentTermId) {
            try {
                var parent = null;
                if (parentTermId == undefined) {
                    //add Term to termset
                    parent = this.RawTermSet;
                }
                else {
                    //find the parent term in the RawTerms
                    for (var i = 0; i < this.FlatTerms.length; i++) {
                        var pt = this.FlatTerms[i];
                        if (pt.Id == parentTermId) {
                            parent = pt;
                        }
                    }
                }
                //make sure the term label doesn't already exist at this level
                if (this.termExists((parentTermId == undefined) ? label : parent.Name + ';' + label))
                    taxpicker.termAddFailed(null, null);
                else {
                    //create the term
                    var id = newGuid();
                    //handle root terms
                    if (parentTermId == undefined) {
                        //initialize the parent as the termset collection
                        parent = {};
                        parent.RawTerm = this.RawTermSet;
                    }
                    this.NewTerm = parent.RawTerm.createTerm(label, taxpicker.LCID, id);
                    spContext.load(this.NewTerm);
                    spContext.executeQueryAsync(Function.createDelegate(taxpicker, taxpicker.termAddSuccess),
                    Function.createDelegate(taxpicker, taxpicker.termAddFailed));
                }
            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }
        },
        //checks if a term exists with the the path passed in
        termExists: function (pathOfTerm) {
            try {
                var termFound = false;
                for (var i = 0; i < this.FlatTerms.length; i++) {
                    if (this.FlatTerms[i].PathOfTerm.toLowerCase() == pathOfTerm.toLowerCase()) {
                        termFound = true;
                        break;
                    }
                }
            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }

            return termFound;
        }
    });
    //********************** End TermSet Class **********************
    //********************** START TaxonomyPicker Class **********************
    //constructor for TaxonomyPicker
    function TaxonomyPicker(control, options, changeCallback) {
        try {
            this.TermSet = new TermSet(options); //the termset the taxonomy picker is bound to...loaded in the inialize function
            this._changeCallback = changeCallback; //event callback for when the control value changes
            this.LCID = (options.lcid) ? options.lcid : 1033; //the locale id for term creation (default is 1033)
            this.Language = (options.language) ? options.language : 'en-us'; //the language code for the control (default is en-us)
            this.MarkerMarkup = '<span id="caretmarker"></span>'; //the marketup const
            this._isMulti = options.isMulti; //specifies if the user can select multiple terms
            this._isReadOnly = options.isReadOnly; //specifies whether the control is used for display purposes
            this.IsOpenForTermCreation = options.IsOpenForTermCreation;
            this._allowFillIn = options.allowFillIn; //specifies if the user can add new terms (only applies to Open Termsets)
            this._termSetId = options.termSetId; //the termset id to bind the control to
            this._useHashtags = options.useHashtags; //indicates that the hashtags termset should be used tp bind the control
            this._useKeywords = options.useKeywords; //indicates that the keywords termset should be used to bind the control
            this._initialValue = control.val(); //the initial value of the control
            this._maxSuggestions = (options.maxSuggestions) ? options.maxSuggestions : 10; //maximum number of suggestions to load...default is 10
            this._control = control; //the wrapper container all the taxonomy pickers controls are contained in
            this._dlgButton = null; //the button used to launch the taxonomy picker dialog
            this._editor = null; //the editor control for the taxonomy picker
            this._suggestionContainer = null; //the suggestions container for the taxonomy picker
            this._dlgSuggestionContainer = null; //the dialog suggestions container for the taxonomy picker
            this._hiddenValidated = control; //the hidden control that contains all validated term selections
            this._waitingDlg = null; //the waiting dialog
            this._selectedTerms = new Array(); //Array of selected terms
            this._tempSelectedTerms = new Array(); //Snapshot of selected terms for use in the picker dialog (kept to support cancel in the dialog)
            this._dialog = null; //the dialog control
            this._dlgCurrTerm = null; //the current term highlighted in the taxonomy picker dialog
            this._dlgCurrTermNode = null; //the current tree node selected
            this._dlgCloseButton = null; //the Close button in the taxonomy picker dialog
            this._dlgOkButton = null; //the Ok button in the taxonomy picker dialog
            this._dlgCancelButton = null; //the Cancel button in the taxonomy picker dialog
            this._dlgSelectButton = null; //the Select >> button in the taxonomy picker dialog
            this._dlgEditor = null; //the editor control in the taxonomy picker dialog
            this._dlgAddNewTermButton = null; //the "Add New Item" link display in the dialog for Open TermSets
            this._dlgNewNode; //container for a new node added to an Open Termset in the taxonomy picker dialog
            this._dlgNewNodeEditor; //the editor field for add new node in the taxonomy picker dialog
            this.FlatTermsGlobal = new Array();

            //initialize the taxonomy picker
            this.initialize();
        } catch (e) {
            ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
        }
    }
    $.extend(TaxonomyPicker.prototype, {
        //initializes the taxonomy picker
        initialize: function () {
            try {
                this.TermSet.initialize(); //initialize the termset to populate available terms
                //get script path so we can load translation files
                var scriptUrl = '';
                var scriptRevision = '';
                $('script').each(function (i, el) {
                    if (el.src.toLowerCase().indexOf('taxonomypickercontrol.js') > -1) {
                        scriptUrl = el.src;
                        scriptRevision = scriptUrl.substring(scriptUrl.indexOf('.js') + 3);
                        scriptUrl = scriptUrl.substring(0, scriptUrl.indexOf('.js'));
                    }
                });
                //load translation files
                var resourcesFile = scriptUrl + '_resources.' + this.Language.substring(0, 2).toLowerCase() + '.js';
                $.getScript(resourcesFile);
                //create a new wrapper for the control using a div
                this._control = $('<div class="cam-taxpicker"></div>');
                //detach the hidden field from the parent and append to the wrapper
                var parent = this._hiddenValidated.parent();
                if (!parent.hasClass('cam-taxpicker')) {
                    this._hiddenValidated = this._hiddenValidated.detach();
                    parent.append(this._control);
                }
                else {
                    this._control = parent;
                    //empty the selected term from hidden field
                    this._hiddenValidated.val("");
                    //added to trigger the change event 
                    this._hiddenValidated.trigger('change');
                    this._selectedTerms = new Array();                    
                }
                this._suggestionContainer = parent.next().is(".cam-taxpicker-suggestion-container") ? parent.next() : $('<div class="cam-taxpicker-suggestion-container"></div>');
                //$('<div class="cam-taxpicker-suggestion-container"></div>');
                this._dlgButton = parent.find(".cam-taxpicker-button").length > 0 ? parent.find(".cam-taxpicker-button") : $('<div class="cam-taxpicker-button"></div>');
                //$('<div class="cam-taxpicker-button"></div>');
                if (!this._isReadOnly) {
                    this._editor = parent.find(".cam-taxpicker-editor").length > 0 ? parent.find(".cam-taxpicker-editor") : $('<div class="cam-taxpicker-editor" contenteditable="true"></div>');
                    //$('<div class="cam-taxpicker-editor" contenteditable="true"></div>');
                    this._control.empty().append(this._editor).append(this._dlgButton).append(this._hiddenValidated);
                    
                    if (!parent.next().is(".cam-taxpicker-suggestion-container")) {
                        this._control.after(this._suggestionContainer);
                    }
                    else {
                        //close the suggestion panel
                        this._suggestionContainer.hide();
                    }
                    this._editor.html('');
                    this._initialValue = '';
                }
                else {
                    this._editor = parent.find(".cam-taxpicker-editor-readonly").length > 0 ? parent.find(".cam-taxpicker-editor-readonly") : $('<div class="cam-taxpicker-editor-readonly" contenteditable="false"></div>');
                    //$('<div class="cam-taxpicker-editor-readonly" contenteditable="false"></div>');
                    this._control.empty().append(this._editor).append(this._hiddenValidated);
                }

                //initialize value if it exists
                if (this._initialValue != undefined && this._initialValue.length > 0) {
                    var terms = JSON.parse(this._initialValue);
                    for (var i = 0; i < terms.length; i++) {
                        //add the term to selected terms array
                        var t = new Term(null);
                        t.Id = terms[i].Id;
                        t.Name = terms[i].Name;
                        this._selectedTerms.push(t);
                    }
                    //refresh the html in the editor control
                    this._editor.html(this.selectedTermsToHtml());
                }
                //wire up control events
                this._dlgButton.click(Function.createDelegate(this, this.showPickerDialog)); //dialog button is clicked
                this._editor.keydown(Function.createDelegate(this, this.keydown)); //key is pressed in the editor control
                $(document).mousedown(Function.createDelegate(this, this.checkExternalClick)); //mousedown somewhere in the document
            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }
        },
        //handle reset
        reset: function () {
            this._selectedTerms = new Array();
            this._editor.html('');
        },
        //handle keydown event in editor control
        keydown: function (event) {
            try {
                // if the control is readonly then ignore all keystrokes
                if (this._isReadOnly) { return false; }
                //get the keynum
                var keynum = event.which;
                //get all parameters to deal with the key event
                var caret = this.getCaret(this._editor[0]); //the cursor position
                var rawText = this._editor.text(); //the raw text in the editor (html stripped out)

                var selection = '';
                if (window.getSelection) //get selection (highlighted text)
                    selection = window.getSelection().toString(); //modern browser
                else
                    selection = document.selection.createRange().text; //IE<9
                //handle specific keys
                if (keynum == 46) { //delete key pressed
                    //delete anything that was selected
                    if (selection.length > 0) {
                        var newText = rawText.substring(0, caret - selection.length) + this.MarkerMarkup + rawText.substring(caret, rawText.length);
                        var textValidation = this.validateText(newText);
                        this._editor.html(textValidation.html);
                        //set the cursor position at the marker
                        this.setCaret();
                        //show suggestions
                        this.showSuggestions(textValidation, caret);
                    }
                    //cancel the keypress
                    return false;
                }
                else if (keynum == 8) { //backspace key pressed
                    //delete anything that was selected OR the last character if nothing selected
                    var newText = '';
                    if (selection.length > 0) {
                        newText = rawText.substring(0, caret - selection.length) + this.MarkerMarkup + rawText.substring(caret, rawText.length);
                        var textValidation = this.validateText(newText);
                        this._editor.html(textValidation.html);
                        //set the cursor position at the marker
                        this.setCaret();
                        //show suggestions
                        this.showSuggestions(textValidation, caret - selection.length - 1);
                    }
                    else {
                        var firstPart = rawText.substring(0, caret - 1);
                        if (firstPart.charAt(firstPart.length - 1) == ';')
                            firstPart = firstPart.substring(0, firstPart.length - 1);
                        newText = firstPart + this.MarkerMarkup + rawText.substring(caret, rawText.length);
                        var textValidation = this.validateText(newText);
                        this._editor.html(textValidation.html);
                        //set the cursor position at the marker
                        this.setCaret();
                        //show suggestions
                        this.showSuggestions(textValidation, caret - 2);
                    }
                    //cancel the keypress
                    return false;
                }
                else if (keynum >= 48 && keynum <= 90 || keynum == 32) { // An ascii character or a space has been pressed
                    // keynum is not taking in account shift key and always results in the uppercase value
                    if (event.shiftKey == false && keynum >= 65 && keynum <= 90) {
                        keynum += 32;
                    }
                    //get new text, taking in account selections
                    var newText = ''
                    var char = String.fromCharCode(keynum);
                    if (keynum == 32) //convert space to &nbsp;
                        char = '&nbsp;';
                    //calculate new text and then convert to html
                    if (caret < rawText.length)
                        newText = rawText.substring(0, caret - selection.length) + String.fromCharCode(keynum) + this.MarkerMarkup + rawText.substring(caret, rawText.length);
                    else
                        newText = rawText.substring(0, caret - selection.length) + rawText.substring(caret, rawText.length) + String.fromCharCode(keynum) + this.MarkerMarkup;

                    var keyInput = this._editor.next().next();
                    if ($(keyInput).attr("id") == "taxPickerKeywordsTerms") {
                        var keyUnsolved = document.getElementById("keyUnResolved");
                        if (keyUnsolved != null && keyUnsolved != 'undefined') {
                            var rex = /(<([^>]+)>)/ig;
                            $("#keyUnResolved").val(newText.replace(rex, ""));
                        }
                    }

                    //get text validation and set html in editor
                    var textValidation = this.validateText(newText);
                    this._editor.html(textValidation.html);
                    //set the cursor position at the marker
                    this.setCaret();
                    //show suggestions
                    this.showSuggestions(textValidation, caret);
                    return false;
                }
                else if (keynum == 9) { //Tab key pressed
                    //validate raw text OR mark invalid
                    var textValidation = this.validateText(rawText);
                    var html = this.markInvalidTerms(textValidation);
                    this._editor.html(html);
                    //close the suggestion panel
                    this._suggestionContainer.hide();
                }
            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }
        },
        keyWordDownMethod: function (event) {


        },
        contentInfoSuccess: function (event, args) {
            try {
                var digest = (event.childNodes[0].childNodes[1]).childNodes[0];
                var KeyWordsPostInput = '{"start":"' + this.Text + '","sspList":"","lcid":1033,"termSetList":"' + this.TermSet.Id + '","anchorId":"00000000-0000-0000-0000-000000000000","isSpanTermStores":true,"isSpanTermSets":true,"isIncludeUnavailable":false,"isIncludeDeprecated":false,"isAddTerms":true,"isIncludePathData":false,"excludeKeyword":false,"excludedTermset":"00000000-0000-0000-0000-000000000000"}';
                var requestURL = decodeURIComponent(getQueryStringParameter("SPHostUrl")) + "/_vti_bin/TaxonomyInternalService.json/GetSuggestions";
                $.ajax({
                    url: requestURL,
                    type: "POST",
                    headers: {
                        "X-RequestDigest": digest.data
                    },
                    data: KeyWordsPostInput,
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    cache: false,
                    success: Function.createDelegate(this, this.termsKeywordLoadedSuccess),
                    error: function ServiceFailed(result) {
                        alert('Service call failed: ' + result.status + '' + result.statusText);
                    }
                });
            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }
        },
        termsKeywordLoadedSuccess: function (event, args) {
            try {
                this.UseKeywords = true;
                if (event.d.Groups.length > 0) {
                    this.Result = event.d.Groups[0].Suggestions;
                    //this.Result = jQuery.parseJSON(event)
                    //set termset properties
                    this.Name = "Keywords";//this.RawTermSet.get_name();
                    //get flat list of terms
                    this.FlatTerms = new Array();
                    for (var i = 0; i < this.Result.length; i++) {
                        var currentTerm = this.Result[i];
                        var term = new KeywordsTerm(currentTerm);
                        this.FlatTerms.push(term);
                    }
                    var topLevel = 0;
                    //sort by Name that all of the choice will return alphabetically
                    this.FlatTerms.sort(function (a, b) {
                        if (a.Level > topLevel) { topLevel = a.Level; }
                        a = a.Name.toLowerCase();
                        b = b.Name.toLowerCase();
                        if (a < b) return -1;
                        if (a > b) return 1;
                        return 0;
                    });
                    this.Terms = new Array();
                    //build a hierarchical representation of Terms by iterating through all of the terms for each level
                    for (var currentLevel = 0; currentLevel <= topLevel; currentLevel++) {
                        for (var i = 0; i < this.FlatTerms.length; i++) {
                            var term = this.FlatTerms[i];
                            if (term.Level == currentLevel) {
                                if (currentLevel == 0) {
                                    this.Terms.push(term.clone());
                                }
                                else {
                                    this.getTermParentCollectionByPath(term.PathOfTerm).push(term);
                                }
                            }
                        }
                    }
                    //mark as terms loaded
                    this.TermsLoaded = true;

                    this.TermSet.FlatTermsGlobal = this.FlatTerms.slice(0);
                    //call OnTermsLoaded event if not null
                    if (this.OnTermsLoaded != null)
                        this.OnTermsLoaded();

                    var suggestions = this.TermSet.getSuggestions(this.Text);
                    //trim suggestions based on what is already in this._selectedTerms
                    suggestions = this.trimSuggestions(suggestions);
                    this._suggestionContainer.empty().append($('<div class="cam-taxpicker-suggestion-title">' + TaxonomyPickerConsts.SUGGESTIONS_HEADER + '</div>'));
                    if (suggestions.length > 0) {
                        $(suggestions).each(Function.createDelegate(this, function (i, e) {
                            if (i < this._maxSuggestions) {
                                var match = e.Name.substring(0, this.Text.length); //get the matched text so we can highlight it
                                var itemHtml = $('<div class="cam-taxpicker-suggestion-item" tabindex="' + i + '" data-item="' + e.Id + '">' + e.Name.replace(match, '<span style="background-color: yellow;">' + match + '</span>') + ' ' + e.PathOfTerm.replace(/;/g, ':') + '</div>');
                                this._suggestionContainer.append(itemHtml);
                                itemHtml.keydown(Function.createDelegate(this, this.suggestionKeydown));
                                itemHtml.click(Function.createDelegate(this, this.suggestionClicked));
                            }
                        }));
                        this._suggestionContainer.closest('.cam-taxpicker-suggestion-item').focus();
                        this._suggestionContainer.show();
                    }
                    else {
                        this._suggestionContainer.hide();
                    }
                }
                else {
                    this._suggestionContainer.hide();
                }
            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }
        },
        //handle keydown event in dlgeditor control
        dlgkeydown: function (event, args) {
            try {
                // if the control is readonly then ignore all keystrokes
                if (this._isReadOnly) { return false; }
                //get the keynum
                var keynum = event.which;
                //get all parameters to deal with the key event
                var caret = this.getCaret(this._dlgEditor[0]); //the cursor position
                var rawText = this._dlgEditor.text(); //the raw text in the editor (html stripped out)
                var selection = '';
                if (window.getSelection) //get selection (highlighted text)
                    selection = window.getSelection().toString(); //modern browser
                else
                    selection = document.selection.createRange().text; //IE<9
                //handle specific keys
                if (keynum == 46) { //delete key pressed
                    //delete anything that was selected
                    if (selection.length > 0) {
                        var newText = rawText.substring(0, caret - selection.length) + this.MarkerMarkup + rawText.substring(caret, rawText.length);
                        var textValidation = this.validateText(newText);
                        this._dlgEditor.html(textValidation.html);
                        //set the cursor position at the marker
                        this.setdlgCaret();
                        //show suggestions
                        this.dlgShowSuggestions(textValidation, caret);
                    }
                    //cancel the keypress
                    return false;
                }
                else if (keynum == 8) { //backspace key pressed
                    //delete anything that was selected OR the last character if nothing selected
                    var newText = '';
                    if (selection.length > 0) {
                        newText = rawText.substring(0, caret - selection.length) + this.MarkerMarkup + rawText.substring(caret, rawText.length);
                        var textValidation = this.validateText(newText);
                        this._dlgEditor.html(textValidation.html);
                        //set the cursor position at the marker
                        this.setdlgCaret();
                        //show suggestions
                        this.dlgShowSuggestions(textValidation, caret - selection.length - 1);
                    }
                    else {
                        var firstPart = rawText.substring(0, caret - 1);
                        if (firstPart.charAt(firstPart.length - 1) == ';')
                            firstPart = firstPart.substring(0, firstPart.length - 1);
                        newText = firstPart + this.MarkerMarkup + rawText.substring(caret, rawText.length);
                        var textValidation = this.validateText(newText);
                        this._dlgEditor.html(textValidation.html);
                        //set the cursor position at the marker
                        this.setdlgCaret();
                        //show suggestions
                        this.dlgShowSuggestions(textValidation, caret - 2);
                    }
                    //cancel the keypress
                    return false;
                }
                else if (keynum >= 48 && keynum <= 90 || keynum == 32) { // An ascii character or a space has been pressed
                    // keynum is not taking in account shift key and always results in the uppercase value
                    if (event.shiftKey == false && keynum >= 65 && keynum <= 90) {
                        keynum += 32;
                    }
                    //get new text, taking in account selections
                    var newText = ''
                    var char = String.fromCharCode(keynum);
                    if (keynum == 32) //convert space to &nbsp;
                        char = '&nbsp;';
                    //calculate new text and then convert to html
                    if (caret < rawText.length)
                        newText = rawText.substring(0, caret - selection.length) + String.fromCharCode(keynum) + this.MarkerMarkup + rawText.substring(caret, rawText.length);
                    else
                        newText = rawText.substring(0, caret - selection.length) + rawText.substring(caret, rawText.length) + String.fromCharCode(keynum) + this.MarkerMarkup;
                    var keyInput = this._editor.next().next();

                    //get text validation and set html in editor
                    var textValidation = this.validateText(newText);
                    this._dlgEditor.html(textValidation.html);
                    //set the cursor position at the marker
                    this.setdlgCaret();
                    //show suggestions
                    this.dlgShowSuggestions(textValidation, caret);
                    return false;
                }
                else if (keynum == 9) { //Tab key pressed
                    //validate raw text OR mark invalid
                    var textValidation = this.validateText(rawText);
                    var html = this.markInvalidTerms(textValidation);
                    this._dlgEditor.html(html);
                    //close the suggestion panel
                    this._dlgSuggestionContainer.hide();
                }
            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }
        },
        //get the cursor position in a content editable div
        getCaret: function (target) {
            try {
                var isContentEditable = target.contentEditable === 'true';
                //HTML5
                if (window.getSelection) {
                    //contenteditable
                    if (isContentEditable) {
                        target.focus();
                        var range1 = window.getSelection().getRangeAt(0),
                        range2 = range1.cloneRange();
                        range2.selectNodeContents(target);
                        range2.setEnd(range1.endContainer, range1.endOffset);
                        return range2.toString().length;
                    }
                    //textarea
                    return target.selectionStart;
                }
                //IE<9
                if (document.selection) {
                    target.focus();
                    //contenteditable
                    if (isContentEditable) {
                        var range1 = document.selection.createRange(),
                        range2 = document.body.createTextRange();
                        range2.moveToElementText(target);
                        range2.setEndPoint('EndToEnd', range1);
                        return range2.text.length;
                    }
                    //textarea
                    var pos = 0,
                    range = target.createTextRange(),
                    range2 = document.selection.createRange().duplicate(),
                    bookmark = range2.getBookmark();
                    range.moveToBookmark(bookmark);
                    while (range.moveStart('character', -1) !== 0) pos++;
                    return pos;
                }
                //not supported
                return 0;
            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }
        },
        //sets the cursor caret (position in the editor control)
        setCaret: function () {
            try {
                //find the marker
                var marker = null;
                for (var i = 0; i < this._editor[0].childNodes.length; i++) {
                    if (this._editor[0].childNodes[i].tagName == 'SPAN' && this._editor[0].childNodes[i].id == 'caretmarker') {
                        marker = this._editor[0].childNodes[i];
                        break;
                    }
                }
                if (marker != null) {
                    //HTML5
                    if (window.getSelection) {
                        //set cursor at the marker
                        var range = document.createRange();
                        range['setStartAfter'](marker);
                        var selection = window.getSelection();
                        selection.removeAllRanges();
                        selection.addRange(range);
                    }
                    //IE<9
                    if (document.selection) {
                        //TODO: this isn't currently working without a selection (BUG)
                        /*
                        range = document.selection.createRange();
                        var range1 = range.duplicate();
                        range1.moveToElementText(marker);
                        range.setEndPoint('StartToEnd', range1);
                        range.setEndPoint('EndToStart', range1);
                        */
                    }
                    //remove the marker
                    marker.parentNode.removeChild(marker);
                }
            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }
        },
        //sets the cursor caret (position in the editor control)
        setdlgCaret: function () {
            try {
                //find the marker
                var marker = null;
                for (var i = 0; i < this._dlgEditor[0].childNodes.length; i++) {
                    if (this._dlgEditor[0].childNodes[i].tagName == 'SPAN' && this._dlgEditor[0].childNodes[i].id == 'caretmarker') {
                        marker = this._dlgEditor[0].childNodes[i];
                        break;
                    }
                }
                if (marker != null) {
                    //HTML5
                    if (window.getSelection) {
                        //set cursor at the marker
                        var range = document.createRange();
                        range['setStartAfter'](marker);
                        var selection = window.getSelection();
                        selection.removeAllRanges();
                        selection.addRange(range);
                    }
                    //IE<9
                    if (document.selection) {
                        //TODO: this isn't currently working without a selection (BUG)
                        /*
                        range = document.selection.createRange();
                        var range1 = range.duplicate();
                        range1.moveToElementText(marker);
                        range.setEndPoint('StartToEnd', range1);
                        range.setEndPoint('EndToStart', range1);
                        */
                    }
                    //remove the marker
                    marker.parentNode.removeChild(marker);
                }
            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }
        },
        //validates the text input into ranges and html output
        validateText: function (txt) {
            try {
                var textValidation = { html: '', ranges: [] };
                var terms = txt.split(';');
                var newTerms = new Array();
                var ptr = 0;
                var rPtr = 0;
                //loop through parts and look for existing validations
                for (var i = 0; i < terms.length; i++) {
                    var t = terms[i].replace(/^\s+/, ""); //trim left
                    var t_compare = t.replace(this.MarkerMarkup, '');
                    var r = { text: t, bottom: rPtr, top: (rPtr + t.length - 1), valid: false };
                    rPtr = r.top;
                    for (var j = ptr; j < this._selectedTerms.length; j++) {
                        if (this._selectedTerms[j].Name.toLowerCase() == t_compare.toLowerCase()) {
                            newTerms.push(this._selectedTerms[j]);
                            r.valid = true;
                            ptr = j + 1;
                            break;
                        }
                    }
                    //add to the range selection
                    textValidation.ranges.push(r);
                    //build the html for this
                    if (r.valid)
                        textValidation.html += '<span class="cam-taxpicker-term-selected">' + t + '</span>';
                    else
                        textValidation.html += t;
                    //add separator
                    if (i < terms.length - 1) {
                        textValidation.html += '<span>;&nbsp;</span>';
                        rPtr += 3;
                    }
                }
                //reset this._selectedTerms
                this._selectedTerms = newTerms;
                this._hiddenValidated.val(JSON.stringify(this._selectedTerms));

            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }
            return textValidation;
        },
        //marks text input with valid and invalid markup
        markInvalidTerms: function (textValidation) {
            try {
                var html = '';
                for (var i = 0; i < textValidation.ranges.length; i++) {
                    if (textValidation.ranges[i].valid)
                        html += '<span class="cam-taxpicker-term-selected">' + textValidation.ranges[i].text + '</span>';
                    else {
                        //check for a single match we can validate against
                        var matches = this.TermSet.getTermsByLabel(textValidation.ranges[i].text);
                        if (matches.length == 1 && (this._selectedTerms.length == 0 || this._isMulti)) {
                            this.pushSelectedTerm(matches[0]);
                            html += '<span class="cam-taxpicker-term-selected">' + textValidation.ranges[i].text + '</span>';
                        }
                        else {
                            if (textValidation.html == '') {

                            }
                            else {
                                html += '<span class="cam-taxpicker-term-invalid">' + textValidation.ranges[i].text + '</span>';
                            }
                        }
                        //check for ambiguous matches
                        if (matches.length > 1) {
                            //TODO: popup anbiguous terms dialog (Enhancement)
                        }
                        else {
                            html += '<span class="cam-taxpicker-term-invalid">' + textValidation.ranges[i].text + '</span>';
                        }
                    }
                    if (i < textValidation.ranges.length - 1)
                        html += '<span>;&nbsp;</span>';

                }

            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }

            return html;
        },
        //shows suggestions based on the unvalidated text being entered
        showSuggestions: function (textValidation, caret) {
            try {
                //find the unvalidated text the cursor is in
                var range = null;
                for (var i = 0; i < textValidation.ranges.length; i++) {
                    if (caret >= textValidation.ranges[i].bottom && caret <= textValidation.ranges[i].top) {
                        if (!textValidation.ranges[i].valid) {
                            range = textValidation.ranges[i];
                        }
                        break;
                    }
                }
                if (range != null) {
                    var txt = range.text;
                    //clear the marker from the txt
                    txt = txt.replace(this.MarkerMarkup, '');
                    this.Text = txt;
                    if (txt.length > 0) {
                        //look for all matching suggestions
                        if (!this._useKeywords) {
                            var suggestions = this.TermSet.getSuggestions(txt);
                            //trim suggestions based on what is already in this._selectedTerms
                            suggestions = this.trimSuggestions(suggestions);
                           
                            this._suggestionContainer.empty().append($('<div class="cam-taxpicker-suggestion-title">' + TaxonomyPickerConsts.SUGGESTIONS_HEADER + '</div>'));
                            if (suggestions.length > 0) {
                                $(suggestions).each(Function.createDelegate(this, function (i, e) {
                                    if (i < this._maxSuggestions) {
                                        var match = e.Name.substring(0, txt.length); //get the matched text so we can highlight it
                                        var itemHtml = $('<div class="cam-taxpicker-suggestion-item" tabindex="' + i + '" data-item="' + e.Id + '">' + e.Name.replace(match, '<span style="background-color: yellow;">' + match + '</span>') + ' [' + this.TermSet.Name + ':' + e.PathOfTerm.replace(/;/g, ':') + ']</div>');
                                        this._suggestionContainer.append(itemHtml);
                                        itemHtml.keydown(Function.createDelegate(this, this.suggestionKeydown));
                                        itemHtml.click(Function.createDelegate(this, this.suggestionClicked));
                                    }
                                }));
                                this._suggestionContainer.closest('.cam-taxpicker-suggestion-item').focus();
                                this._suggestionContainer.show();
                            }
                            else {
                                this._suggestionContainer.hide();
                            }
                        }
                        else {
                            var contentInfoURL = decodeURIComponent(getQueryStringParameter("SPHostUrl")) + "/_api/contextinfo";
                            $.ajax({
                                url: contentInfoURL,
                                type: "POST",
                                contentType: "application/json; charset=utf-8",
                                cache: false,
                                success: Function.createDelegate(this, this.contentInfoSuccess),
                                error: function ServiceFailed(result) {
                                    alert('Service call failed: ' + result.status + '' + result.statusText);
                                }
                            });
                        }

                    }
                }
                else {
                    this._suggestionContainer.hide();
                }
            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }
        },
        //shows suggestions based on the unvalidated text being entered
        dlgShowSuggestions: function (textValidation, caret) {
            try {
                //find the unvalidated text the cursor is in
                var range = null;
                for (var i = 0; i < textValidation.ranges.length; i++) {
                    if (caret >= textValidation.ranges[i].bottom && caret <= textValidation.ranges[i].top) {
                        if (!textValidation.ranges[i].valid) {
                            range = textValidation.ranges[i];
                        }
                        break;
                    }
                }
                if (range != null && this.TermSet.TermsLoaded) {
                    var txt = range.text;
                    //clear the marker from the txt
                    txt = txt.replace(this.MarkerMarkup, '');
                    if (txt.length > 0) {
                        //look for all matching suggestions
                        var suggestions = this.TermSet.getSuggestions(txt);
                        //trim suggestions based on what is already in this._selectedTerms
                        suggestions = this.trimSuggestions(suggestions);
                        this._dlgSuggestionContainer.empty().append($('<div class="cam-taxpicker-suggestion-title">' + TaxonomyPickerConsts.SUGGESTIONS_HEADER + '</div>'));
                        if (suggestions.length > 0) {
                            $(suggestions).each(Function.createDelegate(this, function (i, e) {
                                if (i < this._maxSuggestions) {
                                    var match = e.Name.substring(0, txt.length); //get the matched text so we can highlight it
                                    var itemHtml = $('<div class="cam-taxpicker-suggestion-item" tabindex="' + i + '" data-item="' + e.Id + '">' + e.Name.replace(match, '<span style="background-color: yellow;">' + match + '</span>') + ' [' + this.TermSet.Name + ':' + e.PathOfTerm.replace(/;/g, ':') + ']</div>');
                                    this._dlgSuggestionContainer.append(itemHtml);
                                    itemHtml.click(Function.createDelegate(this, this.dlgSuggestionClicked));
                                }
                            }));
                        }
                        this._dlgSuggestionContainer.show();
                    }
                }
                else
                    this._dlgSuggestionContainer.hide();
            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }
        },
        //term node add is canceled
        termNodeAddCancel: function (event) {
            try {
                //remove the expand/collapse image if needed
                if (this._dlgCurrTermNode != null && this._dlgCurrTermNode.parent().next().children().length <= 1) {
                    this._dlgCurrTermNode.prev().prev().removeClass('collapsed');
                    this._dlgCurrTermNode.prev().prev().removeClass('expanded');
                }
                //cancel the add by removing the new term
                if (this._dlgNewNode != null)
                    this._dlgNewNode.remove();
            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }
        },
        //term node is clicked in the treeview
        termNodeClicked: function (event) {
            try {
                //clear any term that was in the middle of an add
                //this.termNodeAddCancel(event);
                //set the _dlgCurrTermNode
                this._dlgCurrTermNode = $(event.target);
                //change the style of the node to selected
                $('.cam-taxpicker-treenode-title').removeClass('selected');
                $(event.target).addClass('selected');
                //ignore events from root
                if (!$(event.target).hasClass('root')) {
                    //get the term clicked and set currNode
                    var itemdata = $(event.target).attr('data-item').split('|');
                    this._dlgCurrTerm = this.TermSet.getTermById(itemdata[1]);
                }
                else
                    this._dlgCurrTerm = null;
            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }
        },
        //term node is double clicked in the treeview
        termNodeDoubleClicked: function (event) {
            try {
                //clear any term that was in the middle of an add
                //this.termNodeAddCancel(event);
                //set the _dlgCurrTermNode
                this._dlgCurrTermNode = $(event.target);
                //ignore events from root
                if (!$(event.target).hasClass('root')) {
                    //get the term clicked
                    var itemdata = $(event.target).attr('data-item').split('|');
                    term = this.TermSet.getTermById(itemdata[1]);
                    //add the term to selected terms array
                    this.pushSelectedTerm(term);
                    //refresh the html in the editor control
                    this._dlgEditor.html(this.selectedTermsToHtml());
                }
            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }
        },
        //dialog select button is clicked...use this to add any selected node as a selected term
        dialogSelectButtonClicked: function (event) {
            try {
                if (this._dlgCurrTerm != null) {
                    //add the term to selected terms array
                    this.pushSelectedTerm(this._dlgCurrTerm);
                    //refresh the html in the editor control
                    this._dlgEditor.html(this.selectedTermsToHtml());
                }
            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }
        },
        //dialog OK button clicked
        dialogOkClicked: function (event) {
            try {
                //update the control value
                this._editor.html(this.selectedTermsToHtml());
                //close the dialog
                this.closePickerDialog(event);
                if (this._changeCallback != null)
                    this._changeCallback();
            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }
        },
        //dialog Cancel button clicked
        dialogCancelClicked: function (event) {
            try {
                //reset the selected terms
                this._selectedTerms = this._tempSelectedTerms;
                //close the dialog
                this.closePickerDialog(event);
            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }
        },
        //dialog new term button is clicked
        dialogNewTermClicked: function (event) {
            try {
                this._dlgNewNodeEditor = $('<div class="cam-taxpicker-treenode-newnode" style="min-width: 100px;" contenteditable="true"></div>');
                this._dlgNewNode = $('<li class="cam-taxpicker-treenode-li newNode"></li>').append($('<div class="cam-taxpicker-treenode"></div>').append('<div class="cam-taxpicker-expander"></div><img src="../content/images/EMMTerm.png" alt=""/>').append(this._dlgNewNodeEditor));
                //get the container for the new node
                var ul = this._dlgCurrTermNode.parent().next();
                ul.prepend(this._dlgNewNode);
                //toggle the expand on the parent node
                ul.show();
                this._dlgCurrTermNode.prev().prev().removeClass('collapsed');
                this._dlgCurrTermNode.prev().prev().addClass('expanded');
                //set focus on the newNode editor and wire events
                this._dlgNewNodeEditor.focus();
                this._dlgNewNodeEditor.keydown(Function.createDelegate(this, this.dialogNewTermKeydown));
            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }
        },
        //fires for each keydown in the new term editor
        dialogNewTermKeydown: function (event) {
            try {
                //check for Tab, Esc, or Enter keys
                var keynum = event.keyCode;
                if (keynum == 27) { //Esc pressed
                    this.termNodeAddCancel(event);
                }
                else if (keynum == 13 || keynum == 9) { //Enter or Tab pressed
                    //add the new term and cancel the keypress
                    var txt = this._dlgNewNodeEditor.text();
                    //ensure hashtags start with #
                    if (this._useHashtags && txt.indexOf('#') != 0)
                        txt = '#' + txt;
                    if (this._dlgCurrTerm != null)
                        this.TermSet.addTerm(txt, this, this._dlgCurrTerm.Id);
                    else
                        this.TermSet.addTerm(txt, this);
                    //cancel the keypress for Enter
                    if (keynum == 13)
                        return false;
                }
            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }
        },
        //successful callback from creating a new term
        termAddSuccess: function (event, args) {
            try {
                //add the new term to all applicable collections
                var newTerm = new Term(this.TermSet.NewTerm);
                this.TermSet.FlatTerms.push(newTerm); //add to the flat terms list
                var parentTerm = this.TermSet.getTermParentCollectionByPath(newTerm.PathOfTerm);
                parentTerm.push(newTerm.clone());//add to the hierarchy terms list
                //get the container and replace the new node with a non-editable node
                var ul = this._dlgNewNode.parent();
                this._dlgNewNode.remove();
                var newNode = newTerm.toHtmlLabel();
                ul.prepend(newNode);
                //change the style to selected and wire events
                $('.cam-taxpicker-treenode-title').removeClass('selected');
                var title = newNode.find('.cam-taxpicker-treenode-title');
                title.addClass('selected');
                title.click(Function.createDelegate(this, this.termNodeClicked));
                title.dblclick(Function.createDelegate(this, this.termNodeDoubleClicked));
                //set the _dlgCurrTermNode and _dlgCurrTerm
                this._dlgCurrTermNode = $(event.target);
                this._dlgCurrTerm = newTerm;
            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }
        },
        //failed callback from trying to create a new term
        termAddFailed: function (event, args) {
            try {
                //remove the expand/collapse image if needed
                if (this._dlgCurrTermNode.parent().next().children().length <= 1) {
                    this._dlgCurrTermNode.prev().prev().removeClass('collapsed');
                    this._dlgCurrTermNode.prev().prev().removeClass('expanded');
                }
                //cancel the add by removing the new term
                this._dlgNewNode.remove();
            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }
        },
        //fires when a user selected a suggested term in the suggestions list
        suggestionKeydown: function (event) {
            try {
                //check for Keyup, Keydown, Tab, Esc, or Enter keys
                var keynum = event.keyCode;
                var obj = $(event.target);
                if (keynum == 38) {
                    obj.prev().focus();
                }
                else if (keynum == 40) {
                    obj.next().focus();
                }
                else if (keynum == 13 || keynum == 9) {
                    //check sender type...move up to parent if this is a span
                    if (obj[0].tagName == 'SPAN')
                        obj = obj.parent();
                    //get the termId from the data-item attribute of the target
                    var termId = obj.attr('data-item');
                    //get the term from the termset in memory
                    var term = this.TermSet.getTermById(termId);
                    //add the term to selected terms array
                    this.pushSelectedTerm(term);
                    //refresh the html in the editor control
                    this._editor.html(this.selectedTermsToHtml());
                    this._suggestionContainer.hide();
                    this._editor.focus();
                    if (this._changeCallback != null)
                        this._changeCallback();
                }
            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }
        },
        //fires when a user keydown a suggested term in the suggestions list
        suggestionClicked: function (event) {
            try {
                var obj = $(event.target);
                //check sender type...move up to parent if this is a span
                if (obj[0].tagName == 'SPAN')
                    obj = obj.parent();
                //get the termId from the data-item attribute of the target
                var termId = obj.attr('data-item');
                //get the term from the termset in memory
                var term = this.TermSet.getTermById(termId);
                //add the term to selected terms array
                this.pushSelectedTerm(term);
                var editorhtml = this._editor.html();
                if (editorhtml.indexOf(';') > -1 && this._isMulti) {
                    //var regex = '/(]+)>)/ig'; 
                    var regex = /(<([^>]+)>)/ig;
                    var result = editorhtml.replace(regex, "");
                    var splitHtml = result.split(';&nbsp;');
                    if (obj[0].innerText.toLowerCase().indexOf(splitHtml[splitHtml.length - 1].toLowerCase()) > -1) {
                        editorhtml = "";
                        for (var i = 0; i < splitHtml.length - 1; i++) {
                            var isExist = false;
                            for (var j = 0; j < this._selectedTerms.length; j++) {
                                if ((this._selectedTerms[j].Name == (splitHtml[i]))) {
                                    isExist = true;
                                }
                            }
                            if (isExist == false) {
                                editorhtml += splitHtml[i] + ";";
                            }
                        }
                    }
                    //refresh the html in the editor control
                    this._editor.html(editorhtml + this.selectedTermsToHtml());
                }
                else {
                    this._editor.html(this.selectedTermsToHtml());
                }

                this._suggestionContainer.hide();
                this._editor.focus();
                if (this._changeCallback != null)
                    this._changeCallback();
            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }
        },
        //fires when a user selected a suggested term in the suggestions list
        dlgSuggestionClicked: function (event) {
            try {
                var obj = $(event.target);
                //check sender type...move up to parent if this is a span
                if (obj[0].tagName == 'SPAN')
                    obj = obj.parent();
                //get the termId from the data-item attribute of the target
                var termId = obj.attr('data-item');
                //get the term from the termset in memory
                var term = this.TermSet.getTermById(termId);
                //add the term to selected terms array
                this.pushSelectedTerm(term);

                var editorhtml = this._editor.html();
                //refresh the html in the editor control
                if (editorhtml.indexOf(';') > -1) {
                    var splitHtml = editorhtml.split(';');
                    if (obj[0].innerText.indexOf(splitHtml[splitHtml.length - 1]) > -1) {
                        for (var i = 0; i < splitHtml.length; i++) {
                            if (i != splitHtml.length - 1) {
                                editorhtml += splitHtml[i];
                            }
                        }
                        this._dlgEditor.html(editorhtml + this.selectedTermsToHtml());
                    }
                }
                else {
                    this._dlgEditor.html(this.selectedTermsToHtml());
                }
                this._dlgSuggestionContainer.hide();
                this._dlgEditor.focus();
                if (this._changeCallback != null)
                    this._changeCallback();
            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }
        },
        //used to check if focus is lost from the control (invalidate and hide suggestions)
        checkExternalClick: function (event) {
            try {
                //check if the target is outside the picker
                if (!$.contains(this._control[0], event.target) && !$.contains(this._suggestionContainer[0], event.target) && this._dialog != null && !$.contains(this._dialog[0], event.target)) {
                    var rawText = this._editor.text(); //the raw text in the editor (html stripped out)
                    var textValidation = this.validateText(rawText); //get the text validation
                    var html = this.markInvalidTerms(textValidation); //mark invalid terms
                    this._editor.html(html); //set the editor
                    this._suggestionContainer.hide(); //hide suggestions
                }
            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }
        },
        //show the dialog picker
        showPickerDialog: function (event) {
            try {
                //check to make sure the termset has loaded
                if (!this.TermSet.TermsLoaded) {
                    this.TermSet.OnTermsLoaded = Function.createDelegate(this, this.showPickerDialog);
                    //add the waiting indicator to the body
                  //  this._waitingDlg = $('<div class="cam-taxpicker-waiting"><div class="cam-taxpicker-waiting-overlay"></div><div class="cam-taxpicker-waiting-dlg"><div class="cam-taxpicker-waiting-dlg-inner"><img alt="" src="data:image/gif;base64,R0lGODlhEAAQAIAAAFLOQv///yH/C05FVFNDQVBFMi4wAwEAAAAh+QQFCgABACwJAAIAAgACAAACAoRRACH5BAUKAAEALAwABQACAAIAAAIChFEAIfkEBQoAAQAsDAAJAAIAAgAAAgKEUQAh+QQFCgABACwJAAwAAgACAAACAoRRACH5BAUKAAEALAUADAACAAIAAAIChFEAIfkEBQoAAQAsAgAJAAIAAgAAAgKEUQAh+QQFCgABACwCAAUAAgACAAACAoRRACH5BAkKAAEALAIAAgAMAAwAAAINjAFne8kPo5y02ouzLQAh+QQJCgABACwCAAIADAAMAAACF4wBphvID1uCyNEZM7Ov4v1p0hGOZlAAACH5BAkKAAEALAIAAgAMAAwAAAIUjAGmG8gPW4qS2rscRPp1rH3H1BUAIfkECQoAAQAsAgACAAkADAAAAhGMAaaX64peiLJa6rCVFHdQAAAh+QQJCgABACwCAAIABQAMAAACDYwBFqiX3mJjUM63QAEAIfkECQoAAQAsAgACAAUACQAAAgqMARaol95iY9AUACH5BAkKAAEALAIAAgAFAAUAAAIHjAEWqJeuCgAh+QQJCgABACwFAAIAAgACAAACAoRRADs=" style="width: 32px; height: 32px;" /><span class="ms-accentText" style="font-size: 36px;">' + TaxonomyPickerConsts.WORKING_ON_IT + '</span></div></div></div>');
                  //  $('body').append(this._waitingDlg);
                }
                else {
                    //remove the waiting indicator (if exists)
                    $('body').children('.cam-taxpicker-waiting').remove();
                    //capture what terms are current selected (so we can support cancel)
                    this._tempSelectedTerms = this._selectedTerms.slice(0);
                    //initialize the dialog if null
                    if (this._dialog == null) {
                        this._dialog = $('<div class="cam-taxpicker-dialog"></div>');
                        var dlg = $('<div class="cam-taxpicker-dialog-content"></div>');
                        //build dialog header with button
                        var dlgHeader = $('<div class="cam-taxpicker-dialog-content-header"><h1 class="cam-taxpicker-dialog-content-header-title">' + TaxonomyPickerConsts.DIALOG_HEADER + this.TermSet.Name + '</h1></div>');
                        this._dlgCloseButton = $('<div class="cam-taxpicker-dialog-content-close"></div>');
                        dlgHeader.append(this._dlgCloseButton);
                        dlg.append(dlgHeader);
                        //build dialog body
                        var dlgSubheader = $('<div class="cam-taxpicker-dialog-content-subheader"><img class="cam-taxpicker-dialog-content-subheader-img" src="../content/images/EMMDoubleTag.png" alt="" /></div>');
                        this._dlgAddNewTermButton = $('<a style="cursor: pointer;">' + TaxonomyPickerConsts.DIALOG_ADD_LINK + '</a>');
                        if (this.TermSet.IsOpenForTermCreation) {
                            dlgSubheader.append($('<div class="cam-taxpicker-dialog-content-subheader-title">' + TaxonomyPickerConsts.DIALOG_ADD_TITLE + '&nbsp;</div>'));
                            dlgSubheader.append($('<div class="cam-taxpicker-dialog-content-subheader-addnew"></div>').append(this._dlgAddNewTermButton));
                        }
                        var dlgBody = $('<div class="cam-taxpicker-dialog-content-body"></div>');
                        dlgBody.append(dlgSubheader);
                        var dlgBodyContainer = $('<div class="cam-taxpicker-dialog-tree-container"></div>');
                        //build the termset hierarchy
                        this._dlgCurrTermNode = $('<span class="cam-taxpicker-treenode-title root selected">' + this.TermSet.Name + '</span>');
                        var root = $('<li class="cam-taxpicker-treenode-li"></li>').append($('<div class="cam-taxpicker-treenode"></div>').append('<div class="cam-taxpicker-expander expanded"></div>').append('<img src="../content/images/EMMTermSet.png" alt=""/>').append(this._dlgCurrTermNode));
                        root.append(buildTermSetTreeLevel(this.TermSet.Terms, true, false));
                        dlgBodyContainer.append($('<ul class="cam-taxpicker-treenode-ul root" style="height: 100%;"></ul>').append(root));
                        //build the dialog editor area
                        //TODO: convert the dlgEditor with contenteditable="true" just like the main editor (Enhancement)
                        this._dlgSuggestionContainer = $('<div class="cam-taxpicker-suggestion-container"></div>');
                        this._dlgEditor = $('<div class="cam-taxpicker-dialog-selection-editor" RestrictPasteToText="true" AllowMultiLines="false" contenteditable="true"></div>');
                        this._dlgSelectButton = $('<button class="cam-taxpicker-ey-select">' + TaxonomyPickerConsts.BUTTON_TEXT + ' >></button>');
                        dlgBody.append(dlgBodyContainer).append($('<div class="cam-taxpicker-dialog-selection-container"></div>').append(this._dlgSelectButton).append(this._dlgEditor).append(this._dlgSuggestionContainer));
                        dlg.append(dlgBody);
                        this._dialog.empty().append($('<div class="cam-taxpicker-dialog-overlay"></div>')).append(dlg);
                        //add button area
                        var dlgButtonArea = $('<div class="cam-taxpicker-dialog-button-container"></div>')
                        this._dlgOkButton = $('<button class="cam-taxpicker-ey-Primary" style="float: right;">OK</button>');
                        this._dlgCancelButton = $('<button class="cam-taxpicker-ey-secondary" style="float: right;">CANCEL</button>');
                        dlgBody.append(dlgButtonArea.append(this._dlgCancelButton).append(this._dlgOkButton));
                    }
                    //set the value in the dialogs editor field
                    this._dlgEditor.html(this.selectedTermsToHtml());
                    //add the dialog to the body
                    $('body').append(this._dialog);
                    //wire events all the dialog events
                    $('.cam-taxpicker-expander').click(Function.createDelegate(this, this.expandTerm));
                    $('.cam-taxpicker-treenode-title').click(Function.createDelegate(this, this.termNodeClicked));
                    $('.cam-taxpicker-treenode-title').dblclick(Function.createDelegate(this, this.termNodeDoubleClicked));
                    this._dlgSelectButton.click(Function.createDelegate(this, this.dialogSelectButtonClicked));
                    this._dlgCloseButton.click(Function.createDelegate(this, this.dialogCancelClicked));
                    this._dlgOkButton.click(Function.createDelegate(this, this.dialogOkClicked));
                    this._dlgCancelButton.click(Function.createDelegate(this, this.dialogCancelClicked));
                    this._dlgAddNewTermButton.click(Function.createDelegate(this, this.dialogNewTermClicked));
                    this._dlgEditor.keydown(Function.createDelegate(this, this.dlgkeydown));
                }
            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }
        },
        expandTerm: function () {
            try {

                if (!event.srcElement.parentElement.parentElement.children[1].children.length > 0) {
                    var foundTerm = this.TermSet.getTermById($(event.srcElement.parentElement.children[2]).attr("data-item").split("|")[1]);
                    var childrenTerms = new Array();
                    var termEnumerator = this.TermSet.RawTerms.getEnumerator();
                    while (termEnumerator.moveNext()) {
                        var currentTerm = termEnumerator.get_current();
                        var term = new Term(currentTerm);
                        if (term.Level != 0) {
                            var levelIndex = term.PathOfTerm.split(';').indexOf($(event.srcElement.parentElement.children[2]).text());
                            if (levelIndex != -1) {
                                if (term.PathOfTerm.split(';')[levelIndex + 1] == term.Name) {
                                    childrenTerms.push(term);
                                }
                            }
                        }
                    }
                    if (childrenTerms.length > 0) {
                        for (i = 0; i < childrenTerms.length; i++) {
                            var tHtml = childrenTerms[i].toHtmlLabel();
                            tHtml.append($('<ul class="cam-taxpicker-treenode-ul"></ul>'));
                            var html = $(event.srcElement.parentElement.parentElement.children[1]);
                            html.append(tHtml);
                            var divId = '#' + childrenTerms[i].Id;
                            $(divId).click(Function.createDelegate(this, this.expandTerm));
                        }
                        $('.cam-taxpicker-treenode-title').click(Function.createDelegate(this, this.termNodeClicked));
                        $('.cam-taxpicker-treenode-title').dblclick(Function.createDelegate(this, this.termNodeDoubleClicked));
                    }

                    if ($(event.srcElement).hasClass('collapsed')) {
                        $(event.srcElement).removeClass('collapsed');
                        $(event.srcElement).addClass('expanded');
                        $(event.srcElement).parent().next().show();
                    }
                }
                else {
                    if ($(event.srcElement).hasClass('expanded')) {
                        $(event.srcElement).removeClass('expanded');
                        $(event.srcElement).addClass('collapsed');
                        $(event.srcElement).parent().next().hide();
                    }
                    else if ($(event.srcElement).hasClass('collapsed')) {
                        $(event.srcElement).removeClass('collapsed');
                        $(event.srcElement).addClass('expanded');
                        $(event.srcElement).parent().next().show();
                    }
                }
            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }
        },
        //closes the picker dialog
        closePickerDialog: function (event) {
            try {
                //remove the picker dialog from the body
                $('body').children('.cam-taxpicker-dialog').remove();
                this._editor.focus();
            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }
        },
        //adds a new term to the end of this._selectedTerms
        pushSelectedTerm: function (term) {
            try {

                if (!this.existingTerm(term)) {
                    //clone the term so we don't messup the original
                    var clonedTerm = term.clone();
                    //clear the RawTerm so it can be serialized
                    clonedTerm.RawTerm = null;
                    //pop the existing term if this isn't a multi-select
                    if (!this._isMulti)
                        this.popSelectedTerm();
                    //add the term to the selected terms array
                    this._selectedTerms.push(clonedTerm);
                    this._hiddenValidated.val(JSON.stringify(this._selectedTerms));
                }
            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }
        },
        //if the term already exists in the selected terms then don't add it
        existingTerm: function (term) {
            try {
                for (var j = 0; j < this._selectedTerms.length; j++) {
                    if (this._selectedTerms[j].Id == term.Id) {
                        return true;
                    }
                }
                return false;
            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }
        },
        //removes the last term from this._selectedTerms
        popSelectedTerm: function () {
            try {
                //remove the last selected term
                this._selectedTerms.pop();
                this._hiddenValidated.val(JSON.stringify(this._selectedTerms));
            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }
        },
        //converts this._selectedTerms to html for an editor field
        selectedTermsToHtml: function () {
            try {
                var termsHtml = '';
                for (var i = 0; i < this._selectedTerms.length; i++) {
                    var e = this._selectedTerms[i];
                    termsHtml += '<span class="cam-taxpicker-term-selected">' + e.Name + '</span><span>;&nbsp;</span>';
                }
                return termsHtml;
            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }
        },
        //trim suggestions based on existing selections
        trimSuggestions: function (suggestions) {
            try {

                var trimmedSuggestions = new Array();
                for (var i = 0; i < suggestions.length; i++) {
                    var suggestion = suggestions[i];
                    var found = false;
                    for (var j = 0; j < this._selectedTerms.length; j++) {
                        if (this._selectedTerms[j].Id == suggestion.Id) {
                            found = true;
                            break;
                        }
                    }
                    //add the suggestion if it was not found
                    if (!found)
                        trimmedSuggestions.push(suggestion);
                }
                return trimmedSuggestions;
            } catch (e) {
                ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
            }
        }
    });
    //********************** END TaxonomyPicker Class **********************
    //called recursively to build a treeview of terms for a termset
    function buildTermSetTreeLevel(termList, show) {
        try {

            var addlStyle = (show) ? 'style="display: block;"' : '';
            var html = $('<ul class="cam-taxpicker-treenode-ul" ' + addlStyle + '></ul>');
            for (var i = 0; i < termList.length; i++) {
                var term = termList[i];
                //convert the term to an html tree node
                var tHtml = term.toHtmlLabel();
                if (term.IsDeprecated == false) {
                    //add children if they exist
                    //if (term.Children.length > 0) {
                    //    if (isLevel0)
                    //        tHtml.append(buildTermSetTreeLevel(term.Children, false));
                    //}
                    //else {
                    //add empty UL for future elements
                    tHtml.append($('<ul class="cam-taxpicker-treenode-ul"></ul>'));
                    //}
                    //append the term html to the parent ul
                    html.append(tHtml);
                }
            }
            return html;
        } catch (e) {
            ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
        }
    }
    //called recursively to build hierarchical representation of terms in a termset
    function getTerms(term, termEnumerator) {
        try {
            for (i = 0; i < term.termsCount; i++) {
                termEnumerator.moveNext();
                var currentTerm = termEnumerator.get_current();
                var cTerm = {
                    name: currentTerm.get_name(),
                    id: currentTerm.get_id(),
                    pathOfTerm: currentTerm.get_pathOfTerm(),
                    termsCount: currentTerm.get_termsCount(),
                    childTerms: []
                };
                //get the child terms for this term
                getTerms(cTerm, termEnumerator);
                term.childTerms[i] = cTerm;
            }
        } catch (e) {
            ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
        }
    }
    //creates a new guid
    function newGuid() {
        try {
            var result, i, j;
            result = '';
            for (j = 0; j < 32; j++) {
                if (j == 8 || j == 12 || j == 16 || j == 20)
                    result = result + '-';
                i = Math.floor(Math.random() * 16).toString(16).toUpperCase();
                result = result + i;
            }
            return result
        } catch (e) {
            ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
        }
    }
    //extends jquery to support taxpicker function
    $.fn.taxpicker = function (options, ctx, changeCallback) {
        try {
            //TODO: display error message when the control isn't bound correctly????
            //verify context
            if (!ctx)
                return this;
            //verify an empty collection wasn't passed
            if (!this.length)
                return this;
            //make sure this is a hidden element
            if (this[0].tagName.toLowerCase() != 'input' || this[0].type.toLowerCase() != 'hidden')
                return this;
            //set spcontext
            spContext = ctx;
            if ($.taxpicker == undefined)
                $.taxpicker = [];
            //create new TaxonomyPicker instance and increment index (in case we need to re-reference)
            $.taxpicker[taxIndex] = new TaxonomyPicker(this, options, changeCallback);
        } catch (e) {
            ULSOnError("STACK : " + e.stack + " MESSAGE: " + e.message, document.location.href, 0);
        }
    };
})(CAMControl || (CAMControl = {}));
