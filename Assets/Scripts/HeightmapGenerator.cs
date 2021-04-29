using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightmapGenerator : MonoBehaviour
{
    [Range(0,50)]
    public int seed = 19;
    public int numOct = 7;

    /* Controls the decreasing amp. of each octave */
    [Range(.35f, .42f)]
    public float persistancy = .4f;

    /* Controls the increasing freq. of each octave */
    [Range(1.5f, 2.5f)]
    public float lacunarity = 2f;

    public float initScale = 1.5f;

    float[] hMap;
    public float[] Generate(int size)
    {
        hMap = new float[ (size+1) * (size + 1)];

        var PRNG = new System.Random(seed);

        Vector2[] offsets = new Vector2[numOct];
        for (int i = 0; i < numOct; i++)
            offsets[i] = new Vector2( PRNG.Next(0, 999), PRNG.Next(0, 999) );

        float minV = 10f, maxV = -10f;

        for (int i = 0, z = 0; z <= size; z++)
        {
            for (int x = 0; x <= size; x++)
            {
                float str = 1f, freq = initScale;

                float val = 0;

                for (int j = 0; j < numOct; j++)
                {
                    Vector2 pos = offsets[j] + new Vector2(x / (size - 1f),
                                                        z / (size - 1f)) * freq;

                    val += Mathf.PerlinNoise(pos.x, pos.y) * str;

                    str *= persistancy;
                    freq *= lacunarity;
                }

                hMap[i] = val;

                if (val > maxV) maxV = val;
                if (val < minV) minV = val;

                i++;
            }
        }

        for (int i = 0; i < hMap.Length; i++)
        {
            hMap[i] = (hMap[i] - minV) / (maxV - minV);
        }

        return hMap;
    }
}
