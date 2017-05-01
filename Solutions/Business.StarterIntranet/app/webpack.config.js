var webpack = require('webpack');
const path = require('path');
var ExtractTextPlugin = require("extract-text-webpack-plugin");

// CSS files output
// They are outputed directly on the dist root folder because of the webpack resolve url loader. 
// CSS 'url()' are not processed correctly when the CSS files are in a subfolder (or I missed something there...)
var layoutsCssExtractTextPlugin = new ExtractTextPlugin("layouts.css");
var layoutsCssEditExtractTextPlugin = new ExtractTextPlugin("layouts-edit.css");
var portalCssExtractTextPlugin = new ExtractTextPlugin("portal.css");
var bootstrapCssExtractTextPlugin = new ExtractTextPlugin("bootstrap-iso.css");

// Wrap the whole Webpack config with a validator to avoid typos mistakes
module.exports = {

    entry: {
        app: "./main.tsx", // The main entry point for the application.

        // Note that 'es6-promise' and 'whatwg-fetch' are necessary to get pnp work in IE
        // More info here https://github.com/OfficeDev/PnP-JS-Core/wiki/Install-and-Use
        vendor: [
            "jquery",
            "knockout",
            "knockout-mapping",
            "bootstrap/dist/js/bootstrap.js",
            "es6-promise",
            "flickity/dist/flickity.css",
            "whatwg-fetch"
        ]
    },
    
    output: {
        filename: "js/app.js",
        path: path.join(__dirname, "dist"),

        // Expose the entry point as the 'Intranet' global var. 
        // We need this to be able to apply Knockout JS bindings manually for SharePoint display templates (the 'ko' variable is not exposed in the global context)
        library: ['Intranet']
    },

    node: {
        fs: "empty" // For the iCal feature (an error is raised otherwise)
    },

    // Context for entry point
    context: path.join(__dirname, "src"), 

    // Enable sourcemaps for debugging webpack's output.
    // To choose best source map mode: http://cheng.logdown.com/posts/2016/03/25/679045
    // Be careful, source maps are sometimes not recognized correctly by Google Chrome (use Google Chrome Canary if you encounter some troubles)
    devtool: "cheap-module-source-map",
    
    resolve: {                   
        extensions: [".webpack.js", ".web.js", ".js",".ts",".tsx"]        
    },

    module: {
        rules: [
            {
                // All output '.js' files will have any sourcemaps re-processed by 'source-map-loader'.
                test: /\.js$/,
                enforce: "pre",
                loader: "source-map-loader" 
            },
            {
                // We use the text loader to get the raw HTML markup for a Knockout component template file
                test: /\.html$/,
                use: 'text-loader'
            },
            {
                // Ouput images files directly into the 'dist' folder without modifications. They will be used by the CSS stylesheets.
                test: /\.(jpg|png|gif|ico)$/,
                use: [
                    {
                        loader: 'file-loader',
                        query : {
                            name: "./img/[name].[ext]"
                        }
                    }
                ],
            },
            {
                // Loader for TypeScript files (.ts)
                test: /\.tsx?$/,
                use: 'ts-loader'
            },
            {
                // CSS loader for the Office UI fabric styles
                test: /\.css$/,
                use: [
                    'style-loader',
                    'css-loader'
                ]
            },
            {
                // Custom fonts (Myriad Pro)
                test: /\.woff?$|\.woff2?$|\.ttf$|\.eot$|\.svg$/,
                use: [
                    {
                        loader: 'file-loader',
                        query : {
                            name: "./fonts/[name].[ext]"
                        }
                    }
                ],
               
            },
            {
                // Sass Loader. Note that we use the resolve-url loader to insert relative path to images into style sheets
                test: /\.scss$/,
                use: portalCssExtractTextPlugin.extract({
                        fallback: 'style-loader',
                        use: [
                            {
                                loader: 'css-loader',
                            },
                            {
                                loader: 'resolve-url-loader'
                            },
                            {
                                loader: "sass-loader",
                                options: {
                                    sourceMap: true
                                }
                            }
                        ],
                }),
                exclude: /(layouts|layouts-edit)\.scss$/
            },
            {
                // Page layouts styles (display and edit) are bundled separately. The loading is controlled by the page layout itself. 
                test: /layouts\.scss$/,
                use: layoutsCssExtractTextPlugin.extract({
                        fallback: 'style-loader',
                        use: [
                            {
                                loader: 'css-loader',
                            },
                            {
                                loader: "sass-loader",
                                options: {
                                    sourceMap: true
                                }
                            }
                        ],
                }),
            },  
            {
                test: /layouts-edit\.scss$/,
                use: layoutsCssEditExtractTextPlugin.extract({
                        fallback: 'style-loader',
                        use: [
                            {
                                loader: 'css-loader',
                            },
                            {
                                loader: "sass-loader",
                                options: {
                                    sourceMap: true
                                }
                            }
                        ],
                }),
            },  
            {
                // Isolate the Bootstrap CSS to avoid conflicts with the SharePoint default CSS
                // More info here: https://formden.com/blog/isolate-bootstrap
                // We used a customized CSS version of Bootstrap (http://getbootstrap.com/customize/) because we don't need the full package
                test: /\.less$/,
                use: bootstrapCssExtractTextPlugin.extract({
                        fallback: 'style-loader',
                        use: [
                            {
                                loader: 'css-loader',
                                options: {
                                    sourceMap: true
                                }
                            },
                            {
                                loader: "string-replace-loader",
                                query: {
                                    multiple: [
                                        { search: '\.bootstrap-iso body', replace: '.bootstrap-iso' },
                                        { search: '\.bootstrap-iso html', replace: 'bootstrap-iso' }
                                    ]
                                }
                            },
                            {
                                loader: 'less-loader'
                            },                            
                        ],
                }),                
            }, 
        ]
    },

    plugins: [

        // Every time webpack encounters $, jQuery, window.jQuery or ko, it will replace it by the correct library dependency
        // Especially useful for the default bootstrap.min.js file
        new webpack.ProvidePlugin({
            $: 'jquery',
            jQuery: 'jquery',
            "window.jQuery" : 'jquery',
            ko : 'knockout'
        }),

        portalCssExtractTextPlugin,
        bootstrapCssExtractTextPlugin,
        layoutsCssExtractTextPlugin,
        layoutsCssEditExtractTextPlugin,

        // Split the application into chunks
        new webpack.optimize.CommonsChunkPlugin({
            name: 'vendor',
            filename: 'js/vendor.js',
        }),
    ]
};