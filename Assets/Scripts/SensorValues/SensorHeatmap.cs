using UnityEngine;

/// <summary>
/// Represents a sensor heatmap for generating sensor data based on position.
/// </summary>
public class SensorHeatmap : MonoBehaviour
{
    [Tooltip("Key to identify the sensor type for the robot sensor data.")]
    public string sensorKey;

    [Tooltip("Texture representing the heatmap area where sensor values are sampled.")]
    public Texture2D heatmapTexture;

    [Tooltip("Scalar to adjust the sampled heatmap value. Can be used to scale the range of sensor values if necessary.")]
    public float scalar = 1.0f;

    private void Start()
    {
        if (heatmapTexture == null)
        {
            heatmapTexture = Heatmap.GenerateTexture(32, 32, 3.0f);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a wireframe rectangle representing the heatmap area
        Gizmos.color = Color.yellow;
        Vector3 center = transform.position;
        Vector3 size = new Vector3(transform.localScale.x, 0.0f, transform.localScale.z);
        Gizmos.DrawWireCube(center, size);
    }

    /// <summary>
    /// Get the sensor value for the given game scene position.
    /// </summary>
    /// <param name="position">position to get the sensor value for.</param>
    /// <returns>Sampled sensor value for the given position.</returns>
    public float GetSensorValue(Vector3 position)
    {
        Vector2 location = new ((position.x - transform.position.x) / 
                                transform.localScale.x * heatmapTexture.width,
                                (position.z - transform.position.z) / 
                                transform.localScale.z * heatmapTexture.height);
        return SampleHeatmap(heatmapTexture, location);
    }

    /// <summary>
    /// Samples the heatmap texture at the given position and returns the interpolated sensor value.
    /// </summary>
    /// <param name="heatmap">The heatmap texture to sample from.</param>
    /// <param name="position">The position in the heatmap to sample.</param>
    /// <returns>The interpolated sensor value based on the sampled heatmap.</returns>
    private float SampleHeatmap(Texture2D heatmap, Vector2 position)
    {
        int width = heatmap.width;
        int height = heatmap.height;

        // Find the nearest valid position within the heatmap bounds
        int x0 = Mathf.Clamp(Mathf.FloorToInt(position.x), 0, width - 1);
        int y0 = Mathf.Clamp(Mathf.FloorToInt(position.y), 0, height - 1);
        int x1 = Mathf.Clamp(x0 + 1, 0, width - 1);
        int y1 = Mathf.Clamp(y0 + 1, 0, height - 1);

        float fx = position.x - x0;
        float fy = position.y - y0;

        float c00 = heatmap.GetPixel(x0, y0).r;
        float c10 = heatmap.GetPixel(x1, y0).r;
        float c01 = heatmap.GetPixel(x0, y1).r;
        float c11 = heatmap.GetPixel(x1, y1).r;

        float top = Mathf.Lerp(c00, c10, fx);
        float bottom = Mathf.Lerp(c01, c11, fx);
        float value = Mathf.Lerp(top, bottom, fy);

        return value * scalar;
    }
}
