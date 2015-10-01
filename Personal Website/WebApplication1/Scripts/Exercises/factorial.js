// Factorial // 

function factorial() {
    var numInput = parseInt(document.getElementById('myFactBox').value);
    var num = numInput;
    var i = num - 1;
    while (i !== 0) {
        num *= i;
        i--;
    }
    var factNum = document.getElementById('factNum');
    factNum.innerHTML = "The factorial of <b>" + numInput + "</b> is <b>" + num + "</b>";
}