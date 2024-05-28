let connectedConnection = new Set()

module.exports = {
    start: (callback, message) => {
        const WebSocket = require('ws');
    
        const wss = new WebSocket.Server({ port: 8080 });
    
        wss.on('connection', function connection(ws) {
            connectedConnection.add(ws)
            console.log('Client connected');
    
    
            ws.on('message', function incoming(message) {
    
                console.log('Received: %s', message);
                
                ws.send(`${message}`);
            });
    
    
            ws.on('close', function () {
                console.log('Client disconnected');
                connectedConnection.delete(ws)
            });
    
            ws.send("Welcome!");
        });
        callback(null, { message: message });
    },
    broadcast: (callback, message) => {
        connectedConnection.forEach(connectedWebSocket => {
            connectedWebSocket.send(message)
        });
    }
}