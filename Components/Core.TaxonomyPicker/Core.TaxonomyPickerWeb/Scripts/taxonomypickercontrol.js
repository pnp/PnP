var CAMControl;
(function (CAMControl) {
    var spContext; //global sharepoint context used throughout the taxonomy picker control (set in the taxpicker constructor)
    var taxIndex = 0; //keeps index of the taxonomy pickers in use

    //********************** START Term Class **********************
    //constructor for Term
    function Term(rawTerm) {
        if (rawTerm != null) {
            this.Id = rawTerm.get_id().toString(); //Id of the Term from SharePoint
            this.Name = rawTerm.get_name(); //Default label for the term in SharePoint
            this.PathOfTerm = rawTerm.get_pathOfTerm(); //label path of term delimited by semi-colons (ex: World;Europe;Finland)
            this.Children = new Array(); //child terms of the term
            this.Level = rawTerm.get_pathOfTerm().split(';').length - 1; //integer indicating the level of the term
            this.RawTerm = rawTerm;
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
            return $('<li class="cam-taxpicker-treenode-li"><div class="cam-taxpicker-treenode"><div class="cam-taxpicker-expander ' + addlClass + '"></div><img src="../styles/images/EMMTerm.png" alt=""/><span class="cam-taxpicker-treenode-title"  data-item="' + this.Name + '|' + this.Id + '">' + this.Name + '</span></div></li>');
        }
    });
    //********************** END Term Class **********************

    //********************** START TermSet Class **********************
    //constructor for TermSet
    function TermSet(options) {
        this.Id = options.termSetId; //Id of the TermSet in SharePoint
        this.UseHashtags = options.useHashtags; //bool indicating if the Hashtags termset is used during initalization
        this.UseKeywords = options.useKeywords; //bool indicating if the Keywords termset is used during initalization
        this.Terms = new Array(); //Terms of the termset listed in a heirarchy (if applicable)
        this.FlatTerms = new Array(); //Flat representation of terms in the Termset
        this.FlatTermsForSuggestions = new Array();
        this.RawTerms = null; //Raw terms returned from CSOM
        this.TermsLoaded = false; //boolean indicating if the terms have been returned and loaded from CSOM
        this.OnTermsLoaded = null; //optional callback when terms are loaded
        this.Name; //name of the termset
        this.RawTermSet = null; //Raw termset returned from CSOM
        this.TermSetLoaded = false; //boolean indicating if the termset details are loaded
        this.IsOpenForTermCreation = false; //bool indicating if the termset is open for new term creation
        this.NewTerm = null; //the new term being added

        //TODO NEW STUFF HERE
        this.FilterTermId = options.filterTermId; // To support filter terms based on Id
        this.LevelToShowTerms = options.levelToShowTerms; // show terms only till the specified level
        this.UseTermSetasRootNode = options.useTermSetasRootNode //bool indicating if termset to be shown as root node or not
    }
    $.extend(TermSet.prototype, {
        //initializes the Termset, including loading all terms using CSOM
        initialize: function () {
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
            spContext.load(this.RawTerms, 'Include(Id,Name,PathOfTerm,Labels)');
            spContext.executeQueryAsync(Function.createDelegate(this, this.termsLoadedSuccess), Function.createDelegate(this, this.termsLoadedFailed));
        },
        //internal callback when terms are returned from CSOM
        termsLoadedSuccess: function () {
            //set termset properties
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


            var filterTerm;
            if (this.FilterTermId != null && this.FilterTermId) {
                filterTerm = this.getTermById(this.FilterTermId);
            }

            //build a hierarchical representation of Terms by iterating through all of the terms for each level
            for (var currentLevel = 0; currentLevel <= topLevel; currentLevel++) {
                if (this.LevelToShowTerms > currentLevel || typeof(this.LevelToShowTerms) === 'undefined') {
                    for (var i = 0; i < this.FlatTerms.length; i++) {
                        var term = this.FlatTerms[i];
                        if (term.Level == currentLevel) {
                            var path = term.PathOfTerm.split(';');
                            if (
                                ((path.length == this.LevelToShowTerms && this.FilterTermId != null && this.FilterTermId == term.Id) ||
                                (this.FilterTermId != null && term.PathOfTerm.indexOf(filterTerm.Name) > -1 && this.LevelToShowTerms - 1 == term.Level)
                                ) || typeof(filterTerm) == 'undefined')
                            {
                                if (currentLevel == 0) {
                                    this.Terms.push(term.clone());
                                    this.FlatTermsForSuggestions.push(term);
                                }
                                else {
                                    this.getTermParentCollectionByPath(term.PathOfTerm).push(term);
                                    this.FlatTermsForSuggestions.push(term);
                                }
                            }
                        }
                    }
                }
            }

            //mark as terms loaded
            this.TermsLoaded = true;

            //call OnTermsLoaded event if not null
            if (this.OnTermsLoaded != null)
                this.OnTermsLoaded();
        },
        //internal callback when failed CSOM query occurs getting terms
        termsLoadedFailed: function (event, args) {
            //display error message to user
            alert(TaxonomyPickerConsts.TERMSET_LOAD_FAILED);
        },
        //gets a term parent collection based on the path passed in (ex: World;Europe;Finland would return the Europe term)
        getTermParentCollectionByPath: function (path) {
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

            return termList;
        },
        //get suggestions based on the values typed by user
        getSuggestions: function (text) {
            var matches = new Array();
            $(this.FlatTermsForSuggestions).each(function (i, e) {
                if (e.Name.toLowerCase().indexOf(text.toLowerCase()) == 0)
                    matches.push(e);
            });
            return matches;
        },
        //get a term by id
        getTermById: function (id) {
            for (var i = 0; i < this.FlatTerms.length; i++) {
                if (this.FlatTerms[i].Id == id)
                    return this.FlatTerms[i];
            }

            return null;
        },
        //get a term by label match
        getTermsByLabel: function (label) {
            var matches = new Array();
            for (var i = 0; i < this.FlatTerms.length; i++) {
                if (this.FlatTerms[i].Name.toLowerCase() == label.toLowerCase())
                    matches.push(this.FlatTerms[i]);
            }

            return matches;
        },
        //adds a new term to the the root of a termset or as a child of another term
        addTerm: function (label, taxpicker, parentTermId) {
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
        },
        //checks if a term exists with the the path passed in
        termExists: function (pathOfTerm) {
            var termFound = false;
            for (var i = 0; i < this.FlatTerms.length; i++) {
                if (this.FlatTerms[i].PathOfTerm.toLowerCase() == pathOfTerm.toLowerCase()) {
                    termFound = true;
                    break;
                }
            }

            return termFound;
        }
    });
    //********************** End TermSet Class **********************

    //********************** START TaxonomyPicker Class **********************
    //constructor for TaxonomyPicker
    function TaxonomyPicker(control, options, context, changeCallback) {
        this.TermSet = new TermSet(options); //the termset the taxonomy picker is bound to...loaded in the inialize function

        this._context = context; //Context passed in from control
        this._changeCallback = changeCallback; //event callback for when the control value changes
        this.LCID = (options.lcid) ? options.lcid : 1033; //the locale id for term creation (default is 1033)
        this.Language = (options.language) ? options.language : 'en-us'; //the language code for the control (default is en-us)
        this.MarkerMarkup = '<span id="caretmarker"></span>'; //the marketup const
        this._isMulti = options.isMulti; //specifies if the user can select multiple terms
        this._isReadOnly = options.isReadOnly; //specifies whether the control is used for display purposes 
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

        //initialize the taxonomy picker
        this.initialize();
    }

    $.extend(TaxonomyPicker.prototype, {
        //initializes the taxonomy picker
        initialize: function () {
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
            if (typeof CAMControl.resourceLoaded == 'undefined') {
                CAMControl.resourceLoaded = false;
                var resourceFileName = scriptUrl + '_resources.' + this.Language.substring(0, 2).toLowerCase() + '.js';

                jQuery.ajax({
                    dataType: "script",
                    cache: true,
                    url: resourceFileName
                }).done(function () {
                    CAMControl.resourceLoaded = true;
                }).fail(function () {
                    alert('Could not load the resource file ' + resourceFileName);
                });
            }

            //create a new wrapper for the control using a div
            this._control = $('<div class="cam-taxpicker"></div>');

            //detach the hidden field from the parent and append to the wrapper
            var parent = this._hiddenValidated.parent();
            this._hiddenValidated = this._hiddenValidated.detach();
            parent.append(this._control);
            this._suggestionContainer = $('<div class="cam-taxpicker-suggestion-container"></div>');
            this._dlgButton = $('<div class="cam-taxpicker-button"></div>');
            if (!this._isReadOnly) {
                this._editor = $('<div class="cam-taxpicker-editor" contenteditable="true"></div>');
                this._control.empty().append(this._editor).append(this._dlgButton).append(this._hiddenValidated);
                this._control.after(this._suggestionContainer);
            }
            else {

                this._editor = $('<div class="cam-taxpicker-editor-readonly" contenteditable="false"></div>');
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
                this._editor.html(this.selectedTermsToHtml());
            }

            //wire up control events
            this._dlgButton.click(Function.createDelegate(this, this.showPickerDialog)); //dialog button is clicked
            this._editor.keydown(Function.createDelegate(this, this.keydown)); //key is pressed in the editor control
            $(document).mousedown(Function.createDelegate(this, this.checkExternalClick)); //mousedown somewhere in the document
        },
        //handle reset
        reset: function () {
            this._selectedTerms = new Array();
            this._editor.html('');
        },
        //handle keydown event in editor control
        keydown: function (event, args) {
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

                //get text validation and set html in editor
                var textValidation = this.validateText(newText);
                this._editor.html(textValidation.html);

                //set the cursor position at the marker
                this.setCaret();

                //show suggestions
                this.showSuggestions(textValidation, caret);
                return false;
            }
            else if (keynum == 9 || keynum == 13) { //Tab key pressed and also validate on enter
                // support for selecting a suggestion
                var sel = this._suggestionContainer.children('.cam-taxpicker-suggestion-item.selected');
                if (sel.length > 0) {
                    this._editor.blur();
                    sel.click();
                    return false;
                }

                //validate raw text OR mark invalid
                var textValidation = this.validateText(rawText);
                var html = this.markInvalidTerms(textValidation);
                this._editor.html(html);

                //close the suggestion panel
                this._suggestionContainer.hide();

                if (keynum == 13) { // also validate on enter, we need to cancel the enter and blur
                    this._editor.blur();
                    return false;
                }
            }
            else if (keynum == 38 || keynum == 40) { // selecting suggestion with Up or Down key
                if (this._suggestionContainer.css('display') != 'none') {
                    var sel = this._suggestionContainer.children('.cam-taxpicker-suggestion-item.selected');
                    if (sel.length == 0) {
                        sel = this._suggestionContainer.children('.cam-taxpicker-suggestion-item').first();
                        sel.addClass('selected');
                    }
                    else {
                        sel.removeClass('selected');
                        if (keynum == 38) {
                            sel = sel.prev();
                            if (sel.attr('data-item') == null)
                                sel = this._suggestionContainer.children('.cam-taxpicker-suggestion-item').last();
                        }
                        else {
                            sel = sel.next();
                            if (sel.length == 0)
                                sel = this._suggestionContainer.children('.cam-taxpicker-suggestion-item').first();
                        }
                        sel.addClass('selected');
                    }
                }
            }
        },
        //get the cursor position in a content editable div
        getCaret: function (target) {
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
        },
        //sets the cursor caret (position in the editor control)
        setCaret: function () {
            //find the marker
            var marker = null;
            // getting the marker in more reliably
            var jQmarker = this._editor.find('span#caretmarker');
            if (jQmarker.length > 0)
                marker = jQmarker.get(0);

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
        },
        //validates the text input into ranges and html output
        validateText: function (txt) {
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

            return textValidation;
        },
        //marks text input with valid and invalid markup
        markInvalidTerms: function (textValidation) {
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
                    else
                        html += '<span class="cam-taxpicker-term-invalid">' + textValidation.ranges[i].text + '</span>';

                    //check for ambiguous matches
                    if (matches.length > 1) {
                        //TODO: popup anbiguous terms dialog (Enhancement)
                    }
                }

                if (i < textValidation.ranges.length - 1)
                    html += '<span>;&nbsp;</span>';
            }
            return html;
        },
        //shows suggestions based on the unvalidated text being entered
        showSuggestions: function (textValidation, caret) {
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

                    this._suggestionContainer.empty().append($('<div class="cam-taxpicker-suggestion-title">' + TaxonomyPickerConsts.SUGGESTIONS_HEADER + '</div>'));
                    if (suggestions.length > 0) {
                        $(suggestions).each(Function.createDelegate(this, function (i, e) {
                            if (i < this._maxSuggestions) {
                                var match = e.Name.substring(0, txt.length); //get the matched text so we can highlight it
                                var labels = e.RawTerm.get_labels().getEnumerator();
                                var labelStr = "";
                                while (labels.moveNext()) {
                                    var label = labels.get_current();
                                    if (!label.get_isDefaultForLanguage()) {
                                        labelStr += "," + label.get_value();
                                    }
                                }
                                var itemHtml = $('<div class="cam-taxpicker-suggestion-item" data-item="' + e.Id + '">' + e.Name.replace(match, '<span style="background-color: yellow;">' + match + '</span>') + ' [' + this.TermSet.Name + ':' + e.PathOfTerm.replace(/;/g, ':') + labelStr + ']</div>');
                                this._suggestionContainer.append(itemHtml);
                                itemHtml.click(Function.createDelegate(this, this.suggestionClicked));
                            }
                        }));
                    }
                    this._suggestionContainer.show();
                }
            }
            else
                this._suggestionContainer.hide();
        },
        //term node add is canceled
        termNodeAddCancel: function (event) {
            //remove the expand/collapse image if needed
            if (this._dlgCurrTermNode != null && this._dlgCurrTermNode.parent().next().children().length <= 1) {
                this._dlgCurrTermNode.prev().prev().removeClass('collapsed');
                this._dlgCurrTermNode.prev().prev().removeClass('expanded');
            }

            //cancel the add by removing the new term
            if (this._dlgNewNode != null)
                this._dlgNewNode.remove();
        },
        //term node is clicked in the treeview
        termNodeClicked: function (event) {
            //clear any term that was in the middle of an add
            this.termNodeAddCancel(event);

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
        },
        //term node is double clicked in the treeview
        termNodeDoubleClicked: function (event) {
            //clear any term that was in the middle of an add
            this.termNodeAddCancel(event);

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
        },
        //dialog select button is clicked...use this to add any selected node as a selected term
        dialogSelectButtonClicked: function (event) {
            if (this._dlgCurrTerm != null) {
                //add the term to selected terms array
                this.pushSelectedTerm(this._dlgCurrTerm);

                //refresh the html in the editor control
                this._dlgEditor.html(this.selectedTermsToHtml());
            }
        },
        //dialog OK button clicked
        dialogOkClicked: function (event) {
            //update the control value
            this._editor.html(this.selectedTermsToHtml());

            //close the dialog
            this.closePickerDialog(event);

            if (this._changeCallback != null)
                this._changeCallback();
        },
        //dialog Cancel button clicked
        dialogCancelClicked: function (event) {
            //reset the selected terms
            this._selectedTerms = this._tempSelectedTerms;

            //close the dialog
            this.closePickerDialog(event);
        },
        //dialog new term button is clicked
        dialogNewTermClicked: function (event) {
            if ($('.cam-taxpicker-treenode-newnode').length > 0) // don't allow adding multiple nodes at once
                return;

            this._dlgNewNodeEditor = $('<div class="cam-taxpicker-treenode-newnode" style="min-width: 100px;" contenteditable="true"></div>');
            this._dlgNewNode = $('<li class="cam-taxpicker-treenode-li newNode"></li>').append($('<div class="cam-taxpicker-treenode"></div>').append('<div class="cam-taxpicker-expander"></div><img src="../styles/images/EMMTerm.png" alt=""/>').append(this._dlgNewNodeEditor));

            // only one level allowed for keywords, so always add to the root
            if (this.TermSet.UseKeywords || this._dlgCurrTerm == null) {
                $('.cam-taxpicker-treenode-title').removeClass('selected');
                var root = $('.cam-taxpicker-treenode-title.root').first();
                root.addClass('selected');
                this._dlgCurrTermNode = root;
                this._dlgCurrTerm = null;
            }

            //get the container for the new node
            var ul = this._dlgCurrTermNode.parent().next();
            if (ul.length == 0) { // adding a term to a newly added term
                ul = $('<ul class="cam-taxpicker-treenode-ul"></ul>').appendTo(this._dlgCurrTermNode.parent().parent());
            }
            ul.prepend(this._dlgNewNode);

            //toggle the expand on the parent node
            ul.show();
            this._dlgCurrTermNode.prev().prev().removeClass('collapsed');
            this._dlgCurrTermNode.prev().prev().addClass('expanded');

            //set focus on the newNode editor and wire events
            this._dlgNewNodeEditor.focus();
            this._dlgNewNodeEditor.keydown(Function.createDelegate(this, this.dialogNewTermKeydown));
        },
        //fires for each keydown in the new term editor
        dialogNewTermKeydown: function (event) {
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
        },
        //successful callback from creating a new term
        termAddSuccess: function (event, args) {
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
            this._dlgCurrTermNode = title; // title node as current node
            this._dlgCurrTerm = newTerm;
        },
        //failed callback from trying to create a new term
        termAddFailed: function (event, args) {
            //remove the expand/collapse image if needed
            if (this._dlgCurrTermNode.parent().next().children().length <= 1) {
                this._dlgCurrTermNode.prev().prev().removeClass('collapsed');
                this._dlgCurrTermNode.prev().prev().removeClass('expanded');
            }

            //cancel the add by removing the new term
            this._dlgNewNode.remove();
        },
        //fires when a user selected a suggested term in the suggestions list
        suggestionClicked: function (event) {
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

            //refresh the html in the editor control
            this._editor.html(this.selectedTermsToHtml());
            this._suggestionContainer.hide();
            this._editor.focus();

            if (this._changeCallback != null)
                this._changeCallback();
        },
        //used to check if focus is lost from the control (invalidate and hide suggestions)
        checkExternalClick: function (event) {
            //check if the target is outside the picker
            if (!$.contains(this._control[0], event.target) && !$.contains(this._suggestionContainer[0], event.target) && this._dialog != null && !$.contains(this._dialog[0], event.target)) {
                var rawText = this._editor.text(); //the raw text in the editor (html stripped out)
                var textValidation = this.validateText(rawText); //get the text validation
                var html = this.markInvalidTerms(textValidation); //mark invalid terms
                this._editor.html(html); //set the editor
                this._suggestionContainer.hide(); //hide suggestions
            }
        },
        //show the dialog picker
        showPickerDialog: function (event) {
            //check to make sure the termset has loaded
            if (!this.TermSet.TermsLoaded) {
                this.TermSet.OnTermsLoaded = Function.createDelegate(this, this.showPickerDialog);

                //add the waiting indicator to the body
                this._waitingDlg = $('<div class="cam-taxpicker-waiting"><div class="cam-taxpicker-waiting-overlay"></div><div class="cam-taxpicker-waiting-dlg"><div class="cam-taxpicker-waiting-dlg-inner"><img alt="" src="data:image/gif;base64,R0lGODlhEAAQAIAAAFLOQv///yH/C05FVFNDQVBFMi4wAwEAAAAh+QQFCgABACwJAAIAAgACAAACAoRRACH5BAUKAAEALAwABQACAAIAAAIChFEAIfkEBQoAAQAsDAAJAAIAAgAAAgKEUQAh+QQFCgABACwJAAwAAgACAAACAoRRACH5BAUKAAEALAUADAACAAIAAAIChFEAIfkEBQoAAQAsAgAJAAIAAgAAAgKEUQAh+QQFCgABACwCAAUAAgACAAACAoRRACH5BAkKAAEALAIAAgAMAAwAAAINjAFne8kPo5y02ouzLQAh+QQJCgABACwCAAIADAAMAAACF4wBphvID1uCyNEZM7Ov4v1p0hGOZlAAACH5BAkKAAEALAIAAgAMAAwAAAIUjAGmG8gPW4qS2rscRPp1rH3H1BUAIfkECQoAAQAsAgACAAkADAAAAhGMAaaX64peiLJa6rCVFHdQAAAh+QQJCgABACwCAAIABQAMAAACDYwBFqiX3mJjUM63QAEAIfkECQoAAQAsAgACAAUACQAAAgqMARaol95iY9AUACH5BAkKAAEALAIAAgAFAAUAAAIHjAEWqJeuCgAh+QQJCgABACwFAAIAAgACAAACAoRRADs=" style="width: 32px; height: 32px;" /><span class="ms-accentText" style="font-size: 36px;">' + TaxonomyPickerConsts.WORKING_ON_IT + '</span></div></div></div>');
                $('body').append(this._waitingDlg);
            }
            else {
                //remove the waiting indicator (if exists)
                $('body').children('.cam-taxpicker-waiting').remove();

                var termListing;

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
                    var dlgSubheader = $('<div class="cam-taxpicker-dialog-content-subheader"><img class="cam-taxpicker-dialog-content-subheader-img" src="../styles/images/EMMDoubleTag.png" alt="" /></div>');
                    this._dlgAddNewTermButton = $('<a style="cursor: pointer;">' + TaxonomyPickerConsts.DIALOG_ADD_LINK + '</a>');
                    if (this.TermSet.IsOpenForTermCreation) {
                        dlgSubheader.append($('<div class="cam-taxpicker-dialog-content-subheader-title">' + TaxonomyPickerConsts.DIALOG_ADD_TITLE + '&nbsp;</div>'));
                        dlgSubheader.append($('<div class="cam-taxpicker-dialog-content-subheader-addnew"></div>').append(this._dlgAddNewTermButton));
                    }
                    var dlgBody = $('<div class="cam-taxpicker-dialog-content-body"></div>');
                    dlgBody.append(dlgSubheader);
                    var dlgBodyContainer = $('<div class="cam-taxpicker-dialog-tree-container"></div>');

                    //build the termset hierarchy
                    dlgBodyContainer.append($('<ul id="rootNode" class="cam-taxpicker-treenode-ul root" style="height: 100%;"></ul>'));

                    //build the dialog editor area
                    //TODO: convert the dlgEditor with contenteditable="true" just like the main editor (Enhancement)
                    this._dlgEditor = $('<div class="cam-taxpicker-dialog-selection-editor" RestrictPasteToText="true" AllowMultiLines="false"></div>');
                    this._dlgSelectButton = $('<button>' + TaxonomyPickerConsts.BUTTON_TEXT + ' >></button>');
                    dlgBody.append(dlgBodyContainer).append($('<div class="cam-taxpicker-dialog-selection-container"></div>').append(this._dlgSelectButton).append(this._dlgEditor));
                    dlg.append(dlgBody);
                    this._dialog.empty().append($('<div class="cam-taxpicker-dialog-overlay"></div>')).append(dlg);

                    //add button area
                    var dlgButtonArea = $('<div class="cam-taxpicker-dialog-button-container"></div>')
                    this._dlgOkButton = $('<button style="float: right;">Ok</button>');
                    this._dlgCancelButton = $('<button style="float: right;">Cancel</button>');
                    dlgBody.append(dlgButtonArea.append(this._dlgCancelButton).append(this._dlgOkButton));

                }

                //set the value in the dialogs editor field
                this._dlgEditor.html(this.selectedTermsToHtml());

                //add the dialog to the body
                $('body').append(this._dialog);

                var termName = this.TermSet.Name;

                var that = this;

                var outHtml = buildTermSetTreeLevel(this.TermSet.Terms, true, "", function (html) {
                    document.getElementById('rootNode').innerHTML =
                                       '<li class="cam-taxpicker-treenode-li">' +
                                           '<div class="cam-taxpicker-treenode">' +
                                               '<div class="cam-taxpicker-expander expanded">' + '</div>' +
                                               '<img src="../styles/images/EMMTermSet.png" alt=""/>' +
                                               '<span id="currNode" class="cam-taxpicker-treenode-title root selected">' + termName + '</span>' +
                                            '</div>' +
                                            '<ul class="cam-taxpicker-treenode-ul" style="display: block;">' +
                                               html +
                                            '</ul>' +
                                       '</li>' +
                                    '</ul>' +
                                '</div>';

                    that._dlgCurrTermNode = $("#currNode");
                });


                //wire events all the dialog events
                $('.cam-taxpicker-expander').click(function () {
                    //toggle tree node
                    if ($(this).hasClass('expanded')) {
                        $(this).removeClass('expanded');
                        $(this).addClass('collapsed');
                        $(this).parent().next().hide();
                    }
                    else if ($(this).hasClass('collapsed')) {
                        $(this).removeClass('collapsed');
                        $(this).addClass('expanded');
                        $(this).parent().next().show();
                    }
                });

                $('.cam-taxpicker-treenode-title').click(Function.createDelegate(this, this.termNodeClicked));
                $('.cam-taxpicker-treenode-title').dblclick(Function.createDelegate(this, this.termNodeDoubleClicked));
                this._dlgSelectButton.click(Function.createDelegate(this, this.dialogSelectButtonClicked));
                this._dlgCloseButton.click(Function.createDelegate(this, this.dialogCancelClicked));
                this._dlgOkButton.click(Function.createDelegate(this, this.dialogOkClicked));
                this._dlgCancelButton.click(Function.createDelegate(this, this.dialogCancelClicked));
                this._dlgAddNewTermButton.click(Function.createDelegate(this, this.dialogNewTermClicked));
            }
        },
        //closes the picker dialog
        closePickerDialog: function (event) {
            //remove the picker dialog from the body
            $('body').children('.cam-taxpicker-dialog').remove();
        },
        //adds a new term to the end of this._selectedTerms
        pushSelectedTerm: function (term) {
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
        },
        //if the term already exists in the selected terms then don't add it
        existingTerm: function (term) {
            for (var j = 0; j < this._selectedTerms.length; j++) {
                if (this._selectedTerms[j].Id == term.Id) {
                    return true;
                }
            }
            return false;
        },
        //removes the last term from this._selectedTerms
        popSelectedTerm: function () {
            //remove the last selected term
            this._selectedTerms.pop();
            this._hiddenValidated.val(JSON.stringify(this._selectedTerms));
        },
        //converts this._selectedTerms to html for an editor field
        selectedTermsToHtml: function () {
            var termsHtml = '';
            for (var i = 0; i < this._selectedTerms.length; i++) {
                var e = this._selectedTerms[i];
                termsHtml += '<span class="cam-taxpicker-term-selected">' + e.Name + '</span><span>;&nbsp;</span>';
            }
            return termsHtml;
        },
        //trim suggestions based on existing selections
        trimSuggestions: function (suggestions) {
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
        }
    });
    //********************** END TaxonomyPicker Class **********************

    //called recursively to build a treeview of terms for a termset
    function buildTermSetTreeLevel(termList, show, outHtml, cb) {

        var addlStyle = (show) ? 'style="display: block;"' : '';

        var defs = [];

        for (var i = 0, len = termList.length; i < len; i++) {
            var term = termList[i];
            var deferred = $.Deferred();
            defs.push(deferred);

            var addlClass = (term.Children.length > 0) ? 'collapsed' : '';
            var tHtml = "";
            tHtml += '<li class="cam-taxpicker-treenode-li">' +
                         '<div class="cam-taxpicker-treenode">' +
                             '<div class="cam-taxpicker-expander ' + addlClass + '">' +
                             '</div>' +
                             '<img src="../styles/images/EMMTerm.png" alt=""/>' +
                             '<span class="cam-taxpicker-treenode-title"  data-item="' + term.Name + '|' + term.Id + '">' + term.Name + '</span>' +
                         '</div>';

            //add children if they exist
            if (term.Children.length > 0) {
                buildTermSetTreeLevel(term.Children, false, "", function (html) {
                    tHtml += '<ul class="cam-taxpicker-treenode-ul">' + html + "</ul></li>";
                });
            }
            else {
                //TODO We should not add these nodes here. Adds to much overhead on large termsets.
                //These could be created at inserttime as I don't see any case where
                //we have several parents in the containing div. It should be as the commented line below
                //tHtml += '</li>'
                tHtml += '<ul class="cam-taxpicker-treenode-ul"></ul></li>';
            }

            outHtml += tHtml;
            deferred.resolve();
        }

        $.when($, defs).done(function () {
            if (cb) {
                cb(outHtml)
            }
        });
    }

    //called recursively to build hierarchical representation of terms in a termset
    function getTerms(term, termEnumerator) {
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
    }

    //creates a new guid
    function newGuid() {
        var result, i, j;
        result = '';
        for (j = 0; j < 32; j++) {
            if (j == 8 || j == 12 || j == 16 || j == 20)
                result = result + '-';
            i = Math.floor(Math.random() * 16).toString(16).toUpperCase();
            result = result + i;
        }
        return result
    }

    //extends jquery to support taxpicker function
    $.fn.taxpicker = function (options, ctx, changeCallback) {
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
        $.taxpicker[taxIndex] = new TaxonomyPicker(this, options, ctx, changeCallback);
    };
})(CAMControl || (CAMControl = {}));
