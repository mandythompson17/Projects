// Find Longest Word // 
document.getElementById('afile').onchange = function findLongestWord() {
    var longestWord = document.getElementById('longestWord');
    var file = this.files[0];
    var fr = new FileReader();
    fr.onload = function (progressEvent) {
        var words = this.result.split(" ");
        var longWord = words[0];
        for (var i = 1; i < words.length; i++) {
            if (words[i].length > longWord.length) {
                longWord = words[i];
            }
        }
        longestWord.innerHTML = "The longest word in this document is: <b>" + longWord + "</b>";
    };
    fr.readAsText(file);
};