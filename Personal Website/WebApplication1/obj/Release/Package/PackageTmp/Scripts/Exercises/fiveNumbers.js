//Five Numbers//

//Get Numbers//

//Max of Five//
function maxOfFive(num1, num2, num3, num4, num5) {
    var num1 = parseInt(document.getElementById('textBox1').value);
    var num2 = parseInt(document.getElementById('textBox2').value);
    var num3 = parseInt(document.getElementById('textBox3').value);
    var num4 = parseInt(document.getElementById('textBox4').value);
    var num5 = parseInt(document.getElementById('textBox5').value);
    var max = Math.max(num1, num2, num3, num4, num5);
    var maxNumber = document.getElementById('maxNumber');
    maxNumber.innerHTML = "The largest number is <b>" + max + "</b>";
}

//Min of Five//
function minOfFive(num1, num2, num3, num4, num5) {
    var num1 = parseInt(document.getElementById('textBox1').value);
    var num2 = parseInt(document.getElementById('textBox2').value);
    var num3 = parseInt(document.getElementById('textBox3').value);
    var num4 = parseInt(document.getElementById('textBox4').value);
    var num5 = parseInt(document.getElementById('textBox5').value);
    var min = Math.min(num1, num2, num3, num4, num5);
    var minNumber = document.getElementById('minNumber');
    minNumber.innerHTML = "The smallest number is <b>" + min + "</b>";
}

//Mean of Five//
function meanOfFive(num1, num2, num3, num4, num5) {
    var num1 = parseInt(document.getElementById('textBox1').value);
    var num2 = parseInt(document.getElementById('textBox2').value);
    var num3 = parseInt(document.getElementById('textBox3').value);
    var num4 = parseInt(document.getElementById('textBox4').value);
    var num5 = parseInt(document.getElementById('textBox5').value);
    var mean = (num1 + num2 + num3 + num4 + num5) / 5;
    var meanNumber = document.getElementById('meanNumber');
    meanNumber.innerHTML = "The mean of the numbers is <b>" + mean + "</b>";
}

//Sum of Five//
function sumOfFive(num1, num2, num3, num4, num5) {
    var num1 = parseInt(document.getElementById('textBox1').value);
    var num2 = parseInt(document.getElementById('textBox2').value);
    var num3 = parseInt(document.getElementById('textBox3').value);
    var num4 = parseInt(document.getElementById('textBox4').value);
    var num5 = parseInt(document.getElementById('textBox5').value);
    var sum = num1 + num2 + num3 + num4 + num5;
    var sumNumber = document.getElementById('sumNumber');
    sumNumber.innerHTML = "The sum of the numbers is <b>" + sum + "</b>";
}

//Product of Five//
function productOfFive(num1, num2, num3, num4, num5) {
    var num1 = parseInt(document.getElementById('textBox1').value);
    var num2 = parseInt(document.getElementById('textBox2').value);
    var num3 = parseInt(document.getElementById('textBox3').value);
    var num4 = parseInt(document.getElementById('textBox4').value);
    var num5 = parseInt(document.getElementById('textBox5').value);
    var prod = num1 * num2 * num3 * num4 * num5;
    var prodNumber = document.getElementById('prodNumber');
    prodNumber.innerHTML = "The product of the numbers is <b>" + prod + "</b>";
}