var webpack = require("webpack");
var path = require('path');

module.exports = {
  entry: './js/app.jsx',
  output: {
    path: './dist',
    filename: 'app.dist.js',
  },
  devtool: '#source-map',
  node: {
          net: 'empty',
          tls: 'empty',
          dns: 'empty'
  },
  resolve: {
		// require files in app without specifying extensions
		extensions: ['', '.js', '.json', '.jsx', '.less'],
		alias: {
			// pretty useful to have a starting point in nested modules
			'appRoot': path.join(__dirname, 'js'),
			'vendor': 'appRoot/vendor'
		}
	},
  module: {
    loaders: [
      {
        test: /.jsx?$/,        
        loader: 'babel-loader', 
        exclude: /node_modules/,
        query: {
          presets: ['es2015', 'react']
        }
      },      
      { test: /\.css$/, loader: "style-loader!css-loader" },
      { test: /\.jpe?g$|\.gif$|\.png$|\.svg$|\.woff$|\.woff2$|\.eot$|\.ttf$|\.wav$|\.mp3$/, loader: "file-loader" }
      
    ]
  }
};