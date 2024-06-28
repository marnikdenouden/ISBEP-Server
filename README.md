ISBEP Situation Server
=============================

 - [Overview](#overview)
 - [Components](#components)
   - [Web socket server](#web-socket-server)
   - [TCP server](#tcp-server)
   - [Situation](#situation)
   - [Robot display](#robot-display)
   - [Sensor values](#sensor-values)
 - [Server start up](#server-usage)
 - [Server setup](#server-setup)
 - [Dependencies](#dependencies)

-----------------------------

# Overview
Unity situation server for the Innovation Space Bachelor End Project (ISBEP)

# Components
In order to integrate the ISBEP-Simulation with the ISBEP-WebApp a server is used, which is able to provide a stable interface. The Unity engine allows the situation to be displayed, which allows developments to be displayed. Thus allowing a normally quite technical component in the demonstrator system to be made visual and assesible.

## Web socket server
The web socket server allows data to be send and be accessible for the web app. The [web socket module](https://github.com/websockets/ws) for [Node.js](https://nodejs.org/en/) is used to create the web socket server, while [Unity](https://unity.com) uses C#. In order to access this [Node.js](https://nodejs.org/en/) code we use [Jering.Javascript.NodeJS](https://github.com/JeringTech/Javascript.NodeJS), which is a package that allows us to invoke [Node.js](https://nodejs.org/en/) code.

## TCP server
- Input

## Situation
- Display
- Export

## Robot display
- Display
- Update

## Sensor values
- Utilize Robot
- Heatmap
- Appending

# Server installation
- Ensure all dependencies are locally met.
- Clone this repository
- Open the folder as unity project in Unity hub
- Open the unity project with Unity install 2022.3.19f1 (or later)

# Server usage
In order to use the server follow the next steps.
- Set the various settings of the components, such as server-ip's.
- Click run in the Unity editor.

# Server setup
In order to create a new unity server project locally you can follow the next steps.
- Ensure all dependencies are locally met.
- Create a new unity project
o	https://github.com/JeringTech/Javascript.NodeJS?tab=readme-ov-file
o	https://github.com/GlitchEnzo/NuGetForUnity
- Run install command with npm, which is included in Node.js (Required for project setup, or clone [node modules](node_modules))

#### Install command ws
    npm install ws

# Dependencies
[Unity](https://unity.com/download) is required for running and developing this project. The server also uses [Node.js](https://nodejs.org/en/download/) for the [ws module](https://github.com/websockets/ws) to create a web socket server.

## Local requirements
Download and install the following software.
- [Unity](https://unity.com/download)
- [Node.js](https://nodejs.org/en/download/)

> [!TIP]
> Use the [Node.js debugger](https://nodejs.org/en/learn/getting-started/debugging) to inspect the web socket server, after enabling nodeJSDebug at [webSocketServer.cs](Assets/Scripts/Connection/WebSocket/WebSocketServer.cs) script component in the Unity GUI.
