using UnityEngine;
using Jering.Javascript.NodeJS;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using ISBEP.Utility;


namespace ISBEP.Communication
{
    public class WebSocketServer : MonoBehaviour
    {
        [Tooltip("Specify whether debug message for the web socket server should be displayed in the logs.")]
        public bool DebugMessages = false;
        private static readonly string CONTEXT = "Web Socket Server";

        [Tooltip("Specify if node js debugger should run. See https://nodejs.org/en/learn/getting-started/debugging for debugging information.")]
        public bool nodeJSDebug = false;

        [Tooltip("Specify the port to start the web socket server with.")]
        public int port = 5633;

        private INodeJSService nodeJSService;
        public static WebSocketServer Instance { get; private set; }

        private void Awake()
        {
            if (DebugMessages) Util.AddDebugContext(CONTEXT);

            // Set static reference to self, so class has public access point.
            Instance = this;

            // Configure NodeJs access path, so we can call the web socket server javascript file.
            string FilePathNodeJS = Path.Join(Application.dataPath, "/Scripts/Connection/WebSocket/NodeJS");

            Util.DebugLog(CONTEXT, $"FilePathNodeJS data path {FilePathNodeJS}");

            // Setup the NodeJS invoke services. With or without debug settings.
            var services = new ServiceCollection();
            services.AddNodeJS();
            if (nodeJSDebug)
            {
                services.Configure<NodeJSProcessOptions>(options =>
                {
                    options.ProjectPath = FilePathNodeJS;
                    options.NodeAndV8Options = "--inspect-brk";
                });
                services.Configure<OutOfProcessNodeJSServiceOptions>(options =>
                {
                    options.InvocationTimeoutMS = -1;
                });
            } else
            {
                services.Configure<NodeJSProcessOptions>(options =>
                {
                    options.ProjectPath = FilePathNodeJS;
                });
            }
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            nodeJSService = serviceProvider.GetRequiredService<INodeJSService>();
        }

        private void Start()
        {
            StartWebSocketServerAsync();
        }

        private void OnDisable()
        {
            StopWebSocketServerAsync();
        }

        /// <summary>
        /// Start the web socket server.
        /// </summary>
        private async void StartWebSocketServerAsync()
        {
            Util.Log(CONTEXT, $"Starting web socket server at port {port}");
            string log = await nodeJSService.InvokeFromFileAsync<string>("webSocketServer.js", "start", args: new[] { $"{port}" });
            Util.DebugLog(CONTEXT, log);
        }

        /// <summary>
        /// Stop the web socket server.
        /// </summary>
        private async void StopWebSocketServerAsync()
        {
            Util.Log(CONTEXT, $"Stopping web socket server");
            await nodeJSService.InvokeFromFileAsync("webSocketServer.js", "stop", args: new[] { "" });
            Util.DebugLog(CONTEXT, "Stopped the web socket server.");
        }

        /// <summary>
        /// Broadcasts the specified message to all the connections made to the web socket server.
        /// </summary>
        /// <param name="message">Message string to send to all connections of the web socket server.</param>
        public async void BroadcastWebSocketMessage(string message)
        {
            string log = await nodeJSService.InvokeFromFileAsync<string>("webSocketServer.js", "broadcast", args: new[] { message });
            Util.DebugLog(CONTEXT, log);
        }

    }
}
