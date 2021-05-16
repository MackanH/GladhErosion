using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseTexure : MonoBehaviour
{
    public Transform target;

    private float[,] height_map;

    private int m_dimension = 256;

    private Renderer rend;

    private Color[] pix;

    private Texture2D noiseTex;

    private void Start()
    {
        rend = GetComponent<Renderer>();

        noiseTex = new Texture2D(m_dimension, m_dimension);

        pix = new Color[m_dimension * m_dimension];

        rend.material.mainTexture = noiseTex;

        height_map = FindObjectOfType<HeightmapGenerator>().Generate(m_dimension);
    }

    private void GetNoise(float[,] h_map)
    {
        height_map = FindObjectOfType<HeightmapGenerator>().Generate(m_dimension);

        for (int y = 0; y < noiseTex.height; y++)
        {
            for (int x = 0; x < noiseTex.width; x++)
            {
                pix[y * noiseTex.width + x] = new Color(h_map[x, y], h_map[x, y], h_map[x, y]);
            }
        }

        noiseTex.SetPixels(pix);
        noiseTex.Apply();
    }

    private void Update()
    {
        GetNoise(height_map);
    }
}
