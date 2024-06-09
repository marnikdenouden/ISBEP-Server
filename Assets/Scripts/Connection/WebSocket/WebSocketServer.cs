using UnityEngine;
using Jering.Javascript.NodeJS;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine.Assertions;
using System.IO;
using Unity.VisualScripting;


namespace ISBEP.Communication
{
    public class WebSocketServer : MonoBehaviour
    {
        private INodeJSService nodeJSService;
        public static WebSocketServer Instance { get; private set; }

        private void Awake()
        {
            Instance = this;

            string FilePathNodeJS = Path.Join(Application.dataPath, "/Scripts/Connection/WebSocket/NodeJS");
            Debug.Log($"FilePathNodeJS data path {FilePathNodeJS}");

            var services = new ServiceCollection();
            services.AddNodeJS();
            services.Configure<NodeJSProcessOptions>(options =>
            {
                options.ProjectPath = FilePathNodeJS;
                options.NodeAndV8Options = "--inspect-brk";
            });
            services.Configure<OutOfProcessNodeJSServiceOptions>(options =>
            {
                options.InvocationTimeoutMS = -1;
            });
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            nodeJSService = serviceProvider.GetRequiredService<INodeJSService>();
        }

        public class Result
        {
            public string? Message { get; set; }
        }

        private void Start()
        {
            StartServerWithNodeJS();
        }

        private async void StartServerWithNodeJS()
        {
            Result result = await nodeJSService.InvokeFromFileAsync<Result>("webSocketServer.js", "start", args: new[] { "success" });

            Assert.AreEqual("success", result?.Message);
            Debug.Log(result?.Message);

            Result messageResult = await nodeJSService.InvokeFromFileAsync<Result>("webSocketServer.js", "broadcast", args: new[] { "Welcome!" });
            Debug.Log($"Send : {messageResult?.Message}");


            //Result promiseResult = await nodeJSService.InvokeFromFileAsync<Result>("webSocketServer.js", "exampleMethod", args: new[] { "1" });
            //Debug.Log("Returned promise result");
            //Debug.Log($"Promise result : {promiseResult?.Message}");

        }

        public async void BroadcastWebSocketMessage(string message)
        {
            Result messageResult = await nodeJSService.InvokeFromFileAsync<Result>("webSocketServer.js", "broadcast", args: new[] { message });
            Debug.Log($"Send : {messageResult?.Message}");
        }
    }
}
