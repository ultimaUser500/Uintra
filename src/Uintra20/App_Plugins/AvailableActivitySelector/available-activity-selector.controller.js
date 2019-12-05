﻿(function (angular) {
    'use strict';

    function controller($scope, availableActivityService) {
        const self = this;
        console.log($scope.model);
        function onInit() {
            availableActivityService.getAvailableActivityTypes().then(res => {
                self.availableActivityTypes = JSON.parse(res.data);
            });
        }

        onInit();

    }

    controller.$inject = [
        '$scope',
        'availableActivityService'
    ];

    angular.module('umbraco').controller('availableActivityController', controller);
})(angular);