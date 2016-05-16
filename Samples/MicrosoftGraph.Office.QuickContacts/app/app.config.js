(function () {
	var office365app = angular.module('office365app');
	office365app.constant('appId', '00000000-0000-0000-0000-000000000000');
    office365app.constant('graphBetaUrl', 'https://graph.microsoft.com/beta');
    office365app.constant('graphUrl', 'https://graph.microsoft.com/v1.0');
    office365app.constant('sharePointUrl', 'https://contoso.sharepoint.com');
})();