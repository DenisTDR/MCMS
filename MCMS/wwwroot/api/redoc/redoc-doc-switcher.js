console.log(switchDocConfig);

var redocDocSwitchMain = function () {
    var container = document.getElementById("doc-switch-container");
    if (!container) {
        setTimeout(redocDocSwitchMain, 500);
        return;
    }
    var docs = switchDocConfig.docs;
    var links = [];
    for (var i = 0; i < docs.length; i++) {
        var doc = docs[i];
        var styleAttr = doc.name === switchDocConfig.current ? "style='text-decoration: underline; font-weight: bold;'" : "";
        links.push("<a href='../" + doc.name + "' " + styleAttr + ">" + doc.title + "</a>");
    }

    var html = "<div>Switch Swagger doc: " + links.join(' | ') + "</div>";
    container.innerHTML = html;
}
if (switchDocConfig.docs.length) {
    window.addEventListener('load', redocDocSwitchMain);
}
