layoutApp.controller('IndexController', ['$scope', function ($scope) {
    $scope.someLocalVariable = "Hello Index!";
    $scope.someAction = function () { alert("Hi from Index!"); }
}]);