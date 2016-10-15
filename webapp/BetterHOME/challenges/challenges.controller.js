(function () {
    'use strict';

    angular
        .module('app')
        .controller('ChallengesController', ChallengesController);

    ChallengesController.$inject = ['UserService', '$rootScope','$http', '$scope'];
    function ChallengesController(UserService, $rootScope, $http, $scope) {
        $scope.getChallengeTypes = function() {
          $http.get("http://192.168.1.202/alcoserver/api/getChallengeTypes")
          .then(function(response) {
            $scope.challenges = response.data;
          });
        }
        $scope.getChallengeTypes();

        $scope.capitalizeFirstLetter = function(string) {
                  return string.charAt(0).toUpperCase() + string.slice(1);
              }
    }

})();
