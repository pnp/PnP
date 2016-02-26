#PnP JavaScript Core#
##Quick Start Guide##

Unsure where to begin with working on the project? This guide is designed to get you up and working quickly.

1. Install [Visual Studio Code](https://code.visualstudio.com/) - this is the development environment we will use. It is similar to a light weight Visual Studio designed for each editing of client file types such as .ts and .js.

2. Install [Node JS](https://nodejs.org/en/download/) - this provides two key capabilities; the first is the nodejs server which will act as our development server (think iisexpress), the second is npm a package manager (think nuget).

3. Install a console emulator of your choice, for Windows [Cmder](http://cmder.net/) is popular. If installing Cmder choosing the full option will allow you to use git for windows. Whatever option you choose we will refer in the rest of the guide to "console" as the thing you installed in this step.

4. Install the tslint extension in VS Code:
	1. Press Shift + Ctrl + "p" to open the command panel
	2. Begin typing "install extension" and select the command when it appears in view
	3. Begin typing "tslint" and select the package when it appears in view
	4. Restart Code after installation
	5. Open your console and type `npm install -g tslint` to globally install the npm package

5. Install typescript by typing the following code in your console `npm install -g typescript`

6. Install typings by typing the following code in your console `npm install -g typings`

7. Now we need to fork and clone the git repository. This can be done using your [console](https://help.github.com/articles/fork-a-repo/) or using your preferred method.

8. Once you have the code locally, navigate to the root of the project in your console. Type the following commands:
	1. `npm install` - installs all of the npm package dependencies (may take awhile the first time)
	2. `typings install` - installs the required typings files as defined in typings.json 

9. Type `gulp serve` and you should see the browser launch and an alert window display a random string of length 5.

Because we are watching the files, any changes will be reflected in the served files after a new build is complete. You can point from a script tag in your SharePoint site to the http://localhost:3000 site to use the files. Any changes you make will be reflected thanks to the watched build. 