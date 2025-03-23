var connection = new signalR.HubConnectionBuilder().withUrl("/hubs/FriendRequestHub").build();

var refreshBtn = document.getElementById("Refresh");
var requestContainer = document.getElementById("requestContainer");

refreshBtn.addEventListener("click", SendFriendRequests)

connection.on("FriendRequests", (jsonResponse, _status) => {
    // console.log(jsonResponse)
    JSON.parse(jsonResponse).forEach(element => {
        // console.log(element)
    })

    JSON.parse(jsonResponse).forEach(element => {
        requestContainer.innerHTML = "Hit"

        var nameDiv = document.createElement("div");
        nameDiv.innerHTML = `${element.FirstName} ${element.LastName}`;

        var acceptBtn = document.createElement("button");
        acceptBtn.innerHTML = "Accept";

        var rejectBtn = document.createElement("button");
        rejectBtn.innerHTML = "Reject";

        acceptBtn.addEventListener("click", () => {
            fetch(`/AcceptReject?id=${element.UserId}`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    "Purpose": 1,
                    "SenderUserIdentifier": element.Id,
                    "Token": element.Token,
                })
            })
                .then(response => response.json())
                .then(data => console.log("Success:", data))
                .catch(error => console.error("Error:", error));
        })

        rejectBtn.addEventListener("click", () => {
            fetch(`/AcceptReject?id=${element.UserId}`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    "Purpose": 2,
                    "SenderUserIdentifier": element.Id,
                    "Token": element.Token,
                })
            })
                .then(response => response.json())
                .then(data => console.log("Success:", data))
                .catch(error => console.error("Error:", error));
        })
        requestContainer.appendChild(nameDiv);
        requestContainer.appendChild(acceptBtn);
        requestContainer.appendChild(rejectBtn);
    });
})

function SendFriendRequests() {
    requestContainer.innerHTML = "";
    connection.send("SendFriendRequests")
    console.log("request hub hit")
}

function Fufilled() {
    console.log("Friend Requests connnected successfully.");
    SendFriendRequests();
}

function Failed() {
    console.log("Friend Requests connection failed.")
}

connection.start().then(Fufilled, Failed)

var requestIntervals = setInterval(SendFriendRequests, 5000)