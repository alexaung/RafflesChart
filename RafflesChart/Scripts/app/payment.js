var m = angular.module('main');


m.controller('paymentcontroller', ['$scope', '$http', function ($scope, $http) {

    //$scope.paymentdate = "";
   
   
    $scope.getPayments = function () {
       
        $scope.paymentdate = $("#paymentdate").val();
        //alert($scope.paymentdate.length);
        if ($scope.paymentdate == undefined || $scope.paymentdate.length == 0 || $scope.paymentdate.length == 23) {
            $http.get('/ManageSubscription/GetPaymentsJson', {
                params: {
                    paymentdate: $scope.paymentdate,
                    username: $scope.username,
                    useremail: $scope.useremail,
                    subscriptionname: $scope.subscriptionname,
                    paypalref:$scope.paypalref
                },
                headers: { 'Content-Type': 'application/json; charset=UTF-8' }
            })
            .success(function (data) {
                $scope.payments = data;
                console.log(data);
            });
        }
    };

    $scope.getPayments();
}]);