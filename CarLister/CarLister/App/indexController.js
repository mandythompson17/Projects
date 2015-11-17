angular.module('playApp').controller('indexController',
['$interval', '$timeout', function ($interval, $timeout) {
    var self = this;
    this.value = 1;

    /*var line1 = ["Sometimes", "the", "most", "beautiful", "thing,"];
    var line2 = ["the", "most", "innocent", "thing,"];
    var line3 = ["and", "many", "of", "those", "dreams"];
    var line4 = ["pass", "us", "by,"];
    var line5 = ["keep", "passing", "me", "by."];
    this.Angel = [line1, line2, line3, line4, line5];
    this.currentLine = 0;
    this.currentWord = 0;
    this.currentLyric = self.Angel[self.currentLine][self.currentWord];

    this.lyrics = function() {
        if (self.currentWord < self.Angel[self.currentLine].length - 1) {
            self.currentWord++;
        }
        else {
            if (self.currentLine < self.Angel.length - 1) {
                self.currentLine++;
            }

        }
    }*/

    this.Angel = ["Sometimes", "the", "most", "beautiful", "thing,", "the", "most", "innocent", "thing,", "and", "many", "of", "those", "dreams", "pass", "us", "by,", "keep", "passing", "me", "by."]
    this.currentWord = 0;
    this.currentLyric = self.Angel[self.currentWord];

    this.lyrics = function() {
        if (self.currentWord < self.Angel.length - 1) {
            self.currentWord++;
            self.currentLyric = self.Angel[self.currentWord];
        }
        else {
            self.currentWord = 0;
            self.currentLyric = self.Angel[self.currentWord];
        }
    }

    $interval(function () {
        self.lyrics();
    }, 1000);

    this.cars = [
        {
            make: 'Honda',
            year: '2012',
            model: 'CRV',
            trim: '2.4 LX'
        },
        {
            make: 'Ford',
            year: '2008',
            model: 'Explorer',
            trim: 'Sport Trac XLT'
        },
        {
            make: 'Honda',
            year: '2009',
            model: 'Civic',
            trim: 'Coupe EX-L'
        },
        {
            make: 'Ford',
            year: '2003',
            model: 'Mustang',
            trim: 'Automatic'
        },
        {
            make: 'Chrysler',
            year: '2005',
            model: 'PT Cruiser',
            trim: 'Convertible GT'
        },
        {
            make: 'Chevrolet',
            year: '2011',
            model: 'Camaro',
            trim: 'LS'
        }
    ]
    



}]);