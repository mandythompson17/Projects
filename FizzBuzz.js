// JavaScript source code

    var i = 1;
    while (i <= 100) {
       
        if (i % 3 === 0 && i % 5 === 0) {
            document.write("FizzBuzz\n");
        }

        else if (i % 3 === 0) {
            document.write("Fizz\n");
        }

        else if (i % 5 === 0) {
            document.write("Buzz\n");
        }

        else {
            document.write(i + "\n");
        }
        i++;
    }