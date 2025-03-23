var connection = new signalR.HubConnectionBuilder().withUrl("/hubs/DmHubs").build();

var recieverUserId = document.getElementById("recieverUserId").value;
var senderUserId = document.getElementById("senderUserId").value;
var token = document.getElementById("token").value;
var userEmail = document.getElementById("currentUserEmail").value;

var moreMsgBtn = document.getElementById("loadMoreMessages");

var msgBox = document.getElementById("messageBox");

var msgField = document.getElementById("msgField");
var msgBtn = document.getElementById("msgBtn");
var userMessageBubble = document.getElementById("userMessageBubble");
var othersMessageBubble = document.getElementById("othersMessageBubble")
var chatContainer = document.getElementById("ChatContainer");

var connectionId = ""

var pageNumber = 1;
var pageSize = 10;

window.onload= () => {
    GetMessages(pageSize,pageNumber)
}

moreMsgBtn.addEventListener("click", (event) =>{
    event.preventDefault()
    pageNumber++;
    pageSize += 10;
    GetMessages(pageSize, pageNumber)
})

msgBtn.addEventListener("click", () => SendMessage(msgField.value));
msgField.addEventListener("keydown", (event) => {
    if (event.key === "Enter") {
        event.preventDefault()
        SendMessage(msgField.value)
    }
})

connection.on("RecieveDm", (message, dmToken, connectionid) => {
    var route = window.location.href.split("/")
    console.log(`${dmToken.substring(0, 7)} == ${route[4].substring(0, 7)}`)

    if (dmToken.substring(0, 7) == route[4].substring(0, 7)) {

        if (connectionId == connectionid) {
            CloneMessageBubble(userMessageBubble)
        }
        else {
            CloneMessageBubble(othersMessageBubble)
        }

    }
})

connection.on("NotAvailableResponse", (message) => {
    console.log(message)
})

function SendMessage(msg) {
    connection.send("SendDirectMessge", recieverUserId, senderUserId, token, msg);
    console.log("SendMessage hit")
}

function Fufilled() {
    console.log("Dm connected successfully. ConnectionId:", connection.connectionId);
    connectionId = connection.connectionId
}
function Failed() {
    console.log("Dm connection failed.")
}

function CloneMessageBubble(messageBubble) {
    var clonedMessageBubble = messageBubble.cloneNode(true)
    clonedMessageBubble.innerHTML = message;
    clonedMessageBubble.style.display = "block"
    chatContainer.appendChild(clonedMessageBubble);
    clonedMessageBubble.scrollIntoView({ behavior: "smooth" })
    msgField.value = ""
    console.log("others reached")
}

function GetMessages(pgSize = 10, pgNumber = 1) {
    fetch(`/${token}/Messages?PageNumber=${pgNumber}&PageSize=${pgSize}`, {
        headers: { "Content-Type": "application/json" },
        method: "GET"
    })
        .then(response => response.json())
        .then(data => {
            console.log(data)
            // array.forEach(data => {
            //     var sender = data.value.sender;
            //     var receiver = data.value.receiver;
            //     var content = data.value.content;
            //     var date = data.value.timeStamp;

            //     if (userEmail == sender) {
            //         CloneMessageBubble(userMessageBubble)
            //     } else {
            //         CloneMessageBubble(othersMessageBubble)
            //     }
            // });
        })
        .catch(error => console.log("Error:", error))
}

connection.start().then(Fufilled, Failed)