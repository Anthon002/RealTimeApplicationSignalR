// Establish Connection
var broadCastConnection = new signalR.HubConnectionBuilder()
    .configureLogging(signalR.LogLevel.Information)
    .withUrl("/hubs/ChatHub")
    .build()

var generalMessageField = document.getElementById("GeneralMessageField");
var broadCastButton = document.getElementById("SendGeneralMsgBtn");
var chatContainer = document.getElementById("ChatContainer");
var notificationDiv = document.getElementById("isTypingNotficationDiv");

broadCastButton.addEventListener("click", sendMessage);

generalMessageField.addEventListener("input", userIsTyping);
generalMessageField.addEventListener("blur", userIsNotTyping)

// Connect to HubMethod using clientConnectionKey
broadCastConnection.on("SendGeneralMessage", (message, randomUserName) => {
    chatContainer.innerHTML += `<br> ${randomUserName} : ${message}`;
}) // collects the general

broadCastConnection.on("SendGeneralNotification", (notification) => {
    notificationDiv.innerHTML = notification;
})

broadCastConnection.on("SendNotTypingNotification",() =>
{
    notificationDiv.innerHTML = "";
})

//initial hub invocation/sending i.e hit the server hub
function sendMessage() {
    broadCastConnection.send("GeneralMessage", generalMessageField.value);
    notificationDiv.innerHTML = "";
    generalMessageField.value = ""
    console.log("sendMessage hit");
}

function userIsTyping() {
    broadCastConnection.send("UserIsTypingNotification");
    console.log("userIsTyping hit")
}

function userIsNotTyping() {
    broadCastConnection.send("UserIsNotTypingNotification");
}

//Start connection

function fufilled() {
    console.log("Connection to chatHub established successfully");
}

function failed() {
    console.log("Connection to chatHub unsuccessful");
}

broadCastConnection.start().then(fufilled, failed)