# Utility to migrate Discussion List(s) to/from SharePoint Online #

## Summary ##
This utility helps you migrate Discussion Lists from one SharePoint Online site to another. It connects and downlods the discussion topics, replies, and attachements in the list locally. This downloaded content can then be migrated by running the utility again in target mode.

## Features ##
* Download the discussion threads (and attachements) locally
* View your discussion list content in a DataGrid
* Export data in an XML format
* Real time progress of the download/upload operations
* Ability to select a target list
* Ability to replace missing users with a specified account in *app.config*
* Logging of migration activities (configurable via *app.config*)
