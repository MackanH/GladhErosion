using System.Collections;
using System.Collections.Generic;
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
    private float sediment_capacity = 4f;
    [SerializeField]
    private float min_sediment = .01f;
    [SerializeField]
    private float gravity = 4f;
    
    [Header("Droplet Settings")]
    [SerializeField]
    private float init_volume = 1f;
    [SerializeField]
    private float init_speed = 1f;

    public void Erode(float[,] h_map, int dim, int iterations = 1)
    {
        for (int i = 0; i < iterations; i++)
        {
            /* Init a droplet at a random position on the map  with an initial speed and volume */
            Vector2 position = new Vector2(Random.Range(1, dim-1), Random.Range(1, dim-1));

            Droplet drop = new Droplet(position, init_volume, init_speed);

            /* For its maximum lifespan */
            for (int l = 0; l < lifetime; l++)
            {
                /* Calculate the offset between nearest node and drops position */
                Vector2Int node = new Vector2Int( (int)drop.position.x, (int)drop.position.y);

                float offsetX = drop.position.x - node.x;
                float offsetY = drop.position.y - node.y;

                /* Calculate fastest way down the hill using bilinear interpolation between the position and the corners
                 * of a node. Also determine the inital height of a drop. */
                Vector2 direction = CalculateGradient(h_map, drop.position);

                float height = CalculateHeight(h_map, drop.position);

                drop.direction = new Vector2(drop.direction.x * inertia - direction.x * (1 - inertia),
                                             drop.direction.y * inertia - direction.y * (1 - inertia)).normalized;

                /* Update the position one unit step */
                drop.position += drop.direction;

                /* Check whether the drop has come to a stop or if it has fallen off the edge */
                if ( Mathf.Abs(drop.direction.x) < Mathf.Epsilon ||
                     Mathf.Abs(drop.direction.y) < Mathf.Epsilon ||
                     drop.position.x < 0 || drop.position.x >= dim - 1 ||
                     drop.position.y < 0 || drop.position.y >= dim - 1)
                {
                    
                    //Debug.Log("Drop outside bound: " + drop.position);
                    break;
                }

                /* Calculate new height and determine the sediment amount. The amount should not
                 * exceed the difference in height! */
                float new_height = CalculateHeight(h_map, drop.position);

                float delta_height = new_height - height;

                float sediment_amount = Mathf.Max(-delta_height * sediment_capacity * drop.speed * drop.volume, min_sediment);

                /* If the drop has sediment or starts to travel upwards */
                if (drop.sediment > sediment_amount || delta_height > 0)
                {
                    float deposit_amount = (delta_height > 0) ? Mathf.Min(delta_height, drop.sediment) : 
                        (drop.sediment - sediment_amount) * deposite_rate;

                    //Debug.Log("Deposit amount: " + deposit_amount);
                    /* Remove the deposit amount from a drops sediment */
                    drop.sediment -= deposit_amount;
                    //Debug.Log("Drop sediment: " + drop.sediment);
                    /* Add the deposit to the map using bilinear interpolation */
                    h_map[node.x + 0, node.y + 0] += deposit_amount * (1 - offsetX) * (1 - offsetY);
                    h_map[node.x + 1, node.y + 0] += deposit_amount * offsetX * (1 - offsetY);
                    h_map[node.x + 0, node.y + 1] += deposit_amount * offsetY * (1 - offsetX);
                    h_map[node.x + 1, node.y + 1] += deposit_amount * offsetX * offsetY;
                }
                else
                {
                    float erode_amount = Mathf.Min((sediment_amount - drop.sediment) - erode_rate, -delta_height);
                    //Debug.Log("Erode amount: " + erode_amount);
                    h_map[node.x + 0, node.y + 0] -= erode_amount * (1 - offsetX) * (1 - offsetY);
                    h_map[node.x + 1, node.y + 0] -= erode_amount * offsetX * (1 - offsetY);
                    h_map[node.x + 0, node.y + 1] -= erode_amount * offsetY * (1 - offsetX);
                    h_map[node.x + 1, node.y + 1] -= erode_amount * offsetX * offsetY;
                    drop.sediment += erode_amount;
                }

                /* Update drops speed and volume */
                drop.speed = Mathf.Sqrt(drop.speed * drop.speed + delta_height * gravity);
                drop.volume *= (1 - evaporation_rate);
            }
        }
    }

    Vector2 CalculateGradient(float[,] map, Vector2 position)
    {
        Vector2Int node = new Vector2Int( (int)position.x, (int)position.y);

        float offsetX = position.x - node.x;
        float offsetY = position.y - node.y;

        /* (0,0)--(1,0)
         *  |       |
         *  |       |
         * (0,1)--(1,1)
         */
        float nodeNW = map[node.x + 0, node.y + 0];
        float nodeNE = map[node.x + 1, node.y + 0];
        float nodeSW = map[node.x + 0, node.y + 1]; 
        float nodeSE = map[node.x + 1, node.y + 1]; 

        float gradX = (nodeNE - nodeNW) * (1 - offsetY) + (nodeSE - nodeSW) * offsetY;
        float gradY = (nodeSW - nodeNW) * (1 - offsetX) + (nodeSE - nodeNE) * offsetX;

        return new Vector2(gradX, gradY);
    }

    float CalculateHeight(float[,] map, Vector2 position)
    {
        Vector2Int node = new Vector2Int((int)position.x, (int)position.y);

        float offsetX = position.x - node.x;
        float offsetY = position.y - node.y;

        /* (0,0)--(1,0)
         *  |       |
         *  |       |
         * (0,1)--(1,1)
         */
        float nodeNW = map[node.x + 0, node.y + 0];
        float nodeNE = map[node.x + 1, node.y + 0];
        float nodeSW = map[node.x + 0, node.y + 1];
        float nodeSE = map[node.x + 1, node.y + 1];

        return nodeNW * (1 - offsetX) * (1 - offsetY) + nodeNE * offsetX * (1 - offsetY) + 
            nodeSW * (1 - offsetX) * offsetY + nodeSE * offsetX * offsetY;
    }
}

public class Droplet
{
    public Droplet(Vector2 _position, float _volume, float _speed)
    {
        position = _position;
        volume = _volume;
        speed = _speed;
    }

    public Vector2 position;
    public Vector2 direction = new Vector2(.0f, .0f);

    public float volume;
    public float speed;
    public float sediment = 0;
}
