(function () {
    angular.module('playApp').controller('CarsController', ['testCarSvc', function (testCarSvc) {
        var vm = this;

        this.selected = {
            year: '',
            make: '',
            model: '',
            trim: ''
        }

        this.options = {
            years: '',
            makes: '',
            models: '',
            trims: ''
        }

        this.cars = [];

        this.getYears = function () {
           testCarSvc.getYears().then(function (data) {
                vm.options.years = data;
            });
            vm.getCars();
        }

        this.getMakes = function () {
            vm.selected.make = '';
            vm.options.makes = '';
            vm.selected.model = '';
            vm.options.models = '';
            vm.selected.trim = '';
            vm.options.trims = '';
            vm.cars = [];

            testCarSvc.getMakes(vm.selected.year).then(function (data) {
                vm.options.makes = data;
            });
            vm.getCars();
        }

        this.getModels = function () {
            vm.selected.model = '';
            vm.options.models = '';
            vm.selected.trim = '';
            vm.options.trims = '';
            vm.cars = [];
            testCarSvc.getModels(vm.selected.year, vm.selected.make).then(function (data) {
                vm.options.models = data;
            });
            vm.getCars();
        }

        this.getTrims = function () {
            vm.selected.trim = '';
            vm.options.trims = '';
            vm.cars = [];

            testCarSvc.getTrims(vm.selected.year, vm.selected.make, vm.selected.model).then(function (data) {
                vm.options.trims = data;
            });
            vm.getCars();
        }

        this.getCars = function () {
            vm.cars = [];
            testCarSvc.getCars(vm.selected.year, vm.selected.make, vm.selected.model, vm.selected.trim).then(function (data) {
                vm.cars = data;
            });
        }

        this.getYears();

    }]);
})();