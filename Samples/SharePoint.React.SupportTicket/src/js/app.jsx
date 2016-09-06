var React = require('react');
var ReactDOM = require('react-dom');
var CreateTicket = require('./components/CreateTicket.jsx');

require('../node_modules/office-ui-fabric/dist/css/fabric.css');
require('../node_modules/office-ui-fabric/dist/css/fabric.components.css');
require('../css/styles.css');
require('bootstrap-webpack');
require('react-bootstrap');


var App = React.createClass({ 
  render: function() {     
     
    return (
      <div>        
        <CreateTicket />        
      </div>
    );
  },
});





