using UnityEngine;

public class HeightmapGenerator : MonoBehaviour
{
    /* Controls the amount of "detail" */
    [Range(0, 10)]
    public int numOct = 7;

    /* Offsets to generate different terrains */
    [SerializeField][Range(0,100)]
    public float offset_x = .0f;
    [SerializeField][Range(0,100)]
    public float offset_y = .0f;

    /* Controls the decreasing amp. of each octave */
    [SerializeField]
    [Range(.35f, 1f)]//[Range(.35f, .42f)]
    private float persistancy = .4f;

    /* Controls the increasing freq. of each octave */
    [SerializeField][Range(1.5f, 2.5f)]
    private float lacunarity = 2f;

    [SerializeField]
    private float init_freq = 1.5f;

    public float[,] height_map;
    public float[,] Generate(int size)
    {
        height_map = new float[size,size];

        float minV = 10f, maxV = -10f;

        /* Generate a height value for all points on the map between 0 and 1 */
        for (int z = 0; z < size; z++)
        {
            for (int x = 0; x < size; x++)
            {
                float str = 1f, freq = init_freq;

                float val = 0;

                /* For all octaves */
                for (int j = 0; j < numOct; j++)
                {
                    Vector2 pos =  new Vector2(x / (size - 1f), z / (size - 1f)) * freq;

                    val += Mathf.PerlinNoise(pos.x + offset_x, pos.y + offset_y) * str;

                    str *= persistancy;
                    freq *= lacunarity;
                }

                height_map[x, z] = val;

                if (val > maxV) maxV = val;
                if (val < minV) minV = val;
            }
        }
        /* Normalize the values of the height map */
        for(int z = 0; z < size; z++)
        {
            for (int x = 0; x < size; x++)
            {
                height_map[x, z] = (height_map[x, z] - minV) / (maxV - minV);
            }
        }

        return height_map;
    }
}
