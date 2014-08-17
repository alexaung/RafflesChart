var m = angular.module('main');
console.log(m);
var SOURCE_MARKETS = { 'SGX': false, 'HKEX': false, 'AMEX': false, 'NYSE': false, 'NASD': false };
var SOURCE_INDICATORS = { 'MACD': false, 'Stochastic': false, 'RSI': false };
var SOURCE_PATTERN_SCANNERS = { 'triangle': false, 'wedge': false };
var SOURCE_SCANNERS = { 'breakout': false, 'MACD': false };

m.controller('SchemeController', ['$scope', '$http', function ($scope, $http) {
   
    $scope.selectedUsers = 'aaa@123.com\nuser@xyz.com';
        
        console.log('scheme controller loaded');
        
        function initvars() {
            var source = {};
            $scope.sourcemarkets = SOURCE_MARKETS;
            $scope.sourceindicators = SOURCE_INDICATORS;
            $scope.sourcepatternscanners = SOURCE_PATTERN_SCANNERS;
            $scope.sourcescanners = SOURCE_SCANNERS;

        }
        $scope.schemelist = [];

        initvars();


        $scope.schemeid = 0;

        console.log('sourcemarkets:' + $scope.sourcemarkets);
        $scope.usermarkets = ''
         listSchemes();
        $scope.selectedScheme = {};


        //$scope.$watch($scope.selectedUsers, function () {
        //    console.log($scope.selectedUsers);
        //});

        $scope.$watchCollection('sourcemarkets', function (nval, oval) {
            var mkts = [];

            $.each(nval, function (k, val) {

                if (val == true) {
                    mkts.push(k);
                }
            });
            $scope.usermarkets = mkts.join(';');
            console.log('markets:' + $scope.usermarkets);
        });

        $scope.$watchCollection('sourceindicators', function (nval, oval) {
            var mkts = [];

            $.each(nval, function (k, val) {

                if (val == true) {
                    mkts.push(k);
                }
            });
            $scope.userindicators = mkts.join(';');
            console.log('userindicators:' + $scope.userindicators);
        });

        $scope.$watchCollection('sourcepatternscanners', function (nval, oval) {
            var mkts = [];

            $.each(nval, function (k, val) {

                if (val == true) {
                    mkts.push(k);
                }
            });
            $scope.patternscanners = mkts.join(';');
            console.log('sourcePatternScanners:' + $scope.patternscanners);
        });

        $scope.$watchCollection('sourcescanners', function (nval, oval) {
            var mkts = [];

            $.each(nval, function (k, val) {

                if (val == true) {
                    mkts.push(k);
                }
            });
            $scope.scanners = mkts.join(';');
            console.log('sourceScanners:' + $scope.scanners);
        });

        $scope.addScheme = function () {

            if ($.isEmptyObject($scope.name)) {
                return;
            }
            var schm = {};

           
            schm.name = $scope.name;
            schm.description = $scope.description;
            schm.usermarkets = $scope.usermarkets;
            schm.userindicators = $scope.userindicators;
            schm.userbullbeartest = $scope.userbullbeartest;
            schm.userbacktest = $scope.userbacktest;
            schm.patternscanners = $scope.patternscanners;
            schm.scanners = $scope.scanners;
            schm.isscanner = $scope.isscanner;
            console.log('input scheme:' + schm);
            addDbScheme(schm)
        }

        function sample() {
            $http.get('/schemes/sample',
                {
                    headers: { 'Content-Type': 'application/json; charset=UTF-8' }
                })
                .success(function (data) {
                    
                    console.log('sanoke db scheme:' );
                    console.log(data)
                })
            .error(function (data) {
                console.log('error add db scheme:' + data);
            });
        }

        sample();
        function addDbScheme(schm) {
           
            var dbobj = {};
            dbobj.Name = schm.name;
            dbobj.Description = schm.description;
            dbobj.Markets = schm.usermarkets;
            dbobj.Indicators = schm.userindicators;
            dbobj.BullBearTests = schm.userbullbeartest;
            dbobj.BackTests = schm.userbacktest;
            dbobj.PatternScanners = schm.patternscanners;
            dbobj.Scanners = schm.scanners;
            console.log(JSON.stringify(dbobj));
           
            $http.post('/schemes/create',
               dbobj)
                .success(function (data) {
                    console.log('add db scheme:' + data);
                    clearScheme();
                    listSchemes();
                })
            .error(function (data) {
                console.log('error add db scheme:' + data);
            });

        }
        $scope.resultActivate = [];
        $scope.activateDbScheme = function () {
            var dbact = {};

            console.log('selected scheme :');

            console.log($scope.selectedScheme);

            dbact = {
                'schemeId': $scope.selectedScheme.schemeid
                      , 'userupload': $scope.selectedUsers
            };

            console.log(dbact);
            $http.get('/schemesa/schemeactivate',
                {
                    params: dbact,
                    headers: { 'Content-Type': 'application/json; charset=UTF-8' }
                })
                .success(function (data) {
                    console.log('activate db scheme:' + data.retValue);
                    $scope.resultActivate = data.retValue;
                })
            .error(function (data) {
                console.log('error activate db scheme:' + data);
            });
        }
        $scope.selectedFunc = 'Markets';
        $scope.functionSources = ['Markets', 'Indicators', 'Pattern Scanners', 'Scanners'];

        $scope.selectedVal = '';
        $scope.activateDbFunction = function () {
            var dbact = {};

            console.log('selected function :');

            console.log($scope.selectedFunc);

            dbact = {
                'fn': $scope.selectedFunc   ,
                'val': $scope.selectedVal,
                'userupload': $scope.selectedUsers
            };

            console.log(dbact);
            $http.get('/schemesa/functionactivate',
                {
                    params: dbact,
                    headers: { 'Content-Type': 'application/json; charset=UTF-8' }
                })
                .success(function (data) {
                    console.log('activate db function:' + data.retValue);
                    $scope.resultActivate = data.retValue;
                })
            .error(function (data) {
                console.log('error activate db function:' + data);
            });
        }

        function clearScheme() {
            $scope.added = true;
            $scope.addedMsg = "New scheme successfully created";

            $scope.name = "";
            $scope.description = "";
            $scope.usermarkets = "";
            $scope.userindicators = "";
            $scope.userbullbeartest = "";
            $scope.userbacktest = "";
            $scope.patternscanners = "";
            console.log("before clear SOURCE_MARKETS:" + SOURCE_MARKETS);
            initvars();
            $scope.scanners = "";
            $scope.isscanner = false;
            $scope.showScheme = false;
        }

        function listSchemes() {
            $scope.schemelist = [];
            $http.get('/schemes/schemelist',
               {
                   headers: { 'Content-Type': 'application/json; charset=UTF-8' }
               })
               .success(function (data) {
                   console.log('list db schemes:' );
                   console.log( data);
                   $.each(data, function (idx, dataval) {
                       var schm = {};
                       schm.schemeid = dataval.Id;
                       schm.name = dataval.Name;
                       schm.description = dataval.Description;
                       schm.usermarkets = dataval.Markets;
                       schm.userindicators = dataval.Indicators;
                       schm.userbullbeartest = dataval.BullBearTests;
                       schm.userbacktest = dataval.BackTests;
                       schm.patternscanners = dataval.PatternScanners;
                       schm.scanners = dataval.Scanners;
                       $scope.schemelist.push(schm);

                   });
                   console.log('length scheme list:' + $scope.schemelist.length);
                   if ($scope.schemelist.length > 0) {
                       $scope.selectedScheme = $scope.schemelist[0];
                       console.log('selected scheme:');
                       console.log($scope.selectedScheme);
                   }
               })
           .error(function (data) {
               console.log('error list db scheme:' + data);
           });
        }

        $scope.removeScheme = function (id) {
            $http.post('/schemes/deletea',
               { "id": id }
                )
                .success(function (data) {
                    console.log('delete db scheme:')
                    console.log(data);
                    listSchemes();
                })
            .error(function (data) {
                console.log('error delete db scheme:' + data);
            });
        };
    }]);