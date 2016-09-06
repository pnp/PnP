
//const LISTURL= `/_api/Web/Lists/GetByTitle('TicketsQueue')/Items`;

var React = require('react');
var ReactDOM = require('react-dom');
var Joi = require("joi");
var JoiValidationStrategy = require("joi-validation-strategy");
var ReactValidationMixin = require("react-validation-mixin");
var TicketConstants = require('appRoot/constants/TicketConstants');
var Config = require('appRoot/appConfig');

var InputValidationDecorator = React.createClass({
    renderHelpText: function(message) {
        return (
            <span className="help-block ms-borderColor-redDark" >
                {message}
            </span>
        );
    },
    render: function() {
        var error 
            = this.props.getValidationMessages(
                this.props.name);

        var formClass = "ms-TextField ms-borderColor-redDark";

        if (error.length > 0) {
            formClass = formClass + " has-error";
        }

        return (
            <div className={formClass}>
                <label className="ms-font-m ms-fontColor-themeDarkAlt" htmlFor={this.props.name}>
                    {this.props.label}
                </label>
                <br/>
                <input className="ms-font-s-plus ms-TextField-field" {...this.props}/><br/>
                <span className="ms-font-xs">{this.props.description}</span>
                {this.renderHelpText(error)}
            </div>
        );
    }
});

var TextAreaValidationDecorator = React.createClass({
    renderHelpText: function(message) {
        return (
            <span className="help-block ms-fontColor-red">
                {message}
            </span>
            
        );
    },
    render: function() {
        var error 
            = this.props.getValidationMessages(
                this.props.name);

        var formClass = "ms-TextField ms-TextField--multiline";

        if (error.length > 0) {
            formClass = formClass + " has-error";
        }

        return (
            <div className={formClass}>
                <label className="ms-font-m ms-fontColor-themeDarkAlt" htmlFor={this.props.name}>
                    {this.props.label}
                </label>
                <br/>                
                <textarea className="ms-font-s ms-TextField-field" {...this.props}/>
                {this.renderHelpText(error)}
            </div>
        );
    }
});

var App = React.createClass({
  render: function() {
    return (
      <div>
        <h1>Simple Ticket Form (React)</h1>
      </div>
    );
  }
});

var CreateTicket = React.createClass({

    // Field level validators
    validatorTypes: {
        title: Joi.string().required()
            .label("Ticket Title"),
        issuedescription: Joi.string().required()
            .label("Issue Description"),        
        tickethistory: Joi.string().required()
            .label("Ticket History"),
        contactfullname: Joi.string().required()
            .label("Full Name"),        
        contactphone: Joi.string().required()
            .label("Contact Phone"),
        contactemail: Joi.string().email()
              .required()
              .label('Contact Email')        
    },
    getValidatorData: function() {        
        return this.state;
    },       
    getInitialState: function () {
        var valid = (this.props.isValid && this.props.isValid()) || true;    

        // for demo purposes only    
        var x = Math.floor((Math.random() * 1000) + 10);
        var tktinfo = "Ticket #" + x + " - new ticket";

        return { 
            
            type: 'info',
            message: '',
            ticketnumber: x,
            title: null,
            businessimpact: null,
            issuedescription: null,
            tickethistory: tktinfo,
            contactfullname: null,
            contactemail: null,
            contactphone: null,
            supportplan: null
                   
        }
    },    
    resetFormData: function() {

        // Must reset fields through state

        // for demo purposes only    
        var x = Math.floor((Math.random() * 1000) + 10);
        var tktinfo = "Ticket #" + x + " created";

        //alert("resetting form data");
        this.setState({ticketnumber: x});
        this.setState({title: ''});
        this.setState({issuedescription: ''});
        this.setState({tickethistory: tktinfo});
        this.setState({businessimpact: ''});
        this.setState({contactfullname: ''});
        this.setState({contactemail: ''});
        this.setState({contactphone: ''});
        this.setState({supportplan: ''});        
        
    },
    onChange: function(event) {   
        if (event.target.name != "submitButton")       
        { 
            var state = {};
            state[event.target.name] = event.target.value;    
            this.setState(state);
        }
    },
    onFocus: function() {           
        return {
            type: 'info',
            message: ''
        };
    },
    validateEmail: function (event) {  
        alert("Validating email");
        var regx = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        return regx.test(event);
    },    
    onSubmit: function () {
           
        // debug only
        var data = JSON.stringify({            
            TicketNumber: this.state.ticketnumber,
            Title: this.state.title,
            BusinessImpact: this.state.businessimpact,
            IssueDescription: this.state.issuedescription,
            TicketHistory: this.state.tickethistory,
            ContactFullName: this.state.contactfullname,
            ContactEmail: this.state.contactemail,
            ContactPhone: this.state.contactphone,
            SupportPlan: this.state.supportplan            
        });        
        //alert(data);        

        // Handle field level validations        
        var onValidate = function(error) {

            // form validation throws alerts on field level errors
            if (error) {
                if (error.title) {
                    alert(error.title);
                }
                if (error.businessimpact) {
                    alert(error.businessimpact);
                }
                if (error.issuedescription) {
                    alert(error.issuedescription);
                }
                if (error.tickethistory) {
                    alert(error.tickethistory);
                }
                if (error.contactfullname) {
                    alert(error.contactfullname);
                }
                if (error.contactemail) {
                    alert(error.contactemail);
                }
                if (error.contactphone) {
                    alert(error.contactphone);
                }                
                if (error.supportplan) {
                    alert(error.supportplan);
                }

            }        

            // no errors - submit to list via xhr send
            if (!error) {
                this.setState({ type: 'info', message: ' Submitting ticket...' }, this.submitTicketData);                               
            }
        }; 

        this.props.validate(onValidate.bind(this));
                   
    },

    render: function() {

       // The ValidatedInput and ValidatedTextArea sections wrap the input and textarea fields with validation decoration           
       var self = this;       
       return (
            <div className="ms-Grid-col ms-u-sm8 ms-borderColor-themePrimary">
                    <div className="panel panel-default">
                        <div className="panel-heading">
                            <h3 className="panel-title"><i className="ms-Icon ms-Icon--metadata" aria-hidden="true"></i>  Create a support ticket</h3>                    
                        </div>
                        <div className="panel-body">                        
                            <div className="col-sm-9">                                 

                                <InputValidationDecorator 
                                    name="title"
                                    type="text" 
                                    ref="title" 
                                    placeholder="What is the title of your ticket?" 
                                    description=""
                                    label="Ticket Title"
                                    value={this.state.title}
                                    onChange={this.onChange}
                                    onFocus={this.onFocus}   
                                    onBlur={this.props.handleValidation("title")}
                                    getValidationMessages=
                                        {this.props.getValidationMessages}/>

                                <div className="ms-Dropdown">                                   
                                    
                                    <label className="ms-font-m ms-fontColor-themeDarkAlt">Business Impact</label><br/>                                    
                                    <select name="businessimpact" ref="businessimpact" label="Business Impact"
                                        className="ms-font-s-plus ms-TextField-field"                                                                                                                
                                        onChange={this.onChange}                                        
                                        onBlur={this.props.handleValidation("businessimpact")}
                                        value={this.state.businessimpact}
                                        defaultValue="Low Business Impact"
                                        required>                                            
                                            <option value="Low Business Impact">Low Business Impact</option>
                                            <option value="Medium Business Impact">Medium Business Impact</option>
                                            <option value="High Business Impact">High Business Impact</option>                                        
                                    </select>
                                    
                                    <br/>
                                    <span className="ms-font-xs">Select the business impact</span>                                                        
                                    
                                </div>
                                

                                <TextAreaValidationDecorator 
                                    rows="5"
                                    name="issuedescription"                                    
                                    ref="issuedescription" 
                                    placeholder="Help us help you. Give it a description" 
                                    label="Issue Description"
                                    value={this.state.issuedescription}
                                    onChange={this.onChange}
                                    onBlur={this.props.handleValidation("issuedescription")}
                                    getValidationMessages=
                                        {this.props.getValidationMessages}/>  

                                <InputValidationDecorator 
                                    name="contactfullname"
                                    type="text" 
                                    ref="contactfullname" 
                                    placeholder="What is your full name?" 
                                    label="Contact Full Name"
                                    description="This should be your first and last name"
                                    value={this.state.contactfullname}
                                    onChange={this.onChange}
                                    onBlur={this.props.handleValidation("contactfullname")}
                                    getValidationMessages=
                                        {this.props.getValidationMessages}/>
                                                                    
                                <InputValidationDecorator 
                                    name="contactemail"
                                    type="text" 
                                    ref="contactemail" 
                                    placeholder="What is your email address?" 
                                    label="Contact Email"
                                    value={this.state.contactemail}
                                    validate={this.validateEmail}
                                    onChange={this.onChange}
                                    onBlur={this.props.handleValidation("contactemail")}
                                    getValidationMessages=
                                        {this.props.getValidationMessages}
                                    required/>

                                <InputValidationDecorator 
                                    name="contactphone"
                                    type="text" 
                                    ref="contactphone" 
                                    placeholder="What number can we reach you at?" 
                                    label="Contact Phone"
                                    value={this.state.contactphone}
                                    onChange={this.onChange}
                                    onBlur={this.props.handleValidation("contactphone")}
                                    getValidationMessages=
                                        {this.props.getValidationMessages}/>

                                <div className="ms-Dropdown">
                                    <label className="ms-font-m ms-fontColor-themeDarkAlt" htmlFor="supportlevel">Current Support Plan</label><br/>
                                    <select
                                        name="supportplan" 
                                        ref="supportplan" 
                                        label='Support Plan'
                                        className="ms-font-s-plus ms-TextField-field"                                                                           
                                        onChange={this.onChange}  
                                        defaultValue="Basic Free Plan"                                     
                                        value={this.state.supportplan}>                                            
                                            <option value="Basic Free Plan">Basic Free Plan</option>
                                            <option value="Standard Plan">Standard Plan</option>
                                            <option value="Premium Plan">Premium Plan</option>
                                            <option value="Super Duper Plan">Super Duper Plan</option>
                                    </select><br/>
                                    <span className="ms-font-xs">Select your current support plan</span>
                                </div>

                                <InputValidationDecorator 
                                    name="ticketnumber"
                                    type="text" 
                                    ref="ticketnumber" 
                                    placeholder="Generated Ticket Number" 
                                    description=""
                                    label="Ticket Number"
                                    value={this.state.ticketnumber}
                                    onChange={this.onChange}
                                    disabled
                                    getValidationMessages=
                                        {this.props.getValidationMessages}/>

                                <TextAreaValidationDecorator
                                    rows="5" 
                                    name="tickethistory"                                    
                                    ref="tickethistory" 
                                    placeholder="We need some history..." 
                                    label="Ticket History"
                                    value={this.state.tickethistory}
                                    onChange={this.onChange}
                                    onBlur={this.props.handleValidation("tickethistory")}
                                    getValidationMessages=
                                        {this.props.getValidationMessages}/>
                                
                                <br/>
                                <button type="submit" name="submitButton" ref="submitButton" className="ms-Button ms-Button--command ms-borderColor-themePrimary" onClick={this.onSubmit} >
                                    <span className="ms-Button-icon"><i className="ms-Icon ms-Icon--plus"></i></span>
                                    <span className="ms-Button-label">Create Ticket</span>
                                    
                                </button>
                              
                            </div>                                                                         
                          </div>
                          
                        <div className="ms-MessageBanner ms-bgColor-themeSecondary">
                            <div className="ms-MessageBanner-content ms-bgColor-themeSecondary">
                                <div className="ms-MessageBanner-text ms-bgColor-themeSecondary">
                                    <div className="ms-MessageBanner-clipper ms-bgColor-themeSecondary">                                    
                                        {this.state.message}
                                    </div>
                                </div>                            
                                
                            </div>                            
                        </div>                                      
                    </div>              
            </div>                  
        );
    },

    submitTicketData: function () {
    
        // type = "SP.Data. + list name + "ListItem"
        var data = JSON.stringify({
            __metadata: { "type": "SP.Data.TicketsQueueListItem" },
            TicketNumber: this.state.ticketnumber,
            Title: this.state.title,
            BusinessImpact: this.state.businessimpact,
            IssueDescription: this.state.issuedescription,
            TicketHistory: this.state.tickethistory,
            ContactFullName: this.state.contactfullname,
            ContactEmail: this.state.contactemail,
            ContactPhone: this.state.contactphone,
            SupportPlan: this.state.supportplan,
            CurrentStatus: 'New'               
        });
        //alert(data);        

        this.setState({ type: 'success', message: '  Your ticket is being submitted.'});

        // Send the form data.

        /* Uncomment below when deploying to SharePoint  
        var xhr = new XMLHttpRequest();
        var _this = this;
        xhr.onreadystatechange = function() {
            if (xhr.readyState === 4) {
                var response = JSON.parse(xhr.responseText);
                if (xhr.status === 200 && response.status === 'OK') {
                    _this.setState({ type: 'success', message: ' Your ticket has been received. You will be contacted within 24 hours. Thank You!' });                
                }
                else {
                    _this.setState({ type: 'danger', message: ' Sorry, there has been an error. Please try again later or send us an email at support@contoso.com.' });
                }
            }
        };
        
        xhr.open('POST', _spPageContextInfo.webAbsoluteUrl + Config.default.listurl, true);
        xhr.setRequestHeader('Content-Type', 'application/json;odata=verbose');
        xhr.setRequestHeader('Accept', 'application/json;odata=verbose');
        xhr.setRequestHeader('X-RequestDigest', $("#__REQUESTDIGEST").val());  
        
        xhr.dataType = 'json';
        xhr.send(data);  

        */       
        
        this.resetFormData();    

    },
    
    
    requestBuildQueryString: function (params) {
        var queryString = [];
        for(var property in params)
        if (params.hasOwnProperty(property)) {
            queryString.push(encodeURIComponent(property) + '=' + encodeURIComponent(params[property]));
        }
        return queryString.join('&');
        },
    });


module.exports = CreateTicket;

// Decorate our component with validations support
var TicketValidation = ReactValidationMixin(JoiValidationStrategy)(CreateTicket);

ReactDOM.render(
    <TicketValidation/>,
    document.getElementById("app"));