# -----------------
# Global parameters
# -----------------

# This key is used to gather usage data for future improvements (no sensitive date is collected)
$AppInsightsInstrumentationKey = "7496c244-ff35-460d-8acf-f00c8d60ad9c"

# This name will be used to create a separated folder in the style library and the master page catalog.
# If you change this name, don't forget to update :
# - Links in the master page (CSS and JS files)
# - Web Parts XML contents on the provisioning template (display templates paths)
# - Display templates files (relative paths to hover panel display template)
$AppFolderName = "PnP"

# Available languages
$Languages = @(

	[PSCustomObject]@{
        Title="English";
        Label="en";
        LCID=1033;
        TemplateFileName="SubSiteTemplateEN.xml";
        SearchNavigation= @(
            @{
                Title= "Intranet";
                Url="Search.aspx?icon=fa-globe"
            };
            @{
                Title= "Documents";
                Url="SearchDocuments.aspx?icon=fa-book"
            };
            @{
                Title= "Directory";
                Url="SearchPeople.aspx?icon=fa-users"
            };
        )
    }
	[PSCustomObject]@{
        Title="French";
        Label="fr";
        LCID=1036;
        TemplateFileName="SubSiteTemplateFR.xml";
        SearchNavigation= @(
            @{
                Title= "Intranet";
                Url="Recherche.aspx?icon=fa-globe"
            };
            @{
                Title= "Documents";
                Url="RechercheDocuments.aspx?icon=fa-book"
            };
            @{
                Title= "Bottin";
                Url="RecherchePersonnes.aspx?icon=fa-users"
            };
        )
    }
)