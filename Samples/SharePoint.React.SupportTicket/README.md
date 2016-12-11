# Sample React JS Form Application Using React(with form validation), Webpack, SharePoint and the REST API

This application show you how you can quickly build and package React forms with validation and send form data to a SharePoint list via the REST API

![](http://i.imgur.com/0VlaKnt.png)

This application has been built using [React](https://facebook.github.io/react/) and [Webpack](https://webpack.github.io) and demonstrates how you can implement a React form aplication in combination with leveraging SharePoint REST APIs. 

## Prerequisites

In order to use this application you need a SharePoint 2013/2016 tenant. While this application has been built on SharePoint Online, it should work as well with SharePoint on-premises.

## Configuration and Deployment Options

Following are just a few quick steps that you need to complete in order to see this application working in your tenant:

- clone this repository
- in the src/js/appConfig.js file, change the "listurl" value to the URL of your SharePoint tenant and list (that 	will be created named "TicketsQueue")
- place the output "app.dist.js" and "app.dist.js.map" files in the "deploy\SupportTickets.React\assets" folder 
- change the reactcreatesupportticket.webpart file in "deploy\SupportTickets.React\scriptparts" folder to reflect the right path to your site collection's style library so that this pre-configured script editor can find your js file assets.
- in the node.js command line, CD to your SRC folder and run the following:

	

	$ npm install	 
	
	$ npm run dist

	


The deployment for this uses a variant of the PnP APP SCRIPT PART sample to modify the host web by adding a new option as app script part that is preconfigured to use the React app scripts. 

Once you have done the above, run the console app (change the url in the Program.cs file to point to your site collection). This will create the necessary artifacts, as well as add the app script part (pre-configued script editor web part). Then just create a new page, add the app script part and test.


##  Adding an Add-In Script Part To Web Part Gallery ##
Adding of the web part to the host web is simply implemented by uploading the web part to web part gallery using the FileCreationInformation object. In this sample implementation this is done on request when button is pressed, but we could automate this as part of the add-in installation or simply push the web part to web part gallery from remotely location using similar CSOM logic for example during site collection provisioning. In the code we also set the group attribute properly for the item in the web part gallery, so that web parts are grouped under Add-in Script Part group.

For more information: [https://github.com/OfficeDev/PnP/tree/master/Samples/Core.AppScriptPart](https://github.com/OfficeDev/PnP/tree/master/Samples/Core.AppScriptPart)

## Developing

When developing you can use the `$ npm start` command. It will start a new instance of the webpack web server on **https://localhost:5555**. Please note that from that page you won't be able to connect to your SharePoint tenant but you will be able to test how the form and validation works in the application.

## React and Form Validation and Decoration

[React](https://facebook.github.io/react/) is a component (class) structured library for presentation and was developed by Facebook. React is not a "framework". React is declarative, stateful component-based and can also render on the server using node.js

With that being said, there are a few other supporting characters that were used here:

- [Babel](https://babeljs.io)
- [Webpack](https://webpack.github.io)

The solution uses the JOI components and the React validation mixin to implement field and form level validation.


	var Joi = require("joi");
	
	var JoiValidationStrategy = require("joi-validation-strategy");
	
	var ReactValidationMixin = require("react-validation-mixin");


The form does basic string and email format validation at the field level and you move away from the field. Anything marked as required that is not completed when Submit is clicked, will be called out to complete before the submit can happen. 

![](http://i.imgur.com/DbJuwqg.png)

Basically we wrap our elements with a validation class to that we can do validation in a consistent manner and wrap the element with any necessary validation decorators. A wrapped element looks like this:

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
	

With the the use of the React-validation-mixin, we can define validation rules as such:

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

The sample also performs some form-level validation at the time of form submission.

## Basic Flow


![](http://i.imgur.com/KjtU2bf.png)

On submit, once the data is validated, this example formats an XMLHTTPREQUEST and the xhr send the data to a SharePoint list. That's pretty much is for this example. 

Any updates to this sample will include:

- using Flux for two-way data floW additional CRUD operations (searching, editing, etc) 
- It will also show an example for sending data to a WebAPI class, which will in turn, leverage a CSOM common library in a provider hosted add-in scenario, as well as leveraging the PnP.js Core Library.


<img src="https://telemetry.sharepointpnp.com/pnp/samples/Provisioning.React.SupportTicket" />
