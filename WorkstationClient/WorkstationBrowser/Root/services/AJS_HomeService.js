(function () {
    'use strict';

    angular
        .module('app')
        .factory('AJS_HomeService', AJS_HomeService);

    AJS_HomeService.$inject = ['$http'];

    function AJS_HomeService($http) {
        var service = {
            getData: getData
        };

        return service;

        function getData() {
            return $http.get('/Home/RandomTest');
        }
    }
})();