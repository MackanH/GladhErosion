using UnityEngine;

public class Erosion : MonoBehaviour
{
    [Header("Erosion Settings")]
    [SerializeField]
    private int lifetime = 50;
    [SerializeField]
    private float inertia = .05f;
    [SerializeField]
    private float erode_rate = .3f;
    [SerializeField]
    private float deposite_rate = .3f;
    [SerializeField]
    private float evaporation_rate = .01f;
    [SerializeField]
    private float min_volume = .01f;

    [Header("Droplet Settings")]
    [SerializeField]
    private float init_volume = 1f;
    [SerializeField]
    private float density = 25f;
    [SerializeField]
    private float friction = .05f;
    [SerializeField]
    private Vector2 init_speed = new Vector2(0f, 0f);

    public void Erode(float[,] h_map, int dim, int iterations = 1)
    {
        Debug.Log("Erosion started!");

        for (int i = 0; i < iterations; i++)
        {
            /* Init a droplet at a random position on the map  with an initial speed and volume */
            Vector2 position = new Vector2(Random.Range(1, dim - 1), Random.Range(1, dim - 1));

            Droplet drop = new Droplet(position, init_volume, init_speed);

            while(drop.volume >min_volume)
            {
                Vector2 initPos = drop.position;

                Vector2 normal = SurfaceNormal(h_map, (int)initPos.x, (int)initPos.y);

                drop.speed += 1.2f * normal / (drop.volume * density);

                drop.position += 1.2f * drop.speed;

                drop.speed *= (1f - (1.2f * friction));

                /* Check whether the drop has come to a stop or if it has fallen off the edge */
                if (Mathf.Abs(drop.speed.x) < Mathf.Epsilon ||
                     Mathf.Abs(drop.speed.y) < Mathf.Epsilon ||
                     (int)drop.position.x < 0 || (int)drop.position.x >= dim - 1 ||
                     (int)drop.position.y < 0 || (int)drop.position.y >= dim - 1)
                {

                    //Debug.Log("Drop outside bound: " + drop.position);
                    break;
                }

                float maxsediment = drop.volume* drop.speed.magnitude * (h_map[(int)initPos.x, (int)initPos.y]-h_map[(int)drop.position.x,(int)drop.position.y]);

                //if (maxsediment < 0.0f) maxsediment = 0.0f;
                
                float sdiff = maxsediment - drop.sediment;

                drop.sediment += 1.2f * deposite_rate * sdiff;

                //Debug.Log(1.2f * drop.volume * deposite_rate * sdiff);
                
                h_map[(int)initPos.x, (int)initPos.y] -= 1.2f * drop.volume * deposite_rate * sdiff;

                drop.volume *= (1.0f - (1.2f * evaporation_rate));

                //Debug.Log("Volume " + drop.volume);
            }
        }

        Debug.Log("Erosion done!");
    }

    Vector2 SurfaceNormal(float[,] heightmap, int x, int y)
    {
        Vector3 n = Vector3.Scale(new Vector3(0.15f, 0.15f, 0.15f), new Vector3(60 * (heightmap[x,y] - heightmap[x + 1,y]), 1.0f, 0.0f).normalized);  //Positive X
        n += Vector3.Scale(new Vector3(0.15f, 0.15f, 0.15f) , new Vector3(60 * (heightmap[x - 1,y] - heightmap[x,y]), 1.0f, 0.0f).normalized);        //Negative X
        n += Vector3.Scale(new Vector3(0.15f, 0.15f, 0.15f) , new Vector3(0.0f, 1.0f, 60 * (heightmap[x,y] - heightmap[x,y + 1])).normalized);        //Positive Y
        n += Vector3.Scale(new Vector3(0.15f, 0.15f, 0.15f) , new Vector3(0.0f, 1.0f, 60 * (heightmap[x,y - 1] - heightmap[x,y])).normalized);        //Negative Y

        //Diagonals! (This removes the last spatial artifacts)
        n += Vector3.Scale(new Vector3(0.1f, 0.1f, 0.1f) , new Vector3(60 * (heightmap[x,y] - heightmap[x + 1,y + 1]) / Mathf.Sqrt(2), Mathf.Sqrt(2), 60 * (heightmap[x,y] - heightmap[x + 1,y + 1]) / Mathf.Sqrt(2)).normalized);    //Positive Y
        n += Vector3.Scale(new Vector3(0.1f, 0.1f, 0.1f) , new Vector3(60 * (heightmap[x,y] - heightmap[x + 1,y - 1]) / Mathf.Sqrt(2), Mathf.Sqrt(2), 60 * (heightmap[x,y] - heightmap[x + 1,y - 1]) / Mathf.Sqrt(2)).normalized);    //Positive Y
        n += Vector3.Scale(new Vector3(0.1f, 0.1f, 0.1f) , new Vector3(60 * (heightmap[x,y] - heightmap[x - 1,y + 1]) / Mathf.Sqrt(2), Mathf.Sqrt(2), 60 * (heightmap[x,y] - heightmap[x - 1,y + 1]) / Mathf.Sqrt(2)).normalized);    //Positive Y
        n += Vector3.Scale(new Vector3(0.1f, 0.1f, 0.1f) , new Vector3(60 * (heightmap[x,y] - heightmap[x - 1,y - 1]) / Mathf.Sqrt(2), Mathf.Sqrt(2), 60 * (heightmap[x,y] - heightmap[x - 1,y - 1]) / Mathf.Sqrt(2)).normalized);    //Positive Y

        return n;
    }
}

public class Droplet
{
    public Droplet(Vector2 _position, float _volume, Vector2 _speed)
    {
        position = _position;
        volume = _volume;
        speed = _speed;
    }

    public Vector2 position;
    public Vector2 direction = new Vector2(.0f, .0f);

    public float volume;
    public Vector2 speed;
    public float sediment = 0;
}