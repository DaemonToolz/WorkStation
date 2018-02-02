
angular.module('app').controller('AJS_HomeController', ['$scope', 'AJS_HomeService', function ($scope, AJS_HomeService) {
    $scope.name = "Infrgistics";
    AJS_HomeService.getData().then(function (result) {
        $scope.data = result;
    }).catch(function (error) {
        $scope.status = 'Unable to load customer data: ' + error.message;
        console.log($scope.status);
    });
}]);
