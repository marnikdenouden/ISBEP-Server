using UnityEngine;
using Jering.Javascript.NodeJS;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine.Assertions;
using System.IO;
using ISBEP.Utility;


namespace ISBEP.Communication
{
    public class WebSocketServer : MonoBehaviour
    {
        [Tooltip("Specify whether debug message for the web socket server should be displayed in the logs.")]
        public bool DebugMessages = false;
        private readonly string CONTEXT = "Web Socket Server";

        [Tooltip("Specify if node js debugger should run. See https://nodejs.org/en/learn/getting-started/debugging for debugging information.")]
        public bool nodeJSDebug = false;

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

        public class Result
        {
            public string Message { get; set; }
        }

        private void Start()
        {
            StartServerWithNodeJS();
        }

        private void OnDisable()
        {
            StopServerWithNodeJS();
        }

        private async void StartServerWithNodeJS()
        {
            Result result = await nodeJSService.InvokeFromFileAsync<Result>("webSocketServer.js", "start", args: new[] { "success" });

            Assert.AreEqual("success", result?.Message);
            //Debug.Log(result?.Message);

            Result messageResult = await nodeJSService.InvokeFromFileAsync<Result>("webSocketServer.js", "broadcast", args: new[] { "Welcome!" });
            //Debug.Log($"Send : {messageResult?.Message}");


            //Result promiseResult = await nodeJSService.InvokeFromFileAsync<Result>("webSocketServer.js", "exampleMethod", args: new[] { "1" });
            //Debug.Log("Returned promise result");
            //Debug.Log($"Promise result : {promiseResult?.Message}");

        }

        private async void StopServerWithNodeJS()
        {
            await nodeJSService.InvokeFromFileAsync<Result>("webSocketServer.js", "stop", args: new[] { "" });
        }

        public async void BroadcastWebSocketMessage(string message)
        {
            Result messageResult = await nodeJSService.InvokeFromFileAsync<Result>("webSocketServer.js", "broadcast", args: new[] { message });
            Debug.Log($"Send : {messageResult?.Message}");
        }
    }
}
