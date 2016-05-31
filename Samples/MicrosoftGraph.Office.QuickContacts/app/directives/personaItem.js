(function() {
    angular.module('office365app')
        .directive('personaItem', personaItem);

    function personaItem() {
        return {
            restrict: 'C',
            link: link
        };

        function link($scope, element, attrs) {
            element.bind('click', function(event) {

                event.stopImmediatePropagation();

                var curPersona = angular.element(this);
                var curPersonaTasks = angular.element(this.querySelector('.persona-item-tasks'));

                if (!curPersona.hasClass('full') && !curPersonaTasks.hasClass('show')) {
                    curPersona.toggleClass('full');
                } else {
                    var curButton = angular.element(event.target);

                    if (curButton.hasClass('icon-close')) {
                        if (curPersonaTasks.hasClass('show')) {
                            curPersonaTasks.removeClass('show');
                        } else {
                            element.toggleClass('full');
                        }
                    } else {
                        return;
                    }
                }
            });
        }
    }
})();
