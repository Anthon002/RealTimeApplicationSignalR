var connection = new signalR.HubConnectionBuilder().withUrl("/hubs/DmHubs").build();

var recieverEmail = document.getElementById("recieverEmail").value;
var senderEmail = document.getElementById("senderEmail").value;
var token = document.getElementById("token").value;

var msgField = document.getElementById("msgField");
var msgBtn = document.getElementById("msgBtn");

msgBtn.addEventListener("click", SendMessage(msgField.value));

function SendMessage(msg)
{
    connection.send(recieverEmail, senderEmail, token, msg);
    console.log("SendMessage hit")
}

function Fufilled()
{
    console.log("Dm connected successfully.");
}
function Failed()
{
    console.log("Dm connection failed.")
}

connection.start().then(Fufilled, Failed)