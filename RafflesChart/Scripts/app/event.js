var m = angular.module('main');

m.controller('EventController', ['$scope', '$http', function ($scope, $http) {
   
   
        $scope.addDbEventUser = function (ue) {
           
            var dbobj = {};
            dbobj.eventId = ue;
           
           
            $http.post('/events/register',
               dbobj)
                .success(function (data) {
                   
                    console.log('add db user event:' + data);
                    alert('' + data);
               	    //location.reload();		 	
		})
            .error(function (data) {
                console.log('error add db scheme:' + data);
            });

        }
       
           
    }]);
