
"use strict";


//$(document).ready(function () {

var connection = new signalR.HubConnectionBuilder().withUrl("/OrderingHub").build();
connection.on("ReceiveMessage", function (user, message) {
    //alert("Message Received");
    //console.log("Message Received");
    debugger;
    console.log(user);
    console.log(message);
    // if (user == "CanteenManage") {
    // 	//alert("CanteenManage");
    // 	window.location.reload();
    // }
});
connection.start().then(function () {
    //document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});


$('#searchEmp').click(function (event) {
    //alert("btnclick");
    debugger;
    var a = 345345;
    connection.invoke("SendMessage","message Ardhendu").catch(function (err) {
        return console.error(err);
    });
    event.preventDefault();
});

//});