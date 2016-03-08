//
//  MailClient.swift
//  MSGraph.MailClient
//
//  Created by Simon Jaeger (@simonjaegr) on 07/03/16.
//  Copyright © 2016 Office 365 Development Patterns and Practises.
//

import Foundation

struct MailClientConstants {
    static let graphServiceUrl = "https://graph.microsoft.com/"
    static let graphServiceVersion = "v1.0"
}

class MailClient {
    var resolver: ADALDependencyResolver?
    var graphClient: MSGraphServiceClient?
    var ready = false
    
    // Initialize ADAL (Active Directory Authentication Library) and the Microsoft Graph SDK.
    init() {
        self.resolver = ADALDependencyResolver(plist: ())
        self.graphClient = MSGraphServiceClient(url: self.getGraphServiceUrl(), dependencyResolver: self.resolver)
        self.ready = false
    }
    
    // Get the complete Microsoft Graph URL (including the version).
    func getGraphServiceUrl() -> String {
        return MailClientConstants.graphServiceUrl + MailClientConstants.graphServiceVersion
    }
    
    // Let a user sign in, before we can call the Microsoft Graph.
    func signInWithCallback(callback: (Bool) -> Void) {
        self.resolver?.interactiveLogonWithCallback { (result: ADAuthenticationResult!) -> Void in
            self.ready = result.status == AD_SUCCEEDED
            
            // Call the callback so that we may continue.
            callback(self.ready)
        }
    }
    
    // Get the current signed in user.
    func getUserWithCallback(callback: (MSGraphServiceUser!) -> Void) {
        // Make sure that the client is ready.
        if (!self.ready || self.graphClient == nil) {
            print("The mail client has not been initialized properly")
            
            // Nothing to return.
            callback(nil)
        }
        else {
            // Get the user
            self.graphClient?.me.readWithCallback { (user: MSGraphServiceUser!, error: MSOrcError!) -> Void in
                // Return the user.
                callback(user)
            }
        }
    }
    
    // Create a recipient object using an email address.
    func createRecipientWithAddress(address: String) -> MSGraphServiceRecipient {
        // Create the email address object.
        let emailAddress = MSGraphServiceEmailAddress()
        emailAddress.address = address
        
        // Create and return the recipient object.
        let recipient = MSGraphServiceRecipient()
        recipient.emailAddress = emailAddress
        return recipient
    }
    
    // Create a body object using HTML content.
    func createBodyWithContent(content: String) -> MSGraphServiceItemBody {
        let body = MSGraphServiceItemBody()
        
        // Set the content type to HTML.
        body.contentType = .Html
        
        // Set the HTML content.
        body.content = content
        return body
    }
    
    // Create a message object that can be used when sending a mail.
    func createMessageWithSubject(subject: String, content: String, recipients: [String]) -> MSGraphServiceMessage {
        let message = MSGraphServiceMessage()
        
        // Set the message subject.
        message.subject = subject
        
        // Set the importance of the message (Low, Normal, High).
        message.importance = .Normal
        
        // Convert the recipient strings to recipient objects used
        // with the Microsoft Graph.
        let toRecipients = recipients.map({ (recipient: String) -> MSGraphServiceRecipient in return self.createRecipientWithAddress(recipient) })
        
        // Set the recipients of the message.
        message.toRecipients = NSMutableArray(array: toRecipients)
        
        // Create and set the body object.
        let body = self.createBodyWithContent(content)
        message.body = body
        return message
    }
    
    // Send a mail using a message object.
    func sendMailWithMessage(message: MSGraphServiceMessage, callback: (Bool) -> Void) {
        // Make sure that the client is ready.
        if (!self.ready || self.graphClient == nil) {
            print("The mail client has not been initialized properly")
            
            // Return failure.
            callback(false)
        }
        else {
            // Send the mail.
            self.graphClient?.me.operations.sendMailWithMessage(message, saveToSentItems: true, callback: { (status: Int32, error: MSOrcError!) -> Void in
                
                // Call the callback.
                callback(status == 0 && error == nil)
            })
        }
    }
    
    // Send a mail to the current user.
    func sendMeWithCallback(callback: (Bool) -> Void, logger: (String) -> Void) {
        // Let a user sign in.
        logger("Signing in...")
        self.signInWithCallback { (success: Bool) -> Void in
            if (!success) {
                callback(false)
                return
            }
            
            // Get the current user.
            logger("Getting user information...")
            self.getUserWithCallback { (user: MSGraphServiceUser!) -> Void in
                // Validate the user.
                if (user == nil || user.mail == nil) {
                    callback(false)
                    return
                }
                
                // Create the message.
                let message = self.createMessageWithSubject("Hello #Office365Dev", content:
                    "<strong>Lorem ipsum dolor sit amet</strong>, consectetur adipiscing " +
                        "elit, sed do eiusmod tempor incididunt ut labore et dolore " +
                        "magna aliqua. Ut enim ad minim veniam, quis nostrud " +
                        "exercitation ullamco laboris nisi ut aliquip ex ea commodo " +
                        "consequat. Duis aute irure dolor in reprehenderit in voluptate " +
                        "velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint " +
                        "occaecat cupidatat non proident, sunt in culpa qui officia deserunt " +
                    "mollit anim id est laborum.", recipients: [user.mail])
                
                // Send the message.
                logger("Sending mail...")
                self.sendMailWithMessage(message, callback: callback)
            }
        }
    }
}