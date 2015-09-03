'use strict';

var fs = require('fs'),
    express = require('express'),
    http = require('http');

var app = express();

// set static routes
app.use('/', express.static(__dirname + '/src'));
app.use('/vendor', express.static(__dirname + '/bower_components'));

var httpServer = http.createServer(app);

var PORT = process.env.PORT || 80;
httpServer.listen(PORT);

console.log('+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+');
console.log('HTTPS Server listening @ https://%s:%s', PORT);
console.log('+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+');
 