
// Establish Connection
var broadCastConnection = new signalR.HubConnectionBuilder()
    .configureLogging(signalR.LogLevel.Information)
    .withUrl("/hubs/ChatHub")
    .build()

var isConnected = false;

var generalMessageField = document.getElementById("GeneralMessageField");
var broadCastButton = document.getElementById("SendGeneralMsgBtn");
var chatContainer = document.getElementById("ChatContainer");
var notificationDiv = document.getElementById("isTypingNotficationDiv");
var userMessageBubble = document.getElementById("userMessageBubble");
var othersMessageBubble = document.getElementById("othersMessageBubble");
var connectionId = ''

// var recipientEmail = document.getElementById("recipientEmail");
// var testButton = document.getElementById("testButton");

broadCastButton.addEventListener("click", sendMessage);
generalMessageField.addEventListener("keydown", (event)=>{
if (event.key === "Enter"){
    event.preventDefault();
    sendMessage();
}
})
//testButton.addEventListener("click",  FriendRequests)

var interval = setInterval(FriendRequests(), 1000);

generalMessageField.addEventListener("input", userIsTyping);
generalMessageField.addEventListener("blur", userIsNotTyping)

var bubblePos = -65;

// Connect to HubMethod using clientConnectionKey
broadCastConnection.on("SendGeneralMessage", (message, userName, connectionID) => {
    console.log(`js: ${connectionId}. Cs: ${connectionID}`)
    
    if (connectionId ==  connectionID)
    {
        var myBubble = userMessageBubble.cloneNode(true);
        myBubble.style.bottom = `${bubblePos}px`
        myBubble.innerHTML = `<br> ${userName} : ${message}`;
        myBubble.style.display = 'block'
        chatContainer.appendChild(myBubble)
        myBubble.scrollIntoView({ behavior: "smooth"})
        bubblePos += 10
    }
    else
    {
        var othersBubble = othersMessageBubble.cloneNode(true)
        othersBubble.style.bottom = `${bubblePos}px`
        othersBubble.innerHTML = `<br> ${userName} : ${message}`;
        othersBubble.style.display = 'block'
        chatContainer.appendChild(othersBubble)
        othersBubble.scrollIntoView({ behavior: "smooth"})
        bubblePos += 10
    }
    generalMessageField.value = ""
    userIsNotTyping()

    console.log("SendGeneralMessge retrieved")
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

broadCastConnection.on("FriendRequests", (jsonRequests, status) => 
{
    console.log(`${jsonRequests}` + `${status}`)
})

//initial hub invocation/sending i.e hit the server hub
async function sendMessage() {
    console.log(generalMessageField.value)
    var message = generalMessageField.value;
    if (message.trim() == "")
        return;

   // var email = recipientEmail.value;
    await broadCastConnection.send("GeneralMessage", message);
    //await broadCastConnection.send("MessageToRecipient", " ", message);
    // notificationDiv.innerHTML = "";
    // generalMessageField.value = ""
    console.log("General message hit");
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

function FriendRequests()
{
    if (isConnected == true)
    {
        broadCastConnection.send("SendFriendRequests");
        console.log("Test touched.");
    }
}

//Start connection

function fufilled() {
    console.log(`Connection to chatHub established successfully. ConnectionId : ${broadCastConnection.connectionId}`);
    isConnected = true;
    connectionId = broadCastConnection.connectionId
}

function failed() {
    console.log("Connection to chatHub unsuccessful");
}

broadCastConnection.start().then(fufilled, failed)