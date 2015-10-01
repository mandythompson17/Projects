// Perfect Numbers // 

// A //
function perfectNumbers() {
    var num = parseInt(document.getElementById('myPerfectNum').value);
    var perfectResult = document.getElementById('perfectResult');
    if (num < 1) { //no negative numbers or decimals 
        perfectResult.innerHTML = "Please enter a positive integer.";
    }
    else {
        var divisors = new Array();
        var i = 2;
        while (i < num) {
            if (num % i === 0) {
                divisors.push(i);
                i++;
            }
            else {
                i++;
            }
        }
        var sum = 1;
        var r = 0;
        while (r < divisors.length) {
            sum += divisors[r];
            r++;
        }
        if (sum === num) {
            perfectResult.innerHTML = "That is a perfect number!";
        }
        else {
            perfectResult.innerHTML = "That is not a perfect number.";
        }
    }
}

// B //
function numerosPerfectos() {
    var perfecto = document.getElementById('perfecto');
    perfecto.innerHTML = "The perfect numbers between 1 and 10,000 are:<br><br>";
    var num = 2;
    var divisors = new Array();
    while (num <= 10000) {
        var i = 2;
        while (i < num) {
            if (num % i === 0) {
                divisors.push(i);
                i++;
            }
            else {
                i++;
            }
        }
        var sum = 1;
        var r = 0;
        while (r < divisors.length) {
            sum += divisors[r];
            r++;
        }
        if (sum === num) {
            perfecto.innerHTML += "<b>" + num + "</b><br><br>";
            divisors.length = 0;
            num++;
        }
        else {
            divisors.length = 0;
            num++;
        }
    }
}