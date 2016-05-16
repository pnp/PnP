(function () {

    var modification = function (/*string*/ name, /*string*/ selector, /*function*/ modify) {
        this.name = name;
        this.selector = selector;
        this.modify = modify;
    };

    modification.prototype = {
        name: '',
        selector: '',
        select: function () {
            return $(this.selector);
        },
        modify: function () { },
    };


    // the array used to store our modifications
    var modifications = [];

    // add the modification to hide the new site link
    modifications.push(new modification('Remove New Site Link', '#createnewsite', function () { $(this).each(function () { $(this).hide(); }); }));


    // wait for the DOM to be ready
    $(function () {

        // this loop is used to process and log all the modifications
        for (var i = 0; i < modifications.length; i++) {
            var mod = modifications[i];
            $pnp.logging.write('Begin Processing: ' + mod.name, $pnp.logging.levels.Verbose, 'pnp-uimods.js', mod.name);
            try {
                var items = mod.select();
                if (items.length < 1) {
                    $pnp.logging.write('Selector "' + mod.selector + '" returned no results on page, continuing.', $pnp.logging.levels.Verbose, 'pnp-uimods.js', mod.name);
                    continue;
                }
                $pnp.logging.write('Calling: ' + mod.name + ' modify', $pnp.logging.levels.Verbose, 'pnp-uimods.js', mod.name);
                mod.modify.call(items);
                $pnp.logging.write('Called: ' + mod.name + ' modify', $pnp.logging.levels.Verbose, 'pnp-uimods.js', mod.name);
            } catch (e) {
                $pnp.logging.write('Error Processing: ' + mod.name + ': ' + e, $pnp.logging.levels.Error, 'pnp-uimods.js', mod.name);
            }
            $pnp.logging.write('End Processing: ' + mod.name, $pnp.logging.levels.Verbose, 'pnp-uimods.js', mod.name);
        }
    });

})();