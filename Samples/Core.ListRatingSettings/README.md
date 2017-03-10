# Enable Ratings/Likes on List with CSOM #

### Summary ###
This sample lets you enable Social features like Rating/Likes on the List/Library. The dependency is on Publishing Feature. At the time of writing there is no method avaiable on Client SDK for .NET to set the Rating Functionality.
The implementation is a result of **reverse engineering** of  **Server Side Object Model**.



### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)

### Prerequisites ###
Publishing Feature

### Solution ###
Solution | Author(s)
---------|----------
Core.ListRatingSettings | Akhilesh Nirapure (**RapidCircle**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | March 29th 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# How TO USE? #


```C#

//  provide the clientcontext of target web 

var ratingEnabler = new RatingsEnabler(clientContext);


//  1. Library name as per locale
//  2. Experience Ratings/Likes

ratingEnabler.Enable("**ListNameHere**", VotingExperience.Ratings);

```


Steps Performed:

1. Validate if current web is publishing web, else skip processing.
2. Find List/library 
3. Add property to RootFolder of List/Library: key: *Ratings_VotingExperience* value: **Likes/Ratings**
4. Add Ratings & Likes fields to List/Library
5. Add selected experience field to default view, e.g. Ratings as shown in above code.


**Note:** The RatingsEnabler (constructor) takes dependency on Logger which for the sake of sample uses ConsoleLogger. You can implement your own logger as far as it is implementing ILogger interface. 

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.ListRatingSettings" />