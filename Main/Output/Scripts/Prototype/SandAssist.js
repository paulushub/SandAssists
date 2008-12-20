// Contains extra scripts created or used by the Sandcastle Assist projects

function sendfeedback(subject, id, alias) 
{
    var rExp = /\"/gi;
    var url = location.href;
    // Need to replace the double quotes with single quotes for the mailto to work.
    var rExpSingleQuotes = /\'\'"/gi;
    var title = document.getElementsByTagName("TITLE")[0].innerText.replace(rExp, "''");
    location.href = "mailto:" + alias + "?subject=" + subject + title + "&body=Topic%20ID:%20" + id + "%0d%0aURL:%20" + url + "%0d%0a%0d%0aComments:%20";
}
