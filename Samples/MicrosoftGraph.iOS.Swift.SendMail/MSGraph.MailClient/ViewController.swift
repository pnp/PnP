//
//  ViewController.swift
//  MSGraph.MailClient
//
//  Created by Simon Jaeger (@simonjaegr) on 07/03/16.
//  Copyright © 2016 Office 365 Development Patterns and Practises.
//

import UIKit

class ViewController: UIViewController {
    @IBOutlet var label: UILabel?
    @IBOutlet var button: UIButton?
    @IBOutlet var spinner: UIActivityIndicatorView?
    let mailClient = MailClient()
    
    override func viewDidLoad() {
        super.viewDidLoad()
        // Do any additional setup after loading the view, typically from a nib.
    }
    
    // Switch to a light status bar style when this view is appearing
    override func viewWillAppear(animated: Bool) {
        UIApplication.sharedApplication().statusBarStyle = .LightContent
    }
    
    // Switch to the default status bar style when this view is disappearing
    override func viewWillDisappear(animated: Bool) {
        UIApplication.sharedApplication().statusBarStyle = .Default
    }
    
    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
        // Dispose of any resources that can be recreated.
    }
    
    // Log messages by printing them on the UILabel.
    func logMessage(message: String) {
        // Run the logic on the main thread.
        dispatch_async(dispatch_get_main_queue()) { () -> Void in
            self.label?.text = message
        }
    }
    
    // Event handler for when the button is tapped.
    @IBAction func sendMe(sender: UIButton) {
        // Trigger UI.
        self.button?.hidden = true
        self.spinner?.startAnimating()
        
        self.mailClient.sendMeWithCallback({ (success: Bool) -> Void in
            // Log a final message.
            self.logMessage(success
                ? "Check your mailbox!"
                : "Oops... something went wrong!")
            
            // Trigger UI on the main thread.
            dispatch_async(dispatch_get_main_queue()) { () -> Void in
                self.button?.hidden = false
                self.spinner?.stopAnimating()
            }
            }, logger: self.logMessage);
    }
}
