var m = angular.module('main');


m.controller('sorttest', ['$scope', '$http', function ($scope, $http) {
    var data1 = {name:'jack',age:33};
    var data2 = { name: 'simon', age: 6 };
    $scope.sortdata = [data1,data2];
   
    }]);