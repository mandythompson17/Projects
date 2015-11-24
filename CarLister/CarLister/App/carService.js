(function () {
    angular.module('car-finder').factory('carSvc', ['$http', function ($http) {
        var f = {};

        f.getYears = function () {
            return $http.get('/api/Cars/GetYears').then(function (response) {
                return response.data;
            });
        }

        f.getMakes = function (selected) {
            return $http.post('/api/Cars/GetMakes', selected).then(function (response) {
                return response.data;
            });
        }

        f.getModels = function (selected) {
            return $http.post('/api/Cars/GetModels', selected).then(function (response) {
                return response.data;
            });
        }

        f.getTrims = function (selected) {
            return $http.post('/api/Cars/GetTrims', selected).then(function (response) {
                return response.data;
            });
        }

        f.getCars = function (selected) {
            return $http.post('/api/Cars/GetCars', selected).then(function (response) {
                return response.data;
            });
        }

        f.getCarCount = function (selected) {
            return $http.post('/api/Cars/GetCarCount', selected).then(function (response) {
                return response.data;
            });
        }

        f.getCarDetails = function (id) {
            var Id = {
                id: id
            }
            return $http.post('/api/Cars/GetCarDetails', Id).then(function (response) {
                return response.data;
            });
        }


        return f;
    }]);
})();