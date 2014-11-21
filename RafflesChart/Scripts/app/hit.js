var m = angular.module('main');


m.controller('hitcontroller', ['$scope', '$http', function ($scope, $http) {
    $scope.sorthits = function (skey) {
        if ($scope.sortOrder == skey) {
            $scope.sortOrder = '-' + skey;
        } else {
            $scope.sortOrder = skey;
        }
    }

    $scope.sorticon = function (icon) {
        var skey = $scope.sortOrder;
        if (skey.replace("-", "") == icon) {
            if (skey[0] == "-") {
                return "fa fa-sort-desc";
            } else {
                return "fa fa-sort-asc";
            }
        }
        return "";
    };
   
    $scope.getHits = function () {


        $http.get('/Home/GetHitsJson', {
            headers: { 'Content-Type': 'application/json; charset=UTF-8' }
        })
        .success(function (data) {
            $scope.hits = data;
            console.log(data);
        });
    };

    $scope.getHits();
}]);