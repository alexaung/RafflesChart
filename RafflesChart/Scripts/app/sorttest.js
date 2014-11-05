var m = angular.module('main');


m.controller('sorttest', ['$scope', '$http', function ($scope, $http) {
    var data1 = {name:'jack',age:33};
    var data2 = { name: 'simon', age: 6 };
    $scope.sortdata = [data1,data2];
   
    }]);

m.directive('orderCtl',function () {
	return{
		scope:{
			param:'=userParam',
			dir:'='
		},
		template: ' {{dir}} <img src={{src}}" />  User param: {{param}}'
		,
		link:function (scope,el,attrs) {
			el.css("background-color","red");
			scope.dir = "up";
			//alert(el.html);
			scope.src="http://placehold.it/50x50/ff3377";
			//el.text(attrs.valtwo);
			el.bind('mouseenter', function  () {
				//el.css("background-color" , "blue");
			});

			el.bind('click',function  () {	
			//alert(scope.dir);			
				scope.dir= scope.dir=="up"?"down":"up";
				
			    //el.find('img').attr('src' , 'http://placehold.it/50x150/ff8877');
				if(scope.dir=="up"){
				    scope.src = "http://placehold.it/50x150/ff8877";
				} else {
				    scope.src = "http://placehold.it/50x150/aa88bb";
				}
				scope.$apply();
			})
		}
	};
});
	// body...
