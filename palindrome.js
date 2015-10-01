// JavaScript source code
function palindrome(String given) {
    String test = given.toLowerCase();
    var pal = new Boolean();
    var a = 0;
    var b = test.length - 1;
    switch(true) {
        case (test.length % 2 === 0):
            pal = false;
            break;
        default:
            while (a !== b) {
                if (test.charAt(a) === test.charAt(b)) {
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
    return pal;
}