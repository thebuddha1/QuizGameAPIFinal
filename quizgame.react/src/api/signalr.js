import * as signalR from "@microsoft/signalr";

let connection = null;

export async function startConnection() {

    if (
        connection &&
        connection.state === signalR.HubConnectionState.Connected
    ) {
        return connection;
    }

    connection = new signalR.HubConnectionBuilder()
        .withUrl(
            "https://localhost:7001/quizhub"
        )
        .withAutomaticReconnect()
        .build();

    await connection.start();

    console.log("SignalR connected");

    return connection;
}

export function getConnection() {
    return connection;
}