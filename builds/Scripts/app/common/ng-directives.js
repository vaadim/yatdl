/// <reference path="~/Scripts/angular.js"/>

angular.module("app.commonDirectives", ["ngAnimate"])
    .directive("loadingContainer", function () {
        return {
            restrict: 'A',
            scope: false,
            link: function (scope, element, attrs) {
                var loadingLayer = angular.element('<div class="loading"><div style="left:50%; top:50%; margin-left:-30px; margin-top:-10px;"><i class="fa fa-2x fa-spinner fa-spin"></i><span> Загрузка...</span></div></div>');
                element.append(loadingLayer);
                element.addClass('loading-container');
                scope.$watch(attrs.loadingContainer, function (value) {
                    loadingLayer.toggleClass('ng-hide', !value);
                });

            }

        };
    })