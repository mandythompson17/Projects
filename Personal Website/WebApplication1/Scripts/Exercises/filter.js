// Filter Long Words //

document.getElementById('file').onchange = function filterLongWords() {
    var filterResult = document.getElementById('filterResult');
    var file = this.files[0];
    var fr = new FileReader();
    fr.onload = function(progressEvent) {
        var fileSpl = this.result.split(" ");
        var i = document.getElementById('filterBox').value;
        var longWords = new Array();
        for (var n = 0; n < fileSpl.length; n++) {
            var word = fileSpl[n].toLowerCase();		
            if (word.length > i) {
                var duplicate = new Boolean(false);
                var d = 0;
                while (d < longWords.length) {
                    compare = longWords[d];	
                    if (word === compare) {
                        duplicate = true;
                        break;
                    }
                    else {
                        d++;
                    }
                }
                if (duplicate == false) {
                    longWords.push(word);
                }	
            }
        }
        if (longWords.length === 0) {
            filterResult.innerHTML = "This file contains no words longer than <b>" + i + "</b> letters.";
        }
        else {
            filterResult.innerHTML = "The words longer than <b>" + i + "</b> letters in this file are: <br><br>";
            for(var count = 0; count < longWords.length; count++) {
                filterResult.innerHTML += longWords[count] + "<br><br>";
            }
        }
		
    };
    fr.readAsText(file);
};