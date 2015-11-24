(function () {
    angular.module('car-finder').controller('CarsController', ['carSvc', '$q', '$uibModal', function (carSvc, $q, $uibModal) {
        var vm = this;

        this.selected = {
            year: '',
            make: '',
            model: '',
            trim: '',
            filter: '',
            paging: true,
            page: 0,
            perPage: 10,
            sortColumn: 'id',
            sortDirection: '',
            sortByReverse: true
        }

        this.options = {
            years: '',
            makes: '',
            models: '',
            trims: ''
        }


        this.id = {
            id: ''
        };

        this.car = {};

        this.cars = [];

        this.unpagedCarsCount = 0;

        this.loading = false;

        this.getYears = function () {
            carSvc.getYears().then(function (data) {
                vm.options.years = data;
            });
        }

        this.getMakes = function () {
            vm.selected.make = '';
            vm.options.makes = '';
            vm.selected.model = '';
            vm.options.models = '';
            vm.selected.trim = '';
            vm.options.trims = '';
            vm.selected.paging = true;
            vm.selected.page = 0;
            vm.selected.perPage = 10;
            vm.cars = [];
            vm.selected.sortColumn = 'make';
            vm.selected.sortByReverse = false;

            carSvc.getMakes(vm.selected).then(function (data) {
                vm.options.makes = data;
            });
            vm.getCars();
        }

        this.getModels = function () {
            vm.selected.model = '';
            vm.options.models = '';
            vm.selected.trim = '';
            vm.options.trims = '';
            vm.selected.paging = true;
            vm.selected.page = 0;
            vm.selected.perPage = 10;
            vm.cars = [];
            vm.selected.sortColumn = 'model_name';
            vm.selected.sortByReverse = false;
            carSvc.getModels(vm.selected).then(function (data) {
                vm.options.models = data;
            });
            vm.getCars();
        }

        this.getTrims = function () {
            vm.selected.trim = '';
            vm.options.trims = '';
            vm.selected.paging = true;
            vm.selected.page = 0;
            vm.selected.perPage = 10;
            vm.cars = [];
            vm.selected.sortColumn = 'model_trim';
            vm.selected.sortByReverse = false;

            carSvc.getTrims(vm.selected).then(function (data) {
                vm.options.trims = data;
            });
            vm.getCars();
        }

        this.getCars = function () {
            if (!vm.loading) {
                vm.loading = true;
                var s = angular.copy(vm.selected);
                s.page ++;

                $q.all([carSvc.getCars(s), carSvc.getCarCount(s)]).then(function (data) {
                    console.log(data[1]);
                    vm.cars = data[0];
                    vm.unpagedCarsCount = data[1];
                    vm.loading = false;
                });
            }
        }

        this.getCarCount = function () {
            carSvc.getCarCount(vm.selected).then(function (data) {
                vm.unpagedCarsCount = data;
            });
        }

        this.getCarDetails = function () {
            carSvc.getCarDetails(vm.id).then(function (data) {
                vm.car = data;
            })
        }

        this.open = function (id) {
            console.log("Id in open " + id);
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: 'carModal.html',
                controller: 'carModalCtrl as cm',
                size: 'lg',
                resolve: {
                    car: function () {
                        return carSvc.getCarDetails(id);
                    }
                }
            });
        }

        this.getYears();

    }]);

    angular.module('car-finder').controller('carModalCtrl', function ($uibModalInstance, car) {
        var vm = this;

        vm.car = car;

        vm.ok = function () {
            $uibModalInstance.close();
        }

        vm.cancel = function () {
            $uibModalInstance.dismiss();
        }

    });

})();