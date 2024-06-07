let connectedConnection = new Set();
let webSocketServer;

module.exports = {
    start: (callback, message) => {
        const WebSocket = require('ws');
    
        webSocketServer = new WebSocket.Server({ port: 8080 });
    
        webSocketServer.on('connection', function connection(webSocket) {
            connectedConnection.add(webSocket);
            console.log('Client connected');
    
            // Echo
            // webSocket.on('message', function incoming(message) {                
            //     webSocket.send(`${message}`);
            // });
    
            
            webSocket.on('close', function () {
                console.log('Client disconnected');
                connectedConnection.delete(webSocket)
            });
    
            webSocket.send("Welcome!");
        });
        callback(null, { message: message });
    },
    broadcast: (callback, message) => {
        connectedConnection.forEach(connectedWebSocket => {
            connectedWebSocket.send(message);
        });
        callback(null, { message:message })
    },
    exampleMethod: (value) => {
        if (value == 0) {
            throw Error('Value is 0');
        }
        let result = value ^ 2;
        return result;
    }
}