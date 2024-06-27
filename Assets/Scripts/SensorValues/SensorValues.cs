using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorValues : MonoBehaviour
{

    private SensorHeatmap[] sensorHeatmaps;
    public static SensorValues Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        sensorHeatmaps = GetComponents<SensorHeatmap>();
    }

    private void OnDisable()
    {
        Instance = null;
    }

    public Dictionary<string, float> GetSensorData(Vector3 position)
    {
        Dictionary<string, float> sensorData = new();
        foreach (SensorHeatmap sensorHeatmap in sensorHeatmaps)
        {
            sensorData.Add(sensorHeatmap.sensorKey, sensorHeatmap.GetSensorValue(position));
        }
        return sensorData;
    }
}
