using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UIElements;

public class Heatmap : MonoBehaviour
{
    /// <summary>
    /// Generate a heatmap texture for the specified values.
    /// </summary>
    /// <param name="width">Witdh in pixels that the texture should have.</param>
    /// <param name="height">Height in pixels that the texture should have.</param>
    /// <param name="scale">Scale for the perlin noise, about how many times the values go high low across the texture.</param>
    /// <returns></returns>
    public static Texture2D GenerateTexture(int width, int height, float scale) 
    {
        // The origin of the sampled area in the plane.
        float xOrg = Random.value;
        float yOrg = Random.value;

        // Set up the texture and a Color array to hold pixels during processing.
        Texture2D noiseTex = new Texture2D(width, height);
        Color[] pixels = new Color[noiseTex.width * noiseTex.height];

        // For each pixel in the texture...
        for (float y = 0.0F; y<noiseTex.height; y++)
        {
            for (float x = 0.0F; x<noiseTex.width; x++)
            {
                float xCoord = xOrg + x / noiseTex.width * scale;
                float yCoord = yOrg + y / noiseTex.height * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                pixels[(int)y * noiseTex.width + (int)x] = new Color(sample, sample, sample);
            }
        }

        // Copy the pixel data to the texture and load it into the GPU.
        noiseTex.SetPixels(pixels);
        noiseTex.Apply();

        return noiseTex;
    }

}