(function () {
    'use strict';

    angular
        .module('app')
        .controller('StatisticsController', StatisticsController);

    StatisticsController.$inject = ['UserService', '$rootScope', '$scope', '$routeParams', '$location', '$http'];
    function StatisticsController(UserService, $rootScope, $scope, $routeParams, $location, $http) {
      //var param1 = $routeParams.param1;
      console.log('hello');
      console.log($location.search().sensorId);
      var sensorId = $location.search().sensorId;
      $scope.sensorType = $location.search().sensorType;
      $scope.sensorId =  $location.search().sensorId;
      $scope.getSensor = function() {
        //$http.get("http://localhost:3000/sensor")
       $http.get("http://192.168.1.202/alcoserver/api/sensordata?sensorId="+sensorId)
        .then(function(response) {
          $scope.sensor = response.data;

          var xValues = [];
          var i=0;
          $scope.labels = [];
          for(var val in $scope.sensor.Values) {
            xValues[i] = i;
            $scope.labels[i] = i;
            i++;
          }

          $scope.series = ['Series A'];
          $scope.data = [
            $scope.sensor.Values
          ];
          $scope.onClick = function (points, evt) {
            console.log(points, evt);
          };
          $scope.datasetOverride = [{ yAxisID: 'y-axis-1' }];
          $scope.options = {
            scales: {
              yAxes: [
                {
                  id: 'y-axis-1',
                  type: 'linear',
                  display: true,
                  position: 'left'
                }
              ]
            }
          };
        });
      };
      $scope.getSensor();
      var t=setInterval(function() { $scope.getSensor();} ,1000);

};

})();
