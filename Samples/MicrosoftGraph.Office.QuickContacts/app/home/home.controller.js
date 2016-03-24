(function() {
    'use strict';

    angular.module('office365app')
        .controller('homeController', ['dataService', '$rootScope', homeController]);

    function homeController(dataService, $rootScope) {
        var vm = this; // jshint ignore:line
        vm.searchQuery = '';
        vm.searching = false;
        vm.hasSearched = false;
        vm.error = null;

        vm.search = search;
        vm.canSearch = canSearch;
        vm.showContactActions = showContactActions;

        vm.contacts = [];
        vm.nextLink = null;

        svg4everybody();

        function canSearch() {
            return vm.searchQuery !== null && vm.searchQuery.trim().length > 2;
        }

        function search() {
            vm.searching = true;
            vm.hasSearched = true;
            vm.contacts.length = 0;
            vm.nextLink = null;
            vm.error = null;

            dataService.searchForContacts(vm.searchQuery)
                .then(function(peopleInfo) {
                    vm.contacts = peopleInfo.people;
                    vm.nextLink = peopleInfo.nextLink;

                    // try loading additional contact information for all contacts
                    for (var i = 0; i < vm.contacts.length; i++) {
                        var contact = vm.contacts[i];

                        // a contact can have multiple e-mail addresses and because
                        // we can't guess which one would correspond to the user
                        // try loading additional information using every known e-mail
                        // address
                        for (var j = 0; j < contact.emailAddresses.length; j++) {
                            dataService.getUserDetails(contact.emailAddresses[j].address)
                                .then(function(userDetails) {
                                    if (userDetails) {
                                        var c = getContact(vm.contacts, userDetails.email);

                                        if (c !== null) {
                                            c.businessPhones = userDetails.businessPhones;
                                            c.photoUrl = userDetails.photoUrl;
                                        }
                                    }
                                });
                        }
                    }
                    
                    // try load profile URLs. available only for organizational contacts
                    dataService.getProfilesUrls(vm.contacts).then(function(profilesUrls) {
                        for (var i = 0; i < profilesUrls.length; i++) {
                            var profileInfo = profilesUrls[i];
                            
                            for (var j = 0; j < vm.contacts.length; j++) {
                                var contact = vm.contacts[j];
                                var profileSet = false;
                                
                                for (var k = 0; k < contact.emailAddresses.length; k++) {
                                    if (contact.emailAddresses[k].address === profileInfo.email) {
                                        contact.profileUrl = profileInfo.profileUrl;
                                        profileSet = true;
                                        break;
                                    }
                                }
                                
                                if (profileSet) {
                                    break;
                                }
                            }
                        }
                    });
                }, function(err) {
                    vm.error = err;
                })
                .finally(function() {
                    vm.searching = false;
                    $rootScope.$broadcast('searchFinished');
                });
        }

        function getContact(contacts, emailAddress) {
            var contact = null;

            for (var i = 0; i < contacts.length; i++) {
                var c = contacts[i];

                if (c.emailAddresses != null &&
                    c.emailAddresses.length > 0) {
                    for (var j = 0; j < c.emailAddresses.length; j++) {
                        if (c.emailAddresses[j].address === emailAddress) {
                            contact = c;
                            break;
                        }
                    }

                    if (contact !== null) {
                        break;
                    }
                }
            }

            return contact;
        }

        function showContactActions(event) {

            angular.element(event.srcElement).addClass('show');

        }

    }

})();
