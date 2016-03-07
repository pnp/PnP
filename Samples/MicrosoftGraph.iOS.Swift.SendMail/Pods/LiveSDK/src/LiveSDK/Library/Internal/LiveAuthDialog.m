//
//  LiveAuthDialog.m
//  Live SDK for iOS
//
//  Copyright 2015 Microsoft Corporation
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//  
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//


#import "LiveAuthDialog.h"
#import "LiveAuthHelper.h"

@implementation LiveAuthDialog

@synthesize webView, canDismiss, delegate = _delegate;

- (id)initWithNibName:(NSString *)nibNameOrNil 
               bundle:(NSBundle *)nibBundleOrNil
             startUrl:(NSURL *)startUrl 
               endUrl:(NSString *)endUrl
             delegate:(id<LiveAuthDialogDelegate>)delegate
{
    self = [super initWithNibName:nibNameOrNil 
                           bundle:nibBundleOrNil];
    if (self) 
    {
        _startUrl = [startUrl retain];
        _endUrl =  [endUrl retain];
        _delegate = delegate;
        canDismiss = NO;
    }
    
    return self;
}

- (void)dealloc 
{
    [_startUrl release];
    [_endUrl release];
    [webView release];
    
    [super dealloc];
}

- (void)didReceiveMemoryWarning
{
    // Releases the view if it doesn't have a superview.
    [super didReceiveMemoryWarning];
    
    // Release any cached data, images, etc that aren't in use.
}

#pragma mark - View lifecycle

// Implement viewDidLoad to do additional setup after loading the view, typically from a nib.
- (void)viewDidLoad
{
    [super viewDidLoad];
    
    self.webView.delegate = self;
    
    // Override the left button to show a back button
    // which is used to dismiss the modal view    
    UIImage *buttonImage = [LiveAuthHelper getBackButtonImage]; 
    //create the button and assign the image
    UIButton *button = [UIButton buttonWithType:UIButtonTypeCustom];
    [button setImage:buttonImage 
            forState:UIControlStateNormal];
    //set the frame of the button to the size of the image
    button.frame = CGRectMake(0, 0, buttonImage.size.width, buttonImage.size.height);
    
    [button addTarget:self 
               action:@selector(dismissView:) 
     forControlEvents:UIControlEventTouchUpInside]; 
    
    //create a UIBarButtonItem with the button as a custom view
    self.navigationItem.leftBarButtonItem = [[[UIBarButtonItem alloc]
                                              initWithCustomView:button]autorelease];  
    
    //Load the Url request in the UIWebView.
    NSURLRequest *requestObj = [NSURLRequest requestWithURL:_startUrl];
    [webView loadRequest:requestObj];    
}

- (void) viewDidAppear:(BOOL)animated
{
    [super viewDidAppear:animated];
    // We can't dismiss this modal dialog before it appears.
    canDismiss = YES;
}

- (void)viewDidUnload
{
    [super viewDidUnload];
    // Release any retained subviews of the main view.
    // e.g. self.myOutlet = nil;
}

- (void)viewDidDisappear:(BOOL)animated
{
    [super viewDidDisappear:animated];
    [_delegate authDialogDisappeared];
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    // This mehtod is supported in iOS 5. On iOS 6, this method is replaced with
    // (BOOL)shouldAutorotate and (NSUInteger)supportedInterfaceOrientations
    // We don't implement the iOS 6 rotating methods because we choose the default behavior.
    // The behavior specified here for iOS 5 is consistent with iOS6 default behavior. 
    // iPad: Rotate to any orientation
    // iPhone: Rotate to any orientation except for portrait upside down.
    return ([LiveAuthHelper isiPad] || (interfaceOrientation != UIInterfaceOrientationPortraitUpsideDown));
}

// User clicked the "Cancel" button.
- (void) dismissView:(id)sender 
{
    [_delegate authDialogCanceled];
}

#pragma mark UIWebViewDelegate methods

- (BOOL)webView:(UIWebView *)webView shouldStartLoadWithRequest:(NSURLRequest *)request 
 navigationType:(UIWebViewNavigationType)navigationType 
{
    NSURL *url = [request URL];
    
    if ([[url absoluteString] hasPrefix: _endUrl]) 
    {
        [_delegate authDialogCompletedWithResponse:url];
    }
    
    // Always return YES to work around an issue on iOS 6 that returning NO may cause
    // next Login request on UIWebView to hang.
    return YES;
}

- (void)webViewDidFinishLoad:(UIWebView *)webView 
{
}

- (void)webView:(UIWebView *)webView didFailLoadWithError:(NSError *)error 
{
    // Ignore the error triggered by page reload
    if ([error.domain isEqualToString:@"NSURLErrorDomain"] && error.code == -999)
        return;
    
    // Ignore the error triggered by disposing the view.
    if ([error.domain isEqualToString:@"WebKitErrorDomain"] && error.code == 102)
        return;
    
    [_delegate authDialogFailedWithError:error];
}


- (void)webViewDidStartLoad:(UIWebView *)webView
{
}

@end
