(function () {
    angular.module('playApp').factory('testCarSvc', ['$q', function ($q) {
        var f = {};

        f.getYears = function() {
            var p = $q.defer();
            p.resolve(['1999', '2000', '2001', '2002', '2003']);
            return p.promise;
        }

        f.getMakes = function(year) {
            var p = $q.defer();
            switch (year) {
                case '1999':
                case '2000':
                    p.resolve(['honda', 'ford']);
                    break;
                case '2001':
                case '2002':
                case '2003':
                    p.resolve(['honda', 'chevy']);
                    break;
            }
            return p.promise;
        }

        f.getModels = function(year, make) {
            var p = $q.defer();
            switch (make) {
                case 'honda':
                    p.resolve(['civic', 'crv', 'odyssey']);
                    break;
                case 'ford':
                    p.resolve(['mustang', 'explorer']);
                    break;
                case 'chevy':
                    p.resolve(['volt', 'cruise']);
                    break;
            }
            return p.promise;
        }
 
        f.getTrims = function (year, make, model) {
            var p = $q.defer();
            p.resolve(['4-door', '2-door']);
            return p.promise;
        }

        f.getCars = function (year, make, model, trim) {
            var p = $q.defer();
            p.resolve([
                {
                    year: year,
                    make: make,
                    model: model,
                    trim: trim,
                    color: 'gold'
                },
                {
                    year: year,
                    make: make,
                    model: model,
                    trim: trim,
                    color: 'blue'
                }
            ]);
            return p.promise;
        }

        return f;
    }]);
})();