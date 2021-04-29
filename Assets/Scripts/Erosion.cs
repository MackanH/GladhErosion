using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Erosion : MonoBehaviour
{
    // Start is called before the first frame update

    float dt = 1.2f;

    float evapRate = 1e-3f;
    float depoRate = 1e-1f;
    float minVol = 0.01f;
    float friction = 5e-1f;


    public void Erode(float[] hMap)
    {

    }
}

class Droplet
{
    Droplet(Vector2 _pos)
    {
        pos = _pos;
    }

    Vector2 pos;
    Vector2 vel;

    float volume = 1.0f;
    float sediment = 0.0f;
}
