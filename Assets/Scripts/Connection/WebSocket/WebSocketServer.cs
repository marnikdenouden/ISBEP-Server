using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jering.Javascript.NodeJS;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine.Assertions;
using System.IO;
using System.Reflection;
using System;
using Unity.VisualScripting;

public class NodeJSScript : MonoBehaviour
{
    private INodeJSService nodeJSService;

    private void Awake()
    {
        string FilePathNodeJS = Path.Join(Application.dataPath, "/Scripts/Connection/WebSocket/NodeJS");
        Debug.Log($"FilePathNodeJS data path {FilePathNodeJS}");

        var services = new ServiceCollection();
        services.AddNodeJS();
        services.Configure<NodeJSProcessOptions>(options => {
            options.ProjectPath = FilePathNodeJS;
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

        Result messageResult = await nodeJSService.InvokeFromFileAsync<Result>("webSocketServer.js", "broadcast", args: new[] { "Welcome!"});
        Debug.Log($"Send : {messageResult?.Message}");
    }
}
