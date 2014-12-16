
angular.module('nblast', ['ngRoute'])
    .config(['$routeProvider',
        function($routeProvider) {
            'use strict';
            $routeProvider
                .when('/', {
                    templateUrl: 'partials/dashboard.html',
                    controller: 'indexController'
                })
                .when('/search', {
                    templateUrl: 'partials/search.html',
                    controller: 'searchController'
                })
                .otherwise({
                    redirectTo: '/'
                });
        }
    ]);