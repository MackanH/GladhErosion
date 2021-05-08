using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Erosion : MonoBehaviour
{
    [Header("Erosion Settings")]
    [SerializeField]
    private int lifetime = 50;
    [SerializeField]
    private float erode_rate = .3f;
    [SerializeField]
    private float deposite_rate = .3f;
    [SerializeField]
    private float evaporation_rate = .01f;
    [SerializeField]
    private float force = 4f;
    [Header("Droplet Settings")]
    [SerializeField]
    private float init_volume = 1f;
    [SerializeField]
    private float init_speed = 1f;



    public void Erode()
    {

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
