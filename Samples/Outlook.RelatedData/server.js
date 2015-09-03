'use strict';

var fs = require('fs'),
    express = require('express'),
    http = require('http'),
    https = require('https');

var https_options = {
  key: fs.readFileSync('./hostkey.pem'),
  cert: fs.readFileSync('./hostcert.pem')
};

var PORT = 8443,
    HOST = 'localhost';

var app = express();

// set static routes
app.use('/', express.static(__dirname + '/src'));
app.use('/vendor', express.static(__dirname + '/bower_components'));
app.use('/template', express.static(__dirname + '/bower_components/ui.bootstrap/template'));

//https://localhost:8443/ui.bootstrap/template/accordion/accordion-group.html

var server = https.createServer(https_options, app)
                  .listen(PORT, HOST);


var httpServer = http.createServer(app);
var httpOptions = {
	port: 80,
	host: 'localhost'
};

httpServer.listen(httpOptions);

console.log('+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+');
console.log('HTTPS Server listening @ https://%s:%s', HOST, PORT);
console.log('+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+');
