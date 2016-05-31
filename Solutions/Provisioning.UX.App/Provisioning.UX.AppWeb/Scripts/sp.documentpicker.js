(function ($) {

    $.fn.spDocumentPicker = function (options) {

        var appSite = null;

        var methods = {

            init: function () {

                var container = $(this);

                $app.withSPContext(function (spContext) {

                    appSite = new SP.AppContextSite(spContext, settings.hostUrl);

                    container.append('<div class="dropdown input-group sp-documentpicker-wrapper"><input type="hidden" value="[]" /><div class="form-control sp-documentpicker-input"></div><span class="input-group-addon" data-toggle="dropdown"><span class="glyphicon glyphicon-file"></span></span><ul class="dropdown-menu sp-docpicker-dropdown"><li sp-docpicker-nodeType="info">Loading...</li></ul></div>');

                    // we delegate any clicks inside the ul to the root ul node. We will use attributes to establish context
                    container.on('click', 'li[sp-docpicker-nodeType]', methods.dropdownChildClick);

                    // we delegate any command clicks inside the container to the command handler
                    container.on('click', '[sp-docpicker-cmd]', methods.onCommandHandler);

                    // we need to handle the remove clicks
                    container.on('click', 'span.spdocpicker-removeChosen', methods.removeChosenDoc);

                    // we delegate any clicks inside the ul to the root ul node. We will use attributes to establish context
                    container.on('dblclick', 'li[sp-docpicker-nodeType="file"]', methods.onFileDoubleClick);

                    // here we need to load the first node for the collection (so we will load the site, root web, root web libraries, and the immediate child webs)
                    var site = appSite.get_site();
                    var rootWeb = site.get_rootWeb();
                    var childWebs = rootWeb.get_webs();
                    var libraries = rootWeb.get_lists();

                    // add our load requests to the client call
                    spContext.load(rootWeb, 'Title', 'Id');
                    spContext.load(childWebs, 'Include(Title, Id)');
                    spContext.load(libraries, 'Include(Title, Id, BaseTemplate, Hidden)');

                    var ctx = {
                        container: container,
                        site: site,
                        rootWeb: rootWeb,
                        childWebs: childWebs,
                        libraries: libraries
                    };

                    spContext.executeQueryAsync($app.getCtxCallback(ctx, methods.initLoadSuccess), $app.getCtxCallback(ctx, methods.initLoadFail));
                });

                return container;
            },

            initLoadSuccess: function () {
                var ctx = this;
                var controlContext = methods.getControlContext($(ctx.container));

                var rootwebNode = $('<li sp-docpicker-nodeType="web" sp-docpicker-nodeId="' + ctx.rootWeb.get_id() + '" class="sp-docpicker-nocollapse">' + ctx.rootWeb.get_title() + '</li>');

                var innerList = $('<ul class="sp-documentpicker-childul"></ul>');

                // add child webs
                methods.innerAddWebs(ctx.childWebs.getEnumerator(), innerList);

                // add lists
                methods.innerAddLists(ctx.libraries.getEnumerator(), innerList);

                // add the inner list to the root web node
                rootwebNode.append(innerList);
                controlContext.dropdown.empty();
                controlContext.dropdown.append(rootwebNode);

                // now we append our buttons to the root ul node
                controlContext.dropdown.append('<li class="divider"></li>');
                controlContext.dropdown.append('<li><button class="btn btn-primary btn-xs pull-right" sp-docpicker-cmd="select">Select</button</li>');
            },

            initLoadFail: function () {
                var ctx = this;
                var controlContext = methods.getControlContext($(ctx.container));
                controlContext.dropdown.empty();
                controlContext.dropdown.append('<li sp-docpicker-nodeType="error">Error loading libraries from site...</li>');
            },

            dropdownChildClick: function (e) {
                // keep the event here.
                e.preventDefault();
                e.stopPropagation();

                // start with what was clicked
                var source = $(this);

                var nodeType = source.attr('sp-docpicker-nodeType');

                // handle file nodes seperately
                if (nodeType.toLowerCase() == 'file') {
                    methods.selectFile(source);
                    return;
                }

                // we have already processed this node, it is either loading or loaded, either way we just toggle it.
                if (source.is('[sp-docpicker-nodeStatus]')) {
                    methods.toggleNode(source);
                    return;
                }

                // set our tracking status to loading so we won't try and double load it
                source.attr('sp-docpicker-nodeStatus', 'loading');

                switch (nodeType.toLowerCase()) {
                    case 'web':
                        methods.fillWebContainer(source);
                        break;
                    case 'list':
                        methods.fillLibraryContainer(source);
                        break;
                    case 'folder':
                        methods.fillFolderContainer(source);
                        break;
                }
            },

            fillWebContainer: function (/*jQuery*/ node) {

                var webId = node.attr('sp-docpicker-nodeId');

                node.append('<ul class="sp-documentpicker-childul"><li sp-docpicker-nodeType="info">Loading...</li></ul>');

                $app.withSPContext(function (spContext) {

                    var site = appSite.get_site();
                    var web = site.openWebById(webId);
                    var childWebs = web.get_webs();
                    var libraries = web.get_lists();

                    spContext.load(web, 'Title', 'Id');
                    spContext.load(childWebs, 'Include(Id, Title)');
                    spContext.load(libraries, 'Include(Id, Title, BaseTemplate, Hidden)');

                    var ctx = {
                        node: node,
                        webId: webId,
                        web: web,
                        childWebs: childWebs,
                        libraries: libraries
                    };

                    spContext.executeQueryAsync($app.getCtxCallback(ctx, methods.fillWebSuccess), $app.getCtxCallback(ctx, methods.fillWebFail));
                });
            },

            fillWebSuccess: function () {
                var ctx = this;
                var target = $(ctx.node);

                target.attr('sp-docpicker-nodeStatus', 'loaded');

                target.find('span.glyphicon').removeClass('glyphicon-chevron-right').addClass('glyphicon-chevron-down');
                target.children('ul').remove();

                var innerList = $('<ul class="sp-documentpicker-childul"></ul>');

                // add child webs
                methods.innerAddWebs(ctx.childWebs.getEnumerator(), innerList);

                // add lists
                methods.innerAddLists(ctx.libraries.getEnumerator(), innerList);

                if (innerList.children().length > 0) {
                    target.append(innerList);
                }
                else {
                    target.append(innerList.append('<li sp-docpicker-nodeType="info">(nothing)</li>'));
                }
            },

            fillWebFail: function () {
                console.log('fail web');
            },

            fillLibraryContainer: function (/*jQuery*/ node) {
                // we should be inside a web node, so we find the closest one up the tree
                var webId = node.closest('li[sp-docpicker-nodeType="web"]').attr('sp-docpicker-nodeId');
                var libraryId = node.attr('sp-docpicker-nodeId');

                node.append('<ul class="sp-documentpicker-childul"><li sp-docpicker-nodeType="info">Loading...</li></ul>');

                $app.withSPContext(function (spContext) {

                    var site = appSite.get_site();
                    var web = site.openWebById(webId);

                    var list = web.get_lists().getById(new SP.Guid('{' + libraryId + '}'));
                    var rootFolder = list.get_rootFolder();
                    var files = rootFolder.get_files();
                    var subFolders = rootFolder.get_folders();

                    spContext.load(subFolders);
                    spContext.load(files, 'Include(Name, ServerRelativeUrl)');

                    var ctx = {
                        node: node,
                        webId: webId,
                        libraryId: libraryId,
                        subFolders: subFolders,
                        files: files
                    };

                    spContext.executeQueryAsync($app.getCtxCallback(ctx, methods.fillLibrarySuccess), $app.getCtxCallback(ctx, methods.fillLibraryFail));
                });
            },

            fillLibrarySuccess: function () {
                var ctx = this;

                var target = $(ctx.node);
                target.attr('sp-docpicker-nodeStatus', 'loaded');
                target.children('ul').remove();

                var innerList = $('<ul class="sp-documentpicker-childul"></ul>');

                // now we append folders -> folders are clickable
                methods.innerAddFolders(ctx.subFolders.getEnumerator(), innerList);

                // now we append files -> files are selectable
                methods.innerAddFiles(ctx.files.getEnumerator(), innerList);

                if (innerList.children().length > 0) {
                    target.append(innerList);
                }
                else {
                    target.append(innerList.append('<li sp-docpicker-nodeType="info">(nothing)</li>'));
                }
            },

            fillLibraryFail: function () {
                console.log('library fail');
            },

            fillFolderContainer: function (/*jQuery*/ node) {
                // we should be inside a web node, so we find the closest one up the tree
                var webId = node.closest('li[sp-docpicker-nodeType="web"]').attr('sp-docpicker-nodeId');
                var folderPath = node.attr('sp-docpicker-nodeId');

                node.append('<ul class="sp-documentpicker-childul"><li sp-docpicker-nodeType="info">Loading...</li></ul>');

                $app.withSPContext(function (spContext) {

                    var site = appSite.get_site();
                    var web = site.openWebById(webId);
                    var sourceFolder = web.getFolderByServerRelativeUrl(folderPath);
                    var files = sourceFolder.get_files();
                    var subFolders = sourceFolder.get_folders();

                    spContext.load(subFolders);
                    spContext.load(files, 'Include(Name, ServerRelativeUrl)');

                    var ctx = {
                        node: node,
                        webId: webId,
                        folderPath: folderPath,
                        subFolders: subFolders,
                        files: files
                    };

                    spContext.executeQueryAsync($app.getCtxCallback(ctx, methods.fillFolderSuccess), $app.getCtxCallback(ctx, methods.fillFolderFail));
                });
            },

            fillFolderSuccess: function () {
                var ctx = this;

                var target = $(ctx.node);
                target.attr('sp-docpicker-nodeStatus', 'loaded');
                target.children('ul').remove();
                target.find('span.glyphicon').removeClass('glyphicon-folder-close').addClass('glyphicon-folder-open');

                var innerList = $('<ul class="sp-documentpicker-childul"></ul>');

                // now we append folders -> folders are clickable
                methods.innerAddFolders(ctx.subFolders.getEnumerator(), innerList);

                // now we append files -> files are selectable
                methods.innerAddFiles(ctx.files.getEnumerator(), innerList);

                if (innerList.children().length > 0) {
                    target.append(innerList);
                }
                else {
                    target.append(innerList.append('<li sp-docpicker-nodeType="info">(nothing)</li>'));
                }
            },

            fillFolderFail: function () {
                console.log('fill folder fail');
            },

            selectFile: function (/* jQuery */ source) {
                if (source.hasClass('selected')) {
                    source.removeClass('selected')
                } else {
                    if (!settings.allowMultiple && methods.getSelectedCount(source) > 0) {
                        alert('This control is currently configured to allow only one selection. Please selected fewer documents, or remove a previously selected item.');
                        return;
                    }
                    else {
                        source.addClass('selected')
                    }
                }
            },

            toggleNode: function (node) {

                if (node.hasClass('sp-docpicker-nocollapse')) {
                    return;
                }

                var glyph = node.children('span.glyphicon');

                if (glyph.hasClass('glyphicon-chevron-right')) {
                    glyph.removeClass('glyphicon-chevron-right').addClass('glyphicon-chevron-down');
                }
                else if (glyph.hasClass('glyphicon-chevron-down')) {
                    glyph.removeClass('glyphicon-chevron-down').addClass('glyphicon-chevron-right');
                }

                if (glyph.hasClass('glyphicon-folder-open')) {
                    glyph.removeClass('glyphicon-folder-open').addClass('glyphicon-folder-close');
                }
                else if (glyph.hasClass('glyphicon-folder-close')) {
                    glyph.removeClass('glyphicon-folder-close').addClass('glyphicon-folder-open');
                }

                node.children('ul').toggle();
            },

            onCommandHandler: function (e) {
                e.preventDefault();
                e.stopPropagation();

                var source = $(this);

                var cmd = source.attr('sp-docpicker-cmd');
                switch (cmd.toLowerCase()) {
                    case 'select':
                        methods.selectDocs(source);
                        break;
                }
            },

            onFileDoubleClick: function (e) {
                e.preventDefault();
                e.stopPropagation();

                var source = $(this);
                var controlContext = methods.getControlContext(source);
                methods.innerSelectDoc(controlContext, source);
            },

            selectDocs: function (/*jQuery*/ source) {

                var controlContext = methods.getControlContext(source);

                controlContext.dropdown.find('li[sp-docpicker-nodeType="file"].selected').each(function () {
                    methods.innerSelectDoc(controlContext, $(this));
                });

                // clear our previous entries and results                
                controlContext.dropdown.find('li[sp-docpicker-nodeType="file"].selected').removeClass('selected');
            },

            innerSelectDoc: function (/* control context*/ controlContext, /*file li*/ li) {

                // get a data object from the choice
                var chosen = { path: li.attr('sp-docpicker-nodeId'), name: li.text() };

                // update our hidden field value
                var arr = methods.parseValue(controlContext.hiddenInput);
                arr.push(chosen);
                methods.setSelectedDocs(controlContext, arr);
            },

            getSelectedCount: function (/*jQuery*/ ctrlInContainer) {
                var controlContext = methods.getControlContext(ctrlInContainer);
                // account for the items already selected
                return controlContext.dropdown.find('li[sp-docpicker-nodeType="file"].selected').length + controlContext.displayInput.children().length;
            },

            setSelectedDocs: function (/*controlContext*/ controlContext, /*userInfo[]*/ arr) {

                if (arr === null || arr === '' || arr == '' || !$.isArray(arr)) {
                    // count this as an attempt to clear the control
                    methods.emptyControl(controlContext);
                    return;
                }

                // set the hidden input's value with our JSON array
                controlContext.hiddenInput.val(JSON.stringify(arr));

                // update our display of found users     
                controlContext.displayInput.empty();

                for (var i = 0; i < arr.length; i++) {
                    var chosen = arr[i];
                    controlContext.displayInput.append('<span class="sp-docpicker-chosen" sp-path="' + chosen.path + '">' + chosen.name + '<span class="spdocpicker-removeChosen"><span class="glyphicon glyphicon-remove"></span></span></span>');
                }
            },

            removeChosenDoc: function (e) {
                e.preventDefault();
                e.stopPropagation();

                var source = $(this);
                var controlContext = methods.getControlContext(source);
                var chosen = source.closest('span.sp-docpicker-chosen');

                var chosenToRemovePath = chosen.attr('sp-path');

                var arr = methods.parseValue(controlContext.hiddenInput);

                var newArr = [];

                for (var i = 0; i < arr.length; i++) {
                    if (arr[i].path != chosenToRemovePath) {
                        newArr.push(arr[i]);
                    }
                }

                // update our hidden field value
                controlContext.hiddenInput.val(JSON.stringify(newArr));

                // now remove the DOM element
                chosen.remove();
            },

            parseValue: function (hiddenInput) {
                var rawValue = hiddenInput.val();
                return rawValue === '' || rawValue == null ? [] : eval(rawValue);
            },

            getControlContext: function (/*jQuery*/ elementInContainer) {

                var container = elementInContainer.hasClass('sp-documentpicker-wrapper') ? elementInContainer : elementInContainer.closest('div.sp-documentpicker-wrapper');

                if (container.length < 1) {
                    container = elementInContainer.find('div.sp-documentpicker-wrapper');
                }

                return {
                    container: container,
                    hiddenInput: container.find('input[type="hidden"]'),
                    displayInput: container.find('div.sp-documentpicker-input'),
                    dropdown: container.find('ul.sp-docpicker-dropdown')
                };
            },

            emptyControl: function (controlContext) {
                controlContext.hiddenInput.val('');
                controlContext.displayInput.empty();
            },

            defaultNodeFilter: function (node) {

                // we filter out the "forms" library
                if (node.is('[sp-docpicker-nodeType="folder"]') && /^forms/i.test(node.text())) {
                    return false;
                }

                // we don't show sub-sites if that is turned off
                if (!settings.showSubSites && node.is('[sp-docpicker-nodeType="web"]')) {
                    return false;
                }

                // if we have a filter from properties we use it, otherwise return true
                if ($.isFunction(settings.nodeFilter)) {
                    return settings.nodeFilter(node);
                }

                return true;
            },

            innerAddWebs: function (/*enumerator*/ webs, /*jquery*/ targetTag) {
                while (webs.moveNext()) {
                    var web = webs.get_current();
                    var node = $('<li sp-docpicker-nodeType="web" sp-docpicker-nodeId="' + web.get_id() + '"><span class="glyphicon glyphicon-chevron-right"></span>' + web.get_title() + '</li>');
                    if (settings.__nodeFilter(node)) {
                        targetTag.append(node);
                    }
                }
            },

            innerAddLists: function (/*enumerator*/ lists, /*jquery*/ targetTag) {
                while (lists.moveNext()) {
                    var list = lists.get_current();
                    if (list.get_baseTemplate() == '101' && !list.get_hidden()) {
                        var node = $('<li sp-docpicker-nodeType="list" sp-docpicker-nodeId="' + list.get_id() + '"><span class="glyphicon glyphicon-th-list"></span>' + list.get_title() + '</li>');
                        if (settings.__nodeFilter(node)) {
                            targetTag.append(node);
                        }
                    }
                }
            },

            innerAddFolders: function (/*enumerator*/ folders, /*jquery*/ targetTag) {
                while (folders.moveNext()) {
                    var folder = folders.get_current();
                    var node = $('<li sp-docpicker-nodeType="folder" sp-docpicker-nodeId="' + folder.get_serverRelativeUrl() + '"><span class="glyphicon glyphicon-folder-close"></span>' + folder.get_name() + '<span class="badge">' + folder.get_itemCount() + '</span></li>');
                    if (settings.__nodeFilter(node)) {
                        targetTag.append(node);
                    }
                }
            },

            innerAddFiles: function (/*enumerator*/ files, /*jquery*/ targetTag) {
                while (files.moveNext()) {
                    var file = files.get_current();
                    var node = $('<li sp-docpicker-nodeType="file" sp-docpicker-nodeId="' + file.get_serverRelativeUrl() + '"><span class="glyphicon glyphicon-file"></span>' + file.get_name() + '</li>');
                    if (settings.__nodeFilter(node)) {
                        targetTag.append(node);
                    }
                }
            }
        }

        // process our settings
        var settings = $.extend({
            hostUrl: $app.getUrlParamByName('SPHostUrl'),
            onLoaded: null,
            allowMultiple: false,
            showSubSites: false,
            __nodeFilter: methods.defaultNodeFilter,
            nodeFilter: null
        }, options);

        // now we handle our command options
        if (options === 'clear') {

            // clear any matching document picker controls
            return this.each(function () {
                // we expect the target to be a div with a document picker inside
                var container = $(this);
                methods.emptyControl(methods.getControlContext(container));
            });
        }
        else if (options === 'get') {

            // get the value of the specified people picker (expects selector to be a single instance)
            // we expect the target to be a div with a people picker inside
            var container = $(this);
            var controlContext = methods.getControlContext(container);
            return methods.parseValue(controlContext.hiddenInput);
        }
        else if (options === 'set') {

            var value = null;

            if (arguments.length > 1) {

                value = arguments[1];

                // put a single object into an array
                if (!$.isArray(value)) {
                    value = [value];
                }
            }

            //// set any matching people picker controls
            return this.each(function () {

                // we expect the target to be a div with a document picker inside
                var container = $(this);
                var controlContext = methods.getControlContext(container);
                methods.setSelectedDocs(controlContext, value);
            });
        }
        else {

            // default to creation
            return this.each(function () {

                // we expect the target to be a div which we will put things inside
                var documentPicker = methods.init.call(this);

                if ($.isFunction(settings.onLoaded)) {
                    settings.onLoaded.call(documentPicker);
                }
            });
        }
    }
})(jQuery);