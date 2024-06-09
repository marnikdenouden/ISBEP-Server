using ISBEP.Utility;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using ISBEP.Communication;
using Newtonsoft.Json;

namespace ISBEP.Situation
{
    public class RobotCamera : MonoBehaviour
    {
        [Tooltip("Serial number of the robot to idenfity the data.")]
        public string SerialNumber = "00000000";

        [Tooltip("Frame rate of the robot camera.")]
        public float frameRate = 1.0f;

        [Tooltip("Specify whether the robot should be straming their camera feed.")]
        public bool Streaming = true;

        [Tooltip("Camera rendering texture that can be used to send as camera data.")]
        public RenderTexture CameraTexture { get; private set; }

        private void Start()
        {
            CameraTexture = new RenderTexture(1920, 1080, 24);
            Camera camera = GetComponentInChildren<Camera>();
            if (camera != null)
            {
                camera.targetTexture = CameraTexture;
            }
            StartCoroutine(CaptureFrames());
        }

        private void OnDisable()
        {
            Streaming = false;
        }

        // Coroutine to capture frames at a specified rate
        private IEnumerator CaptureFrames()
        {
            while (Streaming)
            {
                // Capture and send the robot camera frame.
                WebSocketServer.Instance.BroadcastWebSocketMessage(GetRobotCameraFrame());

                // Wait for the specified frame rate before straming the next frame
                yield return new WaitForSeconds(1.0f / frameRate);
            }
        }

        string GetRobotCameraFrame()
        {
            Texture2D frame = CaptureFrame();
            byte[] frameBytes = frame.EncodeToPNG();
            string frameBase64 = Convert.ToBase64String(frameBytes);
            RobotCameraData robotCameraData = new RobotCameraData()
            {
                serial = SerialNumber,
                camera_image = frameBase64
            };
            return JsonConvert.SerializeObject(robotCameraData);
        }

        Texture2D CaptureFrame()
        {
            RenderTexture.active = CameraTexture;
            Texture2D texture = new Texture2D(CameraTexture.width, CameraTexture.height, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, CameraTexture.width, CameraTexture.height), 0, 0);
            texture.Apply();
            RenderTexture.active = null;
            return texture;
        }
    }

    [Serializable]
    public struct RobotCameraData
    {
        public string serial;
        public string camera_image;
    }
}