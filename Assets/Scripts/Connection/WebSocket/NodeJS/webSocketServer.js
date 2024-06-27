/**
 * Web socket server code that can be used to provide data to the web app.
 */
const WebSocket = require('ws');

/**
 * Web socket server class the use for testing the web app.
 */
class WebSocketServer {
    /** Connected connections for this web socket server. */
    connectedConnection;

    /** Web socket server */
    webSocketServer;

    constructor() {
        this.connectedConnection = new Set();
        this.webSocketServer;
    }

    /**
     * Start the web socket server.
     */
    start(port) {
        this.stop(); // In case we already have a websocket sever running.
        
        debugger;

        console.log(`Starting webSocketServer at port ${port}`);
        this.webSocketServer = new WebSocket.Server({ port: port });
        
        // Add reference to self to use in functions.
        let self = this;

        // Add connection listener that stores and handles the connection.
        this.webSocketServer.on('connection', function connection(webSocket) {
            self.connectedConnection.add(webSocket);
            console.log('Client connected');

            // Echo
            // webSocket.on('message', function incoming(message) {
            //     webSocket.send(`${message}`);
            // });
        
            webSocket.on('close', function () {
                console.log('Client disconnected');
                self.connectedConnection.delete(webSocket)
            });
        });
    }

    /**
     * Stop the web socket server.
     */
    stop() {
        debugger;

        console.log('Closing webSocketServer, if it has not been closed already.')
        this.webSocketServer?.close();
    }
    
    /**
     * Broadcase a message to all connections to the web socket.
     * 
     * @returns The number of connections the message was send to.
     */
    broadcast(message) {
        debugger;

        console.log(`Broadcasting message '${message}' to ${this.connectedConnection.size} connection(s).`);
        this.connectedConnection.forEach(connectedWebSocket => {
            connectedWebSocket.send(message);
        });
        return this.connectedConnection.size;
    }
}

// Setup the webSocketServer.
let webSocketServer = new WebSocketServer();

module.exports = {
    start: async (port) => {
        try {
            webSocketServer.start(parseInt(port));
        } catch(error) {
            return `Error starting web socket server at port ${port}\n` + error.message;
        }
        return `Succesfully started web socket server at port ${port}.`;
    },
    stop: async () => {
        webSocketServer.stop();
    },
    broadcast: async (message) => {
        let connections = webSocketServer.broadcast(message);
        return `Broadcasted message to ${connections} connections.`;
    },
}