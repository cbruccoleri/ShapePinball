using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RipplePlaneControl : MonoBehaviour
{
    public static event Action<GameObject> OnDeactivate;

    private float _timerActive = 0.0f;

    private Renderer _renderer;


    private void Awake()
    {
        _renderer = gameObject.GetComponent<Renderer>();
        _renderer.material.mainTexture = GenerateNoiseTexture(256, 256, 6.0f);
        Random.InitState((int)(Time.realtimeSinceStartup * 1000.0f));
    }


    // Generate a Noise Texture at the given scale. This method is too slow for
    // real-time rendering, so use judiciously.
    Texture2D GenerateNoiseTexture(int width = 256, int height = 256, float scale = 1.0f)
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] colBuffer = new Color[width * height];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < width; y++) {
                // always sample Perlin Noise in (0, 1) because it repeats
                float xNorm = x / (float)width * scale;
                float yNorm = y / (float)height * scale;
                float lum = Mathf.Clamp01(Mathf.PerlinNoise(xNorm, yNorm));
                colBuffer[y * width + x] = new Color(lum, lum, lum, 1.0f);
                //texture.SetPixel(x, y, color);
            }
        }
        texture.SetPixels(colBuffer);
        // must Apply() to transfer data to the Texture in the GPU
        texture.Apply();
        return texture;
    }

    public void ActivateFromPool(float duration)
    {
        gameObject.SetActive(true);
        _timerActive = duration;
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        // notify listeners, if any
        OnDeactivate?.Invoke(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        _timerActive -= Time.deltaTime;
        if (_timerActive <= 0) {
            Deactivate();
        }
    }

}
