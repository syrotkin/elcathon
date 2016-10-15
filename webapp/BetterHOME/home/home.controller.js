(function () {
    'use strict';

    angular
        .module('app')
        .controller('HomeController', HomeController);

    HomeController.$inject = ['UserService', '$rootScope', '$http', '$scope'];
    function HomeController(UserService, $rootScope, $http, $scope) {
      $scope.getSensors = function() {
        $http.get("http://localhost:3000/sensors")
        .then(function(response) {
          $scope.sensors = response.data;
        });
      }
      $scope.getSensors();

      $scope.capitalizeFirstLetter = function(string) {
                return string.charAt(0).toUpperCase() + string.slice(1);
            }
      $scope.addNewSensor = function () {
        if($scope.newSensor != null) {
          $http.get("http://localhost:3000/addsensor")
            .then(function(response) {
              console.log(response.data);
              $scope.getSensors();
            });
        }
      }
    }

})();
