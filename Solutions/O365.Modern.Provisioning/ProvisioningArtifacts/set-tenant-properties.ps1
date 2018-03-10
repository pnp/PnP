# Set your own values here
$SiteCollectionUrl = "<your site collection url>"
$BotId = "<your bot id>"
$BotDirectLineSecret = "<your bot direct line secret>"

Connect-PnPOnline -Url $SiteCollectionUrl -UseWebLogin

# Set the environment settings in the tenant property bag
Set-PnPStorageEntity -Key "PnPGraphBot_BotId" -Value $BotId -Comment $Comment -Description "Bot ID"
Set-PnPStorageEntity -Key "PnPGraphBot_BotDirectLineSecret" -Value $BotDirectLineSecret -Comment $Comment -Description "Bot Direct Line Secret"
