function pnpLoadFiles() {

    var urlbase = 'https://localhost:44324';
    var files = [
        '/js/pnp-settings.js',
        '/js/pnp-core.js',
        '/js/pnp-clientcache.js',
        '/js/pnp-config.js',
        '/js/pnp-logging.js',
        '/js/pnp-devdashboard.js',
        '/js/pnp-uimods.js'
    ];

    // this function will be used to recursively load all the files
    var engine = function () {

        // maintain context
        var self = this;

        // get the next file to load
        var file = self.files.shift();

        var fullPath = urlbase + file;

        $.ajax({
            type: 'GET',
            url: fullPath,
            cache: true,
            dataType: 'script'
        }).done(function () {
            if (self.files.length > 0) {
                engine.call(self);
            }
        }).fail(function (x, m, e) { console.error('Error loading script "' + fullPath + '": ' + e); });
    };

    // call the engine with our context
    engine.call({ files: files });
};

