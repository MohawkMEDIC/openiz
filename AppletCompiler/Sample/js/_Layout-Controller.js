var layoutApp = angular.module('layout', []);

layoutApp.controller('LayoutController', ['$scope', function ($scope) {

    $scope.someVariable = "Hello!";
    $scope.someAction = function () { alert("Hi from layout!"); }
}]);