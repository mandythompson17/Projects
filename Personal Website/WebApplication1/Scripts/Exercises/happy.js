// Happy Numbers //
function happy() {
    var happyNumbers = document.getElementById('happyNumbers');
    happyNumbers.innerHTML = "The first five Happy numbers are:<br><br>";
    var x = 1;
    var y = x;
    var i = 1;
    while (i <= 5) {
        if (y === 1) {
            happyNumbers.innerHTML += "<b>" + x + "</b><br><br>";
            x++;
            y = x;
            i++;
        }
        else if (y === 4) {
            x++;
            y = x;
        }
        else {
            while (y !== 1 & y !== 4) {
                var num = y.toString();
                if (num.length === 1) {
                    y = parseInt(num);
                    y = Math.pow(y, 2);
                }
                else if (num.length === 2) {
                    var a = parseInt(num.charAt(0));
                    var b = parseInt(num.charAt(1));
                    y = Math.pow(a, 2) + Math.pow(b, 2);
                }
                else {
                    var a = parseInt(num.charAt(0));
                    var b = parseInt(num.charAt(1));
                    var c = parseInt(num.charAt(2));
                    y = Math.pow(a, 2) + Math.pow(b, 2) + Math.pow(c, 2);
                }
            }
        }
    }
}