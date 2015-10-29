// by default the $.getScript method will get a fresh copy of the file each time. For
// performance reasons you may want these scripts to be cached and this version of the Loader
// accomplishes that.
(function (/*string[]*/ files) {

    // create a promise
    var promise = $.Deferred();

    // this function will be used to recursively load all the files
    var engine = function () {

        // maintain context
        var self = this;

        // get the next file to load
        var file = self.files.shift();

        // load the remote script file
        $.ajax({
            type: 'GET',
            url: file,
            cache: true,
            dataType: 'script'
        }).done(function () {
            if (self.files.length > 0) {
                engine.call(self);
            }
            else {
                self.promise.resolve();
            }
        }).fail(self.promise.reject);
    };

    // create our "this" we will apply to the engine function
    var ctx = {
        files: files,
        promise: promise
    };

    // call the engine with our context
    engine.call(ctx);

    // give back the promise
    return promise.promise();

})(['https://localhost:44323/corefunctions.js', 'https://localhost:44323/uimodifications.js']).done(function () { /* all scripts are loaded and I could take actions here */ });