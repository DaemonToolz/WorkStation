(function () {
    'use strict';

    angular.module('app', [
        // Angular modules 
        'ngRoute'

        // Custom modules 

        // 3rd Party Modules

    ]).config(function ($routeProvider, $locationProvider) {
        $locationProvider.hashPrefix(''); 

        $routeProvider
            .when("/angulartest", {
                templateUrl: '/Home/AngularTest'
            })
            
    });
})();