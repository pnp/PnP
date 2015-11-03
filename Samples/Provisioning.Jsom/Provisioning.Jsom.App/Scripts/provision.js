"use strict";

//Set up our namespace
var Contoso = Contoso || {};
Contoso.JSOMProvisioning = Contoso.JSOMProvisioning || {};

Contoso.JSOMProvisioning.ProvisioningClerk = function () {
    var appweburl, hostweburl
    var sitename, siteurl, sitetemplate;
    var sitecolumnname, sitecolumndisplayname, sitecolumntype;
    var contenttypename, contenttypeid;
    var documentlibraryname;
    var filename, filetitle, filefavoritecolor;

    function constructWebCreationInformation(webTitle, webUrl, webTemplate) {
        var wci = new SP.WebCreationInformation();
        wci.set_title(webTitle);
        wci.set_language(1033);
        wci.set_url(webUrl);
        wci.set_useSamePermissionsAsParentSite(true);
        wci.set_webTemplate(webTemplate);
        return wci;
    }
    function constructContentTypeCreationInformation(id, name) {
        var ctci = new SP.ContentTypeCreationInformation();
        ctci.set_description("JSOM Provisioned Demo Content Type");
        ctci.set_group("JSOM Provisioned");
        ctci.set_id(id);
        ctci.set_name(name);
        return ctci;
    }
    function constructFLCI(targetField) {
        var flci = new SP.FieldLinkCreationInformation();
        flci.set_field(targetField);
        return flci;
    }
    function constructLCI() {
        var lci = new SP.ListCreationInformation();
        lci.set_title(documentlibraryname);
        lci.set_templateType(SP.ListTemplateType.documentLibrary);
        return lci;
    }
    function constructFCI() {
        var fci = new SP.FileCreationInformation();
        fci.set_url(filename);
        fci.set_content(new SP.Base64EncodedByteArray())
        return fci;
    }
    function getContext() {
        return new SP.ClientContext(appweburl);
    }
    function getAppContextSite(ctx) {
        var fct = new SP.ProxyWebRequestExecutorFactory(appweburl);
        ctx.set_webRequestExecutorFactory(fct);
        return new SP.AppContextSite(ctx, hostweburl);
    }

    var publicMembers = {
        get_appweburl: function () { return appweburl; },
        set_appweburl: function (rhs) { appweburl = rhs; },
        get_hostweburl: function () { return hostweburl; },
        set_hostweburl: function (rhs) { hostweburl = rhs; },
        get_webtitle: function () { return sitename; },
        set_webtitle: function (rhs) { sitename = rhs; },
        get_weburl: function () { return siteurl; },
        set_weburl: function (rhs) { siteurl = rhs; },
        get_webtemplate: function () { return sitetemplate; },
        set_webtemplate: function (rhs) { sitetemplate = rhs; },
        get_sitecolumnname: function () { return sitecolumnname; },
        set_sitecolumnname: function (rhs) { sitecolumnname = rhs },
        get_sitecolumndisplayname: function () { return sitecolumndisplayname; },
        set_sitecolumndisplayname: function (rhs) { sitecolumndisplayname = rhs },
        get_sitecolumntype: function () { return sitecolumntype; },
        set_sitecolumntype: function (rhs) { sitecolumntype = rhs; },
        get_contenttypename: function () { return contenttypename; },
        set_contenttypename: function (rhs) { contenttypename = rhs; },
        get_contenttypeid: function () { return contenttypeid; },
        set_contenttypeid: function (rhs) { contenttypeid = rhs; },
        get_documentlibraryname: function () { return documentlibraryname; },
        set_documentlibraryname: function (rhs) { documentlibraryname = rhs; },
        get_filename: function () { return filename; },
        set_filename: function (rhs) { filename = rhs; },
        get_filetitle: function () { return filetitle; },
        set_filetitle: function (rhs) { filetitle = rhs; },
        get_filefavoritecolor: function () { return filefavoritecolor; },
        set_filefavoritecolor: function (rhs) { filefavoritecolor = rhs; },

        createsitecolumn: function () {
            // This is the only effective way to create a new column. Adding to the fields collection does not work.
            var fieldschema = '<Field Type="' + sitecolumntype + '" Name="' + sitecolumnname + '" DisplayName="' + sitecolumndisplayname + '" Group="JSOM Provisioned Columns" />';
            var dfd = $.Deferred();

            var ctx = getContext();
            var appctx = getAppContextSite(ctx);

            var targetWeb = appctx.get_site().get_rootWeb();
            var fields = targetWeb.get_fields()
            fields.addFieldAsXml(fieldschema, false, SP.AddFieldOptions.addFieldCheckDisplayName);

            ctx.executeQueryAsync(function () { dfd.resolve(); }, function (sender, args) {
                console.log("Column creation failure: " + args.get_message());
                dfd.reject();
            });
            return dfd.promise();
        },
        deletesitecolumn: function () {
            var dfd = $.Deferred();

            var ctx = getContext();
            var appctx = getAppContextSite(ctx);

            var targetWeb = appctx.get_site().get_rootWeb();
            var fields = targetWeb.get_fields()
            var field = fields.getByTitle(sitecolumndisplayname);
            field.deleteObject();

            ctx.executeQueryAsync(function () { dfd.resolve(); }, function (sender, args) {
                console.log("Column deletion failure: " + args.get_message());
                dfd.reject();
            });
            return dfd.promise();
        },
        createcontenttype: function () {
            var dfd = $.Deferred();

            var ctx = getContext();
            var appctx = getAppContextSite(ctx);

            var targetWeb = appctx.get_site().get_rootWeb();
            var fields = targetWeb.get_fields()
            var field = fields.getByInternalNameOrTitle(sitecolumnname);
            ctx.load(fields);
            ctx.load(field);
            var ctci = constructContentTypeCreationInformation(contenttypeid, contenttypename)
            var newType = targetWeb.get_contentTypes().add(ctci);
            ctx.load(newType);

            ctx.executeQueryAsync(succeed, fail);
            function succeed(sender, args) {
                var fieldRefs = newType.get_fieldLinks();
                ctx.load(fieldRefs);
                ctx.executeQueryAsync(
                    function () {
                        var flci = constructFLCI(field);
                        newType.get_fieldLinks().add(flci);
                        newType.update();
                        ctx.executeQueryAsync(function () { dfd.resolve(); },
                            function (sender, args) {
                                console.log("Content type creation failure: " + args.get_message());
                                dfd.reject();
                            });
                    },
                    function (sender, args) {
                        console.log("Content type creation failure: " + args.get_message());
                        dfd.reject();
                    });
            }
            function fail(sender, args) {
                console.log("Content type creation failure: " + args.get_message());
                dfd.reject();
            }
            return dfd.promise();
        },
        deletecontenttype: function () {
            var dfd = $.Deferred();

            var ctx = getContext();
            var appctx = getAppContextSite(ctx);

            var targetWeb = appctx.get_site().get_rootWeb();
            var webTypes = targetWeb.get_contentTypes();
            var targetType = webTypes.getById(contenttypeid)
            targetType.deleteObject();
            ctx.executeQueryAsync(succeed, fail);
            function succeed() { dfd.resolve(); }
            function fail(sender, args) {
                console.log("Content type deletion failure: " + args.get_message());
                dfd.reject();
            }
            return dfd.promise();
        },
        createsite: function () {
            var dfd = $.Deferred();

            var ctx = getContext();
            var appctx = getAppContextSite(ctx);

            var thisWeb = appctx.get_web();
            ctx.load(thisWeb);
            var wci = constructWebCreationInformation(sitename, siteurl, sitetemplate)
            thisWeb.get_webs().add(wci);
            thisWeb.update();

            ctx.executeQueryAsync(
                function () {
                    dfd.resolve();
                },
                function (sender, args) {
                    console.log("Site creation failure: " + args.get_message());
                    dfd.reject();
            });
            return dfd.promise();
        },
        deletesite: function () {
            var dfd = $.Deferred();

            var ctx = getContext();
            var appctx = getAppContextSite(ctx);

            var thisWeb = appctx.get_web();
            ctx.load(thisWeb);
            ctx.executeQueryAsync(
                function () {
                    var relUrl = thisWeb.get_serverRelativeUrl();
                    var targetWebUrl = (relUrl === "/") ? relUrl + siteurl : relUrl + "/" + siteurl;
                    var targetWeb = appctx.get_site().openWeb(targetWebUrl);
                    targetWeb.deleteObject();

                    ctx.executeQueryAsync(function () { dfd.resolve(); }, function (sender, args) {
                        console.log("Site deletion failure: " + args.get_message());
                        dfd.reject();
                    });
                },
                function (sender, args) {
                console.log("Site deletion failure: " + args.get_message());
                dfd.reject();
            });
            return dfd.promise();
        },
        createdocumentlibrary: function () {
            var dfd = $.Deferred();

            var ctx = getContext();
            var appctx = getAppContextSite(ctx);

            var targetType = appctx.get_site().get_rootWeb().get_contentTypes().getById(contenttypeid);
            var thisWeb = appctx.get_web();
            ctx.load(thisWeb);
            ctx.load(targetType);
            ctx.executeQueryAsync(
                function () {
                    var relUrl = thisWeb.get_serverRelativeUrl();
                    var targetWebUrl = (relUrl === "/") ? relUrl + siteurl : relUrl + "/" + siteurl;
                    var targetWeb = appctx.get_site().openWeb(targetWebUrl);
                    var lci = constructLCI();
                    var newList = targetWeb.get_lists().add(lci);
                    newList.set_contentTypesEnabled(true);
                    var listTypes = newList.get_contentTypes();
                    ctx.load(newList);
                    ctx.load(listTypes);

                    ctx.executeQueryAsync(
                        function () {
                            listTypes.addExistingContentType(targetType);
                            newList.update();
                            ctx.executeQueryAsync(function () { dfd.resolve() }, function (sender, args) {
                                console.log("Document library creation failure: " + args.get_message());
                                dfd.reject()
                            });
                        },
                        function (sender, args) {
                            console.log("Document library creation failure: " + args.get_message());
                            dfd.reject();
                    });
                },
                function (sender, args) {
                    console.log("Document library creation failure: " + args.get_message());
                dfd.reject();
            });
            return dfd.promise();
        },
        deletedocumentlibrary: function () {
            var dfd = $.Deferred();

            var ctx = getContext();
            var appctx = getAppContextSite(ctx);

            var thisWeb = appctx.get_web();
            ctx.load(thisWeb);
            ctx.executeQueryAsync(
                function () {
                    var relUrl = thisWeb.get_serverRelativeUrl();
                    var targetWebUrl = (relUrl === "/") ? relUrl + siteurl : relUrl + "/" + siteurl;
                    var targetWeb = appctx.get_site().openWeb(targetWebUrl);
                    var targetList = targetWeb.get_lists().getByTitle(documentlibraryname);
                    targetList.deleteObject();

                    ctx.executeQueryAsync(function () { dfd.resolve(); }, function (sender, args) {
                        console.log("Document library deletion failure: " + args.get_message());
                        dfd.reject();
                    });
                },
                function (sender, args) {
                    console.log("Document library deletion failure: " + args.get_message());
                    dfd.reject();
                });
            return dfd.promise();
        },
        createfile: function () {
            var dfd = $.Deferred();

            var ctx = getContext();
            var appctx = getAppContextSite(ctx);

            var thisWeb = appctx.get_web();
            ctx.load(thisWeb);
            ctx.executeQueryAsync(
                function () {
                    var relUrl = thisWeb.get_serverRelativeUrl();
                    var targetWebUrl = (relUrl === "/") ? relUrl + siteurl : relUrl + "/" + siteurl;
                    var targetWeb = appctx.get_site().openWeb(targetWebUrl);
                    var targetList = targetWeb.get_lists().getByTitle(documentlibraryname);
                    var fci = constructFCI();
                    var file = targetList.get_rootFolder().get_files().add(fci);
                    ctx.load(file);

                    ctx.executeQueryAsync(function () { dfd.resolve(); }, function (sender, args) {
                        console.log("File creation failure: " + args.get_message());
                        dfd.reject();
                    });
                },
                function (sender, args) {
                    console.log("File creation failure: " + args.get_message());
                    dfd.reject();
                });
            return dfd.promise();
        },
      };
    return publicMembers;
};