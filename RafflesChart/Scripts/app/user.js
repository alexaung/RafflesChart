var m = angular.module('main');


m.controller('usercontroller', ['$scope', '$http', function ($scope, $http) {

    
    $scope.sortOrder = 'Name';
    $scope.sortusers = function () {
        console.log('sorting');
        if ($scope.sortOrder == 'Name') {
            $scope.sortOrder = '-Name';
        } else {
            $scope.sortOrder = 'Name';
        }
    };

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

    $scope.getSchemeFromDb = function () {
        console.log('call db');
        $http.get('/Schemes/DetailJson?id=' + '29')
       .success(function (data) {
           $scope.scheme = data;
           $("#ApplicationUserModel_Name").val(data.Description);
           console.log(data);
       });
    };
   

    var dbobj = {};
    dbobj.Name = "";
    dbobj.Email ="";
        dbobj.PhoneNumber = "";
        dbobj.Captcha = "";
        dbobj.CaptchaChallenge = "";
        dbobj.EventId = 0;;
    $http.get('/Account/GetUsersJson', dbobj)
    .success(function (data) {
        $scope.users = data;
        console.log(data);
    });

    }]);