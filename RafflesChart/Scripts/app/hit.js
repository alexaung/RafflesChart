var m = angular.module('main');


m.controller('hitcontroller', ['$scope', '$http', function ($scope, $http) {

    $scope.visitdate = "";
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
    $scope.visitdate = $("#visitdate").val();
    if($scope.visitdate == undefined || $scope.visitdate.length==23){
            $http.get('/Home/GetHitsJson', {
                params: {dt:$scope.visitdate},
                headers: { 'Content-Type': 'application/json; charset=UTF-8' }
            })
            .success(function (data) {
                $scope.hits = data;
                console.log(data);
            });
        }
    };

    $scope.getHits();
}]);