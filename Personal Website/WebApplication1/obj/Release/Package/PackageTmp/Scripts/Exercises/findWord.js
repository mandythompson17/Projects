// Find Word //

document.getElementById('ffile').onchange = function findWord() {
    var foundWord = document.getElementById('foundWord');
    var file = this.files[0];
    var fr = new FileReader();
    fr.onload = function (progressEvent) {
        var words = this.result.split(" ");
        var theWord = document.getElementById('findBox').value;
        var lookFor = theWord.toLowerCase();
        var count = 0;
        for (var i = 0; i < words.length; i++) {
            var thisWord = words[i].toLowerCase();
            if (thisWord === lookFor) {
                count++;
            }
        }
        foundWord.innerHTML = "The word <b>" + theWord + "</b> occurs <b>" + count + "</b> times in this document.";
    };
    fr.readAsText(file);
};