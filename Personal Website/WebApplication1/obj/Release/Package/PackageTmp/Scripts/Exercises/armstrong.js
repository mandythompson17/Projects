// Armstrong Numbers //

function armstrong() {
    var moonLanding = document.getElementById('moonLanding');
    moonLanding.innerHTML = "The three-digit Armstrong numbers are:<br><br>";
    var num = 100;
    while (num < 1000) {
        var numS = num.toString();
        var a = parseInt(numS.charAt(0));
        var b = parseInt(numS.charAt(1));
        var c = parseInt(numS.charAt(2));
        var sum = Math.pow(a, 3) + Math.pow(b, 3) + Math.pow(c, 3);
        if (sum === num) {
            moonLanding.innerHTML += "<b>" + num + "</b><br><br>";
            num++;
        }
        else {
            num++;
        }
    }
}