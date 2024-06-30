ISBEP Situation Server
=============================

 - [Overview](#overview)
 - [Components](#components)
   - [Web socket server](#web-socket-server)
   - [TCP server](#tcp-server)
   - [Situation](#situation)
   - [Robot display](#robot-display)
   - [Sensor values](#sensor-values)
 - [Server installation](#server-installation)
 - [Server usage](#server-usage)
 - [Dependencies](#dependencies)

-----------------------------

# Overview
Unity situation server for the Innovation Space Bachelor End Project ([ISBEP](https://studiegids.tue.nl/opleidingen/innovation-space/bachelor/isbep-innovation-space-bachelor-end-project))

# Components
In order to integrate the [ISBEP-Simulation](https://github.com/marnikdenouden/ISBEP-Simulation) with the [ISBEP-WebApp](https://github.com/marnikdenouden/ISBEP-WebApp) a server is used, which is able to provide a stable interface. The [Unity](https://unity.com) engine allows the situation to be displayed, which allows developments to be displayed. Thus allowing a normally quite technical component in the demonstrator system to be made visual and accessible.

## Web socket server
The web socket server allows data to be sent and accessed by the web app. The [web socket module](https://github.com/websockets/ws) for [Node.js](https://nodejs.org/en/) is used to create the web socket server, while [Unity](https://unity.com) uses C#. In order to access this [Node.js](https://nodejs.org/en/) code we use [Jering.Javascript.NodeJS](https://github.com/JeringTech/Javascript.NodeJS), which is a package that allows us to invoke [Node.js](https://nodejs.org/en/) code.

## TCP server
The TCP server allows robot data to be received in order to update the [robot display](#robot-display). As a server the system is able to accept multiple client connections, therefore multiple robots could share their data with their own client connection.

## Situation
The [Situation.cs](Assets/Scripts/Situation/Situation.cs) script combines the various elements that make up the situation. Allowing the robots to be found, updated and their data send. The situation also allows the data of the objects to be exported to a JSON file, which can be used by [ISBEP-Simulation](https://github.com/marnikdenouden/ISBEP-Simulation) to have a matching setup.

## Robot display
Robots have data for a position, rotation and sensor values as defined in the [robot controller](Assets/Scripts/Situation/RobotControler.cs) script. The position and rotation are used to update a virtual object to represent the robot, possibly with a distinct color. Furthermore the robots have a camera attached to the virtual object to define their perspective, which can also be streamed using the [TCP Server](#tcp-server) to the [ISBEP-WebApp](https://github.com/marnikdenouden/ISBEP-WebApp).

## Sensor values
For the [situation](#situation) sensor values are generated with heatmaps, which means that after heatmap creation each position has a fixed value for a sensor value. Robots use the [SensorValues.cs](Assets/Scripts/SensorValues/SensorValues.cs) script to retrieve the set of sensor values for their position.

# Server installation
The following steps can be taken to install the server locally.

- Ensure all [dependencies](#dependencies) are locally met.
- Clone [this]() repository
- Open the folder as Unity project in Unity Hub
- Open the Unity project with Unity install 2022.3.19f1 (or later)

# Server usage
In order to run an installed server follow the next steps.

- Set the various settings of the components, such as server-ip's.
- Click run in the Unity editor.

# Dependencies
[Unity](https://unity.com/download) is required for running and developing this project. The server also uses [Node.js](https://nodejs.org/en/download/) for the [ws module](https://github.com/websockets/ws) to create a web socket server.

## Local requirements
Download and install the following software.
- [Unity](https://unity.com/download)
- [Node.js](https://nodejs.org/en/download/)

## Project dependencies
- [ws module](https://github.com/websockets/ws)
- [NuGetForUnity](https://github.com/GlitchEnzo/NuGetForUnity)
- [Jering.Javascript.NodeJS](https://github.com/JeringTech/Javascript.NodeJS)
- [NewtonSoft.JSON-for-Unity](https://github.com/applejag/Newtonsoft.Json-for-Unity)

> [!TIP]
> Use the [Node.js debugger](https://nodejs.org/en/learn/getting-started/debugging) to inspect the web socket server, after enabling nodeJSDebug at [webSocketServer.cs](Assets/Scripts/Connection/WebSocket/WebSocketServer.cs) script component in the Unity GUI.
