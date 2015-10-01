// Fizz Buzz //
function fizzbuzz() {
    var i = 1;
    var fizz = parseInt(document.getElementById('myFizz').value);
    var buzz = parseInt(document.getElementById('myBuzz').value);
    var fbResult = document.getElementById('fbResult');
    while (i <= 100) {

        if (i % fizz === 0 && i % buzz === 0) {
            fbResult.innerHTML += "<h3 class='fizzbuzz'>FizzBuzz</h3>";
        }

        else if (i % fizz === 0) {
            fbResult.innerHTML += "<h3 class='fizz'>Fizz</h3>";
        }

        else if (i % buzz === 0) {
            fbResult.innerHTML += "<h3 class='buzz'>Buzz</h3>";
        }

        else {
            fbResult.innerHTML += i + "\n";
        }
        i++;
    }
}