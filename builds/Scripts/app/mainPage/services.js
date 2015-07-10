/// <reference path="~/Scripts/angular.js"/>

// Must be defined outside
window.apiBaseUrl = window.apiBaseUrl ? window.apiBaseUrl : "";

(function () {

    angular.module("app.EntitiesServices", [])
        .service("CrudService", crudService)
        .service("UtilService", utilService);

    crudService.$inject = ["$http"];
    utilService.$inject = ["$http"];

    function crudService($http) {

        this.list = function (filter, sort, skip, take) {
            return $http.post(window.apiBaseUrl + "list", { filter: filter, sort: sort, skip: skip, take: take });
        };

        this.delete = function (id) {
            return $http.post(window.apiBaseUrl + "delete", { id: id });
        };

        this.saveForm = function (model) {
            return $http.post(window.apiBaseUrl + "save", { model: model });
        };
    }

    function utilService($http) {
        this.importanceList = function () {
            return $http({ cache: true, method: 'GET', url: window.apiBaseUrl + "importance" }).
                success(function (data, status) {
                }).error(function (data, status) {
                });
        };
    }

})();