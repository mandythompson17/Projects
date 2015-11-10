var priority = document.getElementById("priority");
if (priority.innerHTML.toString() == "Urgent")
{
    priority.className += "label label-important";
}
if (priority.innerHTML.toString() == "High")
{
    priority.className += "label label-warning";
}

var status = document.getElementById("StatusName");
if (status.innerHTML.toString() == "Unassigned") {
    status.className += "label label-warning";
}
if (status.innerHTML.toString() == "Assigned / Open") {
    status.className += "label label-info";
}
if (status.innerHTML.toString() == "Ready for Testing") {
    status.className += "label label-success";
}
if (status.innerHTML.toString() == "Resolved") {
    status.className += "label label-important";
}

/*for (var count = 0, count)
var priorityCol = document.getElementById("priority_col");
priorityCol.setAttribute("id", "p1");
if (priorityCol.innerHTML.toString() == "Urgent") {
    priorityCol.className += "label label-important";
}*/