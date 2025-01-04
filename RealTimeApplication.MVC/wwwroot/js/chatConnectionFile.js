// Establish Connection
var broadCastConnection = new signalR.HubConnectionBuilder()
    .configureLogging(signalR.LogLevel.Information)
    .withUrl("/hubs/ChatHub")
    .build()

var generalMessageField = document.getElementById("GeneralMessageField");
var broadCastButton = document.getElementById("SendGeneralMsgBtn");
var chatContainer = document.getElementById("ChatContainer");
var notificationDiv = document.getElementById("isTypingNotficationDiv");
var recipientEmail = document.getElementById("recipientEmail");
var testButton = document.getElementById("testButton");

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

broadCastConnection.on("SendNotTypingNotification", () => {
    notificationDiv.innerHTML = "";
})

broadCastConnection.on("sendToRecipient", (message, userName) => {
    chatContainer.innerHTML += `<br> ${userName} : ${message}`;
})

broadCastConnection.on("TestMessage", (param1, param2) => {
    console.log(param1);
    console.log(param2);
})

//initial hub invocation/sending i.e hit the server hub
async function sendMessage() {
    var message = generalMessageField.value;
    var email = recipientEmail.value;
    await broadCastConnection.send("GeneralMessage", email, message);
    await broadCastConnection.send("MessageToRecipient", email, message);
    notificationDiv.innerHTML = "";
    generalMessageField.value = ""
    console.log("WTF");
}

function userIsTyping() {
    broadCastConnection.send("UserIsTypingNotification");
    console.log("userIsTyping hit")
}

function userIsNotTyping() {
    broadCastConnection.send("UserIsNotTypingNotification");
}

function TestingSignalR() {
    broadCastConnection.send("TestHubMethod", "This is a test message", "Parameter 2");
}

//Start connection

function fufilled() {
    console.log("Connection to chatHub established successfully");
}

function failed() {
    console.log("Connection to chatHub unsuccessful");
}

broadCastConnection.start().then(fufilled, failed)