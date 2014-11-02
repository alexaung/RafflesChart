var m = angular.module('main');


m.controller('usercontroller', ['$scope', '$http', function ($scope, $http) {

    
    //$scope.sortOrder = 'Name';
    $scope.sortusers = function (skey) {
        console.log('sorting');
        if ($scope.sortOrder == skey) {
            $scope.sortOrder = '-' + skey;
        } else {
            $scope.sortOrder = skey;
        }
    };
    $scope.PickedCount = function () {
        if ($scope.users == null) {
            return 0;
        }
        var pick = $scope.users.filter(function (val) {
            return val.Picked;
        });
        return pick.length;
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

    $scope.PickedAll = function () {
        if ($scope.users == null) {
            return ;
        }
        var p = !$scope.allflag;
        var pick = $scope.users.forEach(function (val) {
            val.Picked=p;
        });        
    }
    $scope.ApplyScheme = function () {
        var pick = $scope.users.filter(function (val) {
            return val.Picked;
        });
        var del = pick.map(function (pic) {
            return pic.Email;
        });
        var shcemeid = $('#pickscheme').val();
        console.log(shcemeid)
        $http.post('/Account/SchemePicked', { picked: del, scheme: shcemeid })
            .success(function (data) {               
                $scope.schemeconfirm = 0;
                $scope.searchUserByFilter();
            })
    }
    $scope.DeleteAll = function () {
        var pick = $scope.users.filter(function (val) {
            return val.Picked;
        });
        var del = pick.map(function (pic) {
            return pic.Email;
        });
        console.log(del)
        $http.post('/Account/DeletePicked', { picked: del })
            .success(function (data) {
               
                $scope.pickconfirm = 0;
                $scope.searchUserByFilter();
            })
    }

    $scope.$watch('schemeDb', function (newVal, oldVal) {
        console.log(newVal);
        console.log(oldVal);
        $http.get('/Schemes/DetailJson?id=' + newVal)
       .success(function (data) {
           
           $scope.scheme = data;
           $("#ChartUserModel_CI_Add").prop("checked",data.CIAddFlag);
           $("#ChartUserModel_CustomIndicators").prop("checked",data.CustomIndicatorsFlag);
           $("#ChartUserModel_Scanner").prop("checked",data.ScannerFlag);
           $("#ChartUserModel_Scanner_Add").prop("checked",data.ScannerAddFlag);
           $("#ChartUserModel_Trend_Add").prop("checked",data.TrendAddFlag);
           $("#ChartUserModel_Signal_Add").prop("checked",data.SignalAddFlag);
           $("#ChartUserModel_Pattern_Add").prop("checked",data.PatternAddFlag);
           
           console.log(data);
       });
    });

   
   
    $scope.searchUserByFilter = function () {
        var dbobj = {};
        dbobj.Name = $("#Name").val();
        dbobj.Email = $("#Email").val();
        dbobj.PhoneNumber = $("#PhoneNumber").val();
        dbobj.SelectedScheme = $("#SelectedSchemeDD").val();
        
        $http.get('/Account/GetUsersJson', {
            params: dbobj,
            headers: { 'Content-Type': 'application/json; charset=UTF-8' }
        })
        .success(function (data) {
            $scope.users = data;
            console.log(data);
        });
    }
   

    }]);