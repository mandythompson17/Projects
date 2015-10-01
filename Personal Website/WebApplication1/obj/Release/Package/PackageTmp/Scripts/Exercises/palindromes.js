// Palindromes //
function palindromes() {
    var given = document.getElementById('testPal').value;
    var testWord = given.toLowerCase();
    testWord = testWord.replace(/\s+/g, '');
    var pal = new Boolean(false);
    var a = 0;
    var b = testWord.length - 1;
    switch (true) {
        case (testWord.length % 2 === 0):
            while (b > a) {
                if (testWord.charAt(a) === testWord.charAt(b)) {
                    a++;
                    b--;
                    pal = true;
                }
                else {
                    pal = false;
                    break;
                }
            }
            break;
        default:
            while (a !== b) {
                if (testWord.charAt(a) === testWord.charAt(b)) {
                    a++;
                    b--;
                    pal = true;
                }
                else {
                    pal = false;
                    break;
                }
            }
            break;
    }
    var checkPalind = document.getElementById('checkPalind');
    if (pal === true) {
        checkPalind.innerHTML = "Yes, that is a palindrome!";
    }
    else {
        checkPalind.innerHTML = "No, that is not a palindrome.";
    }
}