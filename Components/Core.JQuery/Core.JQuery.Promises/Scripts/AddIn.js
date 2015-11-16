$pnpcore.onStart(function () {
    
    $('#runtest').on('click', function (e) {
        
        // stop normal button stuff
        e.preventDefault();

        // base example
        $pnpcore.withSPContext(function (context) {
            
            var web = context.get_web();
            context.load(web);

            context.ext_executeQueryRetry(web).done(function () {
                
                $('#result').html('Successfully loaded web! Web Title: ' + this.get_title());
                $('#result').css({ color: 'green' });
                
            }).fail(function (sender, args) {
                
                $('#result').html('Error loading web: ' + args.get_message());
                $('#result').css({ color: 'red' });
                
            });
        });
    });
});