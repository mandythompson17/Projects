// Word Frequency //

document.getElementById('freqfile').onchange = function wordFreq() {
    var wordCount = document.getElementById('wordCount');
    var file = this.files[0];
    var fr = new FileReader();
    fr.onload = function (progressEvent) {
        var words = this.result.split(" ");
        var occs = [];
        for (var i = 0; i < words.length; i++) {
            var thisWord = words[i].toLowerCase();
            var duplicate = false;
            var uniOccs = occs.length;
            for (var n = 0; n < uniOccs; n++) {
                var compare = occs[n][0];
                if (thisWord === compare) {
                    occs[n][1]++;
                    duplicate = true;
                }
            }
            if (duplicate == false) {
                occs.push([thisWord, 1]);
            }
        }
        occs.sort(function (a, b) { return b[1] - a[1] });
        wordCount.innerHTML = "The words in this file and their frequencies:<br><br>";
        for (var x = 0; x < uniOccs; x++) {
            wordCount.innerHTML += "<b>" + occs[x][0] + " " + occs[x][1] + "</b><br><br>";
        }
    };
    fr.readAsText(file);
};