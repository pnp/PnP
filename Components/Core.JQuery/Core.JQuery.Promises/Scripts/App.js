$(function () {

    // setup our page button events so people can test

    var methods = {

        setupExample: function (e) {
            e.preventDefault();

            //for (var h = 0; h < 500; h++) {


                // here we need to make sure we have our testing list and that it has items in it.
                $pnpcore.withSPContext(function (context) {

                    var list = context.get_web().get_lists().getByTitle('AppTestList');
                    context.load(list, 'Include(ItemCount)');

                    //


                    //    context.executeQueryAsync(function () {

                    //        var h2 = 0;

                    //    }, function () {

                    //        var h3 = 0;

                    //    });
                    //}

                    //var web = context.get_web();
                    //context.load(web);

                    context.ext_executeQueryRetry(function () {

                        // stuff

                        alert('loaded list: ' + list.get_itemCount());




                    }).fail(function (sender, args) {
                        // note we are getting the same args we would have gotten by calling executeQueryAsync directly - the same delegates will work


                        var j = 0;

                    });
                });

            //} // end for

        }
    };



    $('#setup-example').on('click', methods.setupExample);
});
