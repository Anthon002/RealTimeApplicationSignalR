var connection = new signalR.HubConnectionBuilder().withUrl("/hubs/DmHubs").build();
var dbName = "MessageDB"; //indexDb
var messageStore = "Message" //indexDB

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
var timeStampDiv = document.getElementById("bubbleTimeStamp")

var connectionId = ""

var pageNumber = 1;
var pageSize = 10;

const messageClass = {
    createDb: () => {
        var request = indexedDB.open(dbName, 1)
        request.onupgradeneeded = (event) => {
            var db = event.target.result
            if (!db.objectStoreNames.contains(messageStore)) {
                var store = db.createObjectStore(messageStore, { autoIncrement: true, keyPath: "Id" })
                store.createIndex("ChatToken", "ChatToken", { unique: false })
                store.createIndex("Content", "Content", { unique: false })
                store.createIndex("ReceiverName", "ReceiverName", { unique: false })
                store.createIndex("SenderUserId", "SenderUserId", { unique: false })
                store.createIndex("ReceiverUserId", "ReceiverUserId", { unique: false })
                store.createIndex("TimeCreated", "TimeCreated", { unique: false })
            }
        }
        request.onsuccess = () => {
            console.log(`${dbName} created successfully`)
        }
        request.onerror = (event) => {
            console.log(`${event.target.error} failed to create`)
          };
    },

    addMessage: (messageRecord) => {
        console.log("addMessage class method hit")
        var request = indexedDB.open(dbName, 1)
        request.onsuccess = (event) => {
            var db = event.target.result
            var transaction = db.transaction(messageStore, "readwrite")
            var store = transaction.onObjectStore(messageStore)
            var response = store.add(messageRecord)
            response.onsuccess = () => { console.log("message saved successfully")}
            response.onerror = (evnt) => { console.log(`Error: ${evnt.target.error}`)}
        }

    }
}
messageClass.createDb()

GetMessages(pageSize, pageNumber) // get messages from server side db

moreMsgBtn.addEventListener("click", (event) => {
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
    console.log(`${dmToken.substring(0, 7)} == ${route[4].substring(0, 7)} : Before if statement`)

    if (dmToken.substring(0, 7) == route[4].substring(0, 7)) {
        console.log("after if statement")
        if (connectionId == connectionid) {
            CloneMessageBubble(userMessageBubble, message, new Date().toLocaleTimeString(),timeStampDiv )
        }
        else {
            CloneMessageBubble(othersMessageBubble, message, new Date().toLocaleTimeString([],{hour:"2-digit", minute: "2-digit"}),timeStampDiv)
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

const connectionState = {
    Fufilled: () => {
        console.log("Dm connected successfully. ConnectionId:", connection.connectionId);
        connectionId = connection.connectionId
    },
    Failed: () => {
        console.log("Dm connection failed.")
    }
}

function GetMessages(pgSize = 10, pgNumber = 1) {
    fetch(`/${token}/Messages?PageNumber=${pgNumber}&PageSize=${pgSize}`, {
        headers: { "Content-Type": "application/json" },
        method: "GET"
    })
        .then(response => response.json())
        .then(data => {
            var records = data.value.records.reverse()
            console.log(records)
            records.forEach(element => {
                var jsDate = new Date(element.timeStamp).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
                console.log(jsDate)
                if (element.receiverName == userEmail) {
                    CloneMessageBubble(othersMessageBubble, element.message, jsDate, timeStampDiv)
                }
                else {
                    CloneMessageBubble(userMessageBubble, element.message, jsDate, timeStampDiv)
                }
            });
        })
        .catch(error => console.log("Error:", error))
}

function CloneMessageBubble(messageBubble, msg, timeStamp, timeStampDiv) {
    var clonedMessageBubble = messageBubble.cloneNode(true)
    var cloneTimeStamp = timeStampDiv.cloneNode(true)

    clonedMessageBubble.innerHTML = msg;
    clonedMessageBubble.style.display = "block"
    clonedMessageBubble.appendChild(cloneTimeStamp)
    chatContainer.appendChild(clonedMessageBubble);

    cloneTimeStamp.innerHTML = " "
    cloneTimeStamp.innerHTML = timeStamp
    clonedMessageBubble.scrollIntoView({ behavior: "smooth" })
    msgField.value = ""
    console.log(`msg bubble reached : ${msg}`)

    // save message to indexedDb
    var messageRecord = {ChatToken : token, Content :  msg, ReceiverName : userEmail, SenderUserId : senderUserId, ReceiverUserId : recieverUserId, TimeCreated : timeStamp }

    messageClass.addMessage(messageRecord)

}

connection.start().then(connectionState.Fufilled, connectionState.Failed)
